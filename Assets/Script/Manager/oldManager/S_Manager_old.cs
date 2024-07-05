using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//==================================================
// �����̃X�N���v�g�������Ă���Q�[���I�u�W�F�N�g�̌Ăѕ�
//   ���̃X�N���v�g���ł�"����"�Ə����܂�
//==================================================

// �쐬��2023/02/17    �X�V��2023/02/21
// �{��
public class S_Manager_old : MonoBehaviour
{
    // �R���g���[���[�������ݎ��̐^�U�l=================
    [Header("----- �{�^����Ԋm�F�p -----"), Space(5)]  // �ȉ���Ԃ̊m�F���C���X�y�N�^��ŉ\
    [Header("���X�e�B�b�N�̏�Ԋm�F")]

    // ���X�e�B�b�N�����֓|���Ă���Ԃ̂�[true]�ɂȂ�
    [Tooltip("���X�e�B�b�N�����ɓ|����Ă����Ԃ̂Ƃ��̂�[true]�ɂȂ�")]
    [SerializeField] private bool Lstick_Left = false;

    // ���X�e�B�b�N���E�֓|���Ă���Ԃ̂�[true]�ɂȂ�
    [Tooltip("���X�e�B�b�N���E�ɓ|����Ă����Ԃ̂Ƃ��̂�[true]�ɂȂ�")]
    [SerializeField] private bool Lstick_Right = false;

    // �R���g���[���[�������ݎ��̐^�U�l=================
    [Header("�\���{�^���̏�Ԋm�F")]

    // ���X�e�B�b�N�����֓|���Ă���Ԃ̂�[true]�ɂȂ�
    [Tooltip("�\���{�^�������ɉ�����Ă����Ԃ̂Ƃ��̂�[true]�ɂȂ�")]
    [SerializeField] private bool dpad_Left = false;

    // ���X�e�B�b�N���E�֓|���Ă���Ԃ̂�[true]�ɂȂ�
    [Tooltip("�\���{�^�����E�ɉ�����Ă����Ԃ̂Ƃ��̂�[true]�ɂȂ�")]
    [SerializeField] private bool dpad_Right = false;

    //==================================================

    // �L�[�{�[�h�p����
    private KeyCode leftArrow = KeyCode.LeftArrow;      // �����L�[
    private KeyCode rightArrow = KeyCode.RightArrow;    // �E���L�[
    private KeyCode leftKey = KeyCode.A;    // A�L�[�����Ƃ���
    private KeyCode rightKey = KeyCode.D;   // D�L�[���E�Ƃ���

    [SerializeField]
    private KeyCode decisionKey = KeyCode.Space;    // �X�y�[�X�L�[������{�^���Ƃ���

    [Header("----- �� �V�[������� -----"), Space(5)]  // �K���C���X�y�N�^��Ŏw��

    [Header("���e�V�[���̖��O")]
    [Tooltip("�^�C�g���ɂ���V�[���̖��O�ɕύX���������珑��������")]
    [SerializeField] private string titleSceneName;

    [Tooltip("�X�e�[�W�I���ɂ���V�[���̖��O�ɕύX���������珑��������")]
    [SerializeField] private string selectSceneName;

    [Tooltip("���U���g�ɂ���V�[���̖��O�ɕύX���������珑��������")]
    [SerializeField] private string resultSceneName;

    [Header("���X�e�[�W�V�[���̖��O")]
    [Tooltip(
        "(���K�{����)\n" +
        "�X�e�[�W�ԍ��ȊO�̃V�[���������\n" +
        "�V�[���̖��O�ɕύX���������珑��������")]
    [SerializeField] private string stageName;

    [Tooltip("���݂��Ă���ړ��������X�e�[�W�ԍ������")]
    [SerializeField] private int stageNum;

    // �J�n���Ƀ��������m�ۂ���
    public static S_Manager_old instance = null;

    // �S�[���̔�����擾����
    public static bool gameClear;

    //==================================================
    // �@�@�@�@�@ (�V���O���g���̐���)
    // �����������݂��Ă��Ȃ��Ƃ��ɂP�o��������
    // ���t�ɂQ�ȏ㑶�݂���Ƃ��͂P�����c���悤�ɂ���
    //==================================================
    // �쐬��2023/02/17
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

