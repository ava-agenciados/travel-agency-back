using travel_agency_back.DTOs.Requests.Booking;

namespace travel_agency_back.DTOs.Requests.Packages
{
    public class BuyPackageDTO
    {
        public string FirstName { get; set; } = string.Empty; // Nome do comprador
        public string LastName { get; set; } = string.Empty; // Sobrenome do comprador
        
        public string CPFPassport { get; set; } = string.Empty; // CPF ou número de passaporte do comprador
        public string PhoneNumber { get; set; } = string.Empty; // Número de telefone do comprador
        public bool IsForeigner { get; set; } = false; // Indica se o comprador é estrangeiro
        public List<CompanionsRequestDTO?> Companions { get; set; } = new (); // Lista de acompanhantes

    }
}
