using UnityEngine;

/// <summary>
/// Banque de blueprint utilisé pour nos premier test
/// </summary>
public class StupidSimBlueprintBank : MonoBehaviour, ISimBlueprintBank
{
    public static StupidSimBlueprintBank instance;

    void Awake()
    {
        instance = this;
    }

    public SimGameObjectBlueprint ballprefab;

    public SimBlueprint GetBlueprint(SimBlueprintId blueprintId)
    {
        switch (blueprintId.value)
        {
            case 1:
                return ballprefab;

            default:
                return null;
        }
    }
}
