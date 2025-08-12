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
        Task<SettingsDto> CreateAsync(SettingsDto createDto);
        Task<SettingsDto> GetByIdAsync(string Id);
        Task<SettingsDto> UpdateAsync(int Id, SettingsDto editDto);
        Task<bool> Update(string Id, string language);
    }
}
