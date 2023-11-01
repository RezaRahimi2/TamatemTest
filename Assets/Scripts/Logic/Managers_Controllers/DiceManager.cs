using Cysharp.Threading.Tasks;
using Events;
using UnityEngine;

namespace GamePlayLogic
{
    // This class manages the dice rolling functionality in the game
    public class DiceManager : SingletonNonMono<DiceManager>
    {
        // Two dice objects for the game
        [SerializeField] private Dice m_dice1;
        [SerializeField] private Dice m_dice2;
    
        // Constructor for the DiceManager class
        public DiceManager()
        {
            // Initialize two dice objects
            m_dice1 = new Dice();
            m_dice2 = new Dice();
        
            // Subscribe to the OnDiceRollEvent
            EventManager.Subscribe<OnDiceRollEvent>(OnRollDice);
        }

        // Method to handle the dice roll event
        private async void OnRollDice(OnDiceRollEvent onDiceRollEvent)
        {
            // Start the dice roll tasks for both dice
            var diceRollResult1Task = m_dice1.RollDice();
            var diceRollResult2Task = m_dice2.RollDice();
        
            // Wait for both dice roll tasks to complete
            var (dice1, dice2) = await UniTask.WhenAll(diceRollResult1Task, diceRollResult2Task);
        
            // Broadcast the dice roll results
            EventManager.Broadcast(new OnDiceRolledEvent()
            { 
                PlayerIndex = onDiceRollEvent.PlayerIndex,
                Dice1LastRoll = dice1, 
                Dice2LastRoll = dice2
            });

            // Log the dice roll results
            Debug.unityLogger.Log($"Dice Rolled: Dice1: {dice1} | Dice2:{dice2}");
        }
    }

}