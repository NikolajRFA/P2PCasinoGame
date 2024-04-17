
using Game;

List<Gamer> gamers = [];

gamers.Add(new Gamer());
gamers.Add(new Gamer());

Cards.deck.Shuffle();

Cards.Deal(gamers);

foreach (var gamer in gamers)
{
    Console.WriteLine("-----");
    foreach (var card in gamer.Hand)
    {
        Console.WriteLine($"{card.Rank} of {card.Suit}");
    }
}