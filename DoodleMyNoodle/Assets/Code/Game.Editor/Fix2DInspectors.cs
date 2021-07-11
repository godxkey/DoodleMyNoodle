using CCC.Editor;
using JetBrains.Annotations;
using Unity.Properties;
using Unity.Properties.UI;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

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

[UsedImplicitly]
class PhysicsColliderInspector : Inspector<CCC.Fix2D.PhysicsColliderBlob>
{
    private Toggle _isCreatedField;
    private EnumField _typeField;
    private FloatField _radiusField;
    private LongField _addressField;

    public override VisualElement Build()
    {
        var container = new VisualElement();

        _isCreatedField = new Toggle("Is Created");
        _addressField = new LongField("Address");
        _typeField = new EnumField("Type", CCC.Fix2D.ColliderType.Invalid);
        _radiusField = new FloatField("Radius");

        container.Add(_isCreatedField);
        container.Add(_addressField);
        container.Add(_typeField);
        container.Add(_radiusField);

        return container;
    }

    public override void Update()
    {
        _isCreatedField.SetValueWithoutNotify(Target.Collider.IsCreated);

        unsafe
        {
            long address = (long)Target.Collider.GetUnsafePtr();
            _addressField.SetValueWithoutNotify(address);
        }

        if (Target.Collider.IsCreated)
        {
            _radiusField.SetValueWithoutNotify(Target.Collider.Value.Radius);
            _typeField.SetValueWithoutNotify(Target.Collider.Value.ColliderType);
        }
    }
}