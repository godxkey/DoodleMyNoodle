using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// UNUSED

//[CreateAssetMenu(fileName = "SceneBlueprintIdInjectorData", menuName = "DoodleMyNoodle/Advanced/SceneBlueprintIdInjectorData")]
//public class SceneBlueprintIdInjectorData : ScriptableObject
//{
//    [System.Serializable]
//    public class SceneData
//    {
//        public string SceneAssetGuid;
//        public ulong NextGameObjectGuidValue;

//        public string GenerateNewGameObjectGuid()
//        {
//            return (NextGameObjectGuidValue++).ToString();
//        }
//        public bool IsGameObjectGuidValid(string gameObjectGuid)
//        {
//            if (ulong.TryParse(gameObjectGuid, out ulong result))
//            {
//                return result < NextGameObjectGuidValue;
//            }
//            else
//            {
//                return false;
//            }
//        }
//    }

//    public List<SceneData> SceneDatas = new List<SceneData>();
//}
