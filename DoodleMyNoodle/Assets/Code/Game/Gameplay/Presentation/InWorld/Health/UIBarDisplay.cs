using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class UIBarDisplay : GameMonoBehaviour
{
    [SerializeField] private GameObject EmptyHearthPrefab;
    [SerializeField] private Transform HearthContainer;

    [SerializeField] private Sprite EmptyHearth;
    [SerializeField] private Sprite FilledHearth;

    private List<GameObject> SpawnedHearth = new List<GameObject>();

    public void SetMaxHealth(int amount)
    {
        while (SpawnedHearth.Count != amount)
        {
            if (SpawnedHearth.Count < amount)
            {
                SpawnedHearth.Add(Instantiate(EmptyHearthPrefab, HearthContainer));
            }
            else if (SpawnedHearth.Count > amount)
            {
                Destroy(SpawnedHearth[SpawnedHearth.Count - 1]);
                SpawnedHearth.RemoveAt(SpawnedHearth.Count - 1);
            }
        }
    }

    public void SetHealth(int amount)
    {
        for (int i = 0; i < amount; i++)
        {
            Image Image = SpawnedHearth[i].GetComponent<Image>();
            if (Image != null)
            {
                Image.sprite = FilledHearth;
            }
        }

        for (int i = amount; i < SpawnedHearth.Count; i++)
        {
            Image Image = SpawnedHearth[i].GetComponent<Image>();
            if (Image != null)
            {
                Image.sprite = EmptyHearth;
            }
        }
    }
}
