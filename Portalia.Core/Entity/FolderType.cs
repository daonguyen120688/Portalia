namespace Portalia.Core.Entity
{
    public class FolderTypeEntity: Repository.Pattern.Ef6.Entity
    {
        public int FolderTypeId { get; set; }
        public string Label { get; set; }
    }
}
