#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class EditorLaunchData
{
    public static bool DevelopmentBuild
    {
        get => EditorPrefs.GetBool("EditorLaunch-devBuild", true);
        set => EditorPrefs.SetBool("EditorLaunch-devBuild", value);
    }

    public static string SymbolsProfile
    {
        get => EditorPrefs.GetString("EditorLaunch-symbolsProfile", "");
        set => EditorPrefs.SetString("EditorLaunch-symbolsProfile", value);
    }

    public static bool AllowDebugging
    {
        get => EditorPrefs.GetBool("EditorLaunch-allowDebugging", true);
        set => EditorPrefs.SetBool("EditorLaunch-allowDebugging", value);
    }

    public static int ProfileLocalId
    {
        get => EditorPrefs.GetInt("EditorLaunch-profile", 0);
        set => EditorPrefs.SetInt("EditorLaunch-profile", value);
    }

    public static bool PlayFromScratch
    {
        get => EditorPrefs.GetBool("EditorLaunch-fromScratch", true);
        set => EditorPrefs.SetBool("EditorLaunch-fromScratch", value);
    }

    public static string Map
    {
        get => EditorPrefs.GetString("EditorLaunch-map");
        set => EditorPrefs.SetString("EditorLaunch-map", value);
    }

    public static string ServerName
    {
        get => EditorPrefs.GetString("EditorLaunch-serverName");
        set => EditorPrefs.SetString("EditorLaunch-serverName", value);
    }

    public static string ExtraArguments
    {
        get => EditorPrefs.GetString("EditorLaunch-extraArguments");
        set => EditorPrefs.SetString("EditorLaunch-extraArguments", value);
    }

    public static bool ServerIsHeadless
    {
        get => EditorPrefs.GetBool("EditorLaunch-serverIsHeadless", false);
        set => EditorPrefs.SetBool("EditorLaunch-serverIsHeadless", value);
    }

    public static bool PlayOnline
    {
        get => EditorPrefs.GetBool("EditorLaunch-playOnline", true);
        set => EditorPrefs.SetBool("EditorLaunch-playOnline", value);
    }

    public static int WhoIsServerId
    {
        get => EditorPrefs.GetInt("EditorLaunch-whoIsServerId", 0);
        set => EditorPrefs.SetInt("EditorLaunch-whoIsServerId", value);
    }

    public static int WhoIsEditorId
    {
        get => EditorPrefs.GetInt("EditorLaunch-whoIsEditorId", 0);
        set => EditorPrefs.SetInt("EditorLaunch-whoIsEditorId", value);
    }

    public static bool LaunchOverrideScreen
    {
        get => EditorPrefs.GetBool("EditorLaunch-launchOverrideScreen", true);
        set => EditorPrefs.SetBool("EditorLaunch-launchOverrideScreen", value);
    }

    public static bool LaunchFullscreen
    {
        get => EditorPrefs.GetBool("EditorLaunch-launchFullscreen", false);
        set => EditorPrefs.SetBool("EditorLaunch-launchFullscreen", value);
    }

    public static int LaunchScreenWidth
    {
        get => EditorPrefs.GetInt("EditorLaunch-launchScreenWidth", 1280);
        set => EditorPrefs.SetInt("EditorLaunch-launchScreenWidth", value);
    }

    public static int LaunchScreenHeight
    {
        get => EditorPrefs.GetInt("EditorLaunch-launchScreenHeight", 720);
        set => EditorPrefs.SetInt("EditorLaunch-launchScreenHeight", value);
    }
}
#endif
