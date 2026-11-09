public class CardData
{
    public Suit CardSuit { get; private set; }
    public Rank CardRank { get; private set; }

    public CardData(Suit suit, Rank rank)
    {
        this.CardSuit = suit;
        this.CardRank = rank;
    }

    public override bool Equals(object obj)
    {
        if (obj is CardData card)
        {
            return card.CardRank == this.CardRank && card.CardSuit == this.CardSuit;
        } else
        {
            return false;
        }
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return $"{this.CardRank} of {this.CardSuit}";
    }
}
