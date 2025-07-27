using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using travel_agency_back.DTOs.Packages;
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
                // Cria LodgingInfo como entidades separadas
                var lodgingList = new List<LodgingInfo>
                {
                    new LodgingInfo { Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Av. Atlântica", Number = "1702", Neighborhood = "Copacabana", City = "Rio de Janeiro", State = "RJ", Country = "Brasil", ZipCode = "22021-001", Complement = "Apto 101" },
                    new LodgingInfo { Baths = 1, Beds = 3, WifiIncluded = true, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Av. Sete de Setembro", Number = "1000", Neighborhood = "Centro", City = "Salvador", State = "BA", Country = "Brasil", ZipCode = "40060-001", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = false, RestaurantOnSite = false, PetAllowed = true, AirConditioned = true, Breakfast = false, Street = "Rua das Gaivotas", Number = "500", Neighborhood = "Ingleses", City = "Florianópolis", State = "SC", Country = "Brasil", ZipCode = "88058-500", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 1, WifiIncluded = false, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = false, Breakfast = true, Street = "Estrada do Turismo", Number = "S/N", Neighborhood = "Tarumã", City = "Manaus", State = "AM", Country = "Brasil", ZipCode = "69041-010", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 3, WifiIncluded = false, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = true, AirConditioned = false, Breakfast = true, Street = "Estrada do Pantanal", Number = "S/N", Neighborhood = "Zona Rural", City = "Corumbá", State = "MS", Country = "Brasil", ZipCode = "79304-000", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Av. das Cataratas", Number = "6798", Neighborhood = "Vila Yolanda", City = "Foz do Iguaçu", State = "PR", Country = "Brasil", ZipCode = "85853-000", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = false, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = false, PetAllowed = true, AirConditioned = false, Breakfast = false, Street = "Rua do Vale", Number = "300", Neighborhood = "Lençóis", City = "Lençóis", State = "BA", Country = "Brasil", ZipCode = "46960-000", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = true, AirConditioned = true, Breakfast = true, Street = "Rua Pilád Rebuá", Number = "1835", Neighborhood = "Centro", City = "Bonito", State = "MS", Country = "Brasil", ZipCode = "79290-000", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Rua do Amparo", Number = "100", Neighborhood = "Amparo", City = "Olinda", State = "PE", Country = "Brasil", ZipCode = "53025-080", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 1, WifiIncluded = true, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Rue de Rivoli", Number = "45", Neighborhood = "1er Arrondissement", City = "Paris", State = "Île-de-France", Country = "França", ZipCode = "75001", Complement = "Quarto 12" },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "5th Avenue", Number = "700", Neighborhood = "Manhattan", City = "New York", State = "NY", Country = "EUA", ZipCode = "10019", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Avenida da Liberdade", Number = "250", Neighborhood = "Avenida", City = "Lisboa", State = "Lisboa", Country = "Portugal", ZipCode = "1250-147", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Avenida 9 de Julio", Number = "1000", Neighborhood = "Centro", City = "Buenos Aires", State = "CABA", Country = "Argentina", ZipCode = "1043", Complement = null },
                    new LodgingInfo { Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = false, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true, Street = "Avenida Libertador", Number = "1234", Neighborhood = "Providencia", City = "Santiago", State = "Región Metropolitana", Country = "Chile", ZipCode = "7500000", Complement = null }
                };
                dbContext.LodgingInfos.AddRange(lodgingList);
                await dbContext.SaveChangesAsync();

                var pacotes = new List<Packages>();
                for (int i = 0; i < lodgingList.Count; i++)
                {
                    pacotes.Add(new Packages
                    {
                        Name = $"Pacote {i + 1}",
                        Description = $"Descrição do pacote {i + 1}",
                        Price = 1000 + i * 100,
                        ImageUrl = $"https://example.com/image{i + 1}.jpg",
                        ActiveFrom = DateTime.Now,
                        ActiveUntil = DateTime.Now.AddMonths(2),
                        BeginDate = DateTime.Now.AddDays(10 + i),
                        EndDate = DateTime.Now.AddDays(17 + i),
                        Origin = "Origem",
                        Destination = "Destino",
                        Quantity = 10 + i,
                        CreatedAt = DateTime.Now,
                        IsAvailable = true,
                        DiscountPercent = 5 + i,
                        LodgingInfoId = lodgingList[i].Id
                    });
                }
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