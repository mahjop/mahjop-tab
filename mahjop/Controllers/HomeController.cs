using mahjop.Data;
using mahjop.Models;
using mahjop.Services;
using mahjop.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Security.Claims;
using System.Threading.Tasks;

namespace mahjop.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IPageViewService _pageViewService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context, UserManager<User> userManager, IPageViewService pageViewService)
        {
            _logger = logger;
            _context = context;
            _userManager = userManager;
            _pageViewService = pageViewService;
        }
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var today = DateTime.Today;

            // التحقق مما إذا تم تسجيل الفيو لهذا المستخدم اليوم بالفعل
            var isViewed = await _context.PageViews.AnyAsync(p => p.UserId == userId && p.VisitTime.Date == today);

            // إذا لم يتم تسجيل الفيو لهذا المستخدم اليوم، سجل الفيو
            if (!isViewed)
            {
                await _pageViewService.AddPageView("Index", "Home", userId);
            }

            // استرجاع الأطباء وتمريرهم إلى العرض
            var doctors = await _context.Doctors.ToListAsync();
            var Departments = await _context.Departments.ToListAsync();
            var viewModel = new DocAppViewModel
            {
                Doctors = doctors,
                Departments = Departments,
            };

            return View(viewModel);
        }

        [HttpGet]
        public IActionResult GetAvailableTimes(DayOfWeek day)
        {
            // استعلام لاسترجاع ساعات العمل لليوم المحدد
            var workingHours = _context.WorkingHours.Where(w => w.Day == day).ToList();

            // قائمة لتخزين الساعات المتاحة
            List<string> availableHours = new List<string>();

            // تحويل ساعات العمل إلى سلاسل نصية وإضافتها إلى القائمة المتاحة
            foreach (var hour in workingHours)
            {
                availableHours.Add(hour.StartTime.ToString("HH:mm"));

                // يمكنك أيضًا إضافة الساعات بتنسيق مخصص هنا، مثل الساعة والدقيقة و AM/PM
            }

            // إرجاع القائمة المتاحة كنتيجة JSON
            return Json(availableHours);
        }


        public async Task<IActionResult> Our_Doctor()
        {
            var doctors = await _context.Doctors.Include(d => d.Department).ToListAsync();
            // استعلام للحصول على ساعات العمل المرتبطة بالطبيب
            var departments = await _context.Departments

                .ToListAsync();

            // تمرير ساعات العمل إلى العرض
            ViewData["Departments"] = departments;
            return View(doctors);
        }
        public async Task<IActionResult> Department_Doctor(int? id)
        {
            if (id == null)
            {
                return BadRequest("Department ID is null.");
            }

            var doctorsInDepartment = await _context.Doctors.Where(d => d.DepartmentId == id).ToListAsync();

            if (doctorsInDepartment == null || !doctorsInDepartment.Any())
            {
                return NotFound("No doctors found in the specified department.");
            }

            return View(doctorsInDepartment);
        }

        public async Task<IActionResult> Details_Our_Doctor(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var doctor = await _context.Doctors
                .Include(a => a.Department)
                .FirstOrDefaultAsync(m => m.DoctorId == id);

            if (doctor == null)
            {
                return NotFound();
            }

            // تمرير معرف الطبيب إلى العرض
            ViewData["DoctorId"] = doctor.DoctorId;

            // استعلام للحصول على ساعات العمل المرتبطة بالطبيب
            var workingHours = await _context.WorkingHours
                .Where(w => w.DoctorId == doctor.DoctorId)
                .ToListAsync();

            // إنشاء مثيل جديد من نموذج الحجز وتعيين القيم المطلوبة
            var appointment = new Appointment
            {
                DoctorId = doctor.DoctorId,
                
            };

            // تمرير الحجز إلى العرض
            ViewData["Appointment"] = appointment;

            // تمرير ساعات العمل إلى العرض
            ViewData["WorkingHours"] = workingHours;

            return View(doctor);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(Appointment model, int doctorId)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var appointment = new Appointment { UserId = userId };
            ViewData["DoctorId"] = doctorId;

            if (await _context.Appointments.AnyAsync(a => a.AppointmentDay == model.AppointmentDay))
            {
                ModelState.AddModelError("AppointmentDay", "This appointment day already exists");
                return View("Index", await _context.Appointment.ToListAsync());
            }


            _context.Appointment.Add(model);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Appointments/Create
        public async Task<IActionResult> Create(int doctorId)
        {
            // Get current user id
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            // Create new Appointment object with UserId set
            var appointment = new Appointment { UserId = userId };

            // Populate ViewData with list of doctors
            ViewData["DoctorId"] = doctorId;

            return View(appointment);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AppointmentId,AppointmentTime,AppointmentDay,IsConfirmed,DoctorId,UserId")] Appointment appointment)
        {
            if (ModelState.IsValid)
            {
                appointment.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Set the user ID here
                _context.Add(appointment);
                await _context.SaveChangesAsync();
                return Json(new { success = true });
            }
            ViewData["DoctorId"] = new SelectList(_context.Doctors, "DoctorId", "NameDoctor", appointment.DoctorId);
            return Json(new { success = false });
        }



        public async Task<IActionResult> Our_lab()
        {
            var lab = await _context.Labs.ToListAsync();
            return View(lab);
        }
        public async Task<IActionResult> Details_Our_Lab(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Pharmacy = await _context.Labs
                      .FirstOrDefaultAsync(m => m.LabId == id);
            if (Pharmacy == null)
            {
                return NotFound();
            }

            return View(Pharmacy);
        }

        public async Task<IActionResult> Our_pharmacy()
        {
            var pharmacy = await _context.Pharmacies.ToListAsync();
            return View(pharmacy);
        }
        public async Task<IActionResult> Details_Our_Pharmacy(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var Pharmacy = await _context.Pharmacies
                      .FirstOrDefaultAsync(m => m.PharmacyId == id);
            if (Pharmacy == null)
            {
                return NotFound();
            }

            return View(Pharmacy);
        }



        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

    }
}
