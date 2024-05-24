using System;

namespace mahjop.Models
{
    public class MedicalHistory
    {
        public int MedicalHistoryId { get; set; } // معرف السيرة المرضية
        public DateTime Date { get; set; } // تاريخ السيرة المرضية
        public string Diagnosis { get; set; } // التشخيص
        public string Notes { get; set; } // ملاحظات إضافية

        // الخصائص الإضافية
        public string DoctorName { get; set; } // اسم الطبيب الذي أجرى الفحص أو أدلى بالتشخيص
        public string Hospital { get; set; } // اسم المستشفى أو المرفق الطبي
        

        // خصائص المريض
        public int PatientId { get; set; } // معرف المريض
        public Patient Patient { get; set; } // مرجع لكائن المريض المرتبط بالسيرة المرضية
    }
}
