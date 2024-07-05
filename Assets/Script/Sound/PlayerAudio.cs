using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource playerSource;

    [Header("歩くサウンド")]
    public AudioClip playerWalkSound;
    [Header("弱ジャンプのサウンド")]
    [SerializeField] private AudioClip playerWeakJumpSound;
    [Header("強ジャンプのサウンド")]
    [SerializeField] private AudioClip playerStrongJumpSound;
    [Header("掴みのサウンド")]
    [SerializeField] private AudioClip playerGrabSound;
    [Header("掴み移動のサウンド")]
    public AudioClip playerGrabMoveSound;
    [Header("掴み離しのサウンド")]
    [SerializeField] private AudioClip playerSeparateSound;
    [Header("掴み引っ張りのサウンド")]
    [SerializeField] private AudioClip playerPullSound;
    [Header("チャージのサウンド")]
    [SerializeField] private AudioClip playerChargeSound;
    [Header("被ダメージのサウンド")]
    [SerializeField] private AudioClip playerDamageSound;

    [Header("歩きのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerWalkVolume;
    [Header("弱ジャンプのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerWeakVolume;
    [Header("強ジャンプのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerStrongVolume;
    [Header("掴みのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerGrabVolume;
    [Header("掴み移動のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerGrabMoveVolume;
    [Header("掴み離しのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerSeparateVolume;
    [Header("掴み引っ張りのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerPullVolume;
    [Header("チャージのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerChargeVolume;

    [Header("被ダメージのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float playerDamageVolume;






    // Start is called before the first frame update
    void Start()
    {
        playerSource = GetComponent<AudioSource>();

    }

    //// Update is called once per frame
    //void Update()
    //{
        
    //}


    public void WalkSound()
    {
        if (playerWalkSound != null)
        {
            playerSource.volume = playerWalkVolume;
            playerSource.clip = playerWalkSound;
            playerSource.Play();
        }
    }
    public void WeakJumpSound()
    {
        playerSource.volume = playerWeakVolume;
        playerSource.clip = playerWeakJumpSound;
        playerSource.Play();
    }

    public void StrongJumpSound()
    {
        playerSource.volume = playerStrongVolume;

        playerSource.clip = playerStrongJumpSound;
        playerSource.Play();
    }

    public void GrabSound()
    {
        playerSource.volume = playerGrabVolume;
        playerSource.clip = playerGrabSound;
        playerSource.Play();
    }
    public void GrabMoveSound()
    {
        if (playerGrabMoveSound != null)
        {
            playerSource.volume = playerGrabMoveVolume;
            playerSource.clip = playerGrabMoveSound;
            playerSource.Play();
        }
    }
    public void SeparateSound()
    {
        playerSource.volume = playerSeparateVolume;
        playerSource.clip = playerSeparateSound;
        playerSource.Play();
    }
    public void PullSound()
    {
        playerSource.volume = playerPullVolume;
        playerSource.clip = playerPullSound;
        playerSource.Play();
    }
    public void ChargeSound()
    {
        playerSource.volume = playerChargeVolume;
        playerSource.clip = playerChargeSound;
        playerSource.Play();
    }
    public void DamageSound()
    {
        playerSeparateVolume = playerDamageVolume;
        playerSource.volume = playerDamageVolume;
        playerSource.Play();
    }
}
