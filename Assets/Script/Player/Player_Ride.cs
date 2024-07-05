using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// ���蔲������
//==================================================
// �쐬��2023/05/01
// �{��
public class Player_Ride : MonoBehaviour
{
    // �e(�v���C���[)�����擾����
    private Rigidbody parentRb;
    private Player_Main parentScript;

    // �e�̎q�ɂ���X�N���v�g���擾
    private Player_Grab pl_Grab;    // �͂ݗp�X�N���v�g
    private Player_Jump pl_Jump;    // �����p�X�N���v�g
    private Player_Fall pl_Fall;    // �����p�X�N���v�g
    private Player_Wave pl_Wave;    // �g�����X�N���v�g

    private void Start()
    {
        // �e�̏����擾
        parentRb = transform.parent.GetComponent<Rigidbody>();
        parentScript = transform.parent.GetComponent<Player_Main>();

        // �e�̎q�ɂ�������擾
        pl_Grab = transform.parent.GetComponentInChildren<Player_Grab>();
        pl_Jump = transform.parent.GetComponentInChildren<Player_Jump>();
        pl_Fall = transform.parent.GetComponentInChildren<Player_Fall>();
    }

    private void FixedUpdate()
    {
        if(parentScript.inRayRide)
        {
            parentScript.pl_Col.isTrigger = true;

        }//----- if_stop -----
        else if(!parentScript.inRayRide && (parentRb.velocity.y < 0 && parentScript.onRayGround))
        {
            parentScript.pl_Col.isTrigger = false;

        }//----- elseif_stop -----
    }
}
