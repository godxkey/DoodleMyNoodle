using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngineX;

[ExecuteAlways]
[RequireComponent(typeof(RectTransform))]
public class GameConsoleGUILineDetailLayout : UIBehaviour, ILayoutSelfController
{
    [SerializeField] private float _maxHeight = 20f;
    [SerializeField] private Graphic _textContinuesTooltip = null;
    [SerializeField] private float _normalTextContinuesTooltipAlpha = 0.5f;

    private DrivenRectTransformTracker _tracker;
    private RectTransform _rect;

    private RectTransform RectTransform
    {
        get
        {
            if (_rect == null)
                _rect = GetComponent<RectTransform>();
            return _rect;
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        SetDirty();
    }

    protected override void OnDisable()
    {
        _tracker.Clear();
        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
        base.OnDisable();
    }

    protected override void OnRectTransformDimensionsChange()
    {
        SetDirty();
    }

    public void SetLayoutHorizontal() { }

    public void SetLayoutVertical()
    {
        _tracker.Clear();
        _tracker.Add(this, RectTransform, DrivenTransformProperties.SizeDeltaY);
        _tracker.Add(this, RectTransform, DrivenTransformProperties.Anchors);

        float remainingVerticalSpace = ((RectTransform)RectTransform.parent).rect.height - RectTransform.anchoredPosition.y;

        float height = Mathf.Min(LayoutUtility.GetPreferredSize(RectTransform, 1), _maxHeight, remainingVerticalSpace);

        RectTransform.anchorMin = Vector2.zero;
        RectTransform.anchorMax = new Vector2(1, 0);
        RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, height);

        DisplayTextContinueTooltip(_maxHeight == height);
    }

    private void DisplayTextContinueTooltip(bool displayed)
    {
        var c = _textContinuesTooltip.color;
        c.a = (displayed ? _normalTextContinuesTooltipAlpha : 0);
        _textContinuesTooltip.color = c;
    }

    protected void SetDirty()
    {
        if (!IsActive())
            return;

        LayoutRebuilder.MarkLayoutForRebuild(RectTransform);
    }

#if UNITY_EDITOR
    protected override void OnValidate()
    {
        SetDirty();
    }

#endif
}