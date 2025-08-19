using System.Runtime.InteropServices;
using UnityEngine;

public static class BrowserTTS
{
#if UNITY_WEBGL && !UNITY_EDITOR
    [DllImport("__Internal")]
    private static extern void SpeakText(string text, string lang, float volume, float rate);
#endif

    public static void Speak(string text, string lang = "es", float volume = 0.2f, float rate = 1.0f)
    {
#if UNITY_WEBGL && !UNITY_EDITOR
        SpeakText(text, lang, volume, rate);
#else
        Debug.Log($"[TTS] ({lang}) {text}");
#endif
    }
}
