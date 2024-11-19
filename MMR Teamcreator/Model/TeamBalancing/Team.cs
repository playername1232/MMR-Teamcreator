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
        public Player Toplaner { get; set; }
        public Player Jungler { get; set; }
        public Player Midlaner { get; set; }
        public Player ADCarry { get; set; }
        public Player Support { get; set; }

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

        public static void SwapTop(ref Team team1, ref Team team2) => (team1.Toplaner, team2.Toplaner) = (team2.Toplaner, team1.Toplaner);
        public static void SwapJungle(ref Team team1, ref Team team2) => (team1.Jungler, team2.Jungler) = (team2.Jungler, team1.Jungler);
        public static void SwapMidlane(ref Team team1, ref Team team2) => (team1.Midlaner, team2.Midlaner) = (team2.Midlaner, team1.Midlaner);
        public static void SwapADCarry(ref Team team1, ref Team team2) => (team1.ADCarry, team2.ADCarry) = (team2.ADCarry, team1.ADCarry);
        public static void SwapSupport(ref Team team1, ref Team team2) => (team1.Support, team2.Support) = (team2.Support, team1.Support);

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