using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using Events;
using GamePlayLogic;
using GamePlayView;
using UnityEngine;
using UnityEngine.Serialization;

namespace GamePlayView
{
    public class GameViewManager : Singleton<GameViewManager>
    {
        [SerializeField] private BoardView m_boardView;
        [SerializeField] private HighlightManager m_highlightManager;
        [SerializeField] private List<PlayerView> m_playerViews;
        [SerializeField] private List<PlayerPositionWithColor> playersPositionParent;
        [SerializeField] private PlayerView m_currentPlayerView;
        [SerializeField] private Dictionary<PlayerView, MoveWithMouse> m_playerWithMoveWithMouseDictionary;
        [SerializeField] private LayerMask m_chipLayer;

        private void Awake()
        {
            if (m_playerViews != null && m_playerViews.Count > 0)
            {
                foreach (PlayerView playerViewsValue in m_playerViews)
                {
                    Destroy(playerViewsValue.gameObject);
                }
            }

            EventManager.Subscribe<OnStartGameEvent>(OnStartGame);
            EventManager.Subscribe<OnSetPlayerTurnEvent>(OnSetPlayerTurn);
            EventManager.Subscribe<OnPlayerSetPlayableChipsEvent>(OnPlayerSetPlayableChips);
            EventManager.Subscribe<OnChipTapEvent>(OnChipTap);
            EventManager.Subscribe<OnChipMovedEvent>(OnChipMoved);
            
        }
        
        public PlayerView GetPlayerByIndex(int index)
        {
            return m_playerViews[index];
        }

        private void OnStartGame(OnStartGameEvent onStartGame)
        {
            m_boardView.Initialize(onStartGame.Board);
            m_playerViews = new List<PlayerView>();
            m_playerWithMoveWithMouseDictionary = new Dictionary<PlayerView, MoveWithMouse>();
            for (var i = 0; i < onStartGame.Players.Count; i++)
            {
                PlayerView playerView = null;

#if Debugging
                if (GameModel.Instance.PlayerNumber == 1 && GameModel.Instance.UsePlayerIndexForPosition)
                {
                    playerView = Instantiate<PlayerView>(PrefabsManager.Instance.PlayerView,
                    playersPositionParent[GameModel.Instance.PlayerPositionIndex].PositionTransform);
                    playerView.Initialize(onStartGame.Players[i], playersPositionParent[GameModel.Instance.PlayerPositionIndex].Color);       
                }
                else
                {
                    playerView = Instantiate<PlayerView>(PrefabsManager.Instance.PlayerView,
                        playersPositionParent[i].PositionTransform);
                    playerView.Initialize(onStartGame.Players[i], playersPositionParent[i].Color);    
                }
#else
                playerView = Instantiate<PlayerView>(PrefabsManager.Instance.PlayerView,
                    playersPositionParent[i].PositionTransform);
                playerView.Initialize(onStartGame.Players[i], playersPositionParent[i].Color);
#endif

                if (!onStartGame.Players[i].IsBot)
                {
                    MoveWithMouse moveWithMouse = playerView.gameObject.AddComponent<MoveWithMouse>();
                    moveWithMouse.Initialize(m_chipLayer);
                    moveWithMouse.enabled = false;
                    m_playerWithMoveWithMouseDictionary.Add(playerView, moveWithMouse);
                }

                m_playerViews.Add(playerView);
            }

            m_highlightManager.Initialize(m_playerViews);
        }

        private void OnSetPlayerTurn(OnSetPlayerTurnEvent onSetPlayerTurnEvent)
        {
            m_currentPlayerView = m_playerViews[onSetPlayerTurnEvent.PlayerIndex];
        }

        private void OnPlayerSetPlayableChips(OnPlayerSetPlayableChipsEvent onPlayerSetPlayableChipsEvent)
        {
            if (!m_currentPlayerView.Player.IsBot)
            {
                m_playerWithMoveWithMouseDictionary[m_currentPlayerView].enabled = true;

                List<ChipView> playableChipViews = m_currentPlayerView.ChipViews
                    .Where(x => x.Chip.IsPlayable).ToList();

                for (var i = 0; i < playableChipViews.Count; i++)
                {
                    playableChipViews[i].BoxCollider.enabled = true;
                }

                if (playableChipViews.Count == 0)
                    EventManager.Broadcast(new OnWaitingForInputFinishedEvent());
            }
        }

        private void OnChipTap(OnChipTapEvent onChipTap)
        {
            Player player = (Player)m_currentPlayerView.Player;
            player.CurrentPlayedChip = onChipTap.ChipGameObject.GetComponent<ChipView>().Chip;

            if (!player.IsBot)
            {
                m_currentPlayerView.ChipViews.ForEach(x => { x.BoxCollider.enabled = false; });
            }

            m_playerWithMoveWithMouseDictionary[m_currentPlayerView].enabled = false;

            player.MoveChip();
            player.MoveIsFinished();
            EventManager.Broadcast(new OnWaitingForInputFinishedEvent());
        }

        private void OnChipMoved(OnChipMovedEvent onChipMovedEvent)
        {
            PlayerView playerView = null;
#if Debugging
            if (GameModel.Instance.PlayerNumber == 1 && GameModel.Instance.UsePlayerIndexForPosition)
            {
                playerView =  m_playerViews[0];
            }
            else
            {
                playerView = m_playerViews[onChipMovedEvent.Chip.PlayerIndex];
            }
#else
           playerView = m_playerViews[onChipMovedEvent.Chip.PlayerIndex];
#endif
            playerView.OnChipMoved(onChipMovedEvent);
        }

        private void OnGameWinner(OnWinnerEvent onWinnerEvent)
        {
        }
    }
}