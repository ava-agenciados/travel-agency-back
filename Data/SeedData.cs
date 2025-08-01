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
                new User { UserName = "mariana.limaYahoo.com", Email = "mariana.limaYahoo.com", FirstName = "Mariana", LastName = "Lima", PhoneNumber = "31999990003", CPFPassport = "11111111114", Role = "Cliente" },
                new User { UserName = "joao.pereira@gmail.com", Email = "joao.pereira@gmail.com", FirstName = "João", LastName = "Pereira", PhoneNumber = "41999990004", CPFPassport = "11111111115", Role = "Cliente" },
                new User { UserName = "fernanda.alves@outlook.com", Email = "fernanda.alves@outlook.com", FirstName = "Fernanda", LastName = "Alves", PhoneNumber = "51999990005", CPFPassport = "11111111116", Role = "Cliente" },
                new User { UserName = "lucas.gomes.yahoo.com", Email = "lucas.gomes.yahoo.com", FirstName = "Lucas", LastName = "Gomes", PhoneNumber = "61999990006", CPFPassport = "11111111117", Role = "Cliente" },
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
                // Cria LodgingInfo premium e realista para cada destino de pacote
                var lodgingList = new List<LodgingInfo>
                {
                    // NACIONAIS
                    new LodgingInfo { // Cataratas do Iguaçu (Foz do Iguaçu)
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Av. das Cataratas", Number = "2420", Neighborhood = "Vila Yolanda", City = "Foz do Iguaçu", State = "PR", Country = "Brasil", ZipCode = "85853-000", Complement = "Hotel 5 estrelas, vista para as Cataratas"
                    },
                    new LodgingInfo { // Maragogi
                        Baths = 2, Beds = 3, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = true, AirConditioned = true, Breakfast = true,
                        Street = "Rod. AL-101 Norte", Number = "km 124", Neighborhood = "Praia de Antunes", City = "Maragogi", State = "AL", Country = "Brasil", ZipCode = "57955-000", Complement = "Resort beira-mar, bangalôs exclusivos"
                    },
                    new LodgingInfo { // Olinda
                        Baths = 1, Beds = 2, WifiIncluded = true, ParkingSpot = false, SwimmingPool = false, FitnessCenter = false, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Rua do Amparo", Number = "100", Neighborhood = "Amparo", City = "Olinda", State = "PE", Country = "Brasil", ZipCode = "53025-080", Complement = "Pousada histórica no centro colonial"
                    },
                    new LodgingInfo { // Recife
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Av. Boa Viagem", Number = "4070", Neighborhood = "Boa Viagem", City = "Recife", State = "PE", Country = "Brasil", ZipCode = "51021-000", Complement = "Hotel de luxo à beira-mar"
                    },
                    new LodgingInfo { // Rio de Janeiro
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Av. Atlântica", Number = "3264", Neighborhood = "Copacabana", City = "Rio de Janeiro", State = "RJ", Country = "Brasil", ZipCode = "22070-001", Complement = "Suíte com vista para o mar, Copacabana Palace"
                    },
                    new LodgingInfo { // Salvador
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = true, AirConditioned = true, Breakfast = true,
                        Street = "Rua Fonte do Boi", Number = "215", Neighborhood = "Rio Vermelho", City = "Salvador", State = "BA", Country = "Brasil", ZipCode = "41940-360", Complement = "Hotel boutique no bairro boêmio"
                    },
                    // INTERNACIONAIS
                    new LodgingInfo { // Londres
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Strand", Number = "336", Neighborhood = "Westminster", City = "London", State = "England", Country = "United Kingdom", ZipCode = "WC2R 1HA", Complement = "The Savoy Hotel, luxo clássico londrino"
                    },
                    new LodgingInfo { // Madri
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Calle de Alcalá", Number = "66", Neighborhood = "Salamanca", City = "Madrid", State = "Madrid", Country = "Espanha", ZipCode = "28009", Complement = "Hotel 5 estrelas próximo ao Parque do Retiro"
                    },
                    new LodgingInfo { // Moscow
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Ulitsa Tverskaya", Number = "3", Neighborhood = "Tverskoy", City = "Moscow", State = "Moscow", Country = "Rússia", ZipCode = "125009", Complement = "Hotel de luxo próximo à Praça Vermelha"
                    },
                    new LodgingInfo { // New York
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "5th Avenue", Number = "700", Neighborhood = "Manhattan", City = "New York", State = "NY", Country = "USA", ZipCode = "10019", Complement = "The Plaza Hotel, Central Park"
                    },
                    new LodgingInfo { // Paris
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Avenue des Champs-Élysées", Number = "31", Neighborhood = "8th arrondissement", City = "Paris", State = "Île-de-France", Country = "França", ZipCode = "75008", Complement = "Hotel Barrière Le Fouquet's Paris"
                    },
                    new LodgingInfo { // Tokyo
                        Baths = 2, Beds = 2, WifiIncluded = true, ParkingSpot = true, SwimmingPool = true, FitnessCenter = true, RestaurantOnSite = true, PetAllowed = false, AirConditioned = true, Breakfast = true,
                        Street = "Nishi Shinjuku", Number = "2-2-1", Neighborhood = "Shinjuku", City = "Tokyo", State = "Tokyo", Country = "Japão", ZipCode = "160-0023", Complement = "Park Hyatt Tokyo, vista para o Monte Fuji"
                    }
                };
                await dbContext.LodgingInfos.AddRangeAsync(lodgingList);
                await dbContext.SaveChangesAsync();

                var lodgingInfos = await dbContext.LodgingInfos.ToListAsync();

                // Mapeamento destino -> LodgingInfo
                int GetLodgingId(string destino)
                {
                    // Use FirstOrDefault and fallback to the first lodgingInfo if not found
                    return destino switch
                    {
                        "Cataratas do Iguaçu" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Foz do Iguaçu"))?.Id ?? lodgingInfos.First().Id,
                        "Maragogi" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Maragogi"))?.Id ?? lodgingInfos.First().Id,
                        "Olinda" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Olinda"))?.Id ?? lodgingInfos.First().Id,
                        "Recife" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Recife"))?.Id ?? lodgingInfos.First().Id,
                        "Rio de Janeiro" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Rio de Janeiro"))?.Id ?? lodgingInfos.First().Id,
                        "Salvador" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Salvador"))?.Id ?? lodgingInfos.First().Id,
                        "Londres" => lodgingInfos.FirstOrDefault(l => l.City.Contains("London"))?.Id ?? lodgingInfos.First().Id,
                        "Madri" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Madrid"))?.Id ?? lodgingInfos.First().Id,
                        "Moscow" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Moscow"))?.Id ?? lodgingInfos.First().Id,
                        "New York" => lodgingInfos.FirstOrDefault(l => l.City.Contains("New York"))?.Id ?? lodgingInfos.First().Id,
                        "Paris" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Paris"))?.Id ?? lodgingInfos.First().Id,
                        "Tokyo" => lodgingInfos.FirstOrDefault(l => l.City.Contains("Tokyo"))?.Id ?? lodgingInfos.First().Id,
                        _ => lodgingInfos.First().Id
                    };
                }

                var pacotes = new List<Packages>
                {
                    new Packages {
                        Name = "Aventura nas Cataratas do Iguaçu",
                        Description = "Explore as maravilhas das Cataratas do Iguaçu, com passeio pelo Parque Nacional, visita à fronteira tríplice e experiências únicas na natureza.",
                        Price = 2500,
                        ImageUrl = "uploads/1.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("10/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("17/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "Cataratas do Iguaçu",
                        Quantity = 20,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 15,
                        LodgingInfoId = GetLodgingId("Cataratas do Iguaçu")
                    },
                    new Packages {
                        Name = "Encantos de Maragogi",
                        Description = "Aproveite as piscinas naturais de Maragogi, o 'Caribe brasileiro', com águas cristalinas, passeios de jangada e culinária local.",
                        Price = 3200,
                        ImageUrl = "uploads/2.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("11/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("18/08/2025", "dd/MM/yyyy", null),
                        Origin = "Recife",
                        Destination = "Maragogi",
                        Quantity = 15,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 0,
                        LodgingInfoId = GetLodgingId("Maragogi")
                    },
                    new Packages {
                        Name = "Carnaval Histórico em Olinda",
                        Description = "Descubra o charme de Olinda, patrimônio mundial, com ladeiras coloridas, igrejas históricas e o melhor carnaval do Brasil.",
                        Price = 2000,
                        ImageUrl = "uploads/3.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("12/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("19/08/2025", "dd/MM/yyyy", null),
                        Origin = "Recife",
                        Destination = "Olinda",
                        Quantity = 10,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 5,
                        LodgingInfoId = GetLodgingId("Olinda")
                    },
                    new Packages {
                        Name = "Recife: Cultura e Praias",
                        Description = "Conheça Recife, a 'Veneza brasileira', com passeios de barco, cultura, história e gastronomia local.",
                        Price = 2300,
                        ImageUrl = "uploads/4.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("13/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("20/08/2025", "dd/MM/yyyy", null),
                        Origin = "Salvador",
                        Destination = "Recife",
                        Quantity = 12,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 10,
                        LodgingInfoId = GetLodgingId("Recife")
                    },
                    new Packages {
                        Name = "Rio de Janeiro: Maravilhas Cariocas",
                        Description = "Viva o Rio de Janeiro, com visita ao Cristo Redentor, Pão de Açúcar, praias famosas e vida noturna agitada.",
                        Price = 3500,
                        ImageUrl = "uploads/5.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("14/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("21/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "Rio de Janeiro",
                        Quantity = 25,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 0,
                        LodgingInfoId = GetLodgingId("Rio de Janeiro")
                    },
                    new Packages {
                        Name = "Salvador: Ritmos e Sabores",
                        Description = "Mergulhe na cultura baiana em Salvador, com Pelourinho, praias, festas e culinária típica.",
                        Price = 2800,
                        ImageUrl = "uploads/6.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("15/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("22/08/2025", "dd/MM/yyyy", null),
                        Origin = "Recife",
                        Destination = "Salvador",
                        Quantity = 18,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 0,
                        LodgingInfoId = GetLodgingId("Salvador")
                    },
                    // INTERNACIONAIS
                    new Packages {
                        Name = "Londres Real e Moderna",
                        Description = "Descubra Londres, com passeios pela Tower Bridge, Big Ben, museus, pubs tradicionais e cultura vibrante.",
                        Price = 12000,
                        ImageUrl = "uploads/7.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("16/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("23/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "Londres",
                        Quantity = 8,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 0,
                        LodgingInfoId = GetLodgingId("Londres")
                    },
                    new Packages {
                        Name = "Madri: Arte e Gastronomia",
                        Description = "Explore Madri, capital espanhola, com arte, cultura, gastronomia e vida noturna animada.",
                        Price = 10500,
                        ImageUrl = "uploads/8.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("17/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("24/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "Madri",
                        Quantity = 10,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 5,
                        LodgingInfoId = GetLodgingId("Madri")
                    },
                    new Packages {
                        Name = "Moscow Imperial",
                        Description = "Conheça Moscow, com a Praça Vermelha, Kremlin, cultura russa e experiências únicas no inverno europeu.",
                        Price = 13000,
                        ImageUrl = "uploads/9.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("18/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("25/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "Moscow",
                        Quantity = 6,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 0,
                        LodgingInfoId = GetLodgingId("Moscow")
                    },
                    new Packages {
                        Name = "New York: Luzes e Broadway",
                        Description = "Viva a experiência de Nova York, com Times Square, Central Park, Broadway e compras inesquecíveis.",
                        Price = 15000,
                        ImageUrl = "uploads/10.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("19/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("26/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "New York",
                        Quantity = 14,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 20,
                        LodgingInfoId = GetLodgingId("New York")
                    },
                    new Packages {
                        Name = "Paris Romântica",
                        Description = "Encante-se com Paris, Torre Eiffel, Louvre, gastronomia francesa e passeios pelo Sena.",
                        Price = 14000,
                        ImageUrl = "uploads/11.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("20/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("27/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "Paris",
                        Quantity = 12,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 15,
                        LodgingInfoId = GetLodgingId("Paris")
                    },
                    new Packages {
                        Name = "Tokyo: Futuro e Tradição",
                        Description = "Descubra Tokyo, tecnologia, cultura pop, templos milenares e culinária oriental.",
                        Price = 16000,
                        ImageUrl = "uploads/12.jpg",
                        ActiveFrom = DateTime.ParseExact("01/08/2025", "dd/MM/yyyy", null),
                        ActiveUntil = DateTime.ParseExact("30/09/2025", "dd/MM/yyyy", null),
                        BeginDate = DateTime.ParseExact("21/08/2025", "dd/MM/yyyy", null),
                        EndDate = DateTime.ParseExact("28/08/2025", "dd/MM/yyyy", null),
                        Origin = "São Paulo",
                        Destination = "Tokyo",
                        Quantity = 7,
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null),
                        IsAvailable = true,
                        DiscountPercent = 0,
                        LodgingInfoId = GetLodgingId("Tokyo")
                    }
                };
                dbContext.Packages.AddRange(pacotes);
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
                        CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null)
                    });
                    comentarioIndex++;
                }
            }
            if (ratings.Count > 0)
            {
                dbContext.Rating.AddRange(ratings);
                await dbContext.SaveChangesAsync();
            }

            // Remove o bloco antigo de associação de mídias por ImageUrl
            // Limpa registros antigos de PackageMedia para evitar conflitos de FK
            dbContext.PackageMedia.RemoveRange(dbContext.PackageMedia);
            await dbContext.SaveChangesAsync();

            // Adiciona todas as mídias reais de cada pacote conforme a estrutura wwwroot/uploads/{packageId}/{arquivo}
            var wwwrootUploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (Directory.Exists(wwwrootUploadsPath))
            {
                // Busca todos os PackageIds válidos do banco
                var validPackageIds = await dbContext.Packages.Select(p => p.Id).ToListAsync();
                var packageDirs = Directory.GetDirectories(wwwrootUploadsPath);
                var realMediaList = new List<PackageMedia>();
                foreach (var packageDir in packageDirs)
                {
                    if (int.TryParse(new DirectoryInfo(packageDir).Name, out int packageId) && validPackageIds.Contains(packageId))
                    {
                        var mediaFiles = Directory.GetFiles(packageDir);
                        foreach (var mediaFile in mediaFiles)
                        {
                            string relativePath = Path.Combine("uploads", packageId.ToString(), Path.GetFileName(mediaFile)).Replace("\\", "/");
                            realMediaList.Add(new PackageMedia
                            {
                                PackageId = packageId,
                                ImageURL = relativePath,
                                MediaType = 1, // 1 para imagem
                                CreatedAt = DateTime.ParseExact("01/07/2025", "dd/MM/yyyy", null)
                            });
                        }
                    }
                }
                if (realMediaList.Count > 0)
                {
                    dbContext.PackageMedia.AddRange(realMediaList);
                    await dbContext.SaveChangesAsync();
                }
            }

            // Adiciona reservas (Bookings) para vários clientes e pacotes, com opções aleatórias e mais reservas
            var bookings = new List<Booking>();
            var bookingStatuses = new[] { "Aprovado", "Pendente", "Recusado" };
            // Usa o random já existente
            int reservasPorClientePorPacote = 2; // Aumenta o volume de reservas
            for (int i = 0; i < clientesSalvos.Count; i++)
            {
                var cliente = clientesSalvos[i];
                for (int j = 0; j < pacotesParaAvaliar.Count; j++)
                {
                    var pacote = pacotesParaAvaliar[j];
                    for (int k = 0; k < reservasPorClientePorPacote; k++)
                    {
                        var status = bookingStatuses[random.Next(bookingStatuses.Length)];
                        var bookingDate = pacote.BeginDate.AddDays(-random.Next(1, 30));
                        var travelDate = pacote.BeginDate.AddDays(random.Next(0, 5));
                        bookings.Add(new Booking
                        {
                            UserId = cliente.Id,
                            PackageId = pacote.Id,
                            BookingDate = bookingDate,
                            TravelDate = travelDate,
                            Status = status,
                            CreatedAt = bookingDate,
                            UpdatedAt = bookingDate.AddHours(random.Next(1, 48)),
                            HasTravelInsurance = random.Next(2) == 0,
                            HasTourGuide = random.Next(2) == 0,
                            HasTour = random.Next(2) == 0,
                            HasActivities = random.Next(2) == 0,
                            FinalPrice = pacote.Price * (1 - ((decimal)(pacote.DiscountPercent ?? 0) / 100m))
                        });
                    }
                }
            }
            if (bookings.Count > 0)
            {
                dbContext.Bookings.AddRange(bookings);
                await dbContext.SaveChangesAsync();

                // Adiciona Payments para cada Booking criado
                var bookingsSalvos = await dbContext.Bookings.ToListAsync();
                var paymentMethods = new[] { "Pix", "Boleto", "CartaoCredito", "CartaoDebito" };
                var payments = new List<Payments>();
                for (int i = 0; i < bookingsSalvos.Count; i++)
                {
                    var booking = bookingsSalvos[i];
                    var paymentStatus = booking.Status;
                    if (paymentStatus != "Aprovado" && paymentStatus != "Pendente" && paymentStatus != "Recusado")
                        paymentStatus = "Pendente";
                    payments.Add(new Payments
                    {
                        BookingId = booking.Id,
                        Amount = booking.FinalPrice,
                        PaymentMethod = paymentMethods[i % paymentMethods.Length],
                        Status = paymentStatus,
                        PaymentDate = booking.CreatedAt,
                        CreatedAt = booking.CreatedAt,
                        UpdatedAt = booking.UpdatedAt
                    });
                }
                if (payments.Count > 0)
                {
                    dbContext.Payments.AddRange(payments);
                    await dbContext.SaveChangesAsync();
                }
            }
        }
    }
}