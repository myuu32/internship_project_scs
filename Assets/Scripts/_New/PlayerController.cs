using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float attackForce = 10f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private float sprintSpeed = 10f;
    [SerializeField] private float sprintTime = 2f;
    [SerializeField] private float sprintCooldown = 2f;
    [SerializeField] private float slideSpeedMultiplier = 0.5f;
    [SerializeField] private float aimspeed = 20f;

    private Transform aimTarget;
    private bool hittingLeft;
    private bool hittingRight;
    private Animator animator;
    private GameObject ball;

    private CharacterController controller;
    private Vector2 moveInput;
    private float turnSmoothVelocity;
    private bool isSprinting = false;
    private bool isSliding = false;
    private float sprintTimer = 0f;
    private float cooldownTimer = 0f;

    private ServeManager serveManager;
    private Serve currentServe;
    private bool hitting = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        serveManager = GetComponent<ServeManager>();
        currentServe = serveManager.flat;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started && !isSprinting && cooldownTimer <= 0) StartSprint();
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            hitting = true;
            currentServe = serveManager.flat;
        }
        else if (context.canceled) hitting = false;
    }

    public void OnHitLeft(InputAction.CallbackContext context)
    {
        hittingLeft = context.ReadValueAsButton();
    }

    public void OnHitRight(InputAction.CallbackContext context)
    {
        hittingRight = context.ReadValueAsButton();
    }

    private void Update()
    {
        MovePlayer();

        if (isSprinting) UpdateSprint();
        else if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isSliding) UpdateSlide();
        
        StartCoroutine(FindAimTarget());

        float h = moveInput.x;
        float v = moveInput.y;

        if (hittingLeft)
        {
            aimTarget.Translate(Vector3.left * aimspeed * 2 * Time.deltaTime); 
        }

        if (hittingRight)
        {
            aimTarget.Translate(Vector3.right * aimspeed * 2 * Time.deltaTime);
        }

        if ((h != 0 || v != 0) && !hittingLeft && !hittingRight)
        {
            transform.Translate(new Vector3(h, 0, v) * aimspeed * Time.deltaTime);
        }

        StartCoroutine(FindBallPos());


    }

    private System.Collections.IEnumerator FindAimTarget()
    {
        while (true)
        {
            int playerIndex = GetComponent<PlayerInput>().playerIndex;

            if (playerIndex == 0)
            {
                aimTarget = GameObject.FindGameObjectWithTag("AimTargetA").transform;
            }
            else if (playerIndex == 1)
            {
                aimTarget = GameObject.FindGameObjectWithTag("AimTargetB").transform;
            }

            if (aimTarget != null)
            {
                yield break;
            }

            yield return null;
        }
    }

    System.Collections.IEnumerator FindBallPos()
    {
        while (true)
        {
            if (ball == null)
            {
                ball = GameObject.FindGameObjectWithTag("TennisBall");
            }
            yield return null;
        }
    }

    private void MovePlayer()
    {
        Vector3 moveDirection = new Vector3(moveInput.x, 0f, moveInput.y).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            Vector3 desiredDirection = cameraForward * moveInput.y + cameraRight * moveInput.x;

            float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            controller.Move(desiredDirection.normalized * moveSpeed * Time.deltaTime);
        }

        ApplyGravity();
    }

    private void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * gravity * Time.deltaTime);
        }
    }

    private void StartSprint()
    {
        isSprinting = true;
        sprintTimer = sprintTime;
        cooldownTimer = sprintCooldown;
        StartSlide();
    }

    private void UpdateSprint()
    {
        sprintTimer -= Time.deltaTime;
        if (sprintTimer <= 0)
        {
            EndSprint();
        }
        else
        {
            controller.Move(transform.forward * sprintSpeed * Time.deltaTime);
        }
    }

    private void EndSprint()
    {
        isSprinting = false;
        isSliding = false;
    }

    private void Attack()
    {
        Debug.Log("Attack with force: " + attackForce);
    }

    private void StartSlide()
    {
        isSliding = true;
    }

    private void UpdateSlide()
    {
        controller.Move(transform.forward * sprintSpeed * slideSpeedMultiplier * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("TennisBall") && hitting)
        {
            Vector3 dir = aimTarget.position - transform.position;
            other.GetComponent<Rigidbody>().velocity = dir.normalized * currentServe.hitForce + new Vector3(0, currentServe.upForce, 0);

            Vector3 ballDir = ball.transform.position - transform.position;
            if (ballDir.x >= 0) animator.Play("Forehand");
            else animator.Play("Backhand");

            hitting = false;
        }
    }
}
