using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ASPNETCore_Core.Models;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore_Core.Services
{
    public interface IEncoreCrawler
    {
        Task CrawlAsync(DbSet<Encore> encores);

        MatchTypeEnum CheckEncoreMatchType(string numberToCheck, string numberWon, IEnumerable<EncoreMatch> encoreMatches);
    }
}
