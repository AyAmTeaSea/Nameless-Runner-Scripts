using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sliding : MonoBehaviour
{
    [Header("References")]
    [SerializeField] Transform orientation;
    [SerializeField] Transform playerObject;
    Rigidbody playerRigidBody;
    PlayerMovement playerMovement;

    [Header("Sliding")]
    [SerializeField] float maxSlideTime;
    [SerializeField] float slideForce;
    float slideTimer;

    [SerializeField] float slideYscale;
    float startYScale;

    [Header("Input")]
    [SerializeField] KeyCode slideKey = KeyCode.LeftControl;
    float horizontalInput;
    float verticalInput;

    // Get players rigidbody, player scale, and PlayerMovement script at the start
    private void Start() 
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();

        startYScale = playerObject.localScale.y;
    }

    // Call function to get player input
    private void Update()
    {
        HandlePlayerInput();
    }

    // Call function to create sliding movement
    private void FixedUpdate() 
    {
        if (playerMovement.sliding)
        {
            SlidingMovement();
        }
    }

    // Start or stop sliding based on the players input
    private void HandlePlayerInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey) && (verticalInput != 0 || horizontalInput != 0))
        {
            StartSliding();
        }

        if (Input.GetKeyUp(slideKey) && playerMovement.sliding)
        {
            StopSliding();
        }
    }

    // Change player scale and start sliding
    private void StartSliding()
    {
        playerMovement.sliding = true;

        playerObject.localScale = new Vector3(playerObject.localScale.x, slideYscale, playerObject.localScale.z);
        playerRigidBody.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        slideTimer = maxSlideTime;
    }

    // Apply force to make the player slide and call StopSliding at the end of the slide
    private void SlidingMovement()
    {
        Vector3 inputDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (!playerMovement.OnSlope() || playerRigidBody.velocity.y > -0.1f)
        {
            playerRigidBody.AddForce(inputDirection.normalized * slideForce, ForceMode.Force);

            slideTimer -= Time.deltaTime;
        }

        else
        {
            playerRigidBody.AddForce(playerMovement.GetSlopeMoveDirection(inputDirection) * slideForce, ForceMode.Force);
        }

        if (slideTimer <= 0)
        {
            StopSliding();
        }
    }

    // Resets sliding and player scale
    private void StopSliding()
    {
        playerMovement.sliding = false;

        playerObject.localScale = new Vector3(playerObject.localScale.x, startYScale, playerObject.localScale.z);
    }
}
