using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;
using System.Security.Claims;

namespace DataTransmissionAPI.Service
{
    public class DashboardService
    {
        private readonly DatabaseContext _context;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContext;

        public DashboardService(DatabaseContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _context = context;
            _mapper = mapper;
            _httpContext = httpContext;
        }

        public async Task<List<DashboardDto>> GetAllDashboardAsync()
        {
            var dashboards = await _context!.Dashboards!.OrderBy(x => x.Name).ToListAsync();
            var dashboardModels = _mapper.Map<List<DashboardDto>>(dashboards);

            var allFunctions = await _context!.Functions!.ToListAsync();
            foreach (var dashboardModel in dashboardModels)
            {
                dashboardModel.Functions = _mapper.Map<List<FunctionDto>>(allFunctions);
            }

            return dashboardModels;
        }

        public async Task<List<RoleDashboardDto>> GetDashboardByRoleAsync(string roleName)
        {
            var role = await _context!.Roles!.FirstOrDefaultAsync(x => x!.Name!.ToLower() == roleName.ToLower());
            var dashboards = await _context!.Dashboards!.Where(x => x.IsDeleted == false).ToListAsync();
            var roleDashboards = new List<RoleDashboardDto>();

            foreach (var dashboard in dashboards)
            {
                var rdash = await _context!.RoleDashboards!
                    .FirstOrDefaultAsync(x => x.RoleName == roleName && x.DashboardId == dashboard.Id);

                var dto = new RoleDashboardDto
                {
                    DashboardId = dashboard.Id,
                    DashboardName = dashboard.Name,
                    FileControl = dashboard.Path,
                    RoleId = role?.Id
                };

                if (rdash != null)
                {
                    dto.Id = rdash.Id;
                    dto.RoleId = rdash.RoleId;
                    dto.RoleName = rdash.RoleName;
                    dto.PermitAccess = (bool)rdash.PermitAccess!;
                }
                else
                {
                    dto.RoleId = role?.Id;
                    dto.RoleName = role?.Name;
                    dto.PermitAccess = false;
                }

                roleDashboards.Add(dto);
            }

            return _mapper.Map<List<RoleDashboardDto>>(roleDashboards);
        }


        public async Task<List<UserDashboardDto>> GetDashboardByUserAsync(string userName)
        {
            var user = await _context!.Users!.FirstOrDefaultAsync(x => x!.UserName!.ToLower() == userName.ToLower());
            var dashboards = await _context!.Dashboards!.Where(x => x.IsDeleted == false).ToListAsync();
            var userDashboards = new List<UserDashboardDto>();

            foreach (var dashboard in dashboards)
            {
                var udash = await _context!.UserDashboards!
                    .FirstOrDefaultAsync(x => x.UserName == userName && x.DashboardId == dashboard.Id);

                var dto = new UserDashboardDto
                {
                    DashboardId = dashboard.Id,
                    DashboardName = dashboard.Name,
                    FileControl = dashboard.Path,
                    Description = dashboard.Description,
                    UserId = user?.Id
                };

                if (udash != null)
                {
                    dto.Id = udash.Id;
                    dto.UserId = udash.UserId;
                    dto.UserName = udash.UserName;
                    dto.PermitAccess = (bool)udash.PermitAccess!;
                }
                else
                {
                    dto.UserId = user?.Id;
                    dto.UserName = user?.UserName;
                    dto.PermitAccess = false;
                }

                userDashboards.Add(dto);
            }

            return _mapper.Map<List<UserDashboardDto>>(userDashboards);
        }

        public async Task<DashboardDto> GetDashboardByIdAsync(int Id)
        {
            var item = await _context!.Dashboards!.FindAsync(Id);
            var dash = _mapper.Map<DashboardDto>(item);
            var functions = await _context!.Functions!.Where(x => x.Id > 0).ToListAsync();
            dash.Functions = _mapper.Map<List<FunctionDto>>(functions);

            return dash;
        }


        public async Task<bool> SaveDashboardAsync(DashboardDto dto)
        {
            var existingItem = await _context.Dashboards!.FirstOrDefaultAsync(d => d.Id == dto.Id);

            if (existingItem == null || dto.Id == 0)
            {
                var newItem = _mapper.Map<Dashboards>(dto);
                newItem.IsDeleted = false;
                newItem.CreatedTime = DateTime.Now;
                newItem.CreatedUser = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? null;
                _context.Dashboards!.Add(newItem);
            }
            else
            {
                var updateItem = await _context.Dashboards!.FirstOrDefaultAsync(d => d.Id == dto.Id);

                updateItem = _mapper.Map(dto, updateItem);

                updateItem!.ModifiedTime = DateTime.Now;
                updateItem.ModifiedUser = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? null;
                _context.Dashboards!.Update(updateItem);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteDashboardAsync(int Id)
        {
            var existingItem = await _context.Dashboards!.FirstOrDefaultAsync(d => d.Id == Id);

            if (existingItem == null) { return false; }

            existingItem!.IsDeleted = true;
            _context.Dashboards!.Update(existingItem);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
