
namespace CCC.InspectorDisplay
{
    public class ShowIfAttribute : HideShowBaseAttribute
    {
        public ShowIfAttribute(string conditionalMemberName) : base(conditionalMemberName) { }
    }
}