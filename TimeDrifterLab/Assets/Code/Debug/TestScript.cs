using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public Transform[] points;

    void Update()
    {
        Debug.Log(CMath.Area2DWithinTriangle(points[0].position, points[1].position, points[2].position));
    }
}
