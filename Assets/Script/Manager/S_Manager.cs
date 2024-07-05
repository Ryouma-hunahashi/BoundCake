using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// �����2023/02/17    �X�V��2023/04/25
// �{��
public class S_Manager : MonoBehaviour
{
    // �J�ڑO�܂ɏ����擾����
    private string sceneNow;
    private string sceneLog;
    
    // �J�n���Ƀ��������m�ۂ���
    public static S_Manager instance = null;

    //==================================================
    // �@�@�@�@�@ (�V���O���g���̐���)
    // �����������݂��Ă��Ȃ��Ƃ��ɂP�o��������
    // ���t�ɂQ�ȏ㑶�݂���Ƃ��͂P�����c���悤�ɂ���
    //==================================================
    // �쐬��2023/04/28
    // �{��
    private void Awake()
    {
        // ���������݂��Ă��Ȃ���Βǉ�����
        if (instance == null)
        {//----- if_start -----
            // ������ǉ�
            instance = this;
            // �V�[���؂�ւ����Ɏ������j������Ȃ��悤�ɂ���
            DontDestroyOnLoad(this.gameObject);
        }//----- if_stop -----
        else
        {//----- else_start -----
            // ���łɎ��������݂��Ă���ꍇ������j������
            Destroy(this.gameObject);
        }//----- else_stop -----
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneChange("ContentsSelect");
        }
    }

    //==================================================
    // �V�[���J�ڂ��s���֐�
    // �߂�l : �Ȃ�
    //  ����  : string �V�[���̖��O
    //==================================================
    // �����2023/04/25    �X�V��2023/04/28
    // �{��
    public void SceneChange(string sceneName)
    {
        // ���O���擾����
        sceneLog = SceneManager.GetActiveScene().name;

        // ���݂̃V�[�����擾
        sceneNow = sceneName;

        // �����Ă������O�̃V�[���ɑJ�ڂ���
        SceneManager.LoadScene(sceneName);
    }
}
