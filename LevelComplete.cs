using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelComplete : MonoBehaviour
{
    PlayerStatus playerStatus;

    // Gets the playerStatus script at the start
    private void Start() 
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
    }

    // When the player comes in contact with this object complete the level
    private void OnTriggerEnter(Collider other) 
    {
        playerStatus.CompleteLevel();
    }
}
