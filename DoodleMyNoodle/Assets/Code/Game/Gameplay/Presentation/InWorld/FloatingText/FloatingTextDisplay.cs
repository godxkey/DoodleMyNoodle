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
    private Sequence _animation;

    public void Display(string text, Color color)
    {
        _number.text = text;
        _number.color = color;

        _animation?.Kill();
        _animation = DOTween.Sequence();
        _animation.Append(_number.DOFade(0, AnimDuration));
        _animation.Join(transform.DOMoveY(transform.position.y + DisplacementAnimOffset, AnimDuration));
        _animation.OnComplete(() =>
        {
            Destroy(gameObject);
        });
    }

    private void OnDisable()
    {
        _animation?.Kill();
    }
}