    //==================================================
    // ���X�e�[�W�ԍ����ő�܂��͍Œ�l�ɂȂ����Ƃ�
    // �@�J��Ԃ���,�ő�,�Œ�l�Ŏ~�߂邩�̑I��p�񋓌^
    // ==================================================
    private enum StageNum
    {//----- enum_start -----
        // �~�߂邩�J��Ԃ����̐ݒ�
        stop,   // ���l�̒[�Ŏ~�߂�
        repeat, // ���l�̒[�𒴂���ƃ��[�v����
        
        // ���ݒ�̏��
        none,
    }//----- enum_stop -----

    // �V�[�����X�g���쐬
    private enum SceneList
    {//----- enum_start -----
        // �^�C�g�����
        title = 0,
        select = 1,
        //==================================================
        // �@�@�@�@�i�Q�[���v���C��ʃV�[�����X�g�j
        // ���X�e�[�W�ǉ����Ƃɔԍ����ɋL�����Ă�������
        // �����[���h���Ƃ�world_[n]
        //   [n] x 10�̐��l��ݒ肵�܂�
        //==================================================
        // world_1
        stage1 = 10,
        stage2,
        stage3,
        stage4,
        stage5,
        // world_2
        stage6 = 20,
        stage7,
        // ���U���g���
        result = 300,

        // ���ݒ�̏��
        none = 500,
    }//----- enum_stop -----

    // �X�e�[�W�ԍ��̍ŏ��l,�ő�l======================
    // �X�e�[�W�ǉ���폜�Ȃǔԍ���
    // �ύX�ɍ��킹�Ă����̐��l��ύX���Ă�������
    [Header("���X�e�[�W�ԍ��̍ŏ�,�ő�l�̓���")]

    // �C���X�y�N�^�ォ��K���P�ȏ�̒l�Őݒ肵�Ă�������
    [Tooltip("�ړ��������ŏ��X�e�[�W�ԍ����P�ȏ�̒l�Őݒ�")]
    [SerializeField] private int minStageNum;

    // �X�e�[�W�ǉ���폜�̓x�ɃC���X�y�N�^���珑�������Ă�������
    [Tooltip("�ړ��������ő�X�e�[�W�ԍ��𑶍݂��鐔�l���Őݒ�")]
    [SerializeField] private int maxStageNum;
    //==================================================

    // �^�C�g��,���U���g�����ԃV�[�� =================
    // ���ɑJ�ڂ���V�[���̃��X�g
    private enum NextSceneList
    {//----- enum_start -----
        title,
        select,
        result,
        stage,
        none,
    }//----- enum_stop -----

    // �^�C�g���V�[������J�ڂ���V�[����I��
    [Header("���e�V�[������ړ�����V�[����I��")]
    [Tooltip("�^�C�g���V�[�����玟�Ɉڍs����V�[����ݒ�")]
    [SerializeField] private NextSceneList titleNextScene = NextSceneList.none;

    // ���U���g�V�[������J�ڂ���V�[����I��
    [Tooltip("���U���g�V�[�����玟�Ɉڍs����V�[����ݒ�")]
    [SerializeField] private NextSceneList resultNextScene = NextSceneList.none;

    //==================================================

    // �X�e�[�W�ԍ����ő�,�ŏ��l�ɂȂ����Ƃ��ǂ�������������Ƃ邩
    [SerializeField, Header("���X�e�[�W�ԍ��̐ݒ�")]
    [Tooltip("�ԍ��̒l�����E�ɒB�����Ƃ��̓���̐ݒ�")]
    private StageNum stageNumStatus = StageNum.none;

    // �V�[���ʒu�̊m�F=================================
    // �C���X�y�N�^�[��ō�����V�[�����m�F�ł���
    [Header("----- ��Ԋm�F�p -----"),Space(5)]  // �ȉ���Ԃ̊m�F���C���X�y�N�^��ŉ\
    [SerializeField, Header("���݂̃V�[��")]
    [Tooltip("���݃A�N�e�B�u�ɂȂ��Ă���V�[���������ɕ\������܂�")]
    private string sceneNameNow = "None";

