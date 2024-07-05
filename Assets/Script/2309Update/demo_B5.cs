using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

// �{�X�N���X���p������
[CustomEditor(typeof(Enemy_Boss))]

#endif

public class demo_B5 : Enemy_Boss
{
    // �ړ��ʒu�̗񋓌^
    private enum POSITION
    {
        MID,    // ����
        LEFT,   //  ��
        RIGHT,  //  �E
    }

    [System.Serializable]
    private struct DIRECTION_DATA
    {
        [SerializeField] private Vector2 pos;
        [SerializeField] private float min, max;

        public Vector2 GetPosition() { return pos; }
        public float GetDistanceMin() { return min; }
        public float GetDistanceMax() { return max; }
    }

    [System.Serializable]
    private struct POSITION_DATA
    {
        // �ړ��n�_��[ X ]���W�̒l
        [SerializeField] private float mid, left, right;

        // �Q�b�g�Z�b�^�[
        public float GetMidPos() { return mid; }
        public float GetLeftPos() { return left; }
        public float GetRightPos() { return right; }
    }

    [System.Serializable]
    private struct SUMMON_DATA
    {
        [SerializeField] private GameObject mob;        // �I�u�W�F�N�g
        [SerializeField] private DIRECTION_DATA left, right;

        // �Q�b�g�Z�b�^�[
        public GameObject GetMobObject() { return mob; }
        public DIRECTION_DATA GetLeftData() { return left; }
        public DIRECTION_DATA GetRightData() { return right; }
    }

    private Vector3 scale;

    // �s���̊J�n�p�̃u�[���A���^
    public bool start;
    private bool inMoving;
    private bool inAction;

    [SerializeField] private float stanTime;        // ���������܂ł̎���

    // ���g�̈ړ��ʒu�̐ݒ�
    [Header("���ړ��ʒu�̐ݒ�")]
    [SerializeField] private POSITION[] standPoint = new POSITION[3];   // �~���ʒu�̐ݒ�
    [SerializeField] private POSITION_DATA posData;                     // �ړ��n�_�̍��W�ݒ�

    [Header("������֌W�̐ݒ�")]
    [SerializeField] private float chargeTime;  // ���߂鎞��
    [SerializeField] private double jumpPower;  // �f�̒�����
    [SerializeField] private float jumpPowMag;  // �����͔{��
    [SerializeField] private float endFall;     // �ǂ��܂ŗ����邩

    [Header("�������֌W�̐ݒ�")]
    [SerializeField] private SUMMON_DATA summon;
    [SerializeField] private float summonTime;      // �����܂ł̎���

    private GameObject summonedMob;     // ���������I�u�W�F�N�g

    [Header("���{�X���o�֘A�̃X�N���v�g��")]
    [SerializeField] private Bossdirection bossCamera;

    private void Start()
    {
        scale = this.transform.localScale;
    }

    private void Update()
    {
        Vector3 plPos = GetTarget().transform.position;
        Vector3 myPos = this.transform.position;

        if(plPos.x < myPos.x)
        {
            this.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        }
    }

    private void FixedUpdate()
    {
        // �s�����J�n���Ȃ���Ԃ̂Ƃ���ړ����͏����𔲂���
        if (!start || inMoving) return;

        if (Input.GetKeyDown(KeyCode.W)) SetState(STATE.DAMAGE);

        // �_���[�W���󂯂��Ȃ��p�̍s���Ɉڍs����
        if (GetState() == STATE.DAMAGE) DamageReaction();

        // �s�����͈ȉ��̏��������s���Ȃ�
        if (inAction) return;

        // ��Ԃɍ��킹�Ċ֐������s����
        FlexibleResponse();
    }

    private void FlexibleResponse()
    {
        // �ҋ@���Ȃ�s�����Ȃ�
        if (GetState() == STATE.IDLE) return;

        // �s�����J�n����
        inAction = true;

        switch (GetState())
        {
            // �o�����̏���
            case STATE.WARNING:
                {
                    // �J�n���̏����p�̊֐����J�n����
                    StartCoroutine(Warning());
                }
                break;
            // ���u�����U������
            case STATE.ATK01:
                {
                    // ���u�����p�̊֐����J�n����
                    StartCoroutine(SummonEnemies());
                }
                break;
        }
    }

    private void DamageReaction()
    {
        Debug.Log("* Start damage reaction.");

        // �s�����J�n����
        inAction = true;
        inMoving = true;

        // �g�p���Ă���s�������ׂĎ~�߂�
        StopAllCoroutines();

        // �t�F�C�Y�ω������{
        StartCoroutine(PhaseChange());
    }

