using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageAudio : MonoBehaviour
{
    private AudioSource stageSource;
    // Start is called before the first frame update

    [Header("ブロック破壊")]
    [SerializeField] AudioClip stageBlockDestroySound;
    [Header("トゲ柱")]
    [SerializeField] AudioClip stageThornSound;
    [Header("トゲ天井衝突")]
    [SerializeField] AudioClip stageThornHitSound;

    //音量調整用
    [Header("ブロック破壊音量")]
    [SerializeField, Range(0f, 1f)] float stageBlockDrstroyVolume;
    [Header("トゲ柱音量")]
    [SerializeField, Range(0f, 1f)] float stageThornVolume;
    [Header("トゲ天井衝突音量")]
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
