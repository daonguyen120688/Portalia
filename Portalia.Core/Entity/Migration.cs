using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Portalia.Core.Entity
{
    [Table("Migration", Schema = "Auth")]
    public class Migration
    {
        [Key, Column(Order = 0)]
        public string OldAspNetUserId { get; set; }
        [Key, Column(Order = 1)]
        public string NewAuth0Id { get; set; }
    }
}
