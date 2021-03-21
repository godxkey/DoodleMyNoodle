using System;
using UnityEngine;
using UnityEngineX;

public class MiniGameUtility
{
    public static GameObject DrawLine(
        Vector3 start, 
        Vector3 end, 
        Color color, 
        float duration,
        float width,
        Shader lineShader)
    {
        GameObject currentLine = new GameObject();
        currentLine.transform.position = start;
        currentLine.name = "New Line";

        currentLine.AddComponent<LineRenderer>();
        LineRenderer renderer = currentLine.GetComponent<LineRenderer>();
        renderer.material = new Material(lineShader);

        renderer.startColor = color;
        renderer.endColor = color;

        renderer.startWidth = width;
        renderer.endWidth = width;

        renderer.SetPosition(0, start);
        renderer.SetPosition(1, end);

        if (duration > 0)
        {
            GameObject.Destroy(currentLine, duration);
        }

        return currentLine;
    }
}