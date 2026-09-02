using SubastaYa.Application.DTOs;
using SubastaYa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubastaYa.Application.Services
{
    public interface ISubastaService
    {
        Task<Subasta> CreateAsync(SubastaDTO subasta);
        Task<SubastaDTO?> GetByIdAsync(int id);
        Task<IEnumerable<SubastaDTO>> GetAllAsync();
    }
}
