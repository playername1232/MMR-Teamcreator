using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MMR_Teamcreator.Model
{
    public class PlayerLaneless
    {
        private string _rankString;
        public string RankString => _rankString;
        public string TwitchNick { get; protected set; }
        public string IngameNick { get; protected set; }
        private int MMR { get; set; }
        public Divisions Rank { get; protected set; }

        public PlayerLaneless(string twitch, Divisions rank, string ingame)
        {
            Rank = rank;
            
            IngameNick = ingame.TrimEnd(' ');
            TwitchNick = twitch.TrimEnd(' ');
            
            _rankString = $"{rank.ToString().ToUpper()[0]}{rank.ToString().Split('_')[1][0]}";
            
            List<string> _rankListStr = new List<string>();
            List<int> _rankListInt = new List<int>();

            _rankListStr = Enum.GetNames(typeof(Divisions)).ToList();

            foreach (int _value in Enum.GetValues(typeof(Divisions)))
                _rankListInt.Add(_value);

            for (int i = 0; i < _rankListStr.Count; i++)
            {
                string tempListRank = $"{_rankListStr[i][0]}{_rankListStr[i].Split('_')[1]}";
                string tempCurrentRank = rank.ToString();

                if (tempListRank == $"{tempCurrentRank[0]}{tempCurrentRank.Split('_')[1]}")
                    MMR = _rankListInt[i];
            } 

            _rankString = $"{rank.ToString().ToUpper()[0]}{rank.ToString().Split('_')[1][0]}";
        }
        public PlayerLaneless(string twitch, string rank, string ingame)
        {
            _rankString = rank;
            List<Divisions> _div = new List<Divisions>()
            {
                Divisions.Iron_4, Divisions.Iron_3, Divisions.Iron_2, Divisions.Iron_1,
                Divisions.Bronze_4, Divisions.Bronze_3, Divisions.Bronze_2, Divisions.Bronze_1,
                Divisions.Silver_4, Divisions.Silver_3, Divisions.Silver_2, Divisions.Silver_1,
                Divisions.Gold_4, Divisions.Gold_3, Divisions.Gold_2, Divisions.Gold_1,
                Divisions.Platinum_4, Divisions.Platinum_3, Divisions.Platinum_2, Divisions.Platinum_1,
                Divisions.Emerald_4, Divisions.Emerald_3, Divisions.Emerald_2, Divisions.Emerald_1,
                Divisions.Diamond_4, Divisions.Diamond_3, Divisions.Diamond_2, Divisions.Diamond_1,
                Divisions.Master_A, Divisions.Grandmaster_M, Divisions.Challenger_H
            };
            for (int i = 0; i < _div.Count; i++)
            {
                rank = rank.ToUpper();
                if (rank[0] == _div[i].ToString().ToUpper().Split('_')[0][0] && rank[1] == _div[i].ToString().ToUpper().Split('_')[1][0])
                {
                    Rank = _div[i];
                    MMR = (int)_div[i];
                }

            }
            IngameNick = ingame.TrimEnd(' ');
            TwitchNick = twitch.TrimEnd(' ');
        }


        /// <summary>
        /// Updates player's Twitch acoount name
        /// </summary>
        /// <param name="nick">Player's new Twitch account name</param>
        public void SetTwitchNick(string nick) => TwitchNick = nick;

        /// <summary>
        /// Updates player's ingame account name
        /// </summary>
        /// <param name="nick">Player's new ingame account name</param>
        public void SetIngameNick(string nick) => IngameNick = nick;

        /// <summary>
        /// Updates player's rank
        /// </summary>
        /// <param name="rank">Player's new rank</param>
        public void SetRank(Divisions rank)
        {
            Rank = rank;
            
            _rankString = $"{rank.ToString().ToUpper()[0]}{rank.ToString().Split('_')[1][0]}";
            
            List<string> _rankListStr = new List<string>();
            List<int> _rankListInt = new List<int>();

            _rankListStr = Enum.GetNames(typeof(Divisions)).ToList();

            foreach (int _value in Enum.GetValues(typeof(Divisions)))
                _rankListInt.Add(_value);

            for (int i = 0; i < _rankListStr.Count; i++)
            {
                string tempListRank = $"{_rankListStr[i][0]}{_rankListStr[i].Split('_')[1]}";
                string tempCurrentRank = rank.ToString();

                if (tempListRank == $"{tempCurrentRank[0]}{tempCurrentRank.Split('_')[1]}")
                    MMR = _rankListInt[i];
            } 

            _rankString = $"{rank.ToString().ToUpper()[0]}{rank.ToString().Split('_')[1][0]}";
        }

        /// <returns>Returns player's MMR</returns>
        public int GetMMR() => MMR;

        public static bool operator ==(PlayerLaneless player1, PlayerLaneless player2)
        {
            if (player1 is null || player2 is null)
                return false;

            if (player1.IngameNick == player2.IngameNick && player1.TwitchNick == player2.TwitchNick)
                return true;
            return false;
        }

        public static bool operator !=(PlayerLaneless player1, PlayerLaneless player2)
        {
            if (player1 is null || player2 is null)
                return false;

            if (player1.IngameNick != player2.IngameNick || player1.TwitchNick != player2.TwitchNick)
                return true;
            return false;
        }

        public override string ToString() => $"TW: {TwitchNick}  Rank: {_rankString}  Ingame: {IngameNick}";

        public override int GetHashCode() => this.TwitchNick.GetHashCode();

        public override bool Equals(object obj) => this == (obj as PlayerLaneless);
    }
}
