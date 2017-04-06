using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPNETCore_Core.Models
{
    public class EncoreMatch
    {
        public int EncoreMatchID { get; set; }

        public int EncoreID { get; set; }

        public MatchTypeEnum MatchType { get; set; }

        public int TicketsWon { get; set; }

        public double Prize { get; set; }

        public Encore Encore { get; set; }
    }

    public enum MatchTypeEnum
    {
        IIIIIII, //All 7 Digits
        OIIIIII, //Last 6 Digits
        OOIIIII, //Last 5 Digits
        OOOIIII, //Last 4 Digits
        OOOOIII, //Last 3 Digits
        OOOOOII, //Last 2 Digits
        OOOOOOI, //Last Digit
        IIIIIIO, //First 6 Digits
        IIIIIOO, //First 5 Digits 
        IIIIOOO, //First 4 Digits 
        IIIOOOO, //First 3 Digits 
        IIOOOOO, //First 2 Digits 
        IIIIIOI, //First 5 Digits + Last Digit 
        IIIIOII, //First 4 Digits + Last 2 Digits 
        IIIIOOI, //First 4 Digits + Last Digit 
        IIIOIII, //First 3 Digits + Last 3 Digits 
        IIIOOII, //First 3 Digits + Last 2 Digits 
        IIIOOOI, //First 3 Digits + Last Digit 
        IIOIIII, //First 2 Digits + Last 4 Digits 
        IIOOIII, //First 2 Digits + Last 3 Digits 
        IIOOOII, //First 2 Digits + Last 2 Digits 
        IIOOOOI, //First 2 Digits + Last Digit 
        OOOOOOO  //No match
    }
}
