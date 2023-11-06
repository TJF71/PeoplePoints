﻿using System;
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
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Identity;
using contactPro2.Services.Interfaces;








namespace contactPro2.Controllers
{
    [Authorize]
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<AppUser> _userManager;
        private readonly IImageService _imageService;

        public ContactsController(ApplicationDbContext context,
                                    UserManager<AppUser> userManager,
                                    IImageService imageService)

        {
            _context = context;
            _userManager = userManager;
            _imageService = imageService;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            string? userId = _userManager.GetUserId(User);

            IEnumerable<Contact> contacts = await _context.Contacts
                                                     .Include(c => c.Categories)
                                                     .Where(c => c.AppUserId == userId)
                                                     .ToListAsync();
            return View(contacts);
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            Contact? contact = await _context.Contacts
                              .Include(c => c.Categories)
                              .FirstOrDefaultAsync(c => c.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public IActionResult Create()
        {
            string? userId = _userManager?.GetUserId(User);

            ViewData["CategoryList"] = new MultiSelectList(_context.Categories.Where(c => c.AppUserId == userId), "Id", "Name");
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Create([Bind("FirstName,LastName,DateOfBirth,Address1,Address2,City,State,ZipCodde,Email,PhoneNumber, ImageFile")] Contact contact, IEnumerable<int> selected)
        {

            ModelState.Remove("AppUserId");

            if (ModelState.IsValid)
            {
                contact.AppUserId = _userManager.GetUserId(User);
                contact.Created = DateTimeOffset.Now;

                if(contact.ImageFile != null)
                {
                    // use image service
                    contact.ImageData = await _imageService.ConvertFileToByteArrayAsync(contact.ImageFile);
                    contact.ImageType = contact.ImageFile.ContentType;
                }

                _context.Add(contact);
                await _context.SaveChangesAsync();

                foreach (int categoryId in selected)
                {
                    Category? category = await _context.Categories.FindAsync(categoryId);

                    if (contact != null && category != null)
                    {
                        contact.Categories.Add(category);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));

            }

            // ModelState is not valid and I am being redirect to the view
            string? userId = _userManager?.GetUserId(User);

            ViewData["CategoryList"] = new SelectList(_context.Categories.Where(c => c.AppUserId == userId), "Id", "Name");
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            Contact? contact = await _context.Contacts.Include(c => c.Categories).FirstOrDefaultAsync(c => c.Id == id);


            if (contact == null)
            {
                return NotFound();
            }

            // Identify the contact's current Categories (by Id) 
            IEnumerable<int> currentCategories = contact.Categories.Select(c => c.Id);

            string? userId = _userManager.GetUserId(User);

            ViewData["CategoryList"] = new MultiSelectList(_context.Categories.Where(c => c.AppUserId == userId), "Id", "Name", currentCategories);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AppUserId,FirstName,LastName,Created,Updated,DateOfBirth,Address1,Address2,City,State,ZipCode,Email,PhoneNumber,ImageFile, ImageData,ImageType")] Contact contact, IEnumerable<int> selected)
        {
            if (id != contact.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    contact.Updated = DateTimeOffset.Now;

                    _context.Update(contact);
                    await _context.SaveChangesAsync();

                    // Removing current categories
                    Contact? updatedContact = await _context.Contacts
                                                    .Include(c => c.Categories)
                                                    .FirstOrDefaultAsync(c => c.Id == contact.Id);

                    updatedContact?.Categories.Clear();
                    _context.Update(updatedContact);
                    await _context.SaveChangesAsync();


                    // Adding selected categories
                    foreach (int categoryId in selected)
                    {
                        Category? category = await _context.Categories.FindAsync(categoryId);

                        if (contact != null && category != null)
                        {
                            contact.Categories.Add(category);
                        }
                    }

                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ContactExists(contact.Id))
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
            ViewData["AppUserId"] = new SelectList(_context.Users, "Id", "Id", contact.AppUserId);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.AppUser)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
            return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
