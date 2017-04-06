using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ASPNETCore_Core.Data;
using ASPNETCore_Core.Models;
using ASPNETCore_Core.Services;

namespace ASPNETCore_Core.Controllers
{
    [Produces("application/json")]
    [Route("api/EncoresApi")]
    public class EncoresApiController : Controller
    {
        private readonly EncoreContext _context;
        private readonly IEncoreCrawler _encoreCrawler;

        public EncoresApiController(EncoreContext context, IEncoreCrawler encoreCrawler)
        {
            _context = context;
            _encoreCrawler = encoreCrawler;
        }

        // GET: api/EncoresApi
        [HttpGet]
        public IEnumerable<Encore> GetEncores()
        {
            return _context.Encores;
        }

        // GET: api/EncoresApi/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetEncore([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Encore encore = await _context.Encores.SingleOrDefaultAsync(m => m.EncoreID == id);

            if (encore == null)
            {
                return NotFound();
            }

            return Ok(encore);
        }

        // GET: api/EncoresApi/lottomax/2017-02-24/evening/1235031
        [HttpGet("{lottoType}/{dtDraw}/{drawType}/{numToCheck}")]
        public async Task<IActionResult> Check([FromRoute] string numToCheck, DateTime dtDraw, LottoTypeEnum lottoType, DrawTypeEnum drawType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var encore = await _context.Encores.SingleOrDefaultAsync(m => m.LottoType ==lottoType &&
                                                                             m.DrawDate == dtDraw &&
                                                                             m.DrawType == drawType);

            if (encore == null)
            {
                return NotFound();
            }

            encore.EncoreMatches = await _context.EncoreMatches.Where(m => m.EncoreID == encore.EncoreID).ToListAsync();
                       
            var encoreToCheck = new Encore
            {
                EncoreID = encore.EncoreID,
                LottoType = encore.LottoType,
                DrawDate = encore.DrawDate,
                DrawType = encore.DrawType,
                TotalCashWon = encore.TotalCashWon,
                WinninNumber = encore.WinninNumber,
                EncoreMatches = new List<EncoreMatch>()
            };

            //get the matchtype of the number to check
            var matchType = _encoreCrawler.CheckEncoreMatchType(numToCheck, encore.WinninNumber, encore.EncoreMatches);
            var encoreMatch = encore.EncoreMatches.Where(m => m.MatchType == matchType).SingleOrDefault();

            if (encoreMatch == null)
            {
                encoreToCheck.EncoreMatches.Add(
                new EncoreMatch
                {
                    EncoreID = encore.EncoreID,
                    MatchType = matchType,
                });
            }
            else
            {
                encoreToCheck.EncoreMatches.Add(
                    new EncoreMatch
                    {
                        EncoreID = encore.EncoreID,
                        EncoreMatchID = encoreMatch.EncoreMatchID,
                        MatchType = encoreMatch.MatchType,
                        Prize = encoreMatch.Prize,
                    });
            }

            return Ok(encoreToCheck);
        }

        // PUT: api/EncoresApi/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEncore([FromRoute] int id, [FromBody] Encore encore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != encore.EncoreID)
            {
                return BadRequest();
            }

            _context.Entry(encore).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EncoreExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/EncoresApi
        [HttpPost]
        public async Task<IActionResult> PostEncore([FromBody] Encore encore)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Encores.Add(encore);
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                if (EncoreExists(encore.EncoreID))
                {
                    return new StatusCodeResult(StatusCodes.Status409Conflict);
                }
                else
                {
                    throw;
                }
            }

            return CreatedAtAction("GetEncore", new { id = encore.EncoreID }, encore);
        }

        // DELETE: api/EncoresApi/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEncore([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Encore encore = await _context.Encores.SingleOrDefaultAsync(m => m.EncoreID == id);
            if (encore == null)
            {
                return NotFound();
            }

            _context.Encores.Remove(encore);
            await _context.SaveChangesAsync();

            return Ok(encore);
        }

        private bool EncoreExists(int id)
        {
            return _context.Encores.Any(e => e.EncoreID == id);
        }
    }
}