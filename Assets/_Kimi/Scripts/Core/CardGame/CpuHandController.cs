using System.Collections.Generic;
using _Kimi.Scripts.Gameplay.CardGame;
using _Kimi.Scripts.UI.CardGame;

// TODO - implement behaviour to pick covered cards

namespace _Kimi.Scripts.Core.CardGame
{
    public class CpuHandController: HandController
    {
        public void Awake()
        {
            this.IsPlayer = false;
        }

        public CardData PlayCard(CardData topOfPile)
        {
            if (this.handSize > 0)
            {
                return this.PickBest(this.Hand, topOfPile);
            } else if (this.handSize == 0 && this.openCards > 0)
            {
                return this.PickBest(this.TableOpen, topOfPile);
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
}