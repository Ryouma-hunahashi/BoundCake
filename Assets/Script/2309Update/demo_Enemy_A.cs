using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class demo_Enemy_A : MonoBehaviour
{
    public enum DIRECTION
    {
        LEFT,
        RIGHT,
    }

    private struct ENEMY_OBJECT
    {
        // �I�u�W�F�N�g�ɂ��Ă�����
        private Rigidbody rb;
        private Animator anim;
        private EnemyAudio audio;
        private Enemy_KB knockback;

        // �Q�b�g�Z�b�^�[
        public Rigidbody GetRigidbody() { return rb; }
        public Animator GetAnimator() { return anim; }
        public EnemyAudio GetAudio() { return audio; }
        public Enemy_KB GetKB() { return knockback; }
        public void SetRidbody(Rigidbody _rigidbody) { rb = _rigidbody; }
        public void SetAnimator(Animator _animator) { anim = _animator; }
        public void SetAudio(EnemyAudio _audio) { audio = _audio; }
        public void SetKB(Enemy_KB _kbScript) { knockback = _kbScript; }
    }

    Enemy_Detect c_detect;

    [SerializeField] private bool summoned;
    public bool inCamera;

    // ���������擾
    public GameObject boss5;
    private demo_B5 boss5_cs;

    // �����J�n����X���W���i�[
    private float startPosition;

    // ���g�̍s�����J�n����
    [SerializeField] private bool start;

    // ���g�̌����Ă������
    private DIRECTION direction;
    public DIRECTION GetDirection() { return direction; }
    public void SetDirection(DIRECTION _dir) { direction = _dir; }

    // ���g�ɂ��Ă���f�[�^
    [Header("���厲�f�[�^")]
    [SerializeField] private ENEMY_OBJECT objData;
    [SerializeField] private float EnemyScale;
    [SerializeField] private float detectionRange;  //���m�͈�

    // �e����
    private bool isSummoned;    // �������ꂽ
    private bool isCharge;      // ���ߒ�
    private bool isTackle;      // �ːi����


    public bool GetChargeState() { return isCharge; }
    public bool GetTackleState() { return isTackle; }

    [Header("���ːi�̐ݒ�")]
    [SerializeField] private float chargeTime;  // ���ߎ���
    [SerializeField] private float tackleTime;  // �ːi����
    [SerializeField] private float speedMag;    // ���x�{��

    [Header("���ړ��̐ݒ�")]
    [SerializeField] private float speed;       // �ړ����x

    [Header("�������̐ݒ�")]
    [SerializeField] private float distMin; // �ŏ��ʒu
    [SerializeField] private float distMax; // �ő�ʒu

    public void SetDistanceMin(float _min) { distMin = _min; }
    public void SetDistanceMax(float _max) { distMax = _max; }

    [Header("���ǏՓ˂̐ݒ�")]
    [SerializeField] private float rayDist;                 // ���C�̋���
    [SerializeField] private LayerMask gLayer = 1 << 6;     // ���C���[�̐ݒ�

    [Header("��������ю��̐ݒ�")]
    [SerializeField] private Vector2 knockbackPower;    // ������ё��x
    [SerializeField] private float stanTime;            // �Փ˃X�^���̐ݒ�

    private bool isKnockback;

    private void Awake()
    {
        // ���g�̏����擾
        objData.SetRidbody(this.GetComponent<Rigidbody>());    // ���W�b�h�{�f�B
        objData.SetAnimator(this.GetComponent<Animator>());    // �A�j���[�^�[
        objData.SetAudio(this.GetComponent<EnemyAudio>());     // �{�X�̃I�[�f�B�I

        c_detect = GetComponent<Enemy_Detect>();

        // ���������G�Ȃ�ȉ����������Ȃ�
        if (!summoned) return;

        // �{�X�̃X�N���v�g���擾
        boss5 = GameObject.Find("boss5").gameObject;
        boss5_cs = boss5.GetComponent<demo_B5>();
    }

    private void Start()
    {
        startPosition = this.transform.position.x;

        transform.localScale = new Vector3(EnemyScale, EnemyScale, EnemyScale);
    }

    private void FixedUpdate()
    {
        Vector3 pos = this.transform.position;
        bool wallHit = HorizontalRayHitWall(pos);

        // �ːi���ĕǂɓ��������Ƃ��A�m�b�N�o�b�N���łȂ���΃X�^��������
        if(wallHit && isTackle && !isKnockback) CollidedWall();

        // �������ꂽ���u���J�����̊O�ɏo���Ȃ�
        if (!inCamera && summoned)
        {
            // ������ԂɈڍs����
            boss5_cs.SetState(Enemy_Boss.STATE.ATK01);

            // ���݂�����
            Destroy(this.gameObject);
        }

        // ���u�����̂ق��ɍs�����Ƃ�
        if(pos.y < -20)
        {
            // �����������u�������Ȃ�{�X�������U����Ԃɂ���
            if(summoned) boss5_cs.SetState(Enemy_Boss.STATE.ATK01);

            // ���݂�����
            Destroy(this.gameObject);
        }

        // �J�n���Ă��Ȃ��Ȃ珈���𔲂���
        if (!start) return;

        // ���߂�A�ːi���Ă���ԂȂ珈���𔲂���
        if (isCharge || isTackle || isKnockback) return;

        // �����Ă�������ɐi�ފ֐����Ăяo��
        MoveObject(speed);

        // �ʂ�߂��������΂���������֐�
        PassedTheDestination(pos.x);

        // ���G�ł����Ȃ�
        if(c_detect.GetDetect())
        {
            // �ːi�U�����J�n����
            StartCoroutine(TackleAction());

            // ���G�ł��Ă��Ȃ���Ԃɂ���
            //isDetect = false;
        }
    }

    private void CollidedWall()
    {
        isKnockback = true;
        isTackle = false;

        // ���ݎg�p���Ă��邷�ׂẴR���[�`�����~�߂�
        StopAllCoroutines();

        // �m�b�N�o�b�N���J�n����
        StartCoroutine(WallHitKnockBack(direction));

        return;
    }

    private bool HorizontalRayHitWall(Vector3 _pos)
    {
        Ray ray = new Ray();
        RaycastHit hit;

        switch(direction)
        {
            case DIRECTION.LEFT:
                ray = new Ray(new Vector3(_pos.x, _pos.y, _pos.z), new Vector3(-1.0f, 0.0f, 0.0f));
                break;
            case DIRECTION.RIGHT:
                ray = new Ray(new Vector3(_pos.x, _pos.y, _pos.z), new Vector3(1.0f, 0.0f, 0.0f));
                break;
        }

        return Physics.Raycast(ray, out hit, rayDist, gLayer);
    }

    private IEnumerator WallHitKnockBack(DIRECTION _dir)
    {
        // ����я����Ɏg�p����e�ϐ��̍쐬
        double limFallSpd = -knockbackPower.y;  // �������x�Œ�l
        double kbPowX = knockbackPower.x;       // ���̐���ё��x
        double kbPowY = knockbackPower.y;       // ���̐���ё��x

        // �A�j���[�V�������~����
        objData.GetAnimator().SetBool("running", false);

        // �������x���Œ�l�𒴂���܂ŌJ��Ԃ�
        while (objData.GetRigidbody().velocity.y > limFallSpd)
        {
            switch (_dir)
            {
                case DIRECTION.LEFT:
                    {
                        objData.GetRigidbody().velocity = new Vector3((float)kbPowX, (float)kbPowY, 0);
                    }
                    break;
                case DIRECTION.RIGHT:
                    {
                        objData.GetRigidbody().velocity = new Vector3((float)-kbPowX, (float)kbPowY, 0);
                    }
                    break;
            }

            kbPowY -= 1;

            yield return null;
        }

        // �X�^���ҋ@���Ԃ����{
        yield return new WaitForSeconds(stanTime);

        // ���g�̑��x����u�O�ɂ���
        objData.GetRigidbody().velocity = Vector3.zero;

        //�m�b�N�o�b�N�̏I��
        isKnockback = false;
    }

    // �ʏ�ړ��A�j���[�V�����֐�
    private void MoveAnimation()
    {
        switch(direction)
        {
            case DIRECTION.LEFT:
                transform.localScale = new Vector3(-1 * EnemyScale, EnemyScale, 1);
                break;
            case DIRECTION.RIGHT:
                transform.localScale = new Vector3(1 * EnemyScale, EnemyScale, 1);
                break;
        }

        objData.GetAnimator().SetBool("running", true);
        if(!objData.GetAudio().enemySource.isPlaying)
        {
            objData.GetAudio().WalkSound();
        }

    }

    // �`���[�W����
    private IEnumerator ChargeTime()
    {
        Debug.Log("�ːi�`���[�W���J�n���܂�");

        // �`���[�W��Ԃɂ���
        isCharge = true;

        objData.GetAnimator().SetBool("running", false);

        // �ړ����x���[���ɂ���
        objData.GetRigidbody().velocity = Vector3.zero;

        yield return new WaitForSeconds(chargeTime);

        // �`���[�W��Ԃ���������
        isCharge = false;
    }

    // �ːi�J�E���g����
    private IEnumerator TackleTime()
    {
        // �ːi��ԂɈڍs
        isTackle = true;

        yield return new WaitForSeconds(tackleTime);

        // �ːi�I��
        isTackle = false;
    }

    // �ːi����
    private IEnumerator TackleAction()
    {
        // �`���[�W��҂�
        yield return StartCoroutine(ChargeTime());

        Debug.Log("�ːi���J�n���܂�");

        StartCoroutine(TackleTime());

        while(isTackle)
        {
            MoveObject(speed * speedMag);

            yield return null;
        }
        
    }


    // ���x�����ē������֐�
    private void MoveObject(float _spd)
    {
        switch (direction)
        {
            case DIRECTION.LEFT:
                objData.GetRigidbody().velocity = new Vector3(-_spd, objData.GetRigidbody().velocity.y, 0);
                break;

            case DIRECTION.RIGHT:
                objData.GetRigidbody().velocity = new Vector3(_spd, objData.GetRigidbody().velocity.y, 0);
                break;
        }

        MoveAnimation();

        return;
    }

    // ���W��ʂ�߂������ɔ��΂���������֐�
    private void PassedTheDestination(float _pos)
    {
        float min = startPosition - distMin;  // X�ŏ��l�̍��W
        float max = startPosition + distMax;  // X�ő�l�̍��W

        // ���g�̌����Ă������
        switch (direction)
        {
            case DIRECTION.LEFT:
                {
                    // �ʂ�߂����������𔽑΂ɂ���
                    if (_pos < min) ChangeDirection();
                }
                break;
            case DIRECTION.RIGHT:
                {
                    // �ʂ�߂����������𔽑΂ɂ���
                    if (_pos > max) ChangeDirection();
                }
                break;
        }
    }

    // �����Ă�������𔽑΂ɂ���֐�
    private void ChangeDirection()
    {
        switch (direction)
        {
            case DIRECTION.LEFT:
                direction = DIRECTION.RIGHT;
                break;

            case DIRECTION.RIGHT:
                direction = DIRECTION.LEFT;
                break;
        }

        return;
    }
}
