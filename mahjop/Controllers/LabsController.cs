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
using Microsoft.AspNetCore.Http;

namespace mahjop.Controllers
{
    public class LabsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public LabsController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }


        // GET: Labs
        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var role = await _roleManager.FindByNameAsync("Admin");
            if (role != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // إذا كان المستخدم غير مسجل دخوله ولديه دور "Admin"
                var applicationDbContext = _context.Labs;
                return View(await applicationDbContext.ToListAsync());
            }

            if (user != null)
            {
                // إذا كان المستخدم مسجل دخوله
                var userId = user.Id;
                var applicationDbContext = _context.Labs;
                return View(await applicationDbContext.Where(o => o.UserId == userId).ToListAsync());

            }

            return View();
        }

        // GET: Labs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs
                .FirstOrDefaultAsync(m => m.LabId == id);
            if (lab == null)
            {
                return NotFound();
            }

            return View(lab);
        }

        // GET: Labs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Labs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("LabId,Name,Description,Address,Rating,LabPicture,IsActive,UserId")] Lab lab)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                var rating = lab.Rating;
                if (ModelState.IsValid)
                {
                    // التحقق من عدم وجود حساب صيدلية مسبقًا لنفس المستخدم
                    if (!_context.Labs.Any(d => d.UserId == userId))
                    {
                        // التحقق من عدم وجود اسم  مكرر
                        if (!_context.Labs.Any(d => d.Name == lab.Name))
                        {
                            if (Request.Form.Files.Count > 0)
                            {
                                var files = Request.Form.Files.FirstOrDefault();
                                using (var dataStream = new MemoryStream())
                                {
                                    await files.CopyToAsync(dataStream);
                                    lab.LabPicture = dataStream.ToArray();
                                }
                            }
                            // إضافة الصيدلية إلى قاعدة البيانات
                            lab.UserId = userId;
                            lab.Rating = rating = 0;
                            _context.Add(lab);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // رسالة الخطأ في حالة وجود اسم المختبرات مكرر
                            ModelState.AddModelError("", "This lab name already exists.");
                        }
                    }
                    else
                    {
                        // رسالة الخطأ في حالة وجود حساب المختبرات بالفعل
                        ModelState.AddModelError("", "You have already created a lab account.");
                    }
                }
            }

            return View(lab);
        }

        // GET: Labs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs.FindAsync(id);
            if (lab == null)
            {
                return NotFound();
            }
            return View(lab);
        }

        // POST: Labs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LabId,Name,Description,Address,Rating,LabPicture,IsActive,UserId")] Lab lab, IFormFile labImage)
        {

            if (id != lab.LabId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existinglab = await _context.Labs.FindAsync(id);

                    if (existinglab != null)
                    {
                        if (labImage != null && labImage.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await labImage.CopyToAsync(memoryStream);
                                existinglab.LabPicture = memoryStream.ToArray();
                            }
                        }

                        // لا تقم بتحديث UserId هنا، احتفظ به كما هو

                        existinglab.Name = lab.Name;
                        existinglab.Description = lab.Description;
                        existinglab.Address = lab.Address;
                        existinglab.Rating = lab.Rating;
                        existinglab.IsActive = lab.IsActive;
                        // قد تحتاج إلى التأكد من مطابقة الخصائص الأخرى أيضًا

                        _context.Update(existinglab);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LabExists(lab.LabId))
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
            // تأكد من أن اسم المتغير الذي يتم إرجاعه إلى عرض العرض هو lab وليس 
            return View(lab);
        }

        // GET: Labs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var lab = await _context.Labs
                .FirstOrDefaultAsync(m => m.LabId == id);
            if (lab == null)
            {
                return NotFound();
            }

            return View(lab);
        }

        // POST: Labs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var lab = await _context.Labs.FindAsync(id);
            _context.Labs.Remove(lab);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LabExists(int id)
        {
            return _context.Labs.Any(e => e.LabId == id);
        }
    }
}
