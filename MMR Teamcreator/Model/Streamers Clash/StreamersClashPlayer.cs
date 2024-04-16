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

        public StreamersClashPlayer(string role, string twitch, string rank, string ingame, bool isCaptain) : base(role, twitch, rank, ingame)
        {
            IsCaptain = isCaptain;
            CaptainBudget = -1;
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

        public override string ToString()
        {
            if(IsCaptain)
                return $"{this.TwitchNick}\t{this.RankString}\t{this.IngameNick}\t{this.Role}\t{this.CaptainBudget}";
            return $"{this.TwitchNick}\t{this.RankString}\t{this.IngameNick}\t{this.Role}\t{this.GetMMR()}";
        }
    }
}
