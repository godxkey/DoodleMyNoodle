using UnityEngine;

public abstract class AudioPlayable : ScriptableObject
{
    [SerializeField] float cooldown = -1;

    [System.NonSerialized]
    protected float lastPlayedTime = -1;

    public float Cooldown { get { return cooldown; } set { cooldown = value; } }

    public virtual bool IsInCooldown
    {
        get
        {
            return cooldown > 0 
                && GetTime() < lastPlayedTime + cooldown;
        }
    }

    public void PlayOn(AudioSource audioSource, float volumeMultiplier = 1)
    {
        if (IsInCooldown)
            return;

        Internal_PlayOn(audioSource, volumeMultiplier);
        lastPlayedTime = GetTime();
    }
    public void PlayOnAndIgnoreCooldown(AudioSource audioSource, float volumeMultiplier = 1)
    {
        Internal_PlayOn(audioSource, volumeMultiplier);
    }

    public void PlayLoopedOn(AudioSource audioSource, float volumeMultiplier = 1)
    {
        Interal_PlayLoopedOn(audioSource, volumeMultiplier);
    }

    protected abstract void Internal_PlayOn(AudioSource audioSource, float volumeMultiplier = 1);
    protected abstract void Interal_PlayLoopedOn(AudioSource audioSource, float volumeMultiplier = 1);

    private float GetTime()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
        {
            return Time.time;
        }
        else
        {
            return (float)UnityEditor.EditorApplication.timeSinceStartup;
        }
#else
        return Time.time;
#endif
    }
}