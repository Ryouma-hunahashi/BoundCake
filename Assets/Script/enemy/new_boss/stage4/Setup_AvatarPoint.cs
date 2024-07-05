using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ���g�̒n�_���J�n���ɐݒ肷��
// ���ʃX�N���v�g��[AvatarPoint]�n�����K�v�ł�
// ���Q�[���J�n���ɕ��g�������ݒ肵�܂�
//==================================================
// �����2023/05/24
// �{��
public class Setup_AvatarPoint : MonoBehaviour
{
    // �]�ڐ�I�y���[�^�[�ݒ�
    public string apo = "AvatarPointOperator";  // �e�I�u�W�F�N�g�̖��O(�Œ�)
    public string pointName = "AvatarPoint";    // �q�̖��O(���O�̌��ID�g�p)

    // ���g�̃v���n�u
    public GameObject avatarPrefab;

    // ���g�Őݒ�I�u�W�F�N�g���w�肷��
    public bool auto = true;    // �����Ń|�C���g��ݒ肷��

    private void Awake()
    {
        // �����Őݒ肵�Ȃ��Ȃ�
        if(auto)
        {
            // �����Őݒ���J�n���܂�
            AutoSetup();

        }//----- if_stop -----
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
        this.gameObject.name = apo;

        // ���g�̎q�̐����擾
        int children = this.transform.childCount;

        // ���g�̎q�̐��J��Ԃ�
        for (int i = 0; i < children; i++)
        {
            // �����n�_���ꎞ�I�Ɋi�[
            GameObject pointObj = transform.GetChild(i).gameObject;

            // �����n�_��ID��U�蕪����
            pointObj.name = pointName + i;

            // ���g���ݒ肳��Ă��Ȃ��Ȃ珈���𔲂���
            if (avatarPrefab == null) continue;

            // ���g��������ID�𖼑O�ɒǋL����
            GameObject avatarObj = Instantiate(avatarPrefab);
            avatarObj.name = "Avatar" + i;

            // ���g�̋������擾����
            Boss4_Avatar_Main avatarMain = avatarObj.GetComponent<Boss4_Avatar_Main>();

            // ���g��ID�����蓖�Ă�
            avatarMain.myID = i;
            
            // ���g�������n�_�̎q�I�u�W�F�N�g�Ɏw�肷��
            avatarObj.transform.SetParent(pointObj.transform, false);

        }//----- for_stop -----
    }
}
