using System.Collections.Generic;

// TODO - implement behaviour to pick covered cards

class CpuHandController: HandController
{
    public void Awake()
    {
        this._isPlayer = false;
    }

    public CardData PlayCard(CardData topOfPile)
    {
        if (this.HandSize > 0)
        {
            return this.PickBest(this._hand, topOfPile);
        } else if (this.HandSize == 0 && this.OpenCards > 0)
        {
            return this.PickBest(this._tableOpen, topOfPile);
        } else
        {
            // modify this
            return null;
        }
    }

    private CardData PickBest(Dictionary<CardView, CardData> collection, CardData topOfPile)
    {
        foreach (var entry in collection)
        {
            if (
                topOfPile == null
                || ((int)topOfPile.CardRank == 2)
                || ((int)topOfPile.CardRank == 5 && topOfPile.CardRank >= entry.Value.CardRank)
                || (((int)topOfPile.CardRank != 5) && topOfPile.CardRank <= entry.Value.CardRank)
            )
            {
                this.ToggleSelect(entry.Value);
                return entry.Value;
            }
        }
        return null;
    }
}