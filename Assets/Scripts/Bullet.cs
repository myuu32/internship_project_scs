using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Speed")]
    [Tooltip("弾の移動速度")]
    [Range(1f, 20f)] // スライダーで調整可能
    public float speed = 10f;

    [Header("Damage")]
    [Tooltip("弾が与えるダメージ")]
    [Range(1f, 50f)] // ゲームバランスに合わせて調整
    public float damage = 20f;

    [Header("Lifetime")]
    [Tooltip("弾が自動的に破壊されるまでの時間")]
    [Range(1f, 10f)] // 弾の寿命の範囲を指定
    public float lifetime = 5f;

    void Start()
    {
        Destroy(gameObject, lifetime); // 一定時間後に弾を自動的に破壊
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // 弾を前方に移動
    }

    private void OnCollisionEnter(Collision collision)
    {
        // プレイヤータグが付いているオブジェクトと衝突した場合
        if (collision.gameObject.CompareTag("Player"))
        {
            Animator playerAnimator = collision.gameObject.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Hit"); // プレイヤーがヒットしたアニメーションをトリガー
            }
            Destroy(gameObject); // 衝突後、弾を破壊
            Debug.Log("Hit"); // ヒットをログに出力
        }
        else
        {
            Destroy(gameObject); // プレイヤー以外のオブジェクトと衝突した場合も弾を破壊
        }
    }
}
