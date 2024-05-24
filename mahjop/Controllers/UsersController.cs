using mahjop.Models;
using mahjop.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mahjop.Controllers
{
    [Authorize(Roles ="Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<User> _signInManager;
        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager,SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }
        public async Task<IActionResult> Index()
        {
           
            var users = await _userManager.Users.Select(user=>new UserViewModel
            { 
             Id=user.Id,
             FirstName=user.FirstName,
             LastName=user.LastName,
             UserName=user.UserName,
             Email=user.Email,
          


        }).ToListAsync();
            return View(users);
        }
        public async Task<IActionResult> Add()
        {
           
            var roles = await _roleManager.Roles.Select(r=>new RoleViewModel { RoleId=r.Id,RoleName=r.Name}).ToListAsync();
            var viewModel = new AddUserViewModel
            {
               
                Roles =roles,

            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AddUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            if (!model.Roles.Any(r => r.IsSelected))
            {
                ModelState.AddModelError("Roles", "Please select at least one role");
                return View(model);
            }
            if (await _userManager.FindByEmailAsync(model.Email) != null)
            {

                ModelState.AddModelError("Email", "Email is already exists");
                return View(model);
            }
            if (await _userManager.FindByNameAsync(model.UserName) != null)
            {

                ModelState.AddModelError("Username", "Email is already exists");
                return View(model);
            }
            var user = new User
            {

                UserName = model.UserName,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };
            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("Roles", error.Description);
                }
                return View(model);
            }
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return NotFound();

            }


            var viewmodel = new EditUserViewModel
            {
                Id = userId,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email

            };
            return View(viewmodel);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByIdAsync(model.Id);
            if (user == null)
            {
                return NotFound();

            }
            var usersameemaile = await _userManager.FindByEmailAsync(model.Email);
            if (usersameemaile != null && usersameemaile.Id != model.Id)
            {
                ModelState.AddModelError("Email", "This email is already assiged to another user");
                return View(model);
            }
            var usersameename = await _userManager.FindByNameAsync(model.UserName);
            if (usersameename != null && usersameename.Id != model.Id)
            {
                ModelState.AddModelError("UserName", "This name is already assiged to another user");
                return View(model);
            }
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.UserName = model.UserName;
            user.Email = model.Email;
            await _userManager.UpdateAsync(user);
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user==null)
            {
                return NotFound();
            }
            var roles = await _roleManager.Roles.ToListAsync();
            var viewModel = new UserRolesViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                Roles = roles.Select(role => new RoleViewModel 
                {
                    RoleId=role.Id,
                    RoleName=role.Name,
                    IsSelected=_userManager.IsInRoleAsync(user,role.Name).Result
                }).ToList()

            };
            return View(viewModel);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageRole(UserRolesViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                return NotFound();

            }
            var userRole = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, userRole);
            await _userManager.AddToRolesAsync(user, model.Roles.Where(r => r.IsSelected).Select(r => r.RoleName));

            //foreach (var role in model.Roles)
            //{
            //    if (userRole.Any(r => r == role.RoleName) && !role.IsSelected)
            //    {
            //        await _userManager.RemoveFromRoleAsync(user, role.RoleName);
            //    }
            //
            //    if (!userRole.Any(r => r == role.RoleName) && role.IsSelected)
            //    {
            //        await _userManager.AddToRoleAsync(user, role.RoleName);
            //    }
            //
            //}
            return RedirectToAction(nameof(Index));
        }
        [HttpDelete]
        
        public async Task<IActionResult> DeleteUser(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user != null)
            {
                var result = await _userManager.DeleteAsync(user);

                if (result.Succeeded)
                {
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false });
        }



    }
}
