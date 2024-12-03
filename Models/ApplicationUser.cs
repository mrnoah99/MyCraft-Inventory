using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace MyCraft_Inventory.Models
{
    public class ApplicationUser : IdentityUser
    {
        public bool IsEmployee { get; set; }
        public string? EmployeeCode { get; set; }
    }
}