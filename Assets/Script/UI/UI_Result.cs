using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//===========================================
// ���U���g�̏���
//===========================================

public class UI_Result : MonoBehaviour
{
    public string resultStage; // ���݂̃��U���g�X�e�[�W���擾

    // ============================
    // �ʃX�N���v�g�Q��
    [SerializeField] OtherEffectManager effect; // effect�����邽��
    [SerializeField] FadeCamera fadeCamera; // �J�������Ó]�����邽��
    [SerializeField] private UIAudio audio;
    // ============================

    //============================================
    // �K�v��Image�摜�i������NULL���ƃG���[���o��j
    [SerializeField] private Image UI_parfait_top; // �p�t�F�̈�ԏ�
    [SerializeField] private Image UI_parfait_mid; // �p�t�F����
    [SerializeField] private Image UI_parfait_btm; // �p�t�F��ԉ�
    [SerializeField] private Image UI_nextStage;   // ���i�ނ��߂̉摜
    [SerializeField] private Image UI_selectStage; // �X�e�[�W�Z���N�g�����p�̉摜
    [SerializeField] private Image ParfaitImage;   // �p�t�F�����悤�̉摜
    [SerializeField] private Image UI_backimage; // �X�e�[�W�ɂ���Ĕw�i���ς��Ƃ��ɗp����ϐ�
    [SerializeField] private Image UI_blackoutimage; // �V�[���؂�ւ��̎��Ɏg���Ó]�p�摜
    [SerializeField] private Image UI_panel_top;    // ���U���g�p�l����̕���
    [SerializeField] private Image UI_panel_mid;    // ���U���g�p�l���^�񒆂̕���
    [SerializeField] private Image UI_panel_btm;    // ���U���g�p�l�����̕���
    [SerializeField] private Image UI_star_left;    // ���̍��摜
    [SerializeField] private Image UI_star_mid;     // ���̐^�񒆉摜
    [SerializeField] private Image UI_star_right;   // ���̉E�摜
    //===========================================

    // �A�j���[�V����
    [SerializeField] private Animator nextStageAnim;
    [SerializeField] private Animator selectStageAnim;

    // ���U���g�̎��ɃX�e�[�W�̔ԍ��擾
    public byte worldnum;
    public byte stagenum;

    // �p�t�F�̍��W�ʒu�擾
    private RectTransform top;
    private RectTransform mid;
    private RectTransform btm;

    // �I������Ă���Ƃ��ɓ����x��ύX����
    [SerializeField, Range(0f, 1f)]
    private float alphaimage;

    // �p�t�F�A�C�e���̗������x
    [SerializeField] private float p_move = 5.0f;
    [SerializeField] private float move_x = 0.1f;
    [SerializeField] private float P_change = 10f;
    [SerializeField] private float weight = 3.0f; // effect����莞�Ԃł���
    private float time;


    // 
    bool nextcheck = true;
    bool selectcheck = false;
    bool btm_effect = true;
    bool mid_effect = true;
    bool top_effect = true;
    bool shine_effect = true;
    bool nextstagecheck = false;
    bool selectstagecheck = false;
    bool audiocheck;

    // �A�ő΍��p
    private WaitForSeconds wait = new WaitForSeconds(0.8f);
    private bool buttonEnabled = true;

