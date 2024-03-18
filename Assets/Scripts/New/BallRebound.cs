using UnityEngine;
using UnityEngine.InputSystem;

public class BallRebound : MonoBehaviour
{
    [SerializeField] private float attackForce = 10f; // 攻击力道
    [SerializeField] private float attackAngle = 45f; // 攻击角度

    private bool isAttacking = false; // 是否正在攻击

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            isAttacking = true;
        }
        else if (context.canceled)
        {
            isAttacking = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (isAttacking && other.CompareTag("TennisBall"))
        {
            Rigidbody ballRigidbody = other.GetComponent<Rigidbody>();
            if (ballRigidbody != null)
            {
                Vector3 attackDirection = Quaternion.AngleAxis(attackAngle, transform.right) * transform.forward;
                ballRigidbody.AddForce(attackDirection * attackForce, ForceMode.Impulse);
            }
        }
    }
}
