using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public float jumpHeight = 2f;
    public float rotationSmoothTime = 0.1f;
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private Animator animator;

    [SerializeField] private bool hasWeapon = false; // Flag to check if the player has a weapon

    private float targetAngle;
    private float rotationSmoothVelocity;
    private Vector3 moveVelocity;
    private float maxSpeed;

    public WeaponController weaponController;

    void Start()
    {
        // Find camera if not assigned
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }

        // Ensure animator is assigned
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        maxSpeed = runSpeed; // For normalizing Speed parameter

        if (hasWeapon)
        {
            SetUpperBodyLayerWeight(1);
        }
        else
        {
            SetUpperBodyLayerWeight(0);
        }
    }

    void Update()
    {
        HandleMovement();
        HandleShooting();
        UpdateAnimator();
        HandleReload();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        Vector3 moveDirection = Vector3.zero;

        if (direction.magnitude >= 0.1f)
        {
            // Calculate target angle based on camera direction
            targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;

            // Smoothly rotate the player to face the movement direction
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref rotationSmoothVelocity, rotationSmoothTime);
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.up);

            // Move in the direction the player is now facing
            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            float speed = Input.GetKey(KeyCode.LeftShift) ? runSpeed : walkSpeed;
            moveDirection *= speed;
        }

        // Handle jumping
        if (characterController.isGrounded && Input.GetKeyDown(KeyCode.Space))
        {
            moveVelocity.y = Mathf.Sqrt(2 * jumpHeight * Mathf.Abs(Physics.gravity.y));
            animator.SetTrigger("Jump"); // Trigger jump animation
        }
        else
        {
            moveVelocity.y += Physics.gravity.y * Time.deltaTime;
        }

        moveDirection.y = moveVelocity.y;
        characterController.Move(moveDirection * Time.deltaTime);
    }

    private void UpdateAnimator()
    {
        // Calculate horizontal speed for animation
        float currentSpeed = new Vector2(characterController.velocity.x, characterController.velocity.z).magnitude;
        float normalizedSpeed = currentSpeed / maxSpeed;

        // Smooth speed to avoid abrupt animation changes
        float smoothedSpeed = Mathf.Lerp(animator.GetFloat("Speed"), normalizedSpeed, Time.deltaTime * 10f);
        animator.SetFloat("Speed", smoothedSpeed);
        animator.SetBool("IsGrounded", characterController.isGrounded);
    }

    public void HandleShooting()
    {
        bool isFiring = hasWeapon && characterController.isGrounded && Input.GetMouseButton(0) && !animator.GetBool("IsReloading");
        animator.SetBool("IsFiring", isFiring);
        if (isFiring)
        {
            weaponController.StartAiming();
        }
        else
        {
            weaponController.StopAiming();
        }
    }
    public void HandleReload()
    {
        if (hasWeapon && Input.GetKeyDown(KeyCode.R) && !animator.GetBool("IsReloading") && !animator.GetBool("IsFiring"))
        {
            animator.SetTrigger("Reload");
            animator.SetBool("IsReloading", true);
            Debug.Log("Player is reloading!");
            StartCoroutine(ReloadComplete());
        }
    }
    IEnumerator ReloadComplete()
    {
        yield return new WaitForSeconds(4.2f); // Simulate reload time
        animator.SetBool("IsReloading", false);
        Debug.Log("Reload complete!");
    }
    private void SetUpperBodyLayerWeight(float weight)
    {
        int upperBodyLayerIndex = animator.GetLayerIndex("UpperBody");
        if (upperBodyLayerIndex != -1)
        {
            animator.SetLayerWeight(upperBodyLayerIndex, weight);
        }
        else
        {
            Debug.LogWarning("UpperBody layer not found in animator.");
        }
    }
}