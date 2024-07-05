using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//========================================
//          �X�e�[�W���̏����X�V
//  ���Q�[�����Ƀp�t�F��������ꂽ����s
//========================================
// �쐬��2023/05/19    �X�V��2023/05/21
// �{��
public class Parfait_Manager : MonoBehaviour
{
    // �V���O���g���̍쐬
    public static Parfait_Manager instanse;

    private void Awake()
    {
        // ���g�����݂��Ă��Ȃ��Ȃ�
        if (instanse == null)
        {
            // ���g���C���X�^���X��
            instanse = this;

            // �V�[���ύX���ɔj������Ȃ��悤�ɂ���
            DontDestroyOnLoad(this.gameObject);
        }//----- if_stop -----
        else
        {
            // ���łɎ��g�����݂��Ă���Ȃ�j��
            Destroy(this.gameObject);

        }//----- else_stop -----
    }

    // �Q�[�����̃p�t�F�ێ��󋵂��i�[
    [System.Serializable]
    public struct ParfaitHolder
    {
        public bool top;
        public bool mid;
        public bool btm;
    }

    // �Q�[�����̎擾���
    public ParfaitHolder getParfait;

    // �I�����ꂽ�X�e�[�W�����擾
    private byte nowWorld;  // ���[���h�ԍ�
    private byte nowStage;  // �X�e�[�W�ԍ�

    // �X�e�[�W�����i�[
    public List<Stage_Manager.WorldInfo> mapInfo;

    private void Start()
    {
        // �X�e�[�W�����擾
        mapInfo = Stage_Manager.instanse.worldInformation;
    }

    //========================================
    //          �p�t�F�擾���̍X�V
    //  ���S�[����Ƀp�t�F�̏�Ԃ��X�V����
    //========================================
    // �쐬��2023/05/19
    // �{��
    public void ParfaitUpdate()
    {
        // �ǋL�ҋ@
    }

    private void OnApplicationQuit()
    {
        mapInfo.Clear();
    }
}
