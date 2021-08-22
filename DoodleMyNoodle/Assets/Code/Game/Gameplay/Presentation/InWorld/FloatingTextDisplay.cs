using System;
using UnityEngine;
using UnityEngineX;
using TMPro;
using DG.Tweening;

public class FloatingTextDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshPro _number;

    public float AnimDuration = 2;
    public float DisplacementAnimOffset = 2;

    public void Display(string text, Color color)
    {
        _number.text = text;
        _number.color = color;

        Sequence animation = DOTween.Sequence();
        animation.Append(_number.DOFade(0, AnimDuration));
        animation.Join(transform.DOMoveY(transform.position.y + DisplacementAnimOffset, AnimDuration));
        animation.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }
}
