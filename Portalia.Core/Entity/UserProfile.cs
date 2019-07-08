using System;
using System.Collections.Generic;

namespace Portalia.Core.Entity
{
    public class UserProfile: Repository.Pattern.Ef6.Entity
    {
        public UserProfile()
        {
            UserProfileAttributes = new List<UserProfileAttribute>();
        }
        public int UserProfileId { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string IdentityUserId { get; set; }
        public virtual ICollection<UserProfileAttribute> UserProfileAttributes { get; set; }
        public byte[] PictureFileBinary { get; set; }
        public string PictureName { get; set; }
        public string Location { get; set; }
    }
}
