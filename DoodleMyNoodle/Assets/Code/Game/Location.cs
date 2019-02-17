using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [ReadOnly]
    public Vector3 pos;

    void Awake()
    {
        pos = transform.position;
    }

    // Debug Display
    void OnDrawGizmos()
    {
        pos = transform.position;
        Gizmos.color = new Color(0, 1, 0, 0.75f);
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
