using System.Collections;
using UnityEngine;

public class GrabItem : MonoBehaviour
{
    public Transform playerHand;
    public float grabSpeed = 1f;
    private GameObject targetItem;
    private bool isHoldingItem = false;
    public CharacterMovement characterMovement;

    private bool canGrab = true;
    public float grabCooldown = 0.5f; 

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J) && canGrab)
        {
            if (!isHoldingItem && targetItem != null)
            {
                StartCoroutine(GrabItemCoroutine(targetItem));
            }
            else if (isHoldingItem)
            {
                DropItem();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((other.CompareTag("Item") || other.CompareTag("Interactable")) && !isHoldingItem)
        {
            targetItem = other.transform.root.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == targetItem)
        {
            targetItem = null;
        }
    }

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
        if (rb != null)
        {
            rb.isKinematic = true;
        }

        Collider itemCollider = item.GetComponent<Collider>();
        if (itemCollider != null)
        {
            itemCollider.enabled = false;
        }

        isHoldingItem = true;
        characterMovement.SetCurrentWeapon(item);
        characterMovement.GetComponent<Rigidbody>().isKinematic = false;

    }

    public IEnumerator GrabCooldown()
    {
        canGrab = false;
        yield return new WaitForSeconds(grabCooldown);
        canGrab = true;
    }

    public void DropItem()
    {
        if (characterMovement != null)
        {
            characterMovement.DropWeapon();
        }
        isHoldingItem = false;
        StartCoroutine(GrabCooldown());
    }
}
