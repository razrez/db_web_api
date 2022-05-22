using System.ComponentModel;

namespace DB.Models.Authorization;

public class ClientCredentialsData
{
    [DefaultValue("client_credentials")]
    public string grant_type { get; set; }
    public string client_id { get; set; }
    public string client_secret { get; set; }
}