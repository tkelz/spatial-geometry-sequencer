using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("Audio Settings")]
    public AudioReverbZone audioReverbZone;

    public void EnableAudioReverb(bool enable)
    {
        audioReverbZone.enabled = enable;
    }

    public void ChangeReverbPreset(AudioReverbPreset preset)
    {
        audioReverbZone.reverbPreset = preset;
    }
}
