using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateProjectile : MonoBehaviour
{
    [SerializeField] float maxProjectileDistance = 50f;
    [SerializeField] Transform playerCamera;
    RaycastHit hitObject;

    // Shoots a raycast towards where the player is looking and from where the player is looking and sends a message to the hit object
    public void ShootProjectile()
    {
        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out hitObject, maxProjectileDistance))
        {
            hitObject.transform.SendMessage("HitByRay");
        }
    }
}
