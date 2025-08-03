using Microsoft.AspNetCore.Mvc;
using System.Linq.Expressions;
using travel_agency_back.DTOs.Requests.Booking;
using travel_agency_back.DTOs.Resposes;
using travel_agency_back.DTOs.Resposes.Packages;
using travel_agency_back.Models;
using travel_agency_back.Repositories.Interfaces;
using travel_agency_back.Services.Interfaces;
using travel_agency_back.Third_party.Mail;

namespace travel_agency_back.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepository;
        private readonly IPackageRepository _packageRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<BookingService> _logger;

        public BookingService(IBookingRepository bookingRepository, IPackageRepository packageRepository, IUserRepository userRepository, ILogger<BookingService> logger)
        {
            _bookingRepository = bookingRepository;
            _packageRepository = packageRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<IActionResult> CreateUserBookingAsync(int userId, CreateNewBookingDTO createNewBooking)
        {
            var paymentMethods = createNewBooking.PaymentMethods?.Select(p => p.PaymentMethod.ToString()).ToList() ?? new List<string>();
            var payment = createNewBooking.PaymentMethods?.FirstOrDefault();
            var userInfo = await _userRepository.GetUserByIdAsync(userId);

            // Busque o pacote antes de criar o Booking
            var package = await _packageRepository.GetPackageByIdAsync(createNewBooking.PackageID);

            if (package == null)
            {
                return new NotFoundObjectResult(new GenericResponseDTO(404, "Pacote não encontrado", false));
            }

            var booking = new Booking
            {
                PackageId = createNewBooking.PackageID,
                TravelDate = createNewBooking.StartTravel,
                EndTravel = createNewBooking.EndTravel, // Volta a aceitar o valor enviado pelo usuário
                Companions = createNewBooking.Companions?.Select(c => new Companions
                {
                    FullName = $"{c.FirstName} {c.LastName}",
                    DocumentNumber = c.CPFPassport,
                }).ToList() ?? new List<Companions>(),
                Payments = createNewBooking.PaymentMethods?.Select(p => new Payments
                {
                    PaymentMethod = p.PaymentMethod.ToString(),
                    PaymentDate = DateTime.UtcNow,
                    Amount = package.Price, // Será ajustado abaixo
                }).ToList() ?? new List<Payments>(),
                HasTravelInsurance = createNewBooking.HasTravelInsurance ?? false,
                HasTourGuide = createNewBooking.HasTourGuide ?? false,
                HasTour = createNewBooking.HasTour ?? false,
                HasActivities = createNewBooking.HasActivities ?? false
            };

            // Cálculo do preço final com opcionais
            decimal basePrice = package.Price;
            int optionals = 0;
            List<string> optionalsList = new();
            if (booking.HasTravelInsurance) { optionals++; optionalsList.Add("Seguro Viagem"); }
            if (booking.HasTourGuide) { optionals++; optionalsList.Add("Guia Turístico"); }
            if (booking.HasTour) { optionals++; optionalsList.Add("Passeio Turístico"); }
            if (booking.HasActivities) { optionals++; optionalsList.Add("Atividades Extras"); }
            decimal extrasValue = basePrice * 0.02m * optionals;
            decimal discount = 0;
            if (package.DiscountPercent.HasValue && package.DiscountPercent.Value > 0)
                discount = (basePrice + extrasValue) * ((decimal)package.DiscountPercent.Value / 100m);
            decimal finalPrice = basePrice + extrasValue - discount;
            booking.FinalPrice = finalPrice;
            // Atualiza o valor em todos os pagamentos
            foreach (var pay in booking.Payments) pay.Amount = finalPrice;

            // Monta string de características do pacote
            string packageFeatures = package.Description;
            if (package.LodgingInfo != null)
            {
                packageFeatures += $"\nAcomodações: {package.LodgingInfo.Beds} camas, {package.LodgingInfo.Baths} banheiros, Wi-Fi: {(package.LodgingInfo.WifiIncluded ? "Sim" : "Não")}, Piscina: {(package.LodgingInfo.SwimmingPool ? "Sim" : "Não")}, Café da manhã: {(package.LodgingInfo.Breakfast ? "Sim" : "Não")}";
            }

            if (userInfo == null)
            {

                return new NotFoundObjectResult(new GenericResponseDTO(404, "Usuário não encontrado", false));
            }
            var packageResponse = new PackageDTO
            {
                Id = package.Id,
                Name = package.Name,
                Description = package.Description,
                Origin = package.Origin,
                Destination = package.Destination,
                Price = package.Price,
                ActiveFrom = package.ActiveFrom,
                ActiveUntil = package.ActiveUntil,
                BeginDate = package.BeginDate,
                EndDate = package.EndDate,
                Quantity = package.Quantity,
                IsAvailable = package.IsAvailable,
                ImageUrl = package.ImageUrl,
            };
           
            switch (payment.PaymentMethod)
            {
                case PaymentMethod.Pix:
                    booking.Status = "Aprovado";
                    booking.Payments.FirstOrDefault().Status = "Aprovado";
                    await EmailService.SendPixPaymentConfirmation(
                        userInfo,
                        createNewBooking.PaymentMethods?.FirstOrDefault(),
                        package,
                        booking,
                        _logger,
                        basePrice,
                        extrasValue,
                        discount,
                        finalPrice,
                        optionalsList,
                        packageFeatures
                    );
                    break;
                case PaymentMethod.Boleto:
                    booking.Status = "Pendente";
                    booking.Payments.FirstOrDefault().Status = "Pendente";
                    EmailService.SendBoletoEmail(
                        userInfo.Email,
                        userInfo.FirstName,
                        userInfo.LastName,
                        userInfo.CPFPassport,
                        finalPrice,
                        package.Name,
                        package.Destination,
                        package.Origin,
                        booking.TravelDate,
                        createNewBooking.EndTravel, // ou outro campo de fim
                        basePrice,
                        extrasValue,
                        discount,
                        optionalsList,
                        packageFeatures
                    );
                    break;
                case PaymentMethod.CreditCard:
                    booking.Status = "Recusado";
                    booking.Payments.FirstOrDefault().Status = "Recusado";
                    EmailService.SendCartaoEmail(
                        userInfo.Email,
                        payment.FirstName,
                        payment.LastName,
                        payment.CPFPassport,
                        finalPrice,
                        package.Name,
                        package.Destination,
                        package.Origin,
                        booking.TravelDate,
                        createNewBooking.EndTravel, // ou outro campo de fim
                        payment.Installments, // parcelas, ajuste conforme necessário
                        "Recusado", // status, ajuste conforme necessário
                        payment.TransactionId.ToString(),
                        null,
                        basePrice,
                        extrasValue,
                        discount,
                        optionalsList,
                        packageFeatures
                    );
                    break;
                case PaymentMethod.DebitCard:
                    booking.Status = "Recusado";
                    booking.Payments.FirstOrDefault().Status = "Recusado";
                    EmailService.SendCartaoEmail(
                        userInfo.Email,
                        payment.FirstName,
                        payment.LastName,
                        payment.CPFPassport,
                        finalPrice,
                        package.Name,
                        package.Destination,
                        package.Origin,
                        booking.TravelDate,
                        createNewBooking.EndTravel, // ou outro campo de fim
                        1, // parcelas, ajuste conforme necessário
                        "Recusado", // status, ajuste conforme necessário
                        payment.TransactionId.ToString(),
                        null,
                        basePrice,
                        extrasValue,
                        discount,
                        optionalsList,
                        packageFeatures
                    );
                    break;
                default:
                    _logger.LogWarning("Método de pagamento não reconhecido para envio de e-mail.");
                    break;
            }
            var result = await _bookingRepository.CreateUserBookingAsync(userId, booking.PackageId, booking);
            
            if (result is OkObjectResult && payment != null)
            {
                return new OkObjectResult(new GenericResponseDTO(200, "Reserva criada com sucesso", true));
            }
            return new ObjectResult(new GenericResponseDTO(500, "Falha ao gerar reserva de viagem", false));
        }

        public Task<IActionResult> GetUserBookingsAsync(int userId)
        {
            var bookings = _bookingRepository.GetUserBookingsAsync(userId);
            if (bookings == null || !bookings.Result.Any())
            {
                return Task.FromResult<IActionResult>(new NotFoundObjectResult(new GenericResponseDTO(404, "Nenhuma reserva encontrada", false)));
            }
            var response = bookings.Result.Select(b => new BookingResponseDTO
            {
                Id = b.Id,
                PackageId = b.PackageId,
                TravelDate = b.TravelDate,
                EndTravel = b.EndTravel, // Corrigido: retorna o EndTravel da reserva
                Status = b.Status,
                Companion = b.Companions?.Select(c => new CompanionResponseDTO
                {
                    FullName = c.FullName,
                    DocumentNumber = c.DocumentNumber
                }).ToList(),
                Payment = b.Payments?.Select(p => new PaymentResponseDTO
                {
                    PaymentMethod = p.PaymentMethod
                }).ToList(),
                LodgingInfo = b.Package?.LodgingInfo == null ? null : new travel_agency_back.DTOs.Packages.LodgingInfoDTO
                {
                    Baths = b.Package.LodgingInfo.Baths,
                    Beds = b.Package.LodgingInfo.Beds,
                    WifiIncluded = b.Package.LodgingInfo.WifiIncluded,
                    ParkingSpot = b.Package.LodgingInfo.ParkingSpot,
                    SwimmingPool = b.Package.LodgingInfo.SwimmingPool,
                    FitnessCenter = b.Package.LodgingInfo.FitnessCenter,
                    RestaurantOnSite = b.Package.LodgingInfo.RestaurantOnSite,
                    PetAllowed = b.Package.LodgingInfo.PetAllowed,
                    AirConditioned = b.Package.LodgingInfo.AirConditioned,
                    Breakfast = b.Package.LodgingInfo.Breakfast,
                    Location = new travel_agency_back.DTOs.Packages.AddressDTO
                    {
                        Street = b.Package.LodgingInfo.Street,
                        Number = b.Package.LodgingInfo.Number,
                        Neighborhood = b.Package.LodgingInfo.Neighborhood,
                        City = b.Package.LodgingInfo.City,
                        State = b.Package.LodgingInfo.State,
                        Country = b.Package.LodgingInfo.Country,
                        ZipCode = b.Package.LodgingInfo.ZipCode,
                        Complement = b.Package.LodgingInfo.Complement
                    }
                },
                DiscountPercent = b.Package?.DiscountPercent,
                HasTravelInsurance = b.HasTravelInsurance,
                HasTourGuide = b.HasTourGuide,
                HasTour = b.HasTour,
                HasActivities = b.HasActivities,
                // NOVO: informações do contratante
                ContractingUser = b.User == null ? null : new UserSummaryDTO
                {
                    Id = b.User.Id,
                    FirstName = b.User.FirstName,
                    LastName = b.User.LastName,
                    Email = b.User.Email,
                    CPFPassport = b.User.CPFPassport
                }
            }).ToList();

            return Task.FromResult<IActionResult>(new OkObjectResult(response));
        }
    }
}
