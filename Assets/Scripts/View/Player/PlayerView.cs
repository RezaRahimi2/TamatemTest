
using System.Collections.Generic;
using System.Linq;
using Events;
using GamePlayLogic;
using UnityEngine;

namespace GamePlayView
{
    public class PlayerView : MonoBehaviour
    {
        [field: SerializeField]public Color PlayerColor{ get; private set; }
        [field: SerializeField]public PlayerBase Player { get; private set; }
        [field: SerializeField] public List<ChipView> ChipViews { get; private set; }
        [SerializeField] private List<Transform> chipTransforms;
        [field: SerializeField]public List<BoxCollider> BoxColliders { get; private set; }

        public void Initialize(PlayerBase player,Color playerColor)
        {
            Player = player;
            PlayerColor = playerColor;
            ChipViews = new List<ChipView>();

            for (var i = player.Chips.Length - 1; i >= 0; i--)
            {
                ChipView chipView = Instantiate(PrefabsManager.Instance.ChipViewPrefab, chipTransforms[i].position,Quaternion.identity,chipTransforms[i]);
                chipView.Initialize(player.IsBot, player.Chips[i], playerColor);
                
                if(player.IsBot)
                    BoxColliders.Add(chipView.BoxCollider);
                
                ChipViews.Add(chipView);   
            }
        }

        
        
        public void OnChipMoved(OnChipMovedEvent chipMovedEvent)
        {
            ChipView chipView = ChipViews.Find(c => c.Chip == chipMovedEvent.Chip);
            if (chipView != null)
            {
                chipView.OnChipMoved(Player.PlayerIndex,chipMovedEvent.MoveToStartHome,chipMovedEvent.MoveToEndHome);
            }
        }
        
        public void OnGameWinner()
        {
            // Display the winner of the game
            // For example, you can show a winning animation or change the color of the player's chips
            foreach (var chipView in ChipViews)
            {
                chipView.ShowWinningAnimation();
            }
        }
    }
}