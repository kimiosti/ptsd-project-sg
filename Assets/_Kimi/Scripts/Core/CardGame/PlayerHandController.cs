using System;
using System.Collections.Generic;
using UnityEngine;

class PlayerHandController: HandController
{
    public void Awake()
    {
        this._isPlayer = true;
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

        foreach (var entry in this._tableCovered)
        {
            if (entry.Value == card) cardView = entry.Key;
        }

        foreach (var entry in this._tableOpen)
        {
            if (entry.Value == card) cardView = entry.Key;
        }

        foreach (var entry in this._hand)
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