    //=============================
    // private RectTransform pafait;
    // Start is called before the first frame update
    void Start()
    {

        // ���݂̃��U���g�̖��O���擾
        resultStage = SceneManager.GetActiveScene().name;
        // ���U���g�V�[���Ȃ�΁A���̃L�����o�X��\������
        if (resultStage == "result1" || resultStage == "result2" ||
            resultStage == "result3" || resultStage == "result4" ||
            resultStage == "result5")
        {
            Result_Manager.instance.Canvas.enabled = true;
        }
        else
        {
            Result_Manager.instance.Canvas.enabled = false;
        }

        // �����ɃX�e�[�W�ԍ���ۑ�
        worldnum = Result_Manager.instance.nowWorld;
        stagenum = Result_Manager.instance.nowStage;

        // ���U���g�̃X�e�[�W�ɂ���Ĕw�i��؂�ւ���
        Check_backGround();

        // �O�̃X�e�[�W�ł̃p�t�F�擾���`�F�b�N����
        CheckParfait();

        // ���ʉ��擾
        audio = GetComponent<UIAudio>();
        // ���݂̃p�t�F�̃A�C�e���̍��W�ʒu�擾
        top = UI_parfait_top.rectTransform;
        mid = UI_parfait_mid.rectTransform;
        btm = UI_parfait_btm.rectTransform;
        // ���̃��U���g��scene�����擾
        //resultStage = Stage_Manager.StageInfo.stageName;

        

        // ================�����ݒ�==================

        // �p�t�F�̒��g�����Z�b�g����
        ParfaitImage.sprite = Resources.Load<Sprite>("glass");

        // ���̃X�e�[�W�ւƃZ���N�g�X�e�[�W�ւ̉摜�����Z�b�g����
        UI_nextStage.enabled = false;
        UI_selectStage.enabled = false;
        //UI_blackoutimage.color = new Color(1.0f, 1.0f, 1.0f, 0f);

        btm_effect = true;
        mid_effect = true;
        top_effect = true;
        shine_effect = true;
        audiocheck = false;
        
        nextStageAnim = UI_nextStage.gameObject.GetComponent<Animator>();
        selectStageAnim = UI_selectStage.gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // �c�����̓��͂��󂯂���
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        bool downButton = Gamepad.current.dpad.down.wasPressedThisFrame; // �\���L�[���̓��͏��
        bool upButton = Gamepad.current.dpad.up.wasPressedThisFrame; // �\���L�[��̓��͏��
        bool eastButton = Gamepad.current.buttonEast.wasPressedThisFrame;  // ���{�^���̓��͏��
        bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame; // ��{�^���̓��͏��

        // ========================================
        // ��ԉ��̃A�C�e��������Ƃ�
        if (UI_parfait_btm.enabled)
        {
            if (btm_effect)
            {
                effect.StartPafe();
                btm_effect = false;
            }
            //�@���W�����������Ɏ����Ă���
            btm.position -= new Vector3(-move_x, p_move, 0);


            if (btm.position.y <= P_change)
            {
                ParfaitImage.sprite = Resources.Load<Sprite>("p2");
                UI_panel_top.sprite = Resources.Load<Sprite>("P1_board");
                UI_star_left.sprite = Resources.Load<Sprite>("star");

                Invoke("btmfalse", 0.5f);
                effect.StopPafe();
            }
        }
        // �^�񒆂̃A�C�e��������Ƃ�
        if (!UI_parfait_btm.enabled && UI_parfait_mid.enabled)
        {
            if (mid_effect)
            {
                effect.StartPafe();
                mid_effect = false;
            }

            // �A�C�e�������ɗU������
            mid.position -= new Vector3(0, p_move, 0);

            // ���̉��̈ʒu�ɂ�������
            if (mid.position.y <= P_change)
            {
                // �p�t�F�̉摜��؂�ւ���
                Debug.Log(ParfaitImage.sprite.name);
                //�@���̃A�C�e���������Ă���摜�̎�
                if (ParfaitImage.sprite.name == "p2")
                {
                    Debug.Log("btm true mid true");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p5");
                    UI_panel_mid.sprite = Resources.Load<Sprite>("P2_board");
                    UI_star_mid.sprite = Resources.Load<Sprite>("star");

                }
                // ���̃A�C�e���������Ă��Ȃ��摜�̎�
                else if (ParfaitImage.sprite.name == "glass")
                {
                    Debug.Log("btm false mid true");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p3");
                    UI_panel_mid.sprite = Resources.Load<Sprite>("P2_board");
                    UI_star_left.sprite = Resources.Load<Sprite>("star");

                }
                effect.StopPafe();
                Invoke("midfalse", 0.5f);
            }
        }
        // top�����������Ƃ�
        if (!UI_parfait_btm.enabled && !UI_parfait_mid.enabled && UI_parfait_top.enabled)
        {
            if (top_effect)
            {
                effect.StartPafe();
                top_effect = false;
            }

            top.position -= new Vector3(move_x, p_move, 0);

            if (top.position.y <= P_change)
            {
                // �S���擾���Ă���
                if (ParfaitImage.sprite.name == "p5")
                {
                    Debug.Log("b&m true top true");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p8");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_right.sprite = Resources.Load<Sprite>("star");

                }
                // btm false mid true�̂Ƃ�
                else if (ParfaitImage.sprite.name == "p3")
                {
                    ParfaitImage.sprite = Resources.Load<Sprite>("p7");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_mid.sprite = Resources.Load<Sprite>("star");

                }
                // btm true  mid false �̂Ƃ� 
                else if (ParfaitImage.sprite.name == "p2")
                {
                    ParfaitImage.sprite = Resources.Load<Sprite>("p6");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_mid.sprite = Resources.Load<Sprite>("star");

                }
                // btm mid��false�̂Ƃ�
                else if (ParfaitImage.sprite.name == "glass")
                {
                    Debug.Log("btm mid false");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p4");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_left.sprite = Resources.Load<Sprite>("star");

                }
                effect.StopPafe();
                Invoke("topfalse", 0.5f);
            }
        }
        // �p�t�F�S���ړ����I������Ƃ�
        if (!UI_parfait_btm.enabled && !UI_parfait_mid.enabled && !UI_parfait_top.enabled)
        {
            if (shine_effect)
            {
                audio.MapSetSound();
                effect.StartFlash();
                shine_effect = false;
            }
            UI_nextStage.enabled = true;
            UI_selectStage.enabled = true;
           
        }
        //=========================================

        Debug.Log(fadeCamera.isFadeOut);

        // �����̉摜���o�Ă���Ƃ�
        if (UI_selectStage.enabled && UI_nextStage.enabled)
        {
            // ��ɃJ�[�\��������Ƃ�
            if (nextcheck)
            {
                // ���̉摜�����������ɂ���
                //UI_nextStage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                //UI_selectStage.color = new Color(1.0f, 1.0f, 1.0f, alphaimage);
                nextStageAnim.SetBool("lockOn", true);
                selectStageAnim.SetBool("lockOn", false);
                // �����{�^���������Ƃ� �A�ŋ֎~����ד�̏����ł����������Ȃ��悤�ɂ���
                if (southButton && buttonEnabled)
                {
                    // �t�F�[�h�A�E�g
                    fadeCamera.isFadeOut = true;
                    nextstagecheck = true;
                    if (buttonEnabled)
                    {
                        // ���̔ԍ��̓Y������1���Z����
                        stagenum++;
                        // �X�e�[�W�̒[�����ɍs������A���̃X�e�[�W�̏��߂���ɂ���
                        if (stagenum > 3)
                        {
                            worldnum++;
                            stagenum = 0;
                        }
                        // ���̏��Ƀp�t�F���擾�������ǂ�����ۑ������̂ŃQ�b�g�����̂��������񃊃Z�b�g����
                        CheckParfaitInit();
                        // ���̃X�e�[�W�̓Y������ۑ�����
                        Result_Manager.instance.nowWorld = worldnum;
                        Result_Manager.instance.nowStage = stagenum;
                        Debug.Log(worldnum);
                        Debug.Log(stagenum);
                        // ���[���h���b�N��false�ɂ���
                        Stage_Manager.instanse.worldInformation[worldnum].worldLock = false;
                        // �{�^���������Ȃ��悤�ɂ���i��i�K�ŉ����Ȃ��悤�ɂ���j
                        buttonEnabled = false;
                    }
                }
                // �t�F�[�h�A�E�g����������Ƃ�
                if (!fadeCamera.isFadeOut && nextstagecheck)
                {
                    // �V�[���؂�ւ���
                    audio.ChangeSceneSound();
                    // ��u�����t���[���x��������
                    StartCoroutine("momentFlame");
                    // ���̃X�e�[�W�փV�[���J�ڂ���
                    S_Manager.instance.SceneChange(Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].sceneName);
                }
            }
            // ���ɃJ�[�\��������Ƃ�
            if (selectcheck)
            {
                // ��̉摜�����������ɂ���
                //UI_selectStage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                //UI_nextStage.color = new Color(1.0f, 1.0f, 1.0f, alphaimage);
                nextStageAnim.SetBool("lockOn", false);
                selectStageAnim.SetBool("lockOn", true);

                // �~�{�^�����������Ƃ�
                if (southButton)
                {
                    // �t�F�[�h�A�E�g����
                    selectstagecheck = true;
                    fadeCamera.isFadeOut = true;

                }
                // �t�F�[�h�A�E�g���I�������
                if (!fadeCamera.isFadeOut && selectstagecheck)
                {
                    // �V�[���؂�ւ���
                    audio.ChangeSceneSound();
                    // ��u�����t���[���x��������
                    StartCoroutine("momentFlame");
                    StartCoroutine("momentFlame");
                    // �X�e�[�W�Z���N�g��
                    SceneManager.LoadScene("ContentsSelect");
                }
            }
            // ��ɃX�e�B�b�N���X���違�\����L�[���������Ƃ�
            if (inputHorizontal > 0.55f || upButton)
            {
                if (audiocheck)
                {
                    // �����Ɍ��ʉ�
                    audio.MoveCursorSound();
                    audiocheck = false;
                }
                // ��̉摜�ɃJ�[�\�������킹��
                nextcheck = true;    
                selectcheck = false;
            }
            else if (inputHorizontal < -0.55f || downButton)
            {
                if (!audiocheck)
                {
                    // �����Ɍ��ʉ�
                    audio.MoveCursorSound();
                    audiocheck = true;
                }
                // ���̉摜�ɃJ�[�\�������킹��
                selectcheck = true;
                nextcheck = false;
            }
            //���Ԍv�����J�n����
            time += Time.deltaTime;

            // ��莞�Ԃ���������
            if (time > weight)
            {
                // �L���L���̃G�t�F�N�g������
                effect.StopFlash();
            }
        }
    }

    
    // �e���U���g�ɉ����Ĕw�i�̉摜��؂�ւ���
    void Check_backGround()
    {
        Debug.Log("test");
        if (resultStage == "result1")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back1");
        }
        if (resultStage == "result2")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back2");
        }
        if (resultStage == "result3")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back3");
        }
        if (resultStage == "result4")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back4");
        }
        if (resultStage == "result5")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back5");
        }
    }

    // �e�X�e�[�W�łǂ̃A�C�e��������������ׂ�֐�
    void CheckParfait()
    {
        if (Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].parfait.top)
        {
            UI_parfait_top.enabled = true;
        }
        else
        {
            UI_parfait_top.enabled = false;
        }
        if (Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].parfait.mid)
        {
            UI_parfait_mid.enabled = true;
        }
        else
        {
            UI_parfait_mid.enabled = false;
        }
        if (Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].parfait.btm)
        {
            UI_parfait_btm.enabled = true;
        }
        else
        {
            UI_parfait_btm.enabled = false;
        }

    }

    // ���Z�b�g����֐�
    void CheckParfaitInit()
    {
        Result_Manager.instance.getParfait.top = false;
        Result_Manager.instance.getParfait.mid = false;
        Result_Manager.instance.getParfait.btm = false;
    }

    void btmfalse()
    {
        UI_parfait_btm.enabled = false;
    }
    void midfalse()
    {
        UI_parfait_mid.enabled = false;
    }
    void topfalse()
    {
        UI_parfait_top.enabled = false;
    }
  IEnumerable momentFlame()
    {
        yield return null;
    }
}