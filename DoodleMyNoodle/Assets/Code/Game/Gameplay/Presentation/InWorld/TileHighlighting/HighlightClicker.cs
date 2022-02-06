using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class HighlightClicker : GamePresentationBehaviour
{
    public System.Action<Vector2> OnClicked;

    private Transform _tr;

    protected override void Awake()
    {
        base.Awake();
        _tr = transform;
    }

    protected override void OnGamePresentationUpdate()
    {
        if(gameObject.activeSelf && IsMouseInsideHighlight(GetMousePositionOnTile()))
        {
            if (Input.GetMouseButtonDown(0))
            {
                OnClicked.Invoke(_tr.position);
            }
        }
    }

    private Vector3 GetMousePositionOnTile()
    {
        Ray ray = CameraService.Instance.ActiveCamera.ScreenPointToRay(Input.mousePosition);

        Plane floorPlane = new Plane(CommonReads.GetFloorPlaneNormal(SimWorld).ToUnityVec(), 0);
        if (floorPlane.Raycast(ray, out float enter))
        {
            Vector3 hitPoint = ray.GetPoint(enter);
            return hitPoint;
        }

        return new Vector3();
    }

    private bool IsMouseInsideHighlight(Vector3 mousePos)
    {
        Vector3 pos = _tr.position;
        return mousePos.x <= pos.x + 0.5f
            && mousePos.x >= pos.x - 0.5f
            && mousePos.y <= pos.y + 0.5f
            && mousePos.y >= pos.y - 0.5f;
    }
}
