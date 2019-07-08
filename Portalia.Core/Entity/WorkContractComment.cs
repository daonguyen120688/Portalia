namespace Portalia.Core.Entity
{
    public class WorkContractComment : Repository.Pattern.Ef6.Entity
    {
        public int WCCommentId { get; set; }
        public int ContractId { get; set; }
        public string Comment { get; set; }
        public System.DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }

        public virtual WorkContract WorkContract { get; set; }
    }
}
