using System.ComponentModel.DataAnnotations;

namespace Portalia.Core.Entity
{
    public class Skill : Repository.Pattern.Ef6.Entity
    {
        [Key]
        public int SkillId { get; set; }
        public string Label { get; set; }
        public int BusinessLineId { get; set;}
        public bool Validated { get; set; }
        public System.DateTime? Created_Date { get; set; }
        public int? CreatedById { get; set; }
    }
}
