using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X�̃��C���X�N���v�g
// ���e������g�ݍ��킹�ă{�X�̍s����ω������܂�
// ��[Setup_AvatarPoint]�X�N���v�g���t�^���ꂽ
// �@�I�u�W�F�N�g�����݂��Ă���V�[�����ł̂ݍs���\�ł�
//==================================================
// �����2023/05/24    �X�V��2023/05/30
// �{��
public class Boss4_Main : MonoBehaviour
{
    [SerializeField] private bool startAction;
    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    // �w�߂̏��
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // ���g�̏��
    private Rigidbody rb;
    public Animator anim;
    private EnemyEffectManager ef_Manager;
    public BossAudio audio;

    // ���g�̏��
    public GameObject[] avaObj;
    public Boss4_Avatar_Main[] avaMain;

    // �������
    public Boss4_MagicBall actMB;
    public Boss4_Invisible actInv;
    public Boss4_Teleportation actTP;

    // �R���[�`���̊i�[
    private IEnumerator cor_Damage;
    private IEnumerator cor_AtkDelay;

    // �X�e�[�^�X
    public BossStatusManager status;

    // ���͋��̃v���n�u
    public GameObject magicBall;

    // �����_�������擾
    public int[] randomPoint;

    // �t�F�C�Y�ݒ�
    public byte nowPhase = 0;
    public byte[] setAvatarPhase = new byte[3];

    // �ҋ@�̐ݒ�
    [SerializeField] private byte stanFrame = 90;
    [SerializeField] private byte hitStopFrame = 30;

    // ��_���[�W���̐ݒ�
    public bool invincibility = false;  // ���G���
    [SerializeField] private sbyte knockBackSpeed = 30;
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f;

    // ���݂̏��
    public bool nowAtkDelay;
    public bool nowStan;
    public bool nowDmgReAct;
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
                // ���g�Ƀ_���[�W��^�����Ȃ���Ԃɂ���
                invincibility = true;

                waveHit = other.gameObject;

                // �_���[�W�A�j���[�V�������J�n ----------

                status.hitPoint--;

                if (nowPhase < 2) nowPhase++;

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
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();



        // ���g�̏����擾
        int avatarCnt = avatarOperator.transform.childCount;
        avaObj = new GameObject[avatarCnt];
        avaMain = new Boss4_Avatar_Main[avatarCnt];

        // �����n�_�̐��J��Ԃ�
        for (int i = 0; i < avatarCnt; i++)
        {
            // ���g���i�[
            avaObj[i] = avatarOperator.transform.GetChild(i).GetChild(0).gameObject;

            // ���g�̃��C���������擾
            avaMain[i] = avaObj[i].GetComponent<Boss4_Avatar_Main>();

        }//----- for_stop -----

        // ���g�̏����擾
        GameObject thisObj = this.gameObject;
        rb = thisObj.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        audio = GetComponent<BossAudio>();
        
        // �������̎擾
        actMB = thisObj.GetComponentInChildren<Boss4_MagicBall>();
        actInv = thisObj.GetComponentInChildren<Boss4_Invisible>();
        actTP = thisObj.GetComponentInChildren<Boss4_Teleportation>();

        startLog = g_camera.BossStart;

        // �R���[�`���̊i�[
        cor_Damage = DamageReaction();
        //cor_AtkDelay = AttackDelay(0);

        // �{�X�̗̑͐ݒ�
        status.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        if (!startSet && startLog != g_camera.BossStart)
        {
            startAction = true;
            startSet = true;
            bgm.BossBattleStart();
        }
        // �U�����J�n����
        if (startAction)
        {
            if (nowDmgReAct) return;
            startAction = false;

            // �����ŏ����ʒu���w��
            //Randomizer();

            // �\����Ԃ�ύX����
            StartCoroutine(actInv.AnimationDelay());

            // �������̌�Ɉʒu��ύX����
            //actInv.InvisibleObjects();
            //actTP.Teleportation();

            //StopCoroutine(cor_AtkDelay);
            //cor_AtkDelay = AttackDelay(magicBallDelay);
            //StartCoroutine(cor_AtkDelay);

        }//----- if_stop -----

