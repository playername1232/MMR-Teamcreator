using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMR_Teamcreator.Model.Streamers_Clash
{
    class StreamersClashResult
    {

        public List<StreamersClashPlayer> TopLaners { get; private set; }
        public List<StreamersClashPlayer> Junglers { get; private set; }
        public List<StreamersClashPlayer> Midlaners { get; private set; }
        public List<StreamersClashPlayer> ADCarries { get; private set; }
        public List<StreamersClashPlayer> Supports { get; private set; }

        public StreamersClashResult()
        {
            TopLaners = new List<StreamersClashPlayer>();
            Junglers = new List<StreamersClashPlayer>();
            Midlaners = new List<StreamersClashPlayer>();
            ADCarries = new List<StreamersClashPlayer>();
            Supports = new List<StreamersClashPlayer>();
        }

        public StreamersClashResult(List<StreamersClashPlayer> topLaners, 
                                    List<StreamersClashPlayer> junglers, 
                                    List<StreamersClashPlayer> midlaners, 
                                    List<StreamersClashPlayer> aDCarries, 
                                    List<StreamersClashPlayer> supports,
                                    bool sortPlayers = true)
        {
            TopLaners = topLaners;
            Junglers = junglers;
            Midlaners = midlaners;
            ADCarries = aDCarries;
            Supports = supports;

            if (sortPlayers)
                SortPlayers();

            AssignBudgetToCaptains();
        }

        void SortPlayers()
        {
            TopLaners = TopLaners.OrderByDescending(x => x.GetMMR()).ToList();
            Junglers = Junglers.OrderByDescending(x => x.GetMMR()).ToList();
            Midlaners = Midlaners.OrderByDescending(x => x.GetMMR()).ToList();
            ADCarries = ADCarries.OrderByDescending(x => x.GetMMR()).ToList();
            Supports = Supports.OrderByDescending(x => x.GetMMR()).ToList();
        }

        void AssignBudgetToCaptains()
        {
            List<List<StreamersClashPlayer>> playerLists = new List<List<StreamersClashPlayer>>()
            {
                TopLaners,
                Junglers,
                Midlaners,
                ADCarries,
                Supports
            };

            playerLists.ForEach(x => x.ForEach(y =>
            {
                if(y.IsCaptain)
                {
                    int budget = GetAverageRankOfRole(Roles.Top) 
                    + GetAverageRankOfRole(Roles.Jungle) 
                    + GetAverageRankOfRole(Roles.Mid) 
                    + GetAverageRankOfRole(Roles.ADC) 
                    + GetAverageRankOfRole(Roles.Support);

                    y.ChangeCaptainBudget(budget - y.GetMMR());
                }
            }));
        }

        public int GetAverageRank()
        {
            int avg = 0;
            int count = TopLaners.Count 
                + Junglers.Count 
                + Midlaners.Count 
                + ADCarries.Count 
                + Supports.Count;


            TopLaners.ForEach(x => avg += x.GetMMR());
            Junglers.ForEach(x => avg += x.GetMMR());
            Midlaners.ForEach(x => avg += x.GetMMR());
            ADCarries.ForEach(x => avg += x.GetMMR());
            Supports.ForEach(x => avg += x.GetMMR());

            return avg / count;
        }

        public int GetAverageRankOfRole(Roles role)
        {
            int avg = 0;

            switch(role)
            {
                case Roles.Top:
                    {
                        TopLaners.ForEach(x => avg += x.GetMMR());
                        avg /= TopLaners.Count;
                        break;
                    }
                case Roles.Jungle:
                    {
                        Junglers.ForEach(x => avg += x.GetMMR());
                        avg /= Junglers.Count;
                        break;
                    }
                case Roles.Mid:
                    {
                        Midlaners.ForEach(x => avg += x.GetMMR());
                        avg /= Midlaners.Count;
                        break;
                    }
                case Roles.ADC:
                    {
                        ADCarries.ForEach(x => avg += x.GetMMR());
                        avg /= ADCarries.Count;
                        break;
                    }
                case Roles.Support:
                    {
                        Supports.ForEach(x => avg += x.GetMMR());
                        avg /= Supports.Count;
                        break;
                    }
            }

            return avg;
        }

        public string[] GetCaptainOutput()
        {
            List<StreamersClashPlayer> captains = GetCaptains();
            string[] res = new string[captains.Count];

            for (int i = 0; i < captains.Count; i++)
            {
                res[i] = captains[i].ToString();
            }

            return res;
        }

        public string[] GetOutput()
        {
            List<StreamersClashPlayer> players = GetPlayersExceptCaptains();

            // Count of players + 5 outputs for lines
            int arrLen = players.Count + 5,
                outIdx = 0;

            string[] res = new string[arrLen];

            // Set Last role to Mid as it will trigger -ne on player role and lastRole for Toplaners 
            Roles lastRole = Roles.Mid;

            for(int i = 0; i < players.Count; i++, outIdx++)
            {
                StreamersClashPlayer player = players[i];

                if (player.Role != lastRole)
                {
                    res[outIdx] = $"{player.Role} avg points = {GetAverageRankOfRole(player.Role)}";
                    lastRole = player.Role;
                    outIdx += 1;
                }
                res[outIdx] = player.ToString();
            }

            return res;
        }

        public List<StreamersClashPlayer> GetPlayersExceptCaptains()
        {
            List<StreamersClashPlayer> res = new List<StreamersClashPlayer>();

            res.AddRange(TopLaners.Where(x => !x.IsCaptain));
            res.AddRange(Junglers.Where(x => !x.IsCaptain));
            res.AddRange(Midlaners.Where(x => !x.IsCaptain));
            res.AddRange(ADCarries.Where(x => !x.IsCaptain));
            res.AddRange(Supports.Where(x => !x.IsCaptain));

            return res;
        }

        public int CountCaptains()
        {
            int res = 0;

            TopLaners.ForEach(x => res += x.IsCaptain ? 1 : 0);
            Junglers.ForEach(x => res += x.IsCaptain ? 1 : 0);
            Midlaners.ForEach(x => res += x.IsCaptain ? 1 : 0);
            ADCarries.ForEach(x => res += x.IsCaptain ? 1 : 0);
            Supports.ForEach(x => res += x.IsCaptain ? 1 : 0);

            return res;
        }

        public List<StreamersClashPlayer> GetCaptains()
        {
            List<StreamersClashPlayer> res = new List<StreamersClashPlayer>();

            res.AddRange(TopLaners.Where(x => x.IsCaptain));
            res.AddRange(Junglers.Where(x => x.IsCaptain));
            res.AddRange(Midlaners.Where(x => x.IsCaptain));
            res.AddRange(ADCarries.Where(x => x.IsCaptain));
            res.AddRange(Supports.Where(x => x.IsCaptain));

            return res;
        }
    }
}