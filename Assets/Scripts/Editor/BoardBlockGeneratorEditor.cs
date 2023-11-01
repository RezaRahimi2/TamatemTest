using System.Collections.Generic;
using System.IO;
using System.Linq;
using GamePlayLogic;
using GamePlayView;
using Tools;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Editor
{
    [CustomEditor(typeof(BoardBlockGenerator))]
    public class BoardBlockGeneratorEditor : UnityEditor.Editor
    {
        private BoardBlockGenerator m_boardBlockGenerator;
        private static GameObject m_blockInstance;

        public override VisualElement CreateInspectorGUI()
        {
            m_boardBlockGenerator = target as BoardBlockGenerator;
            return base.CreateInspectorGUI();
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            if (GUILayout.Button("Add Block Up"))
            {
                var lastChildIndex = LastChildIndex();
                GameObject gameObject = new GameObject($"Block{lastChildIndex + 2}");
                gameObject.transform.SetParent(m_boardBlockGenerator.BlocksParent);
                gameObject.transform.localPosition =
                    m_boardBlockGenerator.BlocksParent.GetChild(lastChildIndex).localPosition
                    + new Vector3(0, 0,
                        m_boardBlockGenerator.BlockCollider.size.z + m_boardBlockGenerator.GapBetweenBlocks);

                gameObject.AddComponent<BlockView>();
                
                if (m_blockInstance == null)
                    m_blockInstance = FindObjectOfType<ChipView>().gameObject;

                m_blockInstance.transform.SetParent(gameObject.transform, false);
                m_blockInstance.transform.localPosition = Vector3.zero;
            }

            if (GUILayout.Button("Add Block Left"))
            {
                int lastChildIndex = LastChildIndex();
                GameObject gameObject = new GameObject($"Block{lastChildIndex + 2}");
                gameObject.transform.SetParent(m_boardBlockGenerator.BlocksParent);
                gameObject.transform.localPosition =
                    m_boardBlockGenerator.BlocksParent.GetChild(lastChildIndex).localPosition
                    - new Vector3(m_boardBlockGenerator.BlockCollider.size.x + m_boardBlockGenerator.GapBetweenBlocks,
                        0, 0);

                gameObject.AddComponent<BlockView>();
                
                if (m_blockInstance == null)
                    m_blockInstance = FindObjectOfType<ChipView>().gameObject;

                m_blockInstance.transform.SetParent(gameObject.transform, false);
                m_blockInstance.transform.localPosition = Vector3.zero;
            }

            if (GUILayout.Button("Add Block Right"))
            {
                int lastChildIndex = LastChildIndex();
                GameObject gameObject = new GameObject($"Block{lastChildIndex + 2}");
                gameObject.transform.SetParent(m_boardBlockGenerator.BlocksParent);
                gameObject.transform.localPosition =
                    m_boardBlockGenerator.BlocksParent.GetChild(lastChildIndex).localPosition
                    + new Vector3(m_boardBlockGenerator.BlockCollider.size.x + m_boardBlockGenerator.GapBetweenBlocks,
                        0, 0);

                gameObject.AddComponent<BlockView>();
                
                if (m_blockInstance == null)
                    m_blockInstance = FindObjectOfType<ChipView>().gameObject;

                m_blockInstance.transform.SetParent(gameObject.transform, false);
                m_blockInstance.transform.localPosition = Vector3.zero;
            }

            if (GUILayout.Button("Add Block Down"))
            {
                int lastChildIndex = LastChildIndex();
                GameObject gameObject = new GameObject($"Block{lastChildIndex + 2}");
                gameObject.transform.SetParent(m_boardBlockGenerator.BlocksParent);
                gameObject.transform.localPosition =
                    m_boardBlockGenerator.BlocksParent.GetChild(lastChildIndex).localPosition
                    - new Vector3(0, 0,
                        m_boardBlockGenerator.BlockCollider.size.z + m_boardBlockGenerator.GapBetweenBlocks);

                gameObject.AddComponent<BlockView>();
                
                if (m_blockInstance == null)
                    m_blockInstance = FindObjectOfType<ChipView>().gameObject;

                m_blockInstance.transform.SetParent(gameObject.transform, false);
                m_blockInstance.transform.localPosition = Vector3.zero;
            }

            if (GUILayout.Button("Rename Blocks"))
            {
                for (var i = 0; i < m_boardBlockGenerator.BlocksParent.childCount; i++)
                {
                    m_boardBlockGenerator.BlocksParent.GetChild(i).name = $"Block{i}";
                }
            }

            if (GUILayout.Button("Sort BlockWithPlayerColor based on GameModel.ColorNameWithIndex Order"))
            {
                var startHomeWithColors = FindObjectOfType<BoardView>().StartHomeWithColorViews;
                var endHomeWithColors = FindObjectOfType<BoardView>().EndHomeWithColors;

                FindObjectOfType<BoardView>()
                    .StartHomeWithColorViews = startHomeWithColors.OrderBy(x =>
                        GameModel.Instance.ColorNameOrder.IndexOf(x.PlayerColorName)).ToList();
                
                FindObjectOfType<BoardView>()
                    .EndHomeWithColors = endHomeWithColors.OrderBy(x =>
                    GameModel.Instance.ColorNameOrder.IndexOf(x.PlayerColorName)).ToList();

            }

        }

        private int LastChildIndex()
        {
            int lastChildIndex = m_boardBlockGenerator.BlocksParent.childCount - 1;
            return lastChildIndex;
        }
    }
}