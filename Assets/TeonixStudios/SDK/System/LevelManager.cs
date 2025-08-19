using HutongGames.PlayMaker;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    void Start()
    {
        if (LocalSDK.LanguageDefs == null)
        {
            LocalSDK.Init();
        }

        LoadLevelProgress();

        LocalizationNotifier.NotifyLanguageChange();
    }

    private void LoadLevelProgress()
    {
        int currentLevel = FsmVariables.GlobalVariables.GetFsmInt("Level").Value;
        int levelScore = PlayerPrefs.GetInt($"ScoreL{currentLevel}", 0);

        // Actualizar UI con el puntaje del nivel
        // textLevelScore.text = $"{levelScore}";
    }

    public void CompleteLevel()
    {
        int currentLevel = FsmVariables.GlobalVariables.GetFsmInt("Level").Value;
        //int scoreEarned = CalculateScore(); // Lógica de puntuación

        // Actualizar puntaje global
        //int totalScore = FsmVariables.GlobalVariables.GetFsmInt("Score").Value + scoreEarned;
        int totalScore = FsmVariables.GlobalVariables.GetFsmInt("Score").Value + FsmVariables.GlobalVariables.GetFsmInt($"ScoreL{currentLevel}").Value;
        FsmVariables.GlobalVariables.GetFsmInt("Score").Value = totalScore;

        // Actualizar puntaje específico del nivel
        //FsmVariables.GlobalVariables.GetFsmInt($"ScoreL{currentLevel}").Value = scoreEarned;

        // Preparar siguiente escena
        FsmVariables.GlobalVariables.GetFsmString("SceneName").Value =
            (currentLevel < 3) ? $"level{currentLevel + 1}" : "mainmenu";

        LocalSDK.SaveProgress();

        SceneManager.LoadScene("_init");
    }
}