    private IEnumerator PhaseChange()
    {
        // �t�F�C�Y��ω�������
        NextPhase();

        // ���݂̃t�F�C�Y���R�����Ȃ�
        if(GetPhase() < 3)
        {
            // �X�^����ԂɈڍs
            GetAnimator().SetBool("Staning", true);
            GetAudio().bossSource.Stop();
            GetAudio().bossSource.loop = true;
            GetAudio().BossStunSound();

            // �X�^���p�ҋ@����
            yield return StartCoroutine(IdleTime(stanTime));

            // �X�^���I��
            GetAnimator().SetBool("Staning", false);
            GetAudio().bossSource.Stop();
            GetAudio().bossSource.loop = false;

            // �t�F�C�Y�ω��p�̃W�����v���J�n
            yield return StartCoroutine(Jump());

            // �����U�����J�n����
            SetState(STATE.ATK01);

            // �s�����I������
            inAction = false;
        }
        else
        {
            // �|�����Ƃ��p�̃W�����v���J�n
            yield return StartCoroutine(Jump(true));

            // �ҋ@��Ԃɂ���
            SetState(STATE.IDLE);

            // ��\���ɂ���
            this.gameObject.SetActive(false);
        }

    }

    private IEnumerator Warning()
    {

        // �W�����v�������I������܂ő҂�
        yield return StartCoroutine(Jump());

        // �ҋ@��Ԃ��I������܂ő҂�
        yield return StartCoroutine(IdleTime());

        // �U����ԂɈڍs
        SetState(STATE.ATK01);

        // �s�����I������
        inAction = false;
    }

    private IEnumerator SummonEnemies()
    {
        yield return StartCoroutine(Summon());

        // �ҋ@��ԂɈڍs
        SetState(STATE.IDLE);

        // �s�����I������
        inAction = false;
    }

