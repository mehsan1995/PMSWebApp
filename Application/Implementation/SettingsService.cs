using Application.DTOs;
using Application.Interfaces;
using AutoMapper;
using DAL;
using DAL.GenericRepository;
using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Implementation
{
    public class SettingsService : ISettingsService
    {
        private readonly IGenericRepository<TenantSettings> _tenantSettingsRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public SettingsService(
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _tenantSettingsRepository = new GenericRepository<TenantSettings>(applicationDbContext);
            _mapper = mapper;
        }


        public async Task<TenantSettingsDto> CreateAsync(TenantSettingsDto createDto)
        {
            try
            {
                var settings = _mapper.Map<TenantSettings>(createDto);
                return _mapper.Map<TenantSettingsDto>(await _tenantSettingsRepository.AddAsync(settings));

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<TenantSettingsDto> GetByIdAsync(int Id)
        {
            var record = await _tenantSettingsRepository.FindAsync(x => x.TenantId == Id);
            return _mapper.Map<TenantSettingsDto>(record.FirstOrDefault());
        }

        public async Task<TenantSettingsDto> UpdateAsync(int Id, TenantSettingsDto editDto)
        {
            var existingRecord = await _tenantSettingsRepository.GetByIdAsync(Id);
            if (existingRecord == null)
                throw new Exception("Settings not found");

            _mapper.Map(editDto, existingRecord);
            await _tenantSettingsRepository.UpdateAsync(existingRecord);

            return _mapper.Map<TenantSettingsDto>(existingRecord);
        }
    }
}
