using UnityEditorX;
using UnityEngine.UIElements;

public abstract class ToolsVisualElementBase : VisualElement
{
    public abstract string UxmlGuid { get; }
    public abstract string UssGuid { get; }

    public ToolsVisualElementBase()
    {
        var uss = AssetDatabaseX.LoadAssetWithGUID<StyleSheet>(UssGuid);
        if (uss != null)
            styleSheets.Add(uss);
        styleSheets.Add(AssetDatabaseX.LoadAssetWithGUID<StyleSheet>(ToolsConstants.Resources.COMMON_STYLES_USS_GUID));
        VisualTreeAsset visualTree = AssetDatabaseX.LoadAssetWithGUID<VisualTreeAsset>(UxmlGuid);
        visualTree.CloneTree(this);
    }
}
