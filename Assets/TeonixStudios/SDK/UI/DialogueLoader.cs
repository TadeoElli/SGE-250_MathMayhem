using UnityEngine;
using SimpleJSON;
using HutongGames.PlayMaker;

public static class DialogueLoader
{
    public static string GetText(string key)
    {
        string lang = FsmVariables.GlobalVariables.GetFsmString("Language").Value;
        if (LocalSDK.LanguageDefs != null && LocalSDK.LanguageDefs[lang] != null)
        {
            return LocalSDK.LanguageDefs[lang][key] ?? key;
        }
        return key;
    }

    public static string GetTTS(string key)
    {
        string ttsKey = key + "tts";
        string text = GetText(ttsKey);
        if (text == ttsKey) // Si no se encontró la clave tts, usar la normal
        {
            text = GetText(key);
        }
        return text;
    }
}