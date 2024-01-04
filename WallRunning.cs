using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallRunning : MonoBehaviour
{
    [Header("Wallrunning")]
    [SerializeField] LayerMask whatIsWall;
    [SerializeField] LayerMask whatIsGround;
    [SerializeField] float wallRunForce;
    [SerializeField] float wallJumpUpForce;
    [SerializeField] float wallJumpSideForce;

    [Header("Input")]
    float horizontalInput;
    float verticalInput;

    [Header("Detection")]
    [SerializeField] KeyCode wallJumpKey = KeyCode.Space;
    [SerializeField] float wallChechDistance;
    [SerializeField] float minHeightOffGround;
    RaycastHit leftWallHit, rightWallHit;
    bool leftWall, rightWall;

    [Header("Exiting")]
    bool exitingWall;
    [SerializeField] float exitWallTime;
    float exitWallTimer;

    [Header("Gravity")]
    [SerializeField] bool useGravity;
    [SerializeField] float gravityCounterForce;

    [Header("References")]
    Rigidbody playerRigidBody;
    PlayerMovement playerMovement;
    [SerializeField] PlayerCamera playerCamera;
    [SerializeField] Transform orientation;

    // At start get the player rigidbody and the PlayerMovement script
    void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerMovement = GetComponent<PlayerMovement>();
    }

    // Call the wall check and state machine
    void Update()
    {
        CheckForWall();
        StateMachine();
    }
    
    // Call wallrunning functions if the player is wallrunning
    private void FixedUpdate() 
    {
        if (playerMovement.wallrunning)
        {
            WallRunningMovement();            
        }
    }

    // Raycast to check for walls
    void CheckForWall()
    {
        rightWall = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallChechDistance, whatIsWall);
        leftWall = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallChechDistance, whatIsWall);
    }

    // Raycast to check for floor/ground
    bool CheckForNoGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, 1f + minHeightOffGround, whatIsGround);
    } 

    // State machine that checks if the player can wallrun, is leaving wallrunning, or ends wallrunning
    void StateMachine()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        
        // Checks if the player has a wall around them, doesn't have ground below them, and isn't exiting a wall
        if ((leftWall || rightWall) && CheckForNoGround() && verticalInput > 0 && !exitingWall)
        {
            if (!playerMovement.wallrunning)
            {
                StartWallRunning();
            }

            if (Input.GetKeyDown(wallJumpKey))
            {
                WallJump();
            }
        }  

        // If the player is exiting wallrunning
        else if (exitingWall)
        {
            if (playerMovement.wallrunning)
                StopWallRunning();

            if (exitWallTimer > 0)
                exitWallTimer -= Time.deltaTime;

            if (exitWallTimer <= 0)
                exitingWall = false;
        }

        // Handle unexpected situations
        else 
        {
            if (playerMovement.wallrunning)
            {
                StopWallRunning();
            }   
        }
    }

    // Function that sets up variables to start wallrunning
    void StartWallRunning()
    {
        playerMovement.wallrunning = true;
        playerRigidBody.velocity = new Vector3(playerRigidBody.velocity.x, 0, playerRigidBody.velocity.z);

        playerCamera.ChangeFOV(100f);

        if (leftWall) playerCamera.TiltCamera(-5f);
        if (rightWall) playerCamera.TiltCamera(5f);
    }

    // Function that sets up wallrunning movement variables that makes the player wallrun
    void WallRunningMovement()
    {
        playerRigidBody.useGravity = useGravity;

        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }

        playerRigidBody.AddForce(wallForward * wallRunForce, ForceMode.Force);
        
        if (!(leftWall && horizontalInput > 0) && !(rightWall && horizontalInput < 0))
        {
            playerRigidBody.AddForce(-wallNormal * 100, ForceMode.Force);
        }

        if (useGravity)
        {
            playerRigidBody.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    // Function that sets up variables on wallrunn stop
    void StopWallRunning()
    {
        playerMovement.wallrunning = false;
        playerRigidBody.useGravity = true;

        playerCamera.ChangeFOV(90f);
        playerCamera.TiltCamera(0);
    }

    // Function to calculate and apply wall jump force
    void WallJump()
    {
        exitingWall = true;
        exitWallTimer = exitWallTime;

        Vector3 wallNormal = rightWall ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * wallJumpUpForce + wallNormal * wallJumpSideForce;

        playerRigidBody.velocity = new Vector3(playerRigidBody.velocity.x, 0f, playerRigidBody.velocity.z);
        playerRigidBody.AddForce(forceToApply, ForceMode.Impulse);
    }
}
