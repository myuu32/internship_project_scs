using UnityEngine;
using System.Collections;
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
    private string pointATag = "PointA";
    private string pointBTag = "PointB";

    private Vector2 moveInput;
    private bool hasReset = false;
    private bool isSprinting = false;
    private bool isSliding = false;
    private float sprintTimer = 0f;
    private float cooldownTimer = 0f;

    private ServeManager serveManager;
    private Serve currentServe;
    private bool hitting = false;

    [Header("Slow Motion Parameters")]
    public float slowdownTimeLength = 0.5f;
    public float slowdownDuration = 2f;
    public float slowdownDelay = 0.5f;

    [Header("Camera Shake Parameters")]
    public float shakeIntensity = 0.2f;
    public float shakeDuration = 0.1f;
    public float shakeDelay = 0.5f;

    //[Header("Camera Zoom In/ Out ")]
    public float cameraZoomAmount = 0.5f;
    //public Vector3[] offsets;

    public CameraManager_2P cameraManager;
    public float[] offsetChangeAmounts; // 偏移量變化的量
    public float[] offsetChangeDelays; // 偏移量變化的延遲時間
    public float[] offsetChangeDurations; // 偏移量變化的持續時間

    private Vector3 originalCameraOffset; // 用於儲存原始偏移量的變量
    private bool isHitting = false;
    public Camera camera;
    public Transform origintransform;
    public Camera originalCamera; // 原始摄像机
    private Camera zoomCamera; // 用于拉近效果的摄像机

    private bool isSlowMotionActive = false; // 標誌位，檢查慢動作是否已經在進行中


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

        if (cameraManager != null && playerIndex >= 0 && playerIndex < cameraManager.offsets.Length)
        {
            originalCameraOffset = cameraManager.offsets[playerIndex];
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

        if (!hasReset)
        {
            int playerIndex = GetComponent<PlayerInput>().playerIndex;
            if (playerIndex >= 1) 
            {
                ResetPoint();
                hasReset = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.F5))
        {
            ResetPoint();
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

    public void ResetCameraOffset()
    {
        int playerIndex = GetComponent<PlayerInput>().playerIndex;
        if (cameraManager != null && playerIndex >= 0 && playerIndex < cameraManager.offsets.Length)
        {
            cameraManager.offsets[playerIndex] = originalCameraOffset;
        }
    }

    public void ResetPoint()
    {
        int playerIndex = GetComponent<PlayerInput>().playerIndex;

        if (playerIndex == 0)
        {
            GameObject pointA = GameObject.FindGameObjectWithTag(pointATag);
            if (pointA != null)
            {
                gameObject.transform.position = pointA.transform.position;
            }
            else
            {
                Debug.LogWarning("PointA not found.");
            }
        }
        else if (playerIndex == 1)
        {
            GameObject pointB = GameObject.FindGameObjectWithTag(pointBTag);
            if (pointB != null)
            {
                gameObject.transform.position = pointB.transform.position;
            }
            else
            {
                Debug.LogWarning("PointB not found.");
            }
        }
    }

    /*
    public void StartZoomIn()
    {
        camera.transform.SetParent(transform);
        Debug.Log("ZoomIn");
    }

    public void StopZoomIn()
    {
        camera.transform.SetParent(origintransform);
        Debug.Log("ZoomOut");
    }
    */

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
            if (!isHitting)
            {
                isHitting = true;

                StartCoroutine(SlowMotionEffect(slowdownTimeLength, slowdownDuration, slowdownDelay));
                StartCoroutine(CameraShake(shakeIntensity, shakeDuration, shakeDelay));

                Vector3 dir = aimTarget.position - transform.position;
                Vector3 ballDir = ball.transform.position - transform.position;

                other.GetComponent<Rigidbody>().velocity = dir.normalized * currentServe.hitForce + new Vector3(0, currentServe.upForce, 0);

                int playerIndex = GetComponent<PlayerInput>().playerIndex;

                if (ballDir.x >= 0)
                {
                    if (playerIndex == 0) animator[playerIndex].Play("Forehand");
                    if (playerIndex == 1) animator[playerIndex].Play("Backhand");
                    GetComponent<Animator>().Play("onForehand");
                }
                else
                {
                    if (playerIndex == 0) animator[playerIndex].Play("Backhand");
                    if (playerIndex == 1) animator[playerIndex].Play("Forehand");
                    GetComponent<Animator>().Play("onForehand");
                }

                StartCoroutine(ChangeCameraOffset(offsetChangeAmounts[playerIndex], offsetChangeDelays[playerIndex], offsetChangeDurations[playerIndex]));

                isHitting = false;
                hitting = false;
            }
        }
    }


    private IEnumerator ChangeCameraOffset(float amount, float delay, float duration)
    {
        yield return new WaitForSeconds(delay);

        if (cameraManager != null && cameraManager.offsets != null && cameraManager.offsets.Length >= 2)
        {
            int playerIndex = GetComponent<PlayerInput>().playerIndex;

            if (playerIndex >= 0 && playerIndex < cameraManager.offsets.Length)
            {
                Vector3 currentOffset = cameraManager.offsets[playerIndex];
                Vector3 originalOffset = currentOffset;

                currentOffset.z += amount;
                cameraManager.offsets[playerIndex] = currentOffset;

                yield return new WaitForSeconds(duration);
                cameraManager.offsets[playerIndex] = originalOffset;

                ResetCameraOffset();
            }
        }
    }

    private IEnumerator SlowMotionEffect(float slowdownTimeLength, float duration, float delay)
    {
        if (isSlowMotionActive)
        {
            yield break;
        }

        isSlowMotionActive = true;

        yield return new WaitForSeconds(delay);

        float originalTimeScale = Time.timeScale;
        Time.timeScale = slowdownTimeLength;

        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = originalTimeScale;
        isSlowMotionActive = false;
    }

    private IEnumerator CameraShake(float intensity, float duration, float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 originalCamPos = camera.transform.localPosition;

        float elapsed = 0.0f;
        float zoom = 1.0f - cameraZoomAmount;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * intensity * zoom;
            float y = Random.Range(-1f, 1f) * intensity * zoom;

            camera.transform.localPosition = new Vector3(originalCamPos.x + x, originalCamPos.y + y, originalCamPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        camera.transform.localPosition = originalCamPos;
    }

    private IEnumerator FindAimTarget()
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

    private IEnumerator FindBallPos()
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