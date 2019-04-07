using UnityEditor;
using UnityEngine;

public class MegaLaunchWindow : EditorWindow
{
    [MenuItem("Tools/Pipeline/Mega Launch Window #&l")]
    public static void ShowWindow()
    {
        GetWindow<MegaLaunchWindow>(false, "Mega Launch", true);
    }


    BuildWindowContent _buildContent = new BuildWindowContent();
    QuickStartDefaultsContent _quickStartContent = new QuickStartDefaultsContent();
    LauncherWindowContent _launcherContent = new LauncherWindowContent();


    void OnGUI()
    {
        var defaultGUIColor = GUI.color;

        GUI.color = defaultGUIColor * new Color(1, 1, 0.5f);
        EditorGUILayout.LabelField("BUILDER", EditorStyles.boldLabel);
        _buildContent.OnGUI();

        GUILayout.Space(30);

        GUI.color = defaultGUIColor * new Color(1, 0.75f, 1);
        EditorGUILayout.LabelField("QUICK START DEFAULTS", EditorStyles.boldLabel);
        _quickStartContent.OnGUI();

        GUILayout.Space(30);

        GUI.color = defaultGUIColor * new Color(0.75f, 1, 1);
        EditorGUILayout.LabelField("LAUNCHER", EditorStyles.boldLabel);
        _launcherContent.OnGUI();

        GUI.color = defaultGUIColor;
    }

    void OnEnable()
    {
        _launcherContent.Load();
    }

    void OnDisable()
    {
        _launcherContent.Save();
    }

}