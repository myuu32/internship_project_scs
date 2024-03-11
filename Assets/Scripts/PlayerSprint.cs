using System.Collections;
using UnityEngine;

public class PlayerSprint : MonoBehaviour
{
    [Header("Sprint Settings")]
    [Tooltip("プレイヤーがスプリントする速度")]
    public float sprintSpeed = 10f;
    [Tooltip("スプリントの最大持続時間")]
    public float sprintTime = 2f;
    [Tooltip("スプリント中の回転速度")]
    public float rotationSpeed = 5f;

    [Header("Sliding Settings")]
    [Tooltip("スプリント後のスライディング効果の持続時間")]
    public float slideDuration = 0.5f;
    [Tooltip("スライディングの速度乗数")]
    public float slideSpeedMultiplier = 0.5f;

    [Header("Cooldown Settings")]
    [Tooltip("スプリント時間を基準としたクールダウン時間の乗数")]
    public float sprintCooldownMultiplier = 1f;

    private float remainingSprintTime;
    private float slideTimer = 0;
    private float cooldownTimer = 0;
    private bool isSprinting = false;
    private Vector3 slideDirection;

    void Update()
    {
        HandleSprintInput();
        HandleSprinting();
        HandleSliding();
    }

    void HandleSprintInput()
    {
        // 左Shiftキーでスプリント開始の入力を処理
        if (Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0 && !isSprinting)
        {
            StartSprint();
        }

        // 左Shiftキーを離すことでスプリント終了の処理
        if (Input.GetKeyUp(KeyCode.LeftShift) && isSprinting)
        {
            EndSprint();
        }
    }

    void HandleSprinting()
    {
        // スプリント処理
        if (isSprinting)
        {
            Quaternion lookRotation = Quaternion.LookRotation(slideDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * rotationSpeed);
            transform.Translate(slideDirection * sprintSpeed * Time.deltaTime, Space.World);
            remainingSprintTime -= Time.deltaTime;

            if (remainingSprintTime <= 0)
            {
                EndSprint();
            }
        }
    }

    void HandleSliding()
    {
        // スライディング処理
        if (slideTimer > 0)
        {
            transform.Translate(slideDirection * sprintSpeed * slideSpeedMultiplier * Time.deltaTime, Space.World);
            slideTimer -= Time.deltaTime;
        }

        // クールダウン時間の処理
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    void StartSprint()
    {
        isSprinting = true;
        remainingSprintTime = sprintTime;
        slideDirection = transform.forward;
    }

    void EndSprint()
    {
        isSprinting = false;
        cooldownTimer = remainingSprintTime * sprintCooldownMultiplier;
        slideTimer = slideDuration;
        remainingSprintTime = 0;
    }
}
