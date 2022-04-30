using Microsoft.EntityFrameworkCore;
using SportBooking.DAL.Entities;
using SportBooking.DAL.Interfaces;

namespace SportBooking.DAL.Repositories;

public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
{
    private readonly DataContext _context;
    private DbSet<T> _entities;

    public GenericRepository(DataContext context)
    {
        _context = context;
        _entities = context.Set<T>();
    }

    public async Task<List<T>> GetAllAsync() => await _entities.ToListAsync();

    public async Task<T?> GetByIdAsync(int id) => await _entities.SingleOrDefaultAsync(t => t.Id == id);

    public async Task InsertAsync(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException("Entity is null");
        }

        await _entities.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException("Entity is null");
        }

        _entities.Update(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(T entity)
    {
        if (entity is null)
        {
            throw new ArgumentNullException("Entity is null");
        }

        _entities.Remove(entity);
        await _context.SaveChangesAsync();
    }
}