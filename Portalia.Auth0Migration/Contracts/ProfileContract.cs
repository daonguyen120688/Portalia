namespace Portalia.Auth0Migration.Contracts
{
    public class ProfileContract
    {
        public string UserId { get; }
        public string UserName { get; }
        public string FirstName { get; }
        public string LastName { get; }
        public string PasswordHash { get; }

        public ProfileContract(string userId, string userName, string firstName, string lastName, string passwordHash)
        {
            UserId = userId;
            UserName = userName;
            FirstName = firstName;
            LastName = lastName;
            PasswordHash = passwordHash;
        }
    }
}
