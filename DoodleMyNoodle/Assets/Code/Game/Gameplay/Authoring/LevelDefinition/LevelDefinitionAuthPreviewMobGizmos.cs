#if UNITY_EDITOR
using UnityEngine;

public class LevelDefinitionAuthPreviewMobGizmos : MonoBehaviour
{
    public MobSpawmModifierFlags MobModifierFlags;

    private void OnDrawGizmos()
    {
        Vector3 positionOffset = new Vector3(0.25f, -0.35f);

        if ((MobModifierFlags & MobSpawmModifierFlags.Armored) != 0)
        {
            DrawModifier(Color.white);
        }
        if ((MobModifierFlags & MobSpawmModifierFlags.Brutal) != 0)
        {
            DrawModifier(Color.red);
        }
        if ((MobModifierFlags & MobSpawmModifierFlags.Fast) != 0)
        {
            DrawModifier(Color.blue);
        }
        if ((MobModifierFlags & MobSpawmModifierFlags.Explosive) != 0)
        {
            DrawModifier(new Color(1, 0.5f, 0f)); // orange
        }

        void DrawModifier(Color color)
        {
            float radius = 0.25f;
            Gizmos.color = color;
            Gizmos.DrawSphere(transform.position + positionOffset, radius);
            positionOffset += Vector3.up * (radius + radius + 0.05f);
        }
    }
}
#endif