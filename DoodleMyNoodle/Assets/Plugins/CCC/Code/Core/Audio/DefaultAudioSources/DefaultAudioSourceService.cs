using UnityEngine;
using System.Collections;
using DG.Tweening;
using System;
using System.Collections.Generic;

public class DefaultAudioSourceService : MonoCoreService<DefaultAudioSourceService>
{
    [SerializeField] AudioSource SFXSource;
    [SerializeField] AudioSource staticSFXSource;
    [SerializeField] AudioSource musicSource1;
    [SerializeField] AudioSource musicSource0;
    [SerializeField] AudioSource voiceSource;
    [SerializeField] DG.Tweening.Ease audioFadeEase = Ease.InOutSine;

    int currentMusicSource = 1;
    List<Coroutine> musicTransitionCalls = new List<Coroutine>();
    List<Tween> musicTransitionTweens = new List<Tween>();

    public override void Initialize(Action<ICoreService> onComplete)
    {
        onComplete(this);
    }

    /// <summary>
    /// Plays the audioplayable. Leave source to 'null' to play on the standard 2D SFX audiosource.
    /// </summary>
    public void PlayStaticSFX(AudioPlayable playable, float delay = 0, float volumeMultiplier = 1, AudioSource source = null)
    {
        PlayNonMusic(playable, delay, volumeMultiplier, source, staticSFXSource);
    }
    /// <summary>
    /// Plays the audioclip. Leave source to 'null' to play on the standard 2D SFX audiosource.
    /// </summary>
    public void PlayStaticSFX(AudioClip clip, float delay = 0, float volume = 1, AudioSource source = null)
    {
        PlayNonMusic(clip, delay, volume, source, staticSFXSource);
    }

    /// <summary>
    /// Plays the audioplayable. Leave source to 'null' to play on the standard 2D Voice audiosource.
    /// </summary>
    public void PlayVoice(AudioPlayable playable, float delay = 0, float volumeMultiplier = 1, AudioSource source = null)
    {
        PlayNonMusic(playable, delay, volumeMultiplier, source, voiceSource);
    }
    /// <summary>
    /// Plays the audioclip. Leave source to 'null' to play on the standard 2D Voice audiosource.
    /// </summary>
    public void PlayVoice(AudioClip clip, float delay = 0, float volume = 1, AudioSource source = null)
    {
        PlayNonMusic(clip, delay, volume, source, voiceSource);
    }

    /// <summary>
    /// Plays the audioplayable. Leave source to 'null' to play on the standard 2D SFX audiosource.
    /// </summary>
    public void PlaySFX(AudioPlayable playable, float delay = 0, float volumeMultiplier = 1, AudioSource source = null)
    {
        PlayNonMusic(playable, delay, volumeMultiplier, source, SFXSource);
    }
    /// <summary>
    /// Plays the audioclip. Leave source to 'null' to play on the standard 2D SFX audiosource.
    /// </summary>
    public void PlaySFX(AudioClip clip, float delay = 0, float volume = 1, AudioSource source = null)
    {
        PlayNonMusic(clip, delay, volume, source, SFXSource);
    }

    private void PlayNonMusic(AudioPlayable playable, float delay, float volumeMultiplier, AudioSource source, AudioSource defaultSource)
    {
        if (playable == null)
            return;

        AudioSource selectedSource = source;
        if (selectedSource == null)
            selectedSource = defaultSource;

        if (delay > 0)
        {
            StartCoroutine(PlayNonMusicIn(playable, delay, volumeMultiplier, selectedSource));
            return;
        }
        else
        {
            playable.PlayOn(selectedSource, volumeMultiplier);
        }
    }
    private void PlayNonMusic(AudioClip clip, float delay, float volume, AudioSource source, AudioSource defaultSource)
    {
        if (clip == null)
            return;

        AudioSource selectedSource = source;
        if (selectedSource == null)
            selectedSource = defaultSource;

        if (delay > 0)
        {
            StartCoroutine(PlayNonMusicIn(clip, delay, volume, selectedSource));
            return;
        }
        else
        {
            selectedSource.PlayOneShot(clip, volume);
        }
    }

