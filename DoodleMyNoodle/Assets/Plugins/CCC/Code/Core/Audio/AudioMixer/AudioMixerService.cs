using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerService : MonoCoreService<AudioMixerService>
{
    [SerializeField] AudioMixerSaver mixerSaver;

    [Tooltip("If set to false, we will do a normal InstantLoad() on start")]
    [SerializeField] bool loadAsyncOnStart;

    public AudioMixer AudioMixer { get { return mixerSaver.AudioMixer; } }
    public AudioMixerSaver AudioMixerSaver { get { return mixerSaver; } }

    public override void Initialize(Action<ICoreService> onComplete)
    {
        if (loadAsyncOnStart)
        {
            mixerSaver.LoadAsync(()=> onComplete(this));
        }
        else
        {
            mixerSaver.Load(() => onComplete(this));
        }
    }
}
