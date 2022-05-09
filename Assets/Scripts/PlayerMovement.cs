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

    private enum MovmentState { idle, running, jumping, falling, attacking }
    private MovmentState playerState = MovmentState.idle;

    //loading components in Awake() 
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

        //InputHandler.instance.OnJumpPressed;
    }

    private void Update() {
        #region TIMER
        //timer for last time grounder and last time jump
        lastGroundedTime -= Time.deltaTime;
        lastJumpTime -= Time.deltaTime;
        #endregion

        #region PHYSICS CHECKS
        //if not jumping
        if (playerState != MovmentState.jumping) {
            //ground check
            if (Physics2D.BoxCast(playerColl.bounds.center, playerColl.bounds.size, 0f, Vector2.down, .01f, groundLayer)) {
                lastGroundedTime = jumpCoyoteTime;
            }
        }
        #endregion

        #region JUMP CHECK
        //check if player is already jumping and if his velocity is lower then 0 means hes falling
        if (playerState == MovmentState.jumping && playerRb.velocity.y < 0) {
            playerState = MovmentState.falling;
        }

        if (CanJump() && lastJumpTime > 0) {
            Jump();
        }
        #endregion

        #region CHECK FOR INPUT
        InputHandler();
        #endregion
    }

    // private void FixedUpdate() {
    //     float moveInput = Input.GetAxis("Horizontal");

    //     #region RUN
    //     if (playerRb.bodyType == RigidbodyType2D.Dynamic) {
    //         //calculate the direction we want to move in and our desired velocity
    //         float tagetSpeed = moveInput * moveSpeed;

    //         //calculate difference between current velocity and desired velocity
    //         float speedDif = tagetSpeed - playerRb.velocity.x;

    //         //change acceleration rate depending on situation
    //         float accelRate = (Mathf.Abs(tagetSpeed) > 0.01f) ? acceleration : decceleration;

    //         //applies acceleraion to speed difference, the raises to a set power so acceleration increases with higher speeds
    //         //finally multiplies by sign to reapply direction
    //         float movement = Mathf.Pow(Mathf.Abs(speedDif) * accelRate, velPower) * Mathf.Sign(speedDif);

    //         //applies force to rigidbody, multiplying by Vector2.right (Shorthand for writing Vector2(1, 0)) 
    //         //so that it only affects X axis
    //         playerRb.AddForce(movement * Vector2.right);
    //     }
    //     #endregion

    //     #region FRICTION
    //     //check if were grounded and that we are trying to stop (not pressing forwards and backwards)
    //     if (lastGroundedTime > 0 && Mathf.Abs(moveInput) < 0.01f) {
    //         //then we use either the friction amount (~0.2) or our velocity
    //         float amount = Mathf.Min(Mathf.Abs(playerRb.velocity.x), Mathf.Abs(frictionAmount));
    //         //sets to movement direction
    //         amount *= Mathf.Sign(playerRb.velocity.x);
    //         //applies force against movement direction
    //         playerRb.AddForce(Vector2.right * -amount, ForceMode2D.Impulse);
    //     }
    //     #endregion

    //     #region JUMP
    //     //checks if was last grounded within coyoteTime and that jump has been pressed within bufferTime
    //     if (lastGroundedTime > 0 && lastJumpTime > 0 && !isJumping) {
    //         Debug.Log("jump!");
    //         Jump();
    //     }
    //     #endregion

    //     #region GROUNDED

    //     #endregion

    //     UpdateAnimation(moveInput);
    // }

    private void UpdateAnimation(float posX) {
        if (posX > 0f) {
            playerState = MovmentState.running;
            playerSprite.flipX = false;

        } else if (posX < 0f) {
            playerState = MovmentState.running;
            playerSprite.flipX = true;
        } else {
            playerState = MovmentState.idle;
        }

        playerAnim.SetInteger("state", (int)playerState);
    }

    private bool CanJump() {
        return lastGroundedTime > 0 && playerState != MovmentState.jumping;
    }
    private void Jump() {
        //make sure we cant jump multiple times
        lastJumpTime = 0;
        lastGroundedTime = 0;

        playerState = MovmentState.jumping;

        #region  PERFORM JUMP
        float force = jumpForce;
        if (playerRb.velocity.y < 0) {
            force -= playerRb.velocity.y;
        }
        playerRb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
        #endregion
    }



    #region INPUTHANDLER CUZ I DONT WANT TO MAKE A NEW CLASS YET PLZ MOVE IT LATER AND DONT FORGET
    private void InputHandler() {
        if (Input.GetButtonDown("Jump") && playerState != MovmentState.jumping) {
            lastJumpTime = jumpBufferTime;
        }
    }
    #endregion
}
