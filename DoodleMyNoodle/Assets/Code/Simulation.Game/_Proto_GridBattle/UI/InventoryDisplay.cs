using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    public void ToggleInventory()
    {
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
