using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBack : MonoBehaviour
{
    // Transform
    private Transform trans;

    // �J������Transform
    private Transform cameraTrans;

    // �J�������W�v�Z�p
    private Vector3 oldCameraPos;
    private Vector3 nowCameraPos;
    private float cameraMoveDisX;

    [Header("�J�����̈ړ��x�ɑ΂���Ǐ]�w��")]
    [SerializeField,Range(0.0f,1.0f)] private float chaseIndex = 1.0f;
    


    // Start is called before the first frame update
    void Start()
    {
        trans = transform;
        cameraTrans = GameObject.FindWithTag("MainCamera").transform;
        if(cameraTrans == null)
        {
            Debug.LogError("�J������������܂���");
        }
        oldCameraPos = cameraTrans.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        // ���݂̃J�����̈ʒu��ۑ�
        nowCameraPos = cameraTrans.position;

        // 1�t���[���Ԃł̃J�����̈ړ��l���v�Z
        cameraMoveDisX = nowCameraPos.x - oldCameraPos.x;

        // �|�W�V�������ړ��l�Ɏw�����|�����������ړ�
        var pos = trans.position;
        pos.x += (cameraMoveDisX*chaseIndex);
        trans.position = pos;

        // 1�t���[���O�̈ʒu�Ƃ��ĕۑ�
        oldCameraPos = nowCameraPos;
    }
}
