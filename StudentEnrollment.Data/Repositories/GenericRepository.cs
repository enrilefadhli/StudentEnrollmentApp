﻿using Microsoft.EntityFrameworkCore;
using StudentEnrollment.Data.Contracts;

namespace StudentEnrollment.Data.Repositories
{
    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly StudentEnrollmentDbContext _db;
        public GenericRepository(StudentEnrollmentDbContext db)
        {
            this._db = db ?? throw new ArgumentNullException(nameof(db));
        }
        public async Task<TEntity> GetAsync(int? id)
        {
            var result = await _db.Set<TEntity>().FindAsync(id) 
                ?? throw new KeyNotFoundException($"Entity of type {typeof(TEntity).Name} with ID {id} not found.");
            return result;
        }
        public async Task<List<TEntity>> GetAllAsync()
        {
            return await _db.Set<TEntity>().ToListAsync();
        }
        public async Task<TEntity> AddAsync(TEntity entity)
        {
            await _db.AddAsync(entity);
            await _db.SaveChangesAsync();
            return entity;
        }
        public async Task UpdateAsync(TEntity entity)
        {
            _db.Update(entity);
            await _db.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var entity = await GetAsync(id);
            _db.Set<TEntity>().Remove(entity);
            await _db.SaveChangesAsync();
        }
        public async Task<bool> ExistsAsync(int id)
        {
            return await _db.Set<TEntity>().AnyAsync(e => e.Id == id);
        }
    }
}
