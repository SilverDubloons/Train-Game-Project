using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] private AudioClip[] tickSounds;
    [SerializeField] private AudioClip[] cardPickupSounds;
    [SerializeField] private AudioClip[] cardDropSounds;
    [SerializeField] private AudioClip[] cardSlideSounds;
    [SerializeField] private AudioClip[] chipSounds;

    [SerializeField] private AudioSource audioSouce;
    [SerializeField] private AudioSource tickSource;
    private Dictionary<string, float> soundLastTimePlayed = new Dictionary<string, float>();
    private Dictionary<string, int> soundIndices = new Dictionary<string, int>();

    private readonly float minSoundInterval = 0.1f;
    private readonly float timeToResetIncrements = 2f;
    public static SoundManager instance;
    void Awake()
    {
        instance = this;
    }
    private void PlaySound(AudioClip[] clips, float volumeFactor = 1f, string soundKey = null, bool randomize = false, bool increment = false)
    {
        if (randomize)
        {
            PlaySound(clips[UnityEngine.Random.Range(0, clips.Length)], volumeFactor, soundKey);
        }
        else if (increment)
        {
            if (soundLastTimePlayed.ContainsKey(soundKey))
            {
                if (Time.time - soundLastTimePlayed[soundKey] > timeToResetIncrements / Preferences.instance.gameSpeed)
                {
                    soundIndices[soundKey] = 0;
                }
            }
            else
            {
                soundIndices[soundKey] = 0;
            }
            PlaySound(clips[Mathf.Min(soundIndices[soundKey], clips.Length - 1)], volumeFactor, soundKey);
            soundIndices[soundKey]++;
        }
    }
    private void PlaySound(AudioClip clip, float volumeFactor = 1f, string soundKey = null)
    {
        if(!Preferences.instance.soundOn)
        {
            return;
        }
        if (Preferences.instance.soundVolume <= 0f)
        {
            return;
        }
        if(Preferences.instance.muteOnFocusLost && !Application.isFocused)
        {
            return;
        }
        if(soundKey != null)
        {
            if(soundLastTimePlayed.ContainsKey(soundKey))
            {
                if (Time.time - soundLastTimePlayed[soundKey] < minSoundInterval)
                {
                    return;
                }
            }
            soundLastTimePlayed[soundKey] = Time.time;
        }
        audioSouce.PlayOneShot(clip, Preferences.instance.soundVolume * volumeFactor);
    }
    public void PlayTickSound()
    {
        if (!Preferences.instance.soundOn)
        {
            return;
        }
        if (Preferences.instance.soundVolume <= 0f)
        {
            return;
        }
        if (Preferences.instance.muteOnFocusLost && !Application.isFocused)
        {
            return;
        }
        if (soundLastTimePlayed.ContainsKey("tick"))
        {
            if (Time.time - soundLastTimePlayed["tick"] > 0.5f / Preferences.instance.gameSpeed)
            {
                soundIndices["tick"] = 0;
            }
        }
        else
        {
            soundIndices["tick"] = 0;
        }
        soundLastTimePlayed["tick"] = Time.time;
        tickSource.pitch = 1f + 0.05f * soundIndices["tick"];
        tickSource.PlayOneShot(tickSounds[UnityEngine.Random.Range(0, tickSounds.Length)], 0.125f);
        soundIndices["tick"]++;
    }
    public void PlayCardPickupSound()
    {
        PlaySound(cardPickupSounds, 0.5f, "cardPickup", randomize: true);
    }
}
