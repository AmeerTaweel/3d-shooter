using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class handles the movement of the player with given input from the input manager
/// </summary>
public class PlayerController : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("The speed at which the player moves")]
    public float moveSpeed = 2f;
    [Tooltip("The speed at which the player rotates (in degrees)")]
    public float lookSpeed = 60f;
    [Tooltip("The power at which the player jumps")]
    public float jumpPower = 8f;
    [Tooltip("The strength of gravity")]
    public float gravity = 9.81f;
    [Header("Required References")]
    [Tooltip("The player shooter script that fires projectiles")]
    public Shooter playerShooter;

    public Health health;
    public List<GameObject> disableWhileDead;

    // The character controller component on the player
    private CharacterController controller;
    private InputManager inputmanager;

    private bool doubleJumpAvailable = false;

    /// <summary>
    /// Description:
    /// Standard Unity function called once before the first Update call
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Start()
    {
        SetupCharacterController();
        SetupInputManager();
    }

    void SetupCharacterController()
    {
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            Debug.LogError("CharacterController component not found on player.");
        }
    }

    void SetupInputManager()
    {
        inputmanager = InputManager.instance;
    }

    /// <summary>
    /// Description:
    /// Standard Unity function called once every frame
    /// Input:
    /// none
    /// Return:
    /// void (no return)
    /// </summary>
    void Update()
    {
        if (health.currentHealth < 0)
        {
            foreach (var obj in disableWhileDead)
            {
                obj.SetActive(false);
            }

            return;
        }
        foreach (var obj in disableWhileDead)
        {
            obj.SetActive(true);
        }
        ProcessMovement();
        ProcessRotation();
    }

    private Vector3 moveDirection;
    void ProcessMovement()
    {
        float horizontalMovement = inputmanager.horizontalMoveAxis;
        float verticalMovement = inputmanager.verticalMoveAxis;
        bool isJumpPressed = inputmanager.jumpPressed;

        if (controller.isGrounded)
        {
            doubleJumpAvailable = true;
            moveDirection = new Vector3(horizontalMovement, 0, verticalMovement);
            moveDirection = transform.TransformDirection(moveDirection);
            moveDirection *= moveSpeed;

            if (isJumpPressed)
            {
                moveDirection.y = jumpPower;
            }
        }
        else
        {
            moveDirection = new Vector3(horizontalMovement * moveSpeed, moveDirection.y, verticalMovement * moveSpeed);
            moveDirection = transform.TransformDirection(moveDirection);
            
            if (isJumpPressed && doubleJumpAvailable)
            {
                moveDirection.y = jumpPower;
                doubleJumpAvailable = false;
            }
        }

        moveDirection.y -= gravity * Time.deltaTime;

        if (controller.isGrounded && moveDirection.y < 0)
        {
            moveDirection.y = -0.3f;
        }
        
        controller.Move(moveDirection * Time.deltaTime);
    }

    void ProcessRotation()
    {
        float horizontalRotationInput = inputmanager.horizontalLookAxis;
        Vector3 playerRotation = transform.rotation.eulerAngles;
        transform.rotation = Quaternion.Euler(new Vector3(
            playerRotation.x,
            playerRotation.y + horizontalRotationInput * lookSpeed * Time.deltaTime,
            playerRotation.z
        ));
    }
}
