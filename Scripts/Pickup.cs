using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    [SerializeField] PickupType pickupType;
    [SerializeField] float pickupRespawnTime = 5f;

    // Gives the player the pick on trigger with this object, finds the PickupHandler script of the player calls a function to start the pickup
    private void OnTriggerEnter(Collider other) 
    {
        FindObjectOfType<PickupHandler>().PickupTool(pickupType);
        GetComponent<Collider>().enabled = false;
        GetComponent<MeshRenderer>().enabled = false;
        StartCoroutine(RespawnPickup());
    }

    // Respawn item after a certain time
    IEnumerator RespawnPickup()
    {
        yield return new WaitForSeconds(pickupRespawnTime);

        GetComponent<Collider>().enabled = true;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
