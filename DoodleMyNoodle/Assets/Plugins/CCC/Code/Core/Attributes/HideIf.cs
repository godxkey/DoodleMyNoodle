
namespace CCC.InspectorDisplay
{
    public class HideIfAttribute : HideShowBaseAttribute
    {
        public HideIfAttribute(string name, Type type = Type.Field) : base(name, type) { }
    }
}