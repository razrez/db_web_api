using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using DB.Models;

namespace DB.Models;

[Table("premium")]
public class Premium
{
    public Premium()
    {
        UserPremiums = new HashSet<UserPremium>();
    }
    
    [Key]
    [Column("id")]
    public int Id { get; set; }
    [Column("name")]
    public string Name { get; set; } = null!;
    [Column("description")]
    public string Description { get; set; } = null!;
    [Column("price")]
    public double Price { get; set; }
    [DefaultValue(1)]
    [Column("users_count")]
    public int UserCount { get; set; }
    
    [JsonIgnore]
    public ICollection<UserPremium> UserPremiums { get; set; }
}