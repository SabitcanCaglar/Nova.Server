using System.Linq.Expressions;
using Base.Application.Common.Paging;
using Base.Domain.Common;
using Core.Persistence.Repositories;
using Microsoft.EntityFrameworkCore.Query;

namespace Base.Application.Repositories;

public interface IAsyncRepository<T> : IQuery<T> where T : IEntity
{
    Task<T?> GetAsync(Expression<Func<T, bool>> predicate);

    Task<IPaginate<T>> GetListAsync(Expression<Func<T, bool>>? predicate = null,
                                    Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
                                    Func<IQueryable<T>, IIncludableQueryable<T, object>>? include = null,
                                    int index = 0, int size = 10, bool enableTracking = true,
                                    CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity);
    Task<T> UpdateAsync(T entity);
    Task<T> DeleteAsync(T entity);
}