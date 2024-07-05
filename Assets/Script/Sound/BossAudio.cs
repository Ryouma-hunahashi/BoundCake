using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAudio : MonoBehaviour
{
    public AudioSource bossSource;
    //====================���̑��E���ʓ�====================
    [Header("�{�X���j���@")]
    [SerializeField] AudioClip bossCrushingSound_1;
    [Header("�{�X���j���A")]
    [SerializeField] AudioClip bossCrushingSound_2;
    [Header("�{�X�_�b�V��")]
    [SerializeField] AudioClip bossDashSound;
    [Header("�{�X�X�^��")]
    [SerializeField] AudioClip bossStunSound;
    [Header("�������ƃ{�[���E������")]
    [SerializeField] AudioClip bossTearBallSound;
    [Header("�������ƃ{�[���E������")]
    [SerializeField] AudioClip bossThrowBallSound;
    [Header("�������ƃ{�[���E����")]
    [SerializeField] AudioClip bossFlightBallSound;
    [Header("�������ƃ{�[���E�ǂ��[��")]
    [SerializeField] AudioClip bossStompBallSound;
    //================�P�{�X=================================
    [Header("1�{�X�K�[�h")]
    [SerializeField] AudioClip boss1_GuardSound;
    [Header("1�{�X���ӂ�")]
    [SerializeField] AudioClip boss1_ShieldBreakSound;
    [Header("1�{�X�U�艺�낵")]
    [SerializeField] AudioClip boss1_AttackSound;
    //================2�{�X=================================

    //================3�{�X================================
    [Header("3�{�X�_�~�[����")]
    [SerializeField] AudioClip boss3_ExPlosionSound;
    [Header("3�{�X�o��E�_�~�[����")]
    [SerializeField] AudioClip boss3_SpawnSound;
    [Header("3�{�X�ړ�")]
    [SerializeField] AudioClip boss3_MoveSound;
    //================4�{�X================================
    [Header("4�{�X�ǔ��e����")]
    [SerializeField] AudioClip boss4_BulletAttackSound;
    [Header("4�{�X�ǔ��e���e")]
    [SerializeField] AudioClip boss4_BulletHitSound;
    [Header("4�{�X�������J�n")]
    [SerializeField] AudioClip boss4_InvisibleStartSound;
    [Header("4�{�X�������I��")]
    [SerializeField] AudioClip boss4_InvisibleEndSound;
    [Header("4�{�X���g�̐���")]
    [SerializeField] AudioClip boss4_DummySpawnSound;
    [Header("4�{�X���g�̏���")]
    [SerializeField] AudioClip boss4_DummyDeleteSound;
    //================5�{�X================================
    [Header("5�{�X��W�����v")]
    [SerializeField] AudioClip boss5_SuperJumpSound;
    [Header("5�{�X�G������")]
    [SerializeField] AudioClip boss5_SummonEnemySound;
    [Header("5�{�X��������")]
    [SerializeField] AudioClip boss5_FloorDeleteSound;

    //���ʒ����p
    [Header("�{�X���j�@�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossCrushing_1Volume;
    [Header("�{�X���j�A�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossCrushing_2Volume;
    [Header("�{�X�_�b�V���̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossDashVolume;
    [Header("�{�X�X�^���̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossStunVolume;
    [Header("�������ƃ{�[��������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossTearBallVolume;
    [Header("�������ƃ{�[��������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossThrowBallVolume;
    [Header("�������ƃ{�[�����ẴT�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossFlightBallVolume;
    [Header("�������ƃ{�[���ǂ��[��̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float bossStompBallVolume;
    [Header("1�{�X�K�[�h�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss1_GuardVolume;
    [Header("1�{�X���ӂ��̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss1_ShieldBreakVolume;
    [Header("�P�{�X�U�艺�낵�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss1_AttackVolume;
    [Header("�R�{�X�_�~�[�����̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss3_ExplosionVolume;
    [Header("�R�{�X�o��E�_�~�[�����̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss3_SpawnVolume;
    [Header("�R�{�X�ړ��̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss3_MoveVolume;
    [Header("�S�{�X�ǔ��e���˂̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss4_BulletAttackVolume;
    [Header("4�{�X�ǔ��e���e�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss4_BulletHitVolume;
    [Header("4�{�X�������J�n�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss4_InvisibleStartVolume;
    [Header("4�{�X�����������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss4_InvisibleEndVolume;
    [Header("4�{�X���g�̐����̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss4_DummySpawnVolume;
    [Header("4�{�X���g�̏��ł̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss4_DummyDeleteVolume;
    [Header("5�{�X��W�����v�̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss5_SuperJumpVolume;
    [Header("5�{�X�G�������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float boss5_SummonEnemyVolume;
    [Header("5�{�X���������̃T�E���h����")]
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
