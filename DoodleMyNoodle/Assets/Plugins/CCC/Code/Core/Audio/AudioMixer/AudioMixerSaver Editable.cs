using System.Collections.Generic;

public partial class AudioMixerSaver
{
    public enum ChannelType
    {
        Master,
        //Voice,
        SFX,
        Music
    }
    private Dictionary<ChannelType, Channel> channels = new Dictionary<ChannelType, Channel>()
    {
        {ChannelType.Master, new Channel("MasterVolume") },
        //{ChannelType.Voice, new Channel("VoiceVolume") },
        {ChannelType.SFX, new Channel("SFXVolume", "StaticSFXVolume") },
        {ChannelType.Music, new Channel("MusicVolume") }
    };

    private void DefaultSettings()
    {
        foreach (var pair in channels)
        {
            var data = pair.Value.data;
            data.dbBoost = 0;
            data.muted = false;
        }
    }
}

