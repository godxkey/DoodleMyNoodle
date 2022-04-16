using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;
using static fixMath;

public class FloatingTextSystem : GamePresentationSystem<FloatingTextSystem>
{
    private struct Request
    {
        public string Text;
        public Color Color;
        public Vector2 Position;
        public Vector2 Scale;
    }

    private List<Request> _requests = new List<Request>();

    public GameObject FloatingNumberPrefab;

    public override void PresentationUpdate()
    {
        foreach (Request request in _requests)
        {
            // TODO : Do a pool system
            GameObject newFloatingNumber = Instantiate(FloatingNumberPrefab, request.Position, Quaternion.identity);
            newFloatingNumber.transform.localScale = request.Scale;
            newFloatingNumber.GetComponent<FloatingTextDisplay>()?.Display(request.Text, request.Color);
        }
        _requests.Clear();
    }

    public void RequestText(Vector2 position, Vector2 scale, string text, Color color)
    {
        _requests.Add(new Request()
        {
            Color = color,
            Position = position,
            Text = text,
            Scale = scale
        });
    }
}