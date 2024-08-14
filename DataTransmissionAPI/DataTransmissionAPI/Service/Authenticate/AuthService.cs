using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using DataTransmissionAPI.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Newtonsoft.Json;
using DataTransmissionAPI.Dto;

namespace DataTransmissionAPI.Service
{
    public class AuthService : IAuthService
    {
        private readonly Data.DatabaseContext _context;
        private readonly UserManager<AspNetUsers> _userManager;
        private readonly RoleManager<AspNetRoles> _roleManager;

        public SignInManager<AspNetUsers> _signInManager { get; }

        private readonly IConfiguration _configuration;
        private readonly IHttpContextAccessor _httpContext;

        public AuthService(UserManager<AspNetUsers> userManager, SignInManager<AspNetUsers> signInManager, RoleManager<AspNetRoles> roleManager, IConfiguration configuration, Data.DatabaseContext context, IHttpContextAccessor httpContext)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _httpContext = httpContext;
        }

        public async Task<bool> RegisterAsync(UserDto dto)
        {
            // Create a new user
            AspNetUsers user = new AspNetUsers
            {
                UserName = dto.UserName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                IsDeleted = false
            };

            var res = await _userManager.CreateAsync(user, dto.Password);

            var role = await _roleManager.Roles.FirstOrDefaultAsync(u => u.IsDefault == true);

            if (res.Succeeded && role != null)
            {
                await _userManager.AddToRoleAsync(user, role.Name!);
                return true;
            }
            return false;
        }

        public async Task<string> LoginAsync(LoginViewDto dto)
        {
            var res = await _signInManager.PasswordSignInAsync(dto.UserName, dto.Password, dto.RememberMe, false);
            if (!res.Succeeded)
            {
                return string.Empty;
            }

            var user = await _userManager.FindByNameAsync(dto.UserName);
            var roles = await _userManager.GetRolesAsync(user!);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user!.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("id", user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? "")
            };

            var addedPermissions = new HashSet<string>();

            foreach (var role in roles)
            {

                claims.Add(new Claim(ClaimTypes.Role, role));
                var rolePermissions = await _context.Permissions!.Where(p => p.RoleName == role).ToListAsync();
                foreach (var permission in rolePermissions)
                {
                    var dashSrc = await _context.Dashboards!.Where(d => d.Id == permission.DashboardId).Select(d => d.Path).FirstOrDefaultAsync();
                    var permissionObject = new
                    {
                        funcCode = permission.FunctionCode,
                        dashSrc = dashSrc!
                    };

                    var permissionJson = JsonConvert.SerializeObject(permissionObject);
                    if (!addedPermissions.Contains(permissionJson))
                    {
                        var permissionClaim = new Claim("Permission", permissionJson);
                        claims.Add(permissionClaim);

                        addedPermissions.Add(permissionJson);
                    }
                }
            }

            var userPermissions = await _context.Permissions!.Where(p => p.UserName == user.UserName).ToListAsync();

            foreach (var permission in userPermissions)
            {
                var dashSrc = await _context.Dashboards!.Where(d => d.Id == permission.DashboardId).Select(d => d.Path).FirstOrDefaultAsync();
                var permissionObject = new
                {
                    funcCode = permission.FunctionCode,
                    dashSrc = dashSrc!
                };

                var permissionJson = JsonConvert.SerializeObject(permissionObject);

                if (!addedPermissions.Contains(permissionJson))
                {
                    var permissionClaim = new Claim("Permission", permissionJson);
                    claims.Add(permissionClaim);
                    addedPermissions.Add(permissionJson);
                }
            }


            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"] ?? ""));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(7),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return JsonConvert.SerializeObject(jwt);
        }


        public async Task<PasswordChangeResult> UpdatePasswordAsync(PasswordChange password)
        {
            if (password.newPassword != password.newConfirmPassword)
            {
                return new PasswordChangeResult(false, "New password and confirmation password do not match.");
            }

            var user = await _userManager.GetUserAsync(_httpContext.HttpContext!.User);
            if (user == null)
            {
                return new PasswordChangeResult(false, "User not found.");
            }

            var lastPasswordChangedDate = user.LastPasswordChangedDate ?? DateTime.MinValue;
            var timeSinceLastChange = DateTime.Now - lastPasswordChangedDate;
            if (timeSinceLastChange.TotalDays < 30)
            {
                var remainingTime = TimeSpan.FromDays(30) - timeSinceLastChange;
                return new PasswordChangeResult(false, $"You can change your password in {remainingTime.Days} day(s), {remainingTime.Hours} hour(s), {remainingTime.Minutes} minute(s), and {remainingTime.Seconds} second(s).");
            }

            var res = await _userManager.ChangePasswordAsync(user, password.currentPassword, password.newPassword);
            if (res.Succeeded)
            {
                user.LastPasswordChangedDate = DateTime.Now;
                await _userManager.UpdateAsync(user);
                return new PasswordChangeResult(true, "Password changed successfully.");
            }

            return new PasswordChangeResult(false, null);
        }


        public async Task<bool> SetPasswordAsync(UserDto dto, string newPassword)
        {
            var user = await _userManager.FindByNameAsync(dto.UserName!);

            if (user != null)
            {
                var removePasswordResult = await _userManager.RemovePasswordAsync(user);
                if (removePasswordResult.Succeeded)
                {
                    var addPasswordResult = await _userManager.AddPasswordAsync(user, newPassword);
                    return addPasswordResult.Succeeded;
                }
            }

            return false;
        }


        public async Task<bool> AssignRoleAsync(AssignRoleDto dto)
        {
            var user = await _userManager.FindByIdAsync(dto.userId);
            if (user == null) { return false; }

            // Remove all existing roles of the user
            var existingRoles = await _userManager.GetRolesAsync(user);
            if (existingRoles == null) { return false; }

            await _userManager.RemoveFromRolesAsync(user!, existingRoles);

            // Add the new role to the user
            await _userManager.AddToRoleAsync(user!, dto.roleName);

            return true;
        }

        public async Task<bool> RemoveRoleAsync(AssignRoleDto dto)
        {
            var u = await _userManager.FindByIdAsync(dto.userId);
            // Check if the user is already in the role
            var isInRole = await _userManager.IsInRoleAsync(u!, dto.roleName);
            if (isInRole)
            {
                await _userManager.RemoveFromRoleAsync(u!, dto.roleName);
                var role = await _roleManager.Roles.FirstOrDefaultAsync(u => u.IsDefault == true);
                await _userManager.AddToRoleAsync(u!, role!.Name!);

                return true;
            }
            return false;
        }

        public async Task<bool> CheckAccessPermission(string userName, string linkControl, string action)
        {
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null || linkControl == null || action == null) { return false; }
            if (await _userManager.IsInRoleAsync(user, "Administrator")) return true;

            var dash = _context.Dashboards.Where(x => x.Path == linkControl).FirstOrDefault();
            if (dash == null) return false;
            var existingPermission = await _context!.Permissions!.FirstOrDefaultAsync(d => d.FunctionCode.ToLower() == action.ToLower() && d.DashboardId == dash.Id && d.UserId == user.Id);
            if (existingPermission != null) return true;

            return false;
        }


        public async Task<bool> LogoutAsync(HttpContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return true;
        }

    }
}
