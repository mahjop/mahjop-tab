using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahjop.Data;
using mahjop.Models;
using Microsoft.AspNetCore.Identity;
using System.IO;
using System.Numerics;
using Microsoft.AspNetCore.Http;

namespace mahjop.Controllers
{
    public class PharmaciesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public PharmaciesController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }



        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var role = await _roleManager.FindByNameAsync("Admin");
            if (role != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // إذا كان المستخدم غير مسجل دخوله ولديه دور "Admin"
                var applicationDbContext = _context.Pharmacies;
                return View(await applicationDbContext.ToListAsync());
            }

            if (user != null)
            {
                // إذا كان المستخدم مسجل دخوله
                var userId = user.Id;
                var applicationDbContext = _context.Pharmacies;
                return View(await applicationDbContext.Where(o => o.UserId == userId).ToListAsync());

            }

            return View();
        }

        // GET: Pharmacies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(m => m.PharmacyId == id);
            if (pharmacy == null)
            {
                return NotFound();
            }

            return View(pharmacy);
        }

        // GET: Pharmacies/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pharmacies/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PharmacyId,Name,Description,Address,Rating,PharmacyPicture,IsActive,UserId")] Pharmacy pharmacy)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                var rating = pharmacy.Rating;
                if (ModelState.IsValid)
                {
                    // التحقق من عدم وجود حساب صيدلية مسبقًا لنفس المستخدم
                    if (!_context.Pharmacies.Any(d => d.UserId == userId))
                    {
                        // التحقق من عدم وجود اسم صيدلية مكرر
                        if (!_context.Pharmacies.Any(d => d.Name == pharmacy.Name))
                        {
                            if (Request.Form.Files.Count > 0)
                            {
                                var files = Request.Form.Files.FirstOrDefault();
                                using (var dataStream = new MemoryStream())
                                {
                                    await files.CopyToAsync(dataStream);
                                    pharmacy.PharmacyPicture = dataStream.ToArray();
                                }
                            }
                            // إضافة الصيدلية إلى قاعدة البيانات
                            pharmacy.UserId = userId;
                            pharmacy.Rating = rating = 0;
                            _context.Add(pharmacy);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // رسالة الخطأ في حالة وجود اسم الصيدلية مكرر
                            ModelState.AddModelError("", "This pharmacy name already exists.");
                        }
                    }
                    else
                    {
                        // رسالة الخطأ في حالة وجود حساب الصيدلية بالفعل
                        ModelState.AddModelError("", "You have already created a pharmacy account.");
                    }
                }
            }
            
            return View(pharmacy);
        }

    

    // GET: Pharmacies/Edit/5
    public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pharmacy = await _context.Pharmacies.FindAsync(id);
            if (pharmacy == null)
            {
                return NotFound();
            }
            return View(pharmacy);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PharmacyId,Name,Description,Address,Rating,PharmacyPicture,IsActive,UserId")] Pharmacy pharmacy, IFormFile pharmacyImage)
        {
            if (id != pharmacy.PharmacyId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingPharmacy = await _context.Pharmacies.FindAsync(id);

                    if (existingPharmacy != null)
                    {
                        if (pharmacyImage != null && pharmacyImage.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await pharmacyImage.CopyToAsync(memoryStream);
                                existingPharmacy.PharmacyPicture = memoryStream.ToArray();
                            }
                        }

                        // لا تقم بتحديث UserId هنا، احتفظ به كما هو

                        existingPharmacy.Name = pharmacy.Name;
                        existingPharmacy.Description = pharmacy.Description;
                        existingPharmacy.Address = pharmacy.Address;
                        existingPharmacy.Rating = pharmacy.Rating;
                        existingPharmacy.IsActive = pharmacy.IsActive;
                        // قد تحتاج إلى التأكد من مطابقة الخصائص الأخرى أيضًا

                        _context.Update(existingPharmacy);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PharmacyExists(pharmacy.PharmacyId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            // تأكد من أن اسم المتغير الذي يتم إرجاعه إلى عرض العرض هو pharmacy وليس doctor
           return View(pharmacy);
        }

        // GET: Pharmacies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pharmacy = await _context.Pharmacies
                .FirstOrDefaultAsync(m => m.PharmacyId == id);
            if (pharmacy == null)
            {
                return NotFound();
            }

            return View(pharmacy);
        }

        // POST: Pharmacies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pharmacy = await _context.Pharmacies.FindAsync(id);
            _context.Pharmacies.Remove(pharmacy);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PharmacyExists(int id)
        {
            return _context.Pharmacies.Any(e => e.PharmacyId == id);
        }
    }
}
