using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_Main : MonoBehaviour
{
    public bool startAction;

    private Rigidbody rb;
    public Animator anim;

    [SerializeField] BossStatusManager status;

    public BossAudio audio;

    private Boss2_Ball bossBall;

    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    // �R���[�`���̊i�[
    public IEnumerator cor_Damage;

    public bool nowDmgReAct;

    private bool invincibility = false;
    private byte invincibilityFrame = 120;

    // �{�X������ё��x
    [SerializeField] private sbyte knockBackSpeed = 30;

    // �X�^����Ԃ̎���
    [SerializeField] private byte stanFrame = 90;

    // �q�b�g�X�g�b�v
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f;
    [SerializeField] private sbyte hitStopFrame = 30;

    // �ҋ@����
    [SerializeField] private byte atkDelayFrame = 150;

    // ���݂̏��
    public bool nowStan;
    public bool nowHitStop;

    // �g�����蔻��
    private GameObject waveHit;
    private GameObject waveLog;

    // �g�̐ݒ�
    [Header("----- �g�̐ݒ� -----")]
    public vfxManager vfxManager;   // ���e����p���鎅��vfxManager
    private sbyte arrayNum = 0;     // �g�𐶐�����Ƃ��߂�l���N���[����
    [SerializeField] private float waveSpeed = 7.5f;    // �g�̑��x
    [SerializeField] private float waveWidth = 0.225f;  // �g�̐U����
    [SerializeField] private float waveHight = 2.0f;    // �g�̍���
    [SerializeField] private GameObject waveObj;        // �g�̔���v���n�u
    public int waveCount = 3;   // �v�[���ɐ�������g�̐�
    [SerializeField] public int waveAngle = -1;  // �g�̕���

    // �g�̃R���W�����̃I�u�W�F�N�g�v�[��
    [System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();

    // �g�R���W�����ɃR���|�[�l���g����Ă���g����̃v�[��
    // �Y�����͔g����̃v�[���ɑΉ�
    private List<waveCollition> l_waveCollitions = new List<waveCollition>();

    bool deleteFg = false;

    private bool startLog = false;
    private bool startSet = false;

    [System.NonSerialized] public WavePool pool;

    private void OnTriggerEnter(Collider other)
    {
        // ���G���,�_���[�W�A�N�V�������Ȃ珈���𔲂���
        if (invincibility || nowDmgReAct) return;

        // �U���ɐG�ꂽ��
        if (other.CompareTag("Wave"))
        {
            if (waveHit == null)
            {
                if (!other.GetComponent<waveCollition>().CheckType(waveCollition.WAVE_TYPE.ENEMY))
                {
                    // ���g�Ƀ_���[�W��^�����Ȃ���Ԃɂ���
                    invincibility = true;

                    waveHit = other.gameObject;

                    // �_���[�W�A�j���[�V�������J�n ----------
                    anim.Play("Damage");
                    anim.SetBool("Damaging", true);
                    audio.bossSource.loop = false;
                    audio.bossSource.Stop();
                    audio.BossCrushing_1Sound();

                    status.hitPoint--;

                    // �ŏI�t�F�C�Y�ł͂Ȃ��Ƃ�
                    if (status.hitPoint > 0)
                    {
                        StopCoroutine(cor_Damage);
                        cor_Damage = DamageReaction();
                        StartCoroutine(cor_Damage);

                    }//----- if_stop ------

                    // �X�V�����g�����O�ƈ�v���Ȃ��Ȃ�
                    if (waveHit != waveLog)
                    {
                        // �g�̃��O���X�V����
                        waveLog = waveHit;

                    }//----- if_stop -----
                }
            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // �U���ɐG��Ă�����
        if (other.CompareTag("Wave"))
        {
            // �G�ꂽ�g���i�[����Ă���Ȃ�
            if (waveHit != null)
            {
                waveHit = null;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bossBall = GetComponentInChildren<Boss2_Ball>();
        anim = GetComponent<Animator>();
        cor_Damage = DamageReaction();
        audio = GetComponent<BossAudio>();
        pool = GetComponent<WavePool>();
        pool.AddEtoPDamage();

        startLog = g_camera.BossStart;

        // �g�̊e�����擾
        for (int i = 0; i < waveCount; i++)
        {
            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
            l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        }//----- for_stop -----

        status.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        if (!startSet && startLog != g_camera.BossStart)
        {
            bgm.BossBattleStart();
            startAction = true;
            startSet = true;
        }
        if (startAction)
        {
            anim.SetBool("ballSetting",true);
            audio.bossSource.Stop();
            audio.bossSource.loop = false;
            audio.BossTearBallSound();
            StartCoroutine(bossBall.SpawnBallDelay());
            startAction = false;
        }

        // ���g�̗̑͂��O�ɂȂ���
        // �q�b�g�X�g�b�v��Ԃɓ����Ă��Ȃ��Ȃ�
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            audio.bossSource.Stop();
            audio.bossSource.loop=false;
            audio.BossCrushing_1Sound();
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----

        if(deleteFg)
        {
            if (!audio.bossSource.isPlaying)
            {
                // ���g���\���ɂ���
                Debug.Log(this.gameObject.name + "���\���ɂ��܂������I");
                g_camera.BossEnd = true;
                this.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator AttackDelayTime()
    {
        for (int i = 0; i < atkDelayFrame; i++)
        {
            yield return null;
        }
        anim.SetBool("ballSetting", true);
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossTearBallSound();
        // �ʂ̎ˏo���s��
        StartCoroutine(bossBall.SpawnBallDelay());
    }

    private IEnumerator DamageReaction()
    {
        Debug.Log("�_���[�W���A�N�V�������J�n���܂������I");

        // �_���[�W���A�N�V������Ԃɂ���
        nowDmgReAct = true;

        // ���A�N�V�����J�n�n�_��ێ�
        Vector3 holdReactPos = transform.position;

        sbyte knockBack_LL = (sbyte)-knockBackSpeed;
        sbyte knockBackDistHold = (sbyte)knockBackSpeed;

        // �������x�������������܂ŌJ��Ԃ�
        while (rb.velocity.y >= knockBack_LL)
        {
            // ���g�ɑ��x������
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            if (rb.velocity.y < 0)
            {
                anim.SetBool("Falling", true);
            }
            knockBackDistHold--;

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        // ���n����
        rb.velocity = Vector3.zero;     // ���x���O�ɂ���
        rb.position = holdReactPos;     // �J�n�n�_�ŌŒ肷��

        // �X�^���A�j���[�V�����J�n ----------
        anim.SetBool("Falling",false);
        anim.SetBool("Damaging", false);
        audio.bossSource.Stop();

        nowStan = true;

        // �w�肵�����Ԃ̊ԃX�^����Ԃɂ���
        for (int i = 0; i < stanFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // �X�^���A�j���[�V�����I�� ----------

        nowStan = false;

        // �_���[�W���A�N�V������Ԃ�����
        nowDmgReAct = false;

        for(int i = 0; i < invincibilityFrame; i++)
        {
            yield return null;
        }

        invincibility = false;
    }

    private IEnumerator KnockOut()
    {
        sbyte knockOut_LL = (sbyte)-knockBackSpeed;

        // �������x�������𒴂�����Ƃ��΂炭�̊ԌJ��Ԃ�
        while (rb.velocity.y > knockOut_LL * 1.5)
        {
            // �㏸�ō��n�_���炢�ɓ��B�����Ȃ�
            if (knockBackSpeed == 0)
            {
                Debug.Log("�q�b�g�X�g�b�v���J�n���܂����I");

                // �Q�[���X�s�[�h���w�肵���l�ɕύX����
                Time.timeScale = hitStop;

                // �q�b�g�X�g�b�v�̌p�����Ԃ̊ԌJ��Ԃ�
                for (int i = 0; i < hitStopFrame; i++)
                {
                    // �P�t���[���x��������
                    yield return null;

                }//----- for_stop -----

                // �Q�[���X�s�[�h��ʏ푬�x�ɖ߂�
                Time.timeScale = 1f;

            }//----- if_stop -----

            // ���g�ɑ��x������
            rb.velocity = new Vector3(0, knockBackSpeed, 0);
            knockBackSpeed--;

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----
        deleteFg = true;
        
    }

    public void WaveCreate(float startPosX, Transform rayTrans)
    {
        pool.WaveCreate(waveSpeed * waveAngle, waveWidth, waveHight,
            waveCollition.WAVE_TYPE.ENEMY, startPosX, rayTrans);
    }
    //{
    //    // �z��ԍ����w�肷��
    //    arrayNum = vfxManager.WaveSpawn(startPosX, waveSpeed * waveAngle, waveHight, waveWidth, 1);

    //    for (int i = 0; i < waveCount; i++)
    //    {
    //        if (l_waveCollisionObj[i].transform.position.z != 0)
    //        {
    //            // �����蔻��ɔԍ����w��
    //            l_waveCollitions[i].waveNum = arrayNum;
    //            // �N���[���ɑΉ������� vfxManager ��ݒ�
    //            l_waveCollitions[i].vfxManager = vfxManager;
    //            // �g���v���C���[�������������g�ɐݒ�
    //            l_waveCollitions[i].waveType = waveCollition.WAVE_TYPE.ENEMY;
    //            // �R���W�����𓮂���
    //            l_waveCollitions[i].waveMode = waveCollition.WAVE_MODE.PLAY;
    //            // �R���W�����̍�����g�̍����ɐݒ�
    //            l_waveCollitions[i].transform.localScale = new Vector3(0, 0, 1);
    //            // �R���W�����̔����ʒu��g�̔����ʒu�ɐݒ�
    //            l_waveCollitions[i].waveStartPosition = new Vector3(startPosX, startPosY, 0);
    //            // �R���W�����̍ő卂�x��ݒ�
    //            l_waveCollitions[i].maxWaveHight = waveHight;

    //            break;
    //        }//----- if_stop -----
    //        else if (i == waveCount - 1)
    //        {
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }//----- elseif_stop -----

    //    }//----- for_stop -----
    //}

    private void OnApplicationQuit()
    {
        l_waveCollisionObj.Clear();
        l_waveCollitions.Clear();
    }
}
