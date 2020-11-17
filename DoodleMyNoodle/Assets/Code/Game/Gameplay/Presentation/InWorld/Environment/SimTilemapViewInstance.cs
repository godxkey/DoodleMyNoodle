using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngineX;

public class SimTilemapViewInstance : MonoBehaviour
{
    [SerializeField] private Tilemap _tilemap;
    
    public static Tilemap Instance { get; private set; }

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        Instance = _tilemap;
    }

    private void OnDestroy()
    {
        Instance = null;
    }
}