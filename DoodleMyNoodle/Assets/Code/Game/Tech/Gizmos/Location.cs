using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CCC.InspectorDisplay;

public class Location : MonoBehaviour
{
    public bool DestroyOnAwake = true;

    public Color GizmoColor = new Color(0, 1, 0, 0.75f);

    [ReadOnlyAlways]
    public Vector3 Pos;

    void Awake()
    {
        Pos = transform.position;

        if (DestroyOnAwake)
        {
            Destroy(this);
        }
    }

    // Debug Display
    void OnDrawGizmos()
    {
        Pos = transform.position;
        Gizmos.color = GizmoColor;
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
