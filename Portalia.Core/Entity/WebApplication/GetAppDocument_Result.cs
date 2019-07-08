
namespace Portalia.Core.Entity.WebApplication
{
    public class GetAppDocument_Result
    {
        public string FileName { get; set; }
        public string Extension { get; set; }
        public byte[] Binary { get; set; }
        public byte TypeId { get; set; }
    }
}
