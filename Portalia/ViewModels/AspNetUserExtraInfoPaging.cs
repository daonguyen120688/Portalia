using System.Collections.Generic;
using Portalia.Core.Entity;
using Portalia.Extentions;

namespace Portalia.ViewModels
{
    public class AspNetUserExtraInfoPaging
    {
        public PagingHeader Paging { get; set; }
        public List<AspNetUserExtraInfo> AspNetUserExtraInfos { get; set; }
    }
}