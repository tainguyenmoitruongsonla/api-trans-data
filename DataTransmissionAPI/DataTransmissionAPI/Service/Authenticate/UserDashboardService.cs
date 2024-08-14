using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Service
{
    public class UserDashboardService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;

        public UserDashboardService(DatabaseContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<UserDashboardDto>> GetAllUserDashboardAsync()
        {
            var items = await _context.UserDashboards!.Where(x => x.Id > 0).ToListAsync();
            return _mapper.Map<List<UserDashboardDto>>(items);
        }

        public async Task<UserDashboardDto> GetUserDashboardByIdAsync(int Id)
        {
            var item = await _context!.UserDashboards!.FindAsync(Id);
            return _mapper.Map<UserDashboardDto>(item);
        }

        public async Task<bool> SaveUserDashboardAsync(UserDashboardDto dto)
        {
            var exitsItem = await _context!.UserDashboards!.FindAsync(dto.Id);

            if (exitsItem == null || dto.Id == 0)
            {
                var newItem = _mapper.Map<UserDashboards>(dto);

                _context.UserDashboards!.Add(newItem);
            }
            else
            {
                var updateItem = await _context.UserDashboards!.FirstOrDefaultAsync(d => d.Id == dto.Id);

                updateItem = _mapper.Map(dto, updateItem);
                _context.UserDashboards!.Update(updateItem!);
            }

            await _context.SaveChangesAsync();
            return true;
        }


        public async Task<bool> DeleteUserDashboardAsync(UserDashboardDto dto)
        {
            var exitsItem = await _context!.UserDashboards!.FindAsync(dto.Id);

            if (exitsItem == null) { return false; }

            _context.UserDashboards.Remove(exitsItem);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
