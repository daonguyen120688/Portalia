using System;
using System.Collections.Generic;

namespace Portalia.Core.Entity
{
    public class AspNetUser : Repository.Pattern.Ef6.Entity
    {
        public AspNetUser()
        {
            AspNetUserClaims = new HashSet<AspNetUserClaim>();
            AspNetUserLogins = new HashSet<AspNetUserLogin>();
            AspNetRoles = new HashSet<AspNetRole>();
            WorkContracts = new HashSet<WorkContract>();
            IsActive = true;
        }

        public string Id { get; set; }
        public string Email { get; set; }
        public bool EmailConfirmed { get; set; }
        public string PasswordHash { get; set; }
        public string SecurityStamp { get; set; }
        public string PhoneNumber { get; set; }
        public bool PhoneNumberConfirmed { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public DateTime? LockoutEndDateUtc { get; set; }
        public bool LockoutEnabled { get; set; }
        public int AccessFailedCount { get; set; }
        public string UserName { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public virtual ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual ICollection<AspNetRole> AspNetRoles { get; set; }
        public virtual ICollection<WorkContract> WorkContracts { get; set; }
        public string FullName => $"{LastName} {FirstName}";
        public bool IsActive { get; set; }
        public DateTime Created { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsNew { get; set; }
        public bool HasChangedPassword { get; set; }
        public bool CanSeeWelcomeCards { get; set; }
        public string EmployeeStatus { get; set; }
    }

    public class AspNetUserExtraInfo
    {
        public int PercentOfInfo { get; set; }
        public string FullName { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmployee { get; set; }
        public bool IsNew { get; set; }
        public string Email { get; set; }
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public int PortaliaUserProfileId { get; set; }
        public int Total { get; set; }
    }
}


