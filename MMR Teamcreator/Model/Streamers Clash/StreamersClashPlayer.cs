using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMR_Teamcreator.Model.Streamers_Clash
{
    class StreamersClashPlayer : Player
    {
        public bool IsCaptain { get; private set; }
        public int CaptainBudget { get; private set; }
        public int PlayerCost { get; set; }
        public Roles SecondaryRole { get; private set; }
        public int SecondaryPlayerCost { get; set; }

        public const int CaptainBaseBudget = 1000;
        public const int BasePlayerCost = 190;
        public const int PlayerRankStep = 10;

        public StreamersClashPlayer(string role, string secondaryRole, string twitch, string rank, string ingame, bool isCaptain = false) : base(role, twitch, rank, ingame)
        {
            IsCaptain = isCaptain;
            CaptainBudget = -1;

            List<Roles> _roles = new List<Roles>()
            {
                Roles.Top, Roles.Jungle, Roles.Mid, Roles.ADC, Roles.Support
            };

            for (int i = 0; i < _roles.Count; i++)
            {
                if (secondaryRole.ToUpper() == _roles[i].ToString().ToUpper())
                    SecondaryRole = _roles[i];
            }
        }

        public StreamersClashPlayer(string role, string twitch, string rank, string ingame, bool isCaptain = true) : base(role, twitch, rank, ingame)
        {
            IsCaptain = isCaptain;
            CaptainBudget = -1;
            this.SecondaryRole = Roles.None;
        }


        public bool ChangeCaptainBudget(int budget)
        {
            if (!IsCaptain)
                return false;
            if (budget < 0)
                return false;

            this.CaptainBudget = budget;
            return true;
        }

        public int GetSecondaryMMR()
        {
            List<string> ranksstr = Enum.GetNames(typeof(Divisions)).ToList();
            List<int> rankmmr = ((ICollection<int>)Enum.GetValues(typeof(Divisions))).ToList();
            Dictionary<string, int> ranks = new Dictionary<string, int>();      


            for (int i = 0; i < ranksstr.Count; i++)
                ranks.Add(ranksstr[i], rankmmr[i]);

            foreach (KeyValuePair<string, int> item in ranks)
            {
                char item_0 = item.Key.ToCharArray()[0];
                char item_1 = item.Key.Split('_')[1].ToCharArray()[0];

                if ($"{item_0}{item_1}" == $"{Rank.ToString()[0]}{Rank.ToString().Split('_')[1][0]}")
                    return Convert.ToInt32(ranks[item.Key]);
            }
            return 0;
        }

        public override string ToString() => ToString("D");

        public string ToString(string format)
        {
            if (IsCaptain)
                return $"{this.TwitchNick}\t{this.RankString}\t{this.IngameNick}\t{this.Role}\t{this.SecondaryRole}\t{this.CaptainBudget}";

            switch (format)
            {
                // Default
                case "D":
                    {
                        return $"{this.TwitchNick}\t{this.RankString}\t{this.IngameNick}\t{this.Role}\t{this.PlayerCost}";
                    }
                // Secondary
                case "S":
                    {
                        return $"{this.TwitchNick}\t{this.RankString}\t{this.IngameNick}\t{this.SecondaryRole}\t{this.PlayerCost}";
                    }
                default:
                    throw new FormatException($"The '{format}' string format is not supported!");
            }
        }
    }
}
