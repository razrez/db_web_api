using System.ComponentModel;

namespace DB.Models.Authorization;

public class RefreshTokenData
{
    [System.ComponentModel.DefaultValue("refresh_token")]
    public string grant_type { get; set; }
    public string refresh_token { get; set; }
}