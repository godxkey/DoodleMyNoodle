using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] private AudioPlayable _music; // global for now but can be per character/ennemy

    public void Start()
    {
        if (_music == null)
            return;

        DefaultAudioSourceService.Instance.PlayMusic(_music);
    }
}