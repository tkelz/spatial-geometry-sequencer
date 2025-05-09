using UnityEngine;
using UnityEngine.UI;

[ExecuteAlways]
[RequireComponent(typeof(Text))]
public class UIPulse : MonoBehaviour
{
    [Tooltip("Cycles per second")]
    public float pulseSpeed = 1f;
    [Tooltip("Max brightness boost")]
    public float amplitude = 0.2f;

    Text uiText;
    Color baseColor;

    void Awake()
    {
        uiText = GetComponent<Text>();
        baseColor = uiText.color;
    }

    void Update()
    {
        float t = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * amplitude + (1 - amplitude);
        uiText.color = baseColor * t;
    }
}

