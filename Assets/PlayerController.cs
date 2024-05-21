using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private Vector3 moveDir;
    public CharacterController characterController;
    private Vector3 playerVelocity;
    private float playerSpeed = 10;
    private float jumpHeight = 1;
    private float gravity = -9.81f;
    
    void Start()
    {
        moveDir = Vector3.zero;
    }

    public void OnMove(InputAction.CallbackContext ctxt)
    {
        Vector2 newMoveDir = ctxt.ReadValue<Vector2>();
        moveDir.x = newMoveDir.x;
        moveDir.z = newMoveDir.y;
    }

    public void OnJump(InputAction.CallbackContext ctxt)
    {
        if (ctxt.performed)
        {
            playerVelocity.y = Mathf.Sqrt(jumpHeight * -3.0f * gravity);
        }
    }
    
    
    void Update()
    {
        if (characterController.isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0;
        }

        characterController.Move(moveDir * playerSpeed * Time.deltaTime);
        playerVelocity.y += gravity * Time.deltaTime;
        characterController.Move(playerVelocity * Time.deltaTime);
    }
}
