using UnityEngine;
using SimpleJSON;
using System.IO;
using HutongGames.PlayMaker;

public static class LocalSDK
{
    public static JSONNode LanguageDefs { get; private set; }
    private static bool isLanguageLoading = false;

    public static void Init(System.Action onComplete = null)
    {
        if (LanguageDefs != null)
        {
            onComplete?.Invoke();
            return;
        }

        if (isLanguageLoading) return;
        isLanguageLoading = true;

#if UNITY_WEBGL && !UNITY_EDITOR
        LoadLanguageWebGL(onComplete);
#else
        LoadLanguageLocal();
        onComplete?.Invoke();
#endif
    }

    private static void LoadLanguageWebGL(System.Action onComplete)
    {
        string path = Path.Combine(Application.streamingAssetsPath, "language.json");
        Debug.Log($"[SDK] Cargando JSON desde: {path}");

        WebGLFileLoader.LoadText(path, (jsonText) =>
        {
            if (!string.IsNullOrEmpty(jsonText))
            {
                LanguageDefs = JSON.Parse(jsonText);
                Debug.Log($"[SDK] JSON cargado: {LanguageDefs != null}");
            }
            else
            {
                Debug.LogError("[SDK] Fallo carga JSON");
                LanguageDefs = new JSONObject();
            }
            onComplete?.Invoke();
        });
    }

    private static void LoadLanguageLocal()
    {
        string path = Path.Combine(Application.streamingAssetsPath, "language.json");

        if (File.Exists(path))
        {
            string jsonText = File.ReadAllText(path);
            LanguageDefs = JSON.Parse(jsonText);
        }
        else
        {
            Debug.LogError("language.json no encontrado en StreamingAssets");
        }
    }

    public static void SaveProgress()
    {
        FsmVariables globals = FsmVariables.GlobalVariables;

        PlayerPrefs.SetInt("Level", globals.GetFsmInt("Level").Value);
        PlayerPrefs.SetInt("Score", globals.GetFsmInt("Score").Value);
        PlayerPrefs.SetInt("CurrentProgress", globals.GetFsmInt("CurrentProgress").Value);
        PlayerPrefs.SetInt("ScoreL1", globals.GetFsmInt("ScoreL1").Value);
        PlayerPrefs.SetInt("ScoreL2", globals.GetFsmInt("ScoreL2").Value);
        PlayerPrefs.SetInt("ScoreL3", globals.GetFsmInt("ScoreL3").Value);
        PlayerPrefs.SetInt("TTSEnabled", globals.GetFsmBool("TTSenabled").Value ? 1 : 0);
        PlayerPrefs.SetInt("ProgressTotal", globals.GetFsmInt("ProgressTotal").Value);
        PlayerPrefs.SetString("Language", globals.GetFsmString("Language").Value);

        PlayerPrefs.Save();
    }

    public static void LoadProgress()
    {
        FsmVariables globals = FsmVariables.GlobalVariables;

        globals.GetFsmInt("Level").Value = PlayerPrefs.GetInt("Level", 1);
        globals.GetFsmInt("Score").Value = PlayerPrefs.GetInt("Score", 0);
        globals.GetFsmInt("CurrentProgress").Value = PlayerPrefs.GetInt("CurrentProgress", 0);
        globals.GetFsmInt("ScoreL1").Value = PlayerPrefs.GetInt("ScoreL1", 0);
        globals.GetFsmInt("ScoreL2").Value = PlayerPrefs.GetInt("ScoreL2", 0);
        globals.GetFsmInt("ScoreL3").Value = PlayerPrefs.GetInt("ScoreL3", 0);
        globals.GetFsmBool("TTSenabled").Value = PlayerPrefs.GetInt("TTSEnabled", 1) == 1;
        globals.GetFsmInt("ProgressTotal").Value = PlayerPrefs.GetInt("ProgressTotal", 8);

        string savedLanguage = PlayerPrefs.GetString("Language", "es");
        globals.GetFsmString("Language").Value = savedLanguage;
    }

    public static void SetLanguage(string langCode)
    {
        FsmVariables.GlobalVariables.GetFsmString("Language").Value = langCode;

        PlayerPrefs.SetString("Language", langCode);
        PlayerPrefs.Save();

#if UNITY_WEBGL && !UNITY_EDITOR
    LoadLanguageWebGL(() => {
        LocalizationNotifier.NotifyLanguageChange();
    });
#else
        LoadLanguageLocal();
        LocalizationNotifier.NotifyLanguageChange();
#endif
    }
}