    private IEnumerator PlayNonMusicIn(AudioPlayable playable, float delay, float volumeMultiplier, AudioSource source)
    {
        yield return new WaitForSecondsRealtime(delay);
        playable.PlayOn(source, volumeMultiplier);
    }
    private IEnumerator PlayNonMusicIn(AudioClip clip, float delay, float volume, AudioSource source)
    {
        yield return new WaitForSecondsRealtime(delay);
        source.PlayOneShot(clip, volume);
    }

    #region Music
    /// <returns>The standard source volume</returns>
    private float Internal_PlayMusic(AudioPlayable playable, bool looping, float volumeMultiplier = 1)
    {
        if (!CheckResources_MusicSource() || playable == null)
            return 0;

        AudioSource source = GetAndIncrementMusicSource();
        source.volume = 1;

        if (looping)
            playable.PlayLoopedOn(source, 1);
        else
            playable.PlayOn(source);

        float stdVolume = source.volume;
        source.volume *= volumeMultiplier;
        return stdVolume;
    }
    private void Internal_PlayMusic(AudioClip clip, bool looping, float volume)
    {
        if (!CheckResources_MusicSource() || clip == null)
            return;

        AudioSource source = GetAndIncrementMusicSource();

        source.volume = volume;
        source.clip = clip;
        source.loop = looping;
        source.Play();
    }

    private void Internal_PlayMusicFaded(AudioPlayable playable, float fadeInDuration, bool looping = true, float startingVolume = 0)
    {
        if (!CheckResources_MusicSource())
            return;

        float stdVolume = Internal_PlayMusic(playable, looping, startingVolume);
        musicTransitionTweens.Add(
            GetMusicSource().DOFade(stdVolume, fadeInDuration).SetEase(audioFadeEase));
    }
    private void Internal_PlayMusicFaded(AudioClip clip, float fadeInDuration, bool looping = true, float volume = 1, float startingVolume = 0)
    {
        if (!CheckResources_MusicSource())
            return;

        Internal_PlayMusic(clip, looping, startingVolume);
        musicTransitionTweens.Add(
            GetMusicSource().DOFade(volume, fadeInDuration).SetEase(audioFadeEase));
    }

    private AudioSource GetAndIncrementMusicSource()
    {
        currentMusicSource++;
        currentMusicSource %= 2;
        return GetMusicSource();
    }
    private AudioSource GetMusicSource()
    {
        return currentMusicSource == 0 ? musicSource0 : musicSource1;
    }
    private AudioSource GetOtherMusicSource()
    {
        return currentMusicSource == 0 ? musicSource1 : musicSource0;
    }

    private void CancelMusicTransitionCalls()
    {
        List<Coroutine> routine = musicTransitionCalls;
        for (int i = 0; i < routine.Count; i++)
        {
            StopCoroutine(routine[i]);
        }
        routine.Clear();

        List<Tween> tweens = musicTransitionTweens;
        for (int i = 0; i < tweens.Count; i++)
        {
            tweens[i].Kill();
        }
        tweens.Clear();
    }
    private void StopSource(AudioSource source)
    {
        source.Stop();
    }
    private void StopSourceFaded(AudioSource source, float fadeDuration, Action onComplete)
    {
        if (fadeDuration > 0 && IsPlayingMusic())
        {
            musicTransitionTweens.Add(
                source.DOFade(0, fadeDuration).OnComplete(delegate ()
                {
                    StopSource(source);
                    onComplete?.Invoke();
                }).SetEase(audioFadeEase));
        }
        else
        {
            StopSource(source);
            onComplete?.Invoke();
        }
    }


    public void PlayMusic(AudioPlayable playable, bool looping = true)
    {
        StopMusic();
        Internal_PlayMusic(playable, looping);
    }
    public void PlayMusic(AudioClip clip, bool looping = true, float volume = 1)
    {
        StopMusic();
        Internal_PlayMusic(clip, looping, volume);
    }

