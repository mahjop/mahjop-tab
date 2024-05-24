using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using mahjop.Data;
using mahjop.Models;

namespace mahjop.Controllers
{
    public class MedicationCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MedicationCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MedicationCategories
        public async Task<IActionResult> Index()
        {
            return View(await _context.MedicationCategories.ToListAsync());
        }

        // GET: MedicationCategories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationCategory = await _context.MedicationCategories
                .FirstOrDefaultAsync(m => m.MedicationCategoryId == id);
            if (medicationCategory == null)
            {
                return NotFound();
            }

            return View(medicationCategory);
        }

        // GET: MedicationCategories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MedicationCategories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MedicationCategoryId,Name")] MedicationCategory medicationCategory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(medicationCategory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(medicationCategory);
        }

        // GET: MedicationCategories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationCategory = await _context.MedicationCategories.FindAsync(id);
            if (medicationCategory == null)
            {
                return NotFound();
            }
            return View(medicationCategory);
        }

        // POST: MedicationCategories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MedicationCategoryId,Name")] MedicationCategory medicationCategory)
        {
            if (id != medicationCategory.MedicationCategoryId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(medicationCategory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MedicationCategoryExists(medicationCategory.MedicationCategoryId))
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
            return View(medicationCategory);
        }

        // GET: MedicationCategories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var medicationCategory = await _context.MedicationCategories
                .FirstOrDefaultAsync(m => m.MedicationCategoryId == id);
            if (medicationCategory == null)
            {
                return NotFound();
            }

            return View(medicationCategory);
        }

        // POST: MedicationCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var medicationCategory = await _context.MedicationCategories.FindAsync(id);
            _context.MedicationCategories.Remove(medicationCategory);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MedicationCategoryExists(int id)
        {
            return _context.MedicationCategories.Any(e => e.MedicationCategoryId == id);
        }
    }
}