    private IEnumerator Jump(bool _KO = false)
    {
        Debug.Log("* Use jump function.");

        // �ړ����J�n����
        inMoving = true;

        // FPS�ɂ��W�����v�̂�����y��
        // ���F( �w��FPS�l ) / ( ���݂�FPS�l )
        double fixFPS = ((60) / Test_FPS.instance.m_fps);

        // �W�����v�Ɏg�p����e�ϐ��̍쐬
        float startJumpPos = this.transform.position.y;     // ����J�n�n�_�̍��W���擾
        double limFallSpeed = (-jumpPower * jumpPowMag);    // �������x�̐���
        double spJumpPower = (jumpPower * jumpPowMag);      // ���f�������x

        // �W�����v�p�A�j���[�V�������J�n����
        GetAnimator().SetBool("JumpEnding",false);
        GetAnimator().Play("Jump");

        // �W�����v�p�I�[�f�B�I�̍Đ�
        GetAudio().bossSource.Stop();
        GetAudio().bossSource.loop = false;
        GetAudio().Boss5_SuperJumpSound();

        // �W�����v�p�̃G�t�F�N�g���J�n����
        GetEffectManager().StartJump();

        // ���􏈗��{�� ------------------------------*
        // �������x�������������܂ŌJ��Ԃ�
        while(GetRigidbody().velocity.y >= limFallSpeed)
        {
            // ���g�ɑ��x��t�^����
            GetRigidbody().velocity = new Vector3(GetRigidbody().velocity.x, (float)spJumpPower, 0);

            // ���x�̒l��FPS�ɍ��킹�Č��������Ă���
            spJumpPower -= fixFPS;

            // �������x���O�ȉ��ɂȂ����Ȃ�
            if(GetRigidbody().velocity.y < 0)
            {
                // �m�b�N�A�E�g���Ă��Ȃ��Ȃ�
                if (!_KO)
                {
                    // �ǂ��ɒ��n����̂��𔻒肷��
                    switch (standPoint[GetPhase()])
                    {
                        // �����ɒ��n����Ƃ��̏���
                        case POSITION.MID:
                            {
                                // �~���ʒu���w�肵�Ĉړ�����
                                GetRigidbody().position = new Vector3(posData.GetMidPos(), this.transform.position.y, 0);
                            }
                            break;
                        // �����ɒ��n����Ƃ��̏���
                        case POSITION.LEFT:
                            {
                                // �~���ʒu���w�肵�Ĉړ�����
                                GetRigidbody().position = new Vector3(posData.GetLeftPos(), this.transform.position.y, 0);
                            }
                            break;
                        // �E���ɒ��n����Ƃ��̏���
                        case POSITION.RIGHT:
                            {
                                // �~���ʒu���w�肵�Ĉړ�����
                                GetRigidbody().position = new Vector3(posData.GetRightPos(), this.transform.position.y, 0);
                            }
                            break;
                    }
                }
                
            }//----- if_stop -----

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        // �m�b�N�A�E�g�����̂Ƃ�
        if(_KO)
        {
            // ���g���ŏI�����n�_����ɑ��݂��Ă���Ȃ�
            while(this.transform.position.y > (startJumpPos - endFall))
            {
                // ���g�ɑ��x��t�^����
                GetRigidbody().velocity = new Vector3(GetRigidbody().velocity.x, (float)spJumpPower, 0);

                // �P�t���[���x��������
                yield return null;

            }//----- while_stop -----

            bossCamera.bossrightcheck = true;

        }//----- if_stop -----

        // ���n�����{�� ------------------------------*
        // ���g�̑��x�𖳂���
        GetRigidbody().velocity = Vector3.zero;

        if(_KO)
        {
            // �ʂ�߂����ԂŎ��g�̈ʒu�𒲐�����
            GetRigidbody().position = new Vector3(GetRigidbody().position.x, startJumpPos - endFall, 0);
        }
        else
        {
            // �ʂ�߂��Ȃ��悤�Ɏ��g�̈ʒu�𒲐�����
            GetRigidbody().position = new Vector3(GetRigidbody().position.x, startJumpPos, 0);
        }

        // ���n�p�A�j���[�V�������J�n����
        GetAnimator().SetBool("JumpEnding",true);

        // �ړ����I������
        inMoving = false;

        Debug.Log("* Used jump function.");
    }

    private IEnumerator Summon()
    {
        Debug.Log("* Use summon function.");

        // �������u�̏����擾
        demo_Enemy_A demoEnemy;

        // �����A�j���[�V����
        GetAnimator().Play("Command");
        //GetEffectManager().StartSummon();

        // �����܂ł̑ҋ@����
        yield return StartCoroutine(IdleTime(summonTime));

        // ��������ʒu�����g�̈ʒu����w�肷��
        switch(standPoint[GetPhase()])
        {
            // ���g�̈ʒu�������ɂȂ��Ă���Ȃ�
            case POSITION.MID:
                {
                    // �����̎擾
                    int random = Random.Range(0, 2);

                    if (random == 0)
                    {
                        summonedMob = Instantiate(summon.GetMobObject(), summon.GetLeftData().GetPosition(), Quaternion.identity);

                        // �������������X�^�[�̃X�N���v�g���擾
                        demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                        demoEnemy.SetDistanceMin(summon.GetLeftData().GetDistanceMin());
                        demoEnemy.SetDistanceMax(summon.GetLeftData().GetDistanceMax());
                        demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.RIGHT);
                    }
                    else if (random == 1)
                    {
                        summonedMob = Instantiate(summon.GetMobObject(), summon.GetRightData().GetPosition(), Quaternion.identity);

                        // �������������X�^�[�̃X�N���v�g���擾
                        demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                        demoEnemy.SetDistanceMin(summon.GetRightData().GetDistanceMin());
                        demoEnemy.SetDistanceMax(summon.GetRightData().GetDistanceMax());
                        demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.LEFT);
                    }
                }
                break;
            // ���g�̈ʒu�����ɂȂ��Ă���Ȃ�
            case POSITION.LEFT:
                {
                    // ���u�̏������s��
                    summonedMob = Instantiate(summon.GetMobObject(), summon.GetRightData().GetPosition(), Quaternion.identity);

                    

                    // �������������X�^�[�̃X�N���v�g���擾
                    demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                    demoEnemy.SetDistanceMin(summon.GetRightData().GetDistanceMin());
                    demoEnemy.SetDistanceMax(summon.GetRightData().GetDistanceMax());
                    demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.LEFT);
                }
                break;
            // ���g�̈ʒu���E�ɂȂ��Ă���Ȃ�
            case POSITION.RIGHT:
                {
                    // ���u�̏������s��
                    summonedMob = Instantiate(summon.GetMobObject(), summon.GetLeftData().GetPosition(), Quaternion.identity);

                    

                    // �������������X�^�[�̃X�N���v�g���擾
                    demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                    demoEnemy.SetDistanceMin(summon.GetLeftData().GetDistanceMin());
                    demoEnemy.SetDistanceMax(summon.GetLeftData().GetDistanceMax());
                    demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.RIGHT);
                }
                break;
        }

        //GetEffectManager().StopSummon();

        Debug.Log("* Used summon function.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Wave"))
        {
            var col = other.gameObject.GetComponent<waveCollition>();
            if(col.CheckType(waveCollition.WAVE_TYPE.PLAYER_ENEMY))
            {
                SetState(STATE.DAMAGE);
            }

        }
    }
}
