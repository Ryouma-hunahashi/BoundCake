using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//======================= ver2.0 ============================
// �X�e�[�W�Z���N�g���ɁA�I�񂾃X�e�[�W�̉摜���Y�[������悤�ɂ��܂����B
// ����֘A�ŁAWorldStatus�\���̗̂v�f��ǉ����܂����B
//===========================================================
// ���s���@2023/05/03   �X�V��2023/05/21
// ���c�@�{��

//[System.Serializable]
//public struct WorldStatus
//{
//    [System.Serializable]
//    public struct StageStatus
//    {
//        // �Y�[�����s����
//        public enum ZoomSet
//        {
//            ON,
//            OFF,
//        }

//        [Tooltip("�ꏊ�̖��O")]
//        public string stageName;
//        [Tooltip("�V�[����")]
//        public string sceneName;
//        [Tooltip("�J�������Œ肷����W\nImage����̑��΍��W")]
//        public Vector3 cameraZoomPos;
//        [Tooltip("�Ή�����Image�ɃY�[�����s�����ۂ�")]
//        public ZoomSet zoomSet;
//    }


//    [Tooltip("�n�}�̖��O")]
//    public string worldName;            // �n�}����ݒ肷��
//    [Tooltip("�ꏊ�̏��")]
//    public List<StageStatus> stages;    // �ꏊ�̏���ݒ肷��

//}

// WorldStatus�ɑΉ�����Image�̏��i�[�p
[System.Serializable]
public struct MapImages
{
    public Image image;         // Image�i�[�p
    public Animator animator;   // Image��Animator�i�[�p
    public Vector3 transPos;    // Image��position�i�[�p
    public testMSelect mSelect;
}

[System.Serializable]
public struct SelectorImages
{
    public Image image;
    public Animator animator;
    public Vector3 transPos;
}

//==============================
//      �X�e�[�W�Z���N�g
//==============================
// �쐬��2023/04/27    �X�V��2023/05/21
// �{���@���c
public class StageSelector : MonoBehaviour
{
    // �f�[�^�����̐ݒ�
    //[SerializeField] private S_Manager s_Manager;

    // �I���ʒu�̏��
    public byte worldNum = 1;
    public byte stageNum = 0;

    // �ʒu�ύX���x�̐ݒ�
    [SerializeField] private byte selectDelayTime = 20;
    [SerializeField] private float zoomSpeed = 30.0f;
    private bool nowDelayTime;  // �ҋ@���

    // �n�}�̏��
    //public List<WorldStatus> worlds = new List<WorldStatus>();

    public List<Stage_Manager.WorldInfo> mapInfo;

    [Header("��O�̃V�[���̖��O")]
    [Tooltip("�Y�[�����Ă��Ȃ���Ԃœ��{�^���������ƃV�[���J��")]
    [SerializeField] private string backSceneName = "";
    [Header("���̃Z���N�g���")]
    [SerializeField] private string rightSceneName = "";
    [Header("�O�̃Z���N�g���")]
    [SerializeField] private string leftSceneName = "";

    // ���̃I�u�W�F��RectTransform�i�[�p
    private RectTransform canvasTransform = null;

    // stageImage�֘A�̏��i�[�p
    [SerializeField] private List<MapImages> mapImages = new List<MapImages>();
    [SerializeField] private List<SelectorImages> selectorImages = new List<SelectorImages>();

    // �O���[�X�P�[���ϊ��p�J�[�u
    [SerializeField] private int grayColor = 25;
    private GameObject cameraObj = null;

    private Vector3 defaultCameraPos;

    private bool cameraZoomFlag = false;
    private bool nowZoomOut = false;
    private bool nowZoom = false;
    private bool sceneMoveSoundSet = true;

    [SerializeField] private AnimationCurve fadeOutCurve;
    private Image fadeOutImage;
    private float elapsedTime;
    [SerializeField] private float fadeOutTime = 1.0f;
    private bool nowfadeOut = false;
    private bool nowStageSet = false;
    Image stageSettingImage;
    private byte settingNum = 0;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectColor;


    UIAudio audio;

