using System.IO;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

public class FileManager : MonoBehaviour
{
    public GameManager gameManager;

    public void OpenDialog() {
        var extensions = new [] {
            // new ExtensionFilter("Image Files", "png", "jpg", "jpeg" ),
            new ExtensionFilter("Sound Files", "mp3", "wav" ),
            // new ExtensionFilter("All Files", "*" ),
        };
        var paths = StandaloneFileBrowser.OpenFilePanel("Open Audio File", "", extensions, false);
        if (paths.Length > 0) {
            StartCoroutine(LoadAndPlay(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string gameObjectName, string methodName, string filter, bool multiple);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name, "OnFileUpload", ".png, .jpg", false);
    }

    // Called from browser
    public void OnFileUpload(string url) {
        StartCoroutine(LoadAndPlay(url));
    }
#endif

    IEnumerator LoadAndPlay(string url) {
        var loader = new WWW(url);
        yield return loader;
        gameManager.beadAudio.clip = loader.GetAudioClip(false, false);
        gameManager.beadAudio.Play();
        gameManager.ChangeAudioName(Path.GetFileNameWithoutExtension(url));
    }
}
