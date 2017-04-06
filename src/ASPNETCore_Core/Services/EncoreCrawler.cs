using ASPNETCore_Core.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;

namespace ASPNETCore_Core.Services
{
    public class EncoreCrawler : IEncoreCrawler
    {
        public Task CrawlAsync(DbSet<Encore> encores)
        {
            CrawLottoMax(encores);
            CrawLotto649(encores);
            CrawDailyGrand(encores);
            CrawOntario49(encores);
            CrawLottario(encores);
            CrawDailyKeno(encores);
            CrawPick4(encores);
            CrawPick3(encores);
            CrawPick2(encores);

            return Task.FromResult(0);
        }

        public MatchTypeEnum CheckEncoreMatchType(string numberToCheck, string numberWon, IEnumerable<EncoreMatch> encoreMatches)
        {
            if (numberToCheck.Length != numberWon.Length)
                return MatchTypeEnum.OOOOOOO;

            //get the actual match type
            var charNumberToCheck = numberToCheck.ToCharArray();
            var charNumberWon = numberWon.ToCharArray();

            string actualMatchType = "";

            for (int i = 0; i < charNumberToCheck.Length; i++)
            {
                if (charNumberToCheck[i] == charNumberWon[i])
                    actualMatchType += "I";
                else
                    actualMatchType += "O";
            }

            if (actualMatchType == "OOOOOOO")
                return MatchTypeEnum.OOOOOOO;

            //find the qualified match type(s)
            var qualifiedMatches = new List<EncoreMatch>();
            var qualifiedMatchPrices = new List<double>();
            var matchTypes = Enum.GetValues(typeof(MatchTypeEnum));

            foreach (MatchTypeEnum matchType in matchTypes)
            {
                var qualifiedMatchType = "";
                for (int i = 0; i < actualMatchType.Length; i++)
                {
                    //0-And bwteen actual and honorable match types
                    if (matchType.ToString()[i] == 'O' || actualMatchType[i] == 'O')
                        qualifiedMatchType += "O";
                    else
                        qualifiedMatchType += "I";
                }

                //we consider the match type is qualified if the 0-And result (between actual and honorable) 
                //equals to the honorable match type
                if (matchType.ToString() == qualifiedMatchType && matchType != MatchTypeEnum.OOOOOOO)
                {
                    var qualifiedMatch = encoreMatches.Where(m => m.MatchType == matchType).Select(m => m).SingleOrDefault();
                    qualifiedMatches.Add(qualifiedMatch);
                    qualifiedMatchPrices.Add(encoreMatches.Where(m => m.MatchType == matchType).Select(m => m.Prize).SingleOrDefault());
                }
            }

            //select the one with the highest price as the final match type
            if (qualifiedMatches.Count <= 0)
                return MatchTypeEnum.OOOOOOO;

            var maxPrize = qualifiedMatchPrices.Max();
            return qualifiedMatches.Where(m => m.Prize == maxPrize).Select(m => m.MatchType).FirstOrDefault();
        }

        private void CrawLottoMax(DbSet<Encore> encores)
        {
            var dtStartDraw = new DateTime(2016, 3, 4, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "lotto-max", LottoTypeEnum.LottoMAX, dtStartDraw);
        }

        private void CrawLotto649(DbSet<Encore> encores)
        {
            //Wednesday
            var dtStartDraw = new DateTime(2016, 3, 2, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "lotto-649", LottoTypeEnum.Lotto649, dtStartDraw);

            //Saturday
            dtStartDraw = new DateTime(2016, 3, 5, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "lotto-649", LottoTypeEnum.Lotto649, dtStartDraw);
        }

        private void CrawDailyGrand(DbSet<Encore> encores)
        {
            //Thursday
            var dtStartDraw = new DateTime(2016, 10, 20, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "daily-grand", LottoTypeEnum.DailyGrand, dtStartDraw);

            //Monday
            dtStartDraw = new DateTime(2016, 10, 24, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "daily-grand", LottoTypeEnum.DailyGrand, dtStartDraw);
        }

        private void CrawOntario49(DbSet<Encore> encores)
        {
            //Wednesday
            var dtStartDraw = new DateTime(2016, 3, 2, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "ontario-49", LottoTypeEnum.Ontario49, dtStartDraw);

            //Saturday
            dtStartDraw = new DateTime(2016, 3, 5, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "ontario-49", LottoTypeEnum.Ontario49, dtStartDraw);
        }