    //==============================
    //      �I���ʒu�ύX�̃f�B���C
    // �߂�l : ����
    //�@����  : ����
    //==============================
    // �쐬��2023/04/27
    // �{��
    private IEnumerator ChangeDelay()
    {
        // �ҋ@��ԂɕύX����
        nowDelayTime = true;

        // �ݒ肳�ꂽ�t���[�����ҋ@����
        for (byte i = 0; i < selectDelayTime; i++)
        {
            // 1�t���[���x��������
            yield return null;
        }//----- for_stop -----

        // ��ҋ@��ԂɕύX����
        nowDelayTime = false;
    }

    private void Start()
    {
        // �f�[�^�̑�����T��
        //s_Manager = GetComponent<S_Manager>();

        mapInfo = Stage_Manager.instanse.worldInformation;

        worldNum = 1;
        stageNum = 0;

        // �q�I�u�W�F�N�g�̐����擾
        byte childCnt = (byte)this.transform.childCount;

        // ��x���X�g���J������
        mapImages.Clear();

        canvasTransform = GetComponent<RectTransform>();
        if (canvasTransform == null)
        {
            Debug.LogError("RectTransform���R���|�[�l���g����Ă��܂���B");
        }

        cameraObj = GameObject.FindWithTag("MainCamera");
        if (cameraObj == null)
        {
            Debug.LogError("�J������������܂���");
        }

        // �q�I�u�W�F�N�g��<Image>�����X�g���Ɋi�[
        for (byte i = 0; i < childCnt; i++)
        {
            // ���X�g���Ɏq�I�u�W�F�N�g��<Image>��ǉ��i�[����B
            mapImages.Add(SetImages(i));
            if (mapInfo[i].worldLock)
            {
                mapImages[i].image.color = new Color(grayColor/255f,grayColor/255f,grayColor/255f,1);
            }


        }//----- for_stop -----
        for (byte i = 0; i < cameraObj.transform.GetChild(0).childCount; i++)
        {
            // ���X�g���ɃJ�������I�u�W�F�N�g��<Image>��ǉ��i�[����B
            selectorImages.Add(SetSelectorImages(i));
        }

        audio = GetComponent<UIAudio>();



        // �����J�������W����
        if (defaultCameraPos == null)
        {
            defaultCameraPos = cameraObj.transform.position;
        }

        if (mapImages[worldNum].mSelect != null)
        {
            mapImages[worldNum].mSelect.Enter();
        }
        else
        {
            mapImages[worldNum].image.color = selectColor;
        }

        fadeOutImage = cameraObj.transform.GetChild(1).GetChild(0).GetComponent<Image>();


    }




    private void FixedUpdate()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ��Ȃ珈���𔲂���
        if (Gamepad.current == null) return;

        // ���[�J���ϐ��̍쐬 & �������J�n ----------------

        // �ړ������̓��͏����擾����
        float inputVertical = Input.GetAxisRaw("Vertical");
        float inputHorizontal = Input.GetAxisRaw("Horizontal");

        //// �X�e�B�b�N�̓��͏����擾���� ----- Vertical
        //bool Lstick_Up = Gamepad.current.leftStick.up.wasPressedThisFrame;
        //bool Lstick_Down = Gamepad.current.leftStick.down.wasPressedThisFrame;

        //// �X�e�B�b�N�̓��͏����擾���� ----- Horizontal
        //bool Lstick_Left = Gamepad.current.leftStick.left.wasPressedThisFrame;
        //bool Lstick_Right = Gamepad.current.leftStick.right.wasPressedThisFrame;

        // �{�^���̓��͏����擾����
        bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame; // ��{�^���̓��͏��
        bool eastButton = Gamepad.current.buttonEast.wasPressedThisFrame;  // ���{�^���̓��͏��

        bool leftButton = Gamepad.current.leftShoulder.wasPressedThisFrame || Gamepad.current.leftTrigger.wasPressedThisFrame;
        bool rightButton = Gamepad.current.rightShoulder.wasPressedThisFrame || Gamepad.current.rightTrigger.wasPressedThisFrame;

        // ���[�J���ϐ��̍쐬 & ���������� ----------------

