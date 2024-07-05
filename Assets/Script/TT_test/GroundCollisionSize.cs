using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollisionSize : MonoBehaviour
{
    [Header("�v���C���[�̃^�O")]
    [SerializeField] private string playerTagName = "Player";
    [Header("�݂̗͂]�T")]
    [Tooltip("�v���C���[�T�C�Y�̔����ɏ�Z����")]
    [SerializeField] private float colScaleMarginIndex = 1.2f;
    // [Player] �I�u�W�F�N�g�i�[�p
    private GameObject playerObject;


    // 3��[BoxCollider]���i�[������B
    // 0�F���S��Collider
    // 1�F�@�E��Collider
    // 2�F�@����Collider
    private BoxCollider[] groundCollider = new BoxCollider[3];

    // ���E�� [BoxCollider] ��X�T�C�Y
    private float bothScaleX;
    // ���E�� [BoxCollider] �̒��SX
    private float bothCollisionCentorX;
    // �^�񒆂� [BoxCollider] ��X�T�C�Y
    private float bottomCollisionScaleX;

    [System.NonSerialized] public bool colSizeSetFg = false;

    
    void Awake()
    {
        // �v���C���[�^�O����v���C���[��T��
        playerObject = GameObject.FindWithTag(playerTagName);
        if (playerObject == null)
        {//----- if_start -----

            Debug.LogError("�v���C���[�I�u�W�F�N�g��������܂���");

        }//----- if_stop -----

        // �R���|�[�l���g���Ă���S�Ă�[BoxCollider]���擾
        groundCollider = GetComponents<BoxCollider>();
        // �O�R���|�[�l���g����Ă��Ȃ���΃G���[
        if(groundCollider.Length != 3)
        {//----- if_start -----

            Debug.LogError("[BoxCollider]������܂���");

        }//----- if_stop -----

        // Collision��ł̃v���C���[�T�C�Y�̔������v�Z�B�݂̗͂]�T���������邽��1.2���|����
        bothScaleX = (playerObject.transform.localScale.x / 2) / transform.localScale.x * colScaleMarginIndex;

        // �R���W�����̑傫���̔�������T�C�Y�̔������������ƂŁA���E�̒��S���v�Z
        bothCollisionCentorX = 0.5f - bothScaleX / 2;

        // �R���W�����̑傫�����獶�E�̑傫�����������ƂŁA�^�񒆂̃R���W�����̑傫�����v�Z
        bottomCollisionScaleX = 1.0f - bothScaleX * 2;



        //===== �^�񒆂�[BoxCollider] =====

        // isTrigger���t���Ă���ΐ؂�
        if(groundCollider[0].isTrigger == true)
        {//----- if_start -----

            groundCollider[0].isTrigger = false;

        }//----- if_stop -----

        // �ǐ^�񒆂ɒ��S���Œ�
        groundCollider[0].center = Vector3.zero;

        // X�T�C�Y�� [bottomCollisionScaleX] �ɕύX
        groundCollider[0].size = new Vector3(bottomCollisionScaleX, groundCollider[0].size.y, groundCollider[0].size.z);



        //===== �E��[BoxCollider] =====

        // isTrigger���t���Ă��Ȃ���Εt����
        if (groundCollider[1].isTrigger == false)
        {//----- if_start -----

            groundCollider[1].isTrigger = true;

        }//----- if_stop -----

        // �E��( + )�� [Trigger] �R���W�����̒��S���w��
        groundCollider[1].center = new Vector3(bothCollisionCentorX, 0, 0);
        // X�T�C�Y�� [bothScaleX] �ɕύX
        groundCollider[1].size = new Vector3(bothScaleX, groundCollider[1].size.y, groundCollider[1].size.z);


        // isTrigger���t���Ă��Ȃ���Εt����
        if (groundCollider[2].isTrigger == false)
        {//----- if_start -----

            groundCollider[2].isTrigger = true;

        }//----- if_stop -----

        // ����( - )�� [Trigger] �R���W�����̒��S���w��
        groundCollider[2].center = new Vector3(-bothCollisionCentorX, 0, 0);
        // X�T�C�Y�� [bothScaleX] �ɕύX
        groundCollider[2].size = new Vector3(bothScaleX, groundCollider[2].size.y, groundCollider[2].size.z);

        

    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if(colSizeSetFg == true)
        {
            // Collision��ł̃v���C���[�T�C�Y�̔������v�Z�B�݂̗͂]�T���������邽��1.2���|����
            bothScaleX = (Mathf.Abs(playerObject.transform.localScale.x / 2)) / transform.localScale.x * colScaleMarginIndex;

            // �R���W�����̑傫���̔�������T�C�Y�̔������������ƂŁA���E�̒��S���v�Z
            bothCollisionCentorX = 0.5f - bothScaleX / 2;

            // �R���W�����̑傫�����獶�E�̑傫�����������ƂŁA�^�񒆂̃R���W�����̑傫�����v�Z
            bottomCollisionScaleX = 1.0f - bothScaleX * 2;



            //===== �^�񒆂�[BoxCollider] =====

           

            // �ǐ^�񒆂ɒ��S���Œ�
            groundCollider[0].center = Vector3.zero;

            // X�T�C�Y�� [bottomCollisionScaleX] �ɕύX
            groundCollider[0].size = new Vector3(bottomCollisionScaleX, groundCollider[0].size.y, groundCollider[0].size.z);



            //===== �E��[BoxCollider] =====

            

            // �E��( + )�� [Trigger] �R���W�����̒��S���w��
            groundCollider[1].center = new Vector3(bothCollisionCentorX, 0, 0);
            // X�T�C�Y�� [bothScaleX] �ɕύX
            groundCollider[1].size = new Vector3(bothScaleX, groundCollider[1].size.y, groundCollider[1].size.z);


            

            // ����( - )�� [Trigger] �R���W�����̒��S���w��
            groundCollider[2].center = new Vector3(-bothCollisionCentorX, 0, 0);
            // X�T�C�Y�� [bothScaleX] �ɕύX
            groundCollider[2].size = new Vector3(bothScaleX, groundCollider[2].size.y, groundCollider[2].size.z);
        }
    }
}
