
namespace CCC.InspectorDisplay
{
    public class ShowIfAttribute : HideShowBaseAttribute
    {
        public ShowIfAttribute(string conditionalMemberName, bool indent = true) : base(conditionalMemberName, indent) { }
    }
}