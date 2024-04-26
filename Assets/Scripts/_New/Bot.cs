using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Bot : MonoBehaviour
{
    private float speed = 50;
    private float hitForce = 80;

    private GameObject ball;
    private Transform aimTarget;

    private Animator animator;

    private Vector3 targetPosition;

    private void Start()
    {
        targetPosition = transform.position;
        animator = GetComponent<Animator>();

        StartCoroutine(FindAimTarget());
        StartCoroutine(FindBallPos());
    }

    private void Update()
    {
        Move();
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

            if (aimTarget == null)
            {
                Debug.LogError("aimTarget is not set.");
                return;
            }

            Rigidbody tennisBallRigidbody = other.GetComponent<Rigidbody>();
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
