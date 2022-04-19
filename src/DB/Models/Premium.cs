using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DB.Models.EnumTypes;
using Microsoft.EntityFrameworkCore;

namespace DB.Models
{
    [Table("premium")]
    public partial class Premium
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = null!;

        [Column("premium_type")] 
        public PremiumType PremiumType { get; set; }

        [Column("start_at", TypeName = "timestamp without time zone")]
        public DateTime StartAt { get; set; }
        
        [Column("end_at", TypeName = "timestamp without time zone")]
        public DateTime EndAt { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Premium")]
        public virtual UserInfo User { get; set; } = null!;
    }
}
