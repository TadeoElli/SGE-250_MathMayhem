using UnityEngine;
using UnityEngine.SceneManagement;
using HutongGames.PlayMaker;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    IEnumerator Start()
    {
        Debug.Log("Iniciando SceneLoader...");

        bool sdkInitialized = false;
        LocalSDK.Init(() => {
            Debug.Log("Callback SDK ejecutado");
            sdkInitialized = true;
        });


        float startTime = Time.time;
        yield return new WaitUntil(() => sdkInitialized && (Time.time - startTime) >= 1f);

        string targetScene = FsmVariables.GlobalVariables.GetFsmString("SceneName").Value;

        if (!IsSceneValid(targetScene))
        {
            Debug.LogError($"Escena inválida: {targetScene}. Redirigiendo a mainmenu");
            targetScene = "mainmenu";
        }
        
        Debug.Log($"Cargando escena: {targetScene}");
        SceneManager.LoadScene(targetScene);
    }

    private bool IsSceneValid(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string scene = System.IO.Path.GetFileNameWithoutExtension(scenePath);
            if (scene == sceneName) return true;
        }
        return false;
    }
}