namespace travel_agency_back.Models
{
    public class UserDocument
    {
        public int Id { get; set; } // Identificador único do documento
        public string DocumentType { get; set; } // Tipo do documento (CPF, Passaporte, etc.)
        public string DocumentNumber { get; set; } // Número do documento
        public int UserId { get; set; } // Chave estrangeira para o usuário associado
        public User User { get; set; } // Navegação para o usuário associado
    }
}
