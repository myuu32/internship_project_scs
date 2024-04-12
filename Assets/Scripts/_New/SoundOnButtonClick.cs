using UnityEngine;
using UnityEngine.UI;

public class SoundOnButtonClick : MonoBehaviour
{
    public AudioClip soundEffect;
    private Button button;
    private AudioSource audioSource;

    void Start()
    {
        button = GetComponent<Button>();
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        button.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (soundEffect != null)
        {
            audioSource.PlayOneShot(soundEffect);
        }
    }
}
