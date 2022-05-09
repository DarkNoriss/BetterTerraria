using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour {
    private void Update() {
        //Input.GetButtonDown("Jump") += ctx => MoveInput = ctx.ReadValue<Vector2>();
        // += ctx => MoveInput = Vector2.zero;

        //	controls.Player.Jump.performed += ctx => OnJumpPressed(new InputArgs { context = ctx});
        //  controls.Player.JumpUp.performed += ctx => OnJumpReleased(new InputArgs { context = ctx });
    }


}