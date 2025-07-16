namespace travel_agency_back.DTOs.Requests
{
    public class CompanionsRequestDTO
    {
        public string FirstName { get; set; }
        public string LastName { get; set; } 
        public string CPFPassport { get; set; }
        public bool IsForeigner { get; set; } = false; // Indica se o acompanhante é estrangeiro
        public string PhneNumber { get; set; } // Número de telefone do acompanhante
    }
}
