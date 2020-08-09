using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UnityEngine;
using UnityEngineX;

public class DebugPanelPlayerAssets : DebugPanel
{
    public override string Title => "Player Assets";

    public override bool CanBeDisplayed => PlayerAssetManager.Instance != null;

    private Vector2 _scrollPosition;

    public override void OnGUI()
    {
        ReadOnlyDictionary<Guid, PlayerAsset> assets = PlayerAssetManager.Instance.GetAssets();

        GUIContent texture = new GUIContent();

        _scrollPosition = GUILayout.BeginScrollView(_scrollPosition, GUILayout.MaxHeight(200));

        foreach (PlayerAsset asset in assets.Values)
        {
            GUILayout.BeginHorizontal();

            if (asset is PlayerDoodleAsset doodleAsset)
            {
                texture.image = doodleAsset.Texture;
                GUILayout.Box(texture, GUILayout.Width(64), GUILayout.Height(64));
            }

            GUILayout.BeginVertical();
            GUILayout.Label($"{asset.Guid}");
            GUILayout.Label($"{asset.Type} - {asset.Author}");
            GUILayout.EndVertical();

            GUILayout.EndHorizontal();
        }

        GUILayout.EndScrollView();
    }
}