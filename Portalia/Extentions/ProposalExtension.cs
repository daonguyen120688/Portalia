using Portalia.Core.Entity;
using Portalia.ViewModels.Proposals;

namespace Portalia.Extentions
{
    public static class ProposalExtension
    {
        public static ProposalDetailViewModel ToProposalDetailViewModel(this Proposal proposal)
        {
            if (proposal == null)
            {
                return null;
            }

            return new ProposalDetailViewModel
            {
                ProposalId = proposal.ProposalId,
                UserId = proposal.UserId
            };
        }
    }
}