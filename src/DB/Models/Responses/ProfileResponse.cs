using DB.Models.EnumTypes;

namespace DB.Models.Responses;

public class ProfileResponse
{
    public string? Email { get; set; }
    public string? Username { get; set; }
    public DateOnly? Birthday { get; set; }
    public Country? Country { get; set; }
    
    public string? ProfileImg { get; set; }
}