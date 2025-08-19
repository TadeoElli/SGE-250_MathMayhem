using UnityEngine;
using SimpleJSON;
using HutongGames.PlayMaker;
using System.Collections;

public class TXSDialogueManager : MonoBehaviour
{
    public static TXSDialogueManager Instance;
    [Header("TTS Delay")]
    [UnityEngine.Tooltip("Duración estimada en segundos por carácter para el cálculo del tiempo de habla del TTS. Recomendado: 0.0315f a 0.0667f")]
    public float clipLengthPerChar = 0.0367f;

    private JSONNode defs;
    private string currentLanguage = "es";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        StartCoroutine(InitializeDialogueSystem());
    }

    private IEnumerator InitializeDialogueSystem()
    {
        bool loaded = false;

        // Inicializar SDK con callback
        LocalSDK.Init(() => {
            loaded = true;
        });

        // Esperar carga
        yield return new WaitUntil(() => loaded);

        defs = LocalSDK.LanguageDefs;
        currentLanguage = FsmVariables.GlobalVariables.GetFsmString("Language").Value;
    }

    public string GetDialogueLine(string key)
    {
        if (defs == null)
        {
            Debug.LogWarning($"Idioma '{currentLanguage}' no disponible");
            return key; // Fallback
        }

        bool hasLanguage = false;
        foreach (var langKey in defs.Keys)
        {
            if (langKey == currentLanguage)
            {
                hasLanguage = true;
                break;
            }
        }

        if (!hasLanguage)
        {
            Debug.LogWarning($"Idioma '{currentLanguage}' no disponible");
            return key; // Fallback
        }

        return defs[currentLanguage][key]?.Value ?? key;
    }

    public string GetTTSLineKey(string key)
    {
        string ttsKey = key + "tts";

        if (defs[currentLanguage][ttsKey] != null)
            return ttsKey;

        return key;
    }

    public string GetTTSDialogueLine(string key)
    {
        string ttsKey = GetTTSLineKey(key);
        if (defs[currentLanguage][ttsKey] != null)
            return defs[currentLanguage][ttsKey].Value;
        Debug.LogWarning($">>>>>>>>> TTS: No se encontró línea TTS para clave: {key}");
        return key; // Fallback
    }

    public float GetClipLength(string textKey)
    {
        var text = GetDialogueLine(textKey);

        if (string.IsNullOrEmpty(text))
        {
            Debug.LogWarning($"Texto vacío para clave: {textKey}");
            return 1.5f; // Duración por defecto
        }

        return text.Length * clipLengthPerChar;
    }
}