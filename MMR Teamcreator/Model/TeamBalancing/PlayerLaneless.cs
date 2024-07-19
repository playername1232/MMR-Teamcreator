﻿using System;
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
        public string TwitchNick { get; private set; }
        public string IngameNick { get; private set; }
        private int MMR { get; set; }
        public Divisions Rank { get; private set; }

        public PlayerLaneless(string twitch, Divisions rank, string ingame)
        {
            TwitchNick = twitch.TrimEnd(' ');
            _rankString = $"{rank.ToString().ToUpper()[0]}{rank.ToString().Split('_')[1][0]}";
            Rank = rank;
            List<string> ranksstr = new List<string>();
            List<int> rankmmr = new List<int>();
            Dictionary<string, int> ranks = new Dictionary<string, int>();

            foreach (string item in Enum.GetNames(typeof(Divisions)))
                ranksstr.Add(item);

            foreach (int item in Enum.GetValues(typeof(Divisions)))
                rankmmr.Add(item);

            for (int i = 0; i < ranksstr.Count; i++)
                ranks.Add(ranksstr[i], rankmmr[i]);

            foreach (KeyValuePair<string, int> item in ranks)
            {
                char item_0 = item.Key.ToCharArray()[0];
                char item_1 = item.Key.Split('_')[1].ToCharArray()[0];

                if ($"{item_0}{item_1}" == $"{rank.ToString()[0]}{rank.ToString().Split('_')[1][0]}")
                    MMR = Convert.ToInt32(ranks[item.Key]);
            }

            IngameNick = ingame.TrimEnd(' ');
            TwitchNick = twitch.TrimEnd(' ');
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
            List<string> _rankListStr = new List<string>();
            List<int> _rankListInt = new List<int>();
            Dictionary<string, int> ranks = new Dictionary<string, int>();

            _rankListStr = Enum.GetNames(typeof(Divisions)).ToList();

            foreach (int _value in Enum.GetValues(typeof(Divisions)))
                _rankListInt.Add(_value);

            for (int i = 0; i < _rankListStr.Count; i++)
                ranks.Add(_rankListStr[i], _rankListInt[i]);

            foreach (KeyValuePair<string, int> item in ranks)
            {
                char item_0 = item.Key.ToCharArray()[0];
                char item_1 = item.Key.Split('_')[1].ToCharArray()[0];

                if ($"{item_0}{item_1}" == $"{rank.ToString()[0]}{rank.ToString().Split('_')[1][0]}")
                    MMR = Convert.ToInt32(ranks[item.Key]);
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

            return player1.IngameNick != player2.IngameNick || player1.TwitchNick != player2.TwitchNick;
        }

        public override string ToString() => $"TW: {TwitchNick}  Rank: {_rankString}  Ingame: {IngameNick}";

        public override int GetHashCode() => this.TwitchNick.GetHashCode();

        public override bool Equals(object obj)
        {
            return this == (obj as PlayerLaneless);
        }
    }
}
