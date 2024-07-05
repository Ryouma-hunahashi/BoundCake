using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundWide : MonoBehaviour
{
    private enum WIDE_VELOCITY
    {
        UP,
        DOWN,
        RIGHT,
        LEFT,
        BOTH,
    };

    //private waveController waveController;

    private Transform groundTransform;
    private Vector3 groundNomalPosition;
    private Vector3 groundNomalLocalScale;
    private Vector3 groundWideStartScale;
    private int onPlayerFg = 0;

    [Header("�X�C�b�`�̃I�u�W�F�N�g")]
    [SerializeField] private GameObject switchObject;
    private VariousSwitches_2 varSwitch;    // �X�C�b�`�X�N���v�g
    [Header("��]������{�r���̃I�u�W�F�N�g")]
    [SerializeField] private GameObject[] bobbinObj = new GameObject[1];
    private List<ObjectRotation> rotationScript = new List<ObjectRotation>();

    [Header("�n�ʂ��L�������")]
    [SerializeField] private WIDE_VELOCITY groundWideVelocity = WIDE_VELOCITY.BOTH;

    enum Wide_Type
    {
        [Tooltip("�ő�T�C�Y�܂ł̊ԁA�g�𓖂Ă��牽�x�ł��L�΂���")]
        MULTI,
        [Tooltip("�X�C�b�`�ǂݎ��̂�\n�L�т���L�т��܂܂ł��B")]
        ONCE,
    }

    [Header("�L�������")]
    [SerializeField] private Wide_Type wideType = Wide_Type.MULTI;

    [Header("�L����ő�T�C�Y")]
    [SerializeField] private float groundMaxScaleX = 20;

    [Header("���̑傫���ɖ߂�܂ł̎���")]
    [SerializeField] private float groundReturnTime = 2.0f;
    private float groundWideElapsedTime = 0.0f;     // �L���肫���Ă���̌o�ߎ���

    [Header("�g�̏��")]// ����̓R���g���[���[�̎g�p���@���m�莟��C���X�y�N�^�[�������
    [SerializeField] private float waveSpeed = 7.5f;
    [SerializeField] private float waveLength = 0.225f;

    [Header("�߂�X�s�[�h")]
    [SerializeField] private float returnSpeed = 3.0f;

    private int groundStandardFixed = 1;

    // private GroundCollisionSize groundCollisionSize;

    private bool switchLog = false;
    private bool bobbinFg = true;
    private sbyte rotateIndex = 1;
    private byte rotateFrameCount = 0;

    // Start is called before the first frame update
    void Start()
    {
        groundTransform = transform;
        groundNomalPosition = groundTransform.position;
        groundNomalLocalScale = groundTransform.localScale;
        switch(groundWideVelocity)
        {
            case WIDE_VELOCITY.RIGHT:
                groundStandardFixed = 1;
                break;
            case WIDE_VELOCITY.LEFT:
                groundStandardFixed = -1;
                break;
            case WIDE_VELOCITY.UP:
                groundStandardFixed = 1;
                break;
            case WIDE_VELOCITY.DOWN:
                groundStandardFixed = -1;
                break;
        }
        
        if(switchObject == null)
        {
            Debug.LogError("�X�C�b�`�̃I�u�W�F�N�g��ݒ肵�Ă��������B");
        }
        varSwitch = switchObject.GetComponent<VariousSwitches_2>();
        if(varSwitch == null)
        {
            Debug.LogError("�X�C�b�`�̃X�N���v�g��������܂���");
        }
        //groundCollisionSize = GetComponent<GroundCollisionSize>();
        for(byte i = 0;i<bobbinObj.Length;i++)
        {
            if(bobbinObj[i] != null)
            {
                rotationScript.Add(bobbinObj[i].GetComponent<ObjectRotation>());
            }
            else
            {
                bobbinFg = false;
                break;
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
#if UNITY_EDITOR
        //�f�o�b�O�p
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (groundTransform.localScale.x < groundMaxScaleX)
            {
                groundWideStartScale = groundTransform.localScale;
                onPlayerFg = 1;
                groundWideElapsedTime = 0.0f;
            }
        }
#endif
        switch (wideType)
        {
            case Wide_Type.MULTI:
                

                if (switchLog != varSwitch.switchStatus)
                {
                    groundWideStartScale = groundTransform.localScale;
                    onPlayerFg = 1;
                    groundWideElapsedTime = 0.0f;
                    switchLog = varSwitch.switchStatus;


                }


                if (onPlayerFg == 1)
                {
                    var scale = groundTransform.localScale;
                    var pos = groundTransform.position;
                    switch (groundWideVelocity)
                    {

                        case WIDE_VELOCITY.UP:
                            scale.x += waveSpeed * Time.deltaTime;
                            //groundTransform.localScale = scale;

                            pos.y = groundNomalPosition.y + groundStandardFixed
                                    * (scale.x - groundNomalLocalScale.x) / 2;
                            if(bobbinFg)
                            {
                                for(byte i  = 0; i < bobbinObj.Length; i++)
                                {
                                    if(rotationScript[i]!=null)
                                    {
                                        rotationScript[i].RotateStart(20, 5);
                                    }
                                    var bobbinPos = bobbinObj[i].transform.position;
                                    bobbinPos.y = pos.y + scale.x / 2;
                                    bobbinObj[i].transform.position = bobbinPos;
                                }
                            }
                            
                            //groundTransform.position = pos;
                            if (scale.x >= groundMaxScaleX)
                            {

                                scale.x = groundMaxScaleX;
                                //groundTransform.localScale = scale;

                                pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;
                                //groundTransform.position = pos;
                                onPlayerFg = 0;
                            }
                            else if (scale.x - groundWideStartScale.x >= waveSpeed * waveLength)
                            {
                                onPlayerFg = 0;
                            }
                            break;
                        case WIDE_VELOCITY.DOWN:
                            scale.x += waveSpeed * Time.deltaTime;
                            pos.y = groundNomalPosition.y + groundStandardFixed
                                    * (scale.x - groundNomalLocalScale.x) / 2;
                            if (bobbinFg)
                            {
                                for (byte i = 0; i < bobbinObj.Length; i++)
                                {
                                    if (rotationScript[i] != null)
                                    {
                                        rotationScript[i].RotateStart(-20, 5);
                                    }
                                    var bobbinPos = bobbinObj[i].transform.position;
                                    bobbinPos.y = pos.y - scale.x / 2;
                                    bobbinObj[i].transform.position = bobbinPos;
                                }
                            }
                            if (scale.x >= groundMaxScaleX)
                            {

                                scale.x = groundMaxScaleX;


                                pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                onPlayerFg = 0;
                            }
                            else if (scale.x - groundWideStartScale.x >= waveSpeed * waveLength)
                            {
                                onPlayerFg = 0;
                            }
                            break;
                        case WIDE_VELOCITY.RIGHT:

                            scale.x += waveSpeed * Time.deltaTime;
                            //groundTransform.localScale = scale;

                            pos.x = groundNomalPosition.x + groundStandardFixed
                                    * (scale.x - groundNomalLocalScale.x) / 2;
                            //groundTransform.position = pos;

                            if (bobbinFg)
                            {
                                for (byte i = 0; i < bobbinObj.Length; i++)
                                {
                                    if (rotationScript[i] != null)
                                    {
                                        rotationScript[i].RotateStart(20, 5);
                                    }
                                    var bobbinPos = bobbinObj[i].transform.position;
                                    bobbinPos.x = pos.x + scale.x / 2;
                                    bobbinObj[i].transform.position = bobbinPos;
                                }
                            }
                            if (scale.x >= groundMaxScaleX)
                            {

                                scale.x = groundMaxScaleX;
                                //groundTransform.localScale = scale;

                                pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;
                                //groundTransform.position = pos;
                                onPlayerFg = 0;
                            }
                            else if (scale.x - groundWideStartScale.x >= waveSpeed * waveLength)
                            {
                                onPlayerFg = 0;
                            }
                            break;
                        case WIDE_VELOCITY.LEFT:
                            scale.x += waveSpeed * Time.deltaTime;
                            pos.x = groundNomalPosition.x + groundStandardFixed
                                    * (scale.x - groundNomalLocalScale.x) / 2;

                            if (bobbinFg)
                            {
                                for (byte i = 0; i < bobbinObj.Length; i++)
                                {
                                    if (rotationScript[i] != null)
                                    {
                                        rotationScript[i].RotateStart(-20, 5);
                                    }
                                    var bobbinPos = bobbinObj[i].transform.position;
                                    bobbinPos.x = pos.x - scale.x / 2;
                                    bobbinObj[i].transform.position = bobbinPos;
                                }
                            }
                            if (scale.x >= groundMaxScaleX)
                            {

                                scale.x = groundMaxScaleX;


                                pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                onPlayerFg = 0;
                            }
                            else if (scale.x - groundWideStartScale.x >= waveSpeed * waveLength)
                            {
                                onPlayerFg = 0;
                            }
                            break;
                        case WIDE_VELOCITY.BOTH:
                            scale.x += 2 * waveSpeed * Time.deltaTime;
                            if (groundTransform.localScale.x >= groundMaxScaleX)
                            {

                                scale.x = groundMaxScaleX;


                                onPlayerFg = 0;
                            }
                            else if (scale.x - groundWideStartScale.x >= waveSpeed * waveLength)
                            {
                                onPlayerFg = 0;
                            }
                            break;
                    }
                    groundTransform.localScale = scale;
                    if (transform.parent != null && transform.parent.CompareTag("Floor"))
                    {
                        groundTransform.parent.position = pos;
                    }
                    else
                    {
                        groundTransform.position = pos;
                    }

                }
                else
                {
                    var scale = groundTransform.localScale;
                    var pos = groundTransform.position;
                    switch (groundWideVelocity)
                    {
                        case WIDE_VELOCITY.UP:
                            if (scale.x > groundNomalLocalScale.x)
                            {
                                groundWideElapsedTime += Time.deltaTime;
                                if (groundWideElapsedTime >= groundReturnTime)
                                {
                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(-20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.y = pos.y+scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }
                                }
                                else if(groundWideElapsedTime>=groundReturnTime/2)
                                {
                                    if(bobbinFg)
                                    {
                                        for(byte i  = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(rotateIndex * 4, 1);
                                            }
                                        }
                                        rotateFrameCount++;
                                        if (rotateFrameCount > 5)
                                        {
                                            rotateIndex *= -1;
                                            rotateFrameCount = 0;
                                        }
                                    }
                                }
                            }
                            else if (scale.x <= groundNomalLocalScale.x)
                            {
                                scale = groundNomalLocalScale;
                                pos.y = groundNomalPosition.y;
                            }
                            break;
                        case WIDE_VELOCITY.DOWN:
                            if (scale.x > groundNomalLocalScale.x)
                            {
                                groundWideElapsedTime += Time.deltaTime;
                                if (groundWideElapsedTime >= groundReturnTime)
                                {
                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.y = pos.y - scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }
                                }
                                else if (groundWideElapsedTime >= groundReturnTime / 2)
                                {
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(rotateIndex * 4, 1);
                                            }
                                        }
                                        rotateFrameCount++;
                                        if (rotateFrameCount > 5)
                                        {
                                            rotateIndex *= -1;
                                            rotateFrameCount = 0;
                                        }
                                    }
                                }
                            }
                            else if (scale.x <= groundNomalLocalScale.x)
                            {
                                scale = groundNomalLocalScale;
                                pos.y = groundNomalPosition.y;
                            }
                            break;
                        case WIDE_VELOCITY.RIGHT:
                            if (scale.x > groundNomalLocalScale.x)
                            {
                                groundWideElapsedTime += Time.deltaTime;
                                if (groundWideElapsedTime >= groundReturnTime)
                                {
                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(-20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.x = pos.x+ scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }
                                }
                                else if (groundWideElapsedTime >= groundReturnTime / 2)
                                {
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(rotateIndex * 4, 1);
                                            }
                                        }
                                        rotateFrameCount++;
                                        if (rotateFrameCount > 5)
                                        {
                                            rotateIndex *= -1;
                                            rotateFrameCount = 0;
                                        }
                                    }
                                }
                            }
                            else if (scale.x <= groundNomalLocalScale.x)
                            {
                                scale = groundNomalLocalScale;
                                pos.x = groundNomalPosition.x;
                            }
                            break;
                        case WIDE_VELOCITY.LEFT:
                            if (scale.x > groundNomalLocalScale.x)
                            {
                                groundWideElapsedTime += Time.deltaTime;
                                if (groundWideElapsedTime >= groundReturnTime)
                                {
                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.x = pos.x - scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }
                                }
                                else if (groundWideElapsedTime >= groundReturnTime / 2)
                                {
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(rotateIndex * 4, 1);
                                            }
                                        }
                                        rotateFrameCount++;
                                        if (rotateFrameCount > 5)
                                        {
                                            rotateIndex *= -1;
                                            rotateFrameCount = 0;
                                        }
                                    }
                                }
                            }
                            else if (scale.x <= groundNomalLocalScale.x)
                            {
                                scale = groundNomalLocalScale;
                                pos.x = groundNomalPosition.x;
                            }
                            break;
                        case WIDE_VELOCITY.BOTH:
                            if (scale.x > groundNomalLocalScale.x)
                            {
                                groundWideElapsedTime += Time.deltaTime;
                                if (groundWideElapsedTime >= groundReturnTime)
                                {
                                    scale.x -= 2 * returnSpeed * Time.deltaTime;
                                }
                            }
                            else if (scale.x <= groundNomalLocalScale.x)
                            {
                                scale = groundNomalLocalScale;
                                pos = groundNomalPosition;
                            }
                            break;
                    }
                    groundTransform.localScale = scale;
                    if(transform.parent != null&&transform.parent.CompareTag("Floor"))
                    {
                        groundTransform.parent.position = pos;
                    }
                    else
                    {
                        groundTransform.position = pos;
                    }
                    

                }
                break;
            case Wide_Type.ONCE:
                if (switchLog != varSwitch.switchStatus)
                {
                    if (varSwitch.switchStatus)
                    {
                        var scale = groundTransform.localScale;
                        var pos = groundTransform.position;
                        switch (groundWideVelocity)
                        {
                            case WIDE_VELOCITY.RIGHT:
                                scale.x += waveSpeed * Time.deltaTime;
                                

                                pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                if (bobbinFg)
                                {
                                    for (byte i = 0; i < bobbinObj.Length; i++)
                                    {
                                        if (rotationScript[i] != null)
                                        {
                                            rotationScript[i].RotateStart(20, 5);
                                        }
                                        var bobbinPos = bobbinObj[i].transform.position;
                                        bobbinPos.x = pos.x + scale.x / 2;
                                        bobbinObj[i].transform.position = bobbinPos;
                                    }
                                }

                                if (scale.x >= groundMaxScaleX)
                                {

                                    scale.x = groundMaxScaleX;
                                    

                                    pos.x = groundNomalPosition.x + groundStandardFixed
                                            * (scale.x - groundNomalLocalScale.x) / 2;
                                    
                                    switchLog = varSwitch.switchStatus;
                                }
                                
                                break;
                            case WIDE_VELOCITY.LEFT:
                                scale.x += waveSpeed * Time.deltaTime;
                                pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                if (bobbinFg)
                                {
                                    for (byte i = 0; i < bobbinObj.Length; i++)
                                    {
                                        if (rotationScript[i] != null)
                                        {
                                            rotationScript[i].RotateStart(-20, 5);
                                        }
                                        var bobbinPos = bobbinObj[i].transform.position;
                                        bobbinPos.x = pos.x - scale.x / 2;
                                        bobbinObj[i].transform.position = bobbinPos;
                                    }
                                }

                                if (scale.x >= groundMaxScaleX)
                                {

                                    scale.x = groundMaxScaleX;


                                    pos.x = groundNomalPosition.x + groundStandardFixed
                                            * (scale.x - groundNomalLocalScale.x) / 2;

                                    switchLog = varSwitch.switchStatus;
                                }
                                
                                break;
                            case WIDE_VELOCITY.UP:
                                scale.x += waveSpeed * Time.deltaTime;
                               

                                pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                if (bobbinFg)
                                {
                                    for (byte i = 0; i < bobbinObj.Length; i++)
                                    {
                                        if (rotationScript[i] != null)
                                        {
                                            rotationScript[i].RotateStart(20, 5);
                                        }
                                        var bobbinPos = bobbinObj[i].transform.position;
                                        bobbinPos.y = pos.y + scale.x / 2;
                                        bobbinObj[i].transform.position = bobbinPos;
                                    }
                                }

                                if (scale.x >= groundMaxScaleX)
                                {

                                    scale.x = groundMaxScaleX;
                 

                                    pos.y = groundNomalPosition.y + groundStandardFixed
                                            * (scale.x - groundNomalLocalScale.x) / 2;
                             
                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                            case WIDE_VELOCITY.DOWN:
                                scale.x += waveSpeed * Time.deltaTime;
                                pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                if (bobbinFg)
                                {
                                    for (byte i = 0; i < bobbinObj.Length; i++)
                                    {
                                        if (rotationScript[i] != null)
                                        {
                                            rotationScript[i].RotateStart(-20, 5);
                                        }
                                        var bobbinPos = bobbinObj[i].transform.position;
                                        bobbinPos.y = pos.y - scale.x / 2;
                                        bobbinObj[i].transform.position = bobbinPos;
                                    }
                                }

                                if (scale.x >= groundMaxScaleX)
                                {

                                    scale.x = groundMaxScaleX;


                                    pos.y = groundNomalPosition.y + groundStandardFixed
                                            * (scale.x - groundNomalLocalScale.x) / 2;

                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                            case WIDE_VELOCITY.BOTH:
                                scale.x += 2 * waveSpeed * Time.deltaTime;
                                if (groundTransform.localScale.x >= groundMaxScaleX)
                                {

                                    scale.x = groundMaxScaleX;


                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                        }
                        groundTransform.localScale = scale;
                        if (transform.parent != null && transform.parent.CompareTag("Floor"))
                        {
                            groundTransform.parent.position = pos;
                        }
                        else
                        {
                            groundTransform.position = pos;
                        }
                    }
                    else
                    {
                        var scale = groundTransform.localScale;
                        var pos = groundTransform.position;
                        switch (groundWideVelocity)
                        {
                            case WIDE_VELOCITY.UP:
                                if (scale.x > groundNomalLocalScale.x)
                                {

                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(-20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.y = pos.y + scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }

                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                }
                                else if (scale.x <= groundNomalLocalScale.x)
                                {
                                    scale = groundNomalLocalScale;
                                    pos = groundNomalPosition;
                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                            case WIDE_VELOCITY.DOWN:
                                if (scale.x > groundNomalLocalScale.x)
                                {
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.y = pos.y - scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }

                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.y = groundNomalPosition.y + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                }
                                else if (scale.x <= groundNomalLocalScale.x)
                                {
                                    scale = groundNomalLocalScale;
                                    pos = groundNomalPosition;
                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                            case WIDE_VELOCITY.RIGHT:
                                if (scale.x > groundNomalLocalScale.x)
                                {
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(-20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.x = pos.x + scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }

                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                }
                                else if (scale.x <= groundNomalLocalScale.x)
                                {
                                    scale = groundNomalLocalScale;
                                    pos = groundNomalPosition;
                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                            case WIDE_VELOCITY.LEFT:
                                if (scale.x > groundNomalLocalScale.x)
                                {
                                    if (bobbinFg)
                                    {
                                        for (byte i = 0; i < bobbinObj.Length; i++)
                                        {
                                            if (rotationScript[i] != null)
                                            {
                                                rotationScript[i].RotateStart(20, 5);
                                            }
                                            var bobbinPos = bobbinObj[i].transform.position;
                                            bobbinPos.y = pos.y - scale.x / 2;
                                            bobbinObj[i].transform.position = bobbinPos;
                                        }
                                    }

                                    scale.x -= returnSpeed * Time.deltaTime;
                                    pos.x = groundNomalPosition.x + groundStandardFixed
                                        * (scale.x - groundNomalLocalScale.x) / 2;

                                }
                                else if (scale.x <= groundNomalLocalScale.x)
                                {
                                    scale = groundNomalLocalScale;
                                    pos = groundNomalPosition;
                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                            case WIDE_VELOCITY.BOTH:
                                if (scale.x > groundNomalLocalScale.x)
                                {
                                    
                                    scale.x -= 2 * returnSpeed * Time.deltaTime;
                                    
                                }
                                else if (groundTransform.localScale.x <= groundNomalLocalScale.x)
                                {
                                    scale = groundNomalLocalScale;
                                    pos = groundNomalPosition;
                                    switchLog = varSwitch.switchStatus;
                                }
                                break;
                        }
                        groundTransform.localScale = scale;
                        if (transform.parent != null && transform.parent.CompareTag("Floor"))
                        {
                            groundTransform.parent.position = pos;
                        }
                        else
                        {
                            groundTransform.position = pos;
                        }
                    }
                }
                break;
        }
    }

    

    //private void OnCollisionEnter(Collision collision)
    //{
    //    // ���������̂��v���C���[�Ȃ�
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        // �v���C���[�̃Q�[���I�u�W�F�N�g���擾
    //        //GameObject playerObj = collision.gameObject.GetComponent<GameObject>();
    //        // �v���C���[���ォ�瓖��������
    //        if (collision.transform.position.y > groundTransform.position.y)
    //        {
    //            if (groundTransform.localScale.x < groundMaxScaleX)
    //            {
    //                groundWideStartScale = groundTransform.localScale;
    //                onPlayerFg = 1;
    //                groundWideElapsedTime = 0.0f;
    //            }

    //        }
    //    }
    //}

    private void OnApplicationQuit()
    {
        rotationScript.Clear();
    }
}

