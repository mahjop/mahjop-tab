using System;

namespace mahjop.Models
{
    public class HealthAssessment
    {
        public int HealthAssessmentId { get; set; } // معرف التقييم الصحي
        public DateTime AssessmentDate { get; set; } // تاريخ التقييم
        public string BloodPressure { get; set; } // ضغط الدم
        public double BloodSugarLevel { get; set; } // نسبة السكر في الدم
        public double Weight { get; set; } // وزن الجسم

        public int PatientId { get; set; } // معرف المريض
        public Patient Patient { get; set; } // مرجع لكائن المريض المرتبط بالتقييم الصحي
    }
}
