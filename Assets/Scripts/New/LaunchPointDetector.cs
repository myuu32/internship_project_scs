using UnityEngine;

public class LaunchPointDetector : MonoBehaviour
{
    public TennisBallLauncher launcherScript; // Reference to the main launcher script

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered the launch point.");
            launcherScript.EnableLaunch(transform.gameObject);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited the launch point.");
            launcherScript.DisableLaunch(transform.gameObject);
        }
    }
}
