using System;
using _Kimi.Scripts.Core.CardGame;

namespace _Kimi.Scripts.Gameplay.CardGame
{
    public class CardData
    {
        public Suit CardSuit { get; }
        public Rank CardRank { get; }

        public CardData(Suit suit, Rank rank)
        {
            this.CardSuit = suit;
            this.CardRank = rank;
        }

        public override bool Equals(object obj)
        {
            if (obj is CardData card)
            {
                return this.Equals(card);
            } else
            {
                return false;
            }
        }

        private bool Equals(CardData other)
        {
            return CardSuit == other.CardSuit && CardRank == other.CardRank;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine((int)CardSuit, (int)CardRank);
        }

        public override string ToString()
        {
            return $"{this.CardRank} of {this.CardSuit}";
        }
    }
}
