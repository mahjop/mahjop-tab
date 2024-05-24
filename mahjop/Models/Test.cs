namespace mahjop.Models
{
    public class Test
    {
        public int TestId { get; set; } // معرف الفحص
        public string Name { get; set; } // اسم الفحص
        public string Result { get; set; } // نتيجة الفحص
        public int PatientId { get; set; } // معرف المريض
        public Patient Patient { get; set; } // مرجع لكائن المريض المرتبط بالفحص
        public string UserId { get; set; }
    }
}
