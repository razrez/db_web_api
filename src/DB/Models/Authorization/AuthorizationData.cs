using System.ComponentModel;

namespace DB.Models.Authorization;

public class PasswordFlowData
{
    [System.ComponentModel.DefaultValue("password")]
    public string grant_type { get; set; }
    [System.ComponentModel.DefaultValue("user01@gmail.com")]
    public string username { get; set; }
    [System.ComponentModel.DefaultValue("qWe!123")]
    public string password { get; set; }
}