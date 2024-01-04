using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
 [Header("Movement")]
    [SerializeField] float moveSpeed;
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    [SerializeField] float slideSpeed;
    [SerializeField] float wallRunningSpeed;
    [SerializeField] float dashSpeed;
    
    public float maxYSpeed;

    float desiredMoveSpeed;
    float lastDesiredMoveSpeed;

    [SerializeField] float dashSpeedChangeFactor;

    [SerializeField] float speedChangeFactor;
    public float groundDrag;

    [Header("Jumping")]
    public float jumpForce;
    public float jumpCooldown;
    public float airMultiplier;
    bool readyToJump;

    [Header("Crouching")]
    public float crouchSpeed;
    public float crouchYScale;
    private float startYScale;
    
    bool crouchingOnWill = true;

    [Header("Keybinds")]
    public KeyCode jumpKey = KeyCode.Space;
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Ground Check")]
    public float playerHeight;
    public LayerMask whatIsGround;
    bool grounded;

    [Header("Slope Handling")]
    public float maxSlopeAngle;
    RaycastHit slopeHit;
    bool exitingSlope;
    

    public Transform orientation;

    float horizontalInput;
    float verticalInput;

    Vector3 moveDirection;

    Rigidbody playerRigidBody;

    public bool sliding;
    public bool wallrunning;
    public bool dashing;
    public bool activeGrappling;

    bool keepMomentum;

    bool canChangeState = true;

    public MovementState state;
    MovementState lastState;

    // All the movement states
    public enum MovementState
    {
        walking,
        sprinting,
        sliding,
        wallrunning,
        crouching,
        dashing,
        air
    }

    // On start freeze rotation, make the player ready to jump and get the players height
    private void Start()
    {
        playerRigidBody = GetComponent<Rigidbody>();
        playerRigidBody.freezeRotation = true;

        readyToJump = true;

        startYScale = transform.localScale.y;
    }

    // Always check if the player is grounded with a raycast, call to get the player input, control the players speed, call the state handler and handle drag
    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.3f, whatIsGround);

        MyInput();        
        SpeedControl();

        if (canChangeState)
            StateHandler();

        // handle drag
        if (state == MovementState.walking || state == MovementState.sprinting || state == MovementState.crouching && !activeGrappling)
            playerRigidBody.drag = groundDrag;
        else
            playerRigidBody.drag = 0;
    }

    // Call player movement scripts
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Function that gets the players input and calls the appropriate functions
    private void MyInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        // Jump when grounded and is ready to jump
        if(Input.GetKey(jumpKey) && readyToJump && grounded)
        {
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }

        canChangeState = !Physics.Raycast(transform.position, Vector3.up, playerHeight, whatIsGround);
        // Check if the player is holding the crouch button
        if (Input.GetKey(crouchKey))
            crouchingOnWill = true;
        else
            crouchingOnWill = false;
        
        // Start crouch
        if (Input.GetKeyDown(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            playerRigidBody.AddForce(Vector3.down * 5f, ForceMode.Impulse);
        }

        // Stop crouch if not under object and player lets go of crouch key
        else if (Input.GetKeyUp(crouchKey) && canChangeState)
        {    
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }

        // Stop crouching when the player isn't under an object and the player isn't hold the crouch button
        else if (canChangeState && !crouchingOnWill && state == MovementState.crouching)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
        }
    }

    // When the players pushes the corresponding buttons to enter a movement state state handler sets certain things up about the player, most commonly the movement speed and movement state
    private void StateHandler()
    {
        // Mode - Dashing
        if (dashing)
        {
            state = MovementState.dashing;
            desiredMoveSpeed = dashSpeed;
            keepMomentum = true;
            speedChangeFactor = dashSpeedChangeFactor;
        }

        // Mode - Sliding
        else if (sliding)
        {
            state = MovementState.sliding;
            
            if (OnSlope() && playerRigidBody.velocity.y < 0.1f)
            {
                desiredMoveSpeed = slideSpeed;
                keepMomentum = true;
            }

            else
            {
                desiredMoveSpeed = sprintSpeed;
            }
        }

        // Mode - Wallrunning
        else if (wallrunning)
        {
            state = MovementState.wallrunning;
            desiredMoveSpeed = wallRunningSpeed;
        }

        // Mode - Crouching
        else if (Input.GetKey(crouchKey))
        {
            state = MovementState.crouching;
            desiredMoveSpeed = crouchSpeed;
        }

        // Mode - Sprinting
        else if(grounded && Input.GetKey(sprintKey))
        {
            state = MovementState.sprinting;
            desiredMoveSpeed = sprintSpeed;
        }

        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.walking;
            desiredMoveSpeed = walkSpeed;
        }

        // Mode - Air
        else
        {
            state = MovementState.air;

            if (desiredMoveSpeed < sprintSpeed)
                desiredMoveSpeed = walkSpeed;
            else
                desiredMoveSpeed = sprintSpeed;
        }

        // Script that adds momentum into the game, the player can retain momentum for a while when leaving or in certain movement states and if the movement change is too major
        bool desiredMoveSpeedChanged = desiredMoveSpeed != lastDesiredMoveSpeed;
        if (lastState == MovementState.dashing)
            keepMomentum = true;

        if (desiredMoveSpeedChanged)
        {
            if (keepMomentum)
            {
                StopAllCoroutines();
                StartCoroutine(SmoothlyLerpMoveSpeed());
            }
            else 
            {
                StopAllCoroutines();
                moveSpeed = desiredMoveSpeed;
            }
        }

        lastDesiredMoveSpeed = desiredMoveSpeed;
        lastState = state;

        if (Mathf.Abs(desiredMoveSpeed - moveSpeed) < 0.1f) keepMomentum = false;
    }

    // Function that allows momentum, uses Lerp'ing in order to control movement speed
    IEnumerator SmoothlyLerpMoveSpeed()
    {
        float time = 0;
        float difference = MathF.Abs(desiredMoveSpeed - moveSpeed);
        float startValue = moveSpeed;

        float boostFactor = speedChangeFactor;

        while (time < difference)
        {
            moveSpeed = Mathf.Lerp(startValue, desiredMoveSpeed, time / difference);

            time += Time.deltaTime * boostFactor;

            yield return null;
        }

        moveSpeed = desiredMoveSpeed;
        speedChangeFactor = 1f;
        keepMomentum = false;
    }

    // The script that applies force to the player depanding on their movement state
    private void MovePlayer()
    {
        if (state == MovementState.sliding || activeGrappling) return;

        // calculate movement direction
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        // on slope
        if (OnSlope() && !exitingSlope)
        {
            playerRigidBody.AddForce(GetSlopeMoveDirection(moveDirection) * moveSpeed * 20f, ForceMode.Force);

            if (playerRigidBody.velocity.y > 0)
                playerRigidBody.AddForce(Vector3.down * 80f, ForceMode.Force);
        }

        // on ground
        else if(grounded)
            playerRigidBody.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);

        // in air
        else if(!grounded)
            playerRigidBody.AddForce(moveDirection.normalized * moveSpeed * 10f * airMultiplier, ForceMode.Force);

        // turn gravity off while on slope
        if (!wallrunning)
            playerRigidBody.useGravity = !OnSlope();
    }

    // Function that limits speed as some movement states movement speed can get out of hand other wise
    private void SpeedControl()
    {
        if (activeGrappling) return;

        // limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (playerRigidBody.velocity.magnitude > moveSpeed)
                playerRigidBody.velocity = playerRigidBody.velocity.normalized * moveSpeed;
        }

        // limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(playerRigidBody.velocity.x, 0f, playerRigidBody.velocity.z);

            // limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                playerRigidBody.velocity = new Vector3(limitedVel.x, playerRigidBody.velocity.y, limitedVel.z);
            }
        }

        // limit y speed
        if (maxYSpeed != 0 && playerRigidBody.velocity.y > maxYSpeed)
        {
            playerRigidBody.velocity = new Vector3(playerRigidBody.velocity.x, maxYSpeed, playerRigidBody.velocity.z);
        }
    }

    // Function that causes the player to jump via force
    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        playerRigidBody.velocity = new Vector3(playerRigidBody.velocity.x, 0f, playerRigidBody.velocity.z);

        playerRigidBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }

    // Function that gets called when the players jump needs to be reset
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }

    // Checks if the player is on a slope by checking the ground below the player via the up direction and the normal of the slope and the angle between
    public bool OnSlope()
    {
        if(Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.5f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return (angle < maxSlopeAngle && angle != 0);
        }

        return false;
    }

    // Direction of the slope
    public Vector3 GetSlopeMoveDirection(Vector3 direction)
    {
        return Vector3.ProjectOnPlane(direction, slopeHit.normal).normalized;
    }

    // Function that is related to the grappling hook script, calculates the force applied to the player when they grapple
    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (Mathf.Sqrt(-2 * trajectoryHeight / gravity) 
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }

    // Function that gets called by the grappling hook script that moves the player, uses the results of the CalculateJumpVelocity function
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {   
        activeGrappling = true;

        calculatedJumpVelocity = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        velocityToSet = new Vector3(calculatedJumpVelocity.x, calculatedJumpVelocity.y + 6.5f, calculatedJumpVelocity.z);

        Invoke(nameof(SetVelocity), 0.1f);

        Invoke(nameof(ResetRestrictions), 3f);
    }

    Vector3 calculatedJumpVelocity, velocityToSet;
    
    // Sets the player velocity to the velocity calculated in JumpToPosition
    void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        playerRigidBody.velocity = velocityToSet;
    }

    bool enableMovementOnNextTouch;

    // Stops grappling on contact with an object
    private void OnCollisionEnter(Collision other) 
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = true;
            ResetRestrictions();

            GetComponent<Grappling>().StopGrappling();
        }
    }

    // Removes the restrictions placed via the grappling functions
    public void ResetRestrictions()
    {
        activeGrappling = false;
    }
}
