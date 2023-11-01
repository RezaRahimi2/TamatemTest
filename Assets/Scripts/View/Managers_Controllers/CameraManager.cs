using System;
using DG.Tweening;
using Events;
using UnityEngine;

namespace DefaultNamespace
{
    public class CameraManager : MonoBehaviour
    {
        [SerializeField]private Transform m_diceCameraTransform;
        private float m_diceCameraStartY;

        private void Awake()
        {
            EventManager.Subscribe<OnDiceRolledEvent>(OnDiceRolled);
        }

        private void OnDiceRolled(OnDiceRolledEvent onDiceRolledEvent)
        {
            m_diceCameraStartY = m_diceCameraTransform.localPosition.y;
            EventManager.Broadcast<OnUIChangingStartedEvent>(new OnUIChangingStartedEvent());
            m_diceCameraTransform.DOLocalMoveY(m_diceCameraStartY + .125f, .5f).SetEase(Ease.OutBounce)
                .OnComplete(() =>
                {
                    m_diceCameraTransform.DOLocalMoveY(m_diceCameraStartY, .5f).SetEase(Ease.InBounce)
                        .OnComplete(() =>
                        {
                            EventManager.Broadcast<OnUIChangingEndEvent>(new OnUIChangingEndEvent());
                        });
                });
        }
    }
}