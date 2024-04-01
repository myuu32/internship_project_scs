
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float[] moveSpeeds;
    public float[] turnSmoothTimes;
    public float[] attackForces;
    public float[] sprintSpeeds;
    public float[] sprintTimes;
    public float[] sprintCooldowns;
    public float[] slideSpeedMultipliers;
    public float[] aimSpeeds;

    public GameObject[] playerModels;
    public Animator[] animator;
    public float[][] modelScales = { new float[] { 0.02f, 0.015f, 0.02f }, new float[] { 0.02f, 0.015f, 0.02f }, new float[] { 0.02f, 0.015f, 0.02f } }; public Vector3 modelPosition = Vector3.zero;


    [SerializeField] private float gravity = 9.81f;

    private CharacterController controller;
    private Transform aimTarget;
    private bool hittingLeft;
    private bool hittingRight;
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
        serveManager = GetComponent<ServeManager>();
        currentServe = serveManager.flat;

        animator = new Animator[playerModels.Length];
        for (int i = 0; i < playerModels.Length; i++)
        {
            animator[i] = playerModels[i].GetComponent<Animator>();
        }
    }

    private void Start()
    {
        int playerIndex = GetComponent<PlayerInput>().playerIndex;

        if (playerIndex >= 0 && playerIndex < playerModels.Length)
        {
            GameObject selectedModel = playerModels[playerIndex];

            GameObject instantiatedModel = Instantiate(selectedModel, transform.position, transform.rotation, transform);

            instantiatedModel.transform.localPosition = modelPosition;
            instantiatedModel.transform.localScale = new Vector3(modelScales[playerIndex][0], modelScales[playerIndex][1], modelScales[playerIndex][2]);
            instantiatedModel.transform.localRotation = Quaternion.identity;

            animator[playerIndex] = instantiatedModel.GetComponent<Animator>();
        }
        else
        {
            Debug.LogError("Player index out of range for player models array!");
        }
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

        bool isMoving = moveInputDirection.magnitude >= 0.1f;

        animator[playerIndex].SetBool("onRun", isMoving);

        if (playerIndex == 1 && isMoving)
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

            int playerIndex = GetComponent<PlayerInput>().playerIndex;

            if (ballDir.x >= 0)
            {
                animator[playerIndex].Play("Forehand");
            }
            else
            {
                animator[playerIndex].Play("Backhand");
            }

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