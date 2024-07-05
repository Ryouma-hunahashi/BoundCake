using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X�̃��C���X�N���v�g
// ���U���⏢���A�_���[�W�Ȃǂ̎厲���i��܂��@
//==================================================
// �����2023/05/26    �X�V��2023/05/28
// �{��
public class Boss3_Main : MonoBehaviour
{
    [SerializeField] private bool startAction;

    // �w�߂̏��
    private GameObject bombOperator;
    private Setup_BombPointer bompSetup;
    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    // ��]���̕ێ��l
    public Rotation groundRotation;         // ��]���̎擾
    private float holdRotateValue;          // ��]���̏����l
    public float stanRotateValue = 0.2f;    // �X�^�����̒l

    // ���g�̏��
    private Rigidbody rb;
    public Animator anim;
    public BossAudio audio;

    // �����̏��
    private Boss3_Bomb boss_Bomb;

    // �R���[�`���̊i�[
    public IEnumerator cor_Damage;

    // �{�X�̐ݒ�
    public BossStatusManager status;

    // �o���n�_�̐ݒ�

    // ���G���
    [SerializeField] private bool invincibility = true;

    // �ҋ@���Ԃ̐ݒ�
    [SerializeField] private byte atkDelayFrame = 60;

    // ��_���[�W���̐ݒ�
    [SerializeField] private sbyte knockBackSpeed = 30;

    // �X�^����Ԃ̐ݒ�
    [SerializeField] private byte stanFrame = 30;
    [SerializeField] private byte blastedStanFrame = 240;

    // �q�b�g�X�g�b�v
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f; 
    [SerializeField] private byte hitStopFrame = 30;

    // ���݂̏��

    public bool nowAtkDelay;
    public bool nowDmgReAct;
    public bool nowStan;
    public bool nowHitStop;

    // �g�����蔻��
    private GameObject waveHit;
    private GameObject waveLog;

    private bool startLog = false;
    private bool startSet = false;


