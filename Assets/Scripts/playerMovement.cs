using System.Net;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class playerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 4.0f;
    public float sprintSpeed = 8.0f;
    public float airSpeedMultiplier = 0.1f;
    public float groundDrag;
    private float universalDrag;
    public Transform orientation;
    private float tempMaxSpeed;
    [SerializeField] private float airSpeedThreshold;

    InputAction moveAction;
    InputAction sprintAction;
    private float xMove;
    private float yMove;
    private Vector3 moveDirection = Vector3.zero;

    [Header("Ground Check")]
    public float playerHeight;
    private float tempPlayerHeight;
    public LayerMask whatIsGround;
    bool grounded;
    private RaycastHit hit;
    public float sphereCastMultiplier = 1.0f;
    public float rayCastMultiplier = 1.0f;
    public float sphereCastRadius = 1.0f;

    [Header("Jump")]
    public float jumpForce = 1.0f;
    InputAction jumpAction;
    public float jumpSpeedMultiplier = 1.0f;
    public float airResistance = 0.0f;
    private float groundSpeed;
    public float highJumpMultiplier = 2.0f;
    private bool jumpedOnce = false;
    private float jumpDelay = 0.2f;
    private float elapsedTime = 0.0f;
    private Vector3 flatVel;

    [Header("Crouch")]
    public float crouchPlayerHeightMultiplier = 0.5f;
    public float crouchGroundDrag;
    public float crouchSpeed;
    InputAction crouchAction;
    public GameObject player;
    [SerializeField] private float crouchImpulse;
    [SerializeField] private float impulseDuration;
    private bool didAddImpulse = false;

    [Header("Gun Stuff")]
    public bool isAiming = false;
    public Vector3 moveVelocity;

    [Header("Wall run")]
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float wallJumpSpeed;
    [SerializeField, Range(0f, 2f)] private float wallDetectLength;
    [SerializeField] private float wallHugForce;
    private bool isWallRun = false;
    private Vector3 wallNormal;
    

    private Rigidbody rb;

    private void Start()
    {
        moveAction = InputSystem.actions.FindAction("Move");
        jumpAction = InputSystem.actions.FindAction("Jump");
        sprintAction = InputSystem.actions.FindAction("Sprint");
        crouchAction = InputSystem.actions.FindAction("Crouch");
        rb= GetComponent<Rigidbody>();
        tempPlayerHeight = playerHeight;
    }

    private void Update()
    {
        moveVelocity = rb.linearVelocity;
        elapsedTime += Time.deltaTime;
        if (elapsedTime > jumpDelay)
        {
            jumpedOnce = false;
            elapsedTime = 0.0f;
        }
        if (jumpAction.WasPressedThisFrame() && grounded && !jumpedOnce)
        {
            Jump();
            jumpedOnce=true;
            elapsedTime=0.0f;
        }
        else if (jumpAction.IsInProgress() && grounded && !jumpedOnce)
        {
            Jump();
            jumpedOnce = true;
            elapsedTime=0.0f;
        }
        
        if (crouchAction.WasPressedThisFrame())
        {
            CrouchStart();
        }
        else if (crouchAction.WasReleasedThisFrame())
        {
            CrouchStop();
        }
        if (crouchAction.IsInProgress())
            Crouching();
        else
            universalDrag = groundDrag;
        
    }
    private void FixedUpdate()
    {
        flatVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        
        if (!crouchAction.IsInProgress())
        {
            if (sprintAction.IsInProgress() && !isAiming)
                RunWalkControl(sprintSpeed);
            else
                RunWalkControl(walkSpeed);
        }
        else
        {
            RunWalkControl(crouchSpeed);
        }
        if (grounded) AntiGravity();
        else WallRun();
        GroundJumpMech(universalDrag);
    }
    private void RunWalkControl(float speed)
    {
        if (grounded)
        {
            Vector3 temp = new(rb.linearVelocity.x, 0, rb.linearVelocity.z);
            groundSpeed = temp.magnitude;
        }
        airSpeedMultiplier = Mathf.Clamp(airSpeedMultiplier, 0.0f, 1.0f);
        if (grounded)
            Movement(1.0f, speed, false);
        else
            Movement(airSpeedMultiplier, groundSpeed, true);
    }
    
    private void GroundJumpMech(float groundDrag)
    {
        grounded = Physics.SphereCast(transform.position, sphereCastRadius, Vector3.down, out _, (playerHeight * 0.5f * sphereCastMultiplier), whatIsGround);
        Physics.Raycast(transform.position, Vector3.down, out hit, (playerHeight * 0.5f * rayCastMultiplier), whatIsGround);
        

        if (grounded)
        {
            rb.linearDamping = groundDrag;
            if (!crouchAction.IsInProgress())
            {
                if (sprintAction.IsInProgress())
                {
                    LimitSpeed(sprintSpeed);
                    tempMaxSpeed = sprintSpeed;
                }
                else
                {
                    LimitSpeed(walkSpeed);
                    tempMaxSpeed = walkSpeed;
                }
            }
            else
            {
                tempMaxSpeed = flatVel.magnitude;
            }
        }
        else
        {
            rb.linearDamping = 0f;
            if (tempMaxSpeed < 3f) tempMaxSpeed = 3f;
            LimitSpeed(tempMaxSpeed * jumpSpeedMultiplier);
        }
    }
    private void Movement(float multiplier, float moveSpeed, bool isInAir)
    {
        xMove = moveAction.ReadValue<Vector2>().x;
        yMove = moveAction.ReadValue <Vector2>().y;
        moveDirection.x = xMove;
        moveDirection.z = yMove;
        moveDirection = Vector3.ProjectOnPlane(orientation.forward, hit.normal) * moveDirection.z + Vector3.ProjectOnPlane(orientation.right, hit.normal) * moveDirection.x;
        float tempMultiplier = multiplier;
        float moveSpeedTemp = moveSpeed;
        if (isInAir && moveSpeedTemp < airSpeedThreshold)
        {
            tempMultiplier = 0.5f;
            moveSpeedTemp = airSpeedThreshold;
        }
        else if (isInAir && flatVel.magnitude >= airSpeedThreshold) tempMultiplier = multiplier;

        rb.AddForce(10f * moveSpeedTemp * tempMultiplier * moveDirection.normalized * Time.fixedDeltaTime, ForceMode.VelocityChange);
    }
    private void LimitSpeed(float speed)
    {
        if (flatVel.magnitude > speed && flatVel.magnitude > 3)
        {
            Vector3 limitedVel = flatVel.normalized * speed;
            Vector3 newVel = new Vector3(limitedVel.x, rb.linearVelocity.y, limitedVel.z);
            rb.linearVelocity = newVel;
        }
    }
    private void Jump()
    {
        float verticalVel = rb.linearVelocity.y;
        if (flatVel.magnitude > 8.0f)
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * (jumpForce + (Mathf.Abs(verticalVel) * highJumpMultiplier)), ForceMode.Impulse);
        }
        else
        {
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        
    }
    private void AntiGravity()
    {
        float gravForce = Physics.gravity.magnitude * rb.mass;
        rb.AddForce(Vector3.up * gravForce * hit.normal.magnitude, ForceMode.Force);
    }
    private void CrouchStart()
    {
        player.transform.localScale = new Vector3(player.transform.localScale.x, 0.5f, player.transform.localScale.z);
        playerHeight *= crouchPlayerHeightMultiplier;
    }
    private void Crouching() { universalDrag = crouchGroundDrag; }
    private void CrouchStop()
    {
        player.transform.localScale = new Vector3(player.transform.localScale.x, 1f, player.transform.localScale.z);
        playerHeight = tempPlayerHeight;
    }
    private void WallRun()
    {
        RaycastHit wallHit;
        if (Physics.Raycast(orientation.transform.position, orientation.transform.right, out wallHit, wallDetectLength)) isWallRun = true;
        else if (Physics.Raycast(orientation.transform.position, -(orientation.transform.right), out wallHit, wallDetectLength)) isWallRun = true;
        else isWallRun = false;
        //isWallRun = Physics.Raycast(orientation.transform.position, -(orientation.transform.right), out wallHit, wallDetectLength);
        if (isWallRun)
        {
            //rb.AddForce(-wallHit.normal * wallHugForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
            //airSpeedMultiplier = 1f;
        }

    }
    private void OnDrawGizmos()
    {
        /*Vector3 endPoint = transform.position + (Vector3.down * (playerHeight * 0.5f * sphereCastMultiplier));
        Vector3 endPoint1 = transform.position + (Vector3.down * (playerHeight * 0.5f * rayCastMultiplier));
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(endPoint, sphereCastRadius);
        Gizmos.DrawLine(transform.position, (transform.position + moveDirection));
        Gizmos.DrawLine(transform.position, endPoint1);
        Gizmos.DrawLine(transform.position, (transform.position + hit.normal));*/
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(orientation.transform.position, ((orientation.transform.position + orientation.transform.right * wallDetectLength)));
        Gizmos.DrawLine(orientation.transform.position, ((orientation.transform.position - orientation.transform.right * wallDetectLength)));
    }
    
}
