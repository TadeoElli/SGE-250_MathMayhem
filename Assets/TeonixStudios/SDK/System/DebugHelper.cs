using HutongGames.PlayMaker;
using TMPro;
using UnityEngine;

public class DebugHelper : MonoBehaviour
{
    public TextMeshProUGUI debugText;

    void Update()
    {
        if (debugText != null)
        {
            string debugInfo = $"Estado del juego:\n";
            debugInfo += $"Dialogos cargados: {LocalSDK.LanguageDefs != null}\n";
            debugInfo += $"Primera vez: {!PlayerPrefs.HasKey("GameInitialized")}\n";
            debugInfo += $"Nivel actual: {PlayerPrefs.GetInt("Level", 1)}\n";
            debugInfo += $"SceneName: {FsmVariables.GlobalVariables.GetFsmString("SceneName").Value}";

            debugText.text = debugInfo;
        }
    }
}