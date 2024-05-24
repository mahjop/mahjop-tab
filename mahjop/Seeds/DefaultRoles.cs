using mahjop.Contants;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mahjop.Seeds
{
    public static class DefaultRoles
    {
        public static async Task SeedAsync(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole(Roles.SuperAdmin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Admin.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.User.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Doctor.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.Lab.ToString()));
                await roleManager.CreateAsync(new IdentityRole(Roles.pharmacy.ToString()));

            }

        }
    }
}
