using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.ProBuilder;

public class Timer : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] float levelRemainingTimeInSeconds;
    [SerializeField] PlayerStatus playerStatus;
    public bool levelComplete;

    // If the level isn't complete or if the player isn't dead, countdown the level timer
    void Update()
    {
        if (levelRemainingTimeInSeconds > 0 && !levelComplete)
        {
            levelRemainingTimeInSeconds -= Time.deltaTime;
            DisplayTime(levelRemainingTimeInSeconds);
        }
        else if (levelComplete)
        {}
        else
        {
            playerStatus.KillPlayer();
        }
    }

    // Display the level timer on the UI
    void DisplayTime(float timeRemaining)
    {
        timeRemaining++;
        float minutes = Mathf.FloorToInt(timeRemaining / 60);
        float seconds = Mathf.FloorToInt(timeRemaining % 60);
        textMeshProUGUI.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}
