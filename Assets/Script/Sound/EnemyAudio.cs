using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    //�G��AudioSource�R���|�[�l���g��ێ�����ϐ�
    public AudioSource enemySource;
    //�G��SE��ϐ��Ɋi�[���Ă���
    [Header("�ėp���s���̃T�E���h")]
    [SerializeField] private AudioClip enemyWalkSound;
    [Header("�m�b�N�o�b�N���̂̃T�E���h")]
    [SerializeField] private AudioClip enemyKnockBackSound;
    [Header("�Ռ��g�i���j�̃T�E���h")]
    [SerializeField] private AudioClip enemySmallShockWaveSound;
    [Header("�Ռ��g�i��j�̃T�E���h")]
    [SerializeField] private AudioClip enemyBigShockWaveSound;
    [Header("�G���G�W�����v�̃T�E���h")]
    [SerializeField] private AudioClip enemyJumpSound;
    [Header("���S���̃T�E���h")]
    [SerializeField] private AudioClip enemyDeathSound;
    [Header("�~�T�C���^�̓G�̔��˂̃T�E���h")]
    [SerializeField] private AudioClip enemyMissileShootSound;
    // Start is called before the first frame update
    void Start()
    {

        enemySource = GetComponent<AudioSource>();
    }
    [Header("�ėp���s�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float enemyWalkVolume;
    [Header("�m�b�N�o�b�N�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float enemyKnockBackVolume;
    [Header("�Ռ��g�i���j�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float enemySmallShockWaveVolume;
    [Header("�Ռ��g�i��j�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float enemyBigShockWaveVolume;
    [Header("�G���I�W�����v�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float enemyJumpVolume;
    [Header("���S���̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float enemyDeathVolume;
    [Header("�~�T�C���^�̓G�̔��˂̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float enemyMissileShootVolume;

    public void WalkSound()
    {
        enemySource.volume = enemyWalkVolume;
        enemySource.PlayOneShot(enemyWalkSound);
    }
    public void KnockBackSound()
    {
        
        enemySource.volume = enemyKnockBackVolume;
        enemySource.PlayOneShot(enemyKnockBackSound);
    }
    public void SmallShockWaveSound()
    {
        enemySource.volume = enemySmallShockWaveVolume;
        enemySource.PlayOneShot(enemySmallShockWaveSound);
    }
    public void BigShockWaveSound()
    {
        enemySource.volume = enemyBigShockWaveVolume;
        enemySource.PlayOneShot(enemyBigShockWaveSound);
    }
    public void JumpSound()
    {
        enemySource.volume = enemyJumpVolume;
        enemySource.PlayOneShot(enemyJumpSound);
    }
    public void DeathSound()
    {
        enemySource.volume = enemyDeathVolume;
        enemySource.PlayOneShot(enemyDeathSound);
    }
    public void MissileShootSound()
    {
        enemySource.volume = enemyMissileShootVolume;
        enemySource.PlayOneShot(enemyMissileShootSound);
    }
}
