namespace travel_agency_back.Models
{


    //lista de pacotes disponiveis NO TOTAL LIVREEEESS
    public class Packages
    {
        public string PackageName { get; set; } = string.Empty;
        public string Destination{ get; set; } = string.Empty;
        public string Origin { get; set; } = string.Empty;
        public decimal Price     { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ImageURL { get; set; } = string.Empty;


        public bool IsAvaliable { get; set; } = true;

        public DateTime DepartureDate{ get; set; }
        public DateTime ReturnDate { get; set; }


    }
}
