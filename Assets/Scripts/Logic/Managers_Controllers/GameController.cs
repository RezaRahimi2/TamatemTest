using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using Events;
using GamePlayView;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlayLogic
{
    public class GameController : Singleton<GameController>
    {
        #region Fields

        // Fields for game control and state management
        [field: SerializeField] public DiceManager DiceManager { get; private set; }
        [field: SerializeField] public Board Board { get; private set; }
        public CancellationTokenSource CancellationToken;
        [SerializeField] private List<PlayerBase> m_players;

        // Flags to control game flow
        [SerializeField] private bool m_waitForDiceAnimationFinished;
        [SerializeField] private bool m_waitForChipAnimationFinished;
        [SerializeField] private bool m_waitForInput;
        [SerializeField] private bool m_waitForUIChanging;

        public PlayerBase GetPlayerByIndex(int index) => m_players.Find(x => x.PlayerIndex == index);

        #endregion

        #region GameLogic

        // Game initialization logic
        public async UniTask Initialize()
        {
            CancellationToken = new CancellationTokenSource();
            EventManager.Subscribe<OnDiceAnimationStartedEvent>((arg) => { m_waitForDiceAnimationFinished = true; });
            EventManager.Subscribe<OnDiceAnimationFinishedEvent>((arg) => { m_waitForDiceAnimationFinished = false; });
            EventManager.Subscribe<OnChipAnimationStartedEvent>((arg) => { m_waitForChipAnimationFinished = true; });
            EventManager.Subscribe<OnChipAnimationFinishedEvent>((arg) => { m_waitForChipAnimationFinished = false; });
            EventManager.Subscribe<OnUIChangingStartedEvent>((arg) => { m_waitForUIChanging = true; });
            EventManager.Subscribe<OnUIChangingEndEvent>((arg) => { m_waitForUIChanging = false; });
            EventManager.Subscribe<OnWaitingForInputStartedEvent>((arg) => { m_waitForInput = true; });
            EventManager.Subscribe<OnWaitingForInputFinishedEvent>((arg) => { m_waitForInput = false; });

            DiceManager = new DiceManager();

            Board = new Board();
            m_players = new List<PlayerBase>();
            if (GameModel.Instance.PlayerBotSetter == null || GameModel.Instance.PlayerBotSetter.Count == 0)
            {
                GameModel.Instance.PlayerBotSetter = new List<bool>();

                for (int i = 0; i < GameModel.Instance.PlayerNumber; i++)
                {
                    GameModel.Instance.PlayerBotSetter.Add(i != 0);
                }
            }

            // Add players to the list
            for (int i = 0; i < GameModel.Instance.PlayerNumber; i++)
            {
                PlayerBase playerBase = null;

#if Debugging
                if (GameModel.Instance.PlayerNumber == 1 && GameModel.Instance.UsePlayerIndexForPosition)
                {
                    playerBase = new Player(GameModel.Instance.PlayerPositionIndex, false,
                        GameModel.Instance.NumberOfChips, GameModel.Instance.PlayerNumber, Board,
                        (13) * GameModel.Instance.PlayerPositionIndex);
                }
                else
                {
                    if (!GameModel.Instance.PlayerBotSetter[i])
                        playerBase = new Player(i, false, GameModel.Instance.NumberOfChips, i, Board, (13) * i);
                    else
                        playerBase = new AI(i, true, GameModel.Instance.NumberOfChips, i, Board, (13) * i);
                }
#else
                if (!GameModel.Instance.PlayerBotSetter[i])
                        playerBase = new Player(i,false, GameModel.Instance.NumberOfChips, i, Board, (13) * i);
                    else
                        playerBase = new AI(i,true, GameModel.Instance.NumberOfChips, i, Board, (13) * i);
#endif
                m_players.Add(playerBase);
            }

            // Start the game
            StartGame();
        }

        private void StartGame()
        {
            Debug.unityLogger.Log($"Starting Game");
            for (var i = 0; i < m_players.Count; i++)
            {
                m_players[i].PlayerIndex = i;
            }

            EventManager.Broadcast(new OnStartGameEvent() { Board = Board, Players = m_players });

            StartRound();
        }

        async UniTask StartRound()
        {
            // Log the start of a new round
            Debug.unityLogger.Log($"GameController.StartRound::Starting Round");

            // Loop through all players
            for (int i = 0; i < m_players.Count; i++)
            {
                // Wait for a second before starting the player's turn
                await UniTask.WaitForSeconds(1f);

                // Get the current player and initialize their state
                PlayerBase player = m_players[i];
                player.ConsecutiveSixes = 0;
                player.IsReadyToPlay = true;

                // Notify the player that it's their turn
                player.OnTurn();

                // Broadcast an event to update the UI with the current player's turn
                EventManager.Broadcast(new OnSetPlayerTurnEvent() { PlayerIndex = player.PlayerIndex });
                Debug.unityLogger.Log(
                    $"<color=yellow>GameController.StartRound::Player {player.PlayerIndex} is ready to play</color>");

                // Wait until all animations and input are finished
                await UniTask.WaitWhile(IsWaiting);

                // Roll the dice for the current player
                await player.RollDies();

                // Wait until all animations and input are finished
                await UniTask.WaitWhile(IsWaiting);

                // If the player has no playable chips, skip their turn
                if (!player.HasPlayableChip())
                    continue;

                // Set the playable chip for the player
                player.SetPlayableChip();

                // Move the player's chip based on the dice roll
                player.MoveChip(player.DiceRollResult1 + player.DiceRollResult2,
                    (player.DiceRollResult1 == 6 || player.DiceRollResult2 == 6));

                // If all of the player's chips are at the end home, break the loop
                if (player.AllChipsAtEndHome())
                    break;

                // Wait until all animations and input are finished
                await UniTask.WaitWhile(IsWaiting);

                // If a six was rolled, the player gets an extra roll after his move
                while ((player.DiceRollResult1 == 6 || player.DiceRollResult2 == 6) && player.ConsecutiveSixes <= 2
                       && !player.AllChipsAtEndHome())
                {
                    // Check if a six was rolled
                    if (player.DiceRollResult1 == 6 || player.DiceRollResult2 == 6)
                    {
                        player.ConsecutiveSixes++;
                    }
                    else
                    {
                        player.ConsecutiveSixes = 0;
                    }

                    // Wait for a second before the next roll
                    await UniTask.WaitForSeconds(1f);
                    Debug.unityLogger.Log(
                        "GameController.StartRound::Player rolled a six, getting an extra roll after his move");

                    // Wait until all animations and input are finished
                    await UniTask.WaitWhile(IsWaiting);

                    // Roll the dice for the current player
                    player.RollDies();

                    // Wait until all animations and input are finished
                    await UniTask.WaitWhile(IsWaiting);

                    // If the player has no playable chips, skip their turn
                    if (!player.HasPlayableChip())
                        continue;

                    // Set the playable chip for the player
                    player.SetPlayableChip();

                    // Move the player's chip based on the dice roll
                    player.MoveChip(player.DiceRollResult1 + player.DiceRollResult2,
                        (player.DiceRollResult1 == 6 || player.DiceRollResult2 == 6));
                }

                // Wait until all animations and input are finished
                await UniTask.WaitWhile(IsWaiting);
            }

            // If there's a winner, determine the winner, else start a new round
            if (CheckGameWinner())
            {
                DetermineGameWinner();
            }
            else
            {
                await StartRound();
            }
        }


        bool CheckGameWinner()
        {
            foreach (PlayerBase player in m_players)
            {
                if (player.AllChipsAtEndHome())
                {
                    return true;
                }
            }

            return false;
        }

        void DetermineGameWinner()
        {
            PlayerBase gameWinner = m_players.Find(player => player.AllChipsAtEndHome());

            EventManager.Broadcast(new OnWinnerEvent() { PlayerIndex = gameWinner.PlayerIndex });
            Debug.Log(
                $"<color=blue>GameController.DetermineGameWinner::The winner is: {gameWinner.PlayerIndex}</color>");
        }

        private bool IsWaiting()
        {
            return m_waitForDiceAnimationFinished || m_waitForChipAnimationFinished || m_waitForInput ||
                   m_waitForUIChanging;
        }

        #endregion
    }
}