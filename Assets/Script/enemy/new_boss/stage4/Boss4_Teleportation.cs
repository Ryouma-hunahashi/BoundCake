using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �����n�_�Ɉړ�����
// ��[Setup_AvatarPointer]�ɂĐ������ꂽ�����n�_��
// �@�����_���ňړ�����X�N���v�g�ł�
// ���V�[�����"AvatarPointOperator"���K�v�ł�
//==================================================
// �����2023/05/24    // �X�V��2023/05/30
// �{��
public class Boss4_Teleportation : MonoBehaviour
{
    // �w�߂̏����擾
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // �e�̏����擾
    private Boss4_Main parMain;

    // ���݂̏��
    public bool teleported;

    private void Start()
    {
        // �w�߂̏����擾
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();

        // �e�̏����擾
        GameObject parObj = this.transform.parent.gameObject;
        parMain = parObj.GetComponent<Boss4_Main>();
    }

    //==================================================
    //          �w�肳�ꂽ�n�_�փ����_���Ɉړ����s���֐�
    // �����g�̃��C����������i�[���ꂽ�����_���Ȓl�𗘗p����
    // �@���g�A���g�̈ړ��n�_������A�����ֈړ����s���܂�
    // �߂�l�F�Ȃ�
    //  ���� �F�Ȃ�
    //==================================================
    // �����2023/05/30
    // �{��
    public void Teleportation()
    {
        if (parMain.nowDmgReAct) return;

        Debug.Log("�]�ڂ��܂�");

        // �ړ��ʒu�𗐐��Ŏw�肷��
        parMain.Randomizer();

        // �e�̏����擾
        GameObject parObj = this.transform.parent.gameObject;

        // �e�̈ʒu��ύX����
        parObj.transform.position = avatarOperator.transform.GetChild(parMain.randomPoint[0]).transform.position;

        // �i�[���ꂽ���g�̐��J��Ԃ�
        for (int i = 0; i < parMain.avaObj.Length; i++)
        {
            // ���g�����݂��Ă��Ȃ��Ȃ珈�����΂�
            if (parMain.avaMain[i].active == false) continue;

            // ���g�̈ʒu��ύX����
            parMain.avaObj[i].transform.position = avatarOperator.transform.GetChild(parMain.randomPoint[i]).transform.position;

        }//----- for_stop -----

        teleported = true;
    }
}
