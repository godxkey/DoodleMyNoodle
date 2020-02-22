using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIActionDisplay : MonoBehaviour
{
    public Transform ImageContainer;
    public GameObject ImagePrefab;
    public List<GameObject> ActionImages = new List<GameObject>();

    private void Update()
    {
        SimPlayerActions simPlayerActions = SimPawnHelpers.GetComponentOnControllersPawn<SimPlayerActions>(PlayerIdHelpers.GetLocalSimPlayerComponent());

        if(simPlayerActions != null)
        {
            while (ActionImages.Count < simPlayerActions.Value)
            {
                ActionImages.Add(Instantiate(ImagePrefab, ImageContainer));
            }

            while (ActionImages.Count > simPlayerActions.Value)
            {
                Destroy(ActionImages.Pop());
            }
        }
    }
}
