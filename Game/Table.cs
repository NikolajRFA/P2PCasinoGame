﻿using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Table
{
    public List<KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>> Cards { get; set; } = [];

    public void AddCards(IEnumerable<StandardPlayingCard> cards)
    {
        foreach (var card in cards)
        {
            var drawPile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
            drawPile.PlaceOnTop(card);
            Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(
                drawPile,
                GameState.CardToValue(card).Item1
            ));
        }
    }
}