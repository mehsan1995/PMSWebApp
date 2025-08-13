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
    public class ProjectService : IProjectService
    {
        private readonly IGenericRepository<Projects> _projectRepository;
        private readonly IGenericRepository<ProjectInformations> _ProjectInformation;
        private readonly IGenericRepository<Documents> _projectDocument;
        private readonly IMapper _mapper;
        private readonly ApplicationDbContext _dbContext;
        public ProjectService(
            IMapper mapper,
            ApplicationDbContext applicationDbContext)
        {
            _projectRepository = new GenericRepository<Projects>(applicationDbContext);
            _ProjectInformation = new GenericRepository<ProjectInformations>(applicationDbContext);
            _projectDocument = new GenericRepository<Documents>(applicationDbContext);
            _mapper = mapper;
            _dbContext = applicationDbContext;
        }
        public async Task<ProjectsDto> CreateAsync(ProjectsDto createDto)
        {
            try
            {
                var record = _mapper.Map<Projects>(createDto);
                await _projectRepository.AddAsync(record);
                return _mapper.Map<ProjectsDto>(record);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ProjectInformationsDto> CreateProjectInformationAsync(ProjectInformationsDto createDto)
        {
            try
            {
                var record = _mapper.Map<ProjectInformations>(createDto);
                await _ProjectInformation.AddAsync(record);
                return _mapper.Map<ProjectInformationsDto>(record);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task DeleteAsync(int Id)
        {
            try
            {
                var record = await _projectRepository.GetByIdAsync(Id);
                _projectRepository.Delete(record);
            }
            catch (Exception)
            {

                throw;
            }
            
        }

        public async Task DeleteProjectInformationAsync(int Id)
        {
            try
            {
                var record = await _ProjectInformation.GetByIdAsync(Id);
                _ProjectInformation.Delete(record);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ProjectsDto> GetByIdAsync(int Id)
        {
            try
            {
                var record = await _projectRepository.GetByIdAsync(Id);
                return _mapper.Map<ProjectsDto>(record);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task<(List<ProjectsDto> Data, int TotalCount)> GetPaginatedRolesAsync(int start, int length, string searchValue, string sortColumn, string sortDirection)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectInformationsDto> GetProjectInformationByIdAsync(int Id)
        {
            try
            {
                var record = await _ProjectInformation.GetByIdAsync(Id);
                return _mapper.Map<ProjectInformationsDto>(record);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Task ToggleStatusAsync(int Id, bool status)
        {
            throw new NotImplementedException();
        }

        public async Task<ProjectsDto> UpdateAsync(int Id, ProjectsDto editDto)
        {
            try
            {
                var existingRecord = await _projectRepository.GetByIdAsync(Id);
                if (existingRecord == null)
                    throw new Exception("Project not found");


                _mapper.Map(editDto, existingRecord);
                await _projectRepository.UpdateAsync(existingRecord);

                return _mapper.Map<ProjectsDto>(existingRecord);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<ProjectInformationsDto> UpdateProjectInformationAsync(int Id, ProjectInformationsDto editDto)
        {
            try
            {
                var existingRecord = await _ProjectInformation.GetByIdAsync(Id);
                if (existingRecord == null)
                    throw new Exception("Project not found");


                _mapper.Map(editDto, existingRecord);
                await _ProjectInformation.UpdateAsync(existingRecord);

                return _mapper.Map<ProjectInformationsDto>(existingRecord);
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<DocumentsDto> UploadDocuments(DocumentsDto createDto)
        {
            try
            {
                var record = _mapper.Map<Documents>(createDto);
                await _projectDocument.AddAsync(record);
                return _mapper.Map<DocumentsDto>(record);
            }
            catch (Exception)
            {

                throw;
            }
          
        }
    }
}
