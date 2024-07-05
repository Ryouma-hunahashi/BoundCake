using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//              �o�O�� - ���u��
// ������x�ł��j�󂪔j�󂳂ꂽ���肪����Ə��U�艺�낵�̍ۂ�
// �ϋv�l�̏�Ԋ֌W�Ȃ��t�F�C�Y���؂�ւ���ĉ񕜂��Ă��܂��܂�
//==================================================
// �����2023/04/10    �X�V��2023/04/11
// �{��
public class Enemy_D_Shield : MonoBehaviour
{
    [SerializeField]
    private GameObject myParent;    // �����̐e���擾
    private Animator parentAnim;    // �e�̃A�j���[�^�[

    private IEnumerator guardWaitTime;

    private Vector3 fixPosition;

    // ��x�̂ݏ���
    public bool startup = false;

    public bool notRepiar;          // �C�������Ȃ��ݒ�

    // ���\���̏��
    [Header("���̍\�����")]
    [SerializeField] private ushort rotateTime;    // ��]�ҋ@����
    public bool rotateNow;      // ��]���̏��
    public bool rotateEnd;      // ��]��̏��

    [Tooltip("����U�艺�낷�܂ł̍d������")]
    [SerializeField] private ushort impactWaitTime;    // �h������܂ł̎���
    public bool shieldImpactNow;        // ���\����
    public bool shieldImpactEnd;        // ���\���I��

    [Header("���̖h����")]
    [Tooltip("�����\���Ă���Ԃ̍d������")]
    [SerializeField] private ushort guardTime;         // �h������܂ł̎���
    public bool guardNow;           // �h�䒆�̏��
    public bool guardEnd;           // �h��I���̏��

    [Header("���̒i�K�ݒ�")]
    [Tooltip("���݂̃t�F�C�Y���")]
    public byte phaseNow;                        // ���݂̃t�F�C�Y
    public byte phaseLog;                        // �ύX�O�̃t�F�C�Y
    public bool lastPhase;
    [Tooltip("�t�F�C�Y���ϋv�l")]
    public byte[] shieldPhase = { 1, 2, 3 };     // ���̊e�ϋv�l

    [Header("���̑ϋv�ݒ�")]
    [Tooltip("���݂̑ϋv�l")]
    public byte shieldHitPoint;      // ���̑ϋv�l
    [Tooltip("���̔j����")]
    public bool shieldBreak;        // ���̔j�����

    [Header("���̏C�����")]
    [Tooltip("�����j�󂳂ꂽ��̍d������")]
    [SerializeField] private int shieldRepairTime;  // ���C���܂ł̎���
    public bool shieldRepairNow;        // ���C�����̏��
    public bool shieldRepairEnd;        // ���C�������̏��

    [Tooltip("�ϋv�l������}����")]
    public bool stopDamage;                     // �ϋv�l������}����
    [Tooltip("�j��s�\�ȏ��")]
    [SerializeField] private bool unbreakable;  // �j��s�\���

    [Header("���̐ݒ�")]
    [Tooltip("���̓����蔻��")]
    public CapsuleCollider shield;          // ���̓����蔻��
    [Tooltip("���̗������x")]
    public float shieldSpeed;               // ���U�艺�낵�U�����x

    public Vector3 shieldPositionStart;     // ���U�艺�낵�J�n�n�_
    public Vector3 shieldPositionEnd;       // ���U�艺�낵�I���n�_

