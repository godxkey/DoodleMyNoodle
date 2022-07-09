using System;
using System.Collections.Generic;
using UnityEngine;


public class GlobalLevelBank : ScriptableObject
{
    public List<LevelDefinitionAuth> Levels = new List<LevelDefinitionAuth>();
}

//public abstract class AssetBank<T> : AssetBankBase
//{
//    public List<T> Elements = new List<T>();
//}

//public abstract class AssetBankBase : ScriptableObject
//{
//}

//public class AutoAssetBankAttribute : Attribute
//{
//    public string AssetPath;

//    public AutoAssetBankAttribute(string assetPath)
//    {
//        AssetPath = assetPath;
//    }
//}

//public class AutoAssetBankListAttribute : Attribute
//{
//}