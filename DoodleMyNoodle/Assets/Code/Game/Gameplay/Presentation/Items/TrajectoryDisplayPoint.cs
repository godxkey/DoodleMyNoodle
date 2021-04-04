using System;
using UnityEngine;
using UnityEngineX;

public class TrajectoryDisplayPoint : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteMain;
    [SerializeField] private SpriteRenderer _spriteShadow;

    public void SetAlpha(float alpha)
    {
        _spriteMain.SetAlpha(alpha);
        _spriteShadow.SetAlpha(alpha * alpha);
    }
}