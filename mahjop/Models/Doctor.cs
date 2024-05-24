using System;

namespace mahjop.Models
{
    public class Doctor
    {
        public int DoctorId { get; set; }
        public string NameDoctor { get; set; }
        public string DescriptionDoctor { get; set; }
        public string Address { get; set; }
        public double Rating { get; set; } // التقييم
        public byte[] DoctorPicture { get; set; }
        public bool Isactive { get; set; }
        public int DepartmentId { get; set; }
        public DateTime AppointmentTime { get; set; }
        public DateTime AppointmentDay { get; set; }
        public Department Department { get; set; }
        public string UserId { get; set; }


    }
}
