using Microsoft.EntityFrameworkCore;
using MovieApi.Domain.Entities;

namespace MovieApi.Application.Interfaces;

public interface IApplicationDbContext
{
    DbSet<Movie> Movies { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}