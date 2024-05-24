using System;

namespace mahjop.Models
{
    public class Patient
    {

        public int PatientId { get; set; } // معرف المريض

        public string FullName { get; set; } // الاسم الكامل للمريض

        public DateTime DateOfBirth { get; set; } // تاريخ ميلاد المريض

        public string Gender { get; set; } // الجنس

        public string Address { get; set; } // عنوان المريض

        public string PhoneNumber { get; set; } // رقم هاتف المريض

        public double Weight { get; set; } // الوزن بالكيلوجرام

        public double Height { get; set; } // الطول بالسنتيمتر
        public string UserId { get; set; }
      


    }
}
