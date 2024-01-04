using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DisplayPickup : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI activePickupText;
    PickupHandler pickupHandler;
    PickupType pickupType;

    // Gets the pickupHandler script at the start
    void Start() 
    {
        pickupHandler = GetComponent<PickupHandler>();
    }

    // Changes the text on the UI
    void Update()
    {
        pickupType = pickupHandler.GetActivePickup();
        
        activePickupText.text = pickupType.ToString();
    }
}
