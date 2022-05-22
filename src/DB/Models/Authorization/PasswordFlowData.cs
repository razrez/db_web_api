using System.ComponentModel;

namespace DB.Models.Authorization;

public class PasswordFlowData
{
    [DefaultValue("password")]
    public string grant_type { get; set; }
    public string username { get; set; }
    public string password { get; set; }
}