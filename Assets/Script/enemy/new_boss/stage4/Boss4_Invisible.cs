using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X�������s��
// �����g�A���g���҂�s����Ԃɂ��܂��B
// �@���ɓ|����Ă��镪�g�͍ēx���������܂ł͕\������܂���
//==================================================
// �����2023/05/24    �X�V��2023/05/30
// �{��
public class Boss4_Invisible : MonoBehaviour
{
    // �w�߂̏����擾
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // �e�̏����擾
    private Boss4_Main parMain;
    private SpriteRenderer parMesh;

    // ���g�̏����擾
    private SpriteRenderer[] avaMesh;

    // ��ԕω��܂ł̎���
    [SerializeField] private byte changeDelay = 180;
    [SerializeField] private byte animationDelay = 60;

    // ���݂̏��
    public bool nowInvisibleAnim;
    public bool nowVisualizeAnim;
    public bool nowInvisible;

    private void Start()
    {
        // �w�߂̏����擾
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();

        // ���g�̏����擾
        int avatarCnt = avatarOperator.transform.childCount;
        avaMesh = new SpriteRenderer[avatarCnt];

        // �����n�_�̐��J��Ԃ�
        for (int i = 0; i < avatarCnt; i++)
        {
            // ���g�̃��b�V�������擾
            avaMesh[i] = avatarOperator.transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();

            // ��x���������Ă���
            avaMesh[i].enabled = false;

        }//----- for_stop -----

        // �e�̏����擾
        GameObject parObj = this.transform.parent.gameObject;
        parMain = parObj.GetComponent<Boss4_Main>();
        parMesh = parObj.GetComponent<SpriteRenderer>();
    }

    public IEnumerator AnimationDelay()
    {
        // �ҋ@���ԏI����A�����A�s������Ԃֈڍs
        // �s����ԂȂ�
        if(nowInvisible)
        {
            // �ҋ@���Ԃ��J�n
            for (int i = 0; i < changeDelay; i++)
            {
                // �P�t���[���x��������
                yield return null;

                // �_���[�W���A�N�V�������s��ꂽ�Ȃ珈���𔲂���
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // ���G��Ԃ���������
            parMain.invincibility = false;

            // ���g�Ɠ|����Ă��Ȃ����g��\������
            VisualizationObjects();

            // �o���A�j���[�V�����̊J�n -----------
            parMain.anim.SetBool("Spawning", true);
            nowVisualizeAnim = true;

            // �`��ҋ@���Ԃ��J�n
            for (int i = 0; i < animationDelay; i++)
            {
                // �P�t���[���x��������
                yield return null;

                // �_���[�W���A�N�V�������s��ꂽ�Ȃ珈���𔲂���
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // �o���A�j���[�V�����̏I�� -----------
            parMain.anim.SetBool("Spawning", false);
            nowVisualizeAnim = false;

        }//----- if_stop -----
        else
        {
            // �ҋ@���Ԃ��J�n
            for (int i = 0; i < changeDelay; i++)
            {
                // �P�t���[���x��������
                yield return null;

                // �_���[�W���A�N�V�������s��ꂽ�Ȃ珈���𔲂���
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // ���ŃA�j���[�V�����̊J�n -----------
            parMain.anim.Play("Hide");
            nowInvisibleAnim = true;

            // �`��ҋ@���Ԃ��J�n
            for (int i = 0; i < animationDelay; i++)
            {
                // �P�t���[���x��������
                yield return null;

                // �_���[�W���A�N�V�������s��ꂽ�Ȃ珈���𔲂���
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // ���ŃA�j���[�V�����̏I�� -----------
            nowInvisibleAnim = false;

            // ���g�ƕ��g��s����Ԃɂ���
            InvisibleObjects();

        }//----- else_stop -----
    }

    //==================================================
    //          �s����Ԃֈڍs����֐�
    // �����g�ƕ��g�S�̂�s��������֐��ł�
    // �߂�l�F�Ȃ�
    //  ���� �F�Ȃ�
    //==================================================
    // �����2023/05/30
    // �{��
    public void InvisibleObjects()
    {
        Debug.Log("���g�ƕ��g��s�������܂�");
        nowInvisible = true;

        // ���g���\���ɂ���
        parMesh.enabled = false;

        // �i�[���ꂽ���b�V���̐��J��Ԃ�
        for(int i = 0; i < avaMesh.Length; i++)
        {
            // ���g���\���ɂ���
            avaMesh[i].enabled = false;

        }//----- for_stop -----

        if (parMain.nowDmgReAct) return;

        // �����_���Ȉʒu�ɓ]�ڂ��J�n����
        parMain.actTP.Teleportation();

        // ���@�U���̑ҋ@��Ԃֈڍs����
        StartCoroutine(parMain.actMB.AttackDelay());
    }

    //==================================================
    //          ����Ԃֈڍs����֐�
    // �����g�ƕ��g�S�̂���������֐��ł�
    // �����łɓ|����Ă��镪�g��A
    // �@���g�̈ʒu�ɑ��݂��镪�g�͕\�����܂���
    // �߂�l�F�Ȃ�
    //  ���� �F�Ȃ�
    //==================================================
    // �����2023/05/30
    // �{��
    public void VisualizationObjects()
    {
        Debug.Log("���g�ƕ��g���������܂�");
        nowInvisible = false;

        // ���g��\������
        parMesh.enabled = true;

        parMain.actTP.Teleportation();

        // �i�[���ꂽ���b�V���̐��J��Ԃ�
        for (int i = 0; i < avaMesh.Length; i++)
        {
            // ���g�����݂��Ă��Ȃ��Ȃ珈�����΂�
            if (parMain.avaMain[i].active == false) continue;

            // �t�F�C�Y�i�s��Ԃɂ���ĕ\�����鐔��ύX����
            if (parMain.setAvatarPhase[parMain.nowPhase] < i) continue;

            // ���g��\������
            avaMesh[i].enabled = true;

        }//----- for_stop -----

        if (parMain.nowDmgReAct) return;

        StartCoroutine(AnimationDelay());
    }
}
