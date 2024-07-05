using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Pause : MonoBehaviour
{
    // �|�[�Y���
    private bool nowPause = false;
    private bool outPause = false;

    // �|�[�Y�؂�ւ����x�̂�
    private bool setPause = false;

    // ���݂̑I��n�_
    private byte selectNum = 0;

    // ���l�ɕύX���������ۂɕύX
    private bool selectChanged = false;

    // �\�������ʂ��i�[
    [SerializeField] private List<Image> buttonImages = new List<Image>();
    [SerializeField] private List<Text> textNames = new List<Text>();

    // �q�̐��𐔒l�Ŋi�[
    private byte childCount;

    [SerializeField] private Animator goSelectAnim;
    [SerializeField] private Animator ContinueAnim;

    private void Start()
    {
        // �q�I�u�W�F�N�g�̐����擾
        childCount = (byte)this.transform.childCount;

        for (byte i = 0; i < buttonImages.Count; i++)
        {
            // UI���\���ɂ���
            buttonImages[i].enabled = false;

        }//----- for_stop -----

        goSelectAnim = buttonImages[0].gameObject.GetComponent<Animator>();
        ContinueAnim = buttonImages[1].gameObject.GetComponent<Animator>();

        for (byte i = 0; i < textNames.Count; i++)
        {
            // UI���\���ɂ���
            textNames[i].enabled = false;

        }//----- for_stop -----
        goSelectAnim.SetBool("lockOn", false);
        ContinueAnim.SetBool("lockOn", true);
    }

    void Update()
    {
        // �Q�[���p�b�h���ڑ�����Ă��Ȃ��Ȃ甲����
        if (Gamepad.current == null) return;

        // �|�[�Y�֌W�̃{�^���̐ݒ�
        bool ActButton = Gamepad.current.buttonSouth.wasPressedThisFrame;
        bool pauseButton = Gamepad.current.startButton.wasPressedThisFrame;

        // �X�e�B�b�N�̏㉺����������擾����
        bool Lstick_Right = Gamepad.current.leftStick.right.wasPressedThisFrame;
        bool Lstick_Left = Gamepad.current.leftStick.left.wasPressedThisFrame;

        // �|�[�Y�{�^���������ꂽ�Ƃ�
        // �܂��́A�|�[�Y�𔲂���Ƃ�
        if (pauseButton || outPause)
        {
            // �|�[�Y��Ԃ�؂�ւ��܂�
            nowPause = !nowPause;

            setPause = true;
            outPause = false;

            Debug.Log("�|�[�Y��Ԃ�[ " + nowPause + " ]�ɂȂ�܂����B");

            // �|�[�Y��Ԃł͂Ȃ��Ȃ����Ƃ�
            if(!nowPause)
            {
                // ������ĊJ����
                Time.timeScale = 1.0f;

                // �ύX���x�̂ݎ��s
                if (setPause)
                {
                    for (byte i = 0; i < buttonImages.Count; i++)
                    {
                        // UI���\���ɂ���
                        buttonImages[i].enabled = false;

                    }//----- for_stop -----


                    for (byte i = 0; i < textNames.Count; i++)
                    {
                        // UI���\���ɂ���
                        textNames[i].enabled = false;

                    }//----- for_stop -----

                    // ��L���s�ڍs�ύX�܂őҋ@
                    setPause = false;

                }//----- if_stop -----

            }//----- if_stop -----

        }//----- if_stop -----

        // �|�[�Y��Ԃ̂Ƃ�
        if(nowPause)
        {
            // ���삪�~�܂��Ă��Ȃ��Ƃ�
            if (Time.timeScale != 0)
            {
                // Fixed���̓�����X�g�b�v������
                Time.timeScale = 0.000001f;
            
            }//----- if_stop -----

            // �ύX���x�̂ݎ��s
            if(setPause)
            {
                // �\�������Ƃ��Ɏw��ʒu�������ʒu�ɖ߂�
                selectNum = 0;
                for (byte i = 0; i < buttonImages.Count; i++)
                {
                    // UI���\���ɂ���
                    buttonImages[i].enabled = true;
                    // �w��ԍ��ɉ����ĐF��ύX����
                    if (selectNum == i)
                    {
                        // �I������Ă���E�B���h�E�̐F��ύX����
                        //buttonImages[i].color = Color.gray;
                        goSelectAnim.SetBool("lockOn", false);

                    }//----- if_stop -----
                    else if (selectNum != i)
                    {
                        // �I������Ă���E�B���h�E�̐F��ύX����
                        //buttonImages[i].color = Color.white; 
                        ContinueAnim.SetBool("lockOn", true);

                    }
                }//----- for_stop -----


                for (byte i = 0; i < textNames.Count; i++)
                {
                    // UI���\���ɂ���
                    textNames[i].enabled = true;

                }//----- for_stop -----
                
                    // ��L���s�ڍs�ύX�܂őҋ@
                    setPause = false;

            }//----- if_stop -----

            // �㉺�ǂ��炩�̓��͂��󂯂Ƃ����Ƃ�
            // �p�x�̋��e�͈͂�������֒������ꍇ
            if (Lstick_Left)
            {
                // �w��ʒu��'0'����Ȃ�
                if (selectNum > 0)
                {
                    // �I��n�_����ւ��炷
                    selectNum--;

                }//----- if_stop -----

                // �ύX��̏�Ԃ𑗂�
                selectChanged = true;

            }//----- if_stop -----
             // �p�x�̋��e�͈͂��������֒������ꍇ
            else if (Lstick_Right)
            {
                // �w��ʒu��'�q�̐� - �z�񒲐�'�̒l�ȉ��Ȃ�
                if (selectNum < childCount - 1)
                {
                    // �I��n�_�����ւ��炷
                    selectNum++;

                }//----- if_stop -----

                // �ύX��̏�Ԃ𑗂�
                selectChanged = true;

            }//----- elseif_stop -----

            // �w��ʒu�ɕύX���������ꍇ
            if (selectChanged)
            {
                selectChanged = false;
                if (selectNum == 0)
                {
                    ContinueAnim.SetBool("lockOn", true);
                    goSelectAnim.SetBool("lockOn", false);
                }
                else if (selectNum == 1)
                {
                    ContinueAnim.SetBool("lockOn", false);
                    goSelectAnim.SetBool("lockOn", true);
                }

                //for (byte i = 0; i <buttonImages.Count; i++)
                //{
                //    if(selectNum == i)
                //    {
                //        // �I������Ă���E�B���h�E�̐F��ύX����
                //        //buttonImages[i].color = Color.gray;
                //        //ContinueAnim.SetBool("lockOn", true);
                        


                //    }//----- if_stop -----
                //    else if(selectNum != i)
                //    {
                //        // �I������Ă���E�B���h�E�̐F��ύX����
                //        //buttonImages[i].color = Color.white;
                //        //goSelectAnim.SetBool("lockOn", false);
                //        //if (selectNum == 0)
                //        //{
                //        //    ContinueAnim.SetBool("lockOn", true);
                //        //    goSelectAnim.SetBool("lockOn", true);
                //        //}
                //        //else if (selectNum == 1)
                //        //{
                //        //    ContinueAnim.SetBool("lockOn", true);
                //        //    goSelectAnim.SetBool("lockOn", false);
                //        //}

                //    }//----- elseif_stop -----

               // }//----- for_stop -----

            }//----- if_stop -----

            // ����{�^���������ꂽ�Ƃ�
            if(ActButton)
            {
                // �I������Ă���UI�ɂ���ĕω�
                switch (selectNum)
                {
                    case 0:
                        {
                            // �|�[�Y��Ԃ𔲂���
                            outPause = true;

                        }// �ĊJ��I��
                        break;
                    case 1:
                        {
                            Time.timeScale = 1.0f;
                            // �X�e�[�W�Z���N�g�ɑ����Ă�������
                            SceneManager.LoadScene("ContentsSelect");
                        }// �I����I��
                        break;
                }//----- switch_stop -----
            }//----- if_stop -----

        }//----- if_stop -----
    }
    private void OnApplicationQuit()
    {
        buttonImages.Clear();
        textNames.Clear();
    }
}
