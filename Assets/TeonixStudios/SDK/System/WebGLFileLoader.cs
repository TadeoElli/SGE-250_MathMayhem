using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class WebGLFileLoader : MonoBehaviour
{
    private static WebGLFileLoader _instance;

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public static void LoadText(string path, System.Action<string> callback)
    {
        Debug.Log($"Intentando cargar: {path}");
        _instance.StartCoroutine(_instance.LoadTextCoroutine(path, callback));
    }

    private IEnumerator LoadTextCoroutine(string path, System.Action<string> callback)
    {
        using (UnityWebRequest www = UnityWebRequest.Get(path))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                callback(www.downloadHandler.text);
            }
            else
            {
                Debug.LogError($"Error cargando {path}: {www.error}");
                callback(null);
            }
        }
    }
}