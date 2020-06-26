using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace UnityEditor.Tilemaps
{
    public class BaseSmartBrush : GridBrushBase
    {
        protected List<GameObject> GetObjectsInCell(GridLayout grid, Transform parent, Vector3Int position)
        {
            var results = new List<GameObject>();
            var childCount = parent.childCount;
            for (var i = 0; i < childCount; i++)
            {
                var child = parent.GetChild(i);
                if (position == grid.WorldToCell(child.position))
                {
                    results.Add(child.gameObject);
                }
            }
            return results;
        }
    }

    [CustomEditor(typeof(BaseSmartBrush))]
    public class BaseSmartBrushEditor : GridBrushEditorBase
    {
        private BaseSmartBrush baseSmartBrush { get { return target as BaseSmartBrush; } }

        public override void OnPaintInspectorGUI()
        {
            base.OnPaintInspectorGUI();
        }

        public override GameObject[] validTargets
        {
            get
            {
                return GameObject.FindObjectsOfType<Tilemap>().Select(x => x.gameObject).ToArray();
            }
        }

        public override void OnPaintSceneGUI(GridLayout gridLayout, GameObject brushTarget, BoundsInt position, GridBrushBase.Tool tool, bool executing)
        {
            //insert here custom code for switch prefab based on button press
            base.OnPaintSceneGUI(gridLayout, brushTarget, position, tool, executing);
        }
    }
}