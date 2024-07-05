using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
    [SerializeField] private Animator nextAnim;
    [SerializeField] private Animator endAnim;
    [SerializeField] private Animator AppEndAnim;
    [SerializeField] private Animator ContinueAnim;

    [SerializeField]private UIAudio audio;
    [SerializeField]private FadeCamera fadecamera;

    [SerializeField] private Canvas canvas;
    [SerializeField] private Canvas AplicationEnd;
    [SerializeField] private Image BackImage;
    [SerializeField] private Image TitleImage;
    [SerializeField] private Image PlayerImage;
    [SerializeField] private Image StartImage;
    [SerializeField] private Image EndImage;
    [SerializeField] private Image TextImage;
    [SerializeField] private Image EndingImage;
    [SerializeField] private Image NotEndingImage;
    [SerializeField] private bool endcheck;

    [SerializeField, Range(0f, 1f)] private float alpha;
    //�{�^���A�Ŗh�~�ϐ�
    bool stopbutton = false;
    bool endUI = true;
    bool B_continue = true;
    bool audiocheck = true;
    bool H_audiocheck = true;
    bool startanimcheck = true;
    bool endanimcheck = true;
    // Start is called before the first frame update
    void Start()
    {
        nextAnim = StartImage.gameObject.GetComponent<Animator>();
        endAnim = EndImage.gameObject.GetComponent<Animator>();
        AppEndAnim = EndingImage.gameObject.GetComponent<Animator>();
        ContinueAnim = NotEndingImage.gameObject.GetComponent<Animator>();
        audio = GetComponent<UIAudio>();
        endcheck = false;
        AplicationEnd.enabled = false;
        stopbutton = false;
        endUI = false;
        B_continue = true;

        nextAnim.SetBool("lockOn", true);
        endAnim.SetBool("lockOn", false);
    }

    // Update is called once per frame
    void Update()
    {
        // �c�������擾����
        float inputhozontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");
        // suoth�{�^�����擾(A�{�^��)
        bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame;
        bool downButton = Gamepad.current.dpad.down.wasPressedThisFrame; // �\���L�[���̓��͏��
        bool upButton = Gamepad.current.dpad.up.wasPressedThisFrame; // �\���L�[��̓��͏��
        bool rightButton = Gamepad.current.dpad.right.wasPressedThisFrame; // �\���L�[���̓��͏��
        bool leftButton = Gamepad.current.dpad.left.wasPressedThisFrame; // �\���L�[��̓��͏��

        
        // �ŏ��X�^�[�g�ɃJ�[�\�������킹�Ă���
        if (!endUI)
        {
            if (!endcheck)
            {
                nextAnim.SetBool("lockOn", true);
                endAnim.SetBool("lockOn", false);
                //if (startanimcheck)
                //{
                //    Debug.Log("startanim");
                //    nextAnim.SetBool("lockOn",true);
                //    //endAnim.Play("UnSelect");
                //    startanimcheck = false;
                //}
                //else
                //{
                //    Debug.Log("Nostartanim");

                //}
                //StartImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                //EndImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);

                // �X�^�[�g�ɃJ�[�\�������킳���Ă���Ƃ��Ƀ{�^����������
                if (southButton)
                {
                    audio.DecisionSound();
                    fadecamera.isFadeOut = true; // �t�F�[�h�A�E�g���J�n����
                    stopbutton = true;
                }
                if (!fadecamera.isFadeOut && stopbutton) // �t�F�[�h�A�E�g���I��������
                {
                    // �V�[����؂�ւ���
                    SceneManager.LoadScene("ContentsSelect");
                }

            }
            if (endcheck) // �I���ɃJ�[�\�������킳�����Ƃ�
            {
                nextAnim.SetBool("lockOn", false);
                endAnim.SetBool("lockOn", true);
                Debug.Log(endAnim);
                //StartImage.color = new Color(1.0f, 1.0f, 1.0f, alpha);
                //EndImage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                if (southButton)
                {
                    audio.DecisionSound();
                    AplicationEnd.enabled = true;
                    endUI = true;
                }
            }
            if (inputVertical > 0.55f || upButton)
            {
                if (audiocheck)
                {
                    audio.MoveCursorSound();
                    audiocheck = false;
                }
                endcheck = false;
            }
            if (inputVertical < -0.55f || downButton)
            {
                if (!audiocheck)
                {
                    audio.MoveCursorSound();
                    audiocheck = true;
                }
                endcheck = true;
            }
        }
        else if(endUI)
        {

            if(B_continue)
            {
                AppEndAnim.SetBool("lockOn", false);
                ContinueAnim.SetBool("lockOn", true);
                
                if (southButton)
                {
                    audio.DecisionSound();
                    AplicationEnd.enabled = false;
                    endUI = false;
                }
            }
            if (!B_continue)
            {
                AppEndAnim.SetBool("lockOn", true);
                ContinueAnim.SetBool("lockOn", false);
                if (southButton)
                {
                    audio.DecisionSound();
                    Quit();
                }
            }
            if (inputhozontal < - 0.55f||leftButton)
            {
                if (!H_audiocheck)
                {
                    audio.MoveCursorSound();
                    H_audiocheck = true;
                }
                B_continue = false;
            }
            if (inputhozontal >0.55f||rightButton)
            {
                if (H_audiocheck)
                {
                    audio.MoveCursorSound();
                    H_audiocheck = false;
                }
                B_continue = true;
            }
        }
    }
    public void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
      Application.Quit();
#endif
    }
}
