using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootRocket : MonoBehaviour
{
    [SerializeField] float rocketSpeed;
    [SerializeField] GameObject rocket;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform rocketStartPosition;

    // Fires the rocket object from the player
    public void LaunchRocket()
    {   
        GameObject rocket2 = Instantiate(rocket, rocketStartPosition.position, playerCamera.rotation);

        rocket2.GetComponent<Rigidbody>().AddRelativeForce(new Vector3(0, 0, rocketSpeed));
    }
}
