
namespace Portalia.Core.Dtos
{
    public class WorkContractCommentDto
    {
        public int WCCommentId { get; set; }
        public int ContractId { get; set; }
        public string Comment { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
