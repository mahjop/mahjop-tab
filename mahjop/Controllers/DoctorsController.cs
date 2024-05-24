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
    public class DoctorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public DoctorsController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager; 
            _roleManager = roleManager;
        }

        // GET: Doctors


        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var role = await _roleManager.FindByNameAsync("Admin");
            if (role != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                // إذا كان المستخدم غير مسجل دخوله ولديه دور "Admin"
                var applicationDbContext = _context.Doctors.Include(d => d.Department);
                return View(await applicationDbContext.ToListAsync());
            }

            if (user != null)
            {
                // إذا كان المستخدم مسجل دخوله
                var userId = user.Id;
                      var applicationDbContext = _context.Doctors.Include(d => d.Department);
                return View(await applicationDbContext.Where(o => o.UserId == userId).ToListAsync());
          
            }

            return View();
        }


        // GET: Doctors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // GET: Doctors/Create
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DoctorId,NameDoctor,DescriptionDoctor,Address,Rating,DoctorPicture,Isactive,DepartmentId,UserId")] Doctor doctor)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var userId = user.Id;
                var rating = doctor.Rating;
                if (ModelState.IsValid)
                {
                    // التحقق من عدم وجود حساب طبيب مسبقًا لنفس المستخدم
                    if (!_context.Doctors.Any(d => d.UserId == userId))
                    {
                        // التحقق من عدم وجود اسم طبيب مكرر
                        if (!_context.Doctors.Any(d => d.NameDoctor == doctor.NameDoctor))
                        {
                            if (Request.Form.Files.Count > 0)
                            {
                                var files = Request.Form.Files.FirstOrDefault();
                                using (var dataStream = new MemoryStream())
                                {
                                    await files.CopyToAsync(dataStream);
                                    doctor.DoctorPicture = dataStream.ToArray();
                                }
                            }
                            // إضافة الطبيب إلى قاعدة البيانات
                            doctor.UserId = userId;
                            doctor.Rating = rating = 0;
                            _context.Add(doctor);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        else
                        {
                            // رسالة الخطأ في حالة وجود اسم طبيب مكرر
                            ModelState.AddModelError("", "This doctor name already exists.");
                        }
                    }
                    else
                    {
                        // رسالة الخطأ في حالة وجود حساب دكتور بالفعل
                        ModelState.AddModelError("", "You have already created a doctor account.");
                    }
                }
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            return View(doctor);
        }



        // GET: Doctors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            return View(doctor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DoctorId,NameDoctor,DescriptionDoctor,Address,Rating,DoctorPicture,Isactive,DepartmentId")] Doctor doctor, IFormFile departmentImage)
        {
            if (id != doctor.DoctorId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDoctor = await _context.Doctors.FindAsync(id);

                    if (existingDoctor != null)
                    {
                        if (departmentImage != null && departmentImage.Length > 0)
                        {
                            using (var memoryStream = new MemoryStream())
                            {
                                await departmentImage.CopyToAsync(memoryStream);
                                existingDoctor.DoctorPicture = memoryStream.ToArray();
                            }
                        }

                        // لا تقم بتحديث UserId هنا، احتفظ به كما هو

                        existingDoctor.NameDoctor = doctor.NameDoctor;
                        existingDoctor.DescriptionDoctor = doctor.DescriptionDoctor;
                        existingDoctor.Address = doctor.Address;
                        existingDoctor.Rating = doctor.Rating;
                        existingDoctor.Isactive = doctor.Isactive;
                        existingDoctor.DepartmentId = doctor.DepartmentId;

                        _context.Update(existingDoctor);
                        await _context.SaveChangesAsync();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DoctorExists(doctor.DoctorId))
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
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "DepartmentId", "Name", doctor.DepartmentId);
            return View(doctor);
        }



        // GET: Doctors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(d => d.Department)
                .FirstOrDefaultAsync(m => m.DoctorId == id);
            if (doctor == null)
            {
                return NotFound();
            }

            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}
