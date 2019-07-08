using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Portalia.Core.Enum;

namespace Portalia.ViewModels.Users
{
    public class PagingUserViewModel
    {
        public int TotalPages { get; set; }
        public int CurrentPage { get; set; }
        public IList<PagingUserItemViewModel> Users { get; set; }
        public IEnumerable<SelectListItem> WorkContractStatuses { get; set; }

        [Display(Name = "Filter by WC form status")]
        public WorkContractStatusEnum SelectedWorkContractStatus { get; set; }
    }
}