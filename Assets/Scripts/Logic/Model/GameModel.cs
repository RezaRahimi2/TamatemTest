using System.Collections.Generic;
using GamePlayView;
using JetBrains.Annotations;
using UnityEngine;

namespace GamePlayLogic
{
    // This class represents the game model and inherits from SingletonNonMono<GameModel>.
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

        // The following properties are only available when debugging.

#if Debugging
        // This boolean determines whether to use the player index for position. By default, it is set to true.
        public bool UsePlayerIndexForPosition = false;

        // This integer represents the player position index. It should be between 0 and PlayerPositionIndex - 1.
        // By default, it is set to 0.
        public int PlayerPositionIndex = 0;
#endif
    }

}