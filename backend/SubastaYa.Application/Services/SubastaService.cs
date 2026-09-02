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

        public async Task<IEnumerable<SubastaDTO>> GetAllAsync()
        {
            var subastas =  await _subastaRepository.GetAllAsync();

            if (subastas == null)
                return null;

            return subastas.Select(s => new SubastaDTO
            {
                Id = s.Id,
                Titulo = s.Titulo,
                Descripcion = s.Descripcion,
                PrecioBase = s.PrecioBase,
                UrlImagen = s.UrlImagen,
                Estado = s.Estado,
                FechaInicio = s.FechaInicio,
                FechaFin = s.FechaFin
            }).ToList();
        }   

        public Task<SubastaDTO?> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}
