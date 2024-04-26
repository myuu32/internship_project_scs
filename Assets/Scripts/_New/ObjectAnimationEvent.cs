using UnityEngine;

public class ObjectAnimationEvent : MonoBehaviour
{
    [SerializeField]
    private GameObject gobj;

    public GameObject ObjectToControl
    {
        get { return gobj; }
        set { gobj = value; }
    }

    public void OpenObject()
    {
        if (gobj != null)
        {
            gobj.SetActive(true);
        }
        else
        {
            Debug.LogWarning("Object to control is not assigned!");
        }
    }
}

