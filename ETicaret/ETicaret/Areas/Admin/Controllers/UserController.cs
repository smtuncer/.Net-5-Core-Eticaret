using ETicaret.Data;
using ETicaret.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ETicaret.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = Diger.Role_Admin)]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var users = _context.applicationUsers.ToList();
            var role = _context.Roles.ToList();
            var userRole = _context.UserRoles.ToList();
            foreach (var item in users)
            {
                var roleId = userRole.FirstOrDefault(i => i.UserId == item.Id).RoleId;
                item.Role = role.FirstOrDefault(u => u.Id == roleId).Name;
            }
            return View(users);
        }
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.applicationUsers
                .FirstOrDefaultAsync(m => m.Id == id.ToString());
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var user = await _context.applicationUsers.FindAsync(id);
            _context.applicationUsers.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
