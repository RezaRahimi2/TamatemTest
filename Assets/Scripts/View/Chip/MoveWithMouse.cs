using System;
using Events;
using GamePlayLogic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

namespace GamePlayView
{
    public class MoveWithMouse : MonoBehaviour
    {
        [SerializeField] private LayerMask m_chipLayer;
        
        public void Initialize(LayerMask chipLayer)
        {
            m_chipLayer = chipLayer;
            enabled = false;
        }

        void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 100, m_chipLayer))
                {
                    Debug.DrawRay(ray.origin, ray.direction * 100, Color.red, 100);
                    Debug.DrawLine(ray.origin, hit.point, Color.yellow, 100);
                    // Perform actions on the selectedObject
                    EventManager.Broadcast(new OnChipTapEvent() { ChipGameObject = hit.transform.gameObject });
                }
            }
        }
    }
}