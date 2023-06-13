
using Base.Domain.Common;

namespace IdentityService.Api.Domain.Entities;

public class OperationClaim : BaseEntity
{
    public string Name { get; set; }

    public OperationClaim()
    {
    }

    public OperationClaim(int id, string name) : base(id)
    {
        Name = name;
    }
}