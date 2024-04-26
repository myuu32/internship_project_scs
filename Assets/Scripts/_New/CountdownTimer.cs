using UnityEngine;
using TMPro;
using System.Collections;

public class CountdownTimer : MonoBehaviour
{
    public TMP_Text countdownText;
    public float countdownTime = 60;
    public GameObject ResultMenu;
    public bool Timeup = false;

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
        Timeup = true;
        countdownText.text = "0";
        ResultMenu.SetActive(true);
        GameObject.Find("ScoreManager").GetComponent<ScoreManager>().ShowResultMenu();
        Time.timeScale = 0f;
    }
}
