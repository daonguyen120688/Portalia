using System.Collections.Generic;
using System.Linq;
using Portalia.Core.Entity;
using Portalia.Core.Enum;

namespace Portalia.Core.Interface.Service
{
    public interface IProposalService
    {
        Proposal Create(string userId, string projectName, string clientName, string description);
        List<Proposal> GetByUser(string userId);
        Proposal GetById(int proposalId,string userId);
        Proposal UpdateStatus(int proposalId, ProposalStatus status);
        void DeleteProposal(int proposalId);
        void UpdateStatus(string userId, ProposalStatus approved);
        void DeleteProposal(int proposalId, string userId);
        Proposal GetById(string userId);
        IQueryable<FolderTypeEntity> GetFolderTypes();
    }
}
