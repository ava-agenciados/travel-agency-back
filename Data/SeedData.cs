using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using travel_agency_back.Models;

namespace travel_agency_back.Data
{
    public static class SeedData
    {
        public static async Task EnsureRolesAndUsersAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var dbContext = serviceProvider.GetRequiredService<ApplicationDBContext>();

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
                    LastName = "Horizon",
                    PhoneNumber = "9876543210",
                    PasswordHash = "Admin@123",
                    CPFPassport = "00000000000",
                    Role = "Admin"
                };
                await userManager.CreateAsync(admin, "Admin@123");
                await userManager.AddToRoleAsync(admin, "Admin");
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
                    LastName = "Horizon",
                    PhoneNumber = "1122334455",
                    PasswordHash = "Atendente@123",
                    CPFPassport = "22222222222",
                    Role = "Atendente"
                };
                await userManager.CreateAsync(funcionario, "Atendente@123");
                await userManager.AddToRoleAsync(funcionario, "Atendente");
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
                    LastName = "Horizon",
                    PhoneNumber = "1234567890",
                    PasswordHash = "Cliente@123",
                    CPFPassport = "11111111111",
                    Role = "Cliente"
                };
                await userManager.CreateAsync(cliente, "Cliente@123");
                await userManager.AddToRoleAsync(cliente, "Cliente");

            }

            // Adiciona múltiplos clientes fictícios (brasileiros e estrangeiros)
            var clientes = new List<User>
            {
                // Brasileiros
                new User { UserName = "ana.silva@gmail.com", Email = "ana.silva@gmail.com", FirstName = "Ana", LastName = "Silva", PhoneNumber = "11999990001", CPFPassport = "11111111112", Role = "Cliente" },
                new User { UserName = "carlos.souza@outlook.com", Email = "carlos.souza@outlook.com", FirstName = "Carlos", LastName = "Souza", PhoneNumber = "21999990002", CPFPassport = "11111111113", Role = "Cliente" },
                new User { UserName = "mariana.lima@yahoo.com", Email = "mariana.lima@yahoo.com", FirstName = "Mariana", LastName = "Lima", PhoneNumber = "31999990003", CPFPassport = "11111111114", Role = "Cliente" },
                new User { UserName = "joao.pereira@gmail.com", Email = "joao.pereira@gmail.com", FirstName = "João", LastName = "Pereira", PhoneNumber = "41999990004", CPFPassport = "11111111115", Role = "Cliente" },
                new User { UserName = "fernanda.alves@outlook.com", Email = "fernanda.alves@outlook.com", FirstName = "Fernanda", LastName = "Alves", PhoneNumber = "51999990005", CPFPassport = "11111111116", Role = "Cliente" },
                new User { UserName = "lucas.gomes@yahoo.com", Email = "lucas.gomes@yahoo.com", FirstName = "Lucas", LastName = "Gomes", PhoneNumber = "61999990006", CPFPassport = "11111111117", Role = "Cliente" },
                new User { UserName = "juliana.martins@gmail.com", Email = "juliana.martins@gmail.com", FirstName = "Juliana", LastName = "Martins", PhoneNumber = "71999990007", CPFPassport = "11111111118", Role = "Cliente" },
                new User { UserName = "rafael.oliveira@outlook.com", Email = "rafael.oliveira@outlook.com", FirstName = "Rafael", LastName = "Oliveira", PhoneNumber = "81999990008", CPFPassport = "11111111119", Role = "Cliente" },

                // Estrangeiros
                new User { UserName = "john.smith@gmail.com", Email = "john.smith@gmail.com", FirstName = "John", LastName = "Smith", PhoneNumber = "99999990009", CPFPassport = "21111111110", Role = "Cliente" },
                new User { UserName = "emma.johnson@outlook.com", Email = "emma.johnson@outlook.com", FirstName = "Emma", LastName = "Johnson", PhoneNumber = "99999990010", CPFPassport = "21111111111", Role = "Cliente" },
                new User { UserName = "liam.brown@yahoo.com", Email = "liam.brown@yahoo.com", FirstName = "Liam", LastName = "Brown", PhoneNumber = "99999990012", CPFPassport = "21111111112", Role = "Cliente" },
                new User { UserName = "olivia.wilson@gmail.com", Email = "olivia.wilson@gmail.com", FirstName = "Olivia", LastName = "Wilson", PhoneNumber = "99999990013", CPFPassport = "21111111113", Role = "Cliente" },
                new User { UserName = "noah.taylor@outlook.com", Email = "noah.taylor@outlook.com", FirstName = "Noah", LastName = "Taylor", PhoneNumber = "99999990014", CPFPassport = "21111111114", Role = "Cliente" },
                new User { UserName = "mia.moore@yahoo.com", Email = "mia.moore@yahoo.com", FirstName = "Mia", LastName = "Moore", PhoneNumber = "99999990015", CPFPassport = "21111111115", Role = "Cliente" },
                new User { UserName = "lucas.martin@gmail.com", Email = "lucas.martin@gmail.com", FirstName = "Lucas", LastName = "Martin", PhoneNumber = "99999990016", CPFPassport = "21111111116", Role = "Cliente" }
            };

            foreach (var cliente in clientes)
            {
                if (await userManager.FindByEmailAsync(cliente.Email) == null)
                {
                    await userManager.CreateAsync(cliente, "Cliente@123");
                    await userManager.AddToRoleAsync(cliente, "Cliente");
                }
            }

            // Adiciona pacotes de exemplo se não existirem
            if (!await dbContext.Packages.AnyAsync())
            {
                var pacotes = new List<Packages>
                {
                    // Nacionais
                    new Packages { Name = "Pacote Verão", Description = "Viagem para o Rio de Janeiro", Price = 1500, ImageUrl = "https://images.pexels.com/photos/351283/pexels-photo-351283.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(2), BeginDate = DateTime.Now.AddDays(10), EndDate = DateTime.Now.AddDays(17), Origin = "São Paulo", Destination = "Rio de Janeiro", Quantity = 20, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Carnaval", Description = "Viagem para Salvador", Price = 2000, ImageUrl = "https://images.pexels.com/photos/3700900/pexels-photo-3700900.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(3), BeginDate = DateTime.Now.AddDays(20), EndDate = DateTime.Now.AddDays(27), Origin = "Belo Horizonte", Destination = "Salvador", Quantity = 15, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Praia", Description = "Viagem para Florianópolis", Price = 1800, ImageUrl = "https://images.pexels.com/photos/164631/pexels-photo-164631.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(1), BeginDate = DateTime.Now.AddDays(5), EndDate = DateTime.Now.AddDays(12), Origin = "Curitiba", Destination = "Florianópolis", Quantity = 10, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Amazônia", Description = "Exploração na Floresta Amazônica", Price = 2500, ImageUrl = "https://images.pexels.com/photos/2739664/pexels-photo-2739664.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(4), BeginDate = DateTime.Now.AddDays(15), EndDate = DateTime.Now.AddDays(22), Origin = "Manaus", Destination = "Amazônia", Quantity = 8, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Pantanal", Description = "Ecoturismo no Pantanal", Price = 2200, ImageUrl = "https://images.pexels.com/photos/221436/pexels-photo-221436.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(2), BeginDate = DateTime.Now.AddDays(18), EndDate = DateTime.Now.AddDays(25), Origin = "Campo Grande", Destination = "Pantanal", Quantity = 12, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Foz do Iguaçu", Description = "Visita às Cataratas do Iguaçu", Price = 1700, ImageUrl = "https://images.pexels.com/photos/1232766/pexels-photo-1232766.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(2), BeginDate = DateTime.Now.AddDays(8), EndDate = DateTime.Now.AddDays(15), Origin = "Curitiba", Destination = "Foz do Iguaçu", Quantity = 14, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Chapada Diamantina", Description = "Aventura na Chapada Diamantina", Price = 2100, ImageUrl = "https://images.pexels.com/photos/21047000/pexels-photo-21047000.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(3), BeginDate = DateTime.Now.AddDays(12), EndDate = DateTime.Now.AddDays(19), Origin = "Salvador", Destination = "Chapada Diamantina", Quantity = 9, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Lençóis Maranhenses", Description = "Passeio pelos Lençóis Maranhenses", Price = 2300, ImageUrl = "https://images.pexels.com/photos/19151869/pexels-photo-19151869.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(2), BeginDate = DateTime.Now.AddDays(14), EndDate = DateTime.Now.AddDays(21), Origin = "São Luís", Destination = "Lençóis Maranhenses", Quantity = 11, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Bonito", Description = "Ecoturismo em Bonito", Price = 1950, ImageUrl = "https://images.pexels.com/photos/10797273/pexels-photo-10797273.png", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(2), BeginDate = DateTime.Now.AddDays(9), EndDate = DateTime.Now.AddDays(16), Origin = "Campo Grande", Destination = "Bonito", Quantity = 13, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Recife e Olinda", Description = "Carnaval em Recife e Olinda", Price = 1600, ImageUrl = "https://images.pexels.com/photos/20538562/pexels-photo-20538562.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(2), BeginDate = DateTime.Now.AddDays(11), EndDate = DateTime.Now.AddDays(18), Origin = "Recife", Destination = "Olinda", Quantity = 16, CreatedAt = DateTime.Now, IsAvailable = true },

                    // Internacionais
                    new Packages { Name = "Pacote Paris", Description = "Viagem romântica para Paris", Price = 7000, ImageUrl = "https://images.pexels.com/photos/1308940/pexels-photo-1308940.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(6), BeginDate = DateTime.Now.AddDays(30), EndDate = DateTime.Now.AddDays(37), Origin = "São Paulo", Destination = "Paris", Quantity = 6, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Nova York", Description = "Turismo em Nova York", Price = 8000, ImageUrl = "https://images.pexels.com/photos/466685/pexels-photo-466685.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(6), BeginDate = DateTime.Now.AddDays(40), EndDate = DateTime.Now.AddDays(47), Origin = "Rio de Janeiro", Destination = "Nova York", Quantity = 7, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Lisboa", Description = "Conheça a capital portuguesa", Price = 6500, ImageUrl = "https://images.pexels.com/photos/33145898/pexels-photo-33145898.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(5), BeginDate = DateTime.Now.AddDays(35), EndDate = DateTime.Now.AddDays(42), Origin = "Salvador", Destination = "Lisboa", Quantity = 8, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Buenos Aires", Description = "Passeio cultural em Buenos Aires", Price = 4000, ImageUrl = "https://images.pexels.com/photos/16228260/pexels-photo-16228260.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(4), BeginDate = DateTime.Now.AddDays(25), EndDate = DateTime.Now.AddDays(32), Origin = "Porto Alegre", Destination = "Buenos Aires", Quantity = 10, CreatedAt = DateTime.Now, IsAvailable = true },
                    new Packages { Name = "Pacote Santiago", Description = "Viagem para Santiago do Chile", Price = 4200, ImageUrl = "https://images.pexels.com/photos/2017747/pexels-photo-2017747.jpeg", ActiveFrom = DateTime.Now, ActiveUntil = DateTime.Now.AddMonths(4), BeginDate = DateTime.Now.AddDays(28), EndDate = DateTime.Now.AddDays(35), Origin = "São Paulo", Destination = "Santiago", Quantity = 9, CreatedAt = DateTime.Now, IsAvailable = true }
                };
                dbContext.Packages.AddRange(pacotes);
                await dbContext.SaveChangesAsync();
            }

            // Adiciona uma imagem em PackageMedia para cada pacote usando o ImageUrl do pacote
            var pacotesSalvos = await dbContext.Packages.ToListAsync();
            var packageMediaList = new List<PackageMedia>();
            foreach (var pacote in pacotesSalvos)
            {
                // Verifica se já existe uma mídia para esse pacote com o mesmo link
                bool exists = await dbContext.PackageMedia.AnyAsync(pm => pm.PackageId == pacote.Id && pm.ImageURL == pacote.ImageUrl);
                if (!exists)
                {
                    packageMediaList.Add(new PackageMedia
                    {
                        PackageId = pacote.Id,
                        ImageURL = pacote.ImageUrl,
                        MediaType = 1, // 1 para imagem
                        CreatedAt = DateTime.Now
                    });
                }
            }
            if (packageMediaList.Count > 0)
            {
                dbContext.PackageMedia.AddRange(packageMediaList);
                await dbContext.SaveChangesAsync();
            }

            // Adiciona Ratings (avaliações) para os pacotes com base nos clientes
            var clientesSalvos = await dbContext.Users.Where(u => u.Role == "Cliente").ToListAsync();
            var pacotesParaAvaliar = await dbContext.Packages.ToListAsync();
            var comentarios = new List<string>
            {
                "Experiência incrível, recomendo muito!",
                "O atendimento foi excelente e o destino maravilhoso.",
                "Gostei bastante, mas poderia ter mais opções de passeios.",
                "Viagem tranquila, tudo conforme o combinado.",
                "O hotel era confortável e bem localizado.",
                "Achei o preço um pouco alto, mas valeu a pena.",
                "Amei cada momento, voltaria com certeza!",
                "O guia turístico era muito atencioso.",
                "Tive alguns problemas com o transporte, mas foram resolvidos.",
                "Paisagens lindas e ótima organização.",
                "A comida local foi uma surpresa positiva!",
                "O pacote superou minhas expectativas.",
                "Viagem razoável, esperava mais do roteiro.",
                "Tudo perfeito, sem nenhum contratempo.",
                "Equipe muito prestativa e simpática.",
                "Não gostei do hotel, mas o passeio compensou.",
                "O clima ajudou bastante, aproveitamos muito.",
                "Faltou organização em alguns momentos.",
                "O transfer foi pontual e confortável.",
                "Voltaria a fechar com a agência!"
            };
            var random = new Random();
            var ratings = new List<Rating>();
            int comentarioIndex = 0;
            foreach (var pacote in pacotesParaAvaliar)
            {
                var clientesParaEstePacote = clientesSalvos.OrderBy(x => random.Next()).Take(5).ToList();
                var notasUsadas = new HashSet<int>();
                foreach (var cliente in clientesParaEstePacote)
                {
                    int nota;
                    do { nota = random.Next(1, 6); } while (!notasUsadas.Add(nota) && notasUsadas.Count < 5);
                    ratings.Add(new Rating
                    {
                        UserId = cliente.Id,
                        PackageId = pacote.Id,
                        Stars = nota,
                        Comment = comentarios[comentarioIndex % comentarios.Count],
                        IsAvailable = true,
                        CreatedAt = DateTime.Now
                    });
                    comentarioIndex++;
                }
            }
            if (ratings.Count > 0)
            {
                dbContext.Rating.AddRange(ratings);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}