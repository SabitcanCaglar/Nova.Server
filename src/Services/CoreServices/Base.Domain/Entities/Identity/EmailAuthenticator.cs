

using Base.Domain.Common;

namespace Base.Domain.Entities.Identity;

public class EmailAuthenticator : BaseEntity
{
    public string UserId { get; set; }
    public string? ActivationKey { get; set; }
    public bool IsVerified { get; set; }

    public virtual User User { get; set; }

    public EmailAuthenticator()
    {
    }

    public EmailAuthenticator(int id, string userId, string? activationKey, bool isVerified) : this()
    {
        Id = id;
        UserId = userId;
        ActivationKey = activationKey;
        IsVerified = isVerified;
    }
}