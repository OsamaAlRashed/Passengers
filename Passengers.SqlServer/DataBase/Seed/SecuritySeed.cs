using Microsoft.AspNetCore.Identity;
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

            var newRole = await CreateNewRoles(roleManager);
            await ClearRoles(roleManager);

            await InsureCreateAccountAsync(userManager, roleManager, newRole);
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

        private static async Task InsureCreateAccountAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager, IEnumerable<string> newRoles)
        {
            #region Admin
            var admin = await userManager.FindByNameAsync("admin");
            if (admin is null)
            {
                admin = new AppUser()
                {
                    UserName = "admin",
                    FullName = "admin admin",
                    Email = "admin@admin",
                    UserType = UserTypes.Admin,
                };
                var createResult = await userManager.CreateAsync(admin, "1111");
                if (createResult.Succeeded)
                {
                    var identityRoles = roleManager.Roles.Select(a => a.Name).ToList();
                    var roleResult = await userManager.AddToRoleAsync(admin, UserTypes.Admin.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
            #endregion

            #region Shop
            var shop = await userManager.FindByNameAsync("shop1");
            if (shop is null)
            {
                shop = new AppUser()
                {
                    UserName = "shop1",
                    FullName = "shop1 shop1",
                    Email = "shop1@shop1",
                    UserType = UserTypes.Shop,
                };
                var createResult = await userManager.CreateAsync(shop, "1111");
                if (createResult.Succeeded)
                {
                    var identityRoles = roleManager.Roles.Select(a => a.Name).ToList();
                    var roleResult = await userManager.AddToRoleAsync(shop, UserTypes.Shop.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
            var shop2 = await userManager.FindByNameAsync("shop2");
            if (shop2 is null)
            {
                shop2 = new AppUser()
                {
                    UserName = "shop2",
                    FullName = "shop2 shop2",
                    Email = "shop2@shop2",
                    UserType = UserTypes.Shop,
                };
                var createResult = await userManager.CreateAsync(shop2, "1111");
                if (createResult.Succeeded)
                {
                    var identityRoles = roleManager.Roles.Select(a => a.Name).ToList();
                    var roleResult = await userManager.AddToRoleAsync(shop2, UserTypes.Shop.ToString());
                    if (!roleResult.Succeeded)
                        throw new Exception(String.Join("\n", roleResult.Errors.Select(error => error.Description)));
                }
            }
            #endregion
        }

    }
}
