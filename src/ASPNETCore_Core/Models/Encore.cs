using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_Core.Models
{
    public class Encore
    {
        public int EncoreID { get; set; }
        public LottoTypeEnum LottoType { get; set; }
        public DrawTypeEnum DrawType { get; set; }
        public string WinninNumber { get; set; }
        public DateTime DrawDate { get; set; }
        public double TotalCashWon { get; set; }

        public ICollection<EncoreMatch> EncoreMatches { get; set; }
    }

    public enum LottoTypeEnum
    {
        LottoMAX,
        Lotto649,
        DailyGrand,
        Ontario49,
        Lottario,
        DailyKeno,
        Pick2,
        Pick3,
        Pick4
    }

    public enum DrawTypeEnum
    {
        Midday,
        Evening
    }
}
