using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using travel_agency_back.Data;
using travel_agency_back.Models;
using travel_agency_back.Services;
using travel_agency_back.Utils;

var builder = WebApplication.CreateBuilder(args);

// Adiciona os controladores MVC � aplica��o, permitindo o uso de controllers e rotas de API.
builder.Services.AddControllers();

// Adiciona suporte � documenta��o autom�tica de endpoints (Swagger/OpenAPI).
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configura o contexto do Entity Framework Core para usar SQL Server, 
// lendo a string de conex�o do arquivo de configura��o (appsettings.json).
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Substitui o hasher padr�o do Identity por uma implementa��o baseada em BCrypt,
// garantindo que as senhas dos usu�rios sejam armazenadas usando o algoritmo BCrypt.
builder.Services.AddScoped<IPasswordHasher<User>, BcryptPasswordHasher<User>>();

// Configura o ASP.NET Core Identity para usar a entidade User personalizada e roles (IdentityRole).
// Define que n�o � necess�rio confirmar a conta por e-mail para autentica��o.
// Usa o ApplicationDBContext para persist�ncia e adiciona provedores de token padr�o.
builder.Services.AddIdentity<User, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDBContext>()
.AddDefaultTokenProviders();

// Registra o AuthService para inje��o de depend�ncia, permitindo que seja utilizado nos controllers.
builder.Services.AddScoped<AuthService>();

// Constr�i a aplica��o web com as configura��es e servi�os definidos acima.
var app = builder.Build();

// Configura o middleware de documenta��o Swagger apenas em ambiente de desenvolvimento,
// facilitando o teste e visualiza��o dos endpoints da API.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Adiciona middleware para redirecionar todas as requisi��es HTTP para HTTPS, aumentando a seguran�a.
app.UseHttpsRedirection();

// Adiciona o middleware de autentica��o, necess�rio para identificar usu�rios autenticados.
app.UseAuthentication();

// Adiciona o middleware de autoriza��o, respons�vel por validar permiss�es de acesso a recursos protegidos.
app.UseAuthorization();

// Mapeia os controllers para as rotas configuradas, ativando os endpoints da API.
app.MapControllers();

// Inicia a aplica��o e come�a a escutar requisi��es HTTP.
app.Run();
