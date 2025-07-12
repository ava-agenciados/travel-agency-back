namespace travel_agency_back.DTOs.Resposes
{
    public class GenericResponseDTO
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }
        public GenericResponseDTO(int StatusCode, string Message, bool Success)
        {
            this.StatusCode = StatusCode;
            this.Message = Message;
            this.Success = Success;
     
        }
    }
}