        private void CrawLottario(DbSet<Encore> encores)
        {
            //Saturday
            var dtStartDraw = new DateTime(2016, 3, 5, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "lottario", LottoTypeEnum.Lottario, dtStartDraw, "2");
        }

        private void CrawDailyKeno(DbSet<Encore> encores)
        {
            var dtStartDraw = new DateTime(2016, 3, 1, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "daily-keno", LottoTypeEnum.DailyKeno, dtStartDraw, "-midday-encore", 1, DrawTypeEnum.Midday);
            CrawlCore(encores, "daily-keno", LottoTypeEnum.DailyKeno, dtStartDraw, "-evening-encore", 1);
        }

        private void CrawPick4(DbSet<Encore> encores)
        {
            var dtStartDraw = new DateTime(2016, 3, 1, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "pick-4", LottoTypeEnum.Pick4, dtStartDraw, "pick-game2", 1, DrawTypeEnum.Midday);
            CrawlCore(encores, "pick-4", LottoTypeEnum.Pick4, dtStartDraw, "pick-game4", 1);
        }

        private void CrawPick3(DbSet<Encore> encores)
        {
            var dtStartDraw = new DateTime(2016, 3, 1, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "pick-3", LottoTypeEnum.Pick3, dtStartDraw, "pick-game2", 1, DrawTypeEnum.Midday);
            CrawlCore(encores, "pick-3", LottoTypeEnum.Pick3, dtStartDraw, "pick-game4", 1);
        }

        private void CrawPick2(DbSet<Encore> encores)
        {
            var dtStartDraw = new DateTime(2016, 3, 1, 0, 0, 0, DateTimeKind.Local);
            CrawlCore(encores, "pick-2", LottoTypeEnum.Pick2, dtStartDraw, "pick-game2", 1, DrawTypeEnum.Midday);
            CrawlCore(encores, "pick-2", LottoTypeEnum.Pick2, dtStartDraw, "pick-game4", 1);
        }

