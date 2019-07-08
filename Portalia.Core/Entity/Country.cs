namespace Portalia.Core.Entity
{
    public class Country: Repository.Pattern.Ef6.Entity
    {
        public string ID { get; set; }
        public string Label { get; set; }
    }
}
