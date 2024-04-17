﻿using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Player
{
    public string Name { get; set; }
    public List<StandardPlayingCard> Hand { get; set; } = [];
    public List<StandardPlayingCard> PointPile { get; set; } = [];
    public int ClearCount { get; set; }

    public bool PlaceCard(Table table, int handIndex)
    {
        var drawPile = new DrawPile<StandardPlayingCard>(isFaceUp: true);
        drawPile.PlaceOnTop(Hand[handIndex]);
        table.Cards.Add(new KeyValuePair<DrawPile<StandardPlayingCard>, List<int>>(drawPile, GameState.CardToValue(Hand[handIndex])));
        Hand.RemoveAt(handIndex);
        
        return true;
    }

    public bool Build(int index, StandardPlayingCard card, int value)
    {
        var kvp = Cards[index];
        var cardValues = GameState.CardToValue(card);

        if (!CompareValues(kvp.Value, cardValues, value)) return false;

        kvp.Key.PlaceOnTop(card);
        kvp.Value.Clear();
        kvp.Value.Add(value);
        return true;
    }

    public bool BuildTable(int index1, int index2, int value)
    {
        var cards1 = Cards[index1].Value;
        var cards2 = Cards[index2].Value;
        return CompareValues(cards1, cards2, value);
    }

    public bool Take(Table table, int index, StandardPlayingCard card)
    {
        if (table.Cards[index].Value.Any(x => GameState.CardToValue(card).Any(y => x == y)))
        {
            foreach (var tableCard in table.Cards[index].Key.Cards)
            {
                PointPile.Add(tableCard);
            }

            if (table.Cards.Count == 1) ClearCount++;
            table.Cards.RemoveAt(index);
            return true;
        }

        return false;
    }

    public bool ClearTable(Table table, StandardPlayingCard standardPlayingCard)
    {
        foreach (var card in table.Cards)
        {
            PointPile.AddRange(card.Key.Cards);
        }

        table.Cards.Clear();
        return true;
    }


    private bool CompareValues(IEnumerable<int> values1, IEnumerable<int> values2, int value)
    {
        return values1.Any(cardValue => values2.Any(tableValue => cardValue + tableValue == value));
    }
}