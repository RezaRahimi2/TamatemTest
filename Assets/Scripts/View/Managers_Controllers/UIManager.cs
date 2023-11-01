using System;
using DG.Tweening;
using Events;
using GamePlayLogic;
using UnityEngine;
using UnityEngine.UI;

namespace GamePlayView
{
    public class UIManager : Singleton<UIManager>
    {
        [SerializeField] private Image m_playerColorShownImage;
        [SerializeField] private Button m_rollButton;

        private PlayerBase m_currentPlayer;

        private void Awake()
        {
            m_rollButton.onClick.AddListener(RollDiceClicked);
            EventManager.Subscribe<OnSetPlayerTurnEvent>(OnSetPlayerTurn);
            EventManager.Subscribe<OnStartDiceRollEvent>(OnStartDiceRoll);
            EventManager.Subscribe<OnBlockChipEvent>(OnBlockChip);
        }

        private void OnStartDiceRoll(OnStartDiceRollEvent startDiceRollEvent)
        {
            m_rollButton.enabled = true;
            m_rollButton.image.DOColor(Color.green, .5f).SetLoops(-1, LoopType.Yoyo).SetTarget(m_rollButton);
        }

        private void OnSetPlayerTurn(OnSetPlayerTurnEvent onSetPlayerTurnEvent)
        {
            PlayerView playerView = GameViewManager.Instance.GetPlayerByIndex(onSetPlayerTurnEvent.PlayerIndex);
            m_playerColorShownImage.DOColor(playerView.PlayerColor, .5f);
            m_currentPlayer = playerView.Player;
        }

        private void OnBlockChip(OnBlockChipEvent onBlockChipEvent)
        {
            Debug.Log($"OnBlockChipEvent: {onBlockChipEvent.Chip} Block: {onBlockChipEvent.BlockChip}");
        }

        public void RollDiceClicked()
        {
            DOTween.Kill(m_rollButton);
            m_rollButton.image.DOColor(Color.gray, .5f);
            m_rollButton.enabled = false;
            EventManager.Broadcast(new OnDiceRollEvent(){ PlayerIndex = m_currentPlayer.PlayerIndex });
        }
    }
}