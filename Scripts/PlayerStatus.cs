using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerStatus : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI textMeshProUGUI;
    [SerializeField] Canvas deathUI, gameUI, levelCompleteUI;
    Timer timer;

    int currentSceneIndex;
    [SerializeField] float respawnTimer;
    bool respawningPlayer;

    // The main script that tracks level completion, death state, timer, and respawning
    // On start find the timer, and disable certain UIs
    void Start() 
    {
        deathUI.enabled = false;
        levelCompleteUI.enabled = false;
        timer = FindObjectOfType<Timer>();
    }

    // Handle respawning
    void Update() 
    {
        if (respawningPlayer)
        {
            if (respawnTimer > 0)
            {
                respawnTimer -= Time.deltaTime;
                DisplayRespawnTimer(respawnTimer);
            }
            else
            {
                RespawnPlayer();
            }
        }
    }

    // Respawns the player on death
    void RespawnPlayer()
    {
        currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
            
        SceneManager.LoadScene(currentSceneIndex);    
    }

    // Shows the respawn timer on the deathUI
    void DisplayRespawnTimer(float remainingRespawnTimer)
    {
        textMeshProUGUI.text = (Mathf.FloorToInt(++remainingRespawnTimer)).ToString();
    }

    // Upon level completion sets up certain things
    public void CompleteLevel()
    {
        gameUI.enabled = false;
        levelCompleteUI.enabled = true;
        GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        FindObjectOfType<PlayerCamera>().enabled = false;
        timer.levelComplete = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // If the player isn't already dead and respawning sets up certain things
    public void KillPlayer()
    {
        if (!respawningPlayer)
        {    
            deathUI.enabled = true;
            gameUI.enabled = false;
            respawningPlayer = true;
            GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
            FindObjectOfType<PlayerCamera>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}
