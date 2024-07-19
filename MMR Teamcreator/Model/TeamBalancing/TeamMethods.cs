using DirectoryChecker;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Documents;

namespace MMR_Teamcreator.Model
{
    public class TeamMethods
    {
        public static List<Team> BalanceTeams(List<Player> topList, List<Player> jungleList, List<Player> midList, List<Player> adcList, List<Player> supportList, bool createRandomPlayers = false, bool save = false)
        {
            TeamMethods xd = new TeamMethods();

            if (createRandomPlayers)
                xd.CreatePlayers(ref topList, ref jungleList, ref midList, ref adcList, ref supportList);

            return xd.InnerTeamBalance(topList, jungleList, midList, adcList, supportList, save);
        }

        public TeamMethods() { }

        private void CreatePlayers(ref List<Player> topList, ref List<Player> jungleList, ref List<Player> midList, ref List<Player> adcList, ref List<Player> suppList)
        {
            Random ran = new Random();

            var values = Enum.GetValues(typeof(Divisions));
            for (int i = 1; i <= 16; i++)
            {
                topList.Add(new Player(Roles.Top, $"Toplaner{i}", (Divisions)values.GetValue(ran.Next(values.Length)), $"ToplanerNick{i}"));
                jungleList.Add(new Player(Roles.Jungle, $"Jungler{i}", (Divisions)values.GetValue(ran.Next(values.Length)), $"JunglerNick{i}"));
                midList.Add(new Player(Roles.Mid, $"Midlaner{i}", (Divisions)values.GetValue(ran.Next(values.Length)), $"MidlanerNick{i}"));
                adcList.Add(new Player(Roles.ADC, $"ADC{i}", (Divisions)values.GetValue(ran.Next(values.Length)), $"ADCNick{i}"));
                suppList.Add(new Player(Roles.Support, $"Supp{i}", (Divisions)values.GetValue(ran.Next(values.Length)), $"SuppNick{i}"));
            }
        }

        #region TeamBalancing
        private List<Team> InnerTeamBalance(List<Player> topList, List<Player> jungleList, List<Player> midList, List<Player> adcList, List<Player> suppList, bool save = false)
        {
            List<Team> _teamList = new List<Team>();

            int topAvg = GetTeamMMR(topList),
                jgAvg = GetTeamMMR(jungleList),
                midAvg = GetTeamMMR(midList),
                adcAvg = GetTeamMMR(adcList),
                SuppAvg = GetTeamMMR(suppList);

            int avgOverall = (topAvg + jgAvg + midAvg + adcAvg + SuppAvg) / 5;

            for (int i = 0; i < 16; i++)
                _teamList.Add(new Team(topList[i], jungleList[i], midList[i], adcList[i], suppList[i]));

            _teamList = SortTeamsByMMR(_teamList);

            int process = 0;

            while (BiggestTeamMMRGap(_teamList) > 2 && process != 250)
            {
                Tuple<int, int> indexes = IndexesOfTwoMostGappedTeams(_teamList);
                Team _team1 = _teamList[indexes.Item1];
                Team _team2 = _teamList[indexes.Item2];
                SwapDiameterClosestPlayers(ref _team1, ref _team2, avgOverall);
                _teamList[indexes.Item1] = _team1;
                _teamList[indexes.Item2] = _team2;

                process++;
            }

            if (save)
                Save(_teamList);

            ExportToExcelJSON(_teamList);

            return _teamList;
        }
        #endregion

        List<Team> SortTeamsByMMR(List<Team> _teams) => _teams.OrderBy(x => x.GetMMR()).ToList();

        private static Dictionary<string, int> RanksCount(List<Team> _teams)
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
            
            _teams.ForEach(team =>
            {
                new List<Player>() { team.Toplaner, team.Jungler, team.Midlaner, team.ADCarry, team.Support }.ForEach(
                    player =>
                    {
                        foreach (string key in _pomDic.Keys)
                        {
                            if (player.Rank.ToString().Split('_')[0] == key)
                            {
                                _ranks[key] += 1;
                                break;
                            }
                        }
                    });
            });
            
            return _pomDic;
        }

        int BiggestTeamMMRGap(List<Team> _teams) => _teams.Max(x => x.GetMMR()) - _teams.Min(x => x.GetMMR());

        Tuple<int, int> IndexesOfTwoMostGappedTeams(List<Team> teams)
        {
            int first = 0, second = 0;

            Team num1 = teams.OrderBy(x => x.GetMMR()).ToList()[0];
            Team num2 = teams.OrderBy(x => x.GetMMR()).ToList()[teams.Count - 1];

            for (int i = 0; i < teams.Count; i++)
            {
                if (teams[i] == num1)
                    first = i;
                if (teams[i] == num2)
                    second = i;
            }

            return new Tuple<int, int>(first, second);
        }

