using System;

namespace Portalia.Core.Entity
{
    public class TrackingAction : Repository.Pattern.Ef6.Entity
    {
        public Guid TrackingActionId { get; set; }
        public string Data { get; set; }
    }
}
