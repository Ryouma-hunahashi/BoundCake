using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAudio : MonoBehaviour
{
    private AudioSource waveSource;


    [Header("波（小）のサウンド")]
    [SerializeField] private AudioClip waveSmallSound;
    [Header("波（大）のサウンド")]
    [SerializeField] private AudioClip waveBigSound;
    [Header("糸が伸びるサウンド")]
    [SerializeField] private AudioClip waveExtendSound;


    [Header("波（小）のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float waveSmallVolume;
    [Header("波（大）のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float waveBigVolume;
    [Header("糸が伸びるのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float waveExtendVolume;

    // Start is called before the first frame update
    void Start()
    {
        waveSource = GetComponent<AudioSource>();
    }

    public void SmallSound()
    {
        waveSource.volume = waveSmallVolume;
        waveSource.PlayOneShot(waveSmallSound);
    }
    public void BigSound()
    {
        waveSource.volume = waveBigVolume;
        waveSource.PlayOneShot(waveBigSound);
    }
    public void ExtendSound()
    {
        waveSource.volume = waveExtendVolume;
        waveSource.PlayOneShot(waveExtendSound);
    }
    // Update is called once per frame
    //void Update()
    //{

    //}
}
