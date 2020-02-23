using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuMouseHoveringDetection : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Camera GameCamera;

    public void OnPointerExit(PointerEventData eventData)
    {
        GameCamera?.GetComponent<CameraMovement>()?.MouseChangedSection(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        GameCamera?.GetComponent<CameraMovement>()?.MouseChangedSection(true);
    }
}
