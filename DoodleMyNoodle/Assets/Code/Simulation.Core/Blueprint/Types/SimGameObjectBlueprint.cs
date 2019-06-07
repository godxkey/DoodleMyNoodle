using UnityEngine;

/// <summary>
/// Blueprint based on a Unity Gameobject prefab
/// </summary>
[System.Serializable]
public class SimGameObjectBlueprint : SimBlueprint
{
    public GameObject prefab;

    public override void InstantiateEntityAndView(out SimEntity entity, out SimEntityView view)
    {
        // instantiate gameobject
        GameObject gameObject = prefab.Duplicate();

        // get view reference
        view = gameObject.GetComponent<SimEntityView>();

        // create entity
        entity = new SimEntity(gameObject.name);

        // add every component serialized the gameobject to the entity
        SimComponentView[] componentViews = gameObject.GetComponents<SimComponentView>();
        for (int i = 0; i < componentViews.Length; i++)
        {
            entity.components.Add(componentViews[i].serializedComponent);
        }
    }

    public bool Validate()
    {
        return prefab != null 
            && prefab.GetComponent<SimEntityView>() != null;
    }
}