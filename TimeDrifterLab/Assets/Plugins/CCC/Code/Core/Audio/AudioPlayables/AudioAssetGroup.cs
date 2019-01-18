using UnityEngine;
using System;

[CreateAssetMenu(menuName = "CCC/Audio/Audio Asset Group", fileName = "AAG_Something", order = 10)]
public class AudioAssetGroup : AudioPlayable
{
    [SerializeField] AudioPlayable[] clips;

    [NonSerialized]
    int lastPickedIndex = -1;

    public AudioPlayable[] Clips { get { return clips; } set { clips = value; } }

    protected override void Internal_PlayOn(AudioSource audioSource, float volumeMultiplier = 1)
    {
        if (CheckRessources() == false)
            return;

        PickAsset().PlayOn(audioSource, volumeMultiplier);
    }

    protected override void Interal_PlayLoopedOn(AudioSource audioSource, float volumeMultiplier = 1)
    {
        if (CheckRessources() == false)
            return;

        PickAsset().PlayLoopedOn(audioSource, volumeMultiplier);
    }

    private bool CheckRessources()
    {
        return clips != null && clips.Length != 0;
    }

    private AudioPlayable PickAsset()
    {
        if (lastPickedIndex >= clips.Length)
            lastPickedIndex = 0;

        if (clips.Length == 1)
            return clips[0];


        int from;
        int to;
        if (lastPickedIndex == -1)
        {
            //On a jamais pick
            from = 0;
            to = clips.Length;
        }
        else
        {
            //On a deja pick
            from = lastPickedIndex + 1;
            to = lastPickedIndex + clips.Length;
        }

        int pickedIndex = UnityEngine.Random.Range(from, to) % clips.Length;
        lastPickedIndex = pickedIndex;
        return clips[pickedIndex];
    }
}
