using System;
using UnityEngine;
using UnityEngineX;

public class DestroyOnPlay : MonoBehaviour
{
    private void Awake()
    {
        Destroy(gameObject);
    }
}