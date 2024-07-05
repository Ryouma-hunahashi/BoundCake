using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X�̏������X�N���v�g
// �������g�����U�����֐��Ƃ��Ĕz�u���܂�
//==================================================
// �����2023/05/16    �X�V��2023/05/18
// �{��
public class Boss1_Shield : MonoBehaviour
{
    // �e�̏����擾
    private GameObject par_Obj;
    private Animator par_Anim;

    // �e�̋��������擾
    private Boss1_Main par_Main;
    private Boss1_Rush par_Rush;
    private WavePool wavePool;

    // �{�̂Ƃ̃Y��
    private Vector3 fixPosition;

    // �R���[�`���̊i�[
    [System.NonSerialized] public IEnumerator cor_Rotate;   // ��]
    [System.NonSerialized] public IEnumerator cor_Impact;   // �Ռ�
    [System.NonSerialized] public IEnumerator cor_Shield;   // �h��
    [System.NonSerialized] public IEnumerator cor_Repair;   // �C��

    // �ҋ@���Ԃ̐ݒ�
    [Header("----- �ҋ@�̐ݒ� -----")]
    [SerializeField] private ushort breakFrame = 10;    // �j�󎞂̖��G����
    [SerializeField] private ushort rotateFrame = 60;   // ��]�ҋ@����
    [SerializeField] private ushort impact_CT = 120;    // �h��J�n�܂ł̎���
    [SerializeField] private ushort shieldFrame = 480;  // �h������܂ł̎���
    [SerializeField] private ushort repairFrame = 45;   // �C���܂ł̎���

    // ����Ԃ̐ݒ�
    [Header("----- ���݂̏�� -----")]
    public bool nowStan;   // ������
    private bool nowBreak;  // �ҋ@��
    public bool nowRotate;  // ��]��
    public bool nowImpact;  // �U����
    public bool nowShield;  // �h�䒆
    public bool nowRepair;  // �C����
    public bool nowDamage;  // ���G��

    // ���̏ڍאݒ�
    [Header("----- ���ڍאݒ� -----")]
    private CapsuleCollider shieldCol;  // ���̓����蔻��
    public sbyte durability;            // �ϋv��
    public float downSpeed = 0.1f;      // �������x
    public Vector3 startPos;            // �����J�n�n�_
    public Vector3 finishPos;           // �����I���n�_
    public bool shieldBreak = false;    // ���̔j�����
    public bool unbreakable = false;    // ���j��s���

    // �t�F�C�Y���
    [Header("----- �i�K�̐ݒ� -----")]
    public byte nowPhase;                   // ���݂̃t�F�C�Y
    private byte logPhase;                  // �ߋ��̃t�F�C�Y
    public byte[] vitPhase = new byte[3];   // �t�F�C�Y���̑ϋv�l
    public bool lastPhase;                  // �ŏI�t�F�C�Y���
    private bool stanSetFg = false;

    // �g�̐ݒ�
    [Header("----- �g�̐ݒ� -----")]
    public vfxManager vfxManager;   // ������p���鎅��vfxManager
    private sbyte arrayNum = 0;     // �g�𐶐�����Ƃ��߂�l���N���[����
    [SerializeField] private float waveSpeed = 7.5f;    // �g�̑��x
    [SerializeField] private float waveWidth = 0.225f;  // �g�̐U����
    [SerializeField] private float waveHight = 2.0f;    // �g�̍���
    [SerializeField] private GameObject waveObj;        // �g�̔���v���n�u
    public int waveCount = 3;   // �v�[���ɐ�������g�̐�
    private int waveAngle = 1;  // �g�̕���

