using mahjop.Models; // استيراد النماذج المرتبطة بقاعدة البيانات
using System.Collections.Generic;
using System;
using System.Threading.Tasks;
using mahjop.Data;
using System.Linq;

namespace mahjop.Seeds
{
    public static class DefaultMedicationCategory
    {
        public static async Task SeedAsync(ApplicationDbContext context)
        {
            // التحقق مما إذا كانت قاعدة البيانات لا تحتوي بالفعل على بيانات في جدول MedicationCategory
            if (!context.MedicationCategories.Any())
            {
                // إضافة بيانات افتراضية إلى جدول MedicationCategory هنا
                var defaultCategories = new List<MedicationCategory>
                {
                    new MedicationCategory { Name = "Allergy" },
                    new MedicationCategory { Name = "Daily" },
                    new MedicationCategory { Name = "Monthly" },
                    // يمكنك إضافة المزيد من الفئات هنا حسب الحاجة
                };

                // إضافة الفئات الافتراضية إلى قاعدة البيانات
                await context.MedicationCategories.AddRangeAsync(defaultCategories);

                // حفظ التغييرات في قاعدة البيانات
                await context.SaveChangesAsync();
            }
        }
    }
}
