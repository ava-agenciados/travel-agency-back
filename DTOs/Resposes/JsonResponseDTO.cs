namespace travel_agency_back.DTOs.Resposes
{
    public class JSONResponseDTO<T>
    {
        private string data;

        public JSONResponseDTO(int StatusCode, string Message, bool Success, string Data)
        {
            this.StatusCode = StatusCode;
            this.Message = Message;
            this.Success = Success;
            data = Data;
        }

        public bool Success { get; set; }
        public T Data { get; set; }
        public string Message { get; set; } = string.Empty;
        public int StatusCode { get; set; }

    }
}