    // �g�̃R���W�����̃I�u�W�F�N�g�v�[��
    [System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();

    // �g�R���W�����ɃR���|�[�l���g����Ă���g����̃v�[��
    // �Y�����͔g����̃v�[���ɑΉ�
    private List<waveCollition> l_waveCollitions = new List<waveCollition>();
    

    private void OnTriggerEnter(Collider other)
    {
        // �����\���Ă����
        // �_���[�W���^�������Ԃ̂Ƃ�
        // "Wave"�^�O���t�����I�u�W�F�N�g�ɐG�ꂽ�Ȃ�
        if (nowShield && !nowDamage && !unbreakable && other.CompareTag("Wave"))
        {
            for (int i = 0; i < waveCount; i++)
            {
                if (other.gameObject == l_waveCollisionObj[i])
                {
                    return;
                }//----- if_stop -----

            }//----- for_stop -----
            if (par_Main.audio.bossSource.isPlaying)
            {
                par_Main.audio.bossSource.Stop();
            }

            par_Main.audio.bossSource.loop = false;
            par_Main.audio.Boss1_GuardSound();
            // ���̑ϋv�l������������
            durability--;

            // �_���[�W��^�����Ȃ���Ԃɂ���
            nowDamage = true;

        }//----- if_stop -----

        if ((1 << other.gameObject.layer) == 1 << 6)
        {
            if (other.transform.childCount == 1)
            {
                vfxManager = other.transform.GetChild(0).GetComponent<vfxManager>();

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // "Wave"�^�O�������I�u�W�F�N�g�����ꂽ��
        if (other.CompareTag("Wave"))
        {
            // �_���[�W��^������悤�ɂ���
            nowDamage = false;

        }//----- if_stop -----
    }

    private void Start()
    {
        // ���g�̖��O��"shield"�ɂ���
        this.gameObject.name = "shield";

        // �e�̏����擾
        par_Obj = transform.parent.gameObject;
        par_Anim = par_Obj.GetComponent<Animator>();

        // �e�̋��������擾
        par_Main = transform.parent.GetComponent<Boss1_Main>();
        par_Rush = transform.parent.GetComponentInChildren<Boss1_Rush>();
        wavePool = GetComponent<WavePool>();

        // ���̓����蔻����擾
        shieldCol = this.GetComponent<CapsuleCollider>();

        // �R���[�`����������
        cor_Rotate = Rotate();
        cor_Impact = ShieldImpact();
        cor_Shield = ShieldGuald();
        cor_Repair = ShieldRepair();

        // �J�n���̃t�F�C�Y�����O�Ƃ��Ċi�[
        logPhase = nowPhase;

        // �����̏��̐e���猩�����΍��W���擾
        fixPosition = par_Obj.transform.position - transform.position;

        // �ŏ��̃t�F�C�Y�ݒ�ǂ���̑ϋv�l��ݒ�
        durability = (sbyte)vitPhase[0];

        // �g�̊e�����擾
        for (int i = 0; i < waveCount; i++)
        {
            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(3, 0, 50), Quaternion.identity));
            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
            l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        }//----- for_stop -----
    }

    private void FixedUpdate()
    {
        // ���̑ϋv�l���O�ȉ��ɂȂ����Ƃ�
        if (durability <= 0)
        {
            if (!nowBreak)
            {
                
                // �{�̍U���\�ɂ���֐�
                StartCoroutine(ShieldBreak());

                nowBreak = true;

            }//----- if_stop -----

            
            nowStan = true;

        }//----- if_stop -----

        // ���̈ʒu��{�̂��瘨�����Ȃ��悤�ɂ���
        this.transform.position = par_Obj.transform.position
                              + new Vector3(fixPosition.x * Mathf.Sign(transform.parent.localScale.x),
                              -fixPosition.y, fixPosition.z);

        // �ŏI�t�F�C�Y�ł͂Ȃ��Ƃ���
        // �t�F�C�Y���ύX���ꂽ�Ȃ�
        if (!lastPhase && (nowPhase != logPhase))
        {
            // ���O���X�V����
            logPhase = nowPhase;

            // �t�F�C�Y�ɍ��킹�ď��̑ϋv�l��ύX����
            durability = (sbyte)vitPhase[nowPhase];

            // �X�^���A�j���[�V�������I��
            par_Anim.SetBool("staning", false);
            stanSetFg = false;
            nowStan = false;
            if (par_Main.audio.bossSource.isPlaying)
            {
                //par_Main.audio.bossSource.Stop();
            }

            par_Main.audio.bossSource.loop = false;

            // �t�F�C�Y���i�s�s�\�ɂȂ�����
            if (nowPhase == vitPhase.Length - 1)
            {
                // �ŏI�t�F�C�Y�̏�Ԃ𑗂�
                lastPhase = true;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    // �����2023/05/18
    public IEnumerator ShieldBreak()
    {
        shieldCol.enabled = false;
        
        
        if (!nowStan)
        {
            Debug.Log("���j�󂷂邺�I");

            par_Main.audio.bossSource.loop = false;
            par_Main.audio.Boss1_ShieldBreakSound();
        }
        // �{�̍U���\�ɂȂ�܂ł̖��G����
        for (int i = 0; i < breakFrame; i++)
        {
            yield return null;

        }//----- if_stop -----


        // �X�^���A�j���[�V�������J�n
        par_Anim.SetBool("staning", true);
        
        if (!stanSetFg&&!par_Main.nowDmgAct)
        {

            Debug.Log("�X�^���`");
            par_Main.audio.bossSource.loop = true;

            par_Main.audio.BossStunSound();
            stanSetFg = true;
        }
        else if(!par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.loop = true;

            par_Main.audio.BossStunSound();
        }
        shieldBreak = true;

        nowBreak = false;
    }

    //==================================================
    //          ����U�艺�낵�n�߂�܂ł̑ҋ@����
    // �����U�艺�낵�U�����J�n����܂ł̑ҋ@�p�֐��A��������
    //==================================================
    // �����2023/05/16
    // �{��
    public IEnumerator Rotate()
    {
        while (nowStan || par_Main.nowDmgAct)
        {
            yield return null;
        }

        Debug.Log("��]���J�n���܂������I");

        // ��]��Ԃ𑗂�
        nowRotate = true;

        // ��]�����������܂ł̎���
        for (int i = 0; i < rotateFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // ��]��Ԃ�����
        nowRotate = false;

        // ���̑���Ɉڍs����
        StopCoroutine(cor_Impact);
        cor_Impact = ShieldImpact();
        StartCoroutine(cor_Impact);
    }


    //==================================================
    //          ���U�艺�낵�U��
    // ���������ɒ@�����ĐU�����΂��܂�
    //==================================================
    // �����2023/05/16
    // �{��
    public IEnumerator ShieldImpact()
    {
        if (nowStan || par_Main.nowDmgAct) yield break;
        // ���\����Ԃ𑗂�
        nowImpact = true;

        // ���\���̃A�j���[�V�������J�n
        par_Anim.Play("ShieldSet");

        // ���̓����蔻����o��������
        shieldCol.enabled = true;

        // ���̒��n�n�_�������ʒu�ɐݒ�
        finishPos = shieldCol.center;

        // ���g��U�艺�낵�J�n�n�_�Ɉړ�
        shieldCol.center = new Vector3(shieldCol.center.x, startPos.y, shieldCol.center.z);

        // ���U�艺�낵�̃A�j���[�V�����J�n
        par_Anim.SetBool("Impact", true);
        if (par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.Stop();
        }
        par_Main.audio.bossSource.loop = false;
        par_Main.audio.Boss1_AttackSound();
        // �U�艺�낵�n�_�ɓ��B���Ă��Ȃ��Ȃ�
        while (finishPos.y < shieldCol.center.y)
        {
            // �P�t���[���x��������
            yield return null;

            // �U�艺�낷
            shieldCol.center = new Vector3(shieldCol.center.x, shieldCol.center.y - downSpeed, shieldCol.center.z);

        }//----- while_stop -----
        
        // ���n�n�_���C������
        shieldCol.center = finishPos;

        // ���U�艺�낵�̃A�j���[�V�����I��
        par_Anim.SetBool("Impact", false);


        // �����j��ł���悤�ɂ���
        unbreakable = false;

        // ���\��������
        nowImpact = false;

        // �g���o��������
        if (transform.parent.localScale.x > 0)
        {
            waveAngle = 1;

        }//----- if_stop -----
        else
        {
            waveAngle = -1;

        }//----- else_stop -----
        //WaveCreate(transform.position.x + ((Mathf.Abs(transform.lossyScale.x) * (0.5f + shieldCol.radius * 2) + 0.05f) * waveAngle), waveHight, vfxManager.transform.parent.position.y);
        wavePool.WaveCreate(waveSpeed, waveWidth, waveHight, waveCollition.WAVE_TYPE.ENEMY,
            transform.position.x + ((Mathf.Abs(transform.lossyScale.x) * (0.5f + shieldCol.radius * 2) + 0.05f) * waveAngle), 
            vfxManager.transform.parent);

        // �h���ԂɈڍs����
        StopCoroutine(cor_Shield);
        cor_Shield = ShieldGuald();
        StartCoroutine(cor_Shield);
    }

    //==================================================
    //          �h��s���̊֐�
    // ���U�艺�낵�������U�������Ɏ󂯂܂�
    //==================================================
    // �����2023/05/16       �X�V��2023/05/18
    // �{��
    public IEnumerator ShieldGuald()
    {
        Debug.Log("�h����J�n���܂������I");

        // �h���Ԃɂ���
        nowShield = true;

        // �h�䂪����������܂ł̎���
        for (int i = 0; i < shieldFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

            // �����j�󂳂�Ă���Ƃ�
            // �C�����łȂ����
            if (shieldBreak && !nowRepair)
            {
                // ���C����ԂɈڍs����
                StopCoroutine(cor_Repair);
                cor_Repair = ShieldRepair();
                StartCoroutine(cor_Repair);

                // �����𔲂���
                yield break;

            }//----- if_stop -----

        }//----- for_stop -----

        // �h�����������
        nowShield = false;

        // �ːi����Ɉڍs����
        StopCoroutine(par_Rush.cor_Rush);
        par_Rush.cor_Rush = par_Rush.RushAction();
        StartCoroutine(par_Rush.cor_Rush);
    }

    //==================================================
    //          �����C�������܂ł̑ҋ@����
    // ���g�U���ɂ���ď����j�󂳂ꂽ�Ƃ��ɏ��̋@�\���ꎞ�I�Ɏ~�߂�
    //==================================================
    // �����2023/05/16
    // �{��
    public IEnumerator ShieldRepair()
    {
        Debug.Log("�����C�����Ă��܂����I");

        // ���̓����蔻���j��
        shieldCol.enabled = false;

        // �h���Ԃ���������
        nowShield = false;

        // ����j��s�ɂ���
        unbreakable = true;

        // �C����Ԃɂ���
        nowRepair = true;

        // �����C������܂ł̎���
        for (int i = 0; i < repairFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // �C��������
        nowRepair = false;

        // ����j��\�ɂ���
        unbreakable = false;

        // ���G��Ԃ�����
        par_Main.invincibility = false;

        // �����j�󂳂�Ă��Ȃ���Ԃɂ���
        shieldBreak = false;

        // ���̓����蔻���t�^����
        shieldCol.enabled = true;

        // ���̓���Ɉڍs����
        StopCoroutine(cor_Rotate);
        cor_Rotate = Rotate();
        StartCoroutine(cor_Rotate);
    }

    private void OnApplicationQuit()
    {
        l_waveCollisionObj.Clear();
        l_waveCollitions.Clear();
    }

    //private void WaveCreate(float startPosX, float waveHight, float startPosY)
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
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(0, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }//----- elseif_stop -----

    //    }//----- for_stop -----
    //}
}
