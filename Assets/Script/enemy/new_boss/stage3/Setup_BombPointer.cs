using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ���e�����n�_�̃Z�b�g�A�b�v
// ����]���̐e�ɐݒu����Ə��̐�������
// �@�����n�_��ݒ肵�܂��B
//==================================================
// �����2023/05/26    �X�V��2023/05/28
// �{��
public class Setup_BombPointer : MonoBehaviour
{
    // �]�ڐ�I�y���[�^�[�ݒ�
    public string bpo = "BombPointOperator";
    public string pointName = "BombPoint";

    // �����Őݒ�I�u�W�F�N�g���w�肷��
    public bool auto = true;

    public bool blasted;    // �����������ǂ���

    private void Awake()
    {
        if(auto)
        {
            AutoSetup();
        }
    }

    //==================================================
    //          �����ŕ��g��ݒ肷��
    // �߂�l �F�Ȃ�
    //�@�����@�F�Ȃ�
    //==================================================
    // �����2023/05/24
    // �{��
    private void AutoSetup()
    {
        // ���g�̖��O��ύX����
        this.gameObject.name = bpo;

        // ���g�̎q�̐����擾
        int children = this.transform.childCount;

        // ���g�̎q�̐��J��Ԃ�
        for (int i = 0; i < children; i++)
        {
            // �q�I�u�W�F�N�g���ꎞ�I�Ɋi�[
            GameObject childObj = transform.GetChild(i).gameObject;
            Vector3 childObj_Pos = childObj.transform.position;

            // ���̐��ɉ����Ďq�I�u�W�F�N�g�𐶐�
            // �q�I�u�W�F�N�g����ID��U�蕪����
            GameObject bombObj = new GameObject(pointName + i);

            // ���̎q�I�u�W�F�N�g�ɐݒ肷��
            bombObj.transform.SetParent(childObj.transform, false);

            // ���̃I�u�W�F�N�g�̏�����ɃX�|�[���|�C���g��ݒ肷��
            bombObj.transform.position = new Vector3(childObj_Pos.x, childObj_Pos.y + 1, childObj_Pos.z);

        }//----- for_stop -----
    }
}