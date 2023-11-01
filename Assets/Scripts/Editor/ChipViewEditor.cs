using GamePlayLogic;
using GamePlayView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(ChipView))]
    public class ChipViewEditor : UnityEditor.Editor
    {
        private ChipView m_chipView;
        
        private byte m_dice1;
        private byte m_dice2;
        
        public override VisualElement CreateInspectorGUI()
        {
            m_chipView = target as ChipView;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            
            m_dice1 = (byte)EditorGUILayout.IntSlider("Dice 1", m_dice1, 1, 6);
            m_dice2 = (byte)EditorGUILayout.IntSlider("Dice 2", m_dice2, 1, 6);
            
            if (GUILayout.Button("Move to position"))
            {
                int playerIndex = -1;
#if Debugging
                if (GameModel.Instance.PlayerNumber == 1 && GameModel.Instance.UsePlayerIndexForPosition)
                {
                    playerIndex = 0;
                }
                else
                {
                    playerIndex = m_chipView.Chip.PlayerIndex;
                }
#else
               playerIndex = m_chipView.Chip.PlayerIndex;
#endif
                
                PlayerBase player = GameViewManager.Instance.GetPlayerByIndex(playerIndex).Player;

                player.DiceRollResult1 = m_dice1;
                player.DiceRollResult2 = m_dice2;
                m_chipView.Chip.MoveIndex = m_chipView.Chip.Position - player.StartBlockIndex;
                m_chipView.Chip.CurrentBlock = GameController.Instance.Board.Blocks[m_chipView.Chip.LastPosition - player.StartBlockIndex];
                player.CurrentPlayedChip = m_chipView.Chip;
                player.MoveChip();

            }
        }
    }
}