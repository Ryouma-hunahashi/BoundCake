using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//======================================================
//          version �F 1.1   �T�v
// �E�{�X��p�X�e�[�^�X��ǉ����܂���
//======================================================
// �Ή���2023/04/08
// �{��
[System.Serializable]
public class BossStatusManager
{
    // �{�X�̖��O
    [Tooltip("�{�X�̖��O")]
    public string name = "";
    // �U���̏��
    [Tooltip("�U�������ǂ���")]
    public bool nowAttack;
    // �e�X�e�[�^�X
    [Tooltip("�{�X��HP�ݒ�")]
    public sbyte hitPoint;
}

public class StatusManager : MonoBehaviour
{
    static public int maxHitPoint = 6;      // �ő�̗�
    static public int nowHitPoint = 3;      // ���̗̑�
    static public int coinCount = 0;        // �R�C���̎擾����
    static public int gameScore = 0;        // �Q�[���S�̂̃X�R�A
}
