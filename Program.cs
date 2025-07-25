using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens; // Adicionado para JWT
using Microsoft.OpenApi.Models; // Necessário para configuração do Swagger com JWT
using System.Text; // Adicionado para JWT
using travel_agency_back.Data;
using travel_agency_back.Models;
using travel_agency_back.Repositories;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Utils;

var builder = WebApplication.CreateBuilder(args);


// Adiciona os controladores MVC à aplicação, permitindo o uso de controllers e rotas de API.
builder.Services.AddControllers();

// Adiciona suporte à documentação automática de endpoints (Swagger/OpenAPI) e configura autenticação JWT no Swagger
builder.Services.AddEndpointsApiExplorer();

// Configuração de política de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Permite requisições apenas do frontend, já configurado na porta 3000
              .AllowAnyHeader() // Permite todos os cabeçalhos HTTP
              .AllowAnyMethod() // Permite todos os métodos HTTP
              .AllowCredentials(); // Importante para cookies/autenticação
    });
});

builder.Services.AddSwaggerGen(options =>
{
    // Configura o Swagger para aceitar JWT Bearer
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Insira o token JWT no formato: Bearer {seu token}"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

// Configura o contexto do Entity Framework Core para usar SQL Server, lendo a string de conexão do arquivo de configuração (appsettings.json).
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Substitui o hasher padrão do Identity por uma implementação baseada em BCrypt, garantindo que as senhas dos usuários sejam armazenadas usando o algoritmo BCrypt.
builder.Services.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher<User>>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IPackageService, PackageService>();
builder.Services.AddScoped<IPackageRepository, PackageRepository>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<AuthService>(); // Registra o AuthService para ser injetado nos controllers
builder.Services.AddScoped<IBookingRepository, BookingRepository>();
builder.Services.AddScoped<IMediaService, MediaService>();

// Configura o ASP.NET Core Identity para usar a entidade User personalizada e roles (IdentityRole).
// Define que não é necessário confirmar a conta por e-mail para autenticação.
// Usa o ApplicationDBContext para persistência e adiciona provedores de token padrão.
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // Não exige confirmação de e-mail para login
})
.AddEntityFrameworkStores<ApplicationDBContext>() // Usa o ApplicationDBContext para armazenar usuários
.AddDefaultTokenProviders(); // Adiciona provedores de token padrão (reset de senha, etc)

// ================= JWT CONFIGURATION =================
// Adiciona a autenticação JWT à aplicação.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer"; // Define o esquema padrão de autenticação como JWT
    options.DefaultChallengeScheme = "JwtBearer";    // Define o esquema padrão de desafio como JWT
})
.AddJwtBearer("JwtBearer", options =>
{
    // Chave secreta para assinar o token (deve ser forte e protegida em produção)
    var key = builder.Configuration["Jwt:Key"] ?? "x7$AqZ!mF2v9@TgLb0#R6eWpYuNcKj3B"; // Busca a chave secreta do appsettings.json
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida o emissor do token (quem gerou)
        ValidateAudience = true, // Valida o público do token (para quem foi gerado)
        ValidateLifetime = true, // Valida se o token está expirado
        ValidateIssuerSigningKey = true, // Valida a assinatura do token
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "travel-agency-api", // Emissor esperado do token JWT
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "travel-agency-client", // Público esperado do token JWT
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), // Chave secreta usada para validar a assinatura do token
        ClockSkew = TimeSpan.Zero // Sem tolerância para expiração (expira exatamente no horário)
    };
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Troque "SeuCookieJwt" pelo nome real do seu cookie
            var token = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        }
    };
});
// =====================================================

builder.Services.AddSwaggerGen(options =>
{
    options.EnableAnnotations(); // <-- Adicione esta linha
                               
});

// Adiciona CORS para permitir cookies entre domínios
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Altere para o domínio do seu front-end
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Constrói a aplicação web com as configurações e serviços definidos acima.
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    await SeedData.EnsureRolesAndUsersAsync(scope.ServiceProvider);
}

// Configura o middleware de documentação Swagger apenas em ambiente de desenvolvimento, facilitando o teste e visualização dos endpoints da API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Ativa o Swagger para documentação da API
    app.UseSwaggerUI(); // Ativa a interface do Swagger
}


app.UseStaticFiles(); // Permite servir arquivos estáticos (como imagens, CSS, JS) da pasta wwwroot

// Adiciona middleware para redirecionar todas as requisições HTTP para HTTPS, aumentando a segurança.
app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
app.UseCors(); // Adiciona o middleware CORS antes de autenticação

app.UseCors("AllowReactApp");

// Adiciona o middleware de autenticação, necessário para identificar usuários autenticados.
app.UseAuthentication(); // Habilita autenticação JWT na pipeline

// Adiciona o middleware de autorização, responsável por validar permissões de acesso a recursos protegidos.
app.UseAuthorization(); // Habilita autorização baseada em políticas e roles

// Mapeia os controllers para as rotas configuradas, ativando os endpoints da API.
app.MapControllers(); // Mapeia os controllers para as rotas

// Inicia a aplicação e começa a escutar requisições HTTP.
app.Run(); // Inicia o servidor web

