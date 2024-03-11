using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance; // AudioManager インスタンス

    public AudioSource bgmSource; // BGM プレイヤー

    public List<SFXClip> sfxClips = new List<SFXClip>(); // SFXClip のリスト

    [Range(0f, 1f)]
    public float bgmVolume = 0.5f; // BGM の音量

    [Range(0f, 1f)]
    public float sfxVolume = 0.5f; // SFX の音量

    private Dictionary<string, AudioSource> sfxSources = new Dictionary<string, AudioSource>(); // SFX サウンドのディクショナリ

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // SFX サウンドの AudioSource を初期化
        foreach (SFXClip sfxClip in sfxClips)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.clip = sfxClip.clip;
            sfxSources.Add(sfxClip.id, source);
        }
    }

    void Start()
    {
        bgmSource.volume = bgmVolume; // 初期 BGM 音量の設定
    }

    // BGM を再生
    public void PlayBGM(AudioClip bgmClip)
    {
        bgmSource.clip = bgmClip;
        bgmSource.Play();
    }

    // SFX を再生
    public void PlaySFX(string id)
    {
        if (sfxSources.ContainsKey(id))
        {
            sfxSources[id].volume = sfxVolume; // SFX 音量の設定
            sfxSources[id].PlayOneShot(sfxSources[id].clip);
        }
        else
        {
            Debug.LogWarning("ID " + id + " の SFX が存在しません。");
        }
    }

    // BGM の音量を設定
    public void SetBGMVolume(float volume)
    {
        bgmVolume = Mathf.Clamp01(volume);
        bgmSource.volume = bgmVolume;
    }

    // SFX の音量を設定
    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
    }
}

[System.Serializable]
public class SFXClip
{
    public string id; // SFX サウンドの ID
    public AudioClip clip; // SFX サウンドの AudioClip
}
