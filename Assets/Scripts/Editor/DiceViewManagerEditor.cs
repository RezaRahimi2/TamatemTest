using GamePlayView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(DiceViewManager))]
    public class DiceViewManagerEditor : UnityEditor.Editor
    { 
        private DiceViewManager m_diceViewManager;
        
        public override VisualElement CreateInspectorGUI()
        {
            m_diceViewManager = target as DiceViewManager;
            return base.CreateInspectorGUI();
        }
        
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Roll Specific Dice"))
            {
                m_diceViewManager.RollDiceDebug(m_diceViewManager.m_dice1LastRoll, m_diceViewManager.m_dice2LastRoll);
            }
            
            if (GUILayout.Button("Roll 6 Dices"))
            {
                m_diceViewManager.RollDiceDebug(6,6);
                UIManager.Instance.RollDiceClicked();
            }
        }
    }
}