using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HighlightClicker : GameMonoBehaviour
{
    public class OnHighlightClicked : UnityEvent<Vector2> { }

    [HideInInspector]
    public OnHighlightClicked OnClicked = new OnHighlightClicked();

    public override void OnGameUpdate()
    {
        if(gameObject.activeSelf && IsMouseInsideHighlight(GetMousePositionOnTile()))
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClicked.Invoke(new Vector2(transform.position.x, transform.position.y));
            }
        }
    }

    private Vector3 GetMousePositionOnTile()
    {
        Ray ray = CameraService.Instance.ActiveCamera.ScreenPointToRay(Input.mousePosition);

        float enter = 0.0f;
        if (SimTileManager.Instance.FloorPlane.Raycast(ray, out enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }

        return new Vector3();
    }

    private bool IsMouseInsideHighlight(Vector3 mousePos)
    {
        return mousePos.x <= transform.position.x + 0.5f
            && mousePos.x >= transform.position.x - 0.5f
            && mousePos.y <= transform.position.y + 0.5f
            && mousePos.y >= transform.position.y - 0.5f;
    }
}
