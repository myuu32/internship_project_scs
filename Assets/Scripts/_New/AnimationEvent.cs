using System.Collections;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public void OpenObject ()
    {
        gameObject.SetActive(true);
    }

    public void CloseObject()
    {
        gameObject.SetActive(false);

    }
}
