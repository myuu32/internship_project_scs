using System.Collections;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    public float moveSpeed = 5.0f;
    public float turnSmoothTime = 0.1f;
    public float jumpForce = 10.0f;
    public Transform handTransform;
    public GameObject handWeaponPrefab;
    public float throwForce = 10f;
    private GameObject currentWeapon;
    private bool isJumping = false;
    private float turnSmoothVelocity;
    public GrabItem grabItem;
    public Animator animator;


    void Awake()
    {
        grabItem = GetComponent<GrabItem>(); 
    }

    void Update()
    {
        MoveAndJump();

        if (Input.GetKeyDown(KeyCode.K) && currentWeapon != null)
        {
            if (currentWeapon.CompareTag("Interactable"))
            {
                Debug.Log("Interactable");
                //Interactable();
            }
            else if (currentWeapon.CompareTag("Item"))
            {
                ThrowWeapon();
            }
            else
            {
                HandAttack();
            }
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            if (currentWeapon != null)
            {
                if (grabItem != null)
                {
                    grabItem.DropItem();
                }
                else
                {
                    Debug.LogError("GrabItem reference not set in CharacterMovement.");
                }
            }
        }
    }

    void MoveAndJump()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Get the camera's forward and right vectors, ignoring the y component
        Vector3 cameraForward = Camera.main.transform.forward;
        cameraForward.y = 0;
        Vector3 cameraRight = Camera.main.transform.right;
        cameraRight.y = 0;

        // Apply the camera's orientation to the input direction
        Vector3 moveDirection = cameraForward * vertical + cameraRight * horizontal;

        if (moveDirection.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            transform.Translate(moveDirection.normalized * moveSpeed * Time.deltaTime, Space.World);
        }

        if (Input.GetButtonDown("Jump") && !isJumping)
        {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
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

    void HandAttack()
    {
    }

    void Interactable()
    {
        animator.SetTrigger("AttackTrigger");
    }

    void ThrowWeapon()
    {
        currentWeapon.transform.SetParent(null);
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.AddForce(transform.forward * throwForce, ForceMode.VelocityChange);
        Collider weaponCollider = currentWeapon.GetComponent<Collider>();
        weaponCollider.enabled = true;
        currentWeapon = null;
    }

    public void DropWeapon()
    {
        if (currentWeapon == null) return; 

        currentWeapon.transform.SetParent(null);
        Rigidbody rb = currentWeapon.GetComponent<Rigidbody>();
        if (rb != null)
            {
                rb.isKinematic = false;
            }

        Collider weaponCollider = currentWeapon.GetComponent<Collider>();
        if (weaponCollider != null)
            {
                weaponCollider.enabled = true;
            }

        currentWeapon.transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
        currentWeapon = null;
    }


    public void SetCurrentWeapon(GameObject weapon)
    {
        currentWeapon = weapon;
        weapon.transform.SetParent(handTransform);
        weapon.transform.localPosition = Vector3.zero;
        weapon.transform.localRotation = Quaternion.identity;
        Rigidbody rb = weapon.GetComponent<Rigidbody>();
        rb.isKinematic = true;
        Collider weaponCollider = weapon.GetComponent<Collider>();
        weaponCollider.enabled = false;
    }

    void FixedUpdate()
    {
        transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
    }
}
