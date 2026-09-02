using Microsoft.EntityFrameworkCore;
using SubastaYa.Application.Interfaces;
using SubastaYa.Data;
using SubastaYa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Infrastructure.Repositories
{
    public class SubastaRepository : ISubastaRepository
    {
        private readonly SubastaYaContext _context;

        public SubastaRepository(SubastaYaContext context)
        {
            _context = context;
        }

        public Task AddAsync(Subasta subasta)
        {
            throw new NotImplementedException();
        }

        public void Delete(Subasta subasta)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Subasta>> GetAllAsync()
        {
            return await _context.Subastas.ToListAsync(); 
        }

        public Task<Subasta?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Subasta subasta)
        {
            throw new NotImplementedException();
        }
    }
}
