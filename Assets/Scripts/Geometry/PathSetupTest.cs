using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class PathSetupTest : MonoBehaviour
{
    public int points = 64;
    public float radius = 2f;
    [Range(0.1f, 4f)]
    public float cyclesPerSecond = 0.5f;

    private Vector3[] pathPositions;
    private LineRenderer lineRenderer;
    private GameObject bead;
    private AudioSource audioSource;
    private float t;

    void Start()
    {
        var listener = Camera.main;
        if (listener == null)
        {
            var go = new GameObject("Main Camera");
            listener = go.AddComponent<Camera>();
        }
        if (listener.GetComponent<AudioListener>() == null)
            listener.gameObject.AddComponent<AudioListener>();

        BuildCircularPath();
        CreateBead();
        SetupAudio();
    }

    void Update()
    {
        if (pathPositions == null || bead == null) return;

        t = (t + Time.deltaTime * cyclesPerSecond) % 1f;
        float f = t * points;
        int i0 = Mathf.FloorToInt(f) % points;
        int i1 = (i0 + 1) % points;
        float frac = f - i0;

        bead.transform.position = Vector3.Lerp(pathPositions[i0], pathPositions[i1], frac);
    }

    void BuildCircularPath()
    {
        var pathGO = new GameObject("Path_Circle");
        lineRenderer = pathGO.AddComponent<LineRenderer>();
        lineRenderer.positionCount = points;
        lineRenderer.widthMultiplier = 0.05f;
        lineRenderer.useWorldSpace = true;
        lineRenderer.loop = true;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Unlit"));
        mat.SetColor("_BaseColor", Color.cyan);
        lineRenderer.material = mat;

        pathPositions = new Vector3[points];
        for (int i = 0; i < points; i++)
        {
            float ang = (2 * Mathf.PI / points) * i;
            pathPositions[i] = new Vector3(Mathf.Cos(ang), 0, Mathf.Sin(ang)) * radius;
            lineRenderer.SetPosition(i, pathPositions[i]);
        }
    }

    void CreateBead()
    {
        bead = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bead.name = "Bead";
        bead.transform.localScale = Vector3.one * 0.2f;

        var mat = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        mat.SetColor("_BaseColor", Color.white);
        mat.EnableKeyword("_EMISSION");
        mat.SetColor("_EmissionColor", Color.cyan * 2f);
        bead.GetComponent<Renderer>().material = mat;

        bead.AddComponent<AudioSource>();
    }

    void SetupAudio()
    {
        audioSource = bead.GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.loop = true;
        audioSource.playOnAwake = true;

        var clip = Resources.Load<AudioClip>("example_audio");
        if (clip != null) audioSource.Play();
        else Debug.LogWarning("Put example_audio.wav in Assets/Resources/");
    }
}
