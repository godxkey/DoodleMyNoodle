
namespace CCC.InspectorDisplay
{
    public class HideIfAttribute : HideShowBaseAttribute
    {
        public HideIfAttribute(string conditionalMemberName, bool indent = false) : base(conditionalMemberName, indent) { }
    }
}