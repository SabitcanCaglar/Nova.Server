using Base.Domain.Entities.Identity;
using Microsoft.EntityFrameworkCore;

namespace Base.Application.Common.Interfaces;

public interface IApplicationDbContext
{
    DbSet<User> Users { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}
