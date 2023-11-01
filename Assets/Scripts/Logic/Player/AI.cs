using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using Events;
using GamePlayLogic;
using UnityEngine;

namespace GamePlayLogic
{
    public class AI : PlayerBase
    {
        public AI(int colorIndex,bool isBot, int numberOfChips, int playerIndex, Board board, int startBlockIndex) : base(colorIndex,isBot, numberOfChips, playerIndex, board, startBlockIndex)
        {
        }
        
        public override async UniTask RollDies()
        {
            DiceRollResult1 = 0;
            DiceRollResult2 = 0;
            EventManager.Subscribe<OnDiceRolledEvent>(OnDiceRolled);
            EventManager.Broadcast(new OnDiceRollEvent() { PlayerIndex = PlayerIndex });
            
            await UniTask.WaitUntil(m_apiCallback.Invoke);
        }

        public void OnDiceRolled(OnDiceRolledEvent onDiceRolledEvent)
        {
            DiceRollResult1 = onDiceRolledEvent.Dice1LastRoll;
            DiceRollResult2 = onDiceRolledEvent.Dice2LastRoll;
            
            EventManager.Unsubscribe<OnDiceRolledEvent>(OnDiceRolled);
        }
        
        public override void MoveChip(int spaces,bool hasSixRoll)
        {
            CurrentPlayedChip = Chips.FirstOrDefault(chip => chip.IsPlayable);
            base.MoveChip();
            MoveIsFinished();
        }
    }
}