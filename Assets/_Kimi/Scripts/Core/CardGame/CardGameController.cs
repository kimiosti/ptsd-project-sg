using System;
using System.Collections.Generic;
using UnityEngine;

// TODO - implement CPU exchange behavior
// TODO - check game win conditions

class CardGameController: MonoBehaviour
{
    private List<CardData> _deck;
    private GameState _gameState;
    [SerializeField] private PlayerHandController _playerHandController;
    [SerializeField] private CpuHandController _cpuHandController;
    [SerializeField] private DeckPileController _deckAndPileController;

    private void Awake()
    {
        this._playerHandController.OnCardClicked += HandleCardClick;
        this._deckAndPileController.OnPileClicked += HandlePileClick;
    }

    private void Start()
    {
        _deck = GenerateShuffledDeck();

        _deckAndPileController.ShowDeck(_deck);
        _deckAndPileController.ShowEmptyPile();

        for (int i=0; i<9; i++)
        {
            var playerCard = _deck[0];
            var cpuCard = _deck[1];

            _playerHandController.Deal(
                i < 3 ? FieldType.TableCovered : (i < 6 ? FieldType.TableOpen : FieldType.Hand),
                playerCard
            );
            _cpuHandController.Deal(
                i < 3 ? FieldType.TableCovered : (i < 6 ? FieldType.TableOpen : FieldType.Hand),
                cpuCard
            );

            _deck.Remove(playerCard);
            _deckAndPileController.Draw(playerCard);
            _deck.Remove(cpuCard);
            _deckAndPileController.Draw(cpuCard);
        }

        _gameState = GameState.Exchange;
    }

    private List<CardData> GenerateShuffledDeck()
    {
        var deck = new List<CardData>();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                deck.Add(new CardData(suit, rank));
            }
        }

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
                    this._playerHandController.ToggleSelect(card);

                    if (this._playerHandController.Selected.Count > 1)
                    {
                        this._playerHandController.ToggleSelect(this._playerHandController.Selected[0]);
                    }
                } else if (field == FieldType.TableOpen && this._playerHandController.Selected.Count > 0)
                {
                    this._playerHandController.SwapCards(card, this._playerHandController.Selected[0]);
                }
                break;

            default:
                if (
                    field == FieldType.Hand
                    || (field == FieldType.TableOpen && this._playerHandController.HandSize == 0)
                )
                {
                    if (MoveChecker.IsMoveAdmissible(
                        this._deckAndPileController.TopOfPile,
                        card
                    ))
                    {
                        this._playerHandController.ToggleSelect(card);
                        if (this._playerHandController.Selected.Count > 1)
                        {
                            List<CardData> toDeselect = new();
                            foreach (var selectedCard in this._playerHandController.Selected)
                            {
                                if (selectedCard.CardRank != card.CardRank) toDeselect.Add(selectedCard);
                            }
                            foreach (var cardToDeselect in toDeselect)
                            {
                                this._playerHandController.ToggleSelect(cardToDeselect);
                            }
                        }
                    }
                } else if (
                    field == FieldType.TableCovered
                    && this._playerHandController.HandSize == 0
                    && this._playerHandController.OpenCards == 0
                )
                {
                    List<CardData> toDeselect = new(this._playerHandController.Selected);
                    foreach (var cardToDeselect in toDeselect)
                    {
                        this._playerHandController.ToggleSelect(cardToDeselect);
                    }
                    this._playerHandController.ToggleSelect(card);
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
                // player discards
                // check for pile flushing
                // check move admissibility, player draws
                // check if CPU has to draw
                // CPU plays admissible card
                // check move admissibility, CPU draws
                // check if player has to draw

                if (this._playerHandController.Selected.Count > 0)
                {
                    foreach (var selectedCard in this._playerHandController.Selected)
                    {
                        this._deckAndPileController.AddToPile(selectedCard);
                        this.CheckAndFlushPile();
                    }
                    this._playerHandController.PlaySelected();
                    
                    this.CheckMoveAdmissibilityAndPickUpPile(this._playerHandController);

                    this.DrawFromDeck(this._playerHandController);

                    this.CheckMovePossibilityAndPickUpPile(this._cpuHandController);

                    this._deckAndPileController.AddToPile(
                        this._cpuHandController.PlayCard(this._deckAndPileController.TopOfPile)
                    );
                    this.CheckAndFlushPile();
                    this._cpuHandController.PlaySelected();

                    this.DrawFromDeck(this._cpuHandController);

                    this.CheckMoveAdmissibilityAndPickUpPile(this._cpuHandController);

                    this.CheckMovePossibilityAndPickUpPile(this._playerHandController);
                }
                break;
        }
    }

    private void CheckAndFlushPile()
    {
        var pile = this._deckAndPileController.GetPileContent();
        if (
            ((int)this._deckAndPileController.TopOfPile.CardRank) == 10
            || (
                pile.Count > 4
                && pile[^1] == pile[^2]
                && pile[^2] == pile[^3]
                && pile[^3] == pile[^4]
            )
        ) this._deckAndPileController.FlushPile();
    }

    private void CheckMoveAdmissibilityAndPickUpPile(HandController player)
    {
        var pile = this._deckAndPileController.GetPileContent();
        if (
            pile.Count > 2
            && !MoveChecker.IsMoveAdmissible(pile[^2], pile[^1])
        ) this.PickUpPile(player);
    }

    private void PickUpPile(HandController player)
    {
        foreach (var card in this._deckAndPileController.GetPileContent())
        {
            player.Deal(FieldType.Hand, card);
        }
        this._deckAndPileController.FlushPile();
    }

    private void DrawFromDeck(HandController player)
    {
        while (player.HandSize < 3 && this._deck.Count > 0)
        {
            this._deckAndPileController.Draw(this._deck[0]);
            player.Deal(FieldType.Hand, this._deck[0]);
            this._deck.Remove(this._deck[0]);
        }
    }

    private void CheckMovePossibilityAndPickUpPile(HandController player)
    {
        if (
            (
                player.CountAvailableMoves(FieldType.Hand, this._deckAndPileController.TopOfPile) == 0
                && player.HandSize != 0
            )
            || (
                player.CountAvailableMoves(FieldType.TableOpen, this._deckAndPileController.TopOfPile) == 0
                && player.HandSize == 0
                && player.OpenCards != 0
            )
        ) this.PickUpPile(player);
    }
}