using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

//=================================
// �X�e�[�W�̃��U���g���
//=================================

public class Result_Manager : MonoBehaviour
{
    // �V���O���g���̍쐬
    public static Result_Manager instance;

    private void Awake()
    {
        // ���g�����݂��Ă��Ȃ��Ȃ�
        if (instance == null)
        {
            // ���M���C���X�^���X��
            instance = this;

            // �V�[���ύX���ɔj������Ȃ��悤�ɂ���
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // ���ɑ��݂��Ă���Ȃ�j��
            Destroy(this.gameObject);
        }
    }
    
    //  Stage_Manager stage_Manager;
    StageSelector stageSelector;
    Parfait_Manager parfait_Manager;
    Get_Parfait get_Parfait;

    [Tooltip("������������X�e�[�W�̏��")]
    public string stagename;
   public Canvas Canvas; // ���g�̃L�����o�X
   
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
    public byte nowWorld;  // ���[���h�ԍ�
    public byte nowStage;  // �X�e�[�W�ԍ�
    // �X�e�[�W�����i�[
    public List<Stage_Manager.WorldInfo> mapInfo;

    bool nextcheck = true;
    public bool selectercheck = false;

    private void OnApplicationQuit()
    {
        mapInfo.Clear();
    }

}
