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

    [MenuItem("Scene Shortcuts/Game_Client", priority = 3003)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_Client() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_Client.unity");

    [MenuItem("Scene Shortcuts/Game_Common", priority = 3004)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_Common() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_Common.unity");

    [MenuItem("Scene Shortcuts/Game_Local", priority = 3005)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_Local() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_Local.unity");

    [MenuItem("Scene Shortcuts/Game_Server", priority = 3006)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_Server() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_Server.unity");

    [MenuItem("Scene Shortcuts/Game_SimulationBasePresentation", priority = 3007)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_SimulationBasePresentation() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_SimulationBasePresentation.unity");

    [MenuItem("Scene Shortcuts/Game_SimulationManagers", priority = 3008)]
    public static void Assets_Scenes_Game_Scene_Assets_Game_SimulationManagers() => LoadScene("Assets/Scenes/Game/Scene Assets/Game_SimulationManagers.unity");

    [MenuItem("Scene Shortcuts/Lvl_GridBattle", priority = 4009)]
    public static void Assets_Scenes_Levels_Scene_Assets_Lvl_GridBattle() => LoadScene("Assets/Scenes/Levels/Scene Assets/Lvl_GridBattle.unity");

    [MenuItem("Scene Shortcuts/Lvl_GridBattle_Presentation", priority = 4010)]
    public static void Assets_Scenes_Levels_Scene_Assets_Lvl_GridBattle_Presentation() => LoadScene("Assets/Scenes/Levels/Scene Assets/Lvl_GridBattle_Presentation.unity");

    [MenuItem("Scene Shortcuts/Lvl_Prototype3", priority = 4011)]
    public static void Assets_Scenes_Levels_Scene_Assets_Lvl_Prototype3() => LoadScene("Assets/Scenes/Levels/Scene Assets/Lvl_Prototype3.unity");

    [MenuItem("Scene Shortcuts/Lvl_SideScrollerExemple", priority = 4012)]
    public static void Assets_Scenes_Levels_Scene_Assets_Lvl_SideScrollerExemple() => LoadScene("Assets/Scenes/Levels/Scene Assets/Lvl_SideScrollerExemple.unity");

    [MenuItem("Scene Shortcuts/Menu_InGameEscape", priority = 5013)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_InGameEscape() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_InGameEscape.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineClientSessionChoice", priority = 5014)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineClientSessionChoice() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineClientSessionChoice.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineRoleChoice", priority = 5015)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineRoleChoice() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineRoleChoice.unity");

    [MenuItem("Scene Shortcuts/Menu_OnlineServerSessionCreation", priority = 5016)]
    public static void Assets_Scenes_Menu_Scene_Assets_Menu_OnlineServerSessionCreation() => LoadScene("Assets/Scenes/Menu/Scene Assets/Menu_OnlineServerSessionCreation.unity");

    [MenuItem("Scene Shortcuts/Misc_LoadingScreen", priority = 6017)]
    public static void Assets_Scenes_Misc_Scene_Assets_Misc_LoadingScreen() => LoadScene("Assets/Scenes/Misc/Scene Assets/Misc_LoadingScreen.unity");


    private static void LoadScene(string scenePath)
    {
        if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
        {
            EditorSceneManager.OpenScene(scenePath);
        }
    }
}
