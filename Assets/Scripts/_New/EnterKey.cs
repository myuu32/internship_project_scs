using UnityEngine;
using UnityEngine.UI;

public class EnterKey : MonoBehaviour
{
    public Button button;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
        {
            if (button != null && button.isActiveAndEnabled)
            {
                button.onClick.Invoke();
            }
        }
    }
}
