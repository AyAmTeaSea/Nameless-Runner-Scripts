using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResetLevelOnCollision : MonoBehaviour
{
    PlayerStatus playerStatus;

    // On contact with this object the player dies
    private void OnCollisionEnter(Collision other) {
        if (other.transform.tag == "Player")
        {
            playerStatus = other.transform.GetComponent<PlayerStatus>();
            playerStatus.KillPlayer();
        }
    }
}