        void SwapDiameterClosestPlayers(ref Team team1, ref Team _team2, int avgRankMMR)
        {
            if (_team2 == null) throw new ArgumentNullException(nameof(_team2));
            Roles swapRole = Roles.Top;
            int endDiff = int.MaxValue;

            int diff = team1.Toplaner.GetMMR() - _team2.Toplaner.GetMMR();

            //If diff > 0 = Weaker team has stronger player on lane
            if (diff < 0)
            {
                diff *= -1;
                diff = avgRankMMR - diff;
                endDiff = diff;
            }

            diff = team1.Jungler.GetMMR() - _team2.Jungler.GetMMR();
            if (diff < 0)
            {
                diff *= -1;
                diff = avgRankMMR - diff;
                if (diff < endDiff)
                {
                    endDiff = diff;
                    swapRole = Roles.Jungle;
                }
            }

            diff = team1.Midlaner.GetMMR() - _team2.Midlaner.GetMMR();
            if (diff < 0)
            {
                diff *= -1;
                diff = avgRankMMR - diff;
                if (diff < endDiff)
                {
                    endDiff = diff;
                    swapRole = Roles.Mid;
                }
            }

            diff = team1.ADCarry.GetMMR() - _team2.ADCarry.GetMMR();
            if (diff < 0)
            {
                diff *= -1;
                diff = avgRankMMR - diff;
                if (diff < endDiff)
                {
                    endDiff = diff;
                    swapRole = Roles.ADC;
                }
            }

            diff = team1.Support.GetMMR() - _team2.Support.GetMMR();
            if (diff < 0)
            {
                diff *= -1;
                diff = avgRankMMR - diff;
                if (diff < endDiff)
                {
                    endDiff = diff;
                    swapRole = Roles.Support;
                }
            }

            if (swapRole == Roles.Top)
                Team.SwapTop(ref team1, ref _team2);
            else if (swapRole == Roles.Jungle)
                Team.SwapJungle(ref team1, ref _team2);
            else if (swapRole == Roles.Mid)
                Team.SwapMidlane(ref team1, ref _team2);
            else if (swapRole == Roles.ADC)
                Team.SwapADCarry(ref team1, ref _team2);
            else
                Team.SwapSupport(ref team1, ref _team2);
        }

        string GetTeamMMRString(int mmr)
        {
            List<string> ranksstr = new List<string>();
            List<int> rankmmr = new List<int>();
            Dictionary<int, string> ranks = new Dictionary<int, string>();

            foreach (string item in Enum.GetNames(typeof(Divisions)))
                ranksstr.Add(item);

            foreach (int item in Enum.GetValues(typeof(Divisions)))
                rankmmr.Add(item);

            for (int i = 0; i < ranksstr.Count; i++)
                ranks.Add(rankmmr[i], ranksstr[i]);

            foreach (KeyValuePair<int, string> item in ranks)
            {
                //int diff = mmr - item.Key;
                if (mmr == item.Key)
                    return item.Value;
            }
            return "ERR_ERR";
        }

        int GetTeamMMR(List<Player> team)
        {
            if (team.Count() > 0)
            {
                int teamMmr = 0;

                team.ForEach(player =>
                {
                    teamMmr += player.GetMMR();
                });
                
                return Convert.ToInt32(teamMmr / team.Count());
            }
            return 0;
        }

        void Save(List<Team> _teams)
        {
            StreamWriter sw = new StreamWriter($@"{System.Environment.CurrentDirectory}\teamy.txt", false);
            try
            {
                for (int i = 0; i < _teams.Count; i++)
                {
                    List<Player> pomList = new List<Player>() { _teams[i].Toplaner, _teams[i].Jungler, _teams[i].Midlaner, _teams[i].ADCarry, _teams[i].Support };
                    string rankstr = GetTeamMMRString(GetTeamMMR(pomList)).Replace('_', ' ');
                    sw.WriteLine($"Team číslo {i + 1} ... Team MMR : {GetTeamMMR(pomList)} - Rank : {rankstr}");

                    for (int j = 0; j < pomList.Count; j++)
                    {
                        sw.WriteLine($"{pomList[j]}");
                    }
                    sw.Write("..........................................................................\n");
                }

                int avgTeamMmr = 0; 
                _teams.ForEach(x => avgTeamMmr += x.GetMMR());
                string avgTeamRank = GetTeamMMRString(Convert.ToInt32(Math.Floor((decimal)avgTeamMmr / 16)));

                sw.Write($"Počet teamů: {_teams.Count}\nPočet hráčů: {_teams.Count * 5}" +
                    $"\nPrůměrný mmr teamů: {Math.Floor((decimal)avgTeamMmr / 16)}" +
                    $"\nPrůměrný rank teamů: {avgTeamRank.Replace('_', ' ')}");

                Dictionary<string, int> _ranksCount = RanksCount(_teams);

                sw.WriteLine("\n...............................Počet ranků................................");

                foreach (KeyValuePair<string, int> item in _ranksCount)
                    sw.WriteLine($"{item.Key}: {item.Value}");

                sw.WriteLine("...........................................................................");
            }
            catch (Exception e)
            {
                MessageBox.Show($"AN ERROR OCCURED\nMessage: {e.Message}", "EXCEPTION");
            }
            finally
            {
                sw.Close();
                sw.Dispose();
                Process.Start($@"{System.Environment.CurrentDirectory}\teamy.txt");
            }
        }

        void ExportToExcelJSON(List<Team> _teams)
        {
            string exportDir = $@"{Environment.CurrentDirectory}\RFPExcelAPI";

            Clipboard.SetText(exportDir);
            if (!Directory.Exists($@"{exportDir}"))
                Directory.CreateDirectory($@"{exportDir}");

            StreamWriter sw = new StreamWriter($@"{exportDir}\RawPlayerDataExport.txt", false, Encoding.UTF8);
            try
            {
                _teams.ForEach(item => new List<Player>() { item.Toplaner, item.Jungler, item.Midlaner, item.ADCarry, item.Support }
                    .ForEach(_innerItem => sw.WriteLine($"{_innerItem.IngameNick}:{_innerItem.RankString}:{_innerItem.TwitchNick}")));
            }
            catch (Exception e)
            {
                MessageBox.Show($"{e.Message}");
            }
            finally
            {
                sw.Close();
                sw.Dispose();
                Process.Start($@"{exportDir}\RawPlayerDataExport.txt");
            }
       
        }
    }
}