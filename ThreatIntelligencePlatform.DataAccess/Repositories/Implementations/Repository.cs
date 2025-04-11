using Microsoft.EntityFrameworkCore;
using ThreatIntelligencePlatform.Business.Interfaces.Infrastructure;
using ThreatIntelligencePlatform.DataAccess.Data;

namespace ThreatIntelligencePlatform.DataAccess.Repositories.Implementations;

public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync()
    {
        return await _context.Set<TEntity>().ToListAsync();
    }

    public async Task<TEntity?> GetByIdAsync(Guid id)
    {
        return await _context.Set<TEntity>().FindAsync(id);
    }

    public async Task<TEntity> AddAsync(TEntity entity)
    {
        var result = await _context.Set<TEntity>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        var result = _context.Set<TEntity>().Update(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        var result = _context.Set<TEntity>().Remove(entity);
        await _context.SaveChangesAsync();
        return result.Entity;
    }
}