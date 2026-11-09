using System;
using System.Collections.Generic;
using UnityEngine;

abstract class HandController: MonoBehaviour
{
    public event Action<FieldType, CardData> OnCardClicked;
    public int HandSize;
    public int OpenCards;
    public int CoveredCards;
    public readonly List<CardData> Selected = new();
    protected bool _isPlayer;
    [SerializeField] protected GameObject _cardPrefab;
    [SerializeField] protected GameObject[] _tableCards = new GameObject[3];
    [SerializeField] protected GameObject _handContainer;
    protected readonly Dictionary<CardView, CardData> _tableCovered = new();
    protected readonly Dictionary<CardView, CardData> _tableOpen = new();
    protected readonly Dictionary<CardView, CardData> _hand = new();

    public void Deal(FieldType field, CardData card)
    {
        GameObject container = field switch
        {
            FieldType.TableCovered => _tableCards[_tableCovered.Count],
            FieldType.TableOpen => _tableCards[_tableOpen.Count],
            _ => _handContainer
        };

        this.AddTo(container, field, card);
        this.HandSize = this._hand.Count;
        this.OpenCards = this._tableOpen.Count;
        this.CoveredCards = this._tableCovered.Count;
    }

    public virtual void SwapCards(CardData tableCard, CardData handCard)
    {
        CardView handCardView = null;
        CardView tableCardView = null;

        foreach (var entry in this._hand)
        {
            if (entry.Value == handCard) handCardView = entry.Key;
        }

        foreach (var entry in this._tableOpen)
        {
            if (entry.Value == tableCard) tableCardView = entry.Key;
        }

        foreach(var elem in this._handContainer.GetComponentsInChildren<CardView>())
        {
            if (elem == handCardView)
            {
                this._hand.Remove(handCardView);
                elem.gameObject.SetActive(false);
                this.AddTo(_handContainer, FieldType.Hand, tableCard);
            }
        }

        foreach (var tableCardContainer in _tableCards)
        {
            foreach (var elem in tableCardContainer.GetComponentsInChildren<CardView>())
            {
                if (elem == tableCardView)
                {
                    this._tableOpen.Remove(tableCardView);
                    elem.gameObject.SetActive(false);
                    this.AddTo(tableCardContainer, FieldType.TableOpen, handCard);
                }
            }
        }
    }

    public virtual void ToggleSelect(CardData card)
    {
        if (this.Selected.Contains(card))
        {
            this.Selected.Remove(card);
        } else
        {
            this.Selected.Add(card);
        }
    }

    public void PlaySelected()
    {
        foreach (var selected in this.Selected)
        {
            CardView cardView = null;
            Dictionary<CardView, CardData> dict = null;

            foreach (var entry in this._tableCovered)
            {
                if (entry.Value == selected)
                {
                    cardView = entry.Key;
                    dict = this._tableCovered;
                }
            }
            foreach (var entry in this._tableOpen)
            {
                if (entry.Value == selected)
                {
                    cardView = entry.Key;
                    dict = this._tableOpen;
                }
            }
            foreach (var entry in this._hand)
            {
                if (entry.Value == selected)
                {
                    cardView = entry.Key;
                    dict = this._hand;
                }
            }
            dict.Remove(cardView);
            cardView.gameObject.SetActive(false);
        }
        this.Selected.Clear();
        this.HandSize = this._hand.Count;
        this.OpenCards = this._tableOpen.Count;
        this.CoveredCards = this._tableCovered.Count;
    }

    public int CountAvailableMoves(FieldType field, CardData topOfPile)
    {
        int c = 0;
        foreach (var entry in this.DictFromField(field))
        {
            if (MoveChecker.IsMoveAdmissible(
                topOfPile,
                entry.Value
            )) c++;
        }
        return c;
    }

    private void AddTo(GameObject container, FieldType field, CardData card)
    {
        GameObject cardObject = Instantiate(_cardPrefab, container.transform.position, Quaternion.identity, container.transform);
        CardView cardView = cardObject.GetComponent<CardView>();

        cardView.SetSprite(
            field switch
            {
                FieldType.TableCovered => SpriteRetriever.CardBackSprite(),
                FieldType.TableOpen => SpriteRetriever.CardSprite(card.CardSuit, card.CardRank),
                _ => _isPlayer ? SpriteRetriever.CardSprite(card.CardSuit, card.CardRank) : SpriteRetriever.CardBackSprite()
            }
        );
        cardView.SetVisible(true);
        cardView.SetClickable(this._isPlayer);

        cardView.OnCardClicked += this.HandleCardClick;

        this.DictFromField(field).Add(cardView, card);
    }

    private Dictionary<CardView, CardData> DictFromField(FieldType fieldType)
    {
        return fieldType switch
        {
            FieldType.TableCovered => _tableCovered,
            FieldType.TableOpen => _tableOpen,
            _ => _hand,
        };
    }

    private void HandleCardClick(CardView cardView)
    {
        if (this._hand.ContainsKey(cardView))
        {
            this.OnCardClicked.Invoke(FieldType.Hand, this._hand[cardView]);            
        } else if (this._tableOpen.ContainsKey(cardView))
        {
            this.OnCardClicked.Invoke(FieldType.TableOpen, this._tableOpen[cardView]);
        } else if (this._tableCovered.ContainsKey(cardView))
        {
            this.OnCardClicked.Invoke(FieldType.TableCovered, this._tableCovered[cardView]);
        }
    }
}