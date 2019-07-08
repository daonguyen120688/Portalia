using System.Collections.Generic;
using Portalia.Core.Enum;

namespace Portalia.ViewModels
{
    public class UploadFile
    {
        public string UserId { get; set; }
        public FolderType FolderType { get; set; }
        public List<byte> FileContent { get; set; }
        public string FileName { get; set; }
    }
}