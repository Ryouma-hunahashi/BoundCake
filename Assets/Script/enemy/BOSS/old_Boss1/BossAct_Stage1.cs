using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAct_Stage1 : MonoBehaviour
{
    // ���g�̏����擾
    private Rigidbody rb;
    // ���g�̃A�j���[�^�[
    private Animator anim;




    [SerializeField] private bool actTest = true;

    [SerializeField] private bool damge;
    [SerializeField] private bool invincibility;    // ���G���

    // �N���X�̎擾
    [SerializeField] public BossStatusManager bossStatusManager;

    // �{�X�̍s��
    [Header("�{�X����")]
    [SerializeField] private Enemy_A_Rush rushAtk;  // �ːi�U���̊i�[
    [SerializeField] private Enemy_D_Shield guard;  // �h���Ԃ̊i�[

    [SerializeField] private List<GameObject> childObjects = new List<GameObject>();

    [SerializeField] private GameObject waveHit;
    [SerializeField] private GameObject waveLog;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wave") && guard.guardNow)
        {
            Debug.Log("�S");
            waveHit = other.gameObject;
        }

        if (other.CompareTag("Wave") && guard.shieldBreak && !invincibility && !guard.guardNow && !rushAtk.rushNow)
        {
            if (waveHit == null)
            {
                Debug.Log("�P");
                waveHit = other.gameObject;

                for (int i = 0; i < guard.waveCount; i++)
                {
                    if (guard.l_waveCollisionObj[i] == waveHit)
                    {
                        return;
                    }
                }

                bossStatusManager.hitPoint--;

                Debug.Log("�_���[�W�I");
                anim.Play("Damage");
                anim.SetBool("staning",false);

                // �t�F�C�Y��i�s����
                if (guard.phaseNow < guard.shieldPhase.Length)
                {
                    Debug.Log("�t�F�C�Y�ύX�I");
                    guard.phaseNow++;
                }

                // ���g�Ƀ_���[�W��^�����Ȃ���Ԃɂ���
                invincibility = true;

                if (waveHit != waveLog)
                {
                    Debug.Log("�Q");
                    waveLog = waveHit;
                }
            }
        }

        //if (1<<other.gameObject.layer == 1 << 6)
        //{
        //    guard.vfxManager = other.transform.GetChild(0).GetComponent<vfxManager>();
        //}

        //// [Wave]�^�O�����Ă���I�u�W�F�N�g�ɐG�ꂽ�� &
        //// �����j�󂳂�Ă���Ƃ�
        //// ���G��Ԃł͂Ȃ��Ȃ�
        //if (other.CompareTag("Wave") && guard.shieldBreak && !invincibility && !guard.stopDamage && !guard.guardNow && !rushAtk.rushNow && guard.notRepiar)
        //{
        //    // ���g�̗̑͂�����������
        //    bossStatusManager.status.hitPoint--;

        //    // �t�F�C�Y��i�s����
        //    if (guard.phaseNow < guard.shieldPhase.Length)
        //    {
        //        guard.phaseNow++;
        //    }

        //    // ���g�Ƀ_���[�W��^�����Ȃ���Ԃɂ���
        //    invincibility = true;

        //}//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wave"))
        {

            if (waveHit != null)
            {
                Debug.Log("�R");
                waveHit = null;

                // ���g�Ƀ_���[�W��^�������Ԃɂ���
                invincibility = false;
            }
        }
    }

    private void Start()
    {
        // �{�X�̊e�������i�[
        rushAtk = this.GetComponentInChildren<Enemy_A_Rush>();
        guard = this.GetComponentInChildren<Enemy_D_Shield>();

        // [Rigidbody]�̎擾
        rb = GetComponent<Rigidbody>();

        // ���܂��擾�ł��Ȃ������ꍇ
        if (rb == null)
        {
            Debug.LogError("[Rigidbody]��������܂���I");

        }//----- if_stop -----

        anim = GetComponent<Animator>();
        if(anim == null)
        {
            Debug.LogError("[Animator]��������܂���");
        }

        // �{�X�̖��O���ݒ肳��Ă���Ȃ�
        if (bossStatusManager.name != "")
        {
            // �ݒ肳��Ă��閼�O�ɕύX����
            this.gameObject.name = bossStatusManager.name;

        }//----- if_stop -----

        // �q�I�u�W�F�N�g�̐����擾
        int childCount = this.transform.childCount;

        // ���X�g����x����������
        childObjects.Clear();

        // ���g�ɂ��Ă���q�I�u�W�F�N�g���擾����
        for (int i = 0; i < childCount; i++)
        {
            // �q�I�u�W�F�N�g�����X�g���Ɋi�[
            childObjects.Add(transform.GetChild(i).gameObject);

        }//----- for_stop -----
    }

    private void FixedUpdate()
    {
        if (!guard.rotateNow && !rushAtk.rushNow && guard.rotateEnd && guard.shieldImpactEnd && guard.guardEnd && !guard.shieldRepairNow && !guard.shieldBreak && guard.startup)
        {
            // �ːi�J�n ----- ���̕ӂɂ��ɂ߂����
            anim.SetBool("rushing", true);
            rushAtk.StartCoroutine(rushAtk.RushAction());

            guard.startup = false;
        }

        if (!guard.rotateNow && !rushAtk.rushNow && !guard.shieldImpactNow && 
            guard.shieldImpactEnd && !guard.guardNow && rushAtk.rushEnd && 
            !guard.shieldRepairNow && !guard.shieldBreak || (guard.shieldRepairEnd && 
            !guard.shieldBreak) || actTest)
        {
            // �U������J�n ----- ���̕ӂɂ��ɂ߂����
            anim.SetBool("rushing",false);
            anim.Play("ShieldSet");

            // �U������Ə��\���͂قړ����ł�

            // ���̈ʒu��ύX����
            guard.ShieldImpact();

            // ��]�ҋ@���J�n����
            guard.StartCoroutine(guard.RotateWaitTime());

            guard.shieldRepairEnd = false;
            actTest = false;
        }
        else if (!guard.rotateNow && !rushAtk.rushNow && !guard.guardNow && guard.rotateEnd && guard.shieldImpactNow && !guard.shieldImpactEnd && !guard.shieldRepairNow)
        {
            // ���U�艺�낵 ----- ���̕ӂɂ��ɂ߂����
            

            // ����ݒ�ʒu�܂ŐU�艺�낷
            guard.ShieldImpact();
        }

        if (guard.lastPhase && (bossStatusManager.hitPoint <= 0))
        {
            // �i�[���Ă���I�u�W�F�N�g�̔j��
            childObjects.Clear();
            waveHit = null;
            waveLog = null;

            // ���̃I�u�W�F�N�g��j�󂷂�
            gameObject.SetActive(false);
        }
    }
}