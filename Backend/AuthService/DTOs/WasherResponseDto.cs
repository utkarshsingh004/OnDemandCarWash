namespace AuthService.DTOs
{
    public class WasherResponseDto
    {
        public Guid WasherId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        public int Priority { get; set; }
    }
}