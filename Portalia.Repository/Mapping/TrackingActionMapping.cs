using System.Data.Entity.ModelConfiguration;
using Portalia.Core.Entity;

namespace Portalia.Repository.Mapping
{
    public class TrackingActionMapping : EntityTypeConfiguration<TrackingAction>
    {
        public TrackingActionMapping()
        {
            ToTable("dbo.TrackingAction");
            HasKey(x => x.TrackingActionId);
        }
    }
}
