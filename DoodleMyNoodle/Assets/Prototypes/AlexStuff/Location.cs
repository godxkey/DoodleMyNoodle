using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Location : MonoBehaviour
{
    [HideInInspector]
    public Vector3 pos;

    void Start()
    {
        pos = transform.position;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 1, 0, 0.75f);
        Gizmos.DrawSphere(transform.position, 0.25f);
    }
}
