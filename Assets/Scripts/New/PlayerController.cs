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
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started && !isSprinting && cooldownTimer <= 0)
        {
            StartSprint();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            Attack();
        }
    }

    void Update()
    {
        MovePlayer();

        if (isSprinting)
        {
            UpdateSprint();
        }
        else if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }

        if (isSliding)
        {
            UpdateSlide();
        }
    }

    void MovePlayer()
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

    void ApplyGravity()
    {
        if (!controller.isGrounded)
        {
            controller.Move(Vector3.down * gravity * Time.deltaTime);
        }
    }

    void StartSprint()
    {
        isSprinting = true;
        sprintTimer = sprintTime;
        cooldownTimer = sprintCooldown;
        StartSlide(); // Start sliding when sprinting starts
    }

    void UpdateSprint()
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

    void EndSprint()
    {
        isSprinting = false;
        // End sliding when sprinting ends
        isSliding = false;
    }

    void Attack()
    {
        Debug.Log("Attack with force: " + attackForce);
    }

    void StartSlide()
    {
        isSliding = true;
    }

    void UpdateSlide()
    {
        // Move the player forward with reduced speed while sliding
        controller.Move(transform.forward * sprintSpeed * slideSpeedMultiplier * Time.deltaTime);
    }
}
