using UnityEngine;

public class DisableObject : MonoBehaviour
{
    public void FalseObject()
    {
        gameObject.SetActive(false);
    }
    public void TrueObject()
    {
        gameObject.SetActive(true);
    }
}
