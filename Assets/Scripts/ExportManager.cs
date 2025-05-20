using UnityEngine;
using System.IO;
using System.Collections.Generic;
using SFB;

[RequireComponent(typeof(AudioListener))]
public class ExportManager : MonoBehaviour
{
    public static ExportManager Instance;

    float audioSeconds = 10;
    List<float> samples;
    bool isRecording = false;
    int channels = 2;
    int sampleRate;
    AudioClip recordedClip;
    float recordedTime;

    void Awake()
    {
        Instance = this;
    }

    // Use this for initialization
    void Start()
    {
        sampleRate = AudioSettings.outputSampleRate;
        samples = new List<float>();
    }

    void OnAudioFilterRead(float[] data, int channels)
    {
        if (!isRecording) return;

        // Store samples
        samples.AddRange(data);
        this.channels = channels;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isRecording) return;

        if (recordedTime > audioSeconds)
        {
            isRecording = false;
            GetAudioClip();
        }
        else
        {
            recordedTime += Time.deltaTime;
            OptionUIManager.Instance.SetRecordProgressBar(recordedTime * 100 / audioSeconds);
        }
    }

    public void StartRecording()
    {
        recordedTime = 0;
        audioSeconds = StemManager.Instance.GetMaxDuration();
        isRecording = true;
        samples.Clear();
        OptionUIManager.Instance.EnableExportOptions(false);
        StemManager.Instance.RestartAllStems();
    }

    public void GetAudioClip()
    {
        if (samples.Count == 0) return;

        int sampleCount = samples.Count / channels;
        float[] data = samples.ToArray();

        recordedClip = AudioClip.Create("CapturedClip", sampleCount, channels, sampleRate, false);
        recordedClip.SetData(data, 0);

        ExportAudio();
        OptionUIManager.Instance.EnableExportOptions(true);
    }

    public void ExportAudio()
    {
        var path = StandaloneFileBrowser.SaveFilePanel("Export Audio", "", "output", "wav");
        if (!string.IsNullOrEmpty(path))
        {
            SaveWav(path);
        }
    }

    public void SaveWav(string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        int sampleCount = samples.Count;
        int channels = 2; // assuming stereo

        FileStream fileStream = new FileStream(path, FileMode.Create);
        BinaryWriter writer = new BinaryWriter(fileStream);

        // Write WAV header
        writer.Write(System.Text.Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(0); // Placeholder for file size
        writer.Write(System.Text.Encoding.ASCII.GetBytes("WAVE"));
        writer.Write(System.Text.Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16); // Subchunk1Size
        writer.Write((short)1); // AudioFormat
        writer.Write((short)channels);
        writer.Write(sampleRate);
        writer.Write(sampleRate * channels * 2); // ByteRate
        writer.Write((short)(channels * 2)); // BlockAlign
        writer.Write((short)16); // BitsPerSample

        writer.Write(System.Text.Encoding.ASCII.GetBytes("data"));
        writer.Write(sampleCount * 2); // Subchunk2Size

        // Convert and write audio data
        foreach (var sample in samples)
        {
            short intData = (short)(sample * short.MaxValue);
            writer.Write(intData);
        }

        // Finalize file size
        fileStream.Seek(4, SeekOrigin.Begin);
        writer.Write((int)(fileStream.Length - 8));

        writer.Close();
        Debug.Log("Saved to: " + path);
    }
}
