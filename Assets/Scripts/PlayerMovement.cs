using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {
    #region COMPONENTS
    private Rigidbody2D playerRb;
    private BoxCollider2D playerColl;
    private Vector2 playerNewForce;
    private SpriteRenderer playerSprite;
    private Animator playerAnim;
    #endregion

    #region MOVEMENT
    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float acceleration;
    [SerializeField] private float decceleration;
    [SerializeField] private float velPower;
    [SerializeField] private float frictionAmount;
    private float moveInput;
    #endregion

    #region JUMP
    [Header("Jump")]
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpSpeed;
    [Range(0f, 1f)] public float jumpCutMultiplier;
    [SerializeField] private float jumpCoyoteTime;
    [SerializeField] private float jumpBufferTime;
    [SerializeField] private float fallGravityMultiplier;
    private float lastGroundedTime = 0;
    private float lastJumpTime = 0;
    #endregion

    #region CHECKS
    [Header("Checks")]
    [SerializeField] private LayerMask groundLayer;
    #endregion

    #region PLAYER STATES
    private enum MovementState { idle, running, jumping, falling, attacking }
    private MovementState playerState = MovementState.idle;
    #endregion

    private void Awake() {
        //fetch the Rigidbody component attached to the Player
        playerRb = GetComponent<Rigidbody2D>();
        //fetch the BoxCollider component attached to the Player
        playerColl = GetComponent<BoxCollider2D>();
        //fetch the SpriteRenderer component attached to the Player
        playerSprite = GetComponent<SpriteRenderer>();
        //fetch the Animator component attached to the Player
        playerAnim = GetComponent<Animator>();
    }
    private void Start() {

    }

    private void Update() {
        UpdateAnimation();

        #region TIMER
        //timer for last time grounder and last time jump
        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
        #endregion

        #region PHYSICS CHECKS
        //if not jumping
        if (playerState != MovementState.jumping) {
            //ground check
            if (Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, .01f, groundLayer)) {
                lastGroundedTime = jumpCoyoteTime;
            } else {
            }
        }
        #endregion

        #region JUMP CHECK
        if (CanJump() && lastJumpTime > 0) {
            Jump();
        }
        #endregion

        #region CHECK FOR INPUT
        InputHandler();
        #endregion


    }

    private void FixedUpdate() {
        #region RUN
        Run();
        #endregion
    }

    private bool CanJump() {
        return lastGroundedTime > 0 && playerState != MovementState.jumping;
    }
    private void Jump() {
        //make sure we cant jump multiple times
        lastJumpTime = 0;
        lastGroundedTime = 0;

        playerState = MovementState.jumping;

        #region  PERFORM JUMP
        float force = jumpForce;
        if (playerRb.velocity.y < 0) {
            force -= playerRb.velocity.y;
        }
        playerRb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }
    private void Run() {
        if (moveInput > 0.01f || moveInput < -0.01f) {
            //calculate the directions and velocity
            float targetSpeed = moveInput * moveSpeed;
            //calculate difference betwee current velocity and desired velocity
            float speedDif = targetSpeed - playerRb.velocity.x;

            #region ACCELERATION
            //gets an acceleration value based on if we are acceleratin or deccelerating
            float acceleraionRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : decceleration;

            //if we want to run but are already going faster than max run speed
            // if ((playerRb.velocity.x > targetSpeed && targetSpeed > 0.01f) || playerRb.velocity.x < targetSpeed && targetSpeed < -0.01f) {
            //     acceleraionRate = 0;
            // }
            #endregion

            float movement = Mathf.Pow(Mathf.Abs(speedDif) * acceleraionRate, velPower) * Mathf.Sign(speedDif);
            Debug.Log("player speed " + movement);
            playerRb.AddForce(movement * Vector2.right);
        }
    }



    #region INPUTHANDLER CUZ I DONT WANT TO MAKE A NEW CLASS YET PLZ MOVE IT LATER AND DONT FORGET
    private void InputHandler() {
        moveInput = Input.GetAxis("Horizontal");

        if (Input.GetButtonDown("Jump") && playerState != MovementState.jumping) {
            lastJumpTime = jumpBufferTime;
        }
    }
    #endregion

    private void UpdateAnimation() {

        if (moveInput > 0f) {
            playerState = MovementState.running;
            playerSprite.flipX = false;
        } else if (moveInput < 0f) {
            playerState = MovementState.running;
            playerSprite.flipX = true;
        } else {
            playerState = MovementState.idle;
        }

        if (playerRb.velocity.y > 0.01f) {
            playerState = MovementState.jumping;
        } else if (playerRb.velocity.y < -0.01f) {
            playerState = MovementState.falling;
        }
        Debug.Log(playerState);
        playerAnim.SetInteger("state", (int)playerState);
    }
}
