using Microsoft.EntityFrameworkCore;

namespace Base.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    //DbSet<ENTITY> TodoLists { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
