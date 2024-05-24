using mahjop.Contants;
using mahjop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mahjop.Controllers
{
    [Authorize(Roles = "Admin")]
    public class RolesController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        public RolesController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var roles = await _roleManager.Roles.ToListAsync();
            return View(roles);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(RoleFormViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Index", await _roleManager.Roles.ToListAsync());
            }

            if (await _roleManager.RoleExistsAsync(model.Name))
            {
                ModelState.AddModelError("Name","Role is exists");
                return View("Index", await _roleManager.Roles.ToListAsync());
            }
            await _roleManager.CreateAsync(new IdentityRole(model.Name.Trim()));

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Edit(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                return NotFound();

            }


            var viewmodel = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name

            };
            return View(viewmodel);
        }
        [Authorize(Roles = "Admin,Super_Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                return NotFound();

            }

            var rolename = await _roleManager.FindByNameAsync(model.RoleName);
            if (rolename != null && rolename.Id != model.Id)
            {
                ModelState.AddModelError("RoleName", "This name is already assiged to another user");
                return View(model);
            }
            role.Name = model.RoleName;
            await _roleManager.UpdateAsync(role);
            return RedirectToAction(nameof(Index));
        }
        [HttpDelete]
        public async Task<IActionResult> DeleteRole(string roleId)
        {
            var user = await _roleManager.FindByIdAsync(roleId);

            if (user != null)
            {
                var result = await _roleManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ManagePermissions(string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role==null)
            {
                return NotFound();
            }
            var roleClaims = _roleManager.GetClaimsAsync(role).Result.Select(c => c.Value).ToList();
            var allClaims = Permissions.GenrateAllPermission();
            var allpermission = allClaims.Select(p => new RoleViewModel { RoleName = p }).ToList();
            foreach (var permission in allpermission)
            {
                if (roleClaims.Any(c=> c==permission.RoleName))
                {
                    permission.IsSelected = true;
                }
               
            }
            var viewmodel = new PermissionFormViewModel
            {
                RoleId = role.Id,
                RoleName = role.Name,
                RoleClaims = allpermission
            };
            return View(viewmodel);

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> ManagePermissions(PermissionFormViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
            {
                return NotFound();
            }
            var roleClaims = await _roleManager.GetClaimsAsync(role);

         
            foreach (var claim in roleClaims)
            {
                await _roleManager.RemoveClaimAsync(role, claim);
            }
            var selectedclaims = model.RoleClaims.Where(c => c.IsSelected).ToList();
            foreach (var claim in selectedclaims)
            {
                await _roleManager.AddClaimAsync(role, new Claim("Permission", claim.RoleName));
            }
            return RedirectToAction(nameof(Index));

        }


    }
}
