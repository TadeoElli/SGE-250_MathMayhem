using HutongGames.PlayMaker;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button newGameButton;
    public Button continueButton;
    public GameObject loadingPanel;
    public TextMeshProUGUI loadingText;
    public GameObject languagePanel;

    public float minLoadTime = 1.0f;

    private bool gameReady = false;
    private bool isFirstTime = true;

    IEnumerator Start()
    {
        // 1. Configuración inicial
        loadingPanel.SetActive(true);
        newGameButton.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        languagePanel.SetActive(false); // Inicialmente oculto

        // Mostrar estado de carga
        UpdateLoadingText("Inicializando sistema...");

        // 2. Inicializar SDK si no está listo
        if (LocalSDK.LanguageDefs == null)
        {
            UpdateLoadingText("Cargando diálogos...");
            LocalSDK.Init();
        }

        // 3. Esperar a que todo esté listo
        yield return StartCoroutine(WaitForGameReady());

        // 4. Configurar botones
        SetupButtons();

        // 5. Ocultar panel de carga
        loadingPanel.SetActive(false);
        gameReady = true;
    }

    private void UpdateLoadingText(string message)
    {
        if (loadingText != null)
        {
            loadingText.text = message;
        }
    }

    private IEnumerator WaitForGameReady()
    {
        float startTime = Time.time;
        bool readyReported = false;

        // Esperar condiciones de carga
        while (!IsGameFullyReady() || (Time.time - startTime) < minLoadTime)
        {
            if (!readyReported && IsGameFullyReady())
            {
                UpdateLoadingText("Preparando interfaz...");
                readyReported = true;
            }
            yield return null;
        }
    }

    private bool IsGameFullyReady()
    {
        // 1. Verificar diálogos
        bool dialoguesReady = LocalSDK.LanguageDefs != null;

        // 2. Verificar sistema de guardado
        bool saveSystemReady = true;

        // 3. Verificar si es primera vez
        isFirstTime = !PlayerPrefs.HasKey("GameInitialized");

        return dialoguesReady && saveSystemReady;
    }

    private void SetupButtons()
    {
        // Configurar botón New Game
        newGameButton.gameObject.SetActive(true);
        newGameButton.onClick.AddListener(OnNewGame);

        // Configurar botón Continue (solo si no es primera vez)
        continueButton.gameObject.SetActive(!isFirstTime);

        if (!isFirstTime)
        {
            continueButton.onClick.AddListener(OnContinue);
        }

        // Registrar primera inicialización
        if (isFirstTime)
        {
            PlayerPrefs.SetInt("GameInitialized", 1);
            PlayerPrefs.Save();
        }

        // Mostrar panel de selección de idioma
        languagePanel.SetActive(true);
    }

    public void OnNewGame()
    {
        PlayerPrefs.DeleteKey("Level");

        var sceneFsmVar = FsmVariables.GlobalVariables.GetFsmString("SceneName");
        if (sceneFsmVar == null)
        {
            Debug.LogError("[MainMenu] No existe el FsmString 'SceneName' en Global Variables");
        }
        else
        {
            sceneFsmVar.Value = "level1";
        }

        PlayerPrefs.SetString("SceneName", "level1");

        PlayerPrefs.Save();

        SceneManager.LoadSceneAsync("_init", LoadSceneMode.Single);
    }


    private void OnContinue()
    {
        int currentLevel = PlayerPrefs.GetInt("Level", 1);
        FsmVariables.GlobalVariables.GetFsmString("SceneName").Value = $"level{currentLevel}";
        SceneManager.LoadScene("_init");
    }

    private void ResetProgressVariables()
    {
        var globals = FsmVariables.GlobalVariables;

        globals.GetFsmInt("Level").Value = 1;
        globals.GetFsmInt("Score").Value = 0;
        globals.GetFsmInt("CurrentProgress").Value = 0;
        globals.GetFsmInt("ScoreL1").Value = 0;
        globals.GetFsmInt("ScoreL2").Value = 0;
        globals.GetFsmInt("ScoreL3").Value = 0;
        globals.GetFsmInt("ProgressTotal").Value = 8;
        globals.GetFsmBool("TTSenabled").Value = true;
        globals.GetFsmString("Language").Value = "es";

        LocalSDK.SaveProgress();
    }
}