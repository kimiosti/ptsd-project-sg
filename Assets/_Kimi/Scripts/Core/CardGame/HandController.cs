using System;
using System.Collections.Generic;
using _Kimi.Scripts.Gameplay.CardGame;
using _Kimi.Scripts.UI.CardGame;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Kimi.Scripts.Core.CardGame
{
    public abstract class HandController: MonoBehaviour
    {
        public event Action<FieldType, CardData> OnCardClicked;
        public int handSize;
        public int openCards;
        public int coveredCards;
        public readonly List<CardData> Selected = new();
        protected bool IsPlayer;
        [SerializeField] protected GameObject cardPrefab;
        [SerializeField] protected GameObject[] tableCards = new GameObject[3];
        [SerializeField] protected GameObject handContainer;
        protected readonly Dictionary<CardView, CardData> TableCovered = new();
        protected readonly Dictionary<CardView, CardData> TableOpen = new();
        protected readonly Dictionary<CardView, CardData> Hand = new();

        public void Deal(FieldType field, CardData card)
        {
            var container = field switch
            {
                FieldType.TableCovered => tableCards[TableCovered.Count],
                FieldType.TableOpen => tableCards[TableOpen.Count],
                _ => handContainer
            };

            this.AddTo(container, field, card);
            this.handSize = this.Hand.Count;
            this.openCards = this.TableOpen.Count;
            this.coveredCards = this.TableCovered.Count;
        }

        public virtual void SwapCards(CardData tableCard, CardData handCard)
        {
            CardView handCardView = null;
            CardView tableCardView = null;

            foreach (var entry in this.Hand)
            {
                if (entry.Value == handCard) handCardView = entry.Key;
            }

            foreach (var entry in this.TableOpen)
            {
                if (entry.Value == tableCard) tableCardView = entry.Key;
            }

            foreach(var elem in this.handContainer.GetComponentsInChildren<CardView>())
            {
                if (handCardView != null && elem == handCardView)
                {
                    this.Hand.Remove(handCardView);
                    elem.gameObject.SetActive(false);
                    this.AddTo(handContainer, FieldType.Hand, tableCard);
                }
            }

            foreach (var tableCardContainer in tableCards)
            {
                foreach (var elem in tableCardContainer.GetComponentsInChildren<CardView>())
                {
                    if (tableCardView != null && elem == tableCardView)
                    {
                        this.TableOpen.Remove(tableCardView);
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

                foreach (var entry in this.TableCovered)
                {
                    if (entry.Value == selected)
                    {
                        cardView = entry.Key;
                        dict = this.TableCovered;
                    }
                }
                foreach (var entry in this.TableOpen)
                {
                    if (entry.Value == selected)
                    {
                        cardView = entry.Key;
                        dict = this.TableOpen;
                    }
                }
                foreach (var entry in this.Hand)
                {
                    if (entry.Value == selected)
                    {
                        cardView = entry.Key;
                        dict = this.Hand;
                    }
                }
                dict?.Remove(cardView);
                cardView?.gameObject.SetActive(false);
            }
            this.Selected.Clear();
            this.handSize = this.Hand.Count;
            this.openCards = this.TableOpen.Count;
            this.coveredCards = this.TableCovered.Count;
        }

        public int CountAvailableMoves(FieldType field, CardData topOfPile)
        {
            var c = 0;
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
            var cardObject = Instantiate(cardPrefab, container.transform.position, Quaternion.identity, container.transform);
            var cardView = cardObject.GetComponent<CardView>();

            cardView.SetSprite(
                field switch
                {
                    FieldType.TableCovered => SpriteRetriever.CardBackSprite(),
                    FieldType.TableOpen => SpriteRetriever.CardSprite(card.CardSuit, card.CardRank),
                    _ => IsPlayer ? SpriteRetriever.CardSprite(card.CardSuit, card.CardRank) : SpriteRetriever.CardBackSprite()
                }
            );
            cardView.SetVisible(true);
            cardView.SetClickable(this.IsPlayer);

            cardView.OnCardClicked += this.HandleCardClick;

            this.DictFromField(field).Add(cardView, card);
        }

        private Dictionary<CardView, CardData> DictFromField(FieldType fieldType)
        {
            return fieldType switch
            {
                FieldType.TableCovered => TableCovered,
                FieldType.TableOpen => TableOpen,
                _ => Hand,
            };
        }

        private void HandleCardClick(CardView cardView)
        {
            if (this.Hand.TryGetValue(cardView, out var valueHand))
            {
                this.OnCardClicked?.Invoke(FieldType.Hand, valueHand);            
            } else if (this.TableOpen.TryGetValue(cardView, out var valueTableOpen))
            {
                this.OnCardClicked?.Invoke(FieldType.TableOpen, valueTableOpen);
            } else if (this.TableCovered.TryGetValue(cardView, out var valueTableCovered))
            {
                this.OnCardClicked?.Invoke(FieldType.TableCovered, valueTableCovered);
            }
        }
    }
}