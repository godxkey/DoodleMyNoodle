using System.Collections.Generic;
using System.IO;
using Unity.Serialization.Json;
using UnityEngine;
using UnityEngineX;

public class PlayerProfileService : MonoCoreService<PlayerProfileService>
{
    static PlayerProfile[] s_hardcodedProfiles = new PlayerProfile[]
    {
        new PlayerProfile() { playerName = "John Pogo", localId = 0}
        , new PlayerProfile() { playerName = "Gadget Garcon", localId = 1}
        , new PlayerProfile() { playerName = "Paul le Fauve", localId = 2}
    };

    PlayerProfile _currentProfile;
    static string ProfileSavePath => Application.persistentDataPath + "/PlayerProfiles";



    public PlayerProfileReadOnly CurrentProfile => _currentProfile;
    public string PlayerName => _currentProfile.playerName;
    public event System.Action OnChangeProfile;

    public void SetPlayerProfile(int localId)
    {
        _currentProfile = s_hardcodedProfiles[localId];
        OnChangeProfile?.Invoke();
    }

    public override void Initialize(System.Action<ICoreService> onComplete)
    {
        _currentProfile = s_hardcodedProfiles[0];

        onComplete(this);
    }

    public static List<PlayerProfile> LoadProfilesOnDisk()
    {
        return new List<PlayerProfile>(s_hardcodedProfiles);
        /*
        if (Directory.Exists(profileSavePath))
        {
            List<PlayerProfile> profiles = new List<PlayerProfile>();

            foreach (string filePath in Directory.EnumerateFiles(profileSavePath, '*' + PlayerProfile.fileNameExtension, SearchOption.TopDirectoryOnly))
            {
                PlayerProfile profile = LoadProfileFromDisk(filePath);
                if (profile != null)
                    profiles.Add(profile);
            }

            return profiles;
        }
        else
        {
            return new List<PlayerProfile>();
        }*/
    }

    public static PlayerProfile LoadProfileFromDisk(string filePath)
    {
        StreamReader file = File.OpenText(filePath);
        
        PlayerProfile profile = null;

        try
        {
            profile = JsonSerialization.FromJson<PlayerProfile>(file.ReadToEnd());

        }
        catch { }

        if (profile == null)
        {
            Log.Error("Failed to deserialize player profile: " + filePath);
        }

        return profile;
    }

    private static void SaveProfileToDisk(PlayerProfile playerProfile)
    {
        string filePath = ProfileSavePath + '/' + playerProfile.GenerateFileName();

        //File.WriteAllText(filePath, JsonConvert.SerializeObject(playerProfile));

        //StreamWriter file = null;
        //if (File.Exists(filePath))
        //{
        //    file = (StreamWriter)File.OpenWrite(filePath);
        //}
        //else
        //{

        //}
        //// serialize JSON directly to a file
        using (StreamWriter file = File.CreateText(filePath))
        {
            file.Write(JsonSerialization.ToJson(playerProfile));
        }
    }

#if UNITY_EDITOR
    public class EditorMethods
    {
        public static void SaveProfileToDisk(PlayerProfile playerProfile) => PlayerProfileService.SaveProfileToDisk(playerProfile);
    }

#endif
}
