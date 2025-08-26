using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerHud : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI timerText;
    [SerializeField] TextMeshProUGUI finishText;
    float elapsedTime;
    private bool hasStarted = false;

    public void StartGame()
    {
        hasStarted = true;
    }
    public void StopGame()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        finishText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        Debug.Log(elapsedTime);
        hasStarted = false;
        elapsedTime = 0;
        timerText.text = string.Format("{0:00}:{1:00}", 0,0);
    }
    // Update is called once per frame
    void Update()
    {
        if (!hasStarted) return;
        elapsedTime += Time.deltaTime;
        int minutes = Mathf.FloorToInt(elapsedTime / 60);
        int seconds = Mathf.FloorToInt(elapsedTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
