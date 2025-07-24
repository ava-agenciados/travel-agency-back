namespace travel_agency_back.DTOs.Requests
{
    public class ChangeUserRoleDTO
    {
        public string Email { get; set; }
        public string NewRole { get; set; }
    }
}