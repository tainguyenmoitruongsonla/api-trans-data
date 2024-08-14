using AutoMapper;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Service
{
    public class PermissionService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public PermissionService(DatabaseContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<List<PermissionDto>> GetAllPermissionAsync()
        {
            var items = await _context.Permissions!.Where(x => x.Id > 0).ToListAsync();
            return _mapper.Map<List<PermissionDto>>(items);
        }

        public async Task<PermissionDto> GetPermissionByIdAsync(int Id)
        {
            var item = await _context.Permissions!.FindAsync(Id);
            return _mapper.Map<PermissionDto>(item);
        }

        public async Task<bool> SavePermissionAsync(PermissionDto dto)
        {
            var existingItem = await _context.Permissions!.FirstOrDefaultAsync(d => d.Id == dto.Id);

            if (existingItem == null || dto.Id == 0)
            {
                var newItem = _mapper.Map<Permissions>(dto);
                _context.Permissions!.Add(newItem);
            }
            else
            {
                var updateItem = await _context.Permissions!.FirstOrDefaultAsync(d => d.Id == dto.Id);

                updateItem = _mapper.Map(dto, updateItem);

                _context.Permissions!.Update(updateItem!);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeletePermissionAsync(PermissionDto dto)
        {
            if (dto.RoleId != null)
            {
                var existingItem = await _context.Permissions!.FirstOrDefaultAsync(d => d.FunctionId == dto.FunctionId && d.DashboardId == dto.DashboardId && d.RoleId == dto.RoleId);

                if (existingItem == null) { return false; }

                _context.Permissions!.Remove(existingItem);
            }
            else if (dto.UserId != null)
            {
                var existingItem = await _context.Permissions!.FirstOrDefaultAsync(d => d.FunctionId == dto.FunctionId && d.DashboardId == dto.DashboardId && d.UserId == dto.UserId);

                if (existingItem == null) { return false; }

                _context.Permissions!.Remove(existingItem);
            }
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
