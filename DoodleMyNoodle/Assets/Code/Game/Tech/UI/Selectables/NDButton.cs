using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonData : IWidgetDataButton
{
    public bool Interactable;

    public KeyCode KeyboardShortcut; // todo: replace that with abstract 'action' (e.g. RewiredAction)
    public Action<Widget> Clicked;
    public Action<Widget> Pressed;
    public Action<Widget> Released;
    public Action<Widget> PointerEntered;
    public Action<Widget> PointerExited;
    public Action<Widget> Selecteded;

    ButtonData IWidgetDataButton.ButtonData => this;
}

public interface IWidgetDataButton
{
    ButtonData ButtonData { get; }
}

public enum SelectableState
{
    /// <summary>
    /// The UI object can be selected.
    /// </summary>
    Normal,

    /// <summary>
    /// The UI object is highlighted.
    /// </summary>
    Highlighted,

    /// <summary>
    /// The UI object is pressed.
    /// </summary>
    Pressed,

    /// <summary>
    /// The UI object is selected
    /// </summary>
    Selected,

    /// <summary>
    /// The UI object cannot be selected.
    /// </summary>
    Disabled,
}

[RequireComponent(typeof(Widget))]
public class NDButton : Button, IWidgetComponent
{
    private ButtonData _buttonData;
    private Widget _widget;
    private SelectableState _state;

    private Widget Widget => _widget ??= GetComponent<Widget>();

    public override void OnPointerUp(PointerEventData eventData)
    {
        _buttonData?.Released?.Invoke(Widget);
        base.OnPointerUp(eventData);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        _buttonData?.Pressed?.Invoke(Widget);
        base.OnPointerDown(eventData);
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _buttonData?.PointerEntered?.Invoke(Widget);
        base.OnPointerEnter(eventData);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        _buttonData?.PointerExited?.Invoke(Widget);
        base.OnPointerExit(eventData);
    }

    public override void OnPointerClick(PointerEventData eventData)
    {
        _buttonData?.Clicked?.Invoke(Widget);
        base.OnPointerClick(eventData);
    }

    public override void OnSubmit(BaseEventData eventData)
    {
        _buttonData?.Clicked?.Invoke(Widget);
        base.OnSubmit(eventData);
    }

    public override void OnSelect(BaseEventData eventData)
    {
        bool supportControllerSelection = false;
        if (!supportControllerSelection)
        {
            return;
        }

        _buttonData?.Selecteded?.Invoke(Widget);
        base.OnSelect(eventData);
    }

    public void SetWidgetData(object widgetData)
    {
        if (!(widgetData is IWidgetDataButton widgetDataButton))
            return;

        _buttonData = widgetDataButton.ButtonData;

        interactable = _buttonData.Interactable;
    }

    protected override void InstantClearState()
    {
        _state = SelectableState.Normal;
        base.InstantClearState();
    }

    protected override void DoStateTransition(SelectionState state, bool instant)
    {
        _state = (SelectableState)state;
        base.DoStateTransition(state, instant);
    }

    private void Update()
    {
        if (_buttonData != null && _buttonData.KeyboardShortcut != KeyCode.None && IsInteractable())
        {
            if (Input.GetKeyDown(_buttonData.KeyboardShortcut))
            {
                ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerDownHandler);
            }

            if (Input.GetKeyUp(_buttonData.KeyboardShortcut) && _state == SelectableState.Pressed)
            {
                ExecuteEvents.Execute(gameObject, new BaseEventData(EventSystem.current), ExecuteEvents.submitHandler);
                ExecuteEvents.Execute(gameObject, new PointerEventData(EventSystem.current), ExecuteEvents.pointerUpHandler);
            }
        }
    }
}
