

using Base.Domain.Common;

namespace Base.Domain.Entities.Identity;

public class EmailAuthenticator : BaseEntity
{
    public Guid UserId { get; set; }
    public string? ActivationKey { get; set; }
    public bool IsVerified { get; set; }

    public virtual User User { get; set; }

    public EmailAuthenticator()
    {
    }

    public EmailAuthenticator(int id, Guid userId, string? activationKey, bool isVerified) : this()
    {
        Id = id;
        UserId = userId;
        ActivationKey = activationKey;
        IsVerified = isVerified;
    }
}