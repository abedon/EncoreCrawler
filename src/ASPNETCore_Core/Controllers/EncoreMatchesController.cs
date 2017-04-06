using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPNETCore_Core.Data;
using ASPNETCore_Core.Models;
using Microsoft.AspNetCore.Authorization;

namespace ASPNETCore_Core.Controllers
{
    [Authorize(Roles = "ADMIN, Admin")]
    public class EncoreMatchesController : Controller
    {
        private readonly EncoreContext _context;

        public EncoreMatchesController(EncoreContext context)
        {
            _context = context;    
        }

        [AllowAnonymous]
        // GET: EncoreMatches
        public async Task<IActionResult> Index()
        {
            var encoreContext = _context.EncoreMatches.Include(e => e.Encore);
            return View(await encoreContext.ToListAsync());
        }

        [AllowAnonymous]
        // GET: EncoreMatches
        public async Task<IActionResult> Get(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encoreContext = _context.EncoreMatches.Where(m => m.EncoreID == id).Include(e => e.Encore);
            return View("Index", await encoreContext.OrderBy(m => m.MatchType).ToListAsync());
        }

        [AllowAnonymous]
        // GET: EncoreMatches/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encoreMatch = await _context.EncoreMatches.SingleOrDefaultAsync(m => m.EncoreMatchID == id);
            if (encoreMatch == null)
            {
                return NotFound();
            }

            return View(encoreMatch);
        }

        // GET: EncoreMatches/Create
        public IActionResult Create(int id)
        {
            var encore = _context.Encores.Where(m => m.EncoreID == id).Select(m => m);
            if (encore == null || encore.Count() == 0)
            {
                return NotFound();
            }

            ViewData["EncoreID"] = new SelectList(encore, "EncoreID", "EncoreID", id);
            ViewData["EncoreIDValue"] = id;
            return View();
        }

        // POST: EncoreMatches/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EncoreMatchID,EncoreID,MatchType,Prize,TicketsWon")] EncoreMatch encoreMatch)
        {
            if (ModelState.IsValid)
            {
                _context.Add(encoreMatch);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Encores", new { id = encoreMatch.EncoreID });
            }
            ViewData["EncoreID"] = new SelectList(_context.Encores, "EncoreID", "EncoreID", encoreMatch.EncoreID);
            return View(encoreMatch);
        }

        // GET: EncoreMatches/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encoreMatch = await _context.EncoreMatches.SingleOrDefaultAsync(m => m.EncoreMatchID == id);
            if (encoreMatch == null)
            {
                return NotFound();
            }
            ViewData["EncoreID"] = new SelectList(_context.Encores, "EncoreID", "EncoreID", encoreMatch.EncoreID);
            return View(encoreMatch);
        }

        // POST: EncoreMatches/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EncoreMatchID,EncoreID,MatchType,Prize,TicketsWon")] EncoreMatch encoreMatch)
        {
            if (id != encoreMatch.EncoreMatchID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(encoreMatch);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EncoreMatchExists(encoreMatch.EncoreMatchID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Details", "Encores", new { id = encoreMatch.EncoreID });
            }
            ViewData["EncoreID"] = new SelectList(_context.Encores, "EncoreID", "EncoreID", encoreMatch.EncoreID);
            return View(encoreMatch);
        }

        // GET: EncoreMatches/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encoreMatch = await _context.EncoreMatches.SingleOrDefaultAsync(m => m.EncoreMatchID == id);
            if (encoreMatch == null)
            {
                return NotFound();
            }

            return View(encoreMatch);
        }

        // POST: EncoreMatches/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var encoreMatch = await _context.EncoreMatches.SingleOrDefaultAsync(m => m.EncoreMatchID == id);
            _context.EncoreMatches.Remove(encoreMatch);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", "Encores", new { id = encoreMatch.EncoreID });
        }

        private bool EncoreMatchExists(int id)
        {
            return _context.EncoreMatches.Any(e => e.EncoreMatchID == id);
        }
    }
}