        //==============================
        // ���[���h�ʒu�����炷
        //==============================
        // �ҋ@��Ԃł͂Ȃ��Ƃ���[L]�{�^���������ꂽ�Ȃ�
        if (leftButton && !nowDelayTime && !cameraZoomFlag && !nowZoomOut && !nowZoom)
        {
            // �V�[���̖��O���o�^����Ă����
            if (leftSceneName != "")
            {
                // �V�[���J�ڂ���
                S_Manager.instance.SceneChange(leftSceneName);

            }//-----if_stop-----

            //// �n�}�ԍ����Œ�l�ł͂Ȃ��Ȃ�
            //if (worldNum != 0)
            //{
            //    // ���݂̃A�j���[�V������؂�
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", false);

            //    worldNum--;
            //    stageNum = 0;

            //    // ���̔ԍ��̃A�j���[�V�������N��
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", true);
            //    // �ʒu�ύX���ꎞ�I�ɒ�~����
            //    StartCoroutine(ChangeDelay());

            //}//----- if_stop -----

        }//----- if_stop -----
        // �ҋ@��Ԃł͂Ȃ��Ƃ���[R]�{�^���������ꂽ�Ȃ�
        else if (rightButton && !nowDelayTime && !cameraZoomFlag && !nowZoomOut && !nowZoom)
        {
            // �V�[���̖��O���o�^����Ă����
            if (rightSceneName != "")
            {
                // �V�[���J�ڂ���
                S_Manager.instance.SceneChange(rightSceneName);

            }//-----if_stop-----



            //// �n�}�ԍ����ő�l�ł͂Ȃ��Ȃ�
            //if (worldNum < (byte)worlds.Count - 1)
            //{
            //    // ���݂̃A�j���[�V������؂�
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", false);

            //    worldNum++;
            //    stageNum = 0;

            //    // ���̔ԍ��̃A�j���[�V�������N��
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", true);

            //}//----- if_stop -----

            //// �ʒu�ύX���ꎞ�I�ɒ�~����
            //StartCoroutine(ChangeDelay());

        }//----- elseif_stop -----

        //==============================
        // �X�e�[�W,���[���h�ʒu�����炷
        //==============================
        // �ҋ@��Ԃł͂Ȃ��Ƃ��ɉ������̓��͂����ꂽ�Ȃ�
        if (!nowDelayTime && (inputHorizontal != 0) && !nowZoom && !nowfadeOut)
        {
            // �w��ʒu�̕ύX
            ChangeHorizontalPosition(inputHorizontal);    // �����͂𑗂�

        }//----- if_stop -----
        //else if(!nowDelayTime && (inputVertical != 0) && !nowZoom)
        //{
        //    // �w��ʒu�̕ύX
        //    ChangeVerticalPosition(inputVertical);        // �c���͂𑗂�
        //}

        // ����{�^�������͂��ꂽ�Ȃ���s
        if (southButton)
        {
            switch (mapInfo[worldNum].zoomSet)
            {
                case Stage_Manager.WorldInfo.ZoomSet.ON:
                    if (cameraZoomFlag && !nowZoom && !nowfadeOut)
                    {
                        audio.StartSound();
                        // --------------------------�Q�[���J�n���̃T�E���h--------------
                        nowfadeOut = true;

                    }//-----if_stop-----
                    else
                    {
                        // �J�����𓮂���
                        audio.BookMarkSound();
                        nowZoom = true;
                        cameraZoomFlag = true;

                    }//-----else_stop-----
                    break;
                case Stage_Manager.WorldInfo.ZoomSet.OFF:
                    if (!nowfadeOut)
                    {
                        nowfadeOut = true;
                    }
                    break;
            }

        }//----- if_stop -----

        if (eastButton)
        {
            if (!nowZoom && cameraZoomFlag)
            {
                //--------------------------�x�̃T�E���h--------------------------
                audio.BookMarkSound();
                for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                {
                    selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", false);
                }
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                stageNum = 0;
                cameraZoomFlag = false;
                nowZoomOut = true;
            }
            else if (!cameraZoomFlag && !nowZoom && !nowZoomOut)
            {
                if (backSceneName != "")
                {
                    S_Manager.instance.SceneChange(backSceneName);
                }
            }
        }

