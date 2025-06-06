// Models/EditUserRolesViewModel.cs
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations; // For Phone attribute if you want validation

namespace LogisticsWebApp.Models
{
    public class EditUserRolesViewModel
    {
        public string UserId { get; set; }

        [Required]
        [Display(Name = "Username")]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone] // Allows for phone number validation
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; } // Add Phone Number here for editing

        public List<RoleSelection> AllRoles { get; set; } = new List<RoleSelection>();
    }

    public class RoleSelection
    {
        public string RoleName { get; set; }
        public bool IsSelected { get; set; }
    }
}
