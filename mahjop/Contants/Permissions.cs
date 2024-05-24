using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace mahjop.Contants
{
    public static class Permissions
    {
        public static List<string> GenratePermissionList(string module)
        {
            return new List<string>()
            {
                $"Permissions.{module}.View",
                $"Permissions.{module}.Create",
                $"Permissions.{module}.Edit",
                $"Permissions.{module}.Delete",
              
            };
        }
        public static List<string> GenrateAllPermission()
        {
            var allPermission = new List<string>();
            var modules = Enum.GetValues(typeof(Modules));
            foreach (var module in modules)
            {
                allPermission.AddRange(GenratePermissionList(module.ToString()));
            }


            return allPermission;
        }
        public static class Products
        {
           // public const string View = "Permissions.Products.View";
           
        }
        
    }
}
