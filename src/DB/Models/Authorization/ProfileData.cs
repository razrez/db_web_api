using DB.Models.EnumTypes;

namespace DB.Models.Authorization;

public class ProfileData
{
    public string Name { get; set; }
    public int BirthYear { get; set; }
    public int BirthMonth { get; set; }
    public int BirthDay { get; set; }
    public string ProfileImg { get; set; }
}