using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseDisplay : GameMonoBehaviour
{
    public static MouseDisplay Instance = null;

    // TOOLTIP
    public GameObject TooltipPanel;
    public Text TooltipName;
    public Text TooltipDescription;

    public override void OnGameReady()
    {
        base.OnGameReady();

        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void Update()
    {
        transform.position = Input.mousePosition;
    }

    private void UpdateHoverText(string name, string description)
    {
        if (!name.IsNullOrEmpty() || !description.IsNullOrEmpty())
        {
            TooltipName.text = name;
            TooltipDescription.text = description;
        }
    }

    public void SetToolTipDisplay(bool IsActive, string name = "", string description = "")
    {
        UpdateHoverText(name, description);
        TooltipPanel.SetActive(IsActive);
    }
}
