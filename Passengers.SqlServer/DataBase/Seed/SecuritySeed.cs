using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Passengers.Models.Security;
using Passengers.SharedKernel.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Passengers.SqlServer.DataBase.Seed
{
    public class SecuritySeed
    {
        public static async Task InitializeAsync(IServiceProvider services)
        {
            var roleManager = services.GetService<RoleManager<IdentityRole<Guid>>>();
            var userManager = services.GetService<UserManager<AppUser>>();
            var context = services.GetService<PassengersDbContext>();

            var newRole = await CreateNewRoles(roleManager);
            await ClearRoles(roleManager);

            await InsureCreateAccountAsync(userManager, roleManager, newRole, context);
        }

        private static async Task<IEnumerable<string>> CreateNewRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = Enum.GetValues(typeof(UserTypes)).Cast<UserTypes>().Select(a => a.ToString());
            var identityRoles = roleManager.Roles.Select(a => a.Name).ToList();
            var newRoles = roles.Except(identityRoles); 

            foreach (var @new in newRoles)
            {
                await roleManager.CreateAsync(new IdentityRole<Guid>() { Name = @new });
            }

            return newRoles;
        }

        private static async Task<IEnumerable<string>> ClearRoles(RoleManager<IdentityRole<Guid>> roleManager)
        {
            var roles = Enum.GetValues(typeof(UserTypes)).Cast<UserTypes>().Select(a => a.ToString());
            var identityRoles = roleManager.Roles.ToList();

            var clearRoles = identityRoles.Where(x => !roles.Contains(x.Name));

            foreach (var @new in clearRoles)
            {
                await roleManager.DeleteAsync(@new);
            }

            return clearRoles.Select(x => x.Name);
        }

        private static async Task InsureCreateAccountAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IEnumerable<string> newRoles, PassengersDbContext context)
        {
            #region Super User
            var superUser = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.UserName == "SuperUser");
            if (superUser is null)
            {
                superUser = new AppUser()
                {
                    UserName = "SuperUser",
                    FullName = "Super User",
                    Email = "SuperUser@SuperUser",
                    UserType = UserTypes.Admin,
                    AccountStatus = AccountStatus.Accepted,
                    DOB = new DateTime(2000,1,1),
                    AddressText = "Syria - Aleppo",
                };
                var createResult = await userManager.CreateAsync(superUser, "passengers");
                if (createResult.Succeeded)
                {
                    var identityRoles = roleManager.Roles.Select(a => a.Name).ToList();
                    var roleResult = await userManager.AddToRolesAsync(superUser, identityRoles);
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
            #endregion

            #region Admin
            var admin = await context.Users.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.UserName == "admin");
            if (admin is null)
            {
                admin = new AppUser()
                {
                    UserName = "admin",
                    FullName = "admin admin",
                    Email = "admin@admin",
                    UserType = UserTypes.Admin,
                    AccountStatus = AccountStatus.Accepted,
                    DOB = new DateTime(2000, 1, 1),
                    AddressText = "Syria - Aleppo",
                };
                var createResult = await userManager.CreateAsync(admin, "passengers");
                if (createResult.Succeeded)
                {
                    var roleResult = await userManager.AddToRoleAsync(admin, UserTypes.Admin.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
            #endregion
        }
    }
}
