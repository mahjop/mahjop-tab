using mahjop.Data;
using mahjop.Models;
using mahjop.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace mahjop.Controllers
{
    public class AdminController : Controller
    {
        
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly IPageViewService _pageViewService;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AdminController(RoleManager<IdentityRole> roleManager, ApplicationDbContext context, UserManager<User> userManager, IPageViewService pageViewService)
        {
            
            _context = context;
            _userManager = userManager;
            _pageViewService = pageViewService;
        }
        public IActionResult Index(int? id)
        {


          
            // استعلام لجلب عدد الفيوهات للصفحة
            var viewsCount = _pageViewService.GetPageViewsCount("Index","Home");

            // إرسال عدد الفيوهات إلى عرض الصفحة
            ViewData["ViewsCount"] = viewsCount;
            //استعلام لجلب عدد المستخدمين 
            var userCount = _userManager.Users.Count();
            // استعلام لجلب عدد المرضى
            var patientsCount = _context.Patients.Count();

            // استعلام لجلب عدد الأطباء
            var doctorsCount = _context.Doctors.Count();
            // استعلام لجلب عدد المختبرات
            var labCount = _context.Labs.Count();
            // استعلام لجلب عدد الصيدليات
            var pharmcyCount = _context.Pharmacies.Count();

            // يمكنك إضافة القيم المسترجعة من قاعدة البيانات إلى عرض الفهرس
            ViewData["PatientsCount"] = patientsCount;
            ViewData["DoctorsCount"] = doctorsCount;
            ViewData["UserCount"] = userCount;
            ViewData["LabCount"] = labCount;
            ViewData["PharmcyCount"] = pharmcyCount;

            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearViews(string operation)
        {
            if (operation == "clearViews")
            {
                // اقترح تنفيذ الطلب المطلوب هنا
                // على سبيل المثال: _pageViewService.ClearAllPageViews();
                _pageViewService.ClearAllPageViews();
            }

            // تحويل المستخدم مرة أخرى إلى الصفحة الحالية بعد إكمال العملية
            return RedirectToAction(nameof(Index));
        }


    }
}
