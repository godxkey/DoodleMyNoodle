using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// Control animation and automation ScrollRect position
/// </summary>
[RequireComponent(typeof(ScrollRect))]
public class ScrollRectAnimator : MonoBehaviour, IBeginDragHandler
{
    [SerializeField, ReadOnly]
    bool isAnimating = false;
    public bool IsAnimating { get { return isAnimating; } }

    [Tooltip("User will cancel active tweens when scrolling")]
    public bool userCancelTween = true;

    protected ScrollRect scrollRect;
    protected RectTransform container;

    protected Tweener tween;

    protected Vector2 targetAnchoredPos;
    protected float totalVerticalSize;

    protected virtual void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
        if (container == null)
            container = scrollRect.content;

        Vector3[] localCorners = new Vector3[4];
        scrollRect.GetComponent<RectTransform>().GetLocalCorners(localCorners);

        totalVerticalSize = Mathf.Abs(localCorners[0].y - localCorners[2].y);
    }

    public virtual void OnBeginDrag(PointerEventData eventData)
    {
        // User drag will cancel tween
        if (userCancelTween)
            StopCurrentAnimation();
    }

    public virtual void StopCurrentAnimation()
    {
        ClearTween();
        isAnimating = false;
    }

    public virtual void ScrollTo(RectTransform target, float duration = 0.6f, Ease ease = Ease.InOutQuint)
    {
        ScrollTo(target.position, duration, ease);
    }

    public virtual void ScrollTo(Vector3 worldPosition, float duration = 0.6f, Ease ease = Ease.InOutQuint)
    {
        isAnimating = true;
        ClearTween();

        // Stop scroll velocity
        scrollRect.velocity = Vector2.zero;

        // Animate to new position
        targetAnchoredPos = GetRelativeLocalPos(worldPosition);
        tween = container.DOAnchorPosY(targetAnchoredPos.y, duration).SetEase(ease).OnComplete(OnTweenComplete);
    }

    public void InstantScrollTo(RectTransform target)
    {
        InstantScrollTo(target.position);
    }

    public void InstantScrollTo(Vector3 worldPosition)
    {
        ClearTween();

        // Stop scroll velocity
        scrollRect.velocity = Vector2.zero;

        // Animate to new position
        targetAnchoredPos = GetRelativeLocalPos(worldPosition);
        Clamp();
    }

    protected virtual void ClearTween()
    {
        if (tween != null)
        {
            tween.Kill();
            tween = null;
        }
    }

    protected virtual void OnTweenComplete()
    {
        Clamp();
        tween = null;
        isAnimating = false;
    }

    protected virtual void Clamp()
    {
        // Clamp container to exact target position
        Canvas.ForceUpdateCanvases();
        container.anchoredPosition = targetAnchoredPos;
    }

    public Vector2 GetRelativeLocalPos(RectTransform target)
    {
        return GetRelativeLocalPos(target.position);
    }

    public Vector2 GetRelativeLocalPos(Vector3 worldPosition)
    {
        float yDif = (container.anchoredPosition.y - (scrollRect.transform.InverseTransformPoint(worldPosition)).y);

        float max = container.sizeDelta.y - totalVerticalSize;
        if (max < 0)
            max = 0;
        yDif = Mathf.Clamp(yDif, 0, max);

        return new Vector2(container.anchoredPosition.x, yDif);
    }
}
