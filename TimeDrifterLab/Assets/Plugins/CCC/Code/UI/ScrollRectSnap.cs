using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

[RequireComponent(typeof(ScrollRect))]
public class ScrollRectSnap : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public enum Direction { Vertical, Horizontal }
    public Direction SnapDirection = Direction.Vertical;
    public RectTransform MovingRect;

    public bool useCustomPositionFunction = false;

    [Header("Standard Position Formula")]
    public float baseSnapDelta = 0;
    public float snapInterval = 100;

    [Header("Acceleration"), Forward]
    public GuidedAccelerationHandler accelerationHandler = new GuidedAccelerationHandler(1000);

    /// <summary>
    /// Doit retourner la anchoredPosition.y cible
    /// </summary>
    public Func<float> customSnapFunction;

    private bool pointerDown = false;

    private ScrollRect ScrollRect
    {
        get { return _scrollRect ?? (_scrollRect = GetComponent<ScrollRect>()); }
    }
    private ScrollRect _scrollRect;

    void Update()
    {
        if (MovingRect != null)
        {
            var targetAnchoredPos = 0f;
            var currentPos = GetCurrentPos();
            if (useCustomPositionFunction && customSnapFunction != null)
            {
                targetAnchoredPos = customSnapFunction();
            }
            else
            {
                targetAnchoredPos = (currentPos - baseSnapDelta).RoundedTo(snapInterval) + baseSnapDelta;
            }

            var deltaTime = Time.deltaTime;
            accelerationHandler.UpdateAcceleration(targetAnchoredPos, currentPos, GetVelocity(), deltaTime);

            if (pointerDown == false)
            {
                ScrollRect.velocity += GetPositiveVelocity() * accelerationHandler.CurrentAcceleration * deltaTime;
            }
        }
    }

    float GetCurrentPos()
    {
        if (SnapDirection == Direction.Vertical)
            return MovingRect.anchoredPosition.y;
        else
            return MovingRect.anchoredPosition.x;
    }

    float GetVelocity()
    {
        if (SnapDirection == Direction.Vertical)
            return ScrollRect.velocity.y;
        else
            return ScrollRect.velocity.x;
    }

    Vector2 GetPositiveVelocity()
    {
        if (SnapDirection == Direction.Vertical)
            return Vector2.up;
        else
            return Vector2.right;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        pointerDown = false;
    }
}
