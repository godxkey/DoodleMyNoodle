using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngineX;

[CreateAssetMenu(menuName = "DoodleMyNoodle/DoodleLibrary")]
public class DoodleLibrary : FileScriptableInterface
{
    private const string _doodleFileName = "doodle";
    private const string _extension = ".png";

    [Serializable]
    public class DoodleLibraryData
    {
        public int TotalDoodleCount = 0;
        public int LastDoodleUsedIndex = -1;
    }

    private DoodleLibraryData _data = new DoodleLibraryData();

    public Texture2D GetLastDoodle()
    {
        return GetDoodle(_data.LastDoodleUsedIndex);
    }

    public List<Texture2D> GetAllDoodles()
    {
        List<Texture2D> resultTextures = new List<Texture2D>();

        for (int i = 0; i < _data.TotalDoodleCount; i++)
        {
            resultTextures.Add(GetDoodle(i));
        }

        return resultTextures;
    }

    public void AddDoodle(Texture2D doodleTexture, bool SetAsLast = false)
    {
        _data.TotalDoodleCount++;
        if (SetAsLast)
        {
            _data.LastDoodleUsedIndex = _data.TotalDoodleCount - 1;
        }

        SaveDoodle(doodleTexture);

        Save();
    }

    public void RemoveDoodle(int index)
    {
        _data.TotalDoodleCount--;

        if (File.Exists(GetDoodlePath(index)))
        {
            try
            {
                File.Delete(GetDoodlePath(index));
            }
            catch (Exception e)
            {
                Log.Info($"Cannot delete texture at path {GetDoodlePath(index)}: {e.Message}");
            }
        }

        Save();
    }

    public void SetLastDoodle(int index)
    {
        _data.LastDoodleUsedIndex = index;
        Save();
    }

    private void SaveDoodle(Texture2D doodleTexture)
    {
        byte[] result = doodleTexture.EncodeToPNG();

        try
        {
            File.WriteAllBytes(GetDoodlePath(_data.TotalDoodleCount - 1), result);
        }
        catch (Exception e)
        {
            Debug.LogError($"Could not export and save doodle in library: {e.Message}");
        }
    }

    private Texture2D GetDoodle(int index)
    {
        Texture2D resultTexture = null;
        if (File.Exists(GetDoodlePath(index)))
        {
            try
            {
                byte[] bytes = File.ReadAllBytes(GetDoodlePath(index));
                resultTexture = new Texture2D(512, 512);
                resultTexture.LoadImage(bytes);
            }
            catch (Exception e)
            {
                Log.Info($"Cannot import texture at path {GetDoodlePath(index)}: {e.Message}");
            }
        }

        return resultTexture;
    }

    private string GetDoodlePath(int index)
    {
        string fullPath = Application.persistentDataPath;
        fullPath = fullPath.Replace('/', '\\');

        if (!fullPath.EndsWith("\\"))
            fullPath += "\\";

        fullPath += _doodleFileName + index + _extension;

        return fullPath;
    }

    protected override object GetLocalData()
    {
        return _data;
    }

    protected override void OverwriteLocalData(object graph)
    {
        _data = (DoodleLibraryData)graph;
    }

    protected override void SetDefaultLocalData()
    {
        _data = new DoodleLibraryData();
        _data.LastDoodleUsedIndex = -1;
        _data.TotalDoodleCount = 0;
    }
}