using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MouseDisplay : GameSystem<MouseDisplay>
{
    // TOOLTIP
    public GameObject TooltipPanel;
    public Text TooltipName;
    public Text TooltipDescription;

    public override bool SystemReady => true;

    public override void OnGameUpdate()
    {
        base.OnGameUpdate();

        if (TooltipPanel.activeSelf)
        {
            transform.position = Input.mousePosition;
        }
    }

    private void UpdateHoverText(string name, string description)
    {
        TooltipName.text = name;
        TooltipDescription.text = description;
    }

    public void SetToolTipDisplay(bool IsActive, string name = "", string description = "")
    {
        UpdateHoverText(name, description);
        TooltipPanel.SetActive(IsActive);
    }
}
