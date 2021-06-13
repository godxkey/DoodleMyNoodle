using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CursorDisplay : GameMonoBehaviour
{
    [SerializeField] private Image MainCursor;
    [SerializeField] private Image SecondaryCursor;

    public void SetCursorsPosition(Vector3 newPosition)
    {
        MainCursor.transform.position = newPosition;
        SecondaryCursor.transform.position = newPosition;
    }

    public void SetPrimaryCursorIcon(Sprite sprite)
    {
        MainCursor.sprite = sprite;
    }

    public void SetPrimaryCursorColor(Color color)
    {
        MainCursor.color = color;
    }

    public void SetPrimaryCursorSize(Vector3 scale)
    {
        MainCursor.transform.localScale = scale;
    }

    public void ChangeDisplaySecondaryCursor(bool shouldDisplay)
    {
        SecondaryCursor.gameObject.SetActive(shouldDisplay);
    }
}