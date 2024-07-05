using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X���@�U��
// �������n�_�����Ƃɑ��݂��Ă��镪�g�Ɩ{�̂���U��
//==================================================
// �����2023/05/24    �X�V��2023/05/30
// �{��
public class Boss4_MagicBall : MonoBehaviour
{
    // �w�߂̏����擾
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // �e�̏����擾
    private Boss4_Main parMain;

    // �ҋ@����
    [SerializeField] private byte atkDelay = 240;

    // ���݂̏��
    public bool shot;

    private void Start()
    {
        // �w�߂̏����擾
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();

        // �e�̏����擾
        GameObject parObj = this.transform.parent.gameObject;
        parMain = parObj.GetComponent<Boss4_Main>();
    }

    public IEnumerator AttackDelay()
    {
        // �ҋ@���Ԃ��J�n
        for(int i = 0; i < atkDelay; i++)
        {
            // �P�t���[���x��������
            yield return null;

            // �_���[�W���A�N�V�������s��ꂽ�Ȃ珈���𔲂���
            if (parMain.nowDmgReAct) yield break;
        }

        // �ҋ@���Ԃ𔲂��U���ֈڍs
        MagicAttack(parMain.magicBall);
    }

    //==================================================
    //          �i�[���ꂽ�I�u�W�F�N�g���ˏo����֐�
    // �����g�Ɠ|����Ă��Ȃ����g�̒n�_����
    // �@�n���ꂽ�I�u�W�F�N�g���o�������܂�
    // �߂�l�F�Ȃ�
    //  ���� �F_magicPrefab(���C���Ɋi�[�����I�u�W�F�N�g���g�p)
    //==================================================
    // �����2023/05/30
    // �{��
    public void MagicAttack(GameObject _magicPrefab)
    {
        Debug.Log("���@�������I�I");

        // �e�̏����擾
        GameObject parObj = this.transform.parent.gameObject;

        // ���͋���{�̂��珢������
        GameObject magic = Instantiate(_magicPrefab);
        magic.transform.position = parObj.transform.position;

        // �����n�_�̐����擾
        int pointCnt = avatarOperator.transform.childCount;

        parMain.audio.bossSource.Stop();
        parMain.audio.Boss4_BulletAttackSound();

        // �����n�_�̉񐔌J��Ԃ�
        for (int i = 0; i < pointCnt; i++)
        {
            // ���g�����݂��Ă��Ȃ��Ȃ珈�����΂�
            if (parMain.avaMain[i].active == false) continue;

            // �t�F�C�Y�i�s��Ԃɂ���Đ���ύX����
            if (parMain.setAvatarPhase[parMain.nowPhase] < i) continue;

            // ���͋�����������
            magic = Instantiate(_magicPrefab);
            magic.transform.position = avatarOperator.transform.GetChild(i).GetChild(0).transform.position;

        }//----- for_stop -----

        shot = true;

        // ���͒e�ˏo�㏢���ҋ@�ֈڍs����
        StartCoroutine(parMain.actInv.AnimationDelay());
    }
}
