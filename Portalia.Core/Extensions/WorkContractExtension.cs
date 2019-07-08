using System.Linq;
using Portalia.Core.Dtos;
using Portalia.Core.Entity;
using Portalia.Core.Enum;

namespace Portalia.Core.Extensions
{
    public static class WorkContractExtension
    {
        public static WorkContractDto ToWorkContractDto(this WorkContract workContract)
        {
            if (workContract == null)
                return null;

            var dto = new WorkContractDto
            {
                ContractId = workContract.ContractId,
                UserId = workContract.UserId,
                Type = workContract.Type,
                Title = workContract.Title,
                FirstName = workContract.FirstName,
                LastName = workContract.LastName,
                DateOfBirth = workContract.DateOfBirth,
                CountryOfBirth = workContract.CountryOfBirth,
                Nationality = workContract.Nationality,
                Address = workContract.Address,
                PostCode = workContract.PostCode,
                City = workContract.City,
                Country = workContract.Country,
                VisaPermitNo = workContract.VisaPermitNo,
                ContractStartDate = workContract.ContractStartDate,
                ContractEndDate = workContract.ContractEndDate,
                ProjectDescription = workContract.ProjectDescription,
                Skills = workContract.Skills,
                ClientNameByCandidate = workContract.ClientNameByCandidate,
                ClientAddress = workContract.ClientAddress,
                Allowances = workContract.Allowances,
                Currency = workContract.Currency,
                Basic = workContract.Basic,
                CandidateId = workContract.CandidateId,
                CreatedDate = workContract.CreatedDate,
                CreatedBy = workContract.CreatedBy,
                UpdatedDate = workContract.UpdatedDate,
                UpdatedBy = workContract.UpdatedBy,
                SSN = workContract.SSN,
                Email = workContract.AspNetUser.Email,
                HighlightedFields=workContract.HighlightedFields,
                DocumentId = workContract.DocumentId ?? 0
            };

            // Status
            if (workContract.WorkContractStatuses != null && workContract.WorkContractStatuses.Any())
            {
                dto.WorkContractStatusId = workContract.WorkContractStatuses
                    .OrderByDescending(x => x.CreatedDate)
                    .Select(x => (WorkContractStatusEnum)x.StatusId)
                    .FirstOrDefault();
            }            

            return dto;
        }

        public static WorkContractDto ExcludeNonRelatedData(this WorkContractDto dto, WorkContract domain)
        {
            if (dto == null || domain == null)
            {
                return null;
            }

            dto.CreatedDate = domain.CreatedDate;
            dto.CreatedBy = domain.CreatedBy;

            return dto;
        }

        public static WorkContract ToWorkContract(this WorkContractDto workContract)
        {
            if (workContract == null)
                return null;

            return new WorkContract
            {
                ContractId = workContract.ContractId,
                UserId = workContract.UserId,
                Type = workContract.Type,
                Title = workContract.Title,
                FirstName = workContract.FirstName,
                LastName = workContract.LastName,
                DateOfBirth = workContract.DateOfBirth,
                CountryOfBirth = workContract.CountryOfBirth,
                Nationality = workContract.Nationality,
                Address = workContract.Address,
                PostCode = workContract.PostCode,
                City = workContract.City,
                Country = workContract.Country,
                VisaPermitNo = workContract.VisaPermitNo,
                ContractStartDate = workContract.ContractStartDate,
                ContractEndDate = workContract.ContractEndDate,
                ProjectDescription = workContract.ProjectDescription,
                Skills = workContract.Skills,
                ClientNameByCandidate = workContract.ClientNameByCandidate,
                ClientAddress = workContract.ClientAddress,
                Allowances = workContract.Allowances,
                Currency = workContract.Currency,
                Basic = workContract.Basic,
                CandidateId = workContract.CandidateId,
                CreatedDate = workContract.CreatedDate,
                CreatedBy = workContract.CreatedBy,
                UpdatedDate = workContract.UpdatedDate,
                UpdatedBy = workContract.UpdatedBy,
                SSN=workContract.SSN
            };
        }

        // Map fields from DTO to Domain model when we update work contract.
        public static void ToUpdateWorkContract(this WorkContract domain, WorkContractDto workContractDto)
        {
            if (domain == null)
            {
                return;
            }

            domain.Title = workContractDto.Title;
            domain.Type = workContractDto.Type;
            domain.FirstName = workContractDto.FirstName;
            domain.LastName = workContractDto.LastName;
            domain.DateOfBirth = workContractDto.DateOfBirth;
            domain.CountryOfBirth = workContractDto.CountryOfBirth;
            domain.Nationality = workContractDto.Nationality;
            domain.Address = workContractDto.Address;
            domain.PostCode = workContractDto.PostCode;
            domain.City = workContractDto.City;
            domain.Country = workContractDto.Country;
            domain.VisaPermitNo = workContractDto.VisaPermitNo;
            domain.ContractStartDate = workContractDto.ContractStartDate;
            domain.ContractEndDate = workContractDto.ContractEndDate;
            domain.ProjectDescription = workContractDto.ProjectDescription;
            domain.Skills = workContractDto.Skills;
            domain.ClientNameByCandidate = workContractDto.ClientNameByCandidate;
            domain.ClientAddress = workContractDto.ClientAddress;
            domain.Allowances = workContractDto.Allowances;
            domain.Currency = workContractDto.Currency;
            domain.Basic = workContractDto.Basic;
            domain.CandidateId = workContractDto.CandidateId;
            domain.UpdatedDate = workContractDto.UpdatedDate;
            domain.UpdatedBy = workContractDto.UpdatedBy;
            domain.SSN = workContractDto.SSN;
            domain.HighlightedFields = workContractDto.HighlightedFields;
        }

        public static WorkContractComment ToWorkContractComment(this WorkContractCommentDto comment)
        {
            if (comment == null)
                return null;

            return new WorkContractComment()
            {
                WCCommentId=comment.WCCommentId,
                ContractId=comment.ContractId,
                Comment=comment.Comment,
                CreatedBy=comment.CreatedBy,
                CreatedDate=comment.CreatedDate
            };
        }

        public static WorkContractCommentDto ToWorkContractCommentDto(this WorkContractComment comment)
        {
            if (comment == null)
                return null;

            return new WorkContractCommentDto()
            {
                WCCommentId = comment.WCCommentId,
                ContractId = comment.ContractId,
                Comment = comment.Comment,
                CreatedBy = comment.CreatedBy,
                CreatedDate = comment.CreatedDate
            };
        }
    }
}
