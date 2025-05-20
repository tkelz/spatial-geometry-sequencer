using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

public class StemManager : MonoBehaviour
{
    public static StemManager Instance;

    [Header("Prefabs")]
    public GameObject stemPrefab;

    public List<StemItem> stems;

    [Header("Room Settings")]
    public AudioReverbZone audioReverbZone;
    public AudioReverbFilter audioReverbFilter;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        stems = new List<StemItem>();
    }

    public float GetMaxDuration()
    {
        float duration = 0f;
        foreach (var stem in stems)
        {
            duration = Mathf.Max(duration, stem.beadAudioSource.clip.length);
        }
        return duration;
    }

    public void DestroyAllStems()
    {
        foreach (var stem in stems)
        {
            Destroy(stem.gameObject);
        }
        stems.Clear();
    }

    public StemItem AddNewStem()
    {
        // Instantiate a stem
        GameObject newStemInstance = Instantiate(stemPrefab, Vector3.zero, Quaternion.identity);
        StemItem stemItem = newStemInstance.GetComponent<StemItem>();
        newStemInstance.name = "Stem_" + (stems.Count + 1);

        // Random color
        Color stemColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
        stemItem.SetStemColor(stemColor);

        stems.Add(stemItem);

        return stemItem;
    }

    public void RemoveStem(int index)
    {
        Destroy(stems[index].gameObject);
        stems.RemoveAt(index);
    }

    public void RestartAllStems()
    {
        foreach (var stem in stems)
        {
            stem.Restart();
        }
    }

    public void EnableAudioReverb(bool enable)
    {
        audioReverbZone.enabled = enable;
        audioReverbFilter.enabled = enable;
    }

    public void ChangeReverbPreset(AudioReverbPreset preset)
    {
        audioReverbZone.reverbPreset = preset;
        audioReverbFilter.reverbPreset = preset;
        // masterMixerGroup.audioMixer.SetFloat("Room", audioReverbZone.room);
        // masterMixerGroup.audioMixer.SetFloat("RoomHF", audioReverbZone.roomHF);
        // masterMixerGroup.audioMixer.SetFloat("RoomLF", audioReverbZone.roomLF);
        // masterMixerGroup.audioMixer.SetFloat("DecayTime", audioReverbZone.decayTime);
        // masterMixerGroup.audioMixer.SetFloat("DecayHFRatio", audioReverbZone.decayHFRatio);
        // masterMixerGroup.audioMixer.SetFloat("Reflections", audioReverbZone.reflections);
        // masterMixerGroup.audioMixer.SetFloat("ReflectionsDelay", audioReverbZone.reflectionsDelay);
        // masterMixerGroup.audioMixer.SetFloat("ReverbLevel", audioReverbZone.reverb);
        // masterMixerGroup.audioMixer.SetFloat("ReverbDelay", audioReverbZone.reverbDelay);
        // masterMixerGroup.audioMixer.SetFloat("Diffusion", audioReverbZone.diffusion);
        // masterMixerGroup.audioMixer.SetFloat("Density", audioReverbZone.density);
        // masterMixerGroup.audioMixer.SetFloat("HFReference", audioReverbZone.HFReference);
        // masterMixerGroup.audioMixer.SetFloat("LFReference", audioReverbZone.LFReference);
    }
}

