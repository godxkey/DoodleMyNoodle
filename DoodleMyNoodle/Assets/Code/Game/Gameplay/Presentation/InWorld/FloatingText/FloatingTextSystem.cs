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
    }

    private List<Request> _requests = new List<Request>();

    public GameObject FloatingNumberPrefab;

    protected override void OnGamePresentationUpdate()
    {
        CreateRequestsFromHPDeltaEvents();

        foreach (Request request in _requests)
        {
            // TODO : Do a pool system
            GameObject newFloatingNumber = Instantiate(FloatingNumberPrefab, request.Position, Quaternion.identity);
            newFloatingNumber.GetComponent<FloatingTextDisplay>()?.Display(request.Text, request.Color);
        }
        _requests.Clear();
    }

    private void CreateRequestsFromHPDeltaEvents()
    {
        foreach (var healthDeltaEvent in PresentationEvents.HealthDeltaEvents.SinceLastPresUpdate)
        {
            Vector2 pos = healthDeltaEvent.Position.ToUnityVec();
            pos += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

            fix displayedValue;
            Color color;

            if (healthDeltaEvent.IsHeal)
            {
                // Heal
                displayedValue = max(1, round(healthDeltaEvent.TotalUncappedDelta));
                color = Color.green;
            }
            else
            {
                // Damage
                displayedValue = max(1, round(-healthDeltaEvent.TotalUncappedDelta));
                color = Color.red;
            }
            RequestText(pos, displayedValue.ToString(), color);
        }
    }

    public void RequestText(Vector2 position, string text, Color color)
    {
        _requests.Add(new Request()
        {
            Color = color,
            Position = position,
            Text = text
        });
    }
}