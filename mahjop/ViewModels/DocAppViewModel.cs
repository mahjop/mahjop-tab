using mahjop.Models;
using System.Collections.Generic;

namespace mahjop.ViewModels
{
    public class DocAppViewModel
    {
        public IList<Doctor> Doctors { get; set; }
        public IList<Appointment> Appointments { get; set; }
        public IList<Department> Departments { get; set; }
    }
}
