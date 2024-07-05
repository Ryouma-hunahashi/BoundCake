using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAudio : MonoBehaviour
{
    public AudioSource playerSource;

    [Header("�����T�E���h")]
    public AudioClip playerWalkSound;
    [Header("��W�����v�̃T�E���h")]
    [SerializeField] private AudioClip playerWeakJumpSound;
    [Header("���W�����v�̃T�E���h")]
    [SerializeField] private AudioClip playerStrongJumpSound;
    [Header("�݂͂̃T�E���h")]
    [SerializeField] private AudioClip playerGrabSound;
    [Header("�݈͂ړ��̃T�E���h")]
    public AudioClip playerGrabMoveSound;
    [Header("�͂ݗ����̃T�E���h")]
    [SerializeField] private AudioClip playerSeparateSound;
    [Header("�݈͂�������̃T�E���h")]
    [SerializeField] private AudioClip playerPullSound;
    [Header("�`���[�W�̃T�E���h")]
    [SerializeField] private AudioClip playerChargeSound;
    [Header("��_���[�W�̃T�E���h")]
    [SerializeField] private AudioClip playerDamageSound;

    [Header("�����̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerWalkVolume;
    [Header("��W�����v�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerWeakVolume;
    [Header("���W�����v�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerStrongVolume;
    [Header("�݂͂̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerGrabVolume;
    [Header("�݈͂ړ��̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerGrabMoveVolume;
    [Header("�͂ݗ����̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerSeparateVolume;
    [Header("�݈͂�������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerPullVolume;
    [Header("�`���[�W�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float playerChargeVolume;

    [Header("��_���[�W�̃T�E���h����")]
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
