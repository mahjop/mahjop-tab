namespace mahjop.Models
{
    public class Medication
    {
        public int MedicationId { get; set; } // معرف الدواء
        public string Name { get; set; } // اسم الدواء
        public string Description { get; set; } // وصف الدواء

        public int DosageFrequency { get; set; } // عدد مرات تناول الدواء

        public int MedicationCategoryId { get; set; }
        public MedicationCategory MedicationCategory { get; set; }

        public int PatientId { get; set; } // معرف المريض
        public Patient Patient { get; set; } // مرجع لكائن المريض المرتبط بالفحص
        public string UserId { get; set; }
    }
}
