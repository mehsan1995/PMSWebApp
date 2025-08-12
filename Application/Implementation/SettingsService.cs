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
        private readonly IGenericRepository<Settings> _tenantSettingsRepository;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public SettingsService(
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _tenantSettingsRepository = new GenericRepository<Settings>(applicationDbContext);
            _mapper = mapper;
        }


        public async Task<SettingsDto> CreateAsync(SettingsDto createDto)
        {
            try
            {
                var settings = _mapper.Map<Settings>(createDto);
                return _mapper.Map<SettingsDto>(await _tenantSettingsRepository.AddAsync(settings));

            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<SettingsDto> GetByIdAsync(string Id)
        {
            var record = await _tenantSettingsRepository.FindAsyncAsNoTracking(x => x.UserId == Id);
            return _mapper.Map<SettingsDto>(record.FirstOrDefault());
        }

        public async Task<bool> Update(string Id, string language)
        {
            var record = await _tenantSettingsRepository.FindAsyncAsNoTracking(x => x.UserId == Id);
            var setting = record.FirstOrDefault();
            if (setting != null)
            {
                setting.Language = language;
                await _tenantSettingsRepository.UpdateAsync(setting);
            }
            return true;
        }

        public async Task<SettingsDto> UpdateAsync(int Id, SettingsDto editDto)
        {
            var existingRecord = await _tenantSettingsRepository.GetByIdAsync(Id);
            if (existingRecord == null)
                throw new Exception("Settings not found");

            _mapper.Map(editDto, existingRecord);
            await _tenantSettingsRepository.UpdateAsync(existingRecord);

            return _mapper.Map<SettingsDto>(existingRecord);
        }
    }
}
