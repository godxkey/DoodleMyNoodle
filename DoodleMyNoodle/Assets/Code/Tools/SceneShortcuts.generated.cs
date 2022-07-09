// THIS CODE IS GENERATED
// DO NOT MODIFY IT

using UnityEditor;
using UnityEditor.SceneManagement;

public static class SceneShortcuts
{

    [MenuItem("Scene Shortcuts/Boot_ApplicationBoot", priority = 1000)]
    public static void Assets_Scenes_Boot_Scene_Assets_Boot_ApplicationBoot() => LoadScene("Assets/Scenes/Boot/Scene Assets/Boot_ApplicationBoot.unity");

    [MenuItem("Scene Shortcuts/Boot_LoadedOnceOnStart", priority = 1001)]
    public static void Assets_Scenes_Boot_Scene_Assets_Boot_LoadedOnceOnStart() => LoadScene("Assets/Scenes/Boot/Scene Assets/Boot_LoadedOnceOnStart.unity");

    [MenuItem("Scene Shortcuts/EmptyScene", priority = 2002)]
    public static void Assets_Scenes_EmptyScene() => LoadScene("Assets/Scenes/EmptyScene.unity");

    [MenuItem("Scene Shortcuts/Game Content (dynamic)", priority = 3003)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_Content__dynamic_() => LoadScene("Assets/Scenes/Game/Scene Assets/Game Content (dynamic).unity");

    [MenuItem("Scene Shortcuts/Game Systems (dynamic)", priority = 3004)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_Systems__dynamic_() => LoadScene("Assets/Scenes/Game/Scene Assets/Game Systems (dynamic).unity");

    [MenuItem("Scene Shortcuts/Game_Core", priority = 3005)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_Core() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_Core.unity");

    [MenuItem("Scene Shortcuts/Game_SimulationInitData", priority = 3006)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_SimulationInitData() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_SimulationInitData.unity");

    [MenuItem("Scene Shortcuts/MAP_EmptyLevel", priority = 4007)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_EmptyLevel() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_EmptyLevel.unity");

    [MenuItem("Scene Shortcuts/MAP_EmptyLevel_Presentation", priority = 4008)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_EmptyLevel_Presentation() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_EmptyLevel_Presentation.unity");

    [MenuItem("Scene Shortcuts/MAP_Gym_Art", priority = 4009)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Gym_Art() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Gym_Art.unity");

    [MenuItem("Scene Shortcuts/MAP_Gym_Fred", priority = 4010)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Gym_Fred() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Gym_Fred.unity");

    [MenuItem("Scene Shortcuts/MAP_Gym_Physics", priority = 4011)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Gym_Physics() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Gym_Physics.unity");

    [MenuItem("Scene Shortcuts/MAP_Gym_Surveys", priority = 4012)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Gym_Surveys() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Gym_Surveys.unity");

    [MenuItem("Scene Shortcuts/MAP_Proto8", priority = 4013)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Proto8() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Proto8.unity");

    [MenuItem("Scene Shortcuts/MAP_Prototype7_Easy", priority = 4014)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Prototype7_Easy() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Prototype7_Easy.unity");

    [MenuItem("Scene Shortcuts/MAP_Prototype7_Easy_Presentation", priority = 4015)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Prototype7_Easy_Presentation() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Prototype7_Easy_Presentation.unity");

    [MenuItem("Scene Shortcuts/MAP_Prototype7_Hard", priority = 4016)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Prototype7_Hard() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Prototype7_Hard.unity");

    [MenuItem("Scene Shortcuts/MAP_Prototype7_Hard_Presentation", priority = 4017)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Prototype7_Hard_Presentation() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Prototype7_Hard_Presentation.unity");

    [MenuItem("Scene Shortcuts/MAP_Prototype7_Medium", priority = 4018)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Prototype7_Medium() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Prototype7_Medium.unity");

    [MenuItem("Scene Shortcuts/MAP_Prototype7_Medium_Presentation", priority = 4019)]
    public static void Assets_Scenes_Levels_Scene_Assets_MAP_Prototype7_Medium_Presentation() => LoadScene("Assets/Scenes/Levels/Scene Assets/MAP_Prototype7_Medium_Presentation.unity");

    [MenuItem("Scene Shortcuts/Menu_InGameEscape", priority = 5020)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_InGameEscape() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_InGameEscape.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineClientSessionChoice", priority = 5021)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineClientSessionChoice() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineClientSessionChoice.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineRoleChoice", priority = 5022)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineRoleChoice() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineRoleChoice.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineServerSessionCreation", priority = 5023)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineServerSessionCreation() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineServerSessionCreation.unity");

    [MenuItem("Scene Shortcuts/Misc_LoadingScreen", priority = 6024)]
    public static void Assets_Scenes_Misc_Scene_Assets_Misc_LoadingScreen() => LoadScene("Assets/Scenes/Misc/Scene Assets/Misc_LoadingScreen.unity");


    private static void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
