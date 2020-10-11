
namespace CCC.InspectorDisplay
{
    public class HideIfAttribute : HideShowBaseAttribute
    {
        public HideIfAttribute(string conditionalMemberName) : base(conditionalMemberName) { }
    }
}