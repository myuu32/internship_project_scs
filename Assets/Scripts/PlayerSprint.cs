using System.Collections;
using UnityEngine;

public class PlayerSprint : MonoBehaviour
{
    [Header("Sprint Settings")]
    [Tooltip("The speed at which the player sprints")]
    public float sprintSpeed = 10f; // 衝刺速度
    [Tooltip("Maximum duration of the sprint")]
    public float sprintTime = 2f; // 衝刺持続時間の最大値
    [Tooltip("Rotation speed during sprinting")]
    public float rotationSpeed = 5f; // 回転速度

    [Header("Sliding Settings")]
    [Tooltip("Duration of the sliding effect after sprinting")]
    public float slideDuration = 0.5f; // スライディング効果の持続時間
    [Tooltip("Speed multiplier for sliding")]
    public float slideSpeedMultiplier = 0.5f; // スライディング速度の乗数

    [Header("Cooldown Settings")]
    [Tooltip("Multiplier for calculating the cooldown time based on the sprint time")]
    public float sprintCooldownMultiplier = 1f; // 冷却時間の乗数

    private float remainingSprintTime; // 残りの衝刺時間
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
        // 左Shiftキーで衝刺開始の入力を処理
        if (Input.GetKeyDown(KeyCode.LeftShift) && cooldownTimer <= 0 && !isSprinting)
        {
            StartSprint();
        }

        // 左Shiftキーを離すことで衝刺終了の処理
        if (Input.GetKeyUp(KeyCode.LeftShift) && isSprinting)
        {
            EndSprint();
        }
    }

    void HandleSprinting()
    {
        // 衝刺処理
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

        // 冷却時間の処理
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
