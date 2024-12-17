namespace BusinessLogic.DTOs.User
{
    public class ReadUserDto
    {
        public int UserId { get; set; }
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? Status { get; set; } // "Active" or "Banned"
        public List<string>? Roles { get; set; }
    }
}
