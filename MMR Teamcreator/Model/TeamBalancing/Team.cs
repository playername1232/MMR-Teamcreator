using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMR_Teamcreator.Model
{
    public class Team
    {
        /// <summary>
        /// pos[0] = Top
        /// pos[1] = Jungle
        /// pos[2] = Mid
        /// pos[3] = Adc
        /// pos[4] = Support
        /// </summary>
        public Player Toplaner { get; private set; }
        public Player Jungler { get; private set; }
        public Player Midlaner { get; private set; }
        public Player ADCarry { get; private set; }
        public Player Support { get; private set; }

        public Team()
        {
            Toplaner = null;
            Jungler = null;
            Midlaner = null;
            ADCarry = null;
            Support = null;
        }

        public Team(Player toplaner, Player jungler, Player midlaner, Player aDCarry, Player support)
        {
            Toplaner = toplaner;
            Jungler = jungler;
            Midlaner = midlaner;
            ADCarry = aDCarry;
            Support = support;
        }

        public int GetMMR()
        {
            if (Toplaner == null)
                return -1;
            if (Jungler == null)
                return -1;
            if (Midlaner == null)
                return -1;
            if (ADCarry == null)
                return -1;
            if (Support == null)
                return -1;


            return (Toplaner.GetMMR() + Jungler.GetMMR() + Midlaner.GetMMR() + ADCarry.GetMMR() + Support.GetMMR()) / 5;
        }

        public static void SwapTop(ref Team _team1, ref Team _team2)
        {
            (_team1.Toplaner, _team2.Toplaner) = (_team2.Toplaner, _team1.Toplaner);
        }
        public static void SwapJungle(ref Team _team1, ref Team _team2)
        {
            (_team1.Jungler, _team2.Jungler) = (_team2.Jungler, _team1.Jungler);
        }
        public static void SwapMidlane(ref Team _team1, ref Team _team2)
        {
            (_team1.Midlaner, _team2.Midlaner) = (_team2.Midlaner, _team1.Midlaner);
        }
        public static void SwapADCarry(ref Team _team1, ref Team _team2)
        {
            (_team1.ADCarry, _team2.ADCarry) = (_team2.ADCarry, _team1.ADCarry);
        }
        public static void SwapSupport(ref Team _team1, ref Team _team2)
        {
            (_team1.Support, _team2.Support) = (_team2.Support, _team1.Support);
        }

        public override string ToString()
        {
            int count = 0;

            if (Toplaner != null)
                count += 1;
            if (Jungler != null)
                count += 1;
            if (Midlaner != null)
                count += 1;
            if (ADCarry != null)
                count += 1;
            if (Support != null)
                count += 1;

            return $"Team contains {count} players";
        }
    }
}