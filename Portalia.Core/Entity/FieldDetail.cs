
namespace Portalia.Core.Entity
{
    public class FieldDetail : Repository.Pattern.Ef6.Entity
    {
        public int FieldDetailId { get; set; }
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public int Order { get; set; }
    }
}
