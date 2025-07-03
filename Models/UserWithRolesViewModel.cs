// Models/UserWithRolesViewModel.cs - UPDATED
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity; // Needed for IdentityUser properties

namespace LogisticsWebApp.Models
{
    public class UserWithRolesViewModel
    {
        public string Id { get; set; } 
        public string UserName { get; set; } 
        public string Email { get; set; } 
        public IEnumerable<string> Roles { get; set; } = new List<string>();
        public string PhoneNumber { get; set; }
    }
}