    private void Internal_TransitionToMusic(Action playMusicFaded, float fadingDuration = 1.5f, float overlap = 0.5f)
    {
        if (!CheckResources_MusicSource())
            return;

        CancelMusicTransitionCalls();

        if (IsPlayingMusic())
        {
            AudioSource firstSource = GetMusicSource();
            float end1stMusicDelay = overlap < 0.5f ? 0 : (overlap - 0.5f) * 2 * fadingDuration;
            float start2ndMusicDelay = overlap > 0.5f ? 0 : (0.5f - overlap) * 2 * fadingDuration;

            musicTransitionCalls.Add(
                this.DelayedCall(end1stMusicDelay,
                    () => StopSourceFaded(firstSource, fadingDuration, null)));
            musicTransitionCalls.Add(
                this.DelayedCall(start2ndMusicDelay, playMusicFaded));
        }
        else
        {
            playMusicFaded();
        }
    }

    /// <summary>
    /// Transitionne vers une nouvelle musique
    /// </summary>
    /// <param name="clip">Le clip a faire jouer</param>
    /// <param name="looping">Est-ce qu'on loop</param>
    /// <param name="volume">Le volume de la 2e musique</param>
    /// <param name="fadingDuration">La duree de fading par musique. ATTENTION, ceci n'est pas necessairement == duree total de la transition</param>
    /// <param name="overlap">L'overlapping des deux musiques en transition (en %). 0 = la 1ere stoppe, puis la 2e commence.   0.5 = les 2 tansitionne en meme temps   1 = la deuxieme commence, puis la 1ere stoppe</param>
    /// <param name="startingVolume">Volume initiale de la 2e musique</param>
    public void TransitionToMusic(AudioPlayable playable, bool looping = true, float fadingDuration = 1.5f, float overlap = 0.5f, float startingVolume = 0)
    {
        Action action = () => Internal_PlayMusicFaded(playable, fadingDuration, looping, startingVolume);
        Internal_TransitionToMusic(action, fadingDuration, overlap);
    }
    /// <summary>
    /// Transitionne vers une nouvelle musique
    /// </summary>
    /// <param name="clip">Le clip a faire jouer</param>
    /// <param name="looping">Est-ce qu'on loop</param>
    /// <param name="volume">Le volume de la 2e musique</param>
    /// <param name="fadingDuration">La duree de fading par musique. ATTENTION, ceci n'est pas necessairement == duree total de la transition</param>
    /// <param name="overlap">L'overlapping des deux musiques en transition (en %). 0 = la 1ere stoppe, puis la 2e commence.   0.5 = les 2 tansitionne en meme temps   1 = la deuxieme commence, puis la 1ere stoppe</param>
    /// <param name="startingVolume">Volume initiale de la 2e musique</param>
    public void TransitionToMusic(AudioClip clip, bool looping = true, float volume = 1, float fadingDuration = 1.5f, float overlap = 0.5f, float startingVolume = 0)
    {
        Action action = () => Internal_PlayMusicFaded(clip, fadingDuration, looping, volume, startingVolume);
        Internal_TransitionToMusic(action, fadingDuration, overlap);
    }

    /// <summary>
    /// Stop la musique en cours.
    /// </summary>
    public void StopMusic()
    {
        if (!CheckResources_MusicSource())
            return;

        CancelMusicTransitionCalls();

        StopSource(GetMusicSource());
    }

    /// <summary>
    /// Stop la musique en cours avec un fadeout.
    /// </summary>
    public void StopMusicFaded(float fadeDuration = 1.5f, Action onComplete = null)
    {
        if (!CheckResources_MusicSource())
            return;

        CancelMusicTransitionCalls();

        StopSourceFaded(GetMusicSource(), fadeDuration, onComplete);
    }

    public bool IsPlayingMusic()
    {
        if (!CheckResources_MusicSource())
            return false;

        return GetMusicSource().isPlaying;
    }
    #endregion

    #region Check Resources
    private bool CheckResources_MusicSource()
    {
        if (musicSource0 == null || musicSource1 == null)
        {
            DebugService.LogError("Il manque 1 ou 2 AudioSource de musique sur l'instance de SoundManager");
            return false;
        }

        return true;
    }
    private bool CheckResources_VoiceSource()
    {
        if (voiceSource == null)
        {
            DebugService.LogError("Aucune 'Voice' AudioSource sur l'instance de SoundManager");
            return false;
        }

        return true;
    }
    private bool CheckResources_SFXSource()
    {
        if (SFXSource == null)
        {
            DebugService.LogError("Aucune 'SFX' AudioSource sur l'instance de SoundManager");
            return false;
        }

        return true;
    }
    #endregion
}
