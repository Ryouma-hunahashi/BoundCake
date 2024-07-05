using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveAudio : MonoBehaviour
{
    private AudioSource waveSource;


    [Header("�g�i���j�̃T�E���h")]
    [SerializeField] private AudioClip waveSmallSound;
    [Header("�g�i��j�̃T�E���h")]
    [SerializeField] private AudioClip waveBigSound;
    [Header("�����L�т�T�E���h")]
    [SerializeField] private AudioClip waveExtendSound;


    [Header("�g�i���j�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float waveSmallVolume;
    [Header("�g�i��j�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float waveBigVolume;
    [Header("�����L�т�̃T�E���h����")]
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
