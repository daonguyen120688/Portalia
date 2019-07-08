using System.IO;
using System.Web;

namespace Portalia.Extentions
{
    public static class FileExtension
    {
        public static string GetFileNameWithoutSpaceBetweenWords(this HttpPostedFileBase file)
        {
            return file?.FileName.Replace(" ", string.Empty);
        }

        public static byte[] GetFileBinary(this HttpPostedFileBase file)
        {
            var target = new MemoryStream();
            file.InputStream.CopyTo(target);
            return target.ToArray();
        }
    }
}