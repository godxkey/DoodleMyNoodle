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

        CurrentValueText.text = Mathf.Ceil(value).ToString();
        MaxValueText.text = Mathf.Ceil(maxValue).ToString();
    }

    public override void PresentationUpdate()
    {
        SetStatBar(GetStatBarValue(), GetStatBarMaxValue());
    }

    // Override this in child class
    protected virtual float GetStatBarMaxValue() { return 1; }
    protected virtual float GetStatBarValue() { return 0; }
}