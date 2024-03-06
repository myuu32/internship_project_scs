using System.Collections;
using UnityEngine;

public class GrabItem : MonoBehaviour
{
    [Header("Player's Hand")]
    public Transform playerHand; // プレイヤーの手
    [Header("Item Grabbing Speed")]
    [Tooltip("アイテムを掴む速度")]
    public float grabSpeed = 1f; // アイテムを掴む速度
    private GameObject targetItem; // ターゲットアイテム
    private bool isHoldingItem = false; // アイテムを持っているかどうか
    public CharacterMovement characterMovement; // キャラクターの動きを制御するコンポーネント

    private bool inTriggerZone = false;


    void Update()
    {
        // Jキーが押された時の処理
        if (Input.GetKeyDown(KeyCode.J))
        {
            // アイテムを持っていない、ターゲットアイテムが存在し、トリガーゾーン内であれば
            if (!isHoldingItem && targetItem != null && inTriggerZone) 
            {
                StartCoroutine(GrabItemCoroutine(targetItem));
            }
            else if (isHoldingItem)
            {
                DropItem();
            }
        }
    }

    // トリガーゾーンに入った時の処理
    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Item") || other.CompareTag("Interactable")) && !isHoldingItem)
        {
            targetItem = other.transform.root.gameObject; 
            inTriggerZone = true; 
        }
    }

    // トリガーゾーンから出た時の処理
    private void OnTriggerExit(Collider other)
    {
        if (other.transform.root.gameObject == targetItem)
        {
            inTriggerZone = false;
        }
    }

    // アイテムを掴む処理
    IEnumerator GrabItemCoroutine(GameObject item)
    {
        characterMovement.GetComponent<Rigidbody>().isKinematic = true;

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

        characterMovement.SetCurrentWeapon(item);
        characterMovement.GetComponent<Rigidbody>().isKinematic = false;

        isHoldingItem = true;// アイテムを持っている状態に
    }

    // アイテムを離す処理
    public void DropItem()
    {
        if (characterMovement != null && isHoldingItem)
        {
            characterMovement.DropWeapon(); // アイテムを離す
            isHoldingItem = false; // アイテムを持っていない状態に
        }
    }
}
