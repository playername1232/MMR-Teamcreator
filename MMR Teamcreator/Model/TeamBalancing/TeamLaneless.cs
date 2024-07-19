using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MMR_Teamcreator.Model
{
    public class TeamLaneless
    {
        public List<PlayerLaneless> Players { get; }

        public TeamLaneless()
        {
            Players = new List<PlayerLaneless>();
        }

        public TeamLaneless(List<PlayerLaneless> players) => Players = players;

        public int GetTeamMMR()
        {
            int mmr = 0;
            Players.ForEach(x => mmr += x.GetMMR());
            return mmr;
        }

        public static void SwapPlayers(ref TeamLaneless team1, ref TeamLaneless team2, int index1, int index2)
        {
            (team1.Players[index1], team2.Players[index2]) = (team2.Players[index2], team1.Players[index1]);
            
            Debug.WriteLine($"Swapping players: TEAM 1 player = Ingame {team1.Players[index1].IngameNick} RANK: {team1.Players[index1].Rank} Twitch: {team1.Players[index1].TwitchNick} with " +
                            $"TEAM 2 player = Ingame: {team2.Players[index2].IngameNick} RANK:{team2.Players[index2].Rank} Twitch: {team2.Players[index2].TwitchNick}");
        }
    }
}