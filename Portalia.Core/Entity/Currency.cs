namespace Portalia.Core.Entity
{
    public class Currency: Repository.Pattern.Ef6.Entity
    {
        public string ID { get; set; }
        public string Label { get; set; }
        public string Symbol { get; set; }
    }
}
