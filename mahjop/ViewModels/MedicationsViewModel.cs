using mahjop.Migrations;
using mahjop.Models;
using System.Collections.Generic;

namespace mahjop.ViewModels
{
    public class MedicationsViewModel
    {
        public IList<Medication> Medications { get; set; }
    }
}