        private void CrawlCore(DbSet<Encore> encores, string lottoName, LottoTypeEnum lottoType, DateTime startDraw, string tabId = "3", int days = 7, DrawTypeEnum drawType = DrawTypeEnum.Evening)
        {
            //OLG website has encore data starting from different date for different lotto type
            //Link: http://lottery.olg.ca/en-ca/winning-numbers/[LottoName]/winning-numbers?startDate=[DrawDate-1]&endDate=[DrawDate+1]
            //E.g.  http://lottery.olg.ca/en-ca/winning-numbers/lotto-649/winning-numbers?startDate=2016-03-01&endDate=2016-03-03

            var linkTemplateLotto = "http://lottery.olg.ca/en-ca/winning-numbers/[LottoName]/winning-numbers?startDate=[PrevDrawDate]&endDate=[NextDrawDate]";
            var dtStartDraw = startDraw;

            while (dtStartDraw.CompareTo(DateTime.Now.Date.ToLocalTime()) <= 0)
            {
                try
                {
                    //we add encore that doesn't exist in DB
                    var bDrawExists =
                         encores.Any(
                            c => c.DrawDate == dtStartDraw && c.LottoType == lottoType && c.DrawType == drawType);
                    if (!bDrawExists)
                    {
                        var linkCurrentLotto = linkTemplateLotto
                            .Replace("[LottoName]", lottoName)
                            .Replace("[PrevDrawDate]", string.Format("{0:yyyy-MM-dd}", dtStartDraw.AddDays(-1)))
                            .Replace("[NextDrawDate]", string.Format("{0:yyyy-MM-dd}", dtStartDraw.AddDays(1)));

                        var newEncore = new Encore
                        {
                            DrawDate = dtStartDraw,
                            LottoType = lottoType,
                            DrawType = drawType,
                            EncoreMatches = new List<EncoreMatch>()
                        };

                        var getHtmlWeb = new HtmlWeb();
                        HtmlDocument document = getHtmlWeb.LoadFromWebAsync(linkCurrentLotto).GetAwaiter().GetResult();

                        var divTags = document.DocumentNode.Descendants("div");
                        var spanTags = document.DocumentNode.Descendants("span");
                        var pTags = document.DocumentNode.Descendants("p");

                        if (pTags != null)
                        {
                            var pSelectedDate = pTags.Where(p => p.Attributes["class"] != null &&
                                                                 p.Attributes["class"].Value ==
                                                                 "large-date selected-date")
                                .Select(p => p)
                                .SingleOrDefault();

                            if (pSelectedDate == null)
                                //if there's no large date displayed on the page, we bypass
                                throw new Exception();
                            else
                            {
                                var selectedDate = DateTime.Parse(pSelectedDate.Attributes["data-selecteddate"].Value);
                                if (dtStartDraw.CompareTo(selectedDate) != 0)
                                    //if the displayed large date is not the date we are looking for, we bypass
                                    throw new Exception();
                            }
                        }

                        if (divTags != null)
                        {
                            var htmlNodes = divTags as IList<HtmlNode> ?? divTags.ToList();
                            if (!htmlNodes.Any(div => div.Attributes["class"] != null &&
                                                      div.Attributes["class"].Value == "draw-completed"))
                            {
                                //if there's no data (draw is in progress or unavailable), we bypass
                                throw new Exception();
                            }

                            //get encore winning number, NOT working for lotto type that has midday and evening draw
                            //var encoreNumTag = spanTags
                            //    .Where(span => span.Attributes["class"] != null && span.Attributes["class"].Value == "number")
                            //    .Select(span => span)
                            //    .SingleOrDefault();
                            //if (encoreNumTag != null)
                            //    newEncore.WinninNumber = encoreNumTag.InnerText.Replace("encore", "");

                            var divTag = htmlNodes
                                .Where(
                                    div =>
                                        div.Attributes["id"] != null &&
                                        (div.Attributes["id"].Value == lottoName + "-game" + tabId ||
                                         div.Attributes["id"].Value == lottoName.Replace("-", "") + "-game" + tabId ||
                                         div.Attributes["id"].Value == tabId))
                                .Select(div => div)
                                .SingleOrDefault();

                            if (divTag == null)
                            {
                                //if the encore tab doesn't exist, we bypass
                                throw new Exception();
                            }

                            //get TotalCashWon
                            var h3Tag = divTag.Descendants("h3").FirstOrDefault();
                            if (h3Tag != null)
                            {
                                if (h3Tag.InnerText.Contains("$"))
                                    newEncore.TotalCashWon =
                                        double.Parse(h3Tag.InnerText.Replace("$", "").Replace(" IN CASH WON!", ""));
                            }

                            var trTags = divTag.Descendants("tr");
                            if (trTags != null)
                            {
                                if (trTags.Count() <= 1)
                                {
                                    //if the data is not available (only one header TR is shown), we bypass
                                    throw new Exception();
                                }

                                var counter = -2;
                                foreach (var trTag in trTags)
                                {
                                    counter++;
                                    if (counter == -1)
                                        continue;

                                    //get winning number
                                    if (counter == 0)
                                    {
                                        var winningNum = trTag.Descendants("td")
                                            .Where(
                                                td =>
                                                    td.Attributes["class"] != null &&
                                                    td.Attributes["class"].Value == "chart-winners-col")
                                            .Select(td => td.InnerText).SingleOrDefault();
                                        if (winningNum != null && winningNum.Replace(" ", "").Length == 7)
                                        {
                                            newEncore.WinninNumber = winningNum.Replace(" ", "");
                                        }
                                    }

                                    //create a new encore match
                                    var newEncoreMatch = new EncoreMatch
                                    {
                                        Encore = newEncore,
                                        MatchType = (MatchTypeEnum)counter
                                    };

                                    //get tickets won for the match
                                    var ticketsWonText = trTag.Descendants("td")
                                        .Where(
                                            td =>
                                                td.Attributes["class"] != null &&
                                                td.Attributes["class"].Value == "chart-odds-col")
                                        .Select(td => td.InnerText).SingleOrDefault();
                                    if (ticketsWonText != null)
                                    {
                                        newEncoreMatch.TicketsWon = int.Parse(ticketsWonText.Replace(",", ""));
                                    }

                                    //get prize for the match
                                    var prizeText = trTag.Descendants("td")
                                        .Where(
                                            td =>
                                                td.Attributes["class"] != null &&
                                                td.Attributes["class"].Value == "chart-prize-col")
                                        .Select(td => td.InnerText.Replace("$", "")).SingleOrDefault();

                                    if (prizeText != null)
                                    {
                                        newEncoreMatch.Prize = double.Parse(prizeText.Replace(",", ""));
                                    }

                                    newEncore.EncoreMatches.Add(newEncoreMatch);
                                }
                            }

                            //add encore to context
                            encores.Add(newEncore);
                        }
                    }
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                    //throw;
                }
                finally
                {
                    dtStartDraw = dtStartDraw.AddDays(days);
                }
            }
        }
    }
}
