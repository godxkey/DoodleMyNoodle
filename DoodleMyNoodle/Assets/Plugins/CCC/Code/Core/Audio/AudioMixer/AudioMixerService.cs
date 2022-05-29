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

    [ConsoleCommand(Description = "Change Volume of the Music")]
    static public void SetMusicVolume(float Volume)
    {
        if (Instance == null)
            return;

        Instance.mixerSaver.SetVolume(AudioMixerSaver.ChannelType.Music, Volume);
        Instance.mixerSaver.Save();
    }

    [ConsoleCommand(Description = "Toggle Mute Music Volume")]
    static public void ToggleMuteMusicVolume()
    {
        if (Instance == null)
            return;

        Instance.mixerSaver.SetMuted(AudioMixerSaver.ChannelType.Music, !Instance.mixerSaver.GetMuted(AudioMixerSaver.ChannelType.Music));
        Instance.mixerSaver.Save();
    }

    [ConsoleCommand(Description = "Change Volume of the SFX")]
    static public void SetSFXVolume(float Volume)
    {
        if (Instance == null)
            return;

        Instance.mixerSaver.SetVolume(AudioMixerSaver.ChannelType.Music, Volume);
        Instance.mixerSaver.Save();
    }

    [ConsoleCommand(Description = "Toggle Mute SFX Volume")]
    static public void ToggleMuteSFXVolume()
    {
        if (Instance == null)
            return;

        Instance.mixerSaver.SetMuted(AudioMixerSaver.ChannelType.SFX, !Instance.mixerSaver.GetMuted(AudioMixerSaver.ChannelType.SFX));
        Instance.mixerSaver.Save();
    }

    [ConsoleCommand(Description = "Set All Volume (Master)")]
    static public void SetAllVolume(float Volume)
    {
        if (Instance == null)
            return;

        Instance.mixerSaver.SetVolume(AudioMixerSaver.ChannelType.Master, Volume);
        Instance.mixerSaver.Save();
    }

    [ConsoleCommand(Description = "Toggle Mute All Volume (Master)")]
    static public void ToggleMuteAll()
    {
        if (Instance == null)
            return;

        Instance.mixerSaver.SetMuted(AudioMixerSaver.ChannelType.Master, !Instance.mixerSaver.GetMuted(AudioMixerSaver.ChannelType.Master));
        Instance.mixerSaver.Save();
    }
}
