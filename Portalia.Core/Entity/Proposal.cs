using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Portalia.Core.Enum;

namespace Portalia.Core.Entity
{
    public class Proposal : Repository.Pattern.Ef6.Entity
    {
        public Proposal()
        {
            Documents = new HashSet<Document>();
            IsActive = true;
        }

        public int ProposalId { get; set; }
        public string ProjectName { get; set; }
        public string ClientName { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public string UserId { get; set; }
        public ProposalStatus ProposalStatus { get; set; }
        public bool IsActive { get; set; }
        [JsonIgnore]
        public virtual ICollection<Document> Documents { get; set; }
    }
}
