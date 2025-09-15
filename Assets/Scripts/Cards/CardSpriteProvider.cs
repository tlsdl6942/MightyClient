using System.Collections.Generic;
using UnityEngine;

namespace Mighty.Cards
{
    public static class CardSpriteProvider
    {
        private static Dictionary<string, Sprite> _byName;

        public static void Warmup()
        {
            if (_byName != null) return;
            _byName = new Dictionary<string, Sprite>();
            var all = Resources.LoadAll<Sprite>("SmallCards");
            foreach (var sp in all)
                _byName[sp.name] = sp;
        }

        public static Sprite Get(Card c)
        {
            Warmup();
            string key = c.IsJoker ? "Joker" : $"{ShortRank(c.Rank)}{ShortSuit(c.Suit)}";

            if (_byName.TryGetValue(key, out var sp)) return sp;
            // 이름 매칭이 다르면 ToString() 기준으로도 시도
            if (_byName.TryGetValue(c.ToString(), out sp)) return sp;

            Debug.LogWarning($"[CardSpriteProvider] Sprite not found for key: {key}");
            return null;
        }

        private static string ShortSuit(Suit s) => s switch
        {
            Suit.Spade => "S",
            Suit.Heart => "H",
            Suit.Diamond => "D",
            Suit.Club => "C",
            _ => "J"
        };

        private static string ShortRank(Rank r) => r switch
        {
            Rank.A => "A",
            Rank.R10 => "10",
            Rank.J => "J",
            Rank.Q => "Q",
            Rank.K => "K",
            _ => ((int)r).ToString().Replace("2", "2").Replace("3", "3").Replace("4", "4").Replace("5", "5")
        };
    }
}
