The implementation of the game logic is 80% done, and I didn't test the logic of blocking a home with several chips of a player,

GameModel has several fields for the players' numbers and chip numbers and which player is an AI.

public class GameModel:SingletonNonMono<GameModel>
{
	// This list represents the order of player colors in the game.
	// It can be null, and by default, it contains Yellow, Green, Blue, and Red.
	[CanBeNull] public List<PlayerColorName> ColorNameOrder = new List<PlayerColorName>()
	{
		PlayerColorName.Yellow,
		PlayerColorName.Green,
		PlayerColorName.Blue,
		PlayerColorName.Red
	};

	// This integer represents the number of players in the game. By default, it is set to 1.
	public int PlayerNumber = 4;

	// This integer represents the number of chips each player has in the game. By default, it is set to 1.
	public int NumberOfChips = 1;

	// This integer represents the number of blocks on the game board. By default, it is set to 52.
	public int BlockNumber = 52;

	// This integer represents the number of home blocks in the game. By default, it is set to 5.
	public int HomeNumber = 5;

	// This list represents whether each player is a bot. By default, all players are set to be bots.
	public List<bool> PlayerBotSetter = new List<bool>()
	{
		false,true,true,true
	};
}