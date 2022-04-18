using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyAfterDelay : MonoBehaviour
{
    [SerializeField] private float _delay = 2f;

    /// <summary>
    /// Changes only have effect if performed before Start is called.
    /// </summary>
    public float Delay { get => _delay; set => _delay = value; }

    void Start()
    {
        Destroy(gameObject, _delay);
    }
}