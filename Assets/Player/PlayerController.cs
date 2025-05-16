using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    private float originalColliderHeight;
    private Vector3 originalColliderCenter;

    public KeyManager keyManager;

    [Header("Movement Settings")]
    public float acceleration = 0.4f;
    public float maxSpeed = 15f;
    public float deceleration = 0.2f;
    private float currentSpeed = 0f;

    [Header("Jump Settings")]
    public float jumpForce = 20f;
    private bool isGrounded;

    [Header("Ground & Wall Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.2f;
    public LayerMask groundLayer;

    private Rigidbody rb;
    private Vector3 moveDirection;

    [Header("Crouch Settings")]
    public float crouchScale = 0.5f;
    public float crouchSpeed = 8f;
    public float standScale = 1f;
    public bool isCrouching = false;
    private float originalSpeed;
    private CapsuleCollider playerCollider;

    [Header("Sprint Settings")]
    public float sprintMultiplier = 1.5f;
    public float maxSprintTime = 2f;
    public float sprintCooldown = 2f;
    public float sprintRegenRate = 1f;
    public float sprintRemaining;
    private bool isSprinting = false;
    private bool sprintOnCooldown = false;

    [Header("Low Surface Detection")]
    public float lowSurfaceCrouchScale = 0.2f;
    public bool nearLowSurface = false;

    [Header("Throwing Settings")]
    public GameObject throwablePrefab; 
    public Transform throwPoint;       
    public float throwForce = 200f; 

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        originalSpeed = maxSpeed;
        sprintRemaining = maxSprintTime;

        originalColliderHeight = playerCollider.height;
        originalColliderCenter = playerCollider.center;
        keyManager = Camera.main.gameObject.GetComponent<KeyManager>();
    }

    void Update()
    {
        HandleGroundCheck();
        ProcessInput();
        HandleCrouch();
        HandleSprint();
        RegenerateSprint();
        HandleThrowing(); 
    }

    void FixedUpdate()
    {
        MovePlayer();
    }

    void ProcessInput()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        moveDirection = new Vector3(horizontal, 0f, vertical).normalized;
    }

    void HandleSprint()
    {
        if (sprintOnCooldown || sprintRemaining <= 0)
        {
            isSprinting = false;
            return;
        }

        if (Input.GetKey(KeyCode.LeftShift) && sprintRemaining > 0)
        {
            isSprinting = true;
            sprintRemaining -= Time.deltaTime;
            if (sprintRemaining <= 0)
            {
                StartCoroutine(StartSprintCooldown());
            }
        }
        else
        {
            isSprinting = false;
        }
    }

    void MovePlayer()
    {
        float speed = isSprinting ? maxSpeed * sprintMultiplier : maxSpeed;

        if (moveDirection.magnitude > 0)
        {
            currentSpeed = Mathf.Min(currentSpeed + acceleration, speed);
        }
        else
        {
            currentSpeed = Mathf.Max(currentSpeed - deceleration, 0);
        }

        Vector3 movement = transform.TransformDirection(moveDirection) * currentSpeed;
        rb.velocity = new Vector3(movement.x, rb.velocity.y, movement.z);
    }

    IEnumerator StartSprintCooldown()
    {
        sprintOnCooldown = true;
        yield return new WaitForSeconds(sprintCooldown);
        sprintOnCooldown = false;
    }

    void RegenerateSprint()
    {
        if (!isSprinting && !sprintOnCooldown && sprintRemaining < maxSprintTime)
        {
            sprintRemaining += sprintRegenRate * Time.deltaTime;
            sprintRemaining = Mathf.Min(sprintRemaining, maxSprintTime);
            Debug.Log("Sprint Regen: " + sprintRemaining.ToString("F2"));
        }
    }

    void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    void HandleCrouch()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (isCrouching)
            {
                TryStandUp();
            }
            else
            {
                StartCrouch();
            }
        }

        if (isCrouching && nearLowSurface)
        {

            transform.localScale = new Vector3(1f, lowSurfaceCrouchScale, 1f);
            playerCollider.height *= lowSurfaceCrouchScale;
        }
    }

    void StartCrouch()
    {
        if (isCrouching) return;

        isCrouching = true;
        transform.localScale = new Vector3(1f, crouchScale, 1f);
        playerCollider.height = originalColliderHeight * crouchScale;
        playerCollider.center = new Vector3(originalColliderCenter.x, originalColliderCenter.y * crouchScale, originalColliderCenter.z);
        maxSpeed = crouchSpeed;
    }


    void TryStandUp()
    {
        if (Physics.Raycast(transform.position, Vector3.up, 1f, groundLayer) || nearLowSurface)
        {
            Debug.Log("Can't stand up! Something is above.");
            return;
        }

        isCrouching = false;
        transform.localScale = new Vector3(1f, standScale, 1f);
        playerCollider.height = originalColliderHeight;
        playerCollider.center = originalColliderCenter;
        maxSpeed = originalSpeed;
    }


    void HandleThrowing()
    {
        if (Input.GetKeyDown(KeyCode.G)) 
        {
            if (throwablePrefab == null || throwPoint == null)
            {
                Debug.LogWarning("Throwable prefab or throw point is not assigned");
                return;
            }
            if (keyManager.noiseMakerAmount > 0){
            GameObject throwable = Instantiate(throwablePrefab, throwPoint.position, throwPoint.rotation);
            Rigidbody rb = throwable.GetComponent<Rigidbody>();

            if (rb != null)
            {
                rb.AddForce(throwPoint.forward * throwForce, ForceMode.VelocityChange);
            }
            keyManager.noiseMakerAmount--;
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("LowSurface"))
        {
            nearLowSurface = true;
        }
    }
private void OnTriggerExit(Collider other)
{
    if (other.CompareTag("LowSurface"))
    {
        nearLowSurface = false;

        if (isCrouching)
        {
            transform.localScale = new Vector3(1f, crouchScale, 1f);
            playerCollider.height = originalColliderHeight * crouchScale;
            playerCollider.center = new Vector3(originalColliderCenter.x, originalColliderCenter.y * crouchScale, originalColliderCenter.z);
        }
    }
}

}
