using System;
using System.Collections.Generic;
using System.Linq;
using Portalia.Core.Dtos;

namespace Portalia.ViewModels.WorkContracts
{
    public class WorkContractApiModel
    {
        public WorkContractApiModel(WorkContractDto dto = null)
        {
            if (dto != null)
            {
                ContractId = dto.ContractId;
                FirstName = dto.FirstName;
                LastName = dto.LastName;
                Email = dto.Email;
                Currency = dto.Currency;
                Basic = dto.Basic;
                CreatedDate = dto.CreatedDate;
                UpdatedDate = dto.UpdatedDate;

                // Data fields
                if (dto.DataFields != null && dto.DataFields.Any())
                {
                    DataFields = dto.DataFields.Select(d => new DataFieldViewModel
                    {
                        FieldId = d.FieldId,
                        Value = d.Value
                    });
                }
            }
        }

        public int ContractId { get; set; }
        
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public string Currency { get; set; }

        public string Basic { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? UpdatedDate { get; set; }

        public IEnumerable<DataFieldViewModel> DataFields { get; set; }
    }
}