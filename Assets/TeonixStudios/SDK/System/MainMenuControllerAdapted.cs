using HutongGames.PlayMaker;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class MainMenuControllerAdapted : MonoBehaviour
{
    public GameObject startPanel;
    public GameObject tutorialPanel;
    public GameObject loadingPanel;
    public GameObject winPanel;
    public GameObject hudStats;
    public TextMeshProUGUI loadingText;

    public float minLoadTime = 1.0f;

    private bool gameReady = false;

    IEnumerator Start()
    {
        // 1. Configuración inicial
        loadingPanel.SetActive(true);
        startPanel.SetActive(false);
        tutorialPanel.SetActive(false);
        winPanel.SetActive(false);
        hudStats.SetActive(false);

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

        return dialoguesReady && saveSystemReady;
    }

    private void SetupButtons()
    {
        startPanel.SetActive(true);
        tutorialPanel.SetActive(false);
        winPanel.SetActive(false);
        hudStats.SetActive(false);
    }

}