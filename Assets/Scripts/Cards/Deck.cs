using System;
using System.Collections.Generic;

namespace Mighty.Cards
{
    public class Deck
    {
        private readonly List<Card> _cards = new List<Card>();
        private readonly Random _rng = new Random();

        public Deck()
        {
            // 52Àå (4¹«´Ì ¡¿ 13·©Å©)
            Array suits = Enum.GetValues(typeof(Suit));
            Array ranks = Enum.GetValues(typeof(Rank));
            foreach (Suit s in suits)
            {
                if (s == Suit.Joker) continue;
                foreach (Rank r in ranks)
                {
                    _cards.Add(new Card { Suit = s, Rank = r });
                }
            }
            // Á¶Ä¿ 1Àå
            _cards.Add(new Card { Suit = Suit.Joker, Rank = Rank.A });
        }

        public void Shuffle()
        {
            for (int i = _cards.Count - 1; i > 0; i--)
            {
                int j = _rng.Next(i + 1);
                (_cards[i], _cards[j]) = (_cards[j], _cards[i]);
            }
        }

        public Card Draw()
        {
            Card top = _cards[^1];
            _cards.RemoveAt(_cards.Count - 1);
            return top;
        }

        public int Count => _cards.Count;
    }
}
