using System;
using System.Collections.Generic;
using System.Linq;
using Portalia.Core.Entity;
using Portalia.Core.Enum;
using Portalia.Core.Interface.Service;
using Portalia.Repository;
using Repository.Pattern.Infrastructure;
using Repository.Pattern.Repositories;
using Repository.Pattern.UnitOfWork;

namespace Portalia.Service
{
    public class ProposalService : IProposalService
    {
        private readonly IRepositoryAsync<Proposal> _proposalRepository;
        private readonly IUnitOfWorkAsync _unitOfWork;
        private readonly PortaliaContext _portaliaContext;
        public ProposalService(IRepositoryAsync<Proposal> proposalRepository, IUnitOfWorkAsync unitOfWork, PortaliaContext portaliaContext)
        {
            _proposalRepository = proposalRepository;
            _unitOfWork = unitOfWork;
            _portaliaContext = portaliaContext;
        }

        public Proposal Create(string userId, string projectName, string clientName, string description)
        {
            var currentDateTime = DateTime.Now;
            var proposal = new Proposal
            {
                ClientName = clientName,
                CreatedDate = currentDateTime,
                Description = description,
                ProjectName = projectName,
                ObjectState = ObjectState.Added,
                ProposalStatus = ProposalStatus.New,
                UpdatedDate = currentDateTime,
                UserId = userId
            };

            _proposalRepository.Insert(proposal);
            _unitOfWork.SaveChanges();

            return proposal;
        }

        public List<Proposal> GetByUser(string userId)
        {
            return _proposalRepository.Queryable().Where(x => x.UserId == userId && x.IsActive).ToList();
        }

        public Proposal GetById(int proposalId, string userId)
        {
            return _proposalRepository.Queryable().FirstOrDefault(x => x.ProposalId == proposalId && x.UserId == userId);
        }

        public Proposal UpdateStatus(int proposalId, ProposalStatus status)
        {
            var proposal = _proposalRepository.Find(proposalId);
            proposal.ProposalStatus = status;
            proposal.ObjectState = ObjectState.Modified;

            _proposalRepository.Update(proposal);
            _unitOfWork.SaveChanges();

            return proposal;
        }

        public void DeleteProposal(int proposalId)
        {
            var proposal = _proposalRepository.Find(proposalId);
            proposal.ObjectState = ObjectState.Modified;
            proposal.IsActive = false;
            _proposalRepository.Update(proposal);
            _unitOfWork.SaveChanges();
        }

        public void UpdateStatus(string userId, ProposalStatus status)
        {
            var proposals = _proposalRepository.Queryable().Where(x => x.UserId == userId && x.IsActive && x.ProposalStatus == ProposalStatus.SentProposal).ToList();
            foreach (var proposal in proposals)
            {
                proposal.ProposalStatus = ProposalStatus.Approved;
                proposal.ObjectState = ObjectState.Modified;
            }
            _unitOfWork.SaveChanges();
        }

        public void DeleteProposal(int proposalId, string userId)
        {
            var proposal = _proposalRepository.Queryable().FirstOrDefault(c => c.ProposalId == proposalId && c.UserId == userId);
            if (proposal == null) return;
            proposal.ObjectState = ObjectState.Modified;
            proposal.IsActive = false;
            _unitOfWork.SaveChanges();
            _proposalRepository.Update(proposal);
        }

        public Proposal GetById(string userId)
        {
            var proposal = _proposalRepository.Queryable().FirstOrDefault(c=>c.UserId == userId);
            if (proposal == null)
            {
                var newProposal = new Proposal()
                {
                    ClientName = "My Space",
                    Description = "My Space",
                    ProjectName = "My Space",
                    ProposalStatus = ProposalStatus.New,
                    ObjectState = ObjectState.Added,
                    UserId = userId,
                    CreatedDate = DateTime.Now
                };
                _proposalRepository.Insert(newProposal);
                _unitOfWork.SaveChanges();
                return newProposal;
            }
            return proposal;
        }

        public IQueryable<FolderTypeEntity> GetFolderTypes()
        {
            var foldertype = _portaliaContext.FolderTypes.AsQueryable();
            return foldertype;
        }
    }
}
