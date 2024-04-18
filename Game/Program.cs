using Game;

var gameState = new GameState(["Alex", "Nikolaj", "Laust", "Bulskov"]);
gameState.Setup();

for (int i = 0; i < 48/gameState.Players.Count/4; i++)
{
    for (int j = 0; j < 4; j++)
    {
        for (int k = 0; k < gameState.Players.Count; k++)
        {
            Console.ReadLine();
        }
    }
    
    gameState.Deal();
}