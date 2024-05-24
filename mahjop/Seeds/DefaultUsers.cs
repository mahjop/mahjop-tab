using mahjop.Contants;
using mahjop.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mahjop.Seeds
{
    public static class DefaultUsers
    {
        public static async Task SeedBasivUserAsync(UserManager<User> userManager)
        {
            var defaultUser = new User
            {
                UserName = "UserMahjop",
                Email = "UserMahjop@2001",
                FirstName = "user@2001",
                LastName = "U",
                ProfilePicture = null,
                EmailConfirmed = true,
            };
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user==null)
            {
                await userManager.CreateAsync(defaultUser, "User@2001");
                await userManager.AddToRoleAsync(defaultUser,Roles.User.ToString());
            }

        }
        public static async Task SeedAdminAsync(UserManager<User> userManager)
        {
            var defaultUser = new User
            {
                UserName = "Admin",
                Email = "Admin@2001",
                FirstName = "Admin@2001",
                LastName = "U",
                ProfilePicture = null,
                EmailConfirmed = true,
            };
            var user = await userManager.FindByEmailAsync(defaultUser.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultUser, "Admin@2001");
                await userManager.AddToRoleAsync(defaultUser, Roles.Admin.ToString());
            }

        }
        /*public static async Task SeedAdminUserAsync(UserManager<User> userManager)
        {
            var defaultAdmin = new User
            {
                UserName = "AdminMahjop",
                Email = "AdminMahjop@2001",
                EmailConfirmed = true,
            };
            var user = await userManager.FindByEmailAsync(defaultAdmin.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultAdmin, "AdminMahjop@2001");
                await userManager.AddToRolesAsync(defaultAdmin, new List<string>
                {
                    Roles.Admin.ToString(),
                    Roles.User.ToString(),
                });
            }

        }
        */
        public static async Task SeedSuperAdminUserAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            var defaultSuperAdmin = new User
            {
                UserName = "Super",
                Email = "mahjopmaharmasda2001@gmail.com",
                FirstName = "usaaaaa",
                LastName = "Uaaa",
                ProfilePicture = null,
                EmailConfirmed = true,
            };
            var user = await userManager.FindByEmailAsync(defaultSuperAdmin.Email);
            if (user == null)
            {
                await userManager.CreateAsync(defaultSuperAdmin, "Super@2001");
                await userManager.AddToRolesAsync(defaultSuperAdmin, new List<string>
                {
                    Roles.SuperAdmin.ToString(),
                    Roles.Admin.ToString(),
                    Roles.User.ToString(),
                });
                await roleManager.SeedClaimsForSuperUser();
                
            }


        }
        private static async Task SeedClaimsForSuperUser(this RoleManager<IdentityRole> roleManager)
        {
            var superadminRole = await roleManager.FindByNameAsync(Roles.SuperAdmin.ToString());
            await roleManager.AddPermissionClaims(superadminRole, "Products");
        }
        public static async Task AddPermissionClaims(this RoleManager<IdentityRole>roleManager,IdentityRole role,string module)
        {
            var allclaims = await roleManager.GetClaimsAsync(role);
            var allPermissions = Permissions.GenratePermissionList(module);
            foreach (var premission in allPermissions)
            {
                if (!allclaims.Any(c=>c.Type == "Premission" && c.Value == premission))
                {
                    await roleManager.AddClaimAsync(role, new Claim("Premission", premission));
                }
            }
        }
    }
}
