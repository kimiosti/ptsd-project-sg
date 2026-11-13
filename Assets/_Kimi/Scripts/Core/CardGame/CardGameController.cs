using System;
using System.Collections.Generic;
using System.Linq;
using _Kimi.Scripts.Core.Utils;
using _Kimi.Scripts.Gameplay.CardGame;
using UnityEngine;
using UnityEngine.Serialization;

// TODO - implement CPU exchange behavior
// TODO - check game win conditions

namespace _Kimi.Scripts.Core.CardGame
{
    public class CardGameController: MonoBehaviour
    {
        private List<CardData> _deck;
        private GameState _gameState;
        [SerializeField] private PlayerHandController playerHandController;
        [SerializeField] private CpuHandController cpuHandController;
        [SerializeField] private DeckPileController deckAndPileController;

        private void Awake()
        {
            this.playerHandController.OnCardClicked += HandleCardClick;
            this.deckAndPileController.OnPileClicked += HandlePileClick;
        }

        private void Start()
        {
            _deck = GenerateShuffledDeck();

            deckAndPileController.ShowDeck(_deck);
            deckAndPileController.ShowEmptyPile();

            for (var i=0; i<9; i++)
            {
                var playerCard = _deck[0];
                var cpuCard = _deck[1];

                playerHandController.Deal(
                    i < 3 ? FieldType.TableCovered : (i < 6 ? FieldType.TableOpen : FieldType.Hand),
                    playerCard
                );
                cpuHandController.Deal(
                    i < 3 ? FieldType.TableCovered : (i < 6 ? FieldType.TableOpen : FieldType.Hand),
                    cpuCard
                );

                _deck.Remove(playerCard);
                deckAndPileController.Draw(playerCard);
                _deck.Remove(cpuCard);
                deckAndPileController.Draw(cpuCard);
            }

            _gameState = GameState.Exchange;
        }

        private static List<CardData> GenerateShuffledDeck()
        {
            var deck = 
            (
                from Suit suit in Enum.GetValues(typeof(Suit))
                from Rank rank in Enum.GetValues(typeof(Rank)
            ) select new CardData(suit, rank)).ToList();

            deck.Shuffle();
            return deck;
        }

        private void HandleCardClick(FieldType field, CardData card)
        {
            switch (this._gameState)
            {
                case GameState.Exchange:
                    if (field == FieldType.Hand)
                    {
                        this.playerHandController.ToggleSelect(card);

                        if (this.playerHandController.Selected.Count > 1)
                        {
                            this.playerHandController.ToggleSelect(this.playerHandController.Selected[0]);
                        }
                    } else if (field == FieldType.TableOpen && this.playerHandController.Selected.Count > 0)
                    {
                        this.playerHandController.SwapCards(card, this.playerHandController.Selected[0]);
                    }
                    break;

                default:
                    if (
                        field == FieldType.Hand
                        || (field == FieldType.TableOpen && this.playerHandController.handSize == 0)
                    )
                    {
                        if (MoveChecker.IsMoveAdmissible(
                                this.deckAndPileController.TopOfPile,
                                card
                            ))
                        {
                            this.playerHandController.ToggleSelect(card);
                            if (this.playerHandController.Selected.Count > 1)
                            {
                                List<CardData> toDeselect = new();
                                foreach (var selectedCard in this.playerHandController.Selected)
                                {
                                    if (selectedCard.CardRank != card.CardRank) toDeselect.Add(selectedCard);
                                }
                                foreach (var cardToDeselect in toDeselect)
                                {
                                    this.playerHandController.ToggleSelect(cardToDeselect);
                                }
                            }
                        }
                    } else if (
                        field == FieldType.TableCovered
                        && this.playerHandController.handSize == 0
                        && this.playerHandController.openCards == 0
                    )
                    {
                        List<CardData> toDeselect = new(this.playerHandController.Selected);
                        foreach (var cardToDeselect in toDeselect)
                        {
                            this.playerHandController.ToggleSelect(cardToDeselect);
                        }
                        this.playerHandController.ToggleSelect(card);
                    }
                    break;
            }
        }

        private void HandlePileClick()
        {
            switch (this._gameState)
            {
                case GameState.Exchange:
                    // TODO - implement CPU exchange behaviour
                    this._gameState = GameState.Play;
                    break;

                default:
                    if (this.playerHandController.Selected.Count > 0)
                    {
                        foreach (var selectedCard in this.playerHandController.Selected)
                        {
                            this.deckAndPileController.AddToPile(selectedCard);
                            this.CheckAndFlushPile();
                        }
                        this.playerHandController.PlaySelected();
                    
                        this.CheckMoveAdmissibilityAndPickUpPile(this.playerHandController);

                        this.DrawFromDeck(this.playerHandController);

                        this.CheckMovePossibilityAndPickUpPile(this.cpuHandController);

                        this.cpuHandController.SelectCards(this.deckAndPileController.TopOfPile);
                        foreach (var card in this.cpuHandController.Selected)
                        {
                            this.deckAndPileController.AddToPile(card);
                            this.CheckAndFlushPile();
                        }
                        this.cpuHandController.PlaySelected();

                        this.DrawFromDeck(this.cpuHandController);

                        this.CheckMoveAdmissibilityAndPickUpPile(this.cpuHandController);

                        this.CheckMovePossibilityAndPickUpPile(this.playerHandController);
                    }
                    break;
            }
        }

        private void CheckAndFlushPile()
        {
            var pile = this.deckAndPileController.GetPileContent();
            if (
                ((int)this.deckAndPileController.TopOfPile.CardRank) == 10
                || (
                    pile.Count > 4
                    && pile[^1] == pile[^2]
                    && pile[^2] == pile[^3]
                    && pile[^3] == pile[^4]
                )
            ) this.deckAndPileController.FlushPile();
        }

        private void CheckMoveAdmissibilityAndPickUpPile(HandController player)
        {
            var pile = this.deckAndPileController.GetPileContent();
            if (
                pile.Count > 2
                && !MoveChecker.IsMoveAdmissible(pile[^2], pile[^1])
            ) this.PickUpPile(player);
        }

        private void PickUpPile(HandController player)
        {
            foreach (var card in this.deckAndPileController.GetPileContent())
            {
                player.Deal(FieldType.Hand, card);
            }
            this.deckAndPileController.FlushPile();
        }

        private void DrawFromDeck(HandController player)
        {
            while (player.handSize < 3 && this._deck.Count > 0)
            {
                this.deckAndPileController.Draw(this._deck[0]);
                player.Deal(FieldType.Hand, this._deck[0]);
                this._deck.Remove(this._deck[0]);
            }
        }

        private void CheckMovePossibilityAndPickUpPile(HandController player)
        {
            if (
                (
                    player.CountAvailableMoves(FieldType.Hand, this.deckAndPileController.TopOfPile) == 0
                    && player.handSize != 0
                )
                || (
                    player.CountAvailableMoves(FieldType.TableOpen, this.deckAndPileController.TopOfPile) == 0
                    && player.handSize == 0
                    && player.openCards != 0
                )
            ) this.PickUpPile(player);
        }
    }
}