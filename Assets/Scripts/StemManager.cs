using System.Collections.Generic;
using UnityEngine;

public class StemManager : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject stemPrefab;

    public List<StemItem> stems;

    void Start()
    {
        stems = new List<StemItem>();
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

    public void RemoveStem(int index) {
        Destroy(stems[index].gameObject);
        stems.RemoveAt(index);
    }
}

