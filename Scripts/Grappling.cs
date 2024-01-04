using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Grappling : MonoBehaviour
{
    [Header("Refereences")]
    PlayerMovement playerMovement;
    [SerializeField] Transform playerCamera;
    [SerializeField] Transform grappler;
    [SerializeField] LayerMask whatIsGrappleable;
    [SerializeField] LineRenderer lineRenderer;

    [Header("Grappling")]
    [SerializeField] float maxGrappleDistance;
    [SerializeField] float grappleDelayTime;
    [SerializeField] float overshootYAxis;

    public Vector3 grapplePoint;

    [Header("Cooldown")]
    [SerializeField] float grapplingCooldown;
    float grapplingCooldownTimer;

    [Header("Input")]
    [SerializeField] int grappleMouseButtonIndex = 1;

    public bool grappling;

    // Get PlayerMovement script at start in order to link it
    private void Start() 
    {
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Calls the grappling functions if the players presses the corresponding button and cooldown the grappling hooks cooldown
    private void Update() 
    {
        if (Input.GetMouseButtonDown(grappleMouseButtonIndex))
        {
            StartGrappling();
        }

        if (grapplingCooldownTimer > 0)
            grapplingCooldownTimer -= Time.deltaTime;
    }

    // If grappling isn't on cooldown, starts the grappling movement
    void StartGrappling()
    {
        if (grapplingCooldownTimer > 0) return;

        grappling = true;

        RaycastHit grappleHitPoint;

        if (Physics.Raycast(playerCamera.position, playerCamera.forward, out grappleHitPoint, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = grappleHitPoint.point;

            Invoke(nameof(GrapplingMovement), grappleDelayTime);
        }
        else 
        {
            grapplePoint = playerCamera.position + playerCamera.forward * maxGrappleDistance;

            Invoke(nameof(StopGrappling), grappleDelayTime);
        }
    }

    // Function that gets used to create the grappling movement, uses the JumpToPosition function from the PlayerMovement script
    void GrapplingMovement()
    {
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) 
        {
            highestPointOnArc = overshootYAxis;
        }

        playerMovement.JumpToPosition(grapplePoint, highestPointOnArc);

        Invoke(nameof(StopGrappling), 1f);
    }

    // Stops grappling
    public void StopGrappling()
    {
        grappling = false;

        grapplingCooldownTimer = grapplingCooldown;
    }
}
