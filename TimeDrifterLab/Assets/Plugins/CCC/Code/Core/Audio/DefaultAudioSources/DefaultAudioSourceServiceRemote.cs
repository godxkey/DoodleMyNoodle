using UnityEngine;

namespace CCC.Hidden
{
    public class DefaultAudioSourceServiceRemote : MonoBehaviour
    {
        public void PlaySFX_AudioClip(AudioClip clip)
        {
            DefaultAudioSourceService.Instance.PlaySFX(clip);
        }
        public void PlaySFX_AudioPlayable(AudioPlayable playable)
        {
            DefaultAudioSourceService.Instance.PlaySFX(playable);
        }
        public void PlayVoice_AudioClip(AudioClip clip)
        {
            DefaultAudioSourceService.Instance.PlayVoice(clip);
        }
        public void PlayVoice_AudioPlayable(AudioPlayable playable)
        {
            DefaultAudioSourceService.Instance.PlayVoice(playable);
        }
        public void PlayStaticSFX_AudioClip(AudioClip clip)
        {
            DefaultAudioSourceService.Instance.PlayStaticSFX(clip);
        }
        public void PlayStaticSFX_AudioPlayable(AudioPlayable playable)
        {
            DefaultAudioSourceService.Instance.PlayStaticSFX(playable);
        }
        public void PlayMusic_AudioClip(AudioClip clip)
        {
            DefaultAudioSourceService.Instance.PlayMusic(clip);
        }
        public void PlayMusic_AudioPlayable(AudioPlayable playable)
        {
            DefaultAudioSourceService.Instance.PlayMusic(playable);
        }
    }
}