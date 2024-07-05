using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAudio : MonoBehaviour
{
    private AudioSource stageSource;
    // Start is called before the first frame update

    [Header("�u���b�N�j��")]
    [SerializeField] AudioClip stageBlockDestroySound;
    [Header("�g�Q��")]
    [SerializeField] AudioClip stageThornSound;
    [Header("�g�Q�V��Փ�")]
    [SerializeField] AudioClip stageThornHitSound;

    //���ʒ����p
    [Header("�u���b�N�j�󉹗�")]
    [SerializeField, Range(0f, 1f)] float stageBlockDrstroyVolume;
    [Header("�g�Q������")]
    [SerializeField, Range(0f, 1f)] float stageThornVolume;
    [Header("�g�Q�V��Փˉ���")]
    [SerializeField, Range(0f, 1f)] float stageThornHitVolume;
    void Start()
    {
        stageSource = GetComponent<AudioSource>();
    }
    public void BlockDestroySound()
    {
        stageSource.volume = stageBlockDrstroyVolume;
        stageSource.PlayOneShot(stageBlockDestroySound);
    }
    public void ThornSound()
    {
        stageSource.volume = stageThornVolume;
        stageSource.PlayOneShot(stageThornSound);
    }
    public void ThornHitSound()
    {
        stageSource.volume = stageThornHitVolume;
        stageSource.PlayOneShot(stageThornHitSound);
    }
}
