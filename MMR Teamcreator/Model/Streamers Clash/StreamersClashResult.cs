using DocumentFormat.OpenXml.Drawing.Diagrams;
using Org.BouncyCastle.Bcpg.OpenPgp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

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
                                    List<StreamersClashPlayer> adCarries, 
                                    List<StreamersClashPlayer> supports)
        {
            TopLaners = topLaners;
            Junglers = junglers;
            Midlaners = midlaners;
            ADCarries = adCarries;
            Supports = supports;
        }

        public int GetAverageRank()
        {
            int avg = 0;
            int count = TopLaners.Count 
                + Junglers.Count 
                + Midlaners.Count 
                + ADCarries.Count 
                + Supports.Count;


            avg += TopLaners.Sum(x => x.GetMMR());
            avg += Junglers.Sum(x => x.GetMMR());
            avg += Midlaners.Sum(x => x.GetMMR());
            avg += ADCarries.Sum(x => x.GetMMR());
            avg += Supports.Sum(x => x.GetMMR());

            return avg / count;
        }

        public int GetAverageRankOfRole(Roles role)
        {
            int avg = 0;

            // TODO: Shorten the code
            switch (role)
            {
                case Roles.Top:
                    {
                        List<StreamersClashPlayer> pom = GetPlayersExceptCaptains(Roles.Top);
                        pom.ForEach(x =>
                        {
                            if (x.Role == role && !x.IsCaptain)
                                avg += x.GetMMR();
                            else if (x.SecondaryRole == role && !x.IsCaptain)
                                avg += x.GetSecondaryMMR();
                        });
                        return (int)Math.Round((double)avg / pom.Count);
                    }
                case Roles.Jungle:
                    {
                        List<StreamersClashPlayer> pom = GetPlayersExceptCaptains(Roles.Jungle);
                        pom.ForEach(x =>
                        {
                            if (x.Role == role && !x.IsCaptain)
                                avg += x.GetMMR();
                            else if (x.SecondaryRole == role && !x.IsCaptain)
                                avg += x.GetSecondaryMMR();
                        });
                        return (int)Math.Round((double)avg / pom.Count);
                    }
                case Roles.Mid:
                    {
                        List<StreamersClashPlayer> pom = GetPlayersExceptCaptains(Roles.Mid);
                        pom.ForEach(x =>
                        {
                            if (x.Role == role && !x.IsCaptain)
                                avg += x.GetMMR();
                            else if (x.SecondaryRole == role && !x.IsCaptain)
                                avg += x.GetSecondaryMMR();
                        });
                        return (int)Math.Round((double)avg / pom.Count);
                    }
                case Roles.ADC:
                    {
                        List<StreamersClashPlayer> pom = GetPlayersExceptCaptains(Roles.ADC);
                        pom.ForEach(x =>
                        {
                            if (x.Role == role && !x.IsCaptain)
                                avg += x.GetMMR();
                            else if (x.SecondaryRole == role && !x.IsCaptain)
                                avg += x.GetSecondaryMMR();
                        });
                        return (int)Math.Round((double)avg / pom.Count);
                    }
                case Roles.Support:
                    {
                        List<StreamersClashPlayer> pom = GetPlayersExceptCaptains(Roles.Support);
                        pom.ForEach(x =>
                        {
                            if (x.Role == role && !x.IsCaptain)
                                avg += x.GetMMR();
                            else if (x.SecondaryRole == role && !x.IsCaptain)
                                avg += x.GetSecondaryMMR();
                        });
                        return (int)Math.Round((double)avg / pom.Count);
                    }
            }

            return -1;
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

            List<string> res = new List<string>();

            Action<List<StreamersClashPlayer>, Roles> WritePlayersIntoOutput = (lane, role) =>
            {
                lane.ForEach(x =>
                {
                    if (!x.IsCaptain)
                    {
                        if (x.Role == role)
                            res.Add(x.ToString("D"));
                        else if (x.SecondaryRole == role)
                            res.Add(x.ToString("S"));
                    }
                });
            };


            res.Add($"Average points overall = {GetAverageRank()}");
            res.Add($"{Roles.Top} avg points = {GetAverageRankOfRole(Roles.Top)}");
            WritePlayersIntoOutput(TopLaners, Roles.Top);

            res.Add($"{Roles.Jungle} avg points = {GetAverageRankOfRole(Roles.Jungle)}");
            WritePlayersIntoOutput(Junglers, Roles.Jungle);

            res.Add($"{Roles.Mid} avg points = {GetAverageRankOfRole(Roles.Mid)}");
            WritePlayersIntoOutput(Midlaners, Roles.Mid);

            res.Add($"{Roles.ADC} avg points = {GetAverageRankOfRole(Roles.ADC)}");
            WritePlayersIntoOutput(ADCarries, Roles.ADC);

            res.Add($"{Roles.Support} avg points = {GetAverageRankOfRole(Roles.Support)}");
            WritePlayersIntoOutput(Supports, Roles.Support);


            return res.ToArray();
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

        public List<StreamersClashPlayer> GetPlayersExceptCaptains(Roles role)
        {
            List<StreamersClashPlayer> res = new List<StreamersClashPlayer>();

            if (role == Roles.Top)
                return TopLaners.Where(x => !x.IsCaptain).ToList();
            if (role == Roles.Jungle)
                return Junglers.Where(x => !x.IsCaptain).ToList();
            if (role == Roles.Mid)
                return Midlaners.Where(x => !x.IsCaptain).ToList();
            if (role == Roles.ADC)
                return ADCarries.Where(x => !x.IsCaptain).ToList();
            if (role == Roles.Support)
                return Supports.Where(x => !x.IsCaptain).ToList();

            return res;
        }

        public int CountCaptains()
        {
            int res = 0;

            res += TopLaners.Count(x => x.IsCaptain);
            res += Junglers.Count(x => x.IsCaptain);
            res += Midlaners.Count(x => x.IsCaptain);
            res += ADCarries.Count(x => x.IsCaptain);
            res += Supports.Count(x => x.IsCaptain);

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