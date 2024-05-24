using System;

namespace mahjop.Models
{
    public class Appointment
    {
        public int AppointmentId { get; set; }
       
        public DateTime AppointmentTime { get; set; }
        public DateTime AppointmentDay { get; set; }
        public bool IsConfirmed { get; set; }
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
        public string UserId { get; set; }
    }
}
