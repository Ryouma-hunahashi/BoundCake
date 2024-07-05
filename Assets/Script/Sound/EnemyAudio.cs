using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAudio : MonoBehaviour
{
    //敵のAudioSourceコンポーネントを保持する変数
    public AudioSource enemySource;
    //敵のSEを変数に格納していく
    [Header("汎用歩行時のサウンド")]
    [SerializeField] private AudioClip enemyWalkSound;
    [Header("ノックバック時ののサウンド")]
    [SerializeField] private AudioClip enemyKnockBackSound;
    [Header("衝撃波（小）のサウンド")]
    [SerializeField] private AudioClip enemySmallShockWaveSound;
    [Header("衝撃波（大）のサウンド")]
    [SerializeField] private AudioClip enemyBigShockWaveSound;
    [Header("雑魚敵ジャンプのサウンド")]
    [SerializeField] private AudioClip enemyJumpSound;
    [Header("死亡時のサウンド")]
    [SerializeField] private AudioClip enemyDeathSound;
    [Header("ミサイル型の敵の発射のサウンド")]
    [SerializeField] private AudioClip enemyMissileShootSound;
    // Start is called before the first frame update
    void Start()
    {

        enemySource = GetComponent<AudioSource>();
    }
    [Header("汎用歩行のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float enemyWalkVolume;
    [Header("ノックバックのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float enemyKnockBackVolume;
    [Header("衝撃波（小）のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float enemySmallShockWaveVolume;
    [Header("衝撃波（大）のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float enemyBigShockWaveVolume;
    [Header("雑魚的ジャンプのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float enemyJumpVolume;
    [Header("死亡時のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float enemyDeathVolume;
    [Header("ミサイル型の敵の発射のサウンド音量")]
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
