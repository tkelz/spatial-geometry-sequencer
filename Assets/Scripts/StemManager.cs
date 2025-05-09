using System.Collections.Generic;
using UnityEngine;

public class StemManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject beadPrefab;
    public List<GameObject> pathPrefabs; // List of available path shapes

    [Header("Settings")]
    public float bpm = 120f;

    private List<Stem> activeStems = new List<Stem>();

    void Start()
    {
        // For now, create a first test stem
        AddNewStem();
    }

    public void AddNewStem()
    {
        if (pathPrefabs.Count == 0)
        {
            Debug.LogWarning("No path prefabs assigned!");
            return;
        }

        // Instantiate a random path
        GameObject pathInstance = Instantiate(pathPrefabs[Random.Range(0, pathPrefabs.Count)], Vector3.zero, Quaternion.identity);

        // Instantiate a bead
        GameObject beadInstance = Instantiate(beadPrefab, pathInstance.transform);

        // Random color
        Color beadColor = Random.ColorHSV(0f, 1f, 0.8f, 1f, 0.8f, 1f);
        beadInstance.GetComponentInChildren<Renderer>().material.color = beadColor;

        // Add AudioSource if missing
        AudioSource audio = beadInstance.GetComponent<AudioSource>();
        if (audio == null)
            audio = beadInstance.AddComponent<AudioSource>();

        // TEMP: Pick a random audio clip from Resources
        AudioClip clip = Resources.Load<AudioClip>("example_audio"); // Later, replace with user-chosen file
        if (clip != null)
        {
            audio.clip = clip;
            audio.spatialBlend = 1f; // 3D Sound
            audio.loop = true;
            audio.Play();
        }

        // Create a stem object to manage it
        Stem newStem = new Stem
        {
            bead = beadInstance,
            path = pathInstance,
            audioSource = audio
        };

        activeStems.Add(newStem);
    }

    void Update()
    {
        foreach (Stem stem in activeStems)
        {
            if (stem.path == null || stem.bead == null) continue;

            float speed = bpm / 60f; // Units per second (simple for now)

            // Move along the path (basic rotation around center as placeholder)
            stem.bead.transform.RotateAround(Vector3.zero, Vector3.up, speed * Time.deltaTime * 20f);
        }
    }

    [System.Serializable]
    public class Stem
    {
        public GameObject bead;
        public GameObject path;
        public AudioSource audioSource;
    }
}

