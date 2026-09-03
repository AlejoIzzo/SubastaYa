using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
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

        public async Task<SubastaCreadaDTO> CrearAsync(CrearSubastaDTO dto)
        {
            DateTime now = DateTime.UtcNow;

            // si no se envía FechaInicio, asumir activa y empezar ahora
            DateTime fechaInicio = dto.FechaInicio ?? now;
            string estado = fechaInicio <= now 
                ? "ACTIVA" 
                : "PROGRAMADA";

            if (fechaInicio < now)
                throw new DominioException("FechaInicio debe estar en el futuro");

            if (dto.FechaFin <= now)
                throw new DominioException("FechaFin debe estar en el futuro");

            if (dto.FechaFin <= fechaInicio)
                throw new DominioException("FechaFin debe ser posterior a FechaInicio");
            
            if (dto.FechaFin - fechaInicio < TimeSpan.FromMinutes(30))
                throw new DominioException("La subasta debe durar un mínimo de 30 minutos");

            if (dto.IncrementoMinimo < 1000)
                throw new DominioException("IncrementoMinimo debe ser mayor o igual a 5000");

            if (dto.PrecioBase < 0)
                throw new DominioException("PrecioBase debe ser mayor a 0");


            var subasta = new Subasta
            {
                Titulo = dto.Titulo,
                Descripcion = dto.Descripcion,
                CategoriaId = dto.CategoriaId,
                PrecioBase = dto.PrecioBase,
                UrlImagen = dto.UrlImagen,
                FechaFin = dto.FechaFin,
                VendedorId = dto.VendedorId,
                IncrementoMinimo = dto.IncrementoMinimo,

                FechaInicio = fechaInicio,
                Estado = estado,
            };

            await _subastaRepository.CrearAsync(subasta);

            return new SubastaCreadaDTO
            {
                Id = subasta.Id, // EF agrega los campos generados por SQLServer automaticamente
                Titulo = subasta.Titulo
            };
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
