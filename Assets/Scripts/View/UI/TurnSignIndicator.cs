using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class TurnSignIndicator : MonoBehaviour
{
    [SerializeField]private Renderer m_renderer;

    public Tween Show()
    {
        float fadeValue = 0;
        var material = m_renderer.material;
        gameObject.SetActive(true);
        return  DOTween.To(() => fadeValue, x => material.SetFloat("_Fade", x),
            1, 0.75f).SetLoops(-1, LoopType.Yoyo);
    }

    public Tween Hide()
    {
        float fadeValue = 0;
        return DOTween.To(() => fadeValue, x => m_renderer.material.SetFloat("_Fade", x),
            0, 0.75f);
    }
}
