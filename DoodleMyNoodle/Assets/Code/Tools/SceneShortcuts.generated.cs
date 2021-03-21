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

    [MenuItem("Scene Shortcuts/Gym_AI", priority = 4007)]
    public static void Assets_Scenes_Levels_Scene_Assets_Gym_AI() => LoadScene("Assets/Scenes/Levels/Scene Assets/Gym_AI.unity");

    [MenuItem("Scene Shortcuts/Gym_Art", priority = 4008)]
    public static void Assets_Scenes_Levels_Scene_Assets_Gym_Art() => LoadScene("Assets/Scenes/Levels/Scene Assets/Gym_Art.unity");

    [MenuItem("Scene Shortcuts/Gym_Interactables", priority = 4009)]
    public static void Assets_Scenes_Levels_Scene_Assets_Gym_Interactables() => LoadScene("Assets/Scenes/Levels/Scene Assets/Gym_Interactables.unity");

    [MenuItem("Scene Shortcuts/Gym_MiniGames", priority = 4010)]
    public static void Assets_Scenes_Levels_Scene_Assets_Gym_MiniGames() => LoadScene("Assets/Scenes/Levels/Scene Assets/Gym_MiniGames.unity");

    [MenuItem("Scene Shortcuts/Gym_Physics", priority = 4011)]
    public static void Assets_Scenes_Levels_Scene_Assets_Gym_Physics() => LoadScene("Assets/Scenes/Levels/Scene Assets/Gym_Physics.unity");

    [MenuItem("Scene Shortcuts/Gym_Surveys", priority = 4012)]
    public static void Assets_Scenes_Levels_Scene_Assets_Gym_Surveys() => LoadScene("Assets/Scenes/Levels/Scene Assets/Gym_Surveys.unity");

    [MenuItem("Scene Shortcuts/Lvl_Prototype3", priority = 4013)]
    public static void Assets_Scenes_Levels_Scene_Assets_Lvl_Prototype3() => LoadScene("Assets/Scenes/Levels/Scene Assets/Lvl_Prototype3.unity");

    [MenuItem("Scene Shortcuts/Lvl_SideScrollerExemple", priority = 4014)]
    public static void Assets_Scenes_Levels_Scene_Assets_Lvl_SideScrollerExemple() => LoadScene("Assets/Scenes/Levels/Scene Assets/Lvl_SideScrollerExemple.unity");

    [MenuItem("Scene Shortcuts/Menu_InGameEscape", priority = 5015)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_InGameEscape() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_InGameEscape.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineClientSessionChoice", priority = 5016)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineClientSessionChoice() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineClientSessionChoice.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineRoleChoice", priority = 5017)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineRoleChoice() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineRoleChoice.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineServerSessionCreation", priority = 5018)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineServerSessionCreation() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineServerSessionCreation.unity");

    [MenuItem("Scene Shortcuts/Misc_LoadingScreen", priority = 6019)]
    public static void Assets_Scenes_Misc_Scene_Assets_Misc_LoadingScreen() => LoadScene("Assets/Scenes/Misc/Scene Assets/Misc_LoadingScreen.unity");


    private static void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
