using System.ComponentModel.DataAnnotations;
using System.Web;

namespace Portalia.ViewModels.WorkContracts
{
    public class UploadWorkContractViewModel
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        [Range(1, 1)]
        public int FolderType { get; set; }

        [Required]
        public HttpPostedFileBase Contract { get; set; }
    }
}