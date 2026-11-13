using System.Collections.Generic;
using _Kimi.Scripts.Gameplay.CardGame;
using _Kimi.Scripts.UI.CardGame;

namespace _Kimi.Scripts.Core.CardGame
{
    public class CpuHandController: HandController
    {
        public void Awake()
        {
            this.IsPlayer = false;
        }

        public void SelectCards(CardData topOfPile)
        {
            if (this.handSize > 0)
            {
                this.PickBest(this.Hand, topOfPile);
            } else if (this.handSize == 0 && this.openCards > 0)
            {
                this.PickBest(this.TableOpen, topOfPile);
            } else
            {
                this.PickFirst(this.TableCovered);
            }
        }

        private void PickFirst(Dictionary<CardView, CardData> collection)
        {
            foreach (var entry in collection)
            {
                this.ToggleSelect(entry.Value);
            }
        }

        private void PickBest(Dictionary<CardView, CardData> collection, CardData topOfPile)
        {
            var twoCount = 0;
            var fiveCount = 0;
            var tenCount = 0;
            var lowestAdmissible = -1;

            foreach (var entry in collection)
            {
                switch ((int)entry.Value.CardRank)
                {
                    case 2:
                        twoCount++;
                        break;
                    case 5:
                        fiveCount++;
                        break;
                    case 10:
                        tenCount++;
                        break;
                    default:
                        if (
                            MoveChecker.IsMoveAdmissible(topOfPile, entry.Value)
                            && (lowestAdmissible == -1 || (int)entry.Value.CardRank < lowestAdmissible)
                        )
                        {
                            lowestAdmissible = (int)entry.Value.CardRank;
                        }
                        break;
                }
            }

            if (lowestAdmissible == -1)
            {
                if (fiveCount > 0) lowestAdmissible = 5;
                else if (tenCount > 0) lowestAdmissible = 10;
                else if (twoCount > 0) lowestAdmissible = 2;
            }

            foreach (var entry in collection)
            {
                if ((int)entry.Value.CardRank != lowestAdmissible) continue;
                this.ToggleSelect(entry.Value);
                if (lowestAdmissible == 2 || lowestAdmissible == 5 || lowestAdmissible == 10) break;
            }
        }
    }
}