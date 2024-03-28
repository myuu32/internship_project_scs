using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float[] moveSpeeds; // 不同玩家ID的移動速度
    public float[] turnSmoothTimes; // 不同玩家ID的轉彎平滑時間
    public float[] attackForces; // 不同玩家ID的攻擊力量
    public float[] sprintSpeeds; // 不同玩家ID的衝刺速度
    public float[] sprintTimes; // 不同玩家ID的衝刺時間
    public float[] sprintCooldowns; // 不同玩家ID的衝刺冷卻時間
    public float[] slideSpeedMultipliers; // 不同玩家ID的滑行速度乘數
    public float[] aimSpeeds; // 不同玩家ID的瞄準速度

    [SerializeField] private float gravity = 9.81f;


    private CharacterController controller;
    private Transform aimTarget;
    private bool hittingLeft;
    private bool hittingRight;
    private Animator animator;
    private GameObject ball;

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
        StartCoroutine(FindBallPos());

        float h = moveInput.x;
        float v = moveInput.y;

        if (hittingLeft)
        {
            aimTarget.Translate(Vector3.left * aimSpeeds[1] * 2 * Time.deltaTime);
        }

        if (hittingRight)
        {
            aimTarget.Translate(Vector3.right * aimSpeeds[1] * 2 * Time.deltaTime);
        }

        if ((h != 0 || v != 0) && !hittingLeft && !hittingRight)
        {
            transform.Translate(new Vector3(h, 0, v) * aimSpeeds[0] * Time.deltaTime);
        }
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

        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        if (playerIndex == 1)
        {
            hittingRight = hittingLeft;
            hittingLeft = false;
        }
    }

    public void OnHitRight(InputAction.CallbackContext context)
    {
        hittingRight = context.ReadValueAsButton();

        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        if (playerIndex == 1)
        {
            hittingLeft = hittingRight;
            hittingRight = false;
        }
    }

    private void MovePlayer()
    {
        Vector3 moveInputDirection = new Vector3(moveInput.x, 0, moveInput.y);

        int playerIndex = GetComponent<PlayerInput>().playerIndex;

        float currentMoveSpeed = moveSpeeds[playerIndex];

        if (playerIndex == 1 && moveInputDirection.magnitude >= 0.1f)
        {
            moveInputDirection *= -1f;
        }

        Vector3 moveVector = moveInputDirection * currentMoveSpeed * Time.deltaTime;
        CollisionFlags flags = controller.Move(moveVector);

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
        sprintTimer = sprintTimes[0];
        cooldownTimer = sprintCooldowns[0];
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
            controller.Move(transform.forward * sprintSpeeds[0] * Time.deltaTime);
        }
    }

    private void EndSprint()
    {
        isSprinting = false;
        isSliding = false;
    }

    private void StartSlide()
    {
        isSliding = true;
    }

    private void UpdateSlide()
    {
        controller.Move(transform.forward * sprintSpeeds[0] * slideSpeedMultipliers[0] * Time.deltaTime);
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
}
