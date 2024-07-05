using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// �W�����v����
//==================================================
// �쐬��2023/04/24    �X�V��2023/05/10
// �{��
public class Player_Jump : MonoBehaviour
{
    //==============================
    // ���̃X�N���v�g���̗���
    // pow : power
    // UL : upperLimit
    //==============================

    // �e(�v���C���[)�����擾
    private Rigidbody mainRb;
    private Player_Main mainScript;

    // �e�̎q�ɂ���X�N���v�g���擾
    private Player_Fall pl_Fall;

    // �W�����v�ʂ̐ݒ� ----------
    public byte[] jumpPow = new byte[2]
    {
        20,28
    };    // �W�����v��(�`���[�W��)

    // �W�����v�`���[�W�̐ݒ� ----------
    public bool chargeNow;          // �`���[�W��
    public byte chargeStartPoint;   // �`���[�W�J�n���̗�
    public byte chargePoint;        // ���݃`���[�W��
    public byte chargeUL = 28;   // �`���[�W�ʏ��

    public byte changeJump;     // �W�����v�͂̕ω����
    public byte changePoint;    // �W�����v��ω�������`���[�W�l

    public float chargeLandDis = 0.25f;
    public float chargeLandSpd = 10f;

    //public bool nowJump;

    // �W�����v���̐ݒ� ----------

    // �K�{�ł͂Ȃ����� -----
    public byte chargeLog;      // �`���[�W�l�̕ێ�


    private void Start()
    {
        // �e�̏����擾
        mainRb = transform.parent.GetComponent<Rigidbody>();
        mainScript = transform.parent.GetComponent<Player_Main>();

        // �e�̎q�ɂ�������擾
        pl_Fall = transform.parent.GetComponentInChildren<Player_Fall>();
    }

    //==================================================
    //          �W�����v�`���[�W���s
    // ���{�^���������ݎ��Ԃɂ���ăW�����v�͂��ω�
    // ���W�����v�ҋ@�̃A�j���[�V�����͂����ɏ���
    // �߂�l : �Ȃ�
    //  ����  : �Ȃ�
    //==================================================
    // �쐬��2023/04/29    �X�V��2023/05/14
    // �{��
    public void ChargePower()
    {
        // �|�C���g������𒴂��Ă��Ȃ����
        if (chargePoint < chargeUL)
        {
            if (!chargeNow && chargePoint < chargeUL&&mainScript.pl_Col.isTrigger == false&&!mainScript.nowJump)
            {
                mainScript.pl_Anim.SetBool("weakJumping", false);
                mainScript.pl_Anim.SetBool("strongJumping", false);
                mainScript.pl_Anim.SetBool("JumpEnding", false);
                mainScript.pl_Anim.Play("JumpCharge");
                mainScript.ef_Manager.PlayAura();
                
            }
            // �`���[�W�ʂ��㏸������
            chargePoint++;
            chargeNow = true;
            if(chargePoint == 5)
            {
                mainScript.pl_Audio.playerSource.loop = false;
                mainScript.pl_Audio.ChargeSound();
            }

            // �e�̃|�W�V�������ꎞ�i�[
            //var parentPos = transform.parent.GetComponent<Rigidbody>();
            // ���̃R���W���������X�ɉ�����B�����镪��߂�l�Ƃ��e��������B
            //parentPos.y-=mainScript.groundLand.ChargeLand(chargePoint,chargeUL,0.25f);
            // �|�W�V�������X�V
            //transform.parent.position = parentPos;
            if (mainScript.groundLand!=null&&!mainScript.groundLand.nowCharge)
            {
                mainScript.groundLand.ChargeLand(chargeLandDis, chargeLandSpd,false);
            }
            mainRb.useGravity = true;

        }//----- if_stop -----
    }

    //==================================================
    //          �`���[�W�W�����v���s
    // ���ړ����x�����鍀�ڂ�����̂ŃR���[�`���ɂȂ��Ă܂�
    // ���ێ����ꂽ�`���[�W�ʂɂ���ăW�����v�͂��ω�
    // ���W�����v�����̃A�j���[�V�����͂����ɏ���
    // �߂�l : �Ȃ�
    //  ����  : _point : �`���[�W�l
    //==================================================
    // �쐬��2023/04/30    �X�V��2023/05/01
    // �{��
    public IEnumerator ChargeJump(float _point)
    {
        Debug.Log("�`���[�W�l[ " + _point + " ]�ŃW�����v���J�n���܂����I");

        mainScript.nowJump = true;
        mainRb.useGravity = false;
        chargeNow = false;
        
        
        if(_point>changePoint)
        {
            mainScript.ef_Manager.PlayJumpWind();
        }

        float i = (60 / Test_FPS.instance.m_fps);
        if (mainScript.groundLand != null)
        {
            // �n�ʂ̃`���[�W���t���O��؂�B
            StartCoroutine(mainScript.groundLand.ChargeEnd());
        }
        // �`���[�W�l���O�ɂȂ�܂ő�����
        while (_point > 0)
        {
            //if (parentScript.underRayGrab)
            //{
            //    // �������J�n����
            //    StartCoroutine(pl_Fall.FallTheAfterJump());
            //    yield break;

            //}//----- if_stop -----

            // �P�t���[������
            // �`���[�W�l�����������Ă���
            yield return null;
            _point-=i;

            // �W�����v���̈ړ����x���X�V����
            mainRb.velocity = new Vector3(mainRb.velocity.x, (float)_point, mainRb.velocity.z);

        }//----- while_stop -----

        // �������J�n����
        StartCoroutine(pl_Fall.FallTheAfterJump());
    }
}