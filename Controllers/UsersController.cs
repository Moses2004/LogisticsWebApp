// Controllers/UsersController.cs
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LogisticsWebApp.Models; // For UserWithRolesViewModel and EditUserRolesViewModel

// Authorize: Only users with the "Admin" or "Manager" role can access this controller.
[Authorize(Roles = "Admin,Manager")]
public class UsersController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager; // Inject RoleManager for role management

    // Constructor to inject UserManager and RoleManager
    public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // GET: Users (Lists all users with their assigned roles)
    public async Task<IActionResult> Index()
    {
        var users = await _userManager.Users.ToListAsync();
        var usersWithRoles = new List<UserWithRolesViewModel>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            usersWithRoles.Add(new UserWithRolesViewModel
            {
                Id = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                Roles = roles,
                PhoneNumber = user.PhoneNumber
            });
        }
        return View(usersWithRoles);
    }

    // GET: Users/Details/{id} (Shows details of a specific user)
    public async Task<IActionResult> Details(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userDetails = new UserWithRolesViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles,
            PhoneNumber = user.PhoneNumber
        };

        return View(userDetails);
    }

    // GET: Users/Edit/{id} (Displays the form to edit an existing user's roles)
    public async Task<IActionResult> Edit(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var userRoles = await _userManager.GetRolesAsync(user);
        var allRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

        var model = new EditUserRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            AllRoles = allRoles.Select(role => new RoleSelection
            {
                RoleName = role,
                IsSelected = userRoles.Contains(role)
            }).ToList()
        };
        return View(model);
    }

    // POST: Users/Edit/{id} (Handles the form submission for editing user roles)
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(string id, EditUserRolesViewModel model)
    {
        if (id != model.UserId)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        // Update basic user properties if they are part of the form
        if (user.PhoneNumber != model.PhoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, model.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                foreach (var error in setPhoneResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }

        // Before re-populating AllRoles for the view, fetch the current user's roles
        // and all available roles outside of the LINQ query.
        var currentUserRoles = await _userManager.GetRolesAsync(user);
        var availableRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();


        // Update user roles based on model.AllRoles
        // Ensure model.AllRoles is not null before processing
        if (model.AllRoles != null)
        {
            var selectedRoles = model.AllRoles.Where(r => r.IsSelected).Select(r => r.RoleName).ToList();

            // Roles to add
            var rolesToAdd = selectedRoles.Except(currentUserRoles);
            var addResult = await _userManager.AddToRolesAsync(user, rolesToAdd);
            if (!addResult.Succeeded)
            {
                foreach (var error in addResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            // Roles to remove
            var rolesToRemove = currentUserRoles.Except(selectedRoles);
            var removeResult = await _userManager.RemoveFromRolesAsync(user, rolesToRemove);
            if (!removeResult.Succeeded)
            {
                foreach (var error in removeResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        }
        else
        {
            // Handle case where model.AllRoles is null, e.g., if no checkboxes were rendered
            ModelState.AddModelError(string.Empty, "Role selection data is missing.");
        }

        if (ModelState.IsValid)
        {
            // Identity updates are typically managed by UserManager and persist immediately.
            // No explicit _context.Update(user) or _context.SaveChangesAsync() needed for role/phone changes.
            return RedirectToAction(nameof(Index));
        }

        // If ModelState is not valid after attempting role/phone updates, re-display the form
        // Re-populate AllRoles for the view based on current user roles and all available roles
        model.AllRoles = availableRoles.Select(role => new RoleSelection
        {
            RoleName = role,
            IsSelected = currentUserRoles.Contains(role) // Use the already fetched currentUserRoles
        }).ToList();

        return View(model);
    }


    // GET: Users/Delete/{id} (Displays the confirmation page for user deletion)
    public async Task<IActionResult> Delete(string id)
    {
        if (id == null)
        {
            return NotFound();
        }

        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
        {
            return NotFound();
        }

        var roles = await _userManager.GetRolesAsync(user);
        var userDetails = new UserWithRolesViewModel
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Roles = roles,
            PhoneNumber = user.PhoneNumber
        };

        return View(userDetails);
    }

    // POST: Users/Delete/{id} (Handles the deletion of a user)
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                // If deletion fails, return to the delete confirmation page with errors
                var roles = await _userManager.GetRolesAsync(user);
                var userDetails = new UserWithRolesViewModel
                {
                    Id = user.Id,
                    UserName = user.UserName,
                    Email = user.Email,
                    Roles = roles,
                    PhoneNumber = user.PhoneNumber
                };
                return View(userDetails);
            }
        }
        return RedirectToAction(nameof(Index));
    }
}
