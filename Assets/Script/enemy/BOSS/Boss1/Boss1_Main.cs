using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X�̃��C���X�N���v�g
// ���e������g�ݍ��킹�ă{�X�̍s����ω������܂�
//==================================================
// �����2023/05/16
// �{��
public class Boss1_Main : MonoBehaviour
{
    // ���g�̏����擾
    private Rigidbody rb;

    // ���g�̃A�j���[�^�[
    private Animator anim;

    public BossStatusManager bossStatus;   // �{�X�̃X�e�[�^�X

    // �{�X�̍s��
    private Boss1_Rush boss_Rush;       // �ːi�U��
    private Boss1_Shield boss_Shield;   // �h��s��


    public BossAudio audio;
    [SerializeField]private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;
    
    // �R���[�`���̊i�[
    public IEnumerator cor_DmgReaction;

    public bool nowDmgAct;

    [Header("�{�X�ғ��ݒ�")]
    [Tooltip("�s�����J�n�����܂�")]
    public bool startAction;    // �s���J�n
    [Tooltip("���G���")]
    public bool invincibility;  // ���G���
    [Tooltip("�{�X���X�g�q�b�g���̐�����ё��x")]
    [SerializeField] private sbyte distance_KB = 35;
    [Tooltip("���n���X�^���t���[��")]
    [SerializeField] private byte stanFrame = 60;

    [Header("�q�b�g�X�g�b�v�̐ݒ�")]
    [Tooltip("�ǂꂭ�炢�̃X�g�b�v�������邩")]
    [SerializeField, Range(0f, 1f)] private float stopSpeed = 0.5f;
    [Tooltip("�q�b�g�X�g�b�v����")]
    [SerializeField] private ushort stopFrame = 45;

    // �q�b�g�X�g�b�v�ғ����
    private bool activeHitStop;

    // �����t���O
    private bool deleteFg = false;

    private GameObject waveHit;
    private GameObject waveLog;

    //[SerializeField] private GameObject bossCamera;
    //private GoalCamera goalCamera;
    private bool startLog = false;
    private bool startSet = false;

