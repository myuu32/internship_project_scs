using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSprint : MonoBehaviour
{
    [Header("Sprint Settings")]
    [Tooltip("The speed at which the player sprints")]
    public float sprintSpeed = 10f; // プレイヤーがスプリントする速度
    [Tooltip("The maximum duration of the sprint")]
    public float sprintTime = 2f; // スプリントの最大持続時間
    [Tooltip("Rotation speed during sprint")]
    public float rotationSpeed = 5f; // スプリント中の回転速度

    [Header("Sliding Settings")]
    [Tooltip("Duration of the sliding effect after sprinting")]
    public float slideDuration = 0.5f; // スプリント後のスライディング効果の持続時間
    [Tooltip("Speed multiplier for sliding")]
    public float slideSpeedMultiplier = 0.5f; // スライディングの速度乗数

    [Header("Cooldown Settings")]
    [Tooltip("Cooldown time as a multiplier of the sprint time")]
    public float sprintCooldownMultiplier = 1f; // スプリント時間を基準としたクールダウン時間の乗数

    private float remainingSprintTime; // 残りのスプリント時間
    private float slideTimer = 0; // スライディングタイマー
    private float cooldownTimer = 0; // クールダウンタイマー
    private bool isSprinting = false; // スプリント中かどうか
    private Vector3 slideDirection; // スライディングの方向

    void Update()
    {
        HandleSprinting();
        HandleSliding();
    }

    // スプリント処理
    void HandleSprinting()
    {
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

    // スライディング処理
    void HandleSliding()
    {
        if (slideTimer > 0)
        {
            transform.Translate(slideDirection * sprintSpeed * slideSpeedMultiplier * Time.deltaTime, Space.World);
            slideTimer -= Time.deltaTime;
        }

        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    // Input Systemから呼ばれる公開メソッド
    public void OnSprint(InputAction.CallbackContext context)
    {
        if (context.started && cooldownTimer <= 0 && !isSprinting)
        {
            StartSprint();
        }
        else if (context.canceled && isSprinting)
        {
            EndSprint();
        }
    }

    // スプリント開始
    void StartSprint()
    {
        isSprinting = true;
        remainingSprintTime = sprintTime;
        slideDirection = transform.forward;
        Debug.Log("Sprint");
    }

    // スプリント終了
    void EndSprint()
    {
        isSprinting = false;
        cooldownTimer = remainingSprintTime * sprintCooldownMultiplier;
        slideTimer = slideDuration;
        remainingSprintTime = 0;
    }
}
