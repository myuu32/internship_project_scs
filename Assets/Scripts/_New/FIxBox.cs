using UnityEngine;
using UnityEngine.InputSystem;

public class FIxBox : MonoBehaviour
{
    public Transform[] spawnLocations;
    private bool[] locationOccupied;

    private void Start()
    {
        locationOccupied = new bool[spawnLocations.Length];
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInput playerInput = other.GetComponent<PlayerInput>();
            if (playerInput != null)
            {
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    for (int i = 0; i < spawnLocations.Length; i++)
                    {
                        if (!locationOccupied[i])
                        {
                            Vector3 spawnPosition = spawnLocations[i].position;

                            Debug.Log($"Fixed Player {playerInput.playerIndex} to Location {i}");

                            rb.MovePosition(spawnPosition);
                            if (i == 1)
                            {
                                other.transform.rotation = Quaternion.Euler(0, 180, 0);
                            }

                            other.GetComponent<PlayerDetails>().playerID = i + 1;
                            other.GetComponent<PlayerDetails>().startPos = spawnPosition;

                            locationOccupied[i] = true;
                            break;
                        }
                    }
                }
            }
        }
    }
}
