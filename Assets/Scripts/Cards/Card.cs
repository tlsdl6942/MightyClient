using System;
using UnityEngine;

namespace Mighty.Cards
{
    public enum Suit { Spade, Heart, Diamond, Club, Joker }
    public enum Rank { A = 1, R2, R3, R4, R5, R6, R7, R8, R9, R10, J, Q, K }

    [Serializable]
    public struct Card
    {
        public Suit Suit;
        public Rank Rank;

        public bool IsJoker => Suit == Suit.Joker;
        public override string ToString()
        {
            return IsJoker ? "Joker" : $"{Rank}{Suit.ToString()[0]}";
        }
    }
}
