using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Events;
using UnityEngine;

namespace GamePlayLogic
{
    [Serializable]
    public abstract class PlayerBase
    {
        // The index of the player's color.
        [field: SerializeField] public int ColorIndex { get; protected set; }
    
        // Indicates whether the player is a bot.
        [field: SerializeField] public bool IsBot { get; protected set; }
    
        // The index of the player.
        [field: SerializeField] public int PlayerIndex { get; set; }
    
        // The index of the starting block for the player.
        [field: SerializeField] public int StartBlockIndex { get; set; }
    
        // The chips that the player has.
        [field: SerializeField] public Chip[] Chips { get; protected set; }
    
        // Indicates whether the player is ready to play.
        [field: SerializeField] public bool IsReadyToPlay { get; set; }
    
        // The board on which the player is playing.
        [field: SerializeField] public Board Board { get; set; }
    
        // Indicates whether the player's chip is in the end home.
        [field: SerializeField] public bool IsChipInEndHome { get; protected set; }
    
        // The chip that the player is currently playing.
        [field: SerializeField] public Chip CurrentPlayedChip { get; set; }

        // The result of the first dice roll.
        [field: SerializeField] public int DiceRollResult1 { get; set; }
    
        // The result of the second dice roll.
        [field: SerializeField] public int DiceRollResult2 { get; set; }
    
        // The number of consecutive sixes the player has rolled.
        [field: SerializeField] public int ConsecutiveSixes { get; set; }
    
        // A callback function that checks if both dice have been rolled.
        protected Func<bool> m_apiCallback;

        // The constructor for the PlayerBase class.
        protected PlayerBase(int colorIndex,bool isBot, int numberOfChips, int playerIndex, Board board, int startBlockIndex)
        {
            m_apiCallback = ()=> DiceRollResult1 != 0 && DiceRollResult2!= 0;
            ColorIndex = colorIndex;
            IsBot = isBot;
            StartBlockIndex = startBlockIndex;
            PlayerIndex = playerIndex;
            Chips = new Chip[numberOfChips];
            for (int i = 0; i < numberOfChips; i++)
            {
                Chips[i] = new Chip(i,playerIndex);
            }

            Board = board;
        }

        // This method is called when it's the player's turn.
        public void OnTurn()
        {
            Debug.unityLogger.Log($"<color=white>GameController.OnTurn::Player {PlayerIndex} Turn</color>");
            if (!IsBot)
            {
                EventManager.Broadcast(new OnStartDiceRollEvent());
            }
        }
        
        // This method is for rolling the dice.
        public abstract UniTask RollDies();

        // This method is for moving the chip.
        public virtual void MoveChip()
        {
            bool hitToOtherChip = false;
            int newBlockIndex;
            int spaces = DiceRollResult1 + DiceRollResult2;

            if (CurrentPlayedChip.IsAtHome)
            {
                newBlockIndex = (StartBlockIndex + spaces) % GameModel.Instance.BlockNumber;
            }
            else
            {
                newBlockIndex = (CurrentPlayedChip.CurrentBlock.Index + spaces) % (GameModel.Instance.BlockNumber - 1);
            }

            // Check if the new block is occupied by an opponent's chip
            if (Board.Blocks[newBlockIndex].IsOccupied &&
                Board.Blocks[newBlockIndex].OccupyingChip.PlayerIndex != PlayerIndex && !Board.Blocks[newBlockIndex].IsBlocked)
            {
                hitToOtherChip = true;
                // Send the opponent's chip back to home
                Board.Blocks[newBlockIndex].OccupyingChip.Position = -1;
                Board.Blocks[newBlockIndex].OccupyingChip.IsAtHome = true;
                Board.Blocks[newBlockIndex].OccupyingChip.MoveToHome();
                Debug.unityLogger.Log($"<color=red>Player {PlayerIndex}-{((PlayerColorName)ColorIndex).ToString()} with chip " +
                                      $"{CurrentPlayedChip.Index}  Hit chip {Board.Blocks[newBlockIndex].OccupyingChip.Index} of " +
                                      $"Player {Board.Blocks[newBlockIndex].OccupyingChip.PlayerIndex}-" +
                                      $"{((PlayerColorName)GameController.Instance.GetPlayerByIndex(Board.Blocks[newBlockIndex].OccupyingChip.PlayerIndex).ColorIndex).ToString()}</color>");
                
                EventManager.Broadcast(new OnHitChipEvent()
                    { PlayerIndex = Board.Blocks[newBlockIndex].OccupyingChip.PlayerIndex, HitChipIndex = Board.Blocks[newBlockIndex].OccupyingChip.Index });
            }
            // Check if the new block is occupied by the player's own chip
            else if (Board.Blocks[newBlockIndex].IsOccupied &&
                     Board.Blocks[newBlockIndex].OccupyingChip.PlayerIndex == PlayerIndex)
            {
                // Block the space
                Board.Blocks[newBlockIndex].Blocking();
                 EventManager.Broadcast(new OnBlockChipEvent()
                    { Chip = CurrentPlayedChip, BlockChip = Board.Blocks[newBlockIndex].OccupyingChip });
            }
            else if(Board.Blocks[newBlockIndex].IsBlocked)
            {
                if(!IsBot)
                    EventManager.Broadcast(new OnWaitingForInputFinishedEvent());
                
                return;
            }

            if (CurrentPlayedChip.IsAtHome)
                CurrentPlayedChip.MoveFromHome(StartBlockIndex);
            else
                CurrentPlayedChip.Move(spaces, hitToOtherChip);
        }
        
        // This method is for moving the chip a specific number of spaces.
        public abstract void MoveChip(int spaces, bool hasSixRoll);

        // This method is called when the move is finished.
        public void MoveIsFinished()
        {
            foreach (Chip chip in Chips)
            {
                chip.IsPlayable = false;
            }
            EventManager.Broadcast(new OnPlayerSetPlayableChipsEvent(PlayerIndex,false));
        }

        // This method checks if the player has a playable chip.
        public bool HasPlayableChip()
        {
            bool playerHasSix = DiceRollResult1 == 6 || DiceRollResult2 == 6;

            bool condition = Chips.Any(chip => chip.IsAtHome == false && !chip.HasReachedEnd);
            
            return playerHasSix?condition || Chips.Any(x => x.IsAtHome): condition;
        }

        // This method sets a chip as playable.
        public void SetPlayableChip()
        {
            List<Chip> playableChipViews = Chips
                .Where(x => !x.IsAtHome && !x.HasReachedEnd).ToList();
            
            if (DiceRollResult1 == 6 || DiceRollResult2 == 6)
            {
                playableChipViews.AddRange(Chips.Where(x => x.IsAtHome && !x.HasReachedEnd).ToList());
            }

            foreach (Chip playableChipView in playableChipViews)
            {
                playableChipView.IsPlayable = true;
            }
            EventManager.Broadcast(new OnPlayerSetPlayableChipsEvent(PlayerIndex,true));
        }
        
        // This method checks if all chips are at the end home.
        public bool AllChipsAtEndHome()
        {
            return Chips.All(chip => chip.HasReachedEnd);
        }

        // This method gets the playable chips.
        public List<Chip> GetPlayableChip(bool homeOnly)
        {
            if (homeOnly)
            {
                Chips.Where(chip => !chip.IsAtHome).ToList();
            }

            return Chips.Where(chip => !chip.HasReachedEnd).ToList();
        }

        // This method checks if the player has chips at home.
        public bool HasChipsAtHome()
        {
            return Chips.Any(chip => chip.IsAtHome);
        }
    }
}