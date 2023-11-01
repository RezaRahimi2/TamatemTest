using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Events;
using GamePlayView;
using UnityEngine;

namespace GamePlayLogic
{
    [Serializable]
    public class Player : PlayerBase
    {
        public Player(int colorIndex,bool isBot, int numberOfChips, int playerIndex, Board board, int startBlockIndex) : base(colorIndex,isBot, numberOfChips, playerIndex, board, startBlockIndex)
        {
        }

        public override async UniTask RollDies()
        {
            DiceRollResult1 = 0;
            DiceRollResult2 = 0;
            EventManager.Subscribe<OnDiceRolledEvent>(OnDiceRolled);
            EventManager.Broadcast(new OnWaitingForInputStartedEvent());
            EventManager.Broadcast(new OnStartDiceRollEvent());
            
            await UniTask.WaitUntil(m_apiCallback.Invoke);
        }

        public void OnDiceRolled(OnDiceRolledEvent onDiceRolledEvent)
        {
            DiceRollResult1 = onDiceRolledEvent.Dice1LastRoll;
            DiceRollResult2 = onDiceRolledEvent.Dice2LastRoll;
            
            EventManager.Broadcast(new OnWaitingForInputFinishedEvent());
            EventManager.Unsubscribe<OnDiceRolledEvent>(OnDiceRolled);
        }
        
        public bool HasReachedEnd(Chip chip)
        {
            // Check if the chip's current block index is in the end home
            IsChipInEndHome = Board.EndHomeWithColors.Any(endHomeWithColor =>
                endHomeWithColor.BlockHomes.Any(endHome => endHome.Index == chip.CurrentBlock.Index));

            return IsChipInEndHome;
        }
         
        public override void MoveChip(int spaces,bool hasSixRoll)
        {
            EventManager.Broadcast(new OnWaitingForInputStartedEvent());
        }
    }
}