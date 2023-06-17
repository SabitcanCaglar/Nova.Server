using Base.Domain.Common;
using Base.Domain.Enums.Identity;
using Microsoft.AspNetCore.Identity;

namespace Base.Domain.Entities.Identity;

public class User : IdentityUser,IEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public byte[] PasswordSalt { get; set; }
    public new byte[] PasswordHash { get; set; }
    public bool Status { get; set; }
    public AuthenticatorType AuthenticatorType { get; set; }

    public virtual ICollection<UserOperationClaim> UserOperationClaims { get; set; }
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; }

    public User()
    {
        UserOperationClaims = new HashSet<UserOperationClaim>();
        RefreshTokens = new HashSet<RefreshToken>();
    }

    public User(string firstName, string lastName, string email, byte[] passwordSalt, byte[] passwordHash,
                bool status, AuthenticatorType authenticatorType) : this()
    {
        FirstName = firstName;
        LastName = lastName;
        Email = email;
        PasswordSalt = passwordSalt;
        PasswordHash = passwordHash;
        Status = status;
        AuthenticatorType = authenticatorType;
        UserName = $"{firstName.ToLower()}{lastName.ToLower()}";
    }
}