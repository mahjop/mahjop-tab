using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mahjop.ViewModels
{
    public class PermissionFormViewModel
    {
        public string RoleId { get; set; }
        public string RoleName { get; set; }
        public List<RoleViewModel> RoleClaims { get; set; }
    }
}
