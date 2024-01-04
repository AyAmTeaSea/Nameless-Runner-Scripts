using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using DG.Tweening;

public class Dashing : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerCamera;
    Rigidbody playerRigidBody;
    PlayerMovement playerMovement;

    [Header("Dashing")]
    [SerializeField] float dashForce;
    [SerializeField] float dashUpwardForce;
    [SerializeField] float maxDashYSpeed;
    [SerializeField] float dashDuration;

    [Header("CameraEffects")]
    [SerializeField] PlayerCamera cam;
    float dashingFOV = 100f;
    float defaultFOV = 90f;

    [Header("Settings")]
    [SerializeField] bool useCameraForward = true;
    [SerializeField] bool allowAllDirections = true;
    [SerializeField] bool disableGravity = false;
    [SerializeField] bool resetValue = true;

    [Header("Cooldown")]
    [SerializeField] float dashCooldown;
    float dashCooldownTimer;

    [Header("Input")]
    [SerializeField] KeyCode dashKey = KeyCode.E;

    // Get the rigidbody and the PlayerMovement script of the player
    private void Start() 
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Call the dash function when the corresponding button is pressed and cooldown the dashings cooldown
    private void Update() 
    {
        if (Input.GetKeyDown(dashKey))
        {
            Dash();
        }

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    // The main dash function, invloves the dash direction, cooldown, getting the max dash speed, setting up variables, changing FOV, and calling other functions
    void Dash()
    {
        if (dashCooldownTimer > 0) return;
        else dashCooldownTimer = dashCooldown;

        playerMovement.dashing = true;
        playerMovement.maxYSpeed = maxDashYSpeed;

        Transform forwardT;

        if (useCameraForward)
        {
            forwardT = playerCamera;
        }
        else 
        {
            forwardT = orientation;
        }

        Vector3 dashDirection = GetDirection(forwardT);

        Vector3 dashForceToApply = dashDirection * dashForce + orientation.up * dashUpwardForce;

        if (disableGravity)
        {
            playerRigidBody.useGravity = false;
        }

        cam.ChangeFOV(dashingFOV);

        delayedDashForceToApply = dashForceToApply;

        Invoke(nameof(DelayedDashForce), 0.025f);

        Invoke(nameof(DashStop), dashDuration);
    }

    // Adds calculated dash force that needs to be added
    Vector3 delayedDashForceToApply;
    void DelayedDashForce()
    {
        if (resetValue)
        {
            playerRigidBody.velocity = Vector3.zero;
        }

        playerRigidBody.AddForce(delayedDashForceToApply, ForceMode.Impulse);
    }

    // Function that sets variables up when the dash ends
    void DashStop()
    {
        playerMovement.dashing = false;
        playerMovement.maxYSpeed = 0;

        cam.ChangeFOV(defaultFOV);

        if (disableGravity)
        {
            playerRigidBody.useGravity = true;
        }
    }

    // Function that gets direction of where to dash
    Vector3 GetDirection(Transform forwardT)
    {
        float horizontalInput = Input.GetAxisRaw("Horizontal");
        float verticalInput = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3();

        if (allowAllDirections)
        {
            direction = forwardT.forward * verticalInput + forwardT.right * horizontalInput;
        }
        else
        {
            direction = forwardT.forward;
        } 

        if (verticalInput == 0 && horizontalInput == 0)
        {
            direction = forwardT.forward;
        }

        return direction.normalized;
    }

    // Changes the FOV for added immersion
    public void ChangeFOV(float fovTarget)
    {
        GetComponent<Camera>().DOFieldOfView(fovTarget, 0.25f);
    }
}
