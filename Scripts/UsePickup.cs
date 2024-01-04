using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsePickup : MonoBehaviour
{
    [SerializeField] int usePickupMouseButtonIndex = 0;
    PickupHandler pickupHandler;
    ActivateProjectile activateProjectile;
    ShootRocket shootRocket;
    ReverseGravity reverseGravity;

    // Get the required components for this script
    private void Start() 
    {
        pickupHandler = GetComponent<PickupHandler>();
        activateProjectile = GetComponent<ActivateProjectile>();
        shootRocket = GetComponent<ShootRocket>();
        reverseGravity = GetComponent<ReverseGravity>();
    }

    // Use the pickup on corresponding mouse input
    private void Update() 
    {
        if (Input.GetMouseButtonDown(usePickupMouseButtonIndex))
        {
            ActivatePickup();
        }
    }

    // Calls the current active pickup function
    private void ActivatePickup()
    {
        if (pickupHandler.GetActivePickup() == PickupType.none) return;

        if (pickupHandler.GetActivePickup() == PickupType.projectile)
        {
            activateProjectile.ShootProjectile();
        }
        else if (pickupHandler.GetActivePickup() == PickupType.rocket)
        {
            shootRocket.LaunchRocket();
        }
        else if (pickupHandler.GetActivePickup() == PickupType.reversegravity)
        {
            reverseGravity.GravityReverse();
        }

        pickupHandler.UseActivePickup();
    }
}
