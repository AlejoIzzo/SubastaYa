using SubastaYa.Application.DTOs;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Services
{
    public class SubastaService : ISubastaService
    {
        private readonly ISubastaRepository _subastaRepository;
        public SubastaService(ISubastaRepository subastaRepository)
        {
            _subastaRepository = subastaRepository;
        }

        public Task<Subasta> CreateAsync(SubastaDTO subasta)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SubastaCatalogDTO>> GetSubastaCatalog()
        {
            return await _subastaRepository.GetSubastaCatalog();
        }

        public async Task<SubastaDetalleDTO?> GetByIdAsync(int id, int ultimasPujasLimit)
        {
            return await _subastaRepository.GetByIdAsync(id, ultimasPujasLimit);
        }
    }
}
