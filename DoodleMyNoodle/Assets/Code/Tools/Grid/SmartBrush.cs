using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    [CreateAssetMenu]
    [CustomGridBrush(false, true, false, "Smart Brush")]
    public class SmartBrush : BaseSmartBrush
    {
        public override void Pick(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, Vector3Int pivot)
        {
            base.Pick(gridLayout, brushTarget, position, pivot);

            Debug.Log("Layer : " + brushTarget.name);
            Debug.Log("Layout : " + gridLayout.name);

            foreach (var target in GridPaintingState.validTargets)
            {
                Debug.Log("Target : " + target.name);
            }
        }
    }

    [CustomEditor(typeof(SmartBrush))]
    public class SmartBrushEditor : BaseSmartBrushEditor
    {
    }
}
