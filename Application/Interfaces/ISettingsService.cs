using Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ISettingsService
    {
        Task<TenantSettingsDto> CreateAsync(TenantSettingsDto createDto);
        Task<TenantSettingsDto> GetByIdAsync(int Id);
        Task<TenantSettingsDto> UpdateAsync(int Id, TenantSettingsDto editDto);
    }
}
