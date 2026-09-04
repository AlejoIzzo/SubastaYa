using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SubastaYa.Application.DTOs;
using SubastaYa.Application.Exceptions;
using SubastaYa.Application.Interfaces;
using SubastaYa.Domain.Entities;

namespace SubastaYa.Application.Services
{
    public class CategoriaService : ICategoriaService
    {
        private readonly ICategoriaRepository _categoriaRepository;
        public CategoriaService(ICategoriaRepository categoriaRepository)
        {
            _categoriaRepository = categoriaRepository;
        }
        public Task<IEnumerable<CategoriaDTO>> GetAllCategorias()
        {
            return _categoriaRepository.GetAllCategoriasDTO();
        }
    }
}
