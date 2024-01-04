using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PickupHandler : MonoBehaviour
{
    [SerializeField] Pickup[] pickups;

    [System.Serializable]
    private class Pickup
    {
        public PickupType pickupType;
    }

    Pickup currentActivePickUp;

    // Sets the players start pickup
    void Start() 
    {
       currentActivePickUp = new Pickup();
       currentActivePickUp.pickupType = PickupType.none; 
    }

    // Called by the Pickup script
    // Allows the player to pickup the corresponding pick up of the object
    public void PickupTool(PickupType pickedupType)
    {
        foreach (Pickup pickup in pickups)
        {
            if (pickup.pickupType == pickedupType)
            {
                currentActivePickUp.pickupType = pickup.pickupType;
                return;
            }
        }
    }

    // Returns the pickupType of the active pickup
    public PickupType GetActivePickup()
    {
        return currentActivePickUp.pickupType;
    }

    // Called when the current pickup wants to be used
    public void UseActivePickup()
    {
        currentActivePickUp.pickupType = PickupType.none;
    }
}
