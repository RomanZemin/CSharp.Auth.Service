﻿using Auth.Application.Interfaces.Persistence;
using Auth.Infrastructure.Persistence.Data;

namespace Auth.Infrastructure.Persistence.Repositories
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly AppDbContext _context;
        private readonly Dictionary<Type, object> repositories;

        public UnitOfWork(AppDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            repositories = new Dictionary<Type, object>();
        }

        public IRepositoryBase<T> GetRepository<T>() where T : class
        {
            if (repositories.TryGetValue(typeof(T), out var repo))
            {
                // Use a safe cast and ensure type correctness
                return (IRepositoryBase<T>)repo;
            }

            // Create and add the repository if not found
            IRepositoryBase<T> newRepo = new RepositoryBase<T>(_context);
            repositories.Add(typeof(T), newRepo);
            return newRepo;
        }

        public async Task<bool> CompleteAsync()
        {
            if (_context == null)
            {
                throw new ObjectDisposedException(nameof(AppDbContext), "Context is already disposed");
            }

            return await _context.SaveChangesAsync() > 0;
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
