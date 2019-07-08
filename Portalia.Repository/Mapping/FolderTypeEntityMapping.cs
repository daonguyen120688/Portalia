using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class FolderTypeEntityMapping: EntityTypeConfiguration<FolderTypeEntity>
    {
        public FolderTypeEntityMapping()
        {
            ToTable("dbo.FolderType");
            HasKey(c => c.FolderTypeId);
        }
    }
}
