using Xyaneon.Games.Cards;
using Xyaneon.Games.Cards.StandardPlayingCards;

namespace Game;

public class Cards : Card
{
    public static StandardPlayingCardDeck deck = new StandardPlayingCardDeck();
    public static void Deal(List<Gamer> gamers)
    {
        foreach (var gamer in gamers)
        {
            for (int i = 0; i < 4; i++)
            {
                gamer.Hand.Add(deck.Draw());
            }
        }
    }
}