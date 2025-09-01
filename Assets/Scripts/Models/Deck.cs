using System.Collections.Generic;

namespace Mighty.Models
{
    public class Deck
    {
        private List<Card> cards = new List<Card>();

        public Deck()
        {
            // 52장 + Joker 2장
            foreach (Suit s in new[] { Suit.Clubs, Suit.Diamonds, Suit.Hearts, Suit.Spades })
            {
                foreach (Rank r in new[] { Rank.Two, Rank.Three, Rank.Four, Rank.Five, Rank.Six, Rank.Seven,
                                           Rank.Eight, Rank.Nine, Rank.Ten, Rank.Jack, Rank.Queen, Rank.King, Rank.Ace })
                {
                    cards.Add(new Card(s, r));
                }
            }

            cards.Add(new Card(Suit.Joker, Rank.Joker));
            cards.Add(new Card(Suit.Joker, Rank.Joker));
        }

        public List<Card> Draw(int count)
        {
            var hand = new List<Card>();
            var rand = new System.Random();
            for (int i = 0; i < count; i++)
            {
                int index = rand.Next(cards.Count);
                hand.Add(cards[index]);
                cards.RemoveAt(index);
            }
            return hand;
        }
    }
}
