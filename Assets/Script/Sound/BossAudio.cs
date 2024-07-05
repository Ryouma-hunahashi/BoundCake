using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{
    public AudioSource bossSource;
    //====================その他・共通等====================
    [Header("ボス撃破音①")]
    [SerializeField] AudioClip bossCrushingSound_1;
    [Header("ボス撃破音②")]
    [SerializeField] AudioClip bossCrushingSound_2;
    [Header("ボスダッシュ")]
    [SerializeField] AudioClip bossDashSound;
    [Header("ボススタン")]
    [SerializeField] AudioClip bossStunSound;
    [Header("モヤっとボール・ちぎる")]
    [SerializeField] AudioClip bossTearBallSound;
    [Header("モヤっとボール・投げる")]
    [SerializeField] AudioClip bossThrowBallSound;
    [Header("モヤっとボール・飛翔")]
    [SerializeField] AudioClip bossFlightBallSound;
    [Header("モヤっとボール・どしーん")]
    [SerializeField] AudioClip bossStompBallSound;
    //================１ボス=================================
    [Header("1ボスガード")]
    [SerializeField] AudioClip boss1_GuardSound;
    [Header("1ボス盾砕き")]
    [SerializeField] AudioClip boss1_ShieldBreakSound;
    [Header("1ボス振り下ろし")]
    [SerializeField] AudioClip boss1_AttackSound;
    //================2ボス=================================

    //================3ボス================================
    [Header("3ボスダミー爆発")]
    [SerializeField] AudioClip boss3_ExPlosionSound;
    [Header("3ボス登場・ダミー生成")]
    [SerializeField] AudioClip boss3_SpawnSound;
    [Header("3ボス移動")]
    [SerializeField] AudioClip boss3_MoveSound;
    //================4ボス================================
    [Header("4ボス追尾弾発射")]
    [SerializeField] AudioClip boss4_BulletAttackSound;
    [Header("4ボス追尾弾着弾")]
    [SerializeField] AudioClip boss4_BulletHitSound;
    [Header("4ボス透明化開始")]
    [SerializeField] AudioClip boss4_InvisibleStartSound;
    [Header("4ボス透明化終了")]
    [SerializeField] AudioClip boss4_InvisibleEndSound;
    [Header("4ボス分身体生成")]
    [SerializeField] AudioClip boss4_DummySpawnSound;
    [Header("4ボス分身体消滅")]
    [SerializeField] AudioClip boss4_DummyDeleteSound;
    //================5ボス================================
    [Header("5ボス大ジャンプ")]
    [SerializeField] AudioClip boss5_SuperJumpSound;
    [Header("5ボス雑魚召喚")]
    [SerializeField] AudioClip boss5_SummonEnemySound;
    [Header("5ボス床を消す")]
    [SerializeField] AudioClip boss5_FloorDeleteSound;

    //音量調整用
    [Header("ボス撃破①のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossCrushing_1Volume;
    [Header("ボス撃破②のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossCrushing_2Volume;
    [Header("ボスダッシュのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossDashVolume;
    [Header("ボススタンのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossStunVolume;
    [Header("モヤっとボールちぎるのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossTearBallVolume;
    [Header("モヤっとボール投げるのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossThrowBallVolume;
    [Header("モヤっとボール飛翔のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossFlightBallVolume;
    [Header("モヤっとボールどしーんのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float bossStompBallVolume;
    [Header("1ボスガードのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss1_GuardVolume;
    [Header("1ボス盾砕きのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss1_ShieldBreakVolume;
    [Header("１ボス振り下ろしのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss1_AttackVolume;
    [Header("３ボスダミー爆発のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss3_ExplosionVolume;
    [Header("３ボス登場・ダミー生成のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss3_SpawnVolume;
    [Header("３ボス移動のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss3_MoveVolume;
    [Header("４ボス追尾弾発射のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss4_BulletAttackVolume;
    [Header("4ボス追尾弾着弾のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss4_BulletHitVolume;
    [Header("4ボス透明化開始のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss4_InvisibleStartVolume;
    [Header("4ボス透明化解除のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss4_InvisibleEndVolume;
    [Header("4ボス分身体生成のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss4_DummySpawnVolume;
    [Header("4ボス分身体消滅のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss4_DummyDeleteVolume;
    [Header("5ボス大ジャンプのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss5_SuperJumpVolume;
    [Header("5ボス雑魚召喚のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss5_SummonEnemyVolume;
    [Header("5ボス床を消すのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float boss5_FloorDeleteVolume;
    // Start is called before the first frame update
    void Start()
    {
        bossSource = GetComponent<AudioSource>();
    }
    public void BossCrushing_1Sound()
    {
        bossSource.volume = bossCrushing_1Volume;
        bossSource.PlayOneShot(bossCrushingSound_1);
    }
    public void BossCrushing_2Sound()
    {
        bossSource.volume = bossCrushing_2Volume;
        bossSource.PlayOneShot(bossCrushingSound_2);
    }
    public void BossDashSound()
    {
        bossSource.volume = bossDashVolume;
        bossSource.PlayOneShot(bossDashSound);
    }
    public void BossStunSound()
    {
        bossSource.volume = bossStunVolume;
        bossSource.PlayOneShot(bossStunSound);
    }
    public void BossTearBallSound()
    {
        bossSource.volume = bossTearBallVolume;
        bossSource.PlayOneShot(bossTearBallSound);
    }
    public void BossThrowBallSound()
    {
        bossSource.volume = bossThrowBallVolume;
        bossSource.PlayOneShot(bossThrowBallSound);
    }
    public void BossFlightBallSound()
    {
        bossSource.volume = bossFlightBallVolume;
        bossSource.PlayOneShot(bossFlightBallSound);
    }
    public void BossStompBallSound()
    {
        bossSource.volume = bossStompBallVolume;
        bossSource.PlayOneShot(bossStompBallSound);
    }
    public void Boss1_GuardSound()
    {
        bossSource.volume = boss1_GuardVolume;
        bossSource.PlayOneShot(boss1_GuardSound);
    }
    public void Boss1_ShieldBreakSound()
    {
        bossSource.volume = boss1_ShieldBreakVolume;
        bossSource.PlayOneShot(boss1_ShieldBreakSound);
    }
    public void Boss1_AttackSound()
    {
        bossSource.volume = boss1_AttackVolume;
        bossSource.PlayOneShot(boss1_AttackSound);
    }
    public void Boss3_ExplosionSound()
    {
        bossSource.volume = boss3_ExplosionVolume;
        bossSource.PlayOneShot(boss3_ExPlosionSound);
    }
    public void Boss3_SpawnSound()
    {
        bossSource.volume = boss3_SpawnVolume;
        bossSource.PlayOneShot(boss3_SpawnSound);
    }
    public void Boss3_MoveSound()
    {
        bossSource.volume = boss3_MoveVolume;
        bossSource.PlayOneShot(boss3_MoveSound);
    }
    public void Boss4_BulletAttackSound()
    {
        bossSource.volume = boss4_BulletAttackVolume;
        bossSource.PlayOneShot(boss4_BulletAttackSound);
    }
    public void Boss4_BulletHitSound()
    {
        bossSource.volume = boss4_BulletHitVolume;
        bossSource.PlayOneShot(boss4_BulletHitSound);
    }
    public void Boss4_InvisibleStartSound()
    {
        bossSource.volume = boss4_InvisibleStartVolume;
        bossSource.PlayOneShot(boss4_InvisibleStartSound);
    }
    public void Boss4_InvisibleEndSound()
    {
        bossSource.volume = boss4_InvisibleEndVolume;
        bossSource.PlayOneShot(boss4_InvisibleEndSound);
    }
    public void Boss4_DummySpawnSound()
    {
        bossSource.volume = boss4_DummySpawnVolume;
        bossSource.PlayOneShot(boss4_DummySpawnSound);
    }
    public void Boss4_DummyDeleteSound()
    {
        bossSource.volume = boss4_DummyDeleteVolume;
        bossSource.PlayOneShot(boss4_DummyDeleteSound);
    }
    public void Boss5_SuperJumpSound()
    {
        bossSource.volume = boss5_SuperJumpVolume;
        bossSource.PlayOneShot(boss5_SuperJumpSound);
    }
    public void Boss5_SummonEnemySound()
    {
        bossSource.volume = boss5_SummonEnemyVolume;
        bossSource.PlayOneShot(boss5_SummonEnemySound);
    }
    public void Boss5_FloorDeleteSound()
    {
        bossSource.volume = boss5_FloorDeleteVolume;
        bossSource.PlayOneShot(boss5_FloorDeleteSound);
    }
}
