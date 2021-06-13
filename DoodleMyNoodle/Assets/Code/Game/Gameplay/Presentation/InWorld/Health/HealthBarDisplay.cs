using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class HealthBarDisplay : GameMonoBehaviour
{
    [SerializeField] private GameObject EmptyHearthPrefab;
    [SerializeField] private Transform HearthContainer;

    [SerializeField] private Sprite EmptyHearth;
    [SerializeField] private Sprite FilledHearth;

    [SerializeField] private CanvasGroup CanvasGroup;
    [SerializeField] private float CanvasFadeSpeed = 0.1f;

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

    private void Update()
    {
        CanvasGroup.alpha -= (Time.deltaTime * CanvasFadeSpeed);


    }

    public void ForceDisplay()
    {
        CanvasGroup.alpha = 1;
    }
}
