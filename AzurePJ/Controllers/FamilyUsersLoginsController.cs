using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using AzurePJ.DbContexts;
using AzurePJ.Models;
using Microsoft.IdentityModel.Tokens;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

namespace AzurePJ.Controllers
{
    public class FamilyUsersLoginsController : Controller
    {
        private readonly DbToDoListContext _context;

        public FamilyUsersLoginsController(DbToDoListContext context)
        {
            _context = context;
        }

        // GET: FamilyUsersLogins
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(FamilyUsersLogin user)
        {
            //if (!ModelState.IsValid)
            //{
            //    return View(user);
            //}
            if (string.IsNullOrEmpty(user.UserName) || string.IsNullOrEmpty(user.Email))
            {
                return NotFound();
            }
            var users = await _context.FamilyUsersLogins.FirstOrDefaultAsync(x => x.UserName == user.UserName && x.Email == user.Email);

            //Users存在しません
            if (users == null)
            {
                return View("Error1");
            }
            return RedirectToAction("Edit");
        }

        // GET: FamilyUsersLogins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familyUsersLogin = await _context.FamilyUsersLogins
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (familyUsersLogin == null)
            {
                return NotFound();
            }

            return View(familyUsersLogin);
        }

        // GET: FamilyUsersLogins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FamilyUsersLogins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,UserName,Relationship,Email,Phone,IsActive,CreatedAt,LastLogin")] FamilyUsersLogin familyUsersLogin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(familyUsersLogin);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(familyUsersLogin);
        }

        // GET: FamilyUsersLogins/Edit/5
        public IActionResult Edit()
        {

            return View();
        }

        // POST: FamilyUsersLogins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("UserId,UserName,Relationship,Email,Phone,IsActive,CreatedAt,LastLogin")] FamilyUsersLogin familyUsersLogin)
        {
            if (id != familyUsersLogin.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(familyUsersLogin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FamilyUsersLoginExists(familyUsersLogin.UserId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(familyUsersLogin);
        }

        // GET: FamilyUsersLogins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var familyUsersLogin = await _context.FamilyUsersLogins
                .FirstOrDefaultAsync(m => m.UserId == id);
            if (familyUsersLogin == null)
            {
                return NotFound();
            }

            return View(familyUsersLogin);
        }

        // POST: FamilyUsersLogins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var familyUsersLogin = await _context.FamilyUsersLogins.FindAsync(id);
            if (familyUsersLogin != null)
            {
                _context.FamilyUsersLogins.Remove(familyUsersLogin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FamilyUsersLoginExists(int id)
        {
            return _context.FamilyUsersLogins.Any(e => e.UserId == id);
        }
    }
}
