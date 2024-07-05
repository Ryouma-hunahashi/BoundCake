using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_Main : MonoBehaviour
{
    //==================================================
    //          �T�{�X�̋����S��
    // ������ȊO�Ƀt�F�C�Y�Q�Əo���X�N���v�g�����݂��܂���
    // �@�{�X�P�̂ł͂���݂̂ō쓮���܂��B
    //==================================================
    // �����2023/05/26    �X�V��2023/05/28
    // �{��
    [System.Serializable]
    public struct LR_Position
    {
        public bool left, right;
    }

    // �������J�n����
    public bool startAction = true;

    // ���g�̏����擾
    private Rigidbody rb;
    private Animator anim;
    private BossAudio audio;
    private Vector3 scale;

    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    private EnemyEffectManager ef_Manager;

    // ���g�̏ڍאݒ�
    public BossStatusManager status;
    public byte nowPhase = 0;

    [SerializeField] private LayerMask groundLayer = 1 << 6;
    [SerializeField] private float rayDistance = 1.5f;
    RaycastHit underRayHit;

    // ��_���[�W���̐ݒ�
    [SerializeField]
    private bool invincibility = false;     // ���G���

    [SerializeField] private byte atkDelayFrame = 180;
    [SerializeField] private byte stanFrame = 90;
    [SerializeField] private sbyte knockBackSpeed = 30;

    // �q�b�g�X�g�b�v�̐ݒ�
    [SerializeField] private byte hitStopFrame = 30;
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f;

    // �R���[�`���̊i�[
    public IEnumerator cor_Damage;

    // �ړ��ʒu�̐ݒ�
    [SerializeField] private LR_Position[] lr_Pos = new LR_Position[3];
    [SerializeField] private float leftPosition;
    [SerializeField] private float rightPosition;
    private bool moved = false;

    // ����W�����v�̐ݒ�
    [SerializeField] private byte specialChargeFrame = 90;
    [SerializeField] private byte specialJumpSpeed = 26;

    [SerializeField] private GameObject switchObj;
    private VariousSwitches_2 switchScript;

    // ���݂̏��
    public bool nowCharge;
    public bool nowJumpAtk;
    public bool nowPhaseChange;

    public bool nowAtkDelay;
    public bool nowDmgReAct;
    public bool nowStan;
    public bool nowHitStop;

    // �g�����蔻��
    private GameObject waveHit;
    private GameObject waveLog;

    private bool startLog = false;
    private bool startSet = false;

    // �g�̐ݒ�
    [Header("----- �g�̐ݒ� -----")]
    private WavePool pool;
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

    private void OnTriggerEnter(Collider other)
    {
        // ���G���,�_���[�W�A�N�V�������Ȃ珈���𔲂���
        if (invincibility || nowDmgReAct || nowJumpAtk) return;

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
                }//-----if_stop-----
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
        // ���g�̏����擾
        GameObject thisObj = this.gameObject;
        rb = thisObj.GetComponent<Rigidbody>();
        scale = thisObj.transform.localScale;
        anim = GetComponent<Animator>();
        audio = GetComponent<BossAudio>();
        pool = GetComponent<WavePool>();
        pool.AddEtoPDamage();

        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        switchScript = switchObj.GetComponent<VariousSwitches_2>();

        startLog = g_camera.BossStart;

        //// �g�̊e�����擾
        //for (int i = 0; i < waveCount; i++)
        //{
        //    l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
        //    l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
        //    l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        //}//----- for_stop -----

        // �R���[�`�����i�[
        cor_Damage = DamageReaction();

        // �t�F�C�Y���Ƃ̈ړ��ʒu���Đݒ肷��
        PositionSelector();

        status.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        Ray underRay = new Ray(transform.position, Vector3.down);
        bool underRayFg = Physics.Raycast(underRay,out underRayHit,rayDistance,groundLayer);
        // ���g�̗̑͂��O�ɂȂ���
        // �q�b�g�X�g�b�v��Ԃɓ����Ă��Ȃ��Ȃ�
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----


        if (!startSet && startLog != g_camera.BossStart)
        {
            startAction = true;
            startSet = true;
            bgm.BossBattleStart();
        }
        if (startAction)
        {
            StartCoroutine(JumpPhaseChange(nowPhase));
            startAction = false;
        }
    }

    //==================================================
    //          �t�F�C�Y���Ƃ̈ʒu�ύX�̏C��
    // ���ݒ肳�ꂽ�l�ɕs��������ꍇ�Ɏ����Œ������܂�
    //==================================================
    // �����2023/05/28
    // �{��
    private void PositionSelector()
    {
        for(int i = 0; i < lr_Pos.Length; i++)
        {
            // �I��������m�肵�Ă��Ȃ��Ȃ�
            if ((lr_Pos[i].left && lr_Pos[i].right) ||
                (!lr_Pos[i].left && !lr_Pos[i].right))
            {
                // �����_���ō��E��I������
                int random = Random.Range(0, 2);

                // �i�[���ꂽ���l�ɂ���đI����ύX����
                switch (random)
                {
                    case 0:
                        {
                            lr_Pos[i].left = true;
                            lr_Pos[i].right = false;
                        }// ����D�悷��
                        break;
                    case 1:
                        {
                            lr_Pos[i].left = false;
                            lr_Pos[i].right = true;
                        }// �E��D�悷��
                        break;
                }//----- switch_stop -----

                Debug.Log(i + "�̏���" + random + "�ɕύX���܂���");

            }//----- if_stop -----

        }//----- for_stop -----
    }

    //==================================================
    //          �U���܂ł̑ҋ@����
    // ���U�����J�n����܂ł̑ҋ@���Ԃ��t���[�����ԂłƂ�܂�
    //==================================================
    // �����2023/05/28
    // �{��
    private IEnumerator AttackDelayTime()
    {
        Debug.Log("�U���̑ҋ@���Ԃɓ���܂������I");
        nowAtkDelay = true;
        ef_Manager.StopSummon();

        // �U���܂ł̑ҋ@���J�n����
        for (int i = 0; i < atkDelayFrame; i++)
        {
            if (nowPhaseChange) yield break;

            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // ����W�����v�`���[�W�ֈڍs����
        nowAtkDelay = false;
        StartCoroutine(SpecialJumpCharge());
    }

    //==================================================
    //          ����W�����v�̃`���[�W
    // ���v���C���[�̓����Ɏ������W�����v�`���[�W���Ԃł�
    //==================================================
    // �����2023/05/28
    // �{��
    public IEnumerator SpecialJumpCharge()
    {
        // �W�����v�`���[�W���J�n����
        Debug.Log("����`���[�W��Ԃɓ���܂������I");
        nowCharge = true;

        // �`���[�W�A�j���[�V�����J�n ----------
        anim.Play("Charge");

        // �`���[�W���Ԃ̊ԌJ��Ԃ�
        for (int i = 0; i < specialChargeFrame; i++)
        {
            if (nowDmgReAct)
            {
                nowCharge = false;
                yield break;

            }//----- if_stop -----

            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        if (nowDmgReAct)
        {
            nowCharge = false;
            yield break;

        }//----- if_stop -----

        // �W�����v�`���[�W���I������
        nowCharge = false;

        // �`���[�W�A�j���[�V�����I�� ----------

        // ����W�����v�U���ֈڍs����
        StartCoroutine(SpecialJumpAttack());
    }

    //==================================================
    //          ����W�����v�{��
    // ���`���[�W���Ԃ�ʉ߂�����ɌĂяo����
    //   �傫���W�����v���s���U���𔭐������܂�
    //==================================================
    // �����2023/05/28
    // �{��
    public IEnumerator SpecialJumpAttack()
    {
        Debug.Log("����W�����v���J�n���܂������I");
        nowJumpAtk = true;

        // �W�����v�J�n�n�_��ێ�
        Vector3 holdJumpPos = transform.position;

        sbyte fallSpeed_LL = (sbyte)-specialJumpSpeed;
        float speed = specialJumpSpeed;

        float i = (60 / Test_FPS.instance.m_fps);

        // �W�����v�A�j���[�V�������J�n���� ----------
        anim.SetBool("JumpEnding", false);
        anim.SetBool("Jumping",true);
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.Boss5_SuperJumpSound();
        ef_Manager.StartJump();
        // �������x�������������܂ŌJ��Ԃ�
        while (rb.velocity.y >= fallSpeed_LL)
        {
            // ���g�ɑ��x��t�^����
            rb.velocity = new Vector3(rb.velocity.x, speed, 0);
            speed-=i;

            // ���x���O�ȉ��ɂȂ����Ȃ�
            if(rb.velocity.y < 0)
            {
                // �����n�̉���������Ȃ炱���ł�
               // ef_Manager.StopJump();

            }//----- if_stop -----

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        // ���n����
        rb.velocity = Vector3.zero; // ���x���O�ɂ���
        rb.position = holdJumpPos;  // �J�n�n�_�֕␳����
        nowJumpAtk = false;

        // �����n�̉����͂����ŏI�������܂�
        anim.SetBool("Jumping", false);
        anim.SetBool("JumpEnding", true);

        yield return null;
        if (underRayHit.transform != null)
        {
            pool.WaveCreate(waveSpeed*waveAngle,waveWidth,waveHight,waveCollition.WAVE_TYPE.ENEMY
            ,transform.position.x, underRayHit.transform);
        }
        

        // �U���ҋ@���ԂɈڍs����
        StartCoroutine(AttackDelayTime());
    }

    //==================================================
    //          �t�F�C�Y���Ƃ̈ʒu�ύX
    // ���U�����ꂽ��Ɏw�肳�ꂽ�����֑傫�����􂵂Ĉړ����s���܂�
    //==================================================
    // �����2023/05/28
    // �{��
    public IEnumerator JumpPhaseChange(byte _phase)
    {
        Debug.Log("�t�F�C�Y�ύX�����W�����v�����{���܂������I");
        nowPhaseChange = true;
        while (nowJumpAtk) yield return null;

        // �W�����v�J�n�n�_��ێ�
        float holdJumpPosY = transform.position.y;

        sbyte fallSpeed_LL = (sbyte)(-specialJumpSpeed * 2);
        float speed = (specialJumpSpeed * 2);

        float i = (60 / Test_FPS.instance.m_fps);

        // �W�����v�A�j���[�V�������J�n���� ----------
        anim.SetBool("JumpEnding", false);
        anim.Play("Jump");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.Boss5_SuperJumpSound();
        ef_Manager.StartJump();

        // �������x�������������܂ŌJ��Ԃ�
        while (rb.velocity.y >= fallSpeed_LL)
        {
            // ���g�ɑ��x��t�^����
            rb.velocity = new Vector3(rb.velocity.x, speed, 0);
            speed-=i;

            // ���x���O�ȉ��ɂȂ����Ȃ�
            if (rb.velocity.y < 0)
            {
                // �����n�̉���������Ȃ炱���ł�
                if(!moved)
                {
                    if (lr_Pos[_phase].left)
                    {
                        rb.position = new Vector3(leftPosition, rb.position.y, 0);
                        this.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
                        waveAngle = 1;
                    }//----- if_stop -----
                    else if (lr_Pos[_phase].right)
                    {
                        rb.position = new Vector3(rightPosition, rb.position.y, 0);
                        this.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                        waveAngle = -1;
                    }//----- if_stop -----
                    //ef_Manager.StopJump();
                    moved = true;

                }//----- if_stop -----

            }//----- if_stop -----

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        // ���n����
        anim.SetBool("JumpEnding", true);
        rb.velocity = Vector3.zero; // ���x���O�ɂ���
        rb.position = new Vector3(rb.position.x, holdJumpPosY, 0);  // �J�n�n�_�֕␳����
        for (byte j = 0; j < 3; j++)
        {
            yield return null;
        }
        
        if (underRayHit.transform != null)
        {
            vfxManager = underRayHit.transform.GetChild(0).GetComponent<vfxManager>();
            //WaveCreate(transform.position.x, underRayHit.transform.position.y);
        }
        
        // �����n�̉����͂����ŏI�������܂�
        moved = false;

        // �G���A�ړ��̏I��
        nowJumpAtk = false;
        nowPhaseChange = false;
        invincibility = false;

        // �t�F�C�Y��ύX
        nowPhase++;

        if (status.hitPoint == 1)
        {
            switchScript.switchStatus = false;
        }


        // �G���G�����A�j���[�V����
        anim.Play("Command");
        ef_Manager.StartSummon();

        // �U���ҋ@���ԂɈڍs����
        StartCoroutine(AttackDelayTime());
    }

    //==================================================
    //          ��_���[�W���̃��A�N�V����
    // ���v���C���[����̐U�����󂯂����ɑ傫�����ˏオ��܂�
    // �����ˏオ������͂��΂炭�̊ԃX�^�����܂�
    //==================================================
    // �����2023/05/28
    // �{��
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
            // ���g�ɑ��x������
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            knockBackDistHold--;
            if(rb.velocity.y < 0)
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
        anim.SetBool("Falling",false);
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
        audio.bossSource.loop = false;

        nowStan = false;

        // �_���[�W���A�N�V������Ԃ�����
        nowDmgReAct = false;

        // �G���A�ړ��W�����v�ֈڍs����
        StartCoroutine(JumpPhaseChange(nowPhase));
    }

    //==================================================
    //          �������̃��A�N�V����
    // ���ŏI�U�����s�������ɑ傫�����ˏオ��A
    // �@�����̊Ԏ��Ԃ��x���Ȃ��ĉ�ʊO�܂ŗ������Ă����܂�
    //==================================================
    // �����2023/05/28
    // �{��
    private IEnumerator KnockOut()
    {
        // �{�X��|�����Ƃ��̏��������s���܂�
        Debug.Log(this.name + "��|���܂������I");

        // �_���[�W���A�N�V������Ԃɂ���
        nowDmgReAct = true;
        anim.Play("Damage");

        // ���A�N�V�����J�n�n�_��ێ�
        Vector3 holdReactPos = transform.position;
        sbyte knockOut_LL = (sbyte)-knockBackSpeed;

        // �������x�������𒴂�����Ƃ��΂炭�̊ԌJ��Ԃ�
        while (rb.velocity.y > knockOut_LL)
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
            if (rb.velocity.y < 0)
            {
                anim.SetBool("Falling", true);
            }

            // ���g�ɑ��x������
            rb.velocity = new Vector3(0, knockBackSpeed, 0);
            knockBackSpeed--;

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        // ���n����
        rb.velocity = Vector3.zero;     // ���x���O�ɂ���
        rb.position = holdReactPos;     // �J�n�n�_�ŌŒ肷��

        for(byte i= 0; i < 45; i++)
        {
            yield return null;
        }
        anim.SetBool("WakeUping", true);
        for(byte i= 0;i<50;i++)
        {
            yield return null;
        }
        // �����ŃG���f�B���O�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I�I

        g_camera.BossEnd = true;



        //// ���g���\���ɂ���
        //Debug.Log(this.gameObject.name + "���\���ɂ��܂������I");
        //this.gameObject.SetActive(false);
    }
    //public void WaveCreate(float startPosX, float startPosY)
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
