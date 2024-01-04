using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMover : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition;

    // Constantly move the camera to where the player is
    void Update()
    {
        transform.position = cameraPosition.position;
    }
}
