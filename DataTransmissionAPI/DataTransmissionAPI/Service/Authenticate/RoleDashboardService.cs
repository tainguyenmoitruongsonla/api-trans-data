using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Service
{
    public class RoleDashboardService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public RoleDashboardService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RoleDashboardDto>> GetAllRoleDashboardAsync()
        {
            var items = await _context.RoleDashboards!.Where(x => x.Id > 0).ToListAsync();
            return _mapper.Map<List<RoleDashboardDto>>(items);
        }

        public async Task<RoleDashboardDto> GetRoleDashboardByIdAsync(int Id)
        {
            var item = await _context!.RoleDashboards!.FindAsync(Id);
            return _mapper.Map<RoleDashboardDto>(item);
        }

        public async Task<bool> SaveRoleDashboardAsync(RoleDashboardDto dto)
        {
            var exitsItem = await _context!.RoleDashboards!.FindAsync(dto.Id);

            if (exitsItem == null || dto.Id == 0)
            {
                var newItem = _mapper.Map<RoleDashboards>(dto);

                _context.RoleDashboards!.Add(newItem);
            }
            else
            {
                var updateItem = await _context.RoleDashboards!.FirstOrDefaultAsync(d => d.Id == dto.Id);

                updateItem = _mapper.Map(dto, updateItem);
                _context.RoleDashboards!.Update(updateItem!);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteRoleDashboardAsync(RoleDashboardDto dto)
        {
            var exitsItem = await _context!.RoleDashboards!.FindAsync(dto.Id);

            if (exitsItem == null) { return false; }

            _context.RoleDashboards.Remove(exitsItem);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
