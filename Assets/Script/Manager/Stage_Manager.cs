using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//========================================
//          �X�e�[�W�̏��
//========================================
public class Stage_Manager : MonoBehaviour
{
    // �V���O���g���̍쐬
    public static Stage_Manager instanse;

    private void Awake()
    {
        
        // ���g�����݂��Ă��Ȃ��Ȃ�
        if(instanse == null)
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

        for(byte i = 0;i<worldInformation.Count;i++)
        {
            worldInformation[i].worldLockLog = worldInformation[i].worldLock;
        }
    }

    // ���[���h���̐ݒ�
    [System.Serializable]
    public class WorldInfo
    {

        // �Y�[���̐ݒ�
        public enum ZoomSet
        {
            ON,
            OFF,
        }
        // ���[���h���
        [Tooltip("�n�}�Ђ̖��O")]
        public string worldName;
        [Tooltip("���[���h�̊J����")]
        public bool worldLock;
        public bool worldLockLog;
        // �X�e�[�W���̃N���X���擾
        [Tooltip("�X�e�[�W�̏��")]
        public List<StageInfo> stageInformation = new List<StageInfo>();
        [Tooltip("�Ή�����Image�ɃY�[�����s��")]
        public ZoomSet zoomSet;
        // �J�������
        [Tooltip("�J�������Œ肷����W\n" + "Image����̑��΍��W")]
        public Vector3 cameraZoomPos;
    }

    // �X�e�[�W���̐ݒ�
    [System.Serializable]
    public class StageInfo
    {
        // �p�t�F�̏������
        [System.Serializable]
        public struct parfaitInfo
        {
            public bool top;    // ��w
            public bool mid;    // ���w
            public bool btm;    // ���w
        }

        

        // �X�e�[�W���
        [Tooltip("�X�e�[�W�̖��O")]
        public string stageName;
        [Tooltip("�V�[���̖��O")]
        public string sceneName;
        [Tooltip("�X�e�[�W�̃��b�N���")]
        public bool stageLock = false;

        // ���W�A�C�e���̏�
        [Tooltip("�p�t�F�̏�����")]
        public parfaitInfo parfait;

        
        
    }

    // �C���X�y�N�^�[�ɕ\�� -----
    // �n�}���̐ݒ�
    public List<WorldInfo> worldInformation = new List<WorldInfo>();

    private void OnApplicationQuit()
    {
        worldInformation.Clear();
       
    }
}