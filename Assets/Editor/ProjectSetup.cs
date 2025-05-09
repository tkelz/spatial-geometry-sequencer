#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;

public static class ProjectSetup
{
    [MenuItem("Tools/Spatial Sequencer/Initial Setup")]
    public static void InitialSetup()
    {
        string[] folders = new[]
        {
            "Assets/Scenes",
            "Assets/Audio",
            "Assets/Audio/Stems",
            "Assets/Materials",
            "Assets/Prefabs",
            "Assets/Scripts",
            "Assets/Scripts/Audio",
            "Assets/Scripts/Geometry",
            "Assets/Scripts/UI",
            "Assets/Resources",
            "Assets/Editor"
        };

        foreach (var path in folders)
        {
            if (!AssetDatabase.IsValidFolder(path))
            {
                var parent = Path.GetDirectoryName(path).Replace("\\","/");
                var name   = Path.GetFileName(path);
                AssetDatabase.CreateFolder(parent, name);
            }
        }

        var scene = EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
        EditorSceneManager.SaveScene(scene, "Assets/Scenes/Main.unity");

        var setupGO = new GameObject("SetupManager");
        var scriptType = typeof(PathSetupTest);
        if (scriptType != null)
            setupGO.AddComponent(scriptType);
        else
            Debug.LogWarning("PathSetupTest script not found. Please add it to Assets/Scripts/Geometry.");

        AssetDatabase.Refresh();
        EditorUtility.DisplayDialog(
            "Spatial Sequencer Setup",
            "Folder structure, Main.unity scene, and SetupManager created.\n\n" +
            "Drop your audio files into Assets/Resources/, and press Play!",
            "OK"
        );
    }
}
#endif
