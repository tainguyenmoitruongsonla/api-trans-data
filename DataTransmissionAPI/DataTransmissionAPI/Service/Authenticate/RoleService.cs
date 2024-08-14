using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;
using System.Data;

namespace DataTransmissionAPI.Service
{
    public class RoleService
    {
        private readonly RoleManager<AspNetRoles> _roleManager;
        private readonly Data.DatabaseContext _context;
        private readonly IMapper _mapper;

        public RoleService(IServiceProvider serviceProvider, Data.DatabaseContext context, IMapper mapper)
        {
            _roleManager = serviceProvider.GetRequiredService<RoleManager<AspNetRoles>>();
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<RoleDto>> GetAllRolesAsync()
        {
            var items = await _context.Roles
                .Where(u => u.IsDeleted == false)
                .ToListAsync();
            var listItems = _mapper.Map<List<RoleDto>>(items);

            foreach (var item in listItems)
            {
                await PopulateDataForRoleAsync(item);
            }

            return listItems;
        }

        public async Task<RoleDto> GetRoleByIdAsync(string roleId)
        {
            var item = await _roleManager.FindByIdAsync(roleId);
            var role = _mapper.Map<RoleDto>(item);
            await PopulateDataForRoleAsync(role);

            return role;
        }

        private async Task PopulateDataForRoleAsync(RoleDto roleDto)
        {
            var dashIds = _context!.RoleDashboards!.Where(x => x.RoleId == roleDto.Id).Select(x => x.DashboardId).ToList();
            if (dashIds.Any())
            {
                var dashboards = await _context.Dashboards.Where(x => x.IsDeleted == false).ToListAsync();
                dashboards = (List<Dashboards>)dashboards.Where(x => dashIds.Contains(x.Id)).ToList();
                roleDto.Dashboards = _mapper.Map<List<DashboardDto>>(dashboards);
                foreach (var dash in roleDto.Dashboards)
                {
                    var functions = await _context!.Functions!.Where(x => x.Id > 0).ToListAsync();
                    if (dash.Path.ToLower() != "user")
                        functions = functions.Where(x => x.PermitCode != "ASSIGNROLE"
                                                    && x.PermitCode != "RESETPASSWORD"
                                                    && x.PermitCode != "SETROLE"
                                                    && x.PermitCode != "ASSIGNFUNCTION"
                                                    ).ToList();
                    dash.Functions = _mapper.Map<List<FunctionDto>>(functions);
                    foreach (var function in dash.Functions)
                    {
                        var existingPermission = await _context.Permissions!.
                            FirstOrDefaultAsync(d => d.FunctionId == function.Id && d.DashboardId == dash.Id && d.RoleId == roleDto.Id);

                        if (existingPermission != null)
                        {
                            function.Status = true;
                        }
                        else
                        {
                            function.Status = false;
                        }
                    }
                }
            }
        }


        public async Task<bool> SaveRoleAsync(RoleDto dto)
        {
            var exitsRole = await _roleManager.FindByIdAsync(dto.Id);

            if (exitsRole == null)
            {
                // Create a new user
                AspNetRoles item = new AspNetRoles();
                if (dto.Name == item.Name) { return false; }
                item.Name = dto.Name;
                item.IsDefault = dto.IsDefault;
                item.IsDeleted = false;

                await _roleManager.CreateAsync(item);
                return true;
            }
            else
            {
                exitsRole.Name = dto.Name;
                exitsRole.IsDefault = dto.IsDefault;
                exitsRole.IsDeleted = false;
                await _roleManager.UpdateAsync(exitsRole);
                return true;
            }
        }

        public async Task<bool> DeleteRoleAsync(string roleId)
        {
            var item = await _roleManager.FindByIdAsync(roleId);

            if (item == null) { return false; }

            // Update role properties based on the RegisterViewModel
            item.IsDeleted = true;
            await _roleManager.UpdateAsync(item);
            return true;
        }
    }
}
