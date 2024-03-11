using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class CharacterMovement : MonoBehaviour
{
    [Tooltip("キャラクターの移動速度")]
    public float moveSpeed = 5.0f; // キャラクターの移動速度
    [Tooltip("曲がる時の滑らかさ")]
    public float turnSmoothTime = 0.1f; // 曲がる時の滑らかさ
    [Tooltip("キャラクターのジャンプ力")]
    public float jumpForce = 10.0f; // キャラクターのジャンプ力

    private Rigidbody rb;
    private bool isJumping = false; // ジャンプ中かどうかのフラグ
    private float turnSmoothVelocity; // 回転の平滑化に使用
    private Vector2 moveInput; // 移動入力

    public Transform playerHand; // プレイヤーの手
    public float grabSpeed = 1f; // アイテムを掴む速度
    private GameObject targetItem; // ターゲットアイテム
    private bool isHoldingItem = false; // アイテムを持っているかどうか
    private bool inTriggerZone = false;

    private PlayerController controls; // Input Actionsのコントロール

    public Animator animator; // アニメーター
    public float throwForce = 10f; // 武器を投げる力
    private bool canPickup = true;


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        controls = new PlayerController();
        controls.Player.Grab.performed += ctx => OnGrab();
        controls.Player.Drop.performed += ctx => OnDrop();
        controls.Player.Attack.performed += ctx => OnAttack();
        controls.Player.Movement.performed += ctx => OnMove(ctx);
        controls.Player.Jump.performed += ctx => OnJump();
    }

    void Update()
    {
        if (moveInput != Vector2.zero)
        {
            MoveCharacter(moveInput);
        }
    }

    void OnEnable()
    {
        controls.Enable();
    }

    void OnDisable()
    {
        controls.Disable();
    }

    // 移動入力処理
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    void MoveCharacter(Vector2 input)
    {
        Vector3 moveDirection = new Vector3(input.x, 0f, input.y).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            // カメラの前方と右方向ベクトルを取得し、y成分を無視
            Vector3 cameraForward = Camera.main.transform.forward;
            cameraForward.y = 0;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraRight.y = 0;
            Vector3 desiredDirection = cameraForward * input.y + cameraRight * input.x;

            // 移動方向に応じてキャラクターを回転
            float targetAngle = Mathf.Atan2(desiredDirection.x, desiredDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            // 移動処理
            transform.Translate(desiredDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
        }
    }

    public void OnJump()
    {
        if (!isJumping)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }

    // アイテムを掴む処理
    public void OnGrab()
    {
        // アイテムを持っていない、ターゲットアイテムが存在し、トリガーゾーン内であれば
        if (!isHoldingItem && targetItem != null && inTriggerZone)
        {
            StartCoroutine(GrabItemCoroutine(targetItem));
        }
    }

    // アイテムを離す処理
    public void OnDrop()
    {
        if (isHoldingItem)
        {
            DropItem();
        }
    }

    // トリガーゾーンに入った時の処理
    void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Item") || other.CompareTag("Interactable")) && !isHoldingItem)
        {
            targetItem = other.transform.root.gameObject;
            inTriggerZone = true;
        }
    }

    // トリガーゾーンから出た時の処理
    void OnTriggerExit(Collider other)
    {
        if (other.transform.root.gameObject == targetItem)
        {
            inTriggerZone = false;
        }
    }

    // アイテムを掴む処理
    IEnumerator GrabItemCoroutine(GameObject item)
    {
        GetComponent<Rigidbody>().isKinematic = true;

        float elapsedTime = 0;
        Vector3 initialPosition = item.transform.position;
        Quaternion initialRotation = item.transform.rotation;

        while (elapsedTime < grabSpeed)
        {
            item.transform.position = Vector3.Lerp(initialPosition, playerHand.position, (elapsedTime / grabSpeed));
            item.transform.rotation = Quaternion.Slerp(initialRotation, playerHand.rotation, (elapsedTime / grabSpeed));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        item.transform.SetParent(playerHand);
        item.transform.localPosition = Vector3.zero;
        item.transform.localRotation = Quaternion.identity;
        Rigidbody rb = item.GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;

        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null) itemCollider.enabled = false;

        SetCurrentWeapon(item);
        GetComponent<Rigidbody>().isKinematic = false;

        isHoldingItem = true; // アイテムを持っている状態に
    }

    // アイテムを離す処理
    public void DropItem()
    {
        if (targetItem != null)
        {
            Debug.Log("drop");

            targetItem.transform.SetParent(null);

            Rigidbody itemRb = targetItem.GetComponent<Rigidbody>();
            if (itemRb != null)
            {
                itemRb.isKinematic = false;
                itemRb.useGravity = true;
                itemRb.AddForce(transform.forward * 1f + transform.up * 0.5f, ForceMode.VelocityChange);
            }

            Collider itemCollider = targetItem.GetComponent<Collider>();
            if (itemCollider != null)
            {
                itemCollider.enabled = true;
            }

            // アイテムを拾う際の遅延
            canPickup = false;
            StartCoroutine(AllowPickupAfterDelay(2f));

            targetItem = null;
            isHoldingItem = false;
        }
    }


    IEnumerator AllowPickupAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        canPickup = true;
    }



    public void OnAttack()
    {
        if (targetItem != null)
        {

            if (targetItem.CompareTag("Interactable"))
            {
                Interactable();
            }
            else if (targetItem.CompareTag("Item"))
            {
                ThrowWeapon(targetItem);
            }
        }
        else
        {
            HandAttack();
        }
    }

    void Interactable()
    {
        //animator.SetTrigger("AttackTrigger");
        Debug.Log("Interactable Attack");
    }

    void ThrowWeapon(GameObject weapon)
    {
        weapon.transform.SetParent(null);
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        Collider weaponCollider = weapon.GetComponent<Collider>();
        weaponCollider.enabled = true;

        targetItem = null;
        isHoldingItem = false;
    }

    void HandAttack()
    {
        Debug.Log("Attack");
    }

    // 現在の武器を設定する
    public void SetCurrentWeapon(GameObject weapon)
    {
        // 現在の武器を設定する処理を実装
    }

    // 現在の武器をクリアする
    public void ClearCurrentWeapon()
    {
        // 現在の武器をクリアする処理を実装
    }
}
