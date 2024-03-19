using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f; // 移動速度
    [SerializeField] private float turnSmoothTime = 0.1f; // 轉向平滑時間
    [SerializeField] private float attackForce = 10f; // 攻擊力道
    [SerializeField] private float gravity = 9.81f; // 重力
    [SerializeField] private float sprintSpeed = 10f; // 衝刺速度
    [SerializeField] private float sprintTime = 2f; // 衝刺時間
    [SerializeField] private float sprintCooldown = 2f; // 衝刺冷卻時間
    [SerializeField] private float slideSpeedMultiplier = 0.5f; // 滑行速度乘數

    [SerializeField] public float aimspeed = 3f;
    [SerializeField] public float hitForce = 40f;

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

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
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
        if (context.started) Attack();
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

        if (isSliding)
        {
            UpdateSlide();
        }

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
        StartSlide(); // Start sliding when sprinting starts
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
        // End sliding when sprinting ends
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
        // Move the player forward with reduced speed while sliding
        controller.Move(transform.forward * sprintSpeed * slideSpeedMultiplier * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TennisBall"))
        {
            Debug.Log("Rerve");
            Vector3 dir = aimTarget.position - transform.position;
            other.GetComponent<Rigidbody>().velocity = dir.normalized * hitForce + new Vector3(0, 45, 0);

            Vector3 ballDir = ball.transform.position - transform.position;
            if (ballDir.x >= 0) animator.Play("Forehand");
            else animator.Play("Backhand");
        }
    }
}
