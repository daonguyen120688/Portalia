namespace Portalia.Core.Entity
{
    public class UserProfileAttribute : Repository.Pattern.Ef6.Entity
    {
        public int UserProfileAttributeId { get; set; }
        public string Value { get; set; }
        public int UserProfileId { get; set; }
        public int AttributeDetailId { get; set; }
        public virtual UserProfile UserProfile { get; set; }
        public virtual AttributeDetail AttributeDetail { get; set; }
        public byte[] FileBinary { get; set; }
    }
}
