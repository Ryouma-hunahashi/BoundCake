using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;


//=========================================
// �Q�[���I�����ɂ������m�F���鎞�̏���
//=========================================

public class UI_ExitConfimation : MonoBehaviour
{
    public GameObject Canvas;


   // [Tooltip("�Q�[�����I���Ƃ��������̎��̃{�^��")]
    //public Button Notbutton;
    //[Tooltip("�Q�[�����I���Ƃ��͂��̎��̃{�^��")]
   // public Button Yesbutton;


    //[Tooltip("�Q�[���I���Ƃ��ɏo��e�L�X�g")]
   // public Text endtext;

    private Image endImage;
    private Image endingImage;
    private Image NotEndingImage;

    //[Tooltip("")]
   // public Text notext;


   // [Tooltip("")]
   // public Text yestext;
   // bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame;
   // bool eastButton = Gamepad.current.buttonEast.wasPressedThisFrame;

    // Start is called before the first frame update
    void Start()
    {
        Canvas.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        bool endButton = Gamepad.current.startButton.wasPressedThisFrame;
        
      
        if(endButton)
        {
            Canvas.SetActive(true); 
        }
        if (Gamepad.current.buttonSouth.wasPressedThisFrame &&Canvas.active)
        {
            Quit();
        }
        if(Canvas.active==true)
        {
            Debug.Log("test");
        }
        if (Gamepad.current.buttonEast.wasPressedThisFrame&& Canvas.active)
        {
            Canvas.SetActive(false);
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
