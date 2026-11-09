using System;
using System.Collections.Generic;
using _Kimi.Scripts.Gameplay.CardGame;
using _Kimi.Scripts.UI.CardGame;
using UnityEngine;
using UnityEngine.Serialization;

namespace _Kimi.Scripts.Core.CardGame
{
    public class DeckPileController: MonoBehaviour
    {
        public CardData TopOfPile { get; private set; } = null;
        public event Action OnPileClicked;
        [SerializeField] private GameObject cardPrefab;
        [SerializeField] private GameObject deckContainer;
        [SerializeField] private GameObject pileContainer;
        private readonly Dictionary<CardView, CardData> _deck = new();
        private readonly Dictionary<CardView, CardData> _pile = new();

        public void ShowDeck(List<CardData> deck)
        {
            foreach (var card in deck)
            {
                this.AddCard(
                    this.deckContainer,
                    this._deck,
                    card,
                    SpriteRetriever.CardBackSprite(),
                    false
                );
            }
        }

        public void Draw(CardData card)
        {
            CardView cardView = null;

            foreach(var entry in this._deck)
            {
                if (entry.Value == card) cardView = entry.Key;
            }

            if (cardView != null)
            {
                this._deck.Remove(cardView);
                cardView.gameObject.SetActive(false);
            }
        }

        public void ShowEmptyPile()
        {
            this.AddCard(
                this.pileContainer,
                this._pile,
                null,
                SpriteRetriever.EmptyCardSprite(),
                true
            );
        }

        public void AddToPile(CardData card)
        {
            this.AddCard(
                this.pileContainer,
                this._pile,
                card,
                SpriteRetriever.CardSprite(card.CardSuit, card.CardRank),
                false
            );
            this.TopOfPile = card;
        }

        public void FlushPile()
        {
            this.TopOfPile = null;
            this._pile.Clear();
            this.ShowEmptyPile();
        }

        public List<CardData> GetPileContent()
        {
            List<CardData> pile = new();
            foreach (var entry in this._pile)
            {
                if (entry.Value != null) pile.Add(entry.Value);
            }
            return pile;
        }

        private void AddCard(GameObject containerObj, Dictionary<CardView, CardData> container, CardData card, Sprite sprite, bool clickable)
        {
            GameObject cardObj = Instantiate(cardPrefab, containerObj.transform.position, Quaternion.identity, containerObj.transform);
            CardView cardView = cardObj.GetComponent<CardView>();

            container.Add(cardView, card);
            cardView.SetSprite(sprite);
            cardView.SetVisible(true);
            cardView.SetClickable(clickable);

            if (container == this._pile) cardView.OnCardClicked += this.HandlePileClick;
        }

        private void HandlePileClick(CardView topOfPile)
        {
            this.OnPileClicked?.Invoke();
        }
    }
}
