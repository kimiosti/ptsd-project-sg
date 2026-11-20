using _Kimi.Scripts.Gameplay.CardGame;
using UnityEngine;

namespace _Kimi.Scripts.Core.CardGame
{
    public enum FieldType
    {
        TableCovered,
        TableOpen,
        Hand
    }

    public enum PlayerType
    {
        Player,
        CPU
    }

    public enum GameState
    {
        Exchange,
        Play,
    }

    public enum Suit
    {
        Hearts,
        Clubs,
        Diamonds,
        Spades
    }

    public enum Rank
    {
        Two = 2,
        Three,
        Four,
        Five,
        Six,
        Seven,
        Eight,
        Nine,
        Ten,
        Jack,
        Queen,
        King,
        Ace
    }

    public static class SpriteRetriever
    {
        public static Sprite CardSprite(Suit suit, Rank rank)
        {
            return Retrieve($"Sprites/Cards/{rank}{suit}");
        }

        public static Sprite CardBackSprite()
        {
            return Retrieve("Sprites/Cards/Back");
        }

        public static Sprite EmptyCardSprite()
        {
            return Retrieve("Sprites/Cards/Empty");
        }

        private static Sprite Retrieve(string path)
        {
            return Resources.Load<Sprite>(path);
        }
    }

    public static class MoveChecker
    {
        public static bool IsMoveAdmissible(CardData topOfPile, CardData card)
        {
            return (
                       topOfPile == null
                   )
                   || (
                       ((int)card.CardRank) == 10 || ((int)card.CardRank) == 5 || ((int)card.CardRank) == 2
                   )
                   || (
                       ((int)topOfPile.CardRank) == 5
                       && card.CardRank <= topOfPile.CardRank
                   )
                   || (
                       ((int)topOfPile.CardRank) != 5
                       && card.CardRank >= topOfPile.CardRank
                   );
        }
    }
}