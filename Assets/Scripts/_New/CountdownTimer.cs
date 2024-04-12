using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public TMP_Text countdownText;
    public float countdownTime = 60;

    private void Start()
    {
        StartCoroutine(StartCountdown());
    }

    private IEnumerator StartCountdown()
    {
        float currentTime = countdownTime;
        while (currentTime > 0)
        {
            countdownText.text = $"{currentTime}";
            yield return new WaitForSeconds(1.0f);
            currentTime--;
        }
        countdownText.text = "TIME'S UP!";
    }
}
