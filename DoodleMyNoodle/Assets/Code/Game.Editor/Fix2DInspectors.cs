using CCC.Editor;
using JetBrains.Annotations;
using Unity.Properties;
using UnityEditor.UIElements;
using UnityEngine;

[UsedImplicitly]
class Fix3Inspector : BaseFieldInspector<Vector3Field, Vector3, fix3>
{
    static Fix3Inspector()
    {
        TypeConversion.Register<fix3, Vector3>(v => (Vector3)v);
        TypeConversion.Register<Vector3, fix3>(v => (fix3)v);
    }
}

[UsedImplicitly]
class Fix2Inspector : BaseFieldInspector<Vector2Field, Vector2, fix2>
{
    static Fix2Inspector()
    {
        TypeConversion.Register<fix2, Vector2>(v => (Vector2)v);
        TypeConversion.Register<Vector2, fix2>(v => (fix2)v);
    }
}

[UsedImplicitly]
class FixInspector : BaseFieldInspector<FloatField, float, fix>
{
    static FixInspector()
    {
        TypeConversion.Register<fix, float>(v => (float)v);
        TypeConversion.Register<float, fix>(v => (fix)v);
    }
}