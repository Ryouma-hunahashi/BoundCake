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
    //ボタン連打防止変数
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
        // 縦方向を取得する
        float inputhozontal = Input.GetAxisRaw("Horizontal");
        float inputVertical = Input.GetAxisRaw("Vertical");
        // suothボタンを取得(Aボタン)
        bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame;
        bool downButton = Gamepad.current.dpad.down.wasPressedThisFrame; // 十字キー下の入力情報
        bool upButton = Gamepad.current.dpad.up.wasPressedThisFrame; // 十字キー上の入力情報
        bool rightButton = Gamepad.current.dpad.right.wasPressedThisFrame; // 十字キー下の入力情報
        bool leftButton = Gamepad.current.dpad.left.wasPressedThisFrame; // 十字キー上の入力情報

        
        // 最初スタートにカーソルを合わせておく
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

                // スタートにカーソルが合わさっているときにボタンを押すと
                if (southButton)
                {
                    audio.DecisionSound();
                    fadecamera.isFadeOut = true; // フェードアウトを開始する
                    stopbutton = true;
                }
                if (!fadecamera.isFadeOut && stopbutton) // フェードアウトが終了した時
                {
                    // シーンを切り替える
                    SceneManager.LoadScene("ContentsSelect");
                }

            }
            if (endcheck) // 終わるにカーソルが合わさったとき
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