    // �C���X�y�N�^�[��łP�O�ɂ����V�[�����m�F�ł���
    [SerializeField,Header("�P�O�̃V�[��")]
    [Tooltip("�P�O�ɃA�N�e�B�u�ɂȂ��Ă����V�[���������ɕ\������܂�")]
    private string sceneNameOld = "None";
    //==================================================

    // GameManager�̎d�l�܂Ŕ�\��---------------------------------------------
    // �C���X�y�N�^�[�ォ�猻�ݑI�����Ă���V�[�����m�F
    //[SerializeField, Header("�I�𒆂̃V�[��")]
    private SceneList sceneSelect = SceneList.none;
    // �C���X�y�N�^�[�ォ��1�O�ɂ����V�[�����m�F�ł���
    //[SerializeField, Header("�O��̃V�[��")]
    //private SceneList sceneLog = SceneList.none;

    // �I�𒆂̃X�e�[�W�ԍ�
    [SerializeField,Header("�I�𒆂̃X�e�[�W�ԍ�")]
    [Tooltip("���ݑI������Ă���X�e�[�W�ԍ��������ɕ\������܂�")]
    private int m_selectStageNum;

    // �R�}���h�p(��ŏ����܂�)

    // �e���͌�̐�������
    private int LD, BW;

    // �쐬��2023/02/17
    // �{��
    //�T���v���V�[���ɖ߂�
    private void HideCommandRoom()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ����null
        if (Gamepad.current == null) return;

        // L�X�e�B�b�N�̉��𗣂�������60�̗P�\
        if (Gamepad.current.leftStick.down.wasReleasedThisFrame)
        {//----- if_start -----
            LD = 60;
        }//----- if_stop -----
         // X�{�^��������������60�̗P�\
        if (Gamepad.current.buttonWest.wasPressedThisFrame && (LD > 0))
        {//----- if_start -----
            BW = 60;
        }//----- if_stop -----
         // B�{�^�������������ƃT���v���V�[���ɖ߂�
        if (Gamepad.current.buttonEast.wasPressedThisFrame && (BW > 0))
        {//----- if_start -----
         SceneManager.LoadScene("SampleScene");
        }//----- if_stop -----

        // �e����̐������Ԃ����炷
        if(LD >= 0)
        {//----- if_start -----
            LD--;
        }//----- if_stop -----

