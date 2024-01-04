using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevel : MonoBehaviour
{
    int nextSceneIndex;

    // When called starts the next scene
    public void StartNextLevel()
    {
        nextSceneIndex = (SceneManager.GetActiveScene().buildIndex) + 1;
            
        SceneManager.LoadScene(nextSceneIndex);  
    }
}
