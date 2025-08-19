using UnityEngine;

public class FirstTimeChecker : MonoBehaviour
{
    private const string FIRST_TIME_KEY = "FirstTime";

    void Start()
    {
        if (!PlayerPrefs.HasKey(FIRST_TIME_KEY))
        {
            PlayerPrefs.SetInt(FIRST_TIME_KEY, 1);
            PlayerPrefs.Save();

            PlayMakerFSM.BroadcastEvent("RESET_PROGRESS");
        }
    }
}