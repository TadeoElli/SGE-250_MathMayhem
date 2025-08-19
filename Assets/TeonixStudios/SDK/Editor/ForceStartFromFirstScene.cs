#if UNITY_EDITOR

using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class ForceStartFromFirstScene
{
    private const string EnabledPrefKey = "AutoPlayFromFirstSceneEnabled";
    private const string ScenePathPrefKey = "AutoPlayFromFirstScene_PreviousScene";

    private static bool Enabled
    {
        get => EditorPrefs.GetBool(EnabledPrefKey, true);
        set => EditorPrefs.SetBool(EnabledPrefKey, value);
    }

    static ForceStartFromFirstScene()
    {
        if (Enabled)
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (!Enabled) return;

        switch (state)
        {
            case PlayModeStateChange.ExitingEditMode:
                SaveCurrentScenePath();
                LoadFirstSceneInBuildSettings();
                break;

            case PlayModeStateChange.EnteredEditMode:
                ReturnToPreviousScene();
                break;
        }
    }

    private static void SaveCurrentScenePath()
    {
        string currentPath = SceneManager.GetActiveScene().path;
        EditorPrefs.SetString(ScenePathPrefKey, currentPath);
    }

    private static void LoadFirstSceneInBuildSettings()
    {
        if (EditorBuildSettings.scenes.Length == 0)
        {
            Debug.LogWarning("No hay escenas en Build Settings.");
            return;
        }

        string firstScenePath = EditorBuildSettings.scenes[0].path;

        if (SceneManager.GetActiveScene().path != firstScenePath)
        {
            if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorSceneManager.OpenScene(firstScenePath);
            }
            else
            {
                EditorApplication.isPlaying = false;
            }
        }
    }

    private static void ReturnToPreviousScene()
    {
        string previousScenePath = EditorPrefs.GetString(ScenePathPrefKey, "");

        if (!string.IsNullOrEmpty(previousScenePath) && File.Exists(previousScenePath))
        {
            EditorSceneManager.OpenScene(previousScenePath);
        }
    }

    [MenuItem("Tools/Teonix Studios/Auto Play From First Scene")]
    private static void ToggleAutoPlay()
    {
        Enabled = !Enabled;
        EditorApplication.playModeStateChanged -= OnPlayModeChanged;

        if (Enabled)
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
            Debug.Log(">>>> Auto Play From First Scene activado.");
        }
        else
        {
            Debug.Log(">>>> Auto Play From First Scene desactivado.");
        }
    }

    [MenuItem("Tools/Teonix Studios/Auto Play From First Scene", true)]
    private static bool ToggleAutoPlayValidate()
    {
        Menu.SetChecked("Tools/Teonix Studios/Auto Play From First Scene", Enabled);
        return true;
    }
}
#endif
