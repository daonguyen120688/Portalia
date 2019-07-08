
namespace Portalia.Core.Entity
{
    public class RefData : Repository.Pattern.Ef6.Entity
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
