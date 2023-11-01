using System;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

namespace GamePlayView
{
    using UnityEngine;
    using UnityEngine.UI;
    using Cysharp.Threading.Tasks;

    public class LoadingView : MonoBehaviour
    {
        [SerializeField] private CanvasGroup m_canvasGroup;
        [SerializeField] private TextMeshProUGUI m_loadingText;
        [SerializeField] private Button m_loadButton;
        [SerializeField] private Image m_loadingImage;

        public void Initialize(Action onStartAction)
        {
            m_loadButton.onClick.AddListener(() => onStartAction?.Invoke());
            m_loadingText.DOFade(0, .5f).SetLoops(-1, LoopType.Yoyo).SetTarget(m_loadingText);
        }

        public void OnStartLoading()
        {
            DOTween.Kill(m_loadingText);
            m_loadingText.DOFade(0, .5f);
        }

        public void OnFinishedLoading(Action onFinishedLoadingCallback)
        {
            m_canvasGroup.DOFade(0, 1).OnComplete(() => { onFinishedLoadingCallback?.Invoke(); });
        }

        public async UniTask SetLoadingProgress(float progress)
        {
            m_loadingImage.fillAmount = progress;
            await UniTask.Yield();
        }
    }
}