using Microsoft.AspNetCore.Identity;
using travel_agency_back.Models;

namespace travel_agency_back.Data
{
    public static class SeedData
    {
        public static async Task EnsureRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            string[] roles = { "Admin", "Cliente", "Atendente" };
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
            }

            // Usuário Admin
            var adminEmail = "admin@horizon.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    FirstName = "Admin",
                    LastName = "Master",
                    PhoneNumber = "9876543210",
                    PasswordHash = "Admin@123",
                    CPFPassport = "00000000000",
                    Role = "Admin"
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
            }

            // Usuário Cliente
            var clienteEmail = "cliente@horizon.com";
            if (await userManager.FindByEmailAsync(clienteEmail) == null)
            {
                var cliente = new User
                {
                    UserName = clienteEmail,
                    Email = clienteEmail,
                    FirstName = "Cliente",
                    LastName = "Teste",
                    PhoneNumber = "1234567890",
                    PasswordHash = "Cliente@123",
                    CPFPassport = "11111111111",
                    Role = "Cliente"
                };
                await userManager.CreateAsync(cliente, "Cliente@123");
                await userManager.AddToRoleAsync(cliente, "Cliente");
            }

            // Usuário Funcionario
            var funcionarioEmail = "atendente@horizon.com";
            if (await userManager.FindByEmailAsync(funcionarioEmail) == null)
            {
                var funcionario = new User
                {
                    UserName = funcionarioEmail,
                    Email = funcionarioEmail,
                    FirstName = "Atendente",
                    LastName = "Atendente",
                    PhoneNumber = "1122334455",
                    PasswordHash = "Atendente@123",
                    CPFPassport = "22222222222",
                    Role = "Atendente"
                };
                await userManager.CreateAsync(funcionario, "Atendente@123");
                await userManager.AddToRoleAsync(funcionario, "Atendente");
            }
        }
    }
}