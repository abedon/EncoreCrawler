using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ASPNETCore_Core.Data;
using ASPNETCore_Core.Models;
using ASPNETCore_Core.Services;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authorization;

namespace ASPNETCore_Core.Controllers
{
    [Authorize]
    public class EncoresController : Controller
    {
        private readonly EncoreContext _context;
        private readonly IEncoreCrawler _encoreCrawler;

        public EncoresController(EncoreContext context, IEncoreCrawler encoreCrawler)
        {
            _context = context;
            _encoreCrawler = encoreCrawler;
        }

        [AllowAnonymous]
        // GET: Encores
        public async Task<IActionResult> Index()
        {
            return View(await _context.Encores.Take(200).ToListAsync());
        }

        [AllowAnonymous]
        // GET: Encores
        public async Task<IActionResult> IndexByType(LottoTypeEnum lottoType)
        {
            return View("Index", await _context.Encores.Where(c => c.LottoType == lottoType)                                                       
                                                       .OrderByDescending(c => c.DrawDate)
                                                       .ThenByDescending(c => c.DrawType)
                                                       .Take(100)
                                                       .ToListAsync());
        }

        [AllowAnonymous]
        // GET: Encores/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encore = await _context.Encores.SingleOrDefaultAsync(m => m.EncoreID == id);
            if (encore == null)
            {
                return NotFound();
            }

            //populate encore matches
            encore.EncoreMatches = await _context.EncoreMatches.Where(m => m.EncoreID == encore.EncoreID).ToListAsync();
            
            return View(encore);
        }

        // GET: Encores/Create
        public IActionResult Create()
        {
            return View();
        }

        public async Task<IActionResult> Craw()
        {
            await _encoreCrawler.CrawlAsync(_context.Encores);
            //save encore and encore matches
            await _context.SaveChangesAsync();

            return View("Index", await _context.Encores.Where(c => c.LottoType == LottoTypeEnum.LottoMAX)
                                                       .OrderByDescending(c => c.DrawDate)
                                                       .ThenByDescending(c => c.DrawType)
                                                       .Take(100)
                                                       .ToListAsync());
        }

        // POST: Encores/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EncoreID,LottoType,DrawDate,DrawType,TotalCashWon,WinninNumber")] Encore encore)
        {
            if (ModelState.IsValid)
            {
                _context.Add(encore);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(encore);
        }

        // GET: Encores/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encore = await _context.Encores.SingleOrDefaultAsync(m => m.EncoreID == id);
            if (encore == null)
            {
                return NotFound();
            }
            return View(encore);
        }

        // POST: Encores/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EncoreID,DrawDate,TotalCashWon,WinninNumber")] Encore encore)
        {
            if (id != encore.EncoreID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(encore);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EncoreExists(encore.EncoreID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            return View(encore);
        }

        // GET: Encores/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var encore = await _context.Encores.SingleOrDefaultAsync(m => m.EncoreID == id);
            if (encore == null)
            {
                return NotFound();
            }

            return View(encore);
        }

        // POST: Encores/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var encore = await _context.Encores.SingleOrDefaultAsync(m => m.EncoreID == id);
            var encoreMatches = await _context.EncoreMatches.Where(m => m.EncoreID == encore.EncoreID).ToListAsync();
            encore.EncoreMatches = encoreMatches;
            _context.Encores.Remove(encore);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", await _context.Encores.Where(c => c.LottoType == encore.LottoType)
                                                                   .OrderByDescending(c => c.DrawDate)
                                                                   .ThenByDescending(c => c.DrawType)
                                                                   .Take(100)
                                                                   .ToListAsync());
        }

        private bool EncoreExists(int id)
        {
            return _context.Encores.Any(e => e.EncoreID == id);
        }

        // GET: Encores/Check
        [AllowAnonymous]
        public IActionResult Check()
        {
            return View();
        }

        // POST: Encores/Check
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Check([Bind("EncoreID,LottoType,DrawDate,DrawType,TotalCashWon,WinninNumber")] Encore encoreToCheck)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var encore = await _context.Encores.SingleOrDefaultAsync(m => m.LottoType == encoreToCheck.LottoType &&
                                                                             m.DrawDate == encoreToCheck.DrawDate &&
                                                                             m.DrawType == encoreToCheck.DrawType);
                    if (encore == null)
                    {
                        return NotFound();
                    }

                    encore.EncoreMatches = await _context.EncoreMatches.Where(m => m.EncoreID == encore.EncoreID).ToListAsync();

                    //get the matchtype of the number to check
                    var matchType = _encoreCrawler.CheckEncoreMatchType(encoreToCheck.WinninNumber, encore.WinninNumber, encore.EncoreMatches);
                    encoreToCheck.EncoreMatches = new List<EncoreMatch>();
                    encoreToCheck.EncoreMatches.Add(
                        new EncoreMatch
                        {
                            MatchType = matchType,
                            Prize = encore.EncoreMatches.Where(m => m.MatchType == matchType).Select(m => m.Prize).SingleOrDefault(),
                        });                    
                }
                catch (Exception)
                {
                    throw;
                }
            }
            return View(encoreToCheck);
        }
    }
}
