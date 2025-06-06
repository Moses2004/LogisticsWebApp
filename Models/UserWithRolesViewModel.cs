// Models/UserWithRolesViewModel.cs - UPDATED
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity; // Needed for IdentityUser properties

namespace LogisticsWebApp.Models
{
    public class UserWithRolesViewModel
    {
        public string Id { get; set; } // Matches IdentityUser's Id
        public string UserName { get; set; } // Matches IdentityUser's UserName
        public string Email { get; set; } // Matches IdentityUser's Email
        public IEnumerable<string> Roles { get; set; } = new List<string>(); // List of role names
        public string PhoneNumber { get; set; } // ADDED: Phone Number property
    }
}
