using Portalia.Core.Dtos;
using System.Globalization;
using Portalia.ViewModels.WorkContracts;

namespace Portalia.Extentions
{
    public static class WorkContractExtension
    {
        public static WorkContractViewModel ToWorkContractViewModel(this WorkContractDto model)
        {
            if (model == null)
                return null;
            return new WorkContractViewModel()
            {
                ContractId = model.ContractId,
                Type = model.Type,
                Title = model.Title,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                CountryOfBirth = model.CountryOfBirth,
                Nationality = model.Nationality,
                Address = model.Address,
                PostCode = model.PostCode,
                City = model.City,
                Country = model.Country,
                VisaPermitNo = model.VisaPermitNo,
                ContractStartDate = model.ContractStartDate,
                ContractEndDate = model.ContractEndDate,
                ProjectDescription = model.ProjectDescription,
                Skills = model.Skills,
                ClientNameByCandidate = model.ClientNameByCandidate,
                ClientAddress = model.ClientAddress,
                AllowancesDisplay = model.Allowances.HasValue
                    ? model.Allowances.Value.ToString(new CultureInfo("de-DE"))
                    : null,
                Currency = model.Currency,
                Basic = model.Basic,
                WCStatus = model.WorkContractStatusId,
                SSN = model.SSN,
                Email = model.Email,
                HighlightedFields=model.HighlightedFields,
                LastCommentDate=model.LastCommentDate.HasValue? $"Dernières réactions de l'administrateur pour la révision ({model.LastCommentDate.Value.ToString("dd/MM/yyyy HH:mm")} UTC+0):" :"",
                FooterLastCommentDate= model.LastCommentDate.HasValue ?$"You last commented on this form on {model.LastCommentDate.Value.ToString("dd/MM/yyyy HH:mm")} (UTC+0)":"",
                DocumentId = model.DocumentId
            };
        }

        public static WorkContractDto ToWorkContractDto(this WorkContractViewModel model)
        {
            if (model == null)
                return null;
            return new WorkContractDto()
            {
                ContractId = model.ContractId,
                Type = model.Type,
                Title = model.Title,
                FirstName = model.FirstName,
                LastName = model.LastName,
                DateOfBirth = model.DateOfBirth,
                CountryOfBirth = model.CountryOfBirth,
                Nationality = model.Nationality,
                Address = model.Address,
                PostCode = model.PostCode,
                City = model.City,
                Country = model.Country,
                VisaPermitNo = model.VisaPermitNo,
                ContractStartDate = model.ContractStartDate,
                ContractEndDate = model.ContractEndDate,
                ProjectDescription = model.ProjectDescription,
                Skills = model.Skills,
                ClientNameByCandidate = model.ClientNameByCandidate,
                ClientAddress = model.ClientAddress,
                Allowances = model.Allowances,
                Currency = model.Currency,
                Basic = model.Basic,
                SSN = model.SSN,
                Email = model.Email,
                HighlightedFields=model.HighlightedFields
            };
        }
    }
}