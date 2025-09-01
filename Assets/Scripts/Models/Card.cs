namespace Mighty.Models
{
    public enum Suit { Clubs, Diamonds, Hearts, Spades, Joker }
    public enum Rank { Two = 2, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace, Joker }

    [System.Serializable]
    public class Card
    {
        public Suit suit;
        public Rank rank;

        public Card(Suit s, Rank r)
        {
            suit = s;
            rank = r;
        }

        public override string ToString()
        {
            return suit == Suit.Joker ? "Joker" : $"{rank} of {suit}";
        }
    }
}
