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

            // Si no se envía FechaInicio, asumir que la subasta inicia inmediatamente como ACTIVA
            DateTime fechaInicio = dto.FechaInicio ?? now;
            string estado = fechaInicio <= now ? "ACTIVA" : "PROGRAMADA";

            if (dto.FechaInicio.HasValue && dto.FechaInicio.Value < now.AddMinutes(-5))
                throw new DominioException("FechaInicio no puede ser anterior a la fecha y hora actual");

            if (dto.FechaFin <= now)
                throw new DominioException("FechaFin debe estar en el futuro");

            if (dto.FechaFin <= fechaInicio)
                throw new DominioException("FechaFin debe ser posterior a FechaInicio");

            if (dto.IncrementoMinimo <= 0)
                throw new DominioException("IncrementoMinimo debe ser mayor a 0");

            if (dto.PrecioBase <= 0)
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

        public async Task<IEnumerable<SubastaCatalogDTO>> GetSubastaCatalog(SubastaFiltroDTO? filtro = null)
        {
            return await _subastaRepository.GetSubastaCatalog(filtro);
        }

        public async Task<SubastaDetalleDTO?> GetByIdAsync(int id, int ultimasPujasLimit = 5)
        {
            return await _subastaRepository.GetByIdAsync(id, ultimasPujasLimit);
        }
    }
}
