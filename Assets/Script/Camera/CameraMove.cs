using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMove : MonoBehaviour
{
    // �v���C���[�I�u�W�F�N�g�i�[�p
    private GameObject playerObject;

    // �J������Transform�i�[�p
    private Transform cameraTrans;

    //�J�����̈ʒu�i�[�p
    private Vector3 cameraPosition;

    [Header("�v���C���[��")]
    [SerializeField] private string playerName = "Player";

    [Header("�J�����̒����_")]
    [SerializeField] private float cameraTargetPosX = 0.0f;
    [SerializeField] private float cameraTargetPosY = 3.0f;

    [Header("�J�����Ƃ̋���")]
    [SerializeField] private float toPlayerDistance = 5.0f;



    // Start is called before the first frame update
    void Start()
    {
        // playerName�̃I�u�W�F�N�g�������ăv���C���[�I�u�W�F�N�g�Ƃ��Ċi�[
        playerObject = GameObject.Find(playerName);
        if(playerObject == null)
        {//----- if_start -----

            Debug.LogError("�v���C���[�I�u�W�F�N�g��������܂���");
            return;

        }//----- if_stop -----

        // �J������Transform���i�[
        cameraTrans = this.gameObject.transform;
        if(cameraTrans == null)
        {//----- if_start -----

            Debug.LogError("�J�����̃g�����X�t�H�[����������܂���");
            return;

        }//----- if_stop -----
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}

    private void LateUpdate()
    {
        // �J�����|�W�V�����ɒ����_�ƃv���C���[����̋�������
        cameraPosition = new Vector3(cameraTargetPosX, cameraTargetPosY, toPlayerDistance);

        // �J�����̈ʒu������
        // �v���C���[�I�u�W�F�N�g�̈ʒu����J�����|�W�V���������炷�B
        cameraTrans.position = playerObject.transform.position + cameraPosition;
    }
}