        // ���g�̗̑͂��O�ɂȂ���
        // �q�b�g�X�g�b�v��Ԃɓ����Ă��Ȃ��Ȃ�
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----
    }

    public void Randomizer()
    {
        // �w�߂̎q�I�u�W�F�N�g�̐����擾����
        int opChildCnt = avatarOperator.transform.childCount;

        // �w�߂̎q�I�u�W�F�N�g�̐��z����쐬����
        randomPoint = new int[opChildCnt];

        // �w�߂̎q�I�u�W�F�N�g�̐��J��Ԃ�
        for (int i = 0; i < opChildCnt; i++)
        {
            // �z����ɂO����q�I�u�W�F�N�g�̐��܂ł̗������i�[����
            randomPoint[i] = Random.Range(0, opChildCnt);

            // ��������������U�蕪������܂ŌJ��Ԃ�
            for (int j = 0; j < i; j++)
            {
                // ���������ɓo�^����Ă�����̂Ȃ�
                if (randomPoint[i] == randomPoint[j])
                {
                    // �Ē��I���s��
                    randomPoint[i] = Random.Range(0, opChildCnt);

                    // �Ē��I���s���Ă���v�����Ƃ��̂��߂�
                    // �ēx�J��Ԃ����悤�ɐݒ肷��
                    j = -1;

                }//----- if_stop -----

            }//----- for_stop -----

        }//----- for_stop -----
    }

    //private IEnumerator AttackDelay(byte _delay)
    //{
    //    // �U���ҋ@���Ԃ��J�n����
    //    nowAtkDelay = true;

    //    for(int i = 0; i < _delay; i++)
    //    {
    //        // �P�t���[���x��������
    //        yield return null;

    //        if (nowDmgReAct) yield break;

    //    }//----- for_stop -----

    //    // �U���ҋ@���Ԃ𔲂���
    //    nowAtkDelay = false;


    //    // ������Ԃ̂Ƃ�
    //    if (!actInv.nowInvisible)
    //    {
    //        startAction = true;
    //    }

    //    // �s����Ԃ̂Ƃ��Ɉړ���Ȃ�
    //    if (actInv.nowInvisible && actTP.teleported)
    //    {
    //        actTP.teleported = false;

    //        // ���@�U�����s��
    //        actMB.MagicAttack(magicBall);

    //        StopCoroutine(cor_AtkDelay);
    //        cor_AtkDelay = AttackDelay(magicBallDelay);
    //        StartCoroutine(cor_AtkDelay);
    //    }

    //    // �s����Ԃ̂Ƃ��ɖ��͋����������Ȃ�
    //    if(actInv.nowInvisible && actMB.shot)
    //    {
    //        actMB.shot = false;

    //        // ������Ԃɂ���
    //        actInv.VisualizationObjects();

    //        // ���g�̓����蔻��𕜊�������
    //        for (int i = 0; i < avaMain.Length; i++)
    //        {
    //            // �O�͖{�̂ɂȂ邽�ߕ\�����Ȃ�
    //            if (i == 0) continue;

    //            avaMain[i].damage = false;
    //        }

    //        // ���G��Ԃ���������
    //        invincibility = false;

    //        StopCoroutine(cor_AtkDelay);
    //        cor_AtkDelay = AttackDelay(teleportDelay);
    //        StartCoroutine(cor_AtkDelay);
    //    }
    //}

    private IEnumerator StanStatus()
    {
        // ���g���݂���悤�ɂ���
        MeshRenderer myMesh = this.gameObject.GetComponent<MeshRenderer>();
        myMesh.enabled = true;

        // �X�^���A�j���[�V�����J�n ----------
        nowStan = true;

        // ���G��Ԃł͂Ȃ��ԌJ��Ԃ�
        while (!invincibility) yield return null;

        // �X�^���A�j���[�V�����I�� ----------
        nowStan = false;
    }

    private IEnumerator DamageReaction()
    {
        Debug.Log("�_���[�W���A�N�V�������J�n���܂������I");

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
            // ���_�ɋ߂��Ȃ��Ă���Ȃ�
            if(rb.velocity.y < 5)
            {
                // ���g���݂���悤�ɂ���
                SpriteRenderer myMesh = this.gameObject.GetComponent<SpriteRenderer>();
                myMesh.enabled = true;

                anim.SetBool("Falling", true);

                actInv.nowInvisible = false;
            }

            // ���g�ɑ��x������
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            knockBackDistHold--;

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        // ���n����
        rb.velocity = Vector3.zero;     // ���x���O�ɂ���
        rb.position = holdReactPos;     // �J�n�n�_�ŌŒ肷��

        // �X�^���A�j���[�V�����J�n ----------
        audio.bossSource.Stop();
        audio.bossSource.loop = true;
        audio.BossStunSound();
        anim.SetBool("Falling",false);
        anim.SetBool("Staning", true);

        nowStan = true;

        // �w�肵�����Ԃ̊ԃX�^����Ԃɂ���
        for (int i = 0; i < stanFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // �X�^���A�j���[�V�����I�� ----------
        audio.bossSource.Stop();
        audio.bossSource.loop=false;
        anim.SetBool("Staning", false);
        nowStan = false;

        // �_���[�W���A�N�V������Ԃ�����
        nowDmgReAct = false;

        // ���g�𕜊�������
        for(int i = 0; i < avaMain.Length; i++)
        {
            // �O�͖{�̂ɂȂ邽�ߕ\�����Ȃ�
            if (i == 0) continue;

            avaMain[i].active = true;
            avaMain[i].damage = false;
        }

        // �U�����J�n����
        startAction = true;
    }

    private IEnumerator KnockOut()
    {
        // �_���[�W���A�N�V������Ԃɂ���
        nowDmgReAct = true;

        // ���g�����ł�����
        actInv.InvisibleObjects();

        // ���g���݂���悤�ɂ���
        SpriteRenderer myMesh = this.gameObject.GetComponent<SpriteRenderer>();
        myMesh.enabled = true;
        actInv.nowInvisible = false;

        sbyte knockOut_LL = (sbyte)-knockBackSpeed;

        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossCrushing_1Sound();

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

        // ���g���\���ɂ���
        Debug.Log(this.gameObject.name + "���\���ɂ��܂������I");
        g_camera.BossEnd = true;
        this.gameObject.SetActive(false);
    }
}
