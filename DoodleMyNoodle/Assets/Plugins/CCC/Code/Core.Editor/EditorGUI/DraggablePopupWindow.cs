using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public abstract class DraggablePopupWindow : EditorWindow
{
    private Vector2 _offset;

    public static T GetDraggableWindow<T>() where T : DraggablePopupWindow
    {
        var array = Resources.FindObjectsOfTypeAll(typeof(T)) as T[];
        var t = (array.Length <= 0) ? null : array[0];

        return t ?? CreateInstance<T>();
    }

    public void ShowAt(in Rect rect, bool focus = true)
    {
        minSize = rect.size;
        position = rect;

        ShowPopup();
        if (focus) Focus();
    }

    protected virtual void OnGUI()
    {
        var e = Event.current;

        // calculate offset for the mouse cursor when start dragging
        if (e.button == 0 && e.type == EventType.MouseDown)
        {
            _offset = position.position - GUIUtility.GUIToScreenPoint(e.mousePosition);
        }

        // drag window
        if (e.button == 0 && e.type == EventType.MouseDrag)
        {
            var mousePos = GUIUtility.GUIToScreenPoint(e.mousePosition);
            position = new Rect(mousePos + _offset, position.size);
        }
    }
}