    private void OnTriggerEnter(Collider other)
    {
        // ���G���,�_���[�W�A�N�V�������Ȃ珈���𔲂���
        if (invincibility || nowDmgReAct) return;

        // �U���ɐG�ꂽ��
        if (other.CompareTag("Wave"))
        {
            if (waveHit == null)
            {
                if (!other.GetComponent<waveCollition>().CheckType( waveCollition.WAVE_TYPE.ENEMY))
                {
                    // ���g�Ƀ_���[�W��^�����Ȃ���Ԃɂ���
                    invincibility = true;

                    waveHit = other.gameObject;

                    // �_���[�W�A�j���[�V�������J�n ----------

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
        // �w�߂̏����擾
        bombOperator = GameObject.Find("BombPointOperator").gameObject;
        bompSetup = bombOperator.GetComponent<Setup_BombPointer>();

        // ��]�̏����l��ێ����Ă���
        holdRotateValue = groundRotation.angularVelocity;

        // ���g�̏����擾
        GameObject thisObj = this.transform.gameObject;
        rb = thisObj.GetComponent<Rigidbody>();
        
        // ���g�̋������擾
        boss_Bomb = thisObj.GetComponentInChildren<Boss3_Bomb>();
        anim = GetComponent<Animator>();
        audio = GetComponent<BossAudio>();

        // �R���[�`���̊i�[
        cor_Damage = DamageReaction();

        // HP�̐ݒ�
        status.hitPoint = 3;
        startLog = g_camera.BossStart;

        // ���e�𐶐��������̎q�I�u�W�F��vfxManager���擾
        //vfxManager = transform.parent.transform.parent.GetChild(0).GetComponentInChildren<vfxManager>();

        // ���g����]��(Operator)�̎q�I�u�W�F�N�g�ɂ��� ----- �o���^�C�~���O�ɖ���Ăяo������
        //GameObject bomb_OP = GameObject.Find("BombPointOperator").gameObject;
        //this.transform.SetParent(bomb_OP.transform, false);
    }

    private void FixedUpdate()
    {

        if (!startSet && startLog != g_camera.BossStart)
        {
            bgm.BossBattleStart();
            startAction = true;
            startSet = true;
        }
        // ���g�̗̑͂��O�ɂȂ���
        // �q�b�g�X�g�b�v��Ԃɓ����Ă��Ȃ��Ȃ�
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----

        // �L�m�R���e�����������Ȃ�
        if (bompSetup.blasted)
        {
            // �X�^����ԂɈڍs����
            StartCoroutine(StanAfterBlast());
            bompSetup.blasted = false;
        }
    }

    private IEnumerator AttackDelayTime()
    {
        // �U���J�n�܂ł̑ҋ@��Ԃ��J�n
        nowAtkDelay = true;

        // ����A�j���[�V�����J�n ----------
        anim.SetBool("Grawing", false);
        anim.Play("Dive");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.Boss3_MoveSound();

        for(int i = 0; i < atkDelayFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;
        }

        // �U�����J�n����
        nowAtkDelay = false;

        // �L�m�R���e����������
        StartCoroutine(boss_Bomb.SpawnBomb());
    }

    private IEnumerator StanAfterBlast()
    {
        // ����������̃X�^��
        invincibility = false;

        // ���̉�]���x��ݒ肵���l�ɕύX����
        groundRotation.angularVelocity = stanRotateValue;

        // �X�^���A�j���[�V�������J�n -----------
        anim.SetBool("Staning", true);
        audio.bossSource.Stop();
        audio.bossSource.loop = true;
        audio.BossStunSound();

        nowStan = true;

        for (int i = 0; i < blastedStanFrame; i++)
        {
            // �U�����󂯂Ă���Ȃ珈���𔲂���
            if (nowDmgReAct) yield break;

            // �P�t���[���x��������
            yield return null;
        }

        // �U�����󂯂Ă���Ȃ珈���𔲂���
        if (nowDmgReAct) yield break;

        // �X�^���A�j���[�V�����I�� ----------
        anim.SetBool("Staning", false);
        audio.bossSource.Stop();
        audio.bossSource.loop=false;


        nowStan = false;

        // ���̉�]���x�������l�ɖ߂�
        groundRotation.angularVelocity = holdRotateValue;

        // �U���ҋ@��ԂɈڍs����
        invincibility = true;
        StartCoroutine(AttackDelayTime());

    }

    private IEnumerator DamageReaction()
    {
        Debug.Log("�_���[�W���A�N�V�������J�n���܂������I");

        // ����̉�]���~�߂�
        groundRotation.angularVelocity = 0;

        // �_���[�W���A�N�V������Ԃɂ���
        nowDmgReAct = true;
        anim.Play("Damage");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossCrushing_1Sound();

        // ���A�N�V�����J�n�n�_��ێ�
        Vector3 holdReactPos = transform.position;

        sbyte knockBack_LL = (sbyte)-knockBackSpeed;
        sbyte knockBackDistHold = (sbyte)knockBackSpeed;

        // �������x�������������܂ŌJ��Ԃ�
        while (rb.velocity.y >= knockBack_LL)
        {
            // ���g�ɑ��x������
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            knockBackDistHold--;
            if(knockBackDistHold<0)
            {
                anim.SetBool("Falling", true);
            }

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        // ���n����
        rb.velocity = Vector3.zero;     // ���x���O�ɂ���
        rb.position = holdReactPos;     // �J�n�n�_�ŌŒ肷��
        
        // �X�^���A�j���[�V�����J�n ----------
        anim.SetBool("Falling", false);
        anim.SetBool("Staning", true);
        audio.bossSource.Stop();
        audio.bossSource.loop = true;
        audio.BossStunSound();
        nowStan = true;

        // �w�肵�����Ԃ̊ԃX�^����Ԃɂ���
        for (int i = 0; i < stanFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // �X�^���A�j���[�V�����I�� ----------
        anim.SetBool("Staning", false);
        audio.bossSource.Stop();
        audio.bossSource.loop= false;

        nowStan = false;

        // �_���[�W���A�N�V������Ԃ�����
        nowDmgReAct = false;

        // ����̉�]���J�n������
        groundRotation.angularVelocity = holdRotateValue;

        // �U���ҋ@��ԂɈڍs����
        StartCoroutine(AttackDelayTime());
    }

    private IEnumerator KnockOut()
    {
        // ���̉�]���~������
        groundRotation.angularVelocity = 0;
        sbyte knockOut_LL = (sbyte)-knockBackSpeed;
        
        anim.Play("Damage");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossCrushing_1Sound();
        // �������x�������𒴂�����Ƃ��΂炭�̊ԌJ��Ԃ�
        while (rb.velocity.y > knockOut_LL * 2)
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

        

        // ���g���\���ɂ���
        Debug.Log(this.gameObject.name + "���\���ɂ��܂������I");
        g_camera.BossEnd = true;
        this.gameObject.SetActive(false);
    }

    
}
