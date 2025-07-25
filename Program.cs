using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens; // Adicionado para JWT
using Microsoft.OpenApi.Models; // Necess�rio para configura��o do Swagger com JWT
using System.Text; // Adicionado para JWT
using travel_agency_back.Data;
using travel_agency_back.Models;
using travel_agency_back.Repositories;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Utils;

var builder = WebApplication.CreateBuilder(args);


// Adiciona os controladores MVC � aplica��o, permitindo o uso de controllers e rotas de API.
builder.Services.AddControllers();

// Adiciona suporte � documenta��o autom�tica de endpoints (Swagger/OpenAPI) e configura autentica��o JWT no Swagger
builder.Services.AddEndpointsApiExplorer();

// Configura��o de pol�tica de CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Permite requisi��es apenas do frontend, j� configurado na porta 3000
              .AllowAnyHeader() // Permite todos os cabe�alhos HTTP
              .AllowAnyMethod() // Permite todos os m�todos HTTP
              .AllowCredentials(); // Importante para cookies/autentica��o
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

// Configura o contexto do Entity Framework Core para usar SQL Server, lendo a string de conex�o do arquivo de configura��o (appsettings.json).
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Substitui o hasher padr�o do Identity por uma implementa��o baseada em BCrypt, garantindo que as senhas dos usu�rios sejam armazenadas usando o algoritmo BCrypt.
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
// Define que n�o � necess�rio confirmar a conta por e-mail para autentica��o.
// Usa o ApplicationDBContext para persist�ncia e adiciona provedores de token padr�o.
builder.Services.AddIdentity<User, IdentityRole<int>>(options =>
{
    options.SignIn.RequireConfirmedAccount = false; // N�o exige confirma��o de e-mail para login
})
.AddEntityFrameworkStores<ApplicationDBContext>() // Usa o ApplicationDBContext para armazenar usu�rios
.AddDefaultTokenProviders(); // Adiciona provedores de token padr�o (reset de senha, etc)

// ================= JWT CONFIGURATION =================
// Adiciona a autentica��o JWT � aplica��o.
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "JwtBearer"; // Define o esquema padr�o de autentica��o como JWT
    options.DefaultChallengeScheme = "JwtBearer";    // Define o esquema padr�o de desafio como JWT
})
.AddJwtBearer("JwtBearer", options =>
{
    // Chave secreta para assinar o token (deve ser forte e protegida em produ��o)
    var key = builder.Configuration["Jwt:Key"] ?? "x7$AqZ!mF2v9@TgLb0#R6eWpYuNcKj3B"; // Busca a chave secreta do appsettings.json
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Valida o emissor do token (quem gerou)
        ValidateAudience = true, // Valida o p�blico do token (para quem foi gerado)
        ValidateLifetime = true, // Valida se o token est� expirado
        ValidateIssuerSigningKey = true, // Valida a assinatura do token
        ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "travel-agency-api", // Emissor esperado do token JWT
        ValidAudience = builder.Configuration["Jwt:Audience"] ?? "travel-agency-client", // P�blico esperado do token JWT
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), // Chave secreta usada para validar a assinatura do token
        ClockSkew = TimeSpan.Zero // Sem toler�ncia para expira��o (expira exatamente no hor�rio)
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

// Adiciona CORS para permitir cookies entre dom�nios
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins("http://localhost:3000") // Altere para o dom�nio do seu front-end
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

// Constr�i a aplica��o web com as configura��es e servi�os definidos acima.
var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    await SeedData.EnsureRolesAndUsersAsync(scope.ServiceProvider);
}

// Configura o middleware de documenta��o Swagger apenas em ambiente de desenvolvimento, facilitando o teste e visualiza��o dos endpoints da API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger(); // Ativa o Swagger para documenta��o da API
    app.UseSwaggerUI(); // Ativa a interface do Swagger
}


app.UseStaticFiles(); // Permite servir arquivos est�ticos (como imagens, CSS, JS) da pasta wwwroot

// Adiciona middleware para redirecionar todas as requisi��es HTTP para HTTPS, aumentando a seguran�a.
app.UseHttpsRedirection(); // Redireciona HTTP para HTTPS
app.UseCors(); // Adiciona o middleware CORS antes de autentica��o

app.UseCors("AllowReactApp");

// Adiciona o middleware de autentica��o, necess�rio para identificar usu�rios autenticados.
app.UseAuthentication(); // Habilita autentica��o JWT na pipeline

// Adiciona o middleware de autoriza��o, respons�vel por validar permiss�es de acesso a recursos protegidos.
app.UseAuthorization(); // Habilita autoriza��o baseada em pol�ticas e roles

// Mapeia os controllers para as rotas configuradas, ativando os endpoints da API.
app.MapControllers(); // Mapeia os controllers para as rotas

// Inicia a aplica��o e come�a a escutar requisi��es HTTP.
app.Run(); // Inicia o servidor web

