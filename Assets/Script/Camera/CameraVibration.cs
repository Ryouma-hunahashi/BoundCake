using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraVibration : MonoBehaviour
{
    // �J��������X�N���v�g�擾
    private CameraController controller;


    [Header("�h��鎞��")]
    [SerializeField] private float vibeTime = 0.3f;
    [Header("�w�i�̗h��鎞��")]
    [SerializeField] private float backGroundvibeTime = 0.5f;
    [Tooltip("������̎���")]
    [SerializeField] private float onePeriodTime = 0.05f;

    [Header("�h��̋���")]
    [Tooltip("�J�����̐U��")]
    public float cameraAmplitude = 0.2f;
    [Tooltip("�w�i�̐U��")]
    public float backAmplitude = -0.2f;

    // �w�i�̃g�����X�t�H�[���i�[
    private Transform backGroundTrans;
    // �w�i�̊�{Y���W�擾
    private float defaultBackPosY;

    // �h�������x
    private float vibeAccelaration;
    // �h��̑��x
    private float vibeVelocity = 0;
    //private float defaultRandomoOffset;

    // �J�����̊�{�|�W�V����
    private float defaultTargetPosY;
    //�S�̗̂h��t���O
    [System.NonSerialized] public bool vibeFg = false;
    // �w�i�̗h��t���O
    bool backVibeFg = false;
    // �J�����̗h��t���O
    bool cameraVibeFg = false;
    // �h��n�߂���̎���
    private float vibeElapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // ���̃I�u�W�F�N�g���̃J�����R���g���[���[���擾
        controller = GetComponent<CameraController>();
        // ������΃G���[
        if(controller == null)
        {
            Debug.LogError("[CameraController] ��������܂���");
        }

        // �w�i�̃^�O����g�����X�t�H�[�����擾
        backGroundTrans = GameObject.FindWithTag("BackGround").transform;

        // �w�i�̊�{�|�W�V�������擾
        defaultBackPosY = backGroundTrans.position.y;

        // �J�����̊�{�|�W�V�������擾
        defaultTargetPosY = controller.cameraTargetPosY;

        //nowAmplitude = maxAmplitude;

        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
//#if UNITY_EDITOR
//        // �f�o�b�O�p�B�X�y�[�X���������Ηh�炷
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            vibeFg = true;
//            //nowAmplitude = maxAmplitude;

//        }
//#endif
        // �h�炷�w�����o���Ƃ�
        if(vibeFg == true)
        {
            // �w�i�A�J����������h�炷�w�����o��
            if(backVibeFg == false && cameraVibeFg == false)
            {
                backVibeFg = true;
                cameraVibeFg = true;
            }
            
            // �G��360�x����������x���t���[�������ĕ������ĉ����x���o��
            vibeAccelaration = (2 * Mathf.PI) / (onePeriodTime * 60);
            // ���x�����Z
            vibeVelocity += vibeAccelaration;
            // ��������̎��Ԃ����Z
            vibeElapsedTime += Time.deltaTime;
            // �w�i��h�炷�w�����o�Ă����
            if (backVibeFg == true)
            {
                // �w�i�̃|�W�V�������ꎞ�i�[
                var backPos = backGroundTrans.position;
                // Y��ݒ肵���U�����ő�Ƃ��ē�����
                backPos.y = defaultBackPosY + (backAmplitude * Mathf.Sin(vibeVelocity));
                backGroundTrans.position = backPos;
                // ���Ԃ��o�߂����
                if (vibeElapsedTime > backGroundvibeTime)
                {
                    // �S��������
                    vibeElapsedTime = 0f;
                    vibeVelocity = 0;
                    backPos.y = defaultBackPosY;
                    backGroundTrans.position = backPos;
                    backVibeFg = false;
                    vibeFg = false;
                }
            }
            // �J������h�炷�w�����o�Ă����
            if(cameraVibeFg == true)
            {
                // �J�����̃|�W�V�������ꎞ�i�[
                var cameraPos = transform.position;
                cameraPos.y = defaultTargetPosY + (cameraAmplitude * Mathf.Sin(vibeVelocity));
                transform.position = cameraPos;


                //�S�~�̎c�[�B�����œ��������Ƃ������c�B�g��Ȃ��Ǝv��
                //controller.cameraTargetPosY = GetVibrationPosition(maxAmplitude, randomOffset, vibeElapsedTime, defaultTargetPosY);
                //transform.position = new Vector3(transform.position.x, GetVibrationPosition(maxAmplitude, randomOffset, vibeElapsedTime, defaultTargetPosY), transform.position.z);

                // ���Ԃ��o�߂����
                if (vibeElapsedTime > vibeTime)
                {
                    // �J�����̗h���������
                    cameraVibeFg = false;

                    //nowAmplitude = 0.0f;
                    controller.cameraTargetPosY = defaultTargetPosY;
                    cameraPos.y = defaultTargetPosY;
                    transform.position = cameraPos;
                }
            }
        }
        

        


        
        
        
        
        
    }

    //private float GetVibrationPosition(float strength,float offset,float elapsedTime,float targetPosY)
    //{
    //    var localStrength = strength;
    //    var localOffset = offset;
    //    var random = GetRandomNoise(localOffset, localStrength, elapsedTime);

    //    random *= localStrength;
    //    nowAmplitude = maxAmplitude;
    //    var ratio = 1.0f - elapsedTime / vibeTime;

    //    nowAmplitude *= ratio;

    //    random = Mathf.Clamp(random, -nowAmplitude, nowAmplitude);

    //    return (defaultTargetPosY + random);
    //}

    //private float GetRandomNoise(float offset, float speed, float time)
    //{
    //    var vibeNoise = Mathf.PerlinNoise(offset + speed * time, 0.0f);

    //    return ((vibeNoise - 0.5f) * 2.0f);
    //}

}
