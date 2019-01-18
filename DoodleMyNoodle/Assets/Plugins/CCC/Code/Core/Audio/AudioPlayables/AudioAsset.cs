using UnityEngine;

[CreateAssetMenu(menuName = "CCC/Audio/Audio Asset", fileName = "AA_Something", order = 9)]
public class AudioAsset : AudioPlayable
{
    [SerializeField] AudioClip clip;
    [SerializeField] float volume = 1;

    public AudioClip Clip { get { return clip; } set { clip = value; } }
    public float Volume { get { return volume; } set { volume = value; } }

    protected override void Internal_PlayOn(AudioSource audioSource, float volumeMultiplier = 1)
    {
        audioSource.PlayOneShot(clip, volume * volumeMultiplier);
    }

    protected override void Interal_PlayLoopedOn(AudioSource audioSource, float volumeMultiplier = 1)
    {
        audioSource.volume = volumeMultiplier * volume;
        audioSource.clip = clip;
        audioSource.loop = true;
        audioSource.Play();
    }
}
