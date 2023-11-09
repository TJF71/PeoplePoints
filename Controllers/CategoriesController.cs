using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using contactPro2.Data;
using contactPro2.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace contactPro2.Controllers
{
    [Authorize]
    public class CategoriesController : CPBaseController
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IEmailSender _emailService;

        public CategoriesController(ApplicationDbContext context,
                                    UserManager<AppUser> userManager,
                                    IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _emailService = emailSender;

        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            //string? userId = _userManager.GetUserId(User);

            IEnumerable<Category> categories = await _context.Categories.Where(c => c.AppUserId == _userId)
                                                                        .ToListAsync();

            return View(categories);
        }

        // GET: Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AppUserId,Name")] Category category)
        {
            ModelState.Remove("AppUserId");
            if (ModelState.IsValid)
            {
                //string? userId = _userManager.GetUserId(User);
                category.AppUserId = _userId;

                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }


        // GET: Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", category.AppUserId);
            return View(category);
        }

        // POST: Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,Name")] Category category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", category.AppUserId);
            return View(category);
        }

        [HttpGet]
        public async Task<IActionResult> EmailCategory(int? id, string? swalMessage)
        {
            if (id == null)
            {
                return NotFound();
            }

            ViewData["SwalMessage"] = swalMessage;

            //string? userId = _userManager?.GetUserId(User);
            Category? category = await _context.Categories
                                        .Include(c => c.Contacts)
                                        .FirstOrDefaultAsync(c => c.Id == id && c.AppUserId == _userId);

            if (category == null)
            {
                return NotFound();
            }

            IEnumerable<string?> emails = category.Contacts.Select(c => c.Email);
            EmailData emailData = new EmailData()
            {
                GroupName = category.Name,
                EmailAddress = string.Join(";", emails),
                EmailSubject = $"Group Message for the {category.Name} category"
            };

            ViewData["CategoryId"] = id;
            ViewData["EmailContacts"] = category.Contacts.ToList();
            return View(emailData);

        }


        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> EmailCategory(EmailData emailData, int? categoryId)

        {
            string? swalMessage = string.Empty;

            if (ModelState.IsValid)
            {
                // sweet alert

                ViewData["SwalMessage"] = string.Empty;

                try
                {


                    string? email = emailData.EmailAddress;
                    string? subject = emailData.EmailSubject;
                    string? htmlMessage = emailData.EmailBody;

                    await _emailService.SendEmailAsync(email!, subject!, htmlMessage!);
                    swalMessage = "Success: Email Sent!";

                }

                catch (Exception)
                {
                    swalMessage = "Error: Email Failed to Send!";
                    throw;
                }
            }

            //string? userId = _userManager?.GetUserId(User);
            Category? category = await _context.Categories
                                        .Include(c => c.Contacts)
                                        .FirstOrDefaultAsync(c => c.Id == categoryId && c.AppUserId == _userId);

            //testing
            ViewData["SwalMessage"] = swalMessage;
            ViewData["CategoryId"] = category?.Id;
            ViewData["EmailContacts"] = category?.Contacts.ToList();

            return View(emailData);

        }



        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Categories == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Categories == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Categories'  is null.");
            }
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }


        private bool CategoryExists(int id)
        {
            return (_context.Categories?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}

//// Get: SearchCategories
//public async Task<IActionResult> SearchCategories(string? searchString)
//{
//    List<Category> categories = new List<Category>();

//    string? userId = _userManager.GetUserId(User);

//    AppUser? appUser = await _context.Users
//                                     .Include(u => u.Contacts)
//                                     .ThenInclude(c => c.Categories)
//                                     .FirstOrDefaultAsync(u => u.Id == userId);
//    if (appUser != null)
//    {
//        if (string.IsNullOrEmpty(searchString))
//        {
//            categories = appUser.Categories.ToList();
//        }
//        else
//        {
//            categories = appUser.Categories.ToList();

//        }

//    }
//    else
//    {
//        return NotFound();
//    }

//    return View(nameof(Index), categories);
//}

