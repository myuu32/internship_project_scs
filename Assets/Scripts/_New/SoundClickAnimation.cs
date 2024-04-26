using UnityEngine;
using UnityEngine.UI;

public class SoundClickAnimation : MonoBehaviour
{
    public AudioClip[] soundEffects;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlaySound(int index)
    {
        if (soundEffects != null && index >= 0 && index < soundEffects.Length)
        {
            audioSource.PlayOneShot(soundEffects[index]);
        }
    }

    public void PlaySecondSound1()
    {
        PlaySound(0);
    }

    public void PlaySecondSound2()
    {
        PlaySound(1);
    }

}
