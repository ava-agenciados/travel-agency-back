using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using travel_agency_back.Data;
using travel_agency_back.Models;
using travel_agency_back.Services;
using travel_agency_back.Utils;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controladores MVC à aplicação, permitindo o uso de controllers e rotas de API.
builder.Services.AddControllers();

// Adiciona suporte à documentação automática de endpoints (Swagger/OpenAPI).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o contexto do Entity Framework Core para usar SQL Server, 
// lendo a string de conexão do arquivo de configuração (appsettings.json).
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Substitui o hasher padrão do Identity por uma implementação baseada em BCrypt,
// garantindo que as senhas dos usuários sejam armazenadas usando o algoritmo BCrypt.
builder.Services.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher<User>>();

// Configura o ASP.NET Core Identity para usar a entidade User personalizada e roles (IdentityRole).
// Define que não é necessário confirmar a conta por e-mail para autenticação.
// Usa o ApplicationDBContext para persistência e adiciona provedores de token padrão.
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

// Registra o AuthService para injeção de dependência, permitindo que seja utilizado nos controllers.
builder.Services.AddScoped<AuthService>();

// Constrói a aplicação web com as configurações e serviços definidos acima.
var app = builder.Build();

// Configura o middleware de documentação Swagger apenas em ambiente de desenvolvimento,
// facilitando o teste e visualização dos endpoints da API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adiciona middleware para redirecionar todas as requisições HTTP para HTTPS, aumentando a segurança.
app.UseHttpsRedirection();

// Adiciona o middleware de autenticação, necessário para identificar usuários autenticados.
app.UseAuthentication();

// Adiciona o middleware de autorização, responsável por validar permissões de acesso a recursos protegidos.
app.UseAuthorization();

// Mapeia os controllers para as rotas configuradas, ativando os endpoints da API.
app.MapControllers();

// Inicia a aplicação e começa a escutar requisições HTTP.
app.Run();
