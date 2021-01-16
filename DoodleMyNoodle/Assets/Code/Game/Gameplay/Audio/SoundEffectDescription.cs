using CCC.InspectorDisplay;
using System;
using UnityEngine;
using UnityEngineX;

public class SoundEffectDescription : GameMonoBehaviour
{
    [SerializeField] private bool _useRandomizeSoundGroup;

    [HideIf("_useRandomizeSoundGroup")]
    [SerializeField] private AudioPlayable _soundEffect;
    [ShowIf("_useRandomizeSoundGroup")]
    [SerializeField] private AudioAssetGroup _soundEffectGroup;

    public AudioPlayable SoundEffect 
    { get 
        {
            if (_useRandomizeSoundGroup)
            {
                return _soundEffectGroup;
            }
            else
            {
                return _soundEffect;
            }
        } 
    }
}