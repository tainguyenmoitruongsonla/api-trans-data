using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using DataTransmissionAPI.Data;
using DataTransmissionAPI.Dto;
using System.Data;
using System.Security.Claims;

namespace DataTransmissionAPI.Service
{
    public class UserService
    {
        private readonly IMapper _mapper;
        private readonly Data.DatabaseContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly RoleManager<AspNetRoles> _roleManager;
        private readonly IHttpContextAccessor _httpContext;


        public UserService(Data.DatabaseContext context, UserManager<AspNetUsers> userManager, RoleManager<AspNetRoles> roleManager, IMapper mapper, IHttpContextAccessor httpContext)
        {
            _mapper = mapper;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _httpContext = httpContext;
        }

        public async Task<List<UserDto>> GetAllUsersAsync(FormFilterUser filter)
        {
            var items = _context.Users
                .Where(u => u.IsDeleted == false)
                .AsQueryable();

            if (!string.IsNullOrEmpty(filter.UserName))
                items = items.Where(u => u.UserName.ToLower().Contains(filter.UserName.ToLower()));

            var us = await items.ToArrayAsync();

            var users = _mapper.Map<List<UserDto>>(us);

            foreach (var u in users)
            {

                var roles = await _userManager.GetRolesAsync(await _userManager.FindByNameAsync(u.UserName));
                if (roles.Count > 0)
                {
                    var role = await _roleManager.FindByNameAsync(roles[0]);
                    if (role != null)
                    {
                        u.Role = role.Name;
                    }
                }
            }

            return users;
        }

        public async Task<UserInfoDto> GetUserInfoByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userInfo = _mapper.Map<UserInfoDto>(user);

            var roles = await _userManager.GetRolesAsync(user!);
            var roleName = roles.FirstOrDefault();
            userInfo.Role = roleName;

            var dashIds = await _context.UserDashboards.Where(x => x.UserId == userInfo.Id).Select(x => x.DashboardId).ToListAsync();
            if (dashIds.Any())
            {
                var dashboards = await _context.Dashboards.Where(x => x.IsDeleted == false).ToListAsync();
                dashboards = (List<Dashboards>)dashboards.Where(x => dashIds.Contains(x.Id)).ToList();
                userInfo.Dashboards = _mapper.Map<List<DashboardDto>>(dashboards);
                foreach (var dash in userInfo.Dashboards)
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
                        var existingPermission = await _context!.Permissions!
                            .FirstOrDefaultAsync(d => d.FunctionId == function.Id && d.DashboardId == dash.Id && d.UserId == userInfo.Id);

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

            return userInfo;

        }

        public async Task<UserDto> GetUserByIdAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var userModel = _mapper.Map<UserDto>(user);
            return userModel;
        }

        public async Task<bool> SaveUserAsync(UserDto model)
        {
            var existingUser = await _userManager.FindByIdAsync(model.Id);

            if (existingUser == null)
            {
                // Create a new user
                AspNetUsers user = new AspNetUsers
                {
                    UserName = model.UserName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,
                    CreatedUser = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? null,
                    CreatedTime = DateTime.Now,
                    IsDeleted = false
                };

                var res = await _userManager.CreateAsync(user, model.Password);

                var role = await _roleManager.Roles.FirstOrDefaultAsync(u => u.IsDefault == true);

                if (res.Succeeded && role != null)
                {
                    await _userManager.AddToRoleAsync(user, role.Name!);
                    return true;
                }
                return false;
            }
            else
            {
                // Update an existing user
                existingUser.UserName = model.UserName;
                existingUser.Email = model.Email;
                existingUser.PhoneNumber = model.PhoneNumber;
                existingUser.IsDeleted = false;
                existingUser.ModifiedTime = DateTime.Now;
                existingUser.ModifiedUser = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? null;
                var res = await _userManager.UpdateAsync(existingUser);
                if (res.Succeeded)
                {
                    return true;
                }
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(UserDto model)
        {
            var user = await _userManager.FindByIdAsync(model.Id!);
            if (user != null)
            {
                user.IsDeleted = true;
                user.ModifiedTime = DateTime.Now;
                user.ModifiedUser = _httpContext.HttpContext?.User.FindFirstValue(ClaimTypes.Name) ?? null;
                await _userManager.UpdateAsync(user);
                return true;
            }
            return false;

        }
    }
}