        if (nowZoom)
        {
            cameraObj.transform.position = Vector3.MoveTowards(cameraObj.transform.position, mapImages[worldNum].transPos + mapInfo[worldNum].cameraZoomPos, zoomSpeed);

            // �J�������ڕW�ɒB�����Ƃ��A�Y�[�����t���O��؂�B
            if (cameraObj.transform.position == mapImages[worldNum].transPos + mapInfo[worldNum].cameraZoomPos)
            {
                for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                {
                    if (!mapInfo[worldNum].stageInformation[i].stageLock)
                    {
                        selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", true);
                    }
                }
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                nowZoom = false;

            }//-----if_stop-----

        }//-----if_stop-----
        else if (nowZoomOut)
        {
            cameraObj.transform.position = Vector3.MoveTowards(cameraObj.transform.position, defaultCameraPos, 30);

            // �J�������ڕW�ɒB�����Ƃ��A�Y�[���A�E�g���t���O��؂�B
            if (cameraObj.transform.position == defaultCameraPos)
            {

                nowZoomOut = false;
            }
        }

        if (nowfadeOut)
        {
            var color = fadeOutImage.color;
            color.a = fadeOutCurve.Evaluate(elapsedTime / fadeOutTime);
            fadeOutImage.color = color;

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= fadeOutTime)
            {
                elapsedTime = fadeOutTime;
                if (sceneMoveSoundSet)
                {
                    //----------------�V�[���J�ڂ̉�
                    audio.ChangeSceneSound();
                    sceneMoveSoundSet = false;
                }
                // �����I����Ă���
                if (!audio.UISource.isPlaying)
                {
                    sceneMoveSoundSet = true;
                    nowfadeOut = false;
                    elapsedTime = 0;
                    Debug.Log((worldNum + 1) + " - " + (stageNum + 1) + " �ɔ�т܂����I");

                    Result_Manager.instance.nowWorld = worldNum; // ���[���h�ԍ���ۑ�
                    Result_Manager.instance.nowStage = stageNum; // �X�e�[�W�ԍ���ۑ�

                    // �V�[���J�ڂ��s��
                    S_Manager.instance.SceneChange(mapInfo[worldNum].stageInformation[stageNum].sceneName);
                }
            }
        }


        if (nowStageSet)
        {
            var color = stageSettingImage.color;
            color.r += 5/255f;
            color.g += 5 / 255f;
            color.b += 5 / 255f;
            if (color.r >=1)
            {
                audio.MapSetSound();
                color.r = 1;
                color.g = 1;
                color.b = 1;
                nowStageSet = false;
            }
            stageSettingImage.color = color;
        }
        else
        {
            for (byte i = 0; i < Stage_Manager.instanse.worldInformation.Count; i++)
            {
                if (Stage_Manager.instanse.worldInformation[i].worldLock !=
                    Stage_Manager.instanse.worldInformation[i].worldLockLog)
                {
                    if (Stage_Manager.instanse.worldInformation[i].worldLockLog)
                    {
                        Stage_Manager.instanse.worldInformation[i].worldLockLog = Stage_Manager.instanse.worldInformation[i].worldLock;
                        mapInfo[i].worldLock = Stage_Manager.instanse.worldInformation[i].worldLockLog;
                        mapInfo[i].worldLockLog = mapInfo[i].worldLock;
                        Debug.Log("�ʂ�₟�I�I");
                        mapImages[i].image.enabled = true;
                        stageSettingImage = mapImages[i].image;
                        var color = stageSettingImage.color;
                        color.r = grayColor/255f;
                        color.g = grayColor/255f;
                        color.b = grayColor/255f;
                        stageSettingImage.color = color;

                        nowStageSet = true;
                        settingNum = i;
                        break;
                    }
                }
            }
        }

    }

    //==============================
    //      �X�e�[�W�Z���N�g
    // �����ړ��݂̂̎���
    // �߂�l : ����
    //�@����  : _inputDirection �����͏��
    //==============================
    // �쐬��2023/04/27
    // �{��
    private void ChangeHorizontalPosition(float _inputDirection)
    {
        // ���͕��������Ȃ�
        if (_inputDirection < -0.55f)
        {
            if (cameraZoomFlag)
            {
                Debug.Log("�������ֈʒu�̕ύX���J�n���܂�");

                // �X�e�[�W�ԍ����Œ�l�����ł͂Ȃ��Ȃ�
                if (stageNum > 0)
                {
                    if (!mapInfo[worldNum].stageInformation[stageNum - 1].stageLock)
                    {
                        //------------------------------------------�x�T�E���h---------------
                        audio.BookMarkSound();
                        // ���݂̃A�j���[�V������؂�

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                        // �X�e�[�W�ԍ�������������
                        stageNum--;
                        // ���̔ԍ��̃A�j���[�V�������N��

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                    }
                }//----- if_stop -----
                else
                {
                    // �n�}�ԍ����Œ�l�ł͂Ȃ��Ȃ�
                    if (worldNum != 0)
                    {
                        if (!mapInfo[worldNum - 1].worldLock)
                        {
                            //------------------------------------------�x�T�E���h---------------
                            audio.BookMarkSound();
                            // ���݂̃A�j���[�V������؂�
                            for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            {
                                selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", false);
                            }
                            mapImages[worldNum].animator.SetBool("lockOn", false);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Exit();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = defaultColor;
                            }
                            // �n�}�ԍ�����������,�X�e�[�W�ԍ����ő�l�ɂ���
                            worldNum--;
                            stageNum = (byte)(mapInfo[worldNum].stageInformation.Count - 1);
                            // ���̔ԍ��̃A�j���[�V�������N��
                            //for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            //{
                            //    selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", true);
                            //}
                            mapImages[worldNum].animator.SetBool("lockOn", true);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Enter();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = selectColor;
                            }
                            ZoomMove();
                        }
                    }//----- if_stop -----

                }//----- else_stop -----
            }
            else
            {
                if (worldNum > 0)
                {
                    if (!mapInfo[worldNum - 1].worldLock)
                    {
                        //------------------------------------�J�[�\���ړ��T�E���h----------------------
                        audio.UISource.Stop();
                        audio.MoveCursorSound();
                        mapImages[worldNum].animator.SetBool("lockOn", false);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Exit();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = defaultColor;
                        }
                        worldNum--;
                        mapImages[worldNum].animator.SetBool("lockOn", true);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Enter();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = selectColor;
                        }
                    }
                }
            }
            // �J�����ʒu���ړ�������B



            // �ʒu�ύX���ꎞ�I�ɒ�~����
            StartCoroutine(ChangeDelay());

        }//----- if_stop -----
        // ���͕������E�Ȃ�
        else if (_inputDirection > 0.55f)
        {
            Debug.Log("�E�����ֈʒu�̕ύX���J�n���܂�");
            if (cameraZoomFlag)
            {
                // �X�e�[�W�ԍ����ő�l�����Ȃ�
                if (stageNum < (byte)mapInfo[worldNum].stageInformation.Count - 1)
                {
                    if (!mapInfo[worldNum].stageInformation[stageNum + 1].stageLock)
                    {
                        //------------------------------------------�x�T�E���h---------------
                        audio.BookMarkSound();

                        // ���݂̃A�j���[�V������؂�

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                        // �X�e�[�W�ԍ����㏸������
                        stageNum++;
                        // ���̔ԍ��̃A�j���[�V�������N��

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                    }
                }//----- if_stop -----
                else
                {
                    // �n�}�ԍ����ő�l�ł͂Ȃ��Ȃ�
                    if (worldNum < (byte)mapInfo.Count - 1)
                    {
                        if (!mapInfo[worldNum + 1].worldLock)
                        {
                            //------------------------------------------�x�T�E���h---------------
                            audio.BookMarkSound();

                            // ���݂̃A�j���[�V������؂�
                            for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            {
                                selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", false);
                            }
                            mapImages[worldNum].animator.SetBool("lockOn", false);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Exit();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = defaultColor;
                            }

                            // �n�}�ԍ����㏸����,�X�e�[�W�ԍ����Œ�l�ɂ���
                            worldNum++;
                            stageNum = 0;
                            // ���̔ԍ��̃A�j���[�V�������N��
                            //for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            //{
                            //    selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", true);
                            //}
                            mapImages[worldNum].animator.SetBool("lockOn", true);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Enter();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = selectColor;
                            }
                            ZoomMove();
                        }
                    }//----- if_stop -----

                }//----- else_stop -----
            }
            else
            {
                if (worldNum < (byte)mapInfo.Count - 1)
                {
                    if (!mapInfo[worldNum + 1].worldLock)
                    {
                        //-----------------------------------�J�[�\���ړ��T�E���h-------------
                        audio.UISource.Stop();
                        audio.MoveCursorSound();
                        mapImages[worldNum].animator.SetBool("lockOn", false);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Exit();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = defaultColor;
                        }
                        worldNum++;
                        mapImages[worldNum].animator.SetBool("lockOn", true);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Enter();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = selectColor;
                        }
                    }
                }
            }
            // �J�����ʒu���ړ�������B


            // �ʒu�ύX���ꎞ�I�ɒ�~����
            StartCoroutine(ChangeDelay());

        }//----- elseif_stop -----

    }

    //==============================
    //      �X�e�[�W�Z���N�g
    // ���c�ړ��݂̂̎���
    // �߂�l : ����
    //�@����  : _inputDirection �c���͏��
    //==============================
    // �쐬��2023/04/27
    // �{��
    private void ChangeVerticalPosition(float _inputDirection)
    {
        // ���͕��������Ȃ�
        if (_inputDirection < -0.55f)
        {
            Debug.Log("�������ֈʒu�̕ύX���J�n���܂�");

            // �X�e�[�W�ԍ����Œ�l�����ł͂Ȃ��Ȃ�
            if (worldNum < (byte)mapInfo.Count - 1)
            {
                // ���݂̃A�j���[�V������؂�
                mapImages[GetImageNumber()].animator.SetBool("lockOn", false);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                // �X�e�[�W�ԍ�������������
                worldNum++;
                if (stageNum > (byte)mapInfo[worldNum].stageInformation.Count - 1)
                {
                    stageNum = (byte)(mapInfo[worldNum].stageInformation.Count - 1);
                }
                // ���̔ԍ��̃A�j���[�V�������N��
                mapImages[GetImageNumber()].animator.SetBool("lockOn", true);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);

            }//----- if_stop -----
            //else
            //{
            //    // �n�}�ԍ����Œ�l�ł͂Ȃ��Ȃ�
            //    if (worldNum != 0)
            //    {
            //        // ���݂̃A�j���[�V������؂�
            //        animators[GetImageNumber()].SetBool("lockOn", false);
            //        // �n�}�ԍ�����������,�X�e�[�W�ԍ����ő�l�ɂ���
            //        worldNum--;
            //        stageNum = (byte)(worlds[worldNum].stages.Count - 1);
            //        // ���̔ԍ��̃A�j���[�V�������N��
            //        animators[GetImageNumber()].SetBool("lockOn", true);
            //    }//----- if_stop -----

            //}//----- else_stop -----

            // ���݈ʒu��ύX����
            // �J�����ʒu���ړ�������B
            ZoomMove();


            // �ʒu�ύX���ꎞ�I�ɒ�~����
            StartCoroutine(ChangeDelay());

        }//----- if_stop -----
        // ���͕������E�Ȃ�
        else if (_inputDirection > 0.55f)
        {
            Debug.Log("������ֈʒu�̕ύX���J�n���܂�");

            // �X�e�[�W�ԍ����ő�l�����Ȃ�
            if (worldNum > 0)
            {
                // ���݂̃A�j���[�V������؂�
                mapImages[GetImageNumber()].animator.SetBool("lockOn", false);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                // �X�e�[�W�ԍ����㏸������
                worldNum--;
                if (stageNum > (byte)mapInfo[worldNum].stageInformation.Count - 1)
                {
                    stageNum = (byte)(mapInfo[worldNum].stageInformation.Count - 1);
                }
                // ���̔ԍ��̃A�j���[�V�������N��
                mapImages[GetImageNumber()].animator.SetBool("lockOn", true);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
            }//----- if_stop -----
            //else
            //{
            //    // �n�}�ԍ����ő�l�ł͂Ȃ��Ȃ�
            //    if (worldNum < (byte)worlds.Count - 1)
            //    {
            //        // ���݂̃A�j���[�V������؂�
            //        animators[GetImageNumber()].SetBool("lockOn", false);
            //        // �n�}�ԍ����㏸����,�X�e�[�W�ԍ����Œ�l�ɂ���
            //        worldNum++;
            //        stageNum = 0;
            //        // ���̔ԍ��̃A�j���[�V�������N��
            //        animators[GetImageNumber()].SetBool("lockOn", true);
            //    }//----- if_stop -----

            //}//----- else_stop -----

            // �J�����ʒu���ړ�������B
            ZoomMove();


            // �ʒu�ύX���ꎞ�I�ɒ�~����
            StartCoroutine(ChangeDelay());

        }//----- elseif_stop -----

    }

    //===========================================
    // ���݂̃X�e�[�W�A���[���h�̔ԍ�����Image�̃��X�g�̔ԍ����Z�o����
    // �߂�l�FImage���X�g�̔ԍ�
    // ��������
    //===========================================
    // 2023/05/01
    // �쐬�ҁ@���c
    private byte GetImageNumber()
    {
        byte stageCount = 0;

        // ���[���h���ɑ��݂���X�e�[�W���ЂƂO�̃��[���h�܂ŉ��Z
        for (byte i = 0; i < worldNum; i++)
        {
            stageCount += (byte)mapInfo[i].stageInformation.Count;
        }
        // ���݂̃X�e�[�W�ԍ������Z���邱�Ƃ�Image�̔ԍ�������
        stageCount += stageNum;

        return stageCount;
    }

    //==========================================
    // Images�̃��X�g��Add����ۂɁA�����擾����B
    // �߂�l�F�o�^�������������Images
    // ����  �F�o�^������Image�̔ԍ�
    //==========================================
    // �쐬���@2023/05/03
    // �쐬�ҁ@���c
    private MapImages SetImages(byte _i)
    {
        // ��悸�o�^������Image���擾
        var trans = transform.GetChild(_i);

        // ���X�g�����쐬
        MapImages Images;
        // �q�I�u�W�F��Image���擾
        Images.image = trans.GetComponent<Image>();
        // �q�I�u�W�F�̃A�j���[�^�[���擾
        Images.animator = trans.GetComponent<Animator>();
        // �q�I�u�W�F�̃��[���h���W���擾�B�e(���̃I�u�W�F)�̍��W�Ɏq�I�u�W�F��RectTrans���W��
        Images.transPos = trans.GetComponent<RectTransform>().position;
        // �q�I�u�W�F�̃}�e���A������X�N���v�g���擾
        Images.mSelect = trans.GetComponent<testMSelect>();

        // Images��Ԃ��B
        return Images;
    }

    private SelectorImages SetSelectorImages(byte _i)
    {
        // �J�����̑��I�u�W�F�ɑ��݂���_i�Ԗڂ�Image���擾
        var trans = cameraObj.transform.GetChild(0).GetChild(_i);
        SelectorImages Images;

        Images.image = trans.GetComponent<Image>();
        Images.animator = trans.GetComponent<Animator>();
        Images.transPos = trans.GetComponent<RectTransform>().position;

        return Images;
    }

    private void ZoomMove()
    {
        switch (mapInfo[worldNum].zoomSet)
        {
            case Stage_Manager.WorldInfo.ZoomSet.ON:
                // ���݈ʒu��ύX����
                if (cameraZoomFlag)
                {
                    nowZoom = true;
                }
                break;
            case Stage_Manager.WorldInfo.ZoomSet.OFF:
                if (cameraZoomFlag)
                {
                    nowZoomOut = true;
                    cameraZoomFlag = false;
                }
                break;
        }
    }

    //===========================================
    // �Q�[���I�����Ƀ��X�g��j������B
    //===========================================
    private void OnApplicationQuit()
    {
        mapInfo.Clear();
        mapImages.Clear();
        selectorImages.Clear();

    }
}