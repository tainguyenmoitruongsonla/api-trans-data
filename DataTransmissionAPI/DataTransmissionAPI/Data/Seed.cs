using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataTransmissionAPI.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(DatabaseContext context, UserManager<AspNetUsers> userManager, RoleManager<AspNetRoles> roleManager)
        {
            // Ensure the database is created and apply migrations
            context.Database.EnsureCreated();

            // Check if there is any data in the database
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations == null || !pendingMigrations.Any())
            {
                await SeedRolesAsync(roleManager);
                await SeedUsersAsync(userManager);
                await SeedFunctionsAsync(context);
                await SeedDashboardAsync(context);
            }
        }

        private static async Task SeedRolesAsync(RoleManager<AspNetRoles> roleManager)
        {
            if (!await roleManager.Roles.AnyAsync())
            {
                await roleManager.CreateAsync(new AspNetRoles { Name = "Administrator", Description = "Quản trị hệ thống", IsDeleted = false });
                await roleManager.CreateAsync(new AspNetRoles { Name = "Professional", Description = "Chuyên viên", IsDeleted = false });
                await roleManager.CreateAsync(new AspNetRoles { Name = "Leader", Description = "Lãnh đạo", IsDeleted = false });
                await roleManager.CreateAsync(new AspNetRoles { Name = "Default", IsDefault = true, IsDeleted = false });
            }
        }

        private static async Task SeedUsersAsync(UserManager<AspNetUsers> userManager)
        {
            if (!await userManager.Users.AnyAsync())
            {
                var admin = new AspNetUsers { UserName = "admin", IsDeleted = false };
                await userManager.CreateAsync(admin, "admin");
                await userManager.AddToRoleAsync(admin, "Administrator");

                var lanpth = new AspNetUsers { UserName = "lan.pth", IsDeleted = false };
                await userManager.CreateAsync(lanpth, "lan.pth");
                await userManager.AddToRoleAsync(lanpth, "Leader");

                var dangnt = new AspNetUsers { UserName = "dang.nt", IsDeleted = false };
                await userManager.CreateAsync(dangnt, "dang.nt");
                await userManager.AddToRoleAsync(dangnt, "Professional");

                var anhnt = new AspNetUsers { UserName = "anh.nt", IsDeleted = false };
                await userManager.CreateAsync(anhnt, "anh.nt");
                await userManager.AddToRoleAsync(anhnt, "Default");
            }
        }

        private static async Task SeedFunctionsAsync(DatabaseContext context)
        {
            if (!await context.Functions!.AnyAsync())
            {
                context.Functions!.AddRange(
                    //Base function
                    new Functions { PermitName = "Xem", PermitCode = "VIEW" },
                    new Functions { PermitName = "Thêm mới", PermitCode = "CREATE" },
                    new Functions { PermitName = "Cập nhật", PermitCode = "EDIT" },
                    new Functions { PermitName = "Xóa", PermitCode = "DELETE" },
                    //Function in user
                    new Functions { PermitName = "Đặt lại mật khẩu", PermitCode = "RESETPASSWORD" },
                    new Functions { PermitName = "Cấp quyền", PermitCode = "SETROLE" },
                    new Functions { PermitName = "Chỉ định quyền", PermitCode = "ASSIGNROLE" },
                    new Functions { PermitName = "Chỉ định Chức nằng", PermitCode = "ASSIGNFUNCTION" });

                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedDashboardAsync(DatabaseContext context)
        {
            if (!await context.Dashboards!.AnyAsync())
            {
                context.Dashboards!.AddRange(
                    new Dashboards { Name = "Users", Path = "user", Description = "Quản lý tài khoản", IsDeleted = false, PermitAccess = false },
                    new Dashboards { Name = "UserInfo", Path = "user-info", Description = "Thông tin tài khoản", IsDeleted = false, PermitAccess = false },
                    new Dashboards { Name = "Roles", Path = "role", Description = "Quản lý role", IsDeleted = false, PermitAccess = false },
                    new Dashboards { Name = "Dashboard", Path = "dashboard", Description = "Quản lý đường dẫ truy cập", IsDeleted = false, PermitAccess = false },

                    new Dashboards { Name = "Manage", Path = "manage", Description = "Quản lý", IsDeleted = false, PermitAccess = false },
                    new Dashboards { Name = "System", Path = "system", Description = "Quản lý hệ thống", IsDeleted = false, PermitAccess = false },
                    new Dashboards { Name = "Permission", Path = "permission", Description = "Quản lý phân quyền truy cập", IsDeleted = false, PermitAccess = false },

                    new Dashboards { Name = "Stations", Path = "station", Description = "Quản lý thông tin trạm", IsDeleted = false, PermitAccess = false },
                    new Dashboards { Name = "RealMeasurementData", Description = "Quản lý dữ liệu thực đo", Path = "water-level-data", IsDeleted = false, PermitAccess = false }

                    );
                await context.SaveChangesAsync();
            }
        }
    }
}