using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    // �v���C���[�I�u�W�F�N�g�i�[
    private GameObject playerObject;

    // �J�����I�u�W�F�N�g�i�[
    private GameObject cameraObject;

    private Camera camera;


    [Header("�v���C���[�̃^�O")]
    [SerializeField] private string playerTagName = "Player";
    
    [Header("�J�����̕\������c��")]
    [SerializeField] private int cameraDefaultHight = 9;
    [SerializeField] private int cameraDefaultWidth = 16;
    private float cameraDefaultAspect;  // �ݒ肵�����A�X�y�N�g��  �c/���Ōv�Z

    [Header("�J��������������Y���W")]
    public float cameraTargetPosY = 3.0f;

    [Header("�J���������̋��ւ̈ړ��ɂ�����b��")]
    [SerializeField] private float cameraMoveSecond = 0.45f;
    // �J�����̈ړ����x�B�O�p�֐��ɕ��荞�ނ̂Ŋp���x�ɂ��Ă���B
    private float moveAnglarAccelaration;
    private float moveAnglarVelocity = 0;

    
    private enum ACCEL_TYPE
    {
        ACCELERATION,
        DECELERATION,
        P_ACCELERATION,
        P_DECELERATION,
    }
    private enum SLIDE_TYPE
    {
        ONE,
        HALF,
        QUARTER
    }
    [Header("�J�����̈ړ��^�C�v")]
    [Tooltip("���X�ɉ�����������")]
    [SerializeField] private ACCEL_TYPE accelType = ACCEL_TYPE.DECELERATION;
    [Tooltip("�~�������猩���ψ�")]
    [SerializeField] private SLIDE_TYPE slideType = SLIDE_TYPE.QUARTER;

    private enum FOLLOW_TYPE
    {
        DEFAULT,
        LINER,
        SECTION_FOLLOW
    }
    [Header("�J�����̒Ǐ]�^�C�v")]
    [SerializeField] private FOLLOW_TYPE followType = FOLLOW_TYPE.DEFAULT;
    [Header("�v���C���[��ǂ�������Œᑬ�x")]
    [SerializeField] private float minFollowSpeed = 3;
    private float maxFollowSpeed = 13;
    private float followSpeed = 5.0f;
    private float nowfollowSpeedIndex;

    [Header("�v���C���[�𗎂�������n�_")]
    [Tooltip("���[����̋���")]
    [SerializeField] private float followPosition = 8.0f;
    [Header("�v���C���[�Ǐ]���J�n���鋗��")]
    [Tooltip("����������n�_����̕�")]
    [SerializeField] private float followLenge = 2.0f;
    [Header("�v���C���[���Ǐ]���J�����̂ǂ̋����܂ōs���邩")]
    [SerializeField] private float maxFollowPosition = 20.0f;
    private float anglarIndex;

    

    [Header("���݃J�������������Ă�����ԍ�(0�X�^�[�g)")]
    [SerializeField] private int cameraTargetNumber = 0;

    [Header("�J�������ړ������������̐�")]
    [SerializeField] private int stageSectionNumber = 3;

    [Header("��悲�Ƃ̕\���͈�")]
    [SerializeField] private List<float> l_stageLength = new List<float>();

    [Header("�f�o�b�O�p�B��悪�؂�ւ�閈�ɃJ��������������X���W�̃��X�g")]
    [SerializeField] private List<float> l_cameraTargetPosX = new List<float>();

    

    private float cameraDistance = 15;


    private float cameraSize;

    

    // �J�������e���Ńv���C���[��Ǐ]����͈͂��i�[���郊�X�g
    [SerializeField] private List<float> l_cameraMoveLength = new List<float>();

    

    // �J���������ɒ���������ԍ�
    [SerializeField] private int cameraTargetNextNumber;

    // �J�����̈ړ��\�͈͂����v���āA
    private float cameraAddDistance = 0;

    // �v���C���[�̃|�W�V�����i�[
    Vector3 playerPosition;
    // �J�����̃|�W�V�����i�[
    Vector3 cameraPosition;
    
    
    // �J�������\������X�͈͂̔����B��ʊO�ɏo�����̔���A�J�����̃^�[�Q�b�g�Ԃ̋����v�Z�Ɏg�p
    private float cameraTargetToEdgeDistance;

    // Start is called before the first frame update
    void Start()
    {
        // �v���C���[�^�O����v���C���[��T��
        playerObject = GameObject.FindWithTag(playerTagName);
        if (playerObject == null)
        {
            Debug.LogError("�v���C���[�I�u�W�F�N�g��������܂���");
        }

        // �J�����I�u�W�F�N�g�i�[
        cameraObject = this.gameObject;
        if (cameraObject == null)
        {
            Debug.LogError("�J�����I�u�W�F�N�g��������܂���");
        }
        camera = this.GetComponent<Camera>();
        if (camera == null)
        {
            Debug.LogError("�J�������R���|�[�l���g����Ă��܂���");
        }

        // �J�����̐ݒ�T�C�Y���擾
        cameraSize = camera.orthographicSize;

        Debug.Log(cameraSize);

        
        cameraDefaultAspect =(float) cameraDefaultHight / (float)cameraDefaultWidth;
        
        // �J�������\�����Ă���͈͂��v�Z
        cameraTargetToEdgeDistance = cameraSize / cameraDefaultAspect;
        Debug.Log(cameraTargetToEdgeDistance);
        // �e�X�e�[�W���͈̔͂��w�肵�Ă��邩�`�F�b�N����B
        if (l_stageLength.Count < stageSectionNumber)
        {
            for (int i = l_stageLength.Count; i < stageSectionNumber; i++)
            {
                l_stageLength.Add(cameraTargetToEdgeDistance * 2);
            }
           
        }
        // ���͈͂��\���͈͈ȉ��ł���΁A�\���͈͂ɌŒ�
        for (int i = 0; i < stageSectionNumber; i++)
        {
            if(l_stageLength[i] <cameraTargetToEdgeDistance*2)
            {
                l_stageLength[i] = cameraTargetToEdgeDistance * 2;
            }
        }


        // �J�����̋�斈�̊�n�_���v�Z
        for (int i = 0; i < stageSectionNumber; i++)
        {
            // �J�������v���C���[��Ǐ]����͈͂��v�Z
            l_cameraMoveLength.Add(l_stageLength[i] - cameraTargetToEdgeDistance * 2);
            if (i == 0)
            {
                // �ŏ��̋���0�Ɏw��
                l_cameraTargetPosX.Add(0);
            }
            else
            {
                // �ړ��͈͂̍��v���v�Z
                cameraAddDistance += l_cameraMoveLength[i - 1];
                // �e���̃J�����Œ�ʒu���v�Z�B�ЂƂO�܂ł̈ړ�����␳�l�Ƃ��ĉ��Z
                l_cameraTargetPosX.Add(i * cameraTargetToEdgeDistance * 2 + cameraAddDistance);

            }

        }

        cameraTargetNextNumber = cameraTargetNumber;
        switch(slideType)
        {
            case SLIDE_TYPE.ONE:
                anglarIndex = 0.5f;
                break;
            case SLIDE_TYPE.HALF:
                anglarIndex = 1;
                break;
            case SLIDE_TYPE.QUARTER:
                anglarIndex = 2;
                break;
        }

        //maxFollowSpeed = playerObject.GetComponent<MovePlayer3_3>().moveSetting.defaultMoveSpeed;
        nowfollowSpeedIndex = maxFollowSpeed / (maxFollowPosition - followPosition);
        

        moveAnglarAccelaration = Mathf.PI / 2 / (cameraMoveSecond*60*anglarIndex);

        // �J�����̈ʒu�������̋��Ɉړ�
        cameraObject.transform.position = new Vector3(l_cameraTargetPosX[cameraTargetNumber], cameraTargetPosY, -cameraDistance);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        playerPosition = playerObject.transform.position;
        cameraPosition = cameraObject.transform.position;
        if (cameraTargetNumber == cameraTargetNextNumber)
        {
            followSpeed = Mathf.Abs(((playerPosition.x+cameraTargetToEdgeDistance-followPosition) - camera.transform.position.x)) * nowfollowSpeedIndex;
            if (followSpeed < minFollowSpeed)
            {
                followSpeed = minFollowSpeed;
            }

            switch (followType)
            {
                case FOLLOW_TYPE.DEFAULT:
                    // �e���̃J�����̍��[�����_����A�E�[�܂ł̊ԂɃv���C���[��Ǐ]����|�C���g�����݂���΃J�����ɒǏ]������B
                    if (l_cameraTargetPosX[cameraTargetNumber] < playerPosition.x + cameraTargetToEdgeDistance / 2 &&
                        l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] > playerPosition.x + cameraTargetToEdgeDistance / 2)
                    {
                        cameraObject.transform.position = new Vector3(playerPosition.x + cameraTargetToEdgeDistance / 2, cameraTargetPosY, -cameraDistance);
                    }
                    break;
                case FOLLOW_TYPE.LINER:
                    // �e���̃J�����̍��[�����_����A�E�[�܂ł̊ԂɃv���C���[��Ǐ]����|�C���g�����݂���΃J�����ɒǏ]������B
                    if (l_cameraTargetPosX[cameraTargetNumber]-cameraTargetToEdgeDistance+followPosition+followLenge < playerPosition.x&&
                        l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] - cameraTargetToEdgeDistance + followPosition - followLenge > playerPosition.x )
                    {
                        Debug.Log("�J�����F�����ړ��B�͈͓��̎���");
                        cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(playerPosition.x + cameraTargetToEdgeDistance - followPosition, cameraTargetPosY, -cameraDistance), followSpeed * Time.deltaTime);
                    }
                    else if (camera.transform.position.x != l_cameraTargetPosX[cameraTargetNumber] && playerPosition.x + cameraTargetToEdgeDistance - followPosition <= l_cameraTargetPosX[cameraTargetNumber])
                    {
                        Debug.Log("�J�����F�����ړ��B���[�Ȃ̂ɓ��B���ĂȂ�����");
                        cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(l_cameraTargetPosX[cameraTargetNumber], cameraTargetPosY, -cameraDistance), followSpeed * Time.deltaTime);
                    }
                    else if (camera.transform.position.x != l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] &&
                            playerPosition.x + cameraTargetToEdgeDistance - followPosition >= l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber])
                    {
                        Debug.Log("�J�����F�����ړ��B�E�[�Ȃ̂ɓ��B���Ă��Ȃ�����");
                        cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber], cameraTargetPosY, -cameraDistance), followSpeed * Time.deltaTime);
                    }
                    break;
                case FOLLOW_TYPE.SECTION_FOLLOW:
                    break;
            }

            // �J�����̕\�����E�̉E�[�Ƀv���C���[�����B�����Ƃ�
            if (l_cameraTargetPosX[cameraTargetNumber] + l_cameraMoveLength[cameraTargetNumber] + cameraTargetToEdgeDistance
                < playerPosition.x)
            {
                // ���̋�悪�ݒ肵�����̌��E�łȂ���ΉE���ɐݒ�
                if (cameraTargetNumber + 1 < stageSectionNumber)
                {

                    cameraTargetNextNumber = cameraTargetNumber + 1;

                }

            }
            // ���[�ɓ��B�����Ƃ�
            else if (l_cameraTargetPosX[cameraTargetNumber] - cameraTargetToEdgeDistance
                    > playerPosition.x)
            {
                // ���̋�悪0�ȉ��łȂ���΍����ɐݒ�
                if (cameraTargetNumber - 1 >= 0)
                {
                    cameraTargetNextNumber = cameraTargetNumber - 1;
                }
            }
        }
        // �J���������_�ԍ����X�V���ꂽ��
        // ���̔ԍ����E���ł����
        if (cameraTargetNextNumber > cameraTargetNumber)
        {
            // ���݂̃J�����̍��W�����̒����_�łȂ��ԃJ�����𓙑������^��������
            if (cameraObject.transform.position.x != l_cameraTargetPosX[cameraTargetNextNumber])
            {
                // �p���x���X�V
                moveAnglarVelocity -= moveAnglarAccelaration;
                switch(accelType)
                {
                    case ACCEL_TYPE.DECELERATION:
                        // �J�������w��b�������Ĉړ�������B
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.ACCELERATION:
                        // �J�������w��b�������Ĉړ�������B
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_DECELERATION:
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_ACCELERATION:
                        cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                }
                // �J�������w��b�������Ĉړ�������B
                //cameraObject.transform.position += new Vector3(cameraTargetToEdgeDistance*2 * Mathf.Abs(Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                //cameraObject.transform.position = Vector3.MoveTowards(cameraObject.transform.position, new Vector3(l_cameraTargetPosX[cameraTargetNextNumber], cameraTargetPosY, -cameraDistance), cameraMoveSecond * Time.deltaTime);

                // �J�������ڕW�n�_�𒴂����ꍇ�A�ڕW�n�_�ɌŒ肷��
                if (cameraObject.transform.position.x > l_cameraTargetPosX[cameraTargetNextNumber])
                {
                    Debug.Log("�J�������B���܂����I");
                    cameraObject.transform.position = new Vector3(l_cameraTargetPosX[cameraTargetNextNumber], cameraTargetPosY, -cameraDistance);
                    // ���x��������
                    moveAnglarVelocity = 0;
                    // ���݂̔ԍ����X�V
                    cameraTargetNumber = cameraTargetNextNumber;
                }
            }
            else
            {
                
            }
        }
        // ���̔ԍ������[�ł����
        else if (cameraTargetNextNumber < cameraTargetNumber)
        {
            // ���݂̃J�����̍��W�� (���̒����_ + �v���C���[�Ǐ]�͈�) �o�Ȃ��ԁA�J�����𓙑������^��������
            if (cameraObject.transform.position.x != l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber])
            {
                // �p���x���X�V
                moveAnglarVelocity -= moveAnglarAccelaration;
                switch (accelType)
                {
                    case ACCEL_TYPE.DECELERATION:
                        // �J�������w��b�������Ĉړ�������B
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.ACCELERATION:
                        // �J�������w��b�������Ĉړ�������B
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * Mathf.Abs(Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_DECELERATION:
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Cos(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                    case ACCEL_TYPE.P_ACCELERATION:
                        cameraObject.transform.position -= new Vector3(cameraTargetToEdgeDistance * 2 * (Mathf.Sin(moveAnglarVelocity)) * (1 / (cameraMoveSecond * 60f)), 0, 0);
                        break;
                }

                //cameraObject.transform.position = Vector3.MoveTowards(
                //    cameraObject.transform.position,
                //    new Vector3(l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber],
                //    cameraTargetPosY, -cameraDistance), cameraMoveSecond * Time.deltaTime);
                
                // �J�������ڕW�n�_�𒴂����ꍇ�A�ڕW�n�_�ɌŒ肷��
                if (cameraObject.transform.position.x < l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber]+0.1f)
                {
                    cameraObject.transform.position = new Vector3(l_cameraTargetPosX[cameraTargetNextNumber] + l_cameraMoveLength[cameraTargetNextNumber],
                                                                  cameraTargetPosY, -cameraDistance);
                    Debug.Log("�J�������B���܂����I");
                    // ���x��������
                    moveAnglarVelocity = 0;
                    // ���݂̔ԍ����X�V
                    cameraTargetNumber = cameraTargetNextNumber;
                }
            }
            else
            {
                
            }
        }

    }

    // �Q�[���I�����Ƀ��X�g��j������B
    private void OnApplicationQuit()
    {

        l_cameraMoveLength.Clear();
        l_cameraTargetPosX.Clear();
        l_stageLength.Clear();
        
    }
}
