#if !UNITY_EDITOR
#define TEST
#endif

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridPreviewGizmo : MonoBehaviour
{
    [SerializeField]
    int _width = 10;
    [SerializeField]
    int _height = 5;

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        for (int i = -1; i < _width; i++)
        {
            Vector3 lineStartPosition = new Vector3(i + 0.5f, -0.5f, 0);
            Vector3 lineEndPosition = new Vector3(i + 0.5f, _height - 0.5f, 0);
            Gizmos.DrawLine(lineStartPosition, lineEndPosition);
        }

        for (int i = -1; i < _height; i++)
        {
            Vector3 lineStartPosition = new Vector3(-0.5f, i + 0.5f, 0);
            Vector3 lineEndPosition = new Vector3(_width - 0.5f, i + 0.5f, 0);
            Gizmos.DrawLine(lineStartPosition, lineEndPosition);
        }
    }
}
