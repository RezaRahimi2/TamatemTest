using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Events;
using GamePlayLogic;
using UnityEngine;

namespace GamePlayView
{
    public class ChipView : MonoBehaviour
    {
        [field: SerializeField] public Color Color { get; private set; }
        [SerializeField] public Renderer m_renderer;
        [field: SerializeField] public TurnSignIndicator TurnSignIndicator { get; private set; }
        [field: SerializeField] public Chip Chip { get; private set; }
        [field: SerializeField] public Rigidbody Rigidbody { get; private set; }
        [field: SerializeField] public BoxCollider BoxCollider { get; private set; }

        private Action m_onHitChip;

        private void Start()
        {
            EventManager.Subscribe<OnHitChipEvent>(OnHitChip);
        }

        private void OnHitChip(OnHitChipEvent onHitChipEvent)
        {
            m_onHitChip = () =>
            {
                PlayerView playerView = GameViewManager.Instance.GetPlayerByIndex(onHitChipEvent.PlayerIndex);
                playerView.ChipViews[onHitChipEvent.HitChipIndex]
                    .OnChipMoved(onHitChipEvent.PlayerIndex, moveToStart: true);
                
                m_onHitChip = null;
            };
        }

        public void Initialize(bool isBot, Chip chip, Color color)
        {
            Chip = chip;
            Chip.CurrentBlock = new Block(-1);
            Chip.CurrentBlock.Occupy(Chip);
            Color = color;
            m_renderer.material.SetColor("_MainColor", Color);
            m_renderer.material.SetColor("_OutlineColor", Color * .5f);
            m_renderer.material.SetFloat("_OutlineWidth", 0);

            if (!isBot)
            {
                Rigidbody = gameObject.AddComponent<Rigidbody>();
                Rigidbody.isKinematic = true;
                BoxCollider = gameObject.AddComponent<BoxCollider>();
                BoxCollider.enabled = false;
            }
        }

        public void OnChipMoved(int playerIndex, bool moveToStart = false, bool moveToEnd = false,
            bool hitToOtherChip = false)
        {
            EventManager.Broadcast(new OnChipAnimationStartedEvent());

            if (!moveToStart)
            {
                // If the total steps a chip has moved is more than 51, move it to the end home
                if (moveToEnd)
                {
                    var endHomes = BoardView.Instance.EndHomeWithColors[GameViewManager.Instance.GetPlayerByIndex(playerIndex).Player.ColorIndex];
                    // Find the end home for this player's color

                    if (endHomes != null)
                    {
                        HomeBlockView homeBlockView =
                            endHomes.HomeBlockViews.Find(x => x.BlockView.Block.Index == Chip.CurrentBlock.Index);

                        int index = 0;
                        int blocksBeforeEndNumber = (GameModel.Instance.BlockNumber - 2) - (Chip.LastPosition - GameViewManager.Instance.GetPlayerByIndex(playerIndex).Player.StartBlockIndex);
                        float _delay = 0;

                        int chipIndex = (Chip.LastPosition + blocksBeforeEndNumber);
                        
                        for (int i = Chip.LastPosition; i <= chipIndex; i++)
                        {
                            var i1 = i;
                            index = i >= GameModel.Instance.BlockNumber
                                ? i % GameModel.Instance.BlockNumber
                                : i;
                            
                            transform.DOMove(BoardView.Instance.BlockViews[index].transform.position, .5f)
                                .SetDelay(_delay++ * .5f)
                                .OnComplete(() =>
                                {
                                    if (i1 == chipIndex)
                                    {
                                        _delay = 0;
                                        int index = endHomes.HomeBlockViews.IndexOf(homeBlockView);
                                        Chip.CurrentBlock = homeBlockView.BlockView.Block;

                                        for (int i = 0; i <= index; i++)
                                        {
                                            var i1 = i;
                                            transform.DOMove(endHomes.HomeBlockViews[i].BlockView.transform.position,
                                                    .5f)
                                                .SetDelay(_delay++ * .5f)
                                                .OnComplete(() =>
                                                {
                                                    if (i1 == index - 1)
                                                    {
                                                        EventManager.Broadcast(new OnChipAnimationFinishedEvent());
                                                    }
                                                });
                                        }
                                    }
                                });
                        }
                        return;
                    }
                }

                Debug.Log("<color=yellow>Chip moved from " + Chip.LastPosition + " to " + Chip.Position + "</color>");

                if (Chip.Position == Chip.LastPosition)
                {
                    transform.DOMove(BoardView.Instance.BlockViews[Chip.LastPosition].transform.position, .5f)
                        .OnComplete(() =>
                        {
                            m_onHitChip?.Invoke();
                            EventManager.Broadcast(new OnChipAnimationFinishedEvent());
                        });
                }

                float delay = 0;

                for (int i = Chip.LastPosition; i <= Chip.Position; i++)
                {
                    var i1 = i;
                    int index = i >= GameModel.Instance.BlockNumber
                        ? i % GameModel.Instance.BlockNumber
                        : i;

                    transform.DOMove(BoardView.Instance.BlockViews[index].transform.position, .5f)
                        .SetDelay(delay++ * .5f)
                        .OnComplete(() =>
                        {
                            if (i1 == Chip.Position - 1)
                            {
                                m_onHitChip?.Invoke();
                                EventManager.Broadcast(new OnChipAnimationFinishedEvent());
                            }
                        });
                }
            }
            else
            {
                BlockWithPlayerColorView startHome = BoardView.Instance.StartHomeWithColorViews
                    .FirstOrDefault(e => e.PlayerColorName == (PlayerColorName)playerIndex);

                if (startHome != null)
                {
                    // Move the chip to the first unoccupied end home
                    HomeBlockView emptyStartHome =
                        startHome.HomeBlockViews.FirstOrDefault(e => !e.BlockView.Block.IsOccupied);

                    transform.DOMove(emptyStartHome.BlockView.transform.position, .5f)
                        .OnComplete(() => { EventManager.Broadcast(new OnChipAnimationFinishedEvent()); });
                }
            }
        }

        public void Highlight(bool highlight)
        {
            // Highlight the chip when it's the player's turn
            // For example, you can change the color of the chip
            //spriteRenderer.color = highlight ? Color.yellow : Color.white;
        }

        public void ShowWinningAnimation()
        {
            // Show a winning animation when the player wins the game
            // For example, you can play an animation or change the color of the chip
            // This is just a placeholder, you'll need to replace it with your actual animation
            //spriteRenderer.color = Color.green;
        }
    }
}