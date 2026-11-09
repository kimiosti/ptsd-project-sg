using _Kimi.Scripts.Gameplay.CardGame;
using _Kimi.Scripts.UI.CardGame;
using UnityEngine;

namespace _Kimi.Scripts.Core.CardGame
{
    public class PlayerHandController: HandController
    {
        public void Awake()
        {
            this.IsPlayer = true;
        }

        public override void SwapCards(CardData tableCard, CardData handCard)
        {
            this.Selected.Clear();
            base.SwapCards(tableCard, handCard);
        }

        public override void ToggleSelect(CardData card)
        {
            base.ToggleSelect(card);
            this.ToggleSelectView(card, this.Selected.Contains(card));
        }

        private void ToggleSelectView(CardData card, bool isSelected)
        {
            CardView cardView = null;

            foreach (var entry in this.TableCovered)
            {
                if (entry.Value == card) cardView = entry.Key;
            }

            foreach (var entry in this.TableOpen)
            {
                if (entry.Value == card) cardView = entry.Key;
            }

            foreach (var entry in this.Hand)
            {
                if (entry.Value == card) cardView = entry.Key;
            }

            if (cardView != null)
            {
                cardView.transform.position = cardView.transform.position + new Vector3(
                    0,
                    isSelected ? 15 : -15,
                    0
                );    
            }
        }
    }
}