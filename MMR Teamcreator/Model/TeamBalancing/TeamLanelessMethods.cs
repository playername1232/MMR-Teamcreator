using ClosedXML;
using DirectoryChecker;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Presentation;
using Microsoft.Win32;
using Renci.SshNet.Messages;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MMR_Teamcreator.Model
{
    public class TeamLanelessMethods
    {
        public static List<TeamLaneless> BalanceTeams(List<PlayerLaneless> players, bool createRandomPlayers)
        {
            TeamLanelessMethods methods = new TeamLanelessMethods();
            if (createRandomPlayers)
                methods.CreatePlayers(ref players);

            return methods.InnerTeamBalance(players);
        }

        public TeamLanelessMethods()
        {/* YEET */}

        private List<TeamLaneless> InnerTeamBalance(List<PlayerLaneless> players)
        {
            List<TeamLaneless> teams = new List<TeamLaneless>();

            int mmr = 0;
            players.ForEach(x => mmr += x.GetMMR());
            int avgMmr = mmr / players.Count;

            int playerIndex = 0;

            for (int i = 0; i < players.Count / MainWindow.PlayersPerTeam; i++)
            {
                teams.Add(new TeamLaneless());
                for (int j = 0; j < MainWindow.PlayersPerTeam; j++, playerIndex++)
                    teams[i].Players.Add(players[playerIndex]);
            }

            teams = SortTeamsByMMR(teams);

            int process = 0;

            while(BiggestTeamMMRGap(teams) > 2 && process != 250)
            {
                Tuple<int, int> indexes = IndexesOfTwoMostGappedTeams(teams);
                TeamLaneless team1 = teams[indexes.Item1];
                TeamLaneless team2 = teams[indexes.Item2];
                SwapDiameterClosestPlayers(ref team1, ref team2, avgMmr);
                teams[indexes.Item1] = team1;
                teams[indexes.Item2] = team2;
                process += 1;
            }

            Save(teams);

            return teams;
        }

        List<TeamLaneless> SortTeamsByMMR(List<TeamLaneless> teams) => teams.OrderBy(x => x.GetTeamMMR()).ToList();

        int BiggestTeamMMRGap(List<TeamLaneless> teams) => teams.Max(x => x.GetTeamMMR()) - teams.Min(x => x.GetTeamMMR());

        Tuple<int, int> IndexesOfTwoMostGappedTeams(List<TeamLaneless> teams)
        {
            int first = 0, second = 0;

            TeamLaneless num1 = teams.OrderBy(x => x.GetTeamMMR()).ToList()[0];
            TeamLaneless num2 = teams.OrderBy(x => x.GetTeamMMR()).ToList()[teams.Count - 1];

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] == num1)
                    first = i;
                if (teams[i] == num2)
                    second = i;
            }

            return new Tuple<int, int>(first, second);
        }     

        void SwapDiameterClosestPlayers(ref TeamLaneless team1, ref TeamLaneless team2, int avgRankInt)
        {
            int close1 = 0, close2 = 0;

            int currentDiff = int.MaxValue;

            for(int i = 0; i < team1.Players.Count; i++)
            {
                for(int j = 0; j < team2.Players.Count; j++)
                {
                    int minRes = team1.Players[i].GetMMR() - team2.Players[j].GetMMR();

                    if(minRes < 0)
                    {
                        minRes += -1;

                        minRes = avgRankInt - minRes;
                        
                        if(minRes < currentDiff)
                        {
                            currentDiff = minRes;
                            close1 = i;
                            close2 = j;
                        }
                    }
                }
            }
            TeamLaneless.SwapPlayers(ref team1, ref team2, close1, close2);
        }

        void Save(List<TeamLaneless> teams, string path = "")
        {
            if (path == "")
                path = $@"{Environment.CurrentDirectory}\ClassicTeams\output.txt";

            StreamWriter sw = new StreamWriter(path, false);

            try
            {
                for(int i = 0; i < teams.Count; i++)
                {
                    List<PlayerLaneless> pomList = teams[i].Players;

                    string rankstr = GetTeamMMRString(GetTeamMMRInt(teams[i])).Replace('_', ' ');
                    sw.WriteLine($"Team číslo {i + 1} ... Team MMR : {GetTeamMMRInt(teams[i])} - Rank : {rankstr} Počet hráčů : {teams[i].Players.Count}");

                    for (int j = 0; j < pomList.Count; j++) 
                    {
                        //sw.WriteLine($"{pomList[j]}");

                        sw.WriteLine($"{pomList[j].IngameNick}\t{pomList[j].RankString}\t{pomList[j].TwitchNick}");
                    }
                    sw.Write("..........................................................................\n");
                }


                int avgTeamMMR = 0;

                teams.ForEach(x => avgTeamMMR += x.GetTeamMMR());
                Debug.WriteLine($"avg/(teamCount*playerCountPerTeam) = {avgTeamMMR}/({teams.Count}*{teams[0].Players.Count}) = {avgTeamMMR / (teams.Count * teams[0].Players.Count)}");
                string avgTeamRank = GetTeamMMRString(Convert.ToInt32(Math.Floor((decimal)avgTeamMMR / (teams.Count * teams[0].Players.Count))));

                sw.Write($"Počet teamů: {teams.Count}\nPočet hráčů: {teams.Count * MainWindow.PlayersPerTeam}\nPrůměrný mmr teamů: {Math.Floor((decimal)avgTeamMMR / (teams.Count * teams[0].Players.Count))}\nPrůměrný rank teamů: {avgTeamRank.Replace('_', ' ')}");


                sw.WriteLine("\n...............................Počet ranků................................");

                Dictionary<string, int> _ranksCount = RanksCount(teams);
                foreach (KeyValuePair<string, int> item in _ranksCount)
                    sw.WriteLine($"{item.Key}: {item.Value}");

                sw.WriteLine("...........................................................................");
            } 
            catch (Exception e)
            {
                MessageBox.Show($"An error occured\nMESSAGE={e.Message}", "ERROR", MessageBoxButton.OK);
            }
            finally
            {
                sw.Close();
                sw.Dispose();
                Process.Start($@"{Environment.CurrentDirectory}\ClassicTeams\output.txt");
            }
        }

        int GetTeamMMRInt(TeamLaneless team)
        {
            if(team.Players.Count > 0)
            {
                int teamMMR = 0;
                team.Players.ForEach(x => teamMMR += x.GetMMR());

                return Convert.ToInt32(teamMMR / team.Players.Count);
            }
            return 0;
        }

        string GetTeamMMRString(int mmr)
        {
            Dictionary<int, string> ranks = new Dictionary<int, string>();

            List<string> ranksstr = Enum.GetNames(typeof(Divisions)).ToList();

            List<int> rankmmr = Enum.GetValues(typeof(Divisions)).Cast<int>().ToList();

            for (int i = 0; i < ranksstr.Count; i++)
                ranks.Add(rankmmr[i], ranksstr[i]);

            foreach (var item in ranks.Where(item => mmr == item.Key))
            {
                return item.Value;
            }
            return "ERR_ERR";
        }

        private static Dictionary<string, int> RanksCount(List<TeamLaneless> teams)
        {
            Dictionary<string, int> _ranks = new Dictionary<string, int>
            {
                { "Challenger", 0 },
                { "Grandmaster", 0 },
                { "Master", 0 },
                { "Diamond", 0 },
                { "Emerald", 0 },
                { "Platinum", 0 },
                { "Gold", 0 },
                { "Silver", 0 },
                { "Bronze", 0 },
                { "Iron", 0 }
            };

            Dictionary<string, int> _pomDic = _ranks;

            foreach (var _player in teams.SelectMany(item => item.Players))
            {
                foreach (string key in _pomDic.Keys)
                {
                    if (_player.Rank.ToString().Split('_')[0] == key)
                    {
                        _ranks[key] += 1;
                        break;
                    }
                }
            }
            return _pomDic;
        }

        private void CreatePlayers(ref List<PlayerLaneless> players)
        {
            Random ran = new Random();

            var values = Enum.GetValues(typeof(Divisions));
            for (int i = 1; i <= 40; i++)
                players.Add(new PlayerLaneless($"TwitchHráče{i}", (Divisions)values.GetValue(ran.Next(values.Length)), $"IngameHráče{i}"));
        }
    }
}
