using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBarDisplay : GamePresentationBehaviour
{
    [SerializeField] protected Slider StatBar = null;
    [SerializeField] protected TextMeshProUGUI CurrentValueText = null;
    [SerializeField] protected TextMeshProUGUI MaxValueText = null;

    public virtual void SetStatBar(float value, float maxValue)
    {
        StatBar.maxValue = maxValue;
        StatBar.value = value;

        CurrentValueText.text = value.ToString();
        MaxValueText.text = maxValue.ToString();
    }

    protected override void OnGamePresentationUpdate()
    {
        SetStatBar(GetStatBarValue(), GetStatBarMaxValue());
    }

    // Override this in child class
    protected virtual float GetStatBarMaxValue() { return 1; }
    protected virtual float GetStatBarValue() { return 0; }
}