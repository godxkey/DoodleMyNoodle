using System.Collections.Generic;
using UnityEngine;
using UnityEngineX;

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

    public override void OnPostSimulationTick()
    {
        Cache.SimWorld.Entities.ForEach((ref DamageEventData damageData) =>
        {
            Vector2 pos = damageData.Position.ToUnityVec();
            pos += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            RequestText(pos, damageData.DamageApplied.ToString(), Color.red);
        });

        Cache.SimWorld.Entities.ForEach((ref HealEventData healingData) =>
        {
            Vector2 pos = healingData.Position.ToUnityVec();
            pos += new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
            RequestText(pos, healingData.HealApplied.ToString(), Color.green);
        });

        foreach (Request request in _requests)
        {
            // TODO : Do a pool system
            GameObject newFloatingNumber = Instantiate(FloatingNumberPrefab, request.Position, Quaternion.identity);
            newFloatingNumber.GetComponent<FloatingTextDisplay>()?.Display(request.Text, request.Color);
        }
        _requests.Clear();
    }

    protected override void OnGamePresentationUpdate() { }

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