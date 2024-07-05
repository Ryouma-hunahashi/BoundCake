using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Test_Goal : MonoBehaviour
{
    // �S�[������
    [Tooltip("�S�[�����ɓ����Ă��锻��")]
    [SerializeField] private bool inGoal;

    [Tooltip("�Q�[�����N���A�������̔���")]
    [SerializeField] private bool gameClear;

    private void OnTriggerStay(Collider other)
    {
        inGoal = true;
    }

    private void OnTriggerExit(Collider other)
    {
        inGoal = false;
    }

    private void Start()
    {
        gameClear = false;

        // �V�[���}�l�[�W���[�ɃN���A���𑗂�
        //S_Manager.gameClear = gameClear;
    }

    private void Update()
    {
        if(inGoal)
        {//----- if_start -----

            if (this.GetComponent<Renderer>().material.color != Color.red)
            {//----- if_start -----

                // �F��ύX����
                this.GetComponent<Renderer>().material.color = Color.red;

            }//----- if_stop -----

            if ((Gamepad.current.leftStick.up.wasPressedThisFrame || Gamepad.current.dpad.up.wasPressedThisFrame))
            {//----- if_start -----

                gameClear = true;
                S_Manager.instance.SceneChange("ContentsSelect");
                // �V�[���}�l�[�W���[�ɃN���A���𑗂�
                //S_Manager.gameClear = gameClear;

            }//----- if_stop -----

        }//----- if_stop -----
        else if(!inGoal)
        {//----- else_start -----

            if(this.GetComponent<Renderer>().material.color != Color.yellow)
            {//----- if_start -----

                // �F��ύX����
                this.GetComponent<Renderer>().material.color = Color.yellow;

            }//----- if_stop -----

            gameClear = false;

            // �V�[���}�l�[�W���[�ɃN���A���𑗂�
           // S_Manager.gameClear = gameClear;

        }//----- else_stop -----
    }
}
