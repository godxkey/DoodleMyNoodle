using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CCC.Achievements
{
    public class AchievementsUtility : MonoCoreService<AchievementsUtility>
    {
        public AchievementInfo achievements;

        public override void Initialize(Action<ICoreService> onComplete)
        {
            achievements.Init(() => onComplete(this));
        }

        public void Unlock(int achievementID)
        {
#if UNITY_PS4
            UnlockPS4(FindInfo(achievementID).Ps4AchievementID);
#elif UNITY_XBOXONE
            UnlockXbox(FindInfo(achievementID).XboxAchievementID);
#elif UNITY_STANDALONE
            UnlockSteam(FindInfo(achievementID).SteamAchievementID);
#endif
        }

        AchievementInfo.Achievements FindInfo(int achievementID)
        {
            for (int i = 0; i < achievements.achievementList.Count; i++)
            {
                if (achievements.achievementList[i].achievementID == achievementID)
                    return achievements.achievementList[i];
            }
            Debug.LogError("Achievement Error : No Achievement Found with this ID");
            return null;
        }

        // https://www.salusgames.com/2017/11/05/integrating-steamworks-and-achievements-into-unity3d/
        public void UnlockSteam(string achievementID)
        {
#if UNITY_STANDALONE

#endif
        }

        public void UnlockPS4()
        {
#if UNITY_PS4

#endif
        }

        public void UnlockXbox()
        {
#if UNITY_XBOXONE

#endif
        }
    }

    [Serializable,CreateAssetMenu(menuName = "CCC/Achievements/New Achievements Info")]
    public class AchievementInfo : ScriptableObject
    {
        // INIT FLAGS

        public const string FlagKey = "SAVE_FlagKey_";

        public void Init(Action onComplete)
        {
            achievementFlagSaver.LoadAsync(() => {
                for (int i = 0; i < achievementFlagList.Count; i++)
                {
                    achievementFlagList[i].flagState = achievementFlagSaver.GetBool(FlagKey + achievementFlagList[i].flagID);
                }
                onComplete();
            });
        }

        // UNLOCK ACHIEVEMENTS INFO

        [Serializable]
        public class Achievements
        {
            public string nameIdentifier;
            public int achievementID;
            public string SteamAchievementID;
            public string Ps4AchievementID;
            public string XboxAchievementID;
        }

        public List<Achievements> achievementList = new List<Achievements>();

        // UNLOCK FLAGS INFO

        public DataSaver achievementFlagSaver;

        [Serializable]
        public class AchievementsFlag
        {
            public string nameIdentifier;
            public int flagID;
            public bool flagState;
        }

        public List<AchievementsFlag> achievementFlagList = new List<AchievementsFlag>();

        public void ChangeFlagState(int flagID, bool newFlagState)
        {
            for (int i = 0; i < achievementFlagList.Count; i++)
            {
                if(achievementFlagList[i].flagID == flagID)
                {
                    achievementFlagList[i].flagState = newFlagState;
                    break;
                }
            }
            achievementFlagSaver.Save();
        }
    }
}
