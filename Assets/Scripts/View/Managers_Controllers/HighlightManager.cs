using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Events;
using GamePlayLogic;
using GamePlayView;
using UnityEngine;

public class HighlightManager : MonoBehaviour
{
    [SerializeField] private List<PlayerView> m_playerViews;
    [SerializeField] private PlayerView m_lastPlayerView;
    private List<Tween> m_lastTween;

    private void Start()
    {
        EventManager.Subscribe<OnPlayerSetPlayableChipsEvent>(OnPlayerSetPlayableChips);
    }

    public void Initialize(List<PlayerView> playerViews)
    {
        m_playerViews = playerViews;
    }

    private void OnPlayerSetPlayableChips(OnPlayerSetPlayableChipsEvent onPlayerSetPlayableChipsEvent)
    {
        PlayerView playerView = m_playerViews.Find(x => x.Player.PlayerIndex == onPlayerSetPlayableChipsEvent.PlayerIndex);

        Hide();

        m_lastTween = new List<Tween>();
        playerView.ChipViews.Where(x=>x.Chip.IsPlayable == onPlayerSetPlayableChipsEvent.ToPlay).ToList().ForEach(chip =>
        {
            float outlineWidth = 0;
            m_lastTween.Add(onPlayerSetPlayableChipsEvent.ToPlay? chip.TurnSignIndicator.Show(): chip.TurnSignIndicator.Hide());
            DOTween.To(() => outlineWidth, x => chip.m_renderer.material.SetFloat("_OutlineWidth", x),
                onPlayerSetPlayableChipsEvent.ToPlay? .004f:0, 0.75f).SetLoops(-1, LoopType.Yoyo).SetTarget(transform);
        });

        m_lastPlayerView = playerView;
    }

    private void Hide()
    {
        if (m_lastTween != null && m_lastTween.Count > 0)
        {
            m_lastTween.ForEach(x => x.Kill());
        }

        if (DOTween.IsTweening(transform))
        {
            DOTween.Kill(transform);
            m_lastPlayerView?.ChipViews.ForEach(chip =>
            {
                chip.m_renderer.material.SetFloat("_OutlineWidth", 0);
                chip.TurnSignIndicator.Hide();
            });
        }
    }
}