using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Events;
using UnityEngine;

namespace GamePlayView
{
    public class DiceViewManager : MonoBehaviour
    {
        #if UNITY_EDITOR
            public int m_dice1LastRoll;
            public int m_dice2LastRoll;
        #endif
        
        public List<DiceView> DiceViews;

        public void Awake()
        {
            EventManager.Subscribe<OnDiceRolledEvent>(OnDiceRolled);
        }

        private void OnDiceRolled(OnDiceRolledEvent onDiceRolledEvent)
        {
            OnDiceRolled(onDiceRolledEvent.Dice2LastRoll, onDiceRolledEvent.Dice1LastRoll);
        }

        private async UniTask OnDiceRolled(int dice1LastRoll, int dice2LastRoll)
        {
            EventManager.Broadcast(new OnDiceAnimationStartedEvent());
            List<bool> diceAnimationCounter = new List<bool>();

            DiceViews.ForEach(x =>
            {   
                diceAnimationCounter.Add(false);
            });
            
            void FinishCallback(int diceIndex)
            {
                diceAnimationCounter[diceIndex] = true;
            }

            DiceViews[0].RollDice(dice1LastRoll,0, FinishCallback);
            DiceViews[1].RollDice(dice2LastRoll,1, FinishCallback);
            
            await UniTask.WaitUntil(()=> diceAnimationCounter.All(x=>x));
            
            EventManager.Broadcast(new OnDiceAnimationFinishedEvent());
        }
        
#if UNITY_EDITOR
        public void RollDiceDebug(int dice1LastRoll, int dice2LastRoll)
        {
            OnDiceRolled(dice1LastRoll, dice2LastRoll);
        }
#endif
        
    }
}