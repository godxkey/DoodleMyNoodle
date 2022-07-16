using UnityEngine;
using Random = UnityEngine.Random;

public class UniqueSpriteZ : MonoBehaviour
{
    public float RangeMin = 0;
    public float RangeMax = 100;

    private void Awake()
    {
        var tr = transform;
        var pos = tr.position;
        pos.z = Random.Range(RangeMin, RangeMax);
        tr.position = pos;
    }
}