    public vfxManager vfxManager;          // ������p���鎅��vfxManager
    private sbyte arrayNum = 0;//�g�𐶐�����Ƃ��ɖ߂�l���󂯎��N���[���ɓn��
    [Header("�g�̐ݒ�")]
    [SerializeField] private float waveSpeed = 7.5f;        // �g�̑��x
    [SerializeField] private float waveWidth = 0.225f;      // �g�̐U����
    [SerializeField] private float waveHight = 2.0f;        // �g�̍���
    [SerializeField] GameObject waveObj;                    // �g�̃R���W�����v���t�@�u
    public int waveCount = 3;             // �v�[���ɐ�������g�̐�
    private int waveAngle = 1;                              // �g�̕���
    // �g�R���W�����̃I�u�W�F�N�g�v�[��
    [System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();
    // �g�R���W�����ɃR���|�[�l���g����Ă��� waveCollition �̃v�[���B
    // �Y�����͔g�R���W�����̃v�[���ɑΉ��B
    private List<waveCollition> l_waveCollitions = new List<waveCollition>();
    //==================================================
    //          ����U�艺�낷�܂ł̍d������
    // ���U�艺�낵�J�n�܂ł̎��Ԃ��d�����ԂƂ��ċ󂯂Ă����܂�
    //==================================================
    // �����2023/04/11
    // �{��
    public IEnumerator RotateWaitTime()
    {
#if UNITY_EDITOR
        Debug.Log("��]���J�n���܂������I");
#endif
        // ��]��Ԃɂ���
        rotateNow = true;
        rotateEnd = false;

        // ��]�����������܂ł̎���
        for (int i = 0; i < rotateTime; i++)
        {
            yield return null;

        }//----- for_stop -----

        // ��]��Ԃ���������
        rotateNow = false;
        rotateEnd = true;
    }


    //==================================================
    //          �h�䒆�̍d������
    // ���U�艺�낵�������U�������Ɏ󂯂܂�
    //==================================================
    // �����2023/04/10    �X�V��2023/04/11
    // �{��
    public IEnumerator WaveGuardTime()
    {
#if UNITY_EDITOR
        Debug.Log("�h����J�n���܂������I");
#endif
        // �h���Ԃɂ���
        guardNow = true;
        guardEnd = false;

        // �h�䂪���������܂ł̎���
        for (int i = 0; i < guardTime; i++)
        {
            yield return null;

            // �����j�󂳂ꂽ�Ȃ�
            if (shieldBreak)
            {
#if UNITY_EDITOR
                Debug.Log("�����j�󂳂�܂������I");
#endif
                // ���̓����蔻���j������
                shield.enabled = false;

                // ���͏C���p�̏����Ɉڍs����
                StartCoroutine(ShieldBreakTime());

                // �����𔲂���
                yield break;

            }//----- if_stop -----

        }//----- for_stop -----

        // �h���Ԃ���������
        guardNow = false;
        guardEnd = true;
    }

    //==================================================
    //          �����j�󂳂ꂽ��̍d������
    // ���U���ɂ���ď����j�󂳂ꂽ�Ƃ��ɏ��̋@�\���ꎞ�I�Ɏ~�߂�
    //==================================================
    // �����2023/04/10    �X�V��2023/04/11
    // �{��
    public IEnumerator ShieldBreakTime()
    {
#if UNITY_EDITOR
        Debug.Log("�����C�����Ă��܂����I");
#endif

        // �h���Ԃ���������
        guardNow = false;
        guardEnd = true;

        // �����j��ł��Ȃ��悤�ɂ���
        unbreakable = true;

        // �C�����̏�Ԃɂ���
        shieldRepairNow = true;
        shieldRepairEnd = false;

        // �����C������܂ł̎���
        for (int i = 0; i < shieldRepairTime; i++)
        {
            yield return null;

        }//----- for_stop -----

        // �C���s�ł͂Ȃ��ꍇ
        if (!notRepiar)
        {
            // �C������������
            shieldRepairNow = false;
            shieldRepairEnd = true;

            // ����j��ł����Ԃɂ���
            shieldBreak = false;
            unbreakable = false;

            // ���̓����蔻���t�^����
            shield.enabled = true;
        }

        // �X�^�[�g�A�b�v���J�n����
        startup = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // [Wave]�^�O�����Ă���I�u�W�F�N�g�ɐG�ꂽ�� &
        // �_���[�W���^�������ԂȂ�
        if (other.CompareTag("Wave") && !stopDamage && !unbreakable && guardNow)
        {
            for (int i = 0; i < waveCount; i++)
            {
                if (other.gameObject == l_waveCollisionObj[i])
                {
                    return;
                }
            }
            // ���̑ϋv�l������������
            shieldHitPoint--;

            // �_���[�W��^�����Ȃ���Ԃɂ���
            stopDamage = true;

        }//----- if_stop -----
        if ((1 << other.gameObject.layer) == 1 << 6)
        {
            if (other.transform.childCount == 1)
            {
                vfxManager = other.transform.GetChild(0).GetComponent<vfxManager>();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // [Wave]�^�O�����Ă���I�u�W�F�N�g�����ꂽ��
        if (other.CompareTag("Wave"))
        {
            // �_���[�W��^�������Ԃɂ���
            stopDamage = false;

        }//----- if_stop -----
    }

    //==================================================
    //          ���U�艺�낵�U��
    // ���n�ʂɌ������ď���U�艺�낵�ĐU�����N�����U������
    //==================================================
    // �����2023/04/10    �X�V��2023/04/11
    // �{��
    public void ShieldImpact()
    {
        // �������X�^�[�g�����Ƃ��Ɉ�x�������s����
        if (!startup)
        {
            // �����\���n�߂�
            shieldImpactNow = true;
            shieldImpactEnd = false;

            if (notRepiar)
            {
                shield.enabled = true;
            }

            // ���̒��n�n�_�������ݒu�ʒu�Őݒ肷��
            shieldPositionEnd = shield.center;

            // ���U�艺�낵�J�n
            shield.center = new Vector3(shield.center.x, shieldPositionStart.y, shield.center.z);
            parentAnim.SetBool("Impact", true);
            // ��x�݂̂̎��s�I��
            startup = true;

        }//----- if_stop -----

        // ���̐U�艺�낵�n�_�ɂ܂����B���Ă��Ȃ��Ȃ�
        if (shieldPositionEnd.y < shield.center.y)
        {
            // �U�艺�낵�I���n�_�܂œ�����
            shield.center = new Vector3(shield.center.x, shield.center.y - shieldSpeed, shield.center.z);

        }//----- if_stop -----

        // ���̒��n�n�_�𒴂����Ȃ�
        else if (shieldPositionEnd.y > shield.center.y)
        {
#if UNITY_EDITOR
            Debug.Log("���̈ʒu���C�����܂������I");
#endif
            // ���n�_���C������
            shield.center = shieldPositionEnd;

            parentAnim.SetBool("Impact", false);

            // �����j��ł���悤�ɂ���
            unbreakable = false;

            // �����ŏ��̍U�����I��
            shieldImpactNow = false;
            shieldImpactEnd = true;

            // ���̊֐������������Ă���
            guardWaitTime = WaveGuardTime();

            // �h���ԂɈڍs����
            StartCoroutine(guardWaitTime);
            if (transform.parent.localScale.x > 0)
            {
                waveAngle = 1;
            }
            else
            {
                waveAngle = -1;
            }
            //WaveCreate(transform.position.x + ((Mathf.Abs(transform.lossyScale.x) * (0.5f + shield.radius * 2) + 0.05f) * waveAngle), waveHight, 0);


        }//----- elseif_stop -----
    }
    //private void WaveCreate(float startPosX, float waveHight, float startPosY)
    //{
    //    arrayNum = vfxManager.WaveSpawn(startPosX, waveSpeed * waveAngle, waveHight, waveWidth, 1);
    //    for (int i = 0; i < waveCount; i++)
    //    {
    //        if (l_waveCollisionObj[i].transform.position.z != 0)
    //        {
    //            //�����蔻��p�̃N���[���𐶐�
    //            //cloneObj = Instantiate(waveObj, new Vector3(groundHit.point.x, waveSpornPosY), Quaternion.identity);
    //            //waveCollition wave = l_waveCollisionObj[i].gameObject.GetComponent<waveCollition>();
    //            // �����蔻��ɔԍ����w��
    //            l_waveCollitions[i].waveNum = arrayNum;
    //            // �N���[���ɑΉ������� vfxManager ��ݒ�
    //            l_waveCollitions[i].vfxManager = vfxManager;
    //            // �g���v���C���[�������������g�ɐݒ�
    //            l_waveCollitions[i].waveType = waveCollition.WAVE_TYPE.ENEMY;
    //            // �R���W�����𓮂���
    //            l_waveCollitions[i].waveMode = waveCollition.WAVE_MODE.PLAY;
    //            // �R���W�����̍�����g�̍����ɐݒ�
    //            l_waveCollisionObj[i].transform.localScale = new Vector3(0, 0, 1);
    //            // �R���W�����̔����ʒu��g�̔����ʒu�ɐݒ�
    //            l_waveCollitions[i].waveStartPosition = new Vector3(startPosX, startPosY, 0.0f);

    //            //l_waveCollisionObj[i].transform.position = new Vector3(groundHit.point.x, waveSpornPosY, 0.0f);
    //            // �R���W�����̍ő�̍�����ݒ�
    //            l_waveCollitions[i].maxWaveHight = waveHight;
    //            break;
    //        }
    //        else if (i == waveCount - 1)
    //        {
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(0, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }
    //    }
    //}
    private void Start()
    {
        // ���g�̖��O��"shield"�ɂ���
        this.gameObject.name = "shield";

        // �����̐e���擾
        myParent = transform.parent.gameObject;
        parentAnim = myParent.GetComponent<Animator>();

        // �R���[�`��������������
        guardWaitTime = WaveGuardTime();

        // �J�n���̃t�F�C�Y�����O�Ƃ��Ċi�[
        phaseLog = phaseNow;

        // �����̏��̐e���猩�����΍��W���擾
        fixPosition = myParent.transform.position-transform.position;

        // �ŏ��̃t�F�C�Y�ݒ�ʂ�̑ϋv�l��ݒ�
        shieldHitPoint = shieldPhase[0];

        for (int i = 0; i < waveCount; i++)
        {
            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(3, 0, 50), Quaternion.identity));
            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
            l_waveCollisionObj[i].AddComponent<EtoP_Damage>();
        }
    }

    private void FixedUpdate()
    {
        // ���̑ϋv�l��0�ȉ��ɂȂ����Ƃ�
        if (shieldHitPoint <= 0)
        {
            // ���j���Ԃɂ���
            shieldBreak = true;
            parentAnim.SetBool("staning", true);
        }

        // ���̈ʒu��{�̂��瘨�����Ȃ��悤�ɂ���
        this.transform.position = myParent.transform.position 
                            + new Vector3(fixPosition.x * Mathf.Sign(transform.parent.localScale.x),
                              -fixPosition.y,fixPosition.z);

        // �t�F�C�Y���ύX���ꂽ�Ȃ�
        if ((phaseNow != phaseLog) && !lastPhase)
        {
            if (phaseNow != shieldPhase.Length)
            {
                Debug.Log("���C������");
                // �t�F�C�Y�ɍ��킹�ď��̑ϋv�l��ύX����
                shieldHitPoint = shieldPhase[phaseNow];
            }

            parentAnim.SetBool("staning", false);

            // ���O���X�V����
            phaseLog = phaseNow;

            startup = false;

            // �C���s�̏ꍇ
            if (notRepiar)
            {
                // �C������������
                shieldRepairNow = false;
                shieldRepairEnd = true;

                // ����j��ł����Ԃɂ���
                shieldBreak = false;
                unbreakable = false;

                // ���̓����蔻���t�^����
                shield.enabled = true;
            }

            // �t�F�C�Y���i�s�s�\�ɂȂ�����
            if (phaseNow == shieldPhase.Length)
            {
                // �ŏI�t�F�C�Y��Ԃɂ���
                lastPhase = true;

            }//----- if_stop -----

        }//----- if_stop -----
    }
}
