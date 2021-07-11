using Unity.Properties.UI;
using UnityEngine.UIElements;

namespace CCC.Editor
{
    public abstract class BaseFieldInspector<TField, TFieldValue, TValue> : Inspector<TValue>
        where TField : BaseField<TFieldValue>, new()
    {
        protected TField _field;

        public override VisualElement Build()
        {
            _field = new TField
            {
                name = Name,
                label = DisplayName,
                tooltip = Tooltip,
                bindingPath = "."
            };
            return _field;
        }
    }
}