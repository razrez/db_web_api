using System.ComponentModel;

namespace DB.Models.Authorization;

public class PasswordFlowData
{
    [DefaultValue("password")]
    public string grant_type { get; set; }
    [DefaultValue("user01@gmail.com")]
    public string username { get; set; }
    [DefaultValue("qWe!123")]
    public string password { get; set; }
}