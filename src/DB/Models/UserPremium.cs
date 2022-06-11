using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DB.Models.EnumTypes;

namespace DB.Models
{
    [Table("user_premium")]
    public class UserPremium
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = null!;
        
        [Key]
        [Column("premium_id")]
        public int PremiumId { get; set; }

        [Column("start_at", TypeName = "timestamp without time zone")]
        public DateTime StartAt { get; set; }
        
        [Column("end_at", TypeName = "timestamp without time zone")]
        public DateTime EndAt { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("UserPremium")]
        public virtual UserInfo User { get; set; } = null!;
        
        public Premium Premium { get; set; }
    }
}
