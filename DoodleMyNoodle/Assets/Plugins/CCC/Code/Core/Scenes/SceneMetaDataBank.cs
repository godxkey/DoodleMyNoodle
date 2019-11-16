using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISceneMetaData
{
    string Name { get; }
    string Path { get; }
    int BuildIndex { get; }
    string AssetGuid { get; }
}

public class SceneMetaDataBank : ScriptableObject
{
    [System.Serializable]
    public class SceneMetaData : ISceneMetaData
    {
        string ISceneMetaData.Name => Name;
        string ISceneMetaData.Path => Path;
        int ISceneMetaData.BuildIndex => BuildIndex;
        string ISceneMetaData.AssetGuid => AssetGuid;

        [SerializeField] internal string Name;
        [SerializeField] internal string Path;
        [SerializeField] internal int BuildIndex;
        [SerializeField] internal string AssetGuid;

        public bool ContentEquals(SceneMetaData other)
        {
            return Name == other.Name
                && Path == other.Path
                && BuildIndex == other.BuildIndex
                && AssetGuid == other.AssetGuid;
        }
    }

    [SerializeField]
    internal List<SceneMetaData> SceneMetaDatasInternal;
    public ReadOnlyList<SceneMetaData, ISceneMetaData> SceneMetaDatas => new ReadOnlyList<SceneMetaData, ISceneMetaData>(SceneMetaDatasInternal);
}
