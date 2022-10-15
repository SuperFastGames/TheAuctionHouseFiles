using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public GameObject MarketMenu;
    private Rigidbody rb;
    private PlayerInputActions playerInput;
    private Animator animator;
    CharacterController characterController;
    Vector2 currentMovementInput;
    Vector3 currentMovement;
    bool isMovementPressed;
    public float speed = 20;
    public float gravity = -50;
    private float groundedGravity = -20f;
    public float rotationFactorPerFrame = 15.0f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        //initialize player input actions and animator
        playerInput = new PlayerInputActions();
        playerInput.Game.Move.started += onMovementInput;
        playerInput.Game.Move.canceled += onMovementInput;
        playerInput.Game.Move.performed += onMovementInput;
        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
    }

    // used for player movement, call this to enable or disable player input detection
    private void OnEnable()
    {
        playerInput.Enable();
    }

    private void OnDisable()
    {
        playerInput.Disable();
    }

    // gravity
     void handleGravity()
    {
        if (characterController.isGrounded){
            currentMovement.y = groundedGravity;
            }
            else
            {
                currentMovement.y += gravity * Time.deltaTime;    
            }
    }
     
    // movement
    void onMovementInput(InputAction.CallbackContext context)
    {
        currentMovementInput = context.ReadValue<Vector2>();
        currentMovement.x = currentMovementInput.x;
        currentMovement.z = currentMovementInput.y;
        isMovementPressed = currentMovementInput.x != 0 || currentMovementInput.y != 0;
        if (isMovementPressed)
        {
            animator.SetBool("isRunning", true);
        }
        else
        {
            animator.SetBool("isRunning", false);
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.transform.tag == "Market")
        {
            MarketMenu.SetActive(true);
        }
    }

    public void CloseMarketMenu()
    {
        MarketMenu.SetActive(false);
    }

    // rotation
    void handleRotation()
    {
        Vector3 positionToLookAt;
        positionToLookAt.x = currentMovement.x;
        positionToLookAt.y = 0.0f;
        positionToLookAt.z = currentMovement.z;
        Quaternion currentRotation = transform.rotation;

        if (isMovementPressed)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation = Quaternion.Slerp(currentRotation, targetRotation, rotationFactorPerFrame * Time.deltaTime);
        }

    }

    // player move functions here as they constantly need to be updated
    void Update()
    {
        // all movement is calulated over Time.deltaTime to unsure uniform speeds across devices
        characterController.Move(currentMovement * speed * Time.deltaTime);
        handleGravity();
        handleRotation();
    }
}