    private void OnTriggerEnter(Collider other)
    {
        // ���G��ԂȂ珈���𔲂���
        if (invincibility) return;

        // �_���[�W�A�N�V�������Ȃ珈���𔲂���
        if (nowDmgAct) return;

        // �U���ɐG�ꂽ��
        if (other.CompareTag("Wave"))
        {
            // �h�䂵�Ă����ԂȂ�
            if (boss_Shield.nowShield)
            {
                waveHit = other.gameObject;

            }//----- if_stop -----

            // �����j�󂳂�Ă���Ȃ�
            if (boss_Shield.shieldBreak && !invincibility)
            {
                if (waveHit == null)
                {
                    // ���g�Ƀ_���[�W��^�����Ȃ���Ԃɂ���
                    invincibility = true;

                    waveHit = other.gameObject;

                    for (int i = 0; i < boss_Shield.waveCount; i++)
                    {
                        if (boss_Shield.l_waveCollisionObj[i] == waveHit)
                        {
                            return;

                        }//----- if_stop -----

                    }//----- for_stop -----
                    
                    audio.bossSource.Stop();
                    
                    audio.bossSource.loop = false;
                    audio.BossCrushing_1Sound();
                    // �_���[�W�A�j���[�V�������J�n
                    anim.Play("DamageUp");
                    anim.SetBool("staning", false);

                    bossStatus.hitPoint--;

                    // �t�F�C�Y��i�s������
                    if (boss_Shield.nowPhase < boss_Shield.vitPhase.Length - 1)
                    {
                        boss_Shield.nowPhase++;

                    }//----- if_stop -----

                    // �ŏI�t�F�C�Y�ł͂Ȃ��Ƃ�
                    if (!boss_Shield.lastPhase)
                    {
                        StopCoroutine(cor_DmgReaction);
                        cor_DmgReaction = DamageAction();
                        StartCoroutine(cor_DmgReaction);

                    }//----- if_stop -----

                    // �X�V�����g�����O�ƈ�v���Ȃ��Ȃ�
                    if (waveHit != waveLog)
                    {
                        // �g�̃��O���X�V����
                        waveLog = waveHit;

                    }//----- if_stop -----

                }//----- if_stop -----

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // �U���ɐG��Ă�����
        if (other.CompareTag("Wave"))
        {
            // �G�ꂽ�g���i�[����Ă�Ȃ�
            if (waveHit != null)
            {
                waveHit = null;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void Start()
    {
        // ���g�̏����擾
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audio = GetComponent<BossAudio>();
        // ���g�̋��������擾
        boss_Rush = GetComponentInChildren<Boss1_Rush>();
        boss_Shield = GetComponentInChildren<Boss1_Shield>();
        

        //goalCamera = bossCamera.GetComponent<GoalCamera>();
        startLog = g_camera.BossStart;

        // �R���[�`���̊i�[
        cor_DmgReaction = DamageAction();

        bossStatus.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        if(!startSet&&startLog!=g_camera.BossStart)
        {
            bgm.BossBattleStart();
            startAction = true;
            startSet = true;
        }
        if (startAction)
        {
            // ��]���J�n����
            StartCoroutine(boss_Shield.Rotate());

            // �J�n���Ԃ�؂�
            startAction = false;
        }//----- if_stop -----

        if (boss_Shield.lastPhase && (bossStatus.hitPoint <= 0) && !activeHitStop)
        {
            StartCoroutine(BossKnockOut());
            activeHitStop = true;
        }
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

    private IEnumerator HitStop()
    {
        Debug.Log("�q�b�g�X�g�b�v���J�n���܂����I");

        Time.timeScale = stopSpeed;

        for (int i = 0; i < stopFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        Time.timeScale = 1f;

        // �ޕ��֐�����΂�
        StartCoroutine(BossKnockOut());
    }

    private IEnumerator BossKnockOut()
    {
        sbyte KO_LowerLimit = (sbyte)-distance_KB;

        while (rb.velocity.y > KO_LowerLimit * 1.5)
        {
            // �P�t���[���x��������
            yield return null;

            if (distance_KB == 0)
            {
                Debug.Log("�q�b�g�X�g�b�v���J�n���܂����I");

                Time.timeScale = stopSpeed;

                for (int i = 0; i < stopFrame; i++)
                {
                    // �P�t���[���x��������
                    yield return null;

                }//----- for_stop -----

                Time.timeScale = 1f;

            }//----- if_stop -----

            // ����������
            rb.velocity = new Vector3(0, distance_KB, 0);
            distance_KB--;

        }//----- while_stop -----
        deleteFg = true;
    }

    public IEnumerator DamageAction()
    {
        Debug.Log("�_���[�W���A�N�V�����J�n");

        nowDmgAct = true;

        sbyte KB_LowerLimit = (sbyte)-distance_KB;
        sbyte KB_distanceHold = (sbyte)distance_KB;

        while (rb.velocity.y >= KB_LowerLimit)
        {
            // �P�t���[���x��������
            yield return null;
            if(rb.velocity.y<0)
            {
                anim.SetBool("DamageFall", true);
            }
            // ����������
            rb.velocity = new Vector3(0, KB_distanceHold, 0);
            KB_distanceHold--;

        }//----- while_stop -----


        // ���n����
        anim.SetBool("DamageFall", false);
        rb.velocity = Vector3.zero;
        rb.position = boss_Rush.fallPos;
        audio.bossSource.Stop();
        audio.BossStunSound();
        audio.bossSource.loop = true;
        anim.SetBool("staning", true);
        boss_Shield.nowStan = true;

        // �w�肵�����ԃX�^����Ԃɂ���
        for (int i = 0; i < stanFrame; i++)
        {
            yield return null;

        }//----- for_stop -----

        // �X�^���A�j���[�V�����I��
        anim.SetBool("staning", false);
        audio.bossSource.loop = false;
        if (audio.bossSource.isPlaying)
        {
            audio.bossSource.Stop();
        }
        boss_Shield.nowStan = false;

        nowDmgAct = false;

        StopCoroutine(boss_Shield.cor_Repair);
        boss_Shield.cor_Repair = boss_Shield.ShieldRepair();
        StartCoroutine(boss_Shield.cor_Repair);
    }
}
