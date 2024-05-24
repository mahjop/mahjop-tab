using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace mahjop.Models
{
    public class User:IdentityUser
    {
        [DefaultValue("Unknown")]
        public string FirstName { get; set; }
        [DefaultValue("Unknown")]
        public string LastName { get; set; }
        public byte[] ProfilePicture { get; set; }

    }
}
