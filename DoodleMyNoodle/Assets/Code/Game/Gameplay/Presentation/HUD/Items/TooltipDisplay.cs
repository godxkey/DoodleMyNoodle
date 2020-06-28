using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipDisplay : GamePresentationSystem<TooltipDisplay>
{
    // TOOLTIP
    public GameObject TooltipPanel;
    public Image TooltipContour;
    public TextMeshProUGUI ItemName;
    public TextMeshProUGUI ItemDescription;

    public Color Common = Color.white;
    public Color UnCommon = Color.green;
    public Color Rare = Color.blue;
    public Color Mythic = Color.magenta;
    public Color Legendary = Color.yellow;

    public float ScreenEdgeToolTipLimit = 15.0f;

    public override bool SystemReady { get => true; }

    private float _displacementX = 0;
    private float _displacementY = 0;

    protected override void Awake()
    {
        base.Awake();

        _displacementX = transform.position.x;
        _displacementY = transform.position.y;
    }

    protected override void OnGamePresentationUpdate()
    {
        if (TooltipPanel.activeSelf)
        {
            UpdateToolTipPosition();
            
        }
    }

    private void UpdateHoverText(string name, string description)
    {
        ItemName.text = name;
        ItemDescription.text = description;
    }

    public void SetToolTipDisplay(bool IsActive, string name = "", string description = "")
    {
        UpdateHoverText(name, description);
        TooltipPanel.SetActive(IsActive);
    }

    // Switch to using this function after branch merge
    public void SetToolTipDisplay(bool IsActive, ItemVisualInfo itemInfo)
    {
        UpdateHoverText(itemInfo.Name, itemInfo.Description);
        TooltipPanel.SetActive(IsActive);
    }

    private void UpdateToolTipPosition()
    {
        bool exitTop = Input.mousePosition.y >= (Screen.height - ScreenEdgeToolTipLimit);
        bool exitBottom = Input.mousePosition.y <= ScreenEdgeToolTipLimit;
        bool exitRight = Input.mousePosition.x >= (Screen.width - ScreenEdgeToolTipLimit);
        bool exitLeft = Input.mousePosition.x <= ScreenEdgeToolTipLimit;

        transform.position = Input.mousePosition + new Vector3(_displacementX, _displacementY, 0);

        Vector3 currentPos = transform.position;

        if (exitTop)
        {
            transform.position = new Vector3(currentPos.x, -1 * currentPos.y, currentPos.z);
        }
        else if (exitBottom)
        {
            transform.position = new Vector3(currentPos.x, Mathf.Abs(currentPos.y), currentPos.z);
        }

        if (exitRight)
        {
            transform.position = new Vector3(-1 * currentPos.x, currentPos.y, currentPos.z);
        }
        else if (exitLeft)
        {
            transform.position = new Vector3(Mathf.Abs(currentPos.x), currentPos.y, currentPos.z);
        }
    }
}
