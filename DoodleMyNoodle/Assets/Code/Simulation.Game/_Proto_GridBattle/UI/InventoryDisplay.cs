using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryDisplay : MonoBehaviour
{
    private bool _hasBeenSetup = false;

    public void ToggleInventory()
    {
        if (!_hasBeenSetup)
        {
            _hasBeenSetup = true;

            //SimPawnManager.Instance.Get
        }
        gameObject.SetActive(!gameObject.activeSelf);
    }
}
