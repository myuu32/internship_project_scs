using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Speed")]
    [Tooltip("The speed at which the bullet moves")]
    [Range(1f, 20f)] // Allows adjustment in the Inspector with a slider
    public float speed = 10f; // 弾の速度

    [Header("Damage")]
    [Tooltip("The damage the bullet deals")]
    [Range(1f, 50f)] // Adjust as needed for your game balance
    public float damage = 20f; // 弾が与えるダメージ

    [Header("Lifetime")]
    [Tooltip("How long before the bullet automatically destroys itself")]
    [Range(1f, 10f)] // Provides a range for bullet lifetime
    public float lifetime = 5f; // 弾の寿命

    void Start()
    {
        Destroy(gameObject, lifetime); // 一定時間後に弾を自動的に破壊する
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime); // 弾を前方に移動させる
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Playerタグが付いているオブジェクトと衝突した場合
        if (collision.gameObject.CompareTag("Player"))
        {
            Animator playerAnimator = collision.gameObject.GetComponent<Animator>();
            if (playerAnimator != null)
            {
                playerAnimator.SetTrigger("Hit"); // プレイヤーがヒットしたアニメーションをトリガーする
            }
            Destroy(gameObject); // 衝突後、弾を破壊する
            Debug.Log("Hit"); // コンソールにヒットをログする
        }
        else
        {
            Destroy(gameObject); // プレイヤー以外のオブジェクトと衝突した場合も弾を破壊する
        }
    }
}


