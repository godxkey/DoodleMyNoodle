using System;
using UnityEngine;
using UnityEngineX;
using DG.Tweening;

public class SpriteMaskAnimation : GamePresentationBehaviour
{
    public float MoveToDestinationY = 1;
    public float MoveToDestinationX = 1;
    public float AnimationDuration = 1;
    public GameObject SpriteMaskToMove;

    private void Start()
    {
        if (SpriteMaskToMove != null)
        {
            SpriteMaskToMove.SetActive(true);

            Sequence sq = DOTween.Sequence();
            sq.Append(SpriteMaskToMove.transform.DOLocalMoveX(MoveToDestinationX, AnimationDuration));
            sq.Join(SpriteMaskToMove.transform.DOLocalMoveY(MoveToDestinationY, AnimationDuration));
        }
    }
}