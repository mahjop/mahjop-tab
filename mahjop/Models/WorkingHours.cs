using System;

namespace mahjop.Models
{
    public class WorkingHours
    {
        public int WorkingHoursId { get; set; }
        public DayOfWeek Day { get; set; } // يوم الأسبوع
        public TimeSpan StartTime { get; set; } // وقت بدء العمل
        public TimeSpan EndTime { get; set; } // وقت نهاية العمل
        public int DoctorId { get; set; }
        public Doctor Doctor { get; set; }
    }
}
