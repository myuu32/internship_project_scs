using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Bot : MonoBehaviour
{
    private float speed = 50; // 移动速度
    private float hitForce = 50; // 击球力度

    private GameObject ball;
    private Transform aimTarget;

    private Animator animator;

    private Vector3 targetPosition; // Bot 想要移动到的位置

    private void Start()
    {
        targetPosition = transform.position; // 初始化 targetPosition 为在场上的初始位置
        animator = GetComponent<Animator>(); // 获取 Animator 的引用

        // 启动协程
        StartCoroutine(FindAimTarget());
        StartCoroutine(FindBallPos());
    }

    private void Update()
    {
        Move(); // 调用移动方法
    }

    private System.Collections.IEnumerator FindAimTarget()
    {
        while (true)
        {
            if (aimTarget == null)
            {
                aimTarget = GameObject.FindGameObjectWithTag("AimTargetB").transform;
            }
            yield return null;
        }
    }

    private System.Collections.IEnumerator FindBallPos()
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

    private void Move()
    {
        if (ball != null)
        {
            targetPosition.x = ball.transform.position.x;
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TennisBall"))
        {
            Debug.Log("Bot Reserve");

            // 确保 aimTarget 已经被找到
            if (aimTarget == null)
            {
                Debug.LogError("aimTarget is not set.");
                return; // 直接返回，避免进一步执行并引发异常
            }

            Rigidbody tennisBallRigidbody = other.GetComponent<Rigidbody>();
            // 确保找到 Rigidbody 组件
            if (tennisBallRigidbody == null)
            {
                Debug.LogError("Rigidbody component not found on the tennis ball.");
                return;
            }

            Vector3 dir = aimTarget.position - transform.position;
            tennisBallRigidbody.velocity = dir.normalized * hitForce + new Vector3(0, 50, 0);

            Vector3 ballDir = ball.transform.position - transform.position;
            if (ballDir.x >= 0)
            {
                animator.Play("Forehand");
            }
            else
            {
                animator.Play("Backhand");
            }
        }
    }

}
