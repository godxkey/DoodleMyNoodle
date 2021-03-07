using UnityEditor;
using UnityEditorX;
using UnityEngine.UIElements;

public abstract class ToolsWindowBase : EditorWindow
{
    protected virtual void OnEnable()
    {
        Rebuild();
    }

    protected abstract string UssGuid { get; }
    protected abstract string UxmlGuid { get; }

    protected abstract void Rebuild(VisualElement root);

    private void Rebuild()
    {
        VisualElement root = rootVisualElement;
        root.Clear();

        var styleSheet = AssetDatabaseX.LoadAssetWithGUID<StyleSheet>(UssGuid);
        var visualTree = AssetDatabaseX.LoadAssetWithGUID<VisualTreeAsset>(UxmlGuid);
        var commonStyleSheet = AssetDatabaseX.LoadAssetWithGUID<StyleSheet>(ToolsConstants.Resources.COMMON_STYLES_USS_GUID);

        if (styleSheet != null)
            root.styleSheets.Add(styleSheet);
        root.styleSheets.Add(commonStyleSheet);
        visualTree.CloneTree(root);

        var refreshButton = root.Q<Button>(name: "refreshButton");
        if (refreshButton != null)
        {
            refreshButton.clickable.clicked -= Rebuild;
            refreshButton.clickable.clicked += Rebuild;
        }

        Rebuild(root);
    }
}