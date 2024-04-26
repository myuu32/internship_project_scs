using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpeedBoostCircles : MonoBehaviour
{
    public List<GameObject> circleObjects;
    public float changeInterval = 5.0f;
    public float range = 5f;
    public MovementDirection direction = MovementDirection.ForwardBackward;

    public enum MovementDirection
    {
        ForwardBackward,
        LeftRight
    }

    private int currentIndex = -1;
    private Vector3 startPosition;
    private int directionSign = 1;

    private void Start()
    {
        StartCoroutine(ActivateRandomCircle());
    }

    public IEnumerator ActivateRandomCircle()
    {
        while (true)
        {
            if (currentIndex != -1)
            {
                circleObjects[currentIndex].SetActive(false);
            }

            int randomIndex = Random.Range(0, circleObjects.Count);
            currentIndex = randomIndex;
            startPosition = circleObjects[currentIndex].transform.position;
            circleObjects[currentIndex].SetActive(true);

            yield return new WaitForSeconds(changeInterval);
        }
    }

    private void Update()
    {
        if (currentIndex == -1) return;

        GameObject currentCircle = circleObjects[currentIndex];
        Vector3 movementDirection = direction == MovementDirection.LeftRight ? Vector3.right : Vector3.forward;
        currentCircle.transform.Translate(movementDirection * directionSign * currentCircle.GetComponent<SpeedBoostCircleTrigger>().speed * Time.deltaTime);

        if (Mathf.Abs(Vector3.Distance(currentCircle.transform.position, startPosition)) >= range)
        {
            directionSign *= -1;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("TennisBall"))
        {
        }
    }
}
