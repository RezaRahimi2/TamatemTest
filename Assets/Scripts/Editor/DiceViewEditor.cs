using DG.Tweening;
using Events;
using GamePlayView;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(DiceView))]
    public class DiceViewEditor : UnityEditor.Editor
    {
        private DiceView m_diceView;
        private int currentDiceNumber;

        public override VisualElement CreateInspectorGUI()
        {
            m_diceView = target as DiceView;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Roll Dice"))
            {
                if (currentDiceNumber == m_diceView.diceRotations.Count)
                {
                    currentDiceNumber = 0;
                }

                Debug.Log(currentDiceNumber);

                if(EditorApplication.isPlaying)
                    m_diceView.RollDice(currentDiceNumber++,0,null);
                else
                    m_diceView.transform.localEulerAngles = m_diceView.diceRotations[currentDiceNumber++].Rotation;
                
            }
        }
    }
}