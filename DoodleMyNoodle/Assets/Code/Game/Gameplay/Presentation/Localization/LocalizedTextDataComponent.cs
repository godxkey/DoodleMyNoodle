using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LocalizedTextDataComponent : MonoBehaviour
{
    public TextData TextData;

    private void Start()
    {
        string resultText = TextData.ToString();

        Text TextComponent = GetComponent<Text>();
        if (TextComponent != null)
        {
            TextComponent.text = resultText;
        }

        TextMeshProUGUI TextMeshProUGUIComponent = GetComponent<TextMeshProUGUI>();
        if (TextMeshProUGUIComponent != null)
        {
            TextMeshProUGUIComponent.text = resultText;
        }

        TextMeshPro TextMeshProComponent = GetComponent<TextMeshPro>();
        if (TextMeshProComponent != null)
        {
            TextMeshProComponent.text = resultText;
        }
    }
}