using System.Collections.Generic;
using UnityEngine;

public class StemManager : MonoBehaviour
{
    public static StemManager Instance;

    [Header("Prefabs")]
    public GameObject stemPrefab;

    public List<StemItem> stems;

    [Header("Room Settings")]
    public AudioReverbZone audioReverbZone;
    public AudioReverbFilter audioReverbFilter;

    public bool isPlaying { get; set; } = false;
    public float elapsedTime { get; set; } = 0f;
    public float maxDuration { get; set; } = 0f;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        stems = new List<StemItem>();
    }

    void Update()
    {
        if (isPlaying)
        {
            elapsedTime += Time.deltaTime;
            // if (elapsedTime >= GetMaxDuration())
            // {
            //     elapsedTime = 0f;
            //     RestartAllStems();
            // }
            StemUIManager.Instance.SetTimeSlider(elapsedTime % maxDuration / maxDuration);
        }
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

    public void Play()
    {
        OptionUIManager.Instance.EnableStemOptions(false);
        maxDuration = GetMaxDuration();
        elapsedTime = 0f;
        isPlaying = true;
        foreach (var stem in stems)
        {
            stem.StartStem();
        }
    }

    public void Pause()
    {
        OptionUIManager.Instance.EnableStemOptions(StemUIManager.Instance.GetSelectedIndex() != -1);
        StemUIManager.Instance.SetTimeSlider(0);
        isPlaying = false;
        foreach (var stem in stems)
        {
            stem.PauseStem();
        }
    }

    public void RestartAllStems()
    {
        isPlaying = true;
        foreach (var stem in stems)
        {
            stem.Restart();
        }
    }

    public void MuteOtherStems(StemItem stemItem)
    {
        foreach (var stem in stems)
        {
            stem.EnableAudio(stem == stemItem);
        }
    }

    public void UnmuteAllStems()
    {
        foreach (var stem in stems)
        {
            stem.EnableAudio(true);
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
    }

    public void SetReverbRoomSize(int size)
    {
        audioReverbZone.room = size;
        audioReverbFilter.room = size;
    }

    public void SetReverbLevel(int level)
    {
        audioReverbZone.reverb = level;
        audioReverbFilter.reverbLevel = level;
    }

    public void SetReverbDelay(float delay)
    {
        audioReverbZone.reverbDelay = delay;
        audioReverbFilter.reverbDelay = delay;
    }

    public void SetReverbReflections(int reflections)
    {
        audioReverbZone.reflections = reflections;
        audioReverbFilter.reflectionsLevel = reflections;
    }

    public void SetReverbReflectionsDelay(float delay)
    {
        audioReverbZone.reflectionsDelay = delay;
        audioReverbFilter.reflectionsDelay = delay;
    }
}

