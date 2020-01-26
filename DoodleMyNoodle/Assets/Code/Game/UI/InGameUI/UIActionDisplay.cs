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
        SimPlayerActions simPlayerActions = SimPawnHelpers.GetComponentOnMainPlayer<SimPlayerActions>(PlayerIdHelpers.GetLocalSimPlayerComponent());

        if(simPlayerActions != null)
        {
            if (ActionImages.Count == simPlayerActions.Value)
            {
                return;
            }
            else if (ActionImages.Count < simPlayerActions.Value)
            {
                while (ActionImages.Count != simPlayerActions.Value)
                {
                    ActionImages.Add(Instantiate(ImagePrefab, ImageContainer));
                }
            }
            else if (ActionImages.Count > simPlayerActions.Value)
            {
                while (ActionImages.Count != simPlayerActions.Value)
                {
                    GameObject imageToDestroy = ActionImages.Last();
                    ActionImages.Remove(imageToDestroy);
                    Destroy(imageToDestroy);
                }
            }
        }
    }
}