        if (BW >= 0)
        {//----- if_start -----
            BW--;
        }//----- if_stop -----
    }

    // �쐬��2023/02/18
    // �{��
    private void SelectSceneCommandRoom()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ����null
        if (Gamepad.current == null) return;

        if(Gamepad.current.leftTrigger.wasPressedThisFrame &&
            Gamepad.current.rightTrigger.wasPressedThisFrame)
        {
            SceneManager.LoadScene("select");
        }
    }
    // �����܂ŃR�}���h

    //==================================================
    // �@�@�@�X�e�[�W�ԍ��̒�~�ƌJ��Ԃ�����
    // ��[stageNumStatus]�ɂ���Đݒ肳��Ă���ϐ���
    // �@�g�p���ăX�e�[�W�ԍ��̐��l���ȉ��̓����ɕϓ�������
    //==================================================
    // �����2023/02/17
    // �{��
    private void StopOrRepeatOfTheStageSelectNum()
    {
        // [minStageNum]/[maxStageNum]��
        // �������ݒ肳��Ă��Ȃ��ꍇ�G���[���o��
        if (minStageNum < 1)
        {//----- if_start -----
            Debug.LogError("[minStageNum]���������ݒ肳��Ă��܂���I");
        }//----- if_stop -----
        //if (maxStageNum)
        //{
        //    Debug.LogError("[maxStageNum]���������ݒ肳��Ă��܂���I");
        //}

        // [stageNumStatus]�̏�Ԃɂ���ē�����ύX����
        switch (stageNumStatus)
        {//----- switch_start -----
            // ���l���ő�,�ŏ��𒴂��Ȃ��悤�ɂ���
            case StageNum.stop:
                if (m_selectStageNum < minStageNum)
                {//----- if_start -----
                    m_selectStageNum = minStageNum;
                }//----- if_stop -----
                if (m_selectStageNum > maxStageNum)
                {//----- if_start -----
                    m_selectStageNum = maxStageNum;
                }//----- if_stop -----
                break;
            // ���l���ő�,�ŏ��𒴂�����,���]���ČJ��Ԃ�
            case StageNum.repeat:
                if (m_selectStageNum < minStageNum)
                {//----- if_start -----
                    m_selectStageNum = maxStageNum;
                }//----- if_stop -----
                if (m_selectStageNum > maxStageNum)
                {//----- if_start -----
                    m_selectStageNum = minStageNum;
                }//----- if_stop -----
                break;

            // [stageNumStatus]���������ݒ肳��Ă��Ȃ��ꍇ�G���[�\�L
            case StageNum.none:
            default:
                Debug.LogError("[stageNumStatus]���������ݒ肳��Ă��܂���I");
                break;
        }//----- switch_stop -----

    }

    //==================================================
    // �R���g���[���[����
    // ���X�e�B�b�N��|���Ă���Ԃ�(Lstick_Left,Right)�ɐ^���o�͂���
    // ���^���o�͂���Ă���ԑI�𒆂̃X�e�[�W�ԍ���ύX����
    //==================================================
    // �����2023/02/17
    // �{��
    private void StageSelectWithController()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ����null
        if (Gamepad.current == null) return;

        // L�X�e�B�b�N�����ɂ�������Ă����
        // Lstick_Left��true�ɂ���
        if (Gamepad.current.leftStick.left.wasPressedThisFrame)
        {//----- if_start -----
            Lstick_Left = true;
        }//----- if_stop -----
        else if(Gamepad.current.leftStick.left.wasReleasedThisFrame)
        {//----- elseif_start -----
            Lstick_Left = false;
        }//----- elseif_stop -----

        // L�X�e�B�b�N���E�ɂ�������Ă����
        // Lstick_Right��true�ɂ���
        if (Gamepad.current.leftStick.right.wasPressedThisFrame)
        {//----- if_start -----
            Lstick_Right = true;
        }//----- if_stop -----
        else if (Gamepad.current.leftStick.right.wasReleasedThisFrame)
        {//----- elseif_start -----
            Lstick_Right = false;
        }//----- elseif_stop -----

        // �\���{�^�������ɂ�����Ă����
        // dpad_Left��true�ɂ���
        if (Gamepad.current.dpad.left.wasPressedThisFrame)
        {//----- if_start -----
            dpad_Left = true;
        }//----- if_stop -----
        else if (Gamepad.current.dpad.left.wasReleasedThisFrame)
        {//----- elseif_start -----
            dpad_Left = false;
        }//----- elseif_stop -----

        // �\���{�^�����E�ɂ�����Ă����
        // dpad_Right��true�ɂ���
        if (Gamepad.current.dpad.right.wasPressedThisFrame)
        {//----- if_start -----
            dpad_Right = true;
        }//----- if_stop -----
        else if (Gamepad.current.dpad.right.wasReleasedThisFrame)
        {//----- elseif_start -----
            dpad_Right = false;
        }//----- elseif_stop -----
    }

    //==================================================
    // ���R���g���[���\��L�[�{�[�h����ɍ��킹��
    // �@�I���ʒu�̕ύX���s��
    //==================================================
    // �����2023/02/17
    // �{��
    private void ChangeSelectPosition()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ����null
        if (Gamepad.current == null) return;

        // world_1�̋󂫃X�e�[�W���΂�����
        // stage1�ȏ�,stage5�ȉ��̂Ƃ��Ɏ��{
        // ���s������
        //=�e�X�g���{=======================================

        if ((sceneSelect >= SceneList.stage1) || (sceneSelect <= SceneList.stage5))
        {//----- if_start -----
            if (Lstick_Left)
            {//----- if_start -----
                sceneSelect--;
            }//----- if_stop -----
            if (Lstick_Right)
            {//----- if_start -----
                sceneSelect++;
            }//----- if_stop -----
        }//----- if_stop -----
        if ((sceneSelect >= SceneList.stage6) || (sceneSelect <= SceneList.stage7))
        {//----- if_start -----
            if (Lstick_Left)
            {//----- if_start -----
                sceneSelect--;
            }//----- if_stop -----
            if (Lstick_Right)
            {//----- if_start -----
                sceneSelect++;
            }//----- if_stop -----
        }//----- if_stop -----

        //==================================================
    }

    //==================================================
    // �R���g���[���[����
    // ������{�^�����������Ƃ��ɃV�[����ύX����
    //==================================================
    // �����2023/02/17
    // �{��
    private void ChangeSceneWithControllerDecisionButton()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ����null
        if (Gamepad.current == null) return;

        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {//----- if_start -----
            // �V�[����[selectStageNum]�Ŏw�肵���X�e�[�W�ɕύX����
            SceneManager.LoadScene("stage" + m_selectStageNum);
            // ���ݎg�p���Ă���V�[�����L�^����
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    //==================================================
    // �L�[�{�[�h����
    // ������L�[���������Ƃ��ɃV�[����ύX����
    //==================================================
    // �����2023/02/17
    // �{��
    private void ChangeSceneWithDecisionKey()
    {
        if (Input.GetKeyDown(decisionKey))
        {//----- if_start -----
            // �V�[����[selectStageNum]�Ŏw�肵���X�e�[�W�ɕύX����
            SceneManager.LoadScene("stage" + m_selectStageNum);
            // ���ݎg�p���Ă���V�[�����L�^����
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    //==================================================
    // �R���g���[���[����
    // ���X�e�B�b�N,�\���{�^������ɂ���Ďw��X�e�[�W�ԍ���ύX����
    //==================================================
    // �����2023/02/17
    // �{��
    private void ChangeStageNumberWithController()
    {
        // ���X�e�B�b�N�����ɓ|�����Ƃ���\���{�^���̍��������ꂽ�Ƃ��Ɏ��s
        if (Gamepad.current.leftStick.left.wasPressedThisFrame ||
            Gamepad.current.dpad.left.wasPressedThisFrame)
        {//----- if_start -----
            m_selectStageNum--;
        }//----- if_stop -----
        // ���X�e�B�b�N���E�ɓ|�����Ƃ���\���{�^���̉E�������ꂽ�Ƃ��Ɏ��s
        if (Gamepad.current.leftStick.right.wasPressedThisFrame ||
            Gamepad.current.dpad.right.wasPressedThisFrame)
        {//----- if_start -----
            m_selectStageNum++;
        }//----- if_stop -----
    }

    //==================================================
    // �L�[�{�[�h����
    // ��A,D�L�[,���E�A���[�L�[����ɂ���Ďw��X�e�[�W�ԍ���ύX����
    //==================================================
    // �����2023/02/17
    // �{��
    private void ChangeStageNumberWithKeyboard()
    {
        // �����͂����ꂽ�Ƃ��Ɏ��s
        if (Input.GetKeyDown(leftArrow) || Input.GetKeyDown(leftKey))
        {//----- if_start -----
            m_selectStageNum--;
        }//----- if_stop -----
        // �E���͂����ꂽ�Ƃ��Ɏ��s
        if (Input.GetKeyDown(rightArrow) || Input.GetKeyDown(rightKey))
        {//----- if_start -----
            m_selectStageNum++;
        }//----- if_stop -----
    }

    //==================================================
    //      [NextScene]�����ׂ�V�[����I��
    //==================================================
    // �쐬��2023/02/20    �X�V��2023/02/21
    // �{��
    private void EnumSceneList()
    {
        if (SceneManager.GetActiveScene().name == titleSceneName)
        {
            switch (titleNextScene)
            {//----- switcj_start -----
             // �^�C�g������^�C�g���Ɉړ����悤�Ƃ���ƃG���[���o��
                case NextSceneList.title:
                    Debug.LogError("�������O�̃V�[���ɔ�ڂ��Ƃ��Ă��܂��I");
                    break;
                case NextSceneList.select:
                    SceneManager.LoadScene("" + selectSceneName);
                    break;
                case NextSceneList.result:
                    SceneManager.LoadScene("" + resultSceneName);
                    break;
                case NextSceneList.stage:
                    SceneManager.LoadScene(stageName + stageNum);
                    break;
                case NextSceneList.none:
                default:
                    Debug.LogError("[titleNextScene]���������ݒ肳��Ă��܂���I");
                    break;
            }//----- switcj_stop -----
        }

        if (SceneManager.GetActiveScene().name == resultSceneName)
        {
            switch (resultNextScene)
            {//----- switcj_start -----
             // ���U���g���烊�U���g�Ɉړ����悤�Ƃ���ƃG���[���o��
                case NextSceneList.title:
                    SceneManager.LoadScene("" + titleSceneName);
                    break;
                case NextSceneList.select:
                    SceneManager.LoadScene("" + selectSceneName);
                    break;
                case NextSceneList.result:
                    Debug.LogError("�������O�̃V�[���ɔ�ڂ��Ƃ��Ă��܂��I");
                    break;
                case NextSceneList.stage:
                    SceneManager.LoadScene(stageName + stageNum);
                    break;
                case NextSceneList.none:
                default:
                    Debug.LogError("[resultNextScene]���������ݒ肳��Ă��܂���I");
                    break;
            }//----- switcj_stop -----
        }
    }

    //=�e�X�g���{=======================================

    // �쐬��2023/02/18
    // �{��
    private void ControlForDecisionWithController()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ����null
        if (Gamepad.current == null) return;

        // ����{�^���������ꂽ��X�e�[�W�I����
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {//----- if_start -----
            EnumSceneList();
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    // �쐬��2023/02/17
    // �{��
    private void ControlForDecisionWithKeyboard()
    {
        // ����L�[�������ꂽ��X�e�[�W�I����
        if (Input.GetKeyDown(decisionKey))
        {//----- if_start -----
            EnumSceneList();
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    //==================================================

    //==================================================
    //      ���L�G���[���O��\��
    // ���K�v���ڂ�string���ɕs��������ꍇ
    // ���K�v���ڂ̎��Ɉړ�����V�[���ɕs��������ꍇ
    //==================================================
    // �쐬��2023/02/21
    // �{��
    private void ErrorLog()
    {
        // �V�[���̖��O���L������Ă��Ȃ��ꍇ�ɃG���[��\��������
        // �^�C�g���V�[�����L���G���[
        if (titleSceneName == "")
        {//----- if_start -----
            Debug.LogError("[titleSceneName]���L������Ă��܂���I");
        }//----- if_stop -----

        // �X�e�[�W�Z���N�g���L���G���[
        if (selectSceneName == "")
        {//----- if_start -----
            Debug.LogError("[selectSceneName]���L������Ă��܂���I");
        }//----- if_stop -----

        // ���U���g�V�[�����L���G���[
        if (resultSceneName == "")
        {//----- if_start -----
            Debug.LogError("[resultSceneName]���L������Ă��܂���I");
        }

        // �����V�[���Ɉړ�����悤�ɐݒ肳��Ă����ꍇ�G���[��\��������
        if (titleNextScene == NextSceneList.title ||
            resultNextScene == NextSceneList.result)
        {//----- if_start -----
            Debug.LogError("�������O�̃V�[���ɔ�ڂ��Ƃ��Ă��܂��I");
        }//----- if_stop -----

        if (titleNextScene == NextSceneList.none)
        {//----- if_start -----
            Debug.LogError("[titleNextScene]���ړ�����V�[�����ݒ肳��Ă��܂���I");
        }//----- if_stop -----

        if (resultNextScene == NextSceneList.none)
        {//----- if_start -----
            Debug.LogError("[resultNextScene]���ݒ肳��Ă��܂���I");
        }//----- if_stop -----
    }

    private void Update()
    {
        //=�e�X�g���{=======================================

        // ������V�[���̖��O��[title]�܂���[result]�Ȃ��p�̑���ɕύX
        if (SceneManager.GetActiveScene().name == "title" ||
            SceneManager.GetActiveScene().name == "result")
        {//----- if_start -----
            // �Q�[���p�b�h���ڑ�����Ă��Ȃ��ꍇ�̓L�[�{�[�h����ɂ���
            // �Q�[���p�b�h���ڑ�����Ă���ꍇ�̓R���g���[���[����ɂ���
            if (Gamepad.current == null)
            {//----- if_start -----
                ControlForDecisionWithKeyboard();
            }//----- if_stop -----
            else
            {//----- else_start -----
                ControlForDecisionWithController();
            }//----- else_stop -----
        }//----- if_stop -----


        // ��[m_selectStageNum]�̑���Ɠ����ɓ��͂����
        // �@�X�e�[�W�V�[�����痣�E�ł��Ȃ��o�O���������܂�
        // ������V�[���̖��O��[stage(selectStageNum)]�Ȃ��p�̑���ɕύX
        if (SceneManager.GetActiveScene().name == stageName + m_selectStageNum)
        {//----- if_start -----

            // ����{�^���������ꂽ��X�e�[�W�I����
            if (gameClear)
            {//----- if_start -----
                SceneManager.LoadScene(resultSceneName);
                sceneNameOld = SceneManager.GetActiveScene().name;
            }//----- if_stop -----

        }//----- if_stop -----

        //==================================================

        // ������V�[���̖��O��[select]�܂���[SampleScene]�Ȃ��p�̑���ɕύX
        if (SceneManager.GetActiveScene().name == "select" ||
            SceneManager.GetActiveScene().name == "SampleScene")
        {//----- if_start -----
            // �Q�[���p�b�h���ڑ�����Ă��Ȃ��ꍇ�̓L�[�{�[�h����ɂ���
            // �Q�[���p�b�h���ڑ�����Ă���ꍇ�̓R���g���[���[����ɂ���
            if (Gamepad.current == null)
            {//----- if_start -----
                // ����L�[�ŃV�[����؂�ւ���
                ChangeSceneWithDecisionKey();
                // ���E�̖��,AD�L�[����ŃX�e�[�W�ԍ���ύX����
                ChangeStageNumberWithKeyboard();
            }//----- if_stop -----
            else
            {//----- else_start -----
                // ����{�^���ŃV�[����؂�ւ���
                ChangeSceneWithControllerDecisionButton();
                // �X�e�B�b�N,�\���L�[����ŃX�e�[�W�ԍ���ύX����
                ChangeStageNumberWithController();
                // �X�e�B�b�N��������Ă����Ԃ��m�F����
                StageSelectWithController();
            }//----- else_stop -----
        }//----- if_stop -----

        // ���ݎg�p���Ă���V�[����������������
        sceneNameNow = SceneManager.GetActiveScene().name;

        // �R���g���[���p�T���v���V�[���J�ڃR�}���h
        HideCommandRoom();

        //=�e�X�g���{=======================================

        // ���݂̃V�[������[select]�ł͂Ȃ��Ƃ�
        // ZR,ZL�������ɉ����ꂽ�Ȃ�X�e�[�W�I����ʂɑJ��
        if (SceneManager.GetActiveScene().name != "select")
        {//----- if_start -----
            SelectSceneCommandRoom();
        }//----- if_stop -----

        // ���݂̃V�[������[select]�̂Ƃ�
        // B�{�^��,S�L�[�������ꂽ�Ȃ�[title]�֑J��
        if (SceneManager.GetActiveScene().name == "select")
        {//----- if_start -----
            if (Gamepad.current == null)
            {//----- if_start -----
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SceneManager.LoadScene("title");
                    sceneNameOld = SceneManager.GetActiveScene().name;
                }
            }//----- if_stop -----
            else
            {//----- else_start -----
                if(Gamepad.current.buttonEast.wasPressedThisFrame)
                {
                    SceneManager.LoadScene("title");
                    sceneNameOld = SceneManager.GetActiveScene().name;
                }
            }//----- else_stop -----
        }//----- if_stop -----

        //==================================================

        // �X�e�[�W�ԍ��I�����̐ݒ�
        StopOrRepeatOfTheStageSelectNum();

        //----- �e�X�g�i�K�ňꎞ��~�� -----
        //StageSelectWithController();
        //ChangeSelectPosition();

        // �G���[�̕\��
        ErrorLog();
    }
}
