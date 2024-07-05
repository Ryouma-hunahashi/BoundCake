using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// �͂ݏ���
//==================================================
// �쐬��2023/05/02    �X�V��2023/05/04
// �{��
public class Player_Grab : MonoBehaviour
{
    // �e(�v���C���[)�����擾
    private Rigidbody parentRb;
    private Player_Main parentScript;

    // �e�̎q�ɂ���X�N���v�g���擾
    private Player_Fall pl_Fall;

    // �W�����v�`���[�W�̐ݒ� ----------
    public bool chargeNow;          // �`���[�W��
    public byte chargeStartPoint;   // �`���[�W�J�n���̗�
    public byte chargePoint;        // ���݃`���[�W��
    public byte chargeLimit = 20;   // �`���[�W�ʏ��

    public float chargeLandDis = 0.25f;
    public float chargeLandSpd = 10f;
    // �W�����v���̐ݒ� ----------

    private void Start()
    {
        // �e�̏����擾
        parentRb = transform.parent.GetComponent<Rigidbody>();
        parentScript = transform.parent.GetComponent<Player_Main>();

        

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
    // �쐬��2023/05/02    �X�V��2023/05/04
    // �{��
    public void ChargePower()
    {
        // �|�C���g������𒴂��Ă��Ȃ����
        if (chargePoint < chargeLimit)
        {
            // ������ԃ`���[�W���莞�̃A�j���[�V����
            //if(!chargeNow)
            //{
            //    parentScript.pl_Anim.SetBool("grabJumping", false);
            //    parentScript.pl_Anim.Play("grabCharge");
            //}
            // �`���[�W�ʂ��㏸������
            chargePoint++;
            chargeNow = true;
            if (!parentScript.groundLand.nowCharge)
            {
                if (parentScript.groundLand != null)
                {
                    parentScript.groundLand.ChargeLand(chargeLandDis, chargeLandSpd, true);
                }
                parentScript.pl_Audio.playerSource.loop = false;
                parentScript.pl_Audio.ChargeSound();
                parentScript.ef_Manager.PlayAura();
            }
           

        }//----- if_stop -----
        //else
        //{
        //    chargeNow = false;

        //}//----- else_stop -----
    }

    //==================================================
    //          �`���[�W�W�����v���s
    // ���ړ����x�����鍀�ڂ�����̂ŃR���[�`���ɂȂ��Ă܂�
    // ���ێ����ꂽ�`���[�W�ʂɂ���ăW�����v�͂��ω�
    // ���W�����v�����̃A�j���[�V�����͂����ɏ���
    // �߂�l : �Ȃ�
    //  ����  : _point : �`���[�W�l
    //==================================================
    // �쐬��2023/05/02    �X�V��2023/05/04
    // �{��
    public IEnumerator ChargeJump(float _point)
    {
        Debug.Log("�`���[�W�l[ " + _point + " ]�Œ͂݃W�����v���J�n���܂����I");
        
        parentScript.pl_Col.isTrigger = true;

        parentScript.inRayRide = true;

        parentScript.nowJump = false;

        if (parentScript.groundLand != null)
        {
            StartCoroutine(parentScript.groundLand.ChargeEnd());
        }
        if(_point>chargeLimit/2)
        {
            parentScript.ef_Manager.PlayJumpWind();
        }

        

        float i = (60 / Test_FPS.instance.m_fps);

        // �`���[�W�l���O�ɂȂ�܂ő�����
        while (_point > 0)
        {
            // �P�t���[������
            // �`���[�W�l�����������Ă���
            yield return null;
            _point-=i;
            
            // �W�����v���̈ړ����x���X�V����
            parentRb.velocity = new Vector3(parentRb.velocity.x, (float)_point, parentRb.velocity.y);

        }//----- while_stop -----

        parentScript.pl_Col.isTrigger = false;

        parentScript.nowJump = true;

        //chargePoint = chargeStartPoint;

        // �������J�n����
        StartCoroutine(pl_Fall.FallTheAfterJump());
    }
}
