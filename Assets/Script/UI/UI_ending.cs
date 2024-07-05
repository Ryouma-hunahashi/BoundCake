using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_ending : MonoBehaviour
{
    [SerializeField] private Image titleimage;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame;

        if(southButton)
        {
            SceneManager.LoadScene("Title");
        }
    }
}
