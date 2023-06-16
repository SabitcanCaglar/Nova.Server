
using Base.Domain.Common;

namespace Base.Domain.Entities.Identity;

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