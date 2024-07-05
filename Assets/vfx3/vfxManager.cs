using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


//=======================4/20�@�X�V�@����==========================---
//update�֐����ł�posY�̎擾�ɏ����ǉ�
//�ړ������g���Ƃ���vfxManager�̐V�K�ϐ�moveFlg��true�ɂ���
//�f�t�H���g�͌Œ菰�ifalse�j
//====================================================================


//=======================4/22�@�X�V�@����==========================---
//�G�̔g�̔g�`���M�U�M�U�ɂ���@�\�ǉ�
//����ɔ���WaveSpawn�֐��i176�s�ڕt�߁j�Ɉ�����V�����ǉ������̂ŁA
//MovePlayer��Enemy�̔g�������̌Ăяo������float�^�̈�����������
//�M�U�M�U�ɂ���Ƃ���1�A�ʏ�̔g�̎���0��n��
//====================================================================

public class vfxManager : MonoBehaviour
{
    // Start is called before the first frame update
    //�����������g�̃G�t�F�N�g�̐ݒ�
    //�g��VFX�I�u�W�F�N�g��ɂ����̃X�N���v�g�����������
    VisualEffect effect;
    //�g�̃G�t�F�N�g�ƘA��������n��
    //[Header("�g�ƘA��������n�ʁi�����蔻��j�̃I�u�W�F�N�g")]
    //[SerializeField] GameObject ground;
    [SerializeField] float testWaveHeight = 0;
    [SerializeField] float testWaveWidth = 0;
    //waveClash2�֐��ł̌v�Z�Ɏg�p����ϐ�
    float clashSpeed = 0;
    float clashTime = 0;
    float clashDistance = 0;
    float dampingDistance = 0;
    float dampingTime = 0;

    public float chargeShift = 0;
    //�z��H�݂����ȂȂɂ��B
    //Buffer�̒��g����Őݒ肵���G�t�F�N�g�Ɉ����n���B

    GraphicsBuffer waveSpeedBuffer; //�g�̃X�s�[�h�̃o�b�t�@
    GraphicsBuffer waveStartPosBuffer;�@//�g�̊J�n�ʒu�̃o�b�t�@
    GraphicsBuffer waveStartTimeBuffer;�@//�g�̊J�n���Ԃ̃o�b�t�@
    GraphicsBuffer waveHeightBuffer;//�g�̍������i�[����o�b�t�@
    GraphicsBuffer waveWidthBuffer;//�g���i�g�̉����j���i�[����o�b�t�@
    GraphicsBuffer waveFrequencyBuffer;//�g�̐U���񐔊i�[����o�b�t�@
    [System.NonSerialized] public GraphicsBuffer endPosBuffer;
    GraphicsBuffer EnemyFlgBuffer;//�g���G�̂��̂��i�g�`���M�U�M�U�ɂ��邩�j���i�[����o�b�t�@

    float endPos = 0;

    //�X�N���v�g��waveSpeedBuffer���̃O���t�B�b�N�o�b�t�@�Ɉ����n���p�̔z��
    //�g�𔭐��������Ƃ����ɏ���������
    [System.NonSerialized] public float[] waveSpeedArray = new float[10];//�g�̑��x�A���l�����Ȃ�E�����A���Ȃ獶����
    [System.NonSerialized] public float[] waveStartTimeArray = new float[10];//�g�̊J�n����
    [System.NonSerialized] public float[] waveStartPosArray = new float[10];//�g�̊J�n�ʒu
    [System.NonSerialized] public float[] waveHeightArray = new float[10];//�U���i�g�̍����j
    [System.NonSerialized] public float[] waveWidthArray = new float[10];//�g���i�g�̉����j
    [System.NonSerialized] public float[] endPosArray = new float[10];
    [System.NonSerialized] public float[] enemyFlgArray = new float[10];//1�Ȃ�M�U�M�U�A0�Ȃ�ʏ�̔g

    //true�Ȃ�c��
    [Header("�c���̏ꍇ�`�F�b�N������")]
    public bool warpWave = false;

    //int waveCnt = 0;

    [SerializeField] private float rideShiftScale = 1.6f;
    [SerializeField] private float grabShiftScale = 1.75f;
    float landScaleY = 0;


    [System.NonSerialized] public sbyte waveSpawnCnt = 0;//���ݔ������Ă���g�̐�
    //======================4��19���E�����ҏW==============================================
    [Header("�X�C�b�`���g���Ƃ��̓`�F�b�N������")]
    [SerializeField] private bool useSwitch = false;
    [SerializeField] private VariousSwitches_2 VariousSwitches_2;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private string playerTag = "Player";
    //=============================�ҏW�I��================================================
    //======================4��20���E�����ҏW==============================================
    [Header("�ړ����̏ꍇ�̓`�F�b�N�����")]
    [SerializeField] private bool moveFlg = false;
    [Header("�V�䎅�̏ꍇ�`�F�b�N������")]
    [SerializeField] private bool topFg = false;
    //=============================�ҏW�I��================================================

    void Start()
    {
        //���̃X�N���v�g���Ǘ�����g��VFX�R���|�[�l���g���擾����
        effect = GetComponent<VisualEffect>();

        //�O���t�B�b�N�o�b�t�@�̐錾
        //�����i�s���A�o�b�t�@�i�z��H�j�̗v�f���Asizeof(�o�b�t�@�ň����ϐ��̌^)�j
        waveSpeedBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveStartPosBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveStartTimeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveHeightBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveWidthBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        endPosBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        EnemyFlgBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        effect.SetFloat("posY", transform.position.y);
        effect.SetFloat("posX", transform.position.x);
        effect.SetFloat("startPosY", transform.position.y);
        
        if(warpWave==true)
        {
            effect.SetFloat("posY", transform.position.x);
            effect.SetFloat("posX", transform.position.y);
            //seffect.SetFloat("ShiftScale", 0);
            //effect.SetFloat("waveLong", transform.parent.gameObject.transform.localScale.y);
        }
        if(topFg)
        {
            effect.SetFloat("DownScaleY2", -0.45f);
            effect.SetFloat("const", 1.2f);
            effect.SetFloat("ShiftScale", -0.45f);

        }
        effect.SetFloat("waveLong", transform.parent.gameObject.transform.localScale.x);
        effect.SendEvent("OnPlay");

        //======================4��19���E�����ҏW==============================================
        if(useSwitch)
        {
            if(VariousSwitches_2==null)
            {
                Debug.Log("Swicth�ݒ肵�ĂȂ�");
            }
        }
        playerObj = GameObject.FindWithTag(playerTag).gameObject;
        //=============================�ҏW�I��================================================

    }

    // Update is called once per frame
    void Update()
    {
        
        
        effect.SetFloat("waveLong", transform.parent.localScale.x);
        effect.SetVector3("playerPos", playerObj.transform.position);
        if(moveFlg)
        {
            if(warpWave)
            {
                effect.SetFloat("posY", transform.position.x);
                effect.SetFloat("posX", transform.position.y);
            }
            else
            {
                effect.SetFloat("posX", transform.position.x);
                effect.SetFloat("posY", transform.parent.parent.position.y);
                effect.SetFloat("startPosY", transform.parent.position.y);
                landScaleY = transform.parent.parent.position.y-transform.parent.position.y;
            }
        }
        if (!topFg)
        {
            if (playerObj.transform.position.x > transform.position.x - 1.5f &&
                playerObj.transform.position.x < transform.position.x + transform.parent.localScale.x + 1.5f)
            {
                if (!warpWave)
                {
                    if (playerObj.transform.position.y > transform.position.y)
                    {
                        effect.SetFloat("DownScaleY2", rideShiftScale+landScaleY);
                    }
                    else
                    {
                        effect.SetFloat("DownScaleY2", grabShiftScale+landScaleY);
                        effect.SetFloat("range", -0.45f - chargeShift);
                    }
                }
            }
        }
        

        ////�X�y�[�X�L�[���������Ƃ�
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    //�J�n���Ԃ̎擾
        //    waveStartTimeArray[pushCnt]=Time.time;
        //    pushCnt++;
        //    waveStartTimeArray[pushCnt] = Time.time;

        //    //�O���t�B�b�N�o�b�t�@�ɔz��̒l��������
        //    waveSpeedBuffer.SetData(waveSpeedArray);
        //    waveStartPosBuffer.SetData(waveStartPosArray);
        //    waveStartTimeBuffer.SetData(waveStartTimeArray);

        //    //�z����������O���t�B�b�N�o�b�t�@��VFX�̃p�����[�^�ɑ��M����
        //    effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        //    effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        //    effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);


        //}
        //effect.SetFloat("posY", transform.position.y);
        //effect.SetFloat("posX", transform.position.x);
        //if (warpWave == true)
        //{
        //    effect.SetFloat("posY", transform.position.x);
        //    effect.SetFloat("posX", transform.position.y);
        //    //effect.SetFloat("waveLong", transform.parent.gameObject.transform.localScale.y);
        //}
    }

    //=========================================================================s
    //�g���������̊֐�()
    //�߂�l�F�����������g�̔ԍ�
    //�����F�i�g�̊J�n�ʒu�A�g�̑��x�j
    //�⑫�E�g�̑��x�����Ȃ�E�����A���Ȃ獶�����Ɉړ�����
    //
    //=========================================================================
    public sbyte WaveSpawn(float waveStartPosX, float waveStartSpeed, float waveHeight, float waveWidth,float enemyFlg)
    {
        waveSpawnCnt++;
        if (waveSpawnCnt >= waveSpeedArray.Length)
        {
            waveSpawnCnt = 0;
        }
        //======================4��14���E�����ҏW==============================================
        if (useSwitch)
        {
            if(VariousSwitches_2==null)
            {
                Debug.Log("Switch�ݒ肵�ĂȂ�");
            }
            if (VariousSwitches_2.switchStatus == false)
            {
                return -1;
            }
        }
        //=============================�ҏW�I��================================================



        //�g�̊J�n���Ԃ̎擾
        waveStartTimeArray[waveSpawnCnt] = Time.time;
        //�g�̑��x�̐ݒ�
        waveSpeedArray[waveSpawnCnt] = waveStartSpeed;
        //�g�̊J�n�ʒu�̐ݒ�
        waveStartPosArray[waveSpawnCnt] = waveStartPosX;
        //�U���̐ݒ�
        waveHeightArray[waveSpawnCnt] = waveHeight;
        //�g���̐ݒ�
        waveWidthArray[waveSpawnCnt] = waveWidth;

        endPosArray[waveSpawnCnt] = 0;

        //�g�`�̐ݒ�
        enemyFlgArray[waveSpawnCnt] = enemyFlg;

        //�e�O���t�B�b�N�o�b�t�@�ɔz��̒l��������
        waveSpeedBuffer.SetData(waveSpeedArray);
        waveStartPosBuffer.SetData(waveStartPosArray);
        waveStartTimeBuffer.SetData(waveStartTimeArray);
        waveHeightBuffer.SetData(waveHeightArray);
        waveWidthBuffer.SetData(waveWidthArray);
        endPosBuffer.SetData(endPosArray);
        EnemyFlgBuffer.SetData(enemyFlgArray);

        //�z����������O���t�B�b�N�o�b�t�@��VFX�̃p�����[�^�ɑ��M����
        effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
        effect.SetGraphicsBuffer("waveWidthBuffer", waveWidthBuffer);
        effect.SetGraphicsBuffer("endPosBuffer",endPosBuffer);
        effect.SetGraphicsBuffer("EnemyFlgBuffer",EnemyFlgBuffer);
        //���������g�̔ԍ���Ԃ�
        return waveSpawnCnt;



    }

    
    //=============================================================
    //�g���m�̏Փˎ��̏����֐�()
    //�߂�l�F�Ȃ�
    //�����F�i�傫�����̔g�i�g�`�j�̔z��̓Y���A���������̔g�i�g�a�j�̔z��̓Y���j
    //�⑫�E
    //

    //=============================================================
    public void WaveClash(int waveNumA, int waveNumB)
    {
        //�傫���g��g�`�A�������g��g�a�Ƃ���

        //�g�̑��x�̐ݒ�
        //�Ԃ��������ɔg�������E����������
        //waveSpeedArray[waveSpawnCnt] = waveStartSpeed;

        //�g�̊J�n�ʒu�̐ݒ�
        //�Ԃ��������ɔg���ړ�������
        //waveStartPosArray[waveSpawnCnt] = waveStartPosX;

        //�U���̐ݒ�
        //�傫���g�̐U���i�����j�ɏ��������̔g�̐U���̐��l�����Z����
        //���������̔g�������i�������O�ɂ���j
        //waveHeightArray[waveNumA] += waveHeightArray[waveNumB] / 2;
        waveHeightArray[waveNumB] = 0;
        //�g���̐ݒ�
        //�Ԃ��������ɔg����ύX������
        //waveWidthArray[waveSpawnCnt] = testWaveWidth;

        //�e�O���t�B�b�N�o�b�t�@�ɔz��̒l��������
        //waveSpeedBuffer.SetData(waveSpeedArray);
        //waveStartPosBuffer.SetData(waveStartPosArray);
        //waveStartTimeBuffer.SetData(waveStartTimeArray);
        waveHeightBuffer.SetData(waveHeightArray);
        //waveWidthBuffer.SetData(waveWidthArray);

        //�z����������O���t�B�b�N�o�b�t�@��VFX�̃p�����[�^�ɑ��M����
        //effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        //effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        //effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
        //effect.SetGraphicsBuffer("waveWidthBuffer", waveWidthBuffer);



    }
    //=============================================================
    //�g���m�̏Փˎ��̏����֐�()
    //�߂�l�F�Ȃ�
    //�����F�i�傫�����̔g�i�g�`�j�̔z��̓Y���A���������̔g�i�g�a�j�̔z��̓Y���j
    //�⑫�E
    //

    //=============================================================
    public void WaveClash2(int waveNumA, int waveNumB)
    {
        //�傫���g��g�`�A�������g��g�a�Ƃ���


        //�gA�ƔgB�̑��Α��x�����߂�
        clashSpeed = Mathf.Abs(waveSpeedArray[waveNumA] - waveSpeedArray[waveNumB]);
        //���ݍ��܂��܂ł̋����݂͌��̔g���Ɠ�����
        clashDistance = Mathf.Abs(waveSpeedArray[waveNumA] * waveWidthArray[waveNumA] * 2);

        clashTime = clashDistance / clashSpeed;

        StartCoroutine(WaitWave(waveNumA, waveNumB));
    }
    IEnumerator WaitWave(int waveNumA, int waveNumB)
    {
        yield return new WaitForSeconds(clashTime + 0.1f);
        //�g�̑��x�̐ݒ�
        //�Ԃ��������ɔg�������E����������
        //waveSpeedArray[waveSpawnCnt] = waveStartSpeed;

        //�g�̊J�n�ʒu�̐ݒ�
        //�Ԃ��������ɔg���ړ�������
        //waveStartPosArray[waveSpawnCnt] = waveStartPosX;

        //�U���̐ݒ�
        //�傫���g�̐U���i�����j�ɏ��������̔g�̐U���̐��l�����Z����
        //���������̔g�������i�������O�ɂ���j
        waveHeightArray[waveNumA] += waveHeightArray[waveNumB] / 2;
        waveHeightArray[waveNumB] = 0;
        //�g���̐ݒ�
        //�Ԃ��������ɔg����ύX������
        //waveWidthArray[waveSpawnCnt] = testWaveWidth;

        //�e�O���t�B�b�N�o�b�t�@�ɔz��̒l��������
        //waveSpeedBuffer.SetData(waveSpeedArray);
        //waveStartPosBuffer.SetData(waveStartPosArray);
        //waveStartTimeBuffer.SetData(waveStartTimeArray);
        waveHeightBuffer.SetData(waveHeightArray);
        //waveWidthBuffer.SetData(waveWidthArray);

        //�z����������O���t�B�b�N�o�b�t�@��VFX�̃p�����[�^�ɑ��M����
        //effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        //effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        //effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
        //effect.SetGraphicsBuffer("waveWidthBuffer", waveWidthBuffer);

    }

    //=================================================================
    // �g�z���֐��B�������ʒu�Ŕg���I��
    // �߂�l����
    // ����1�F�����������g�̔z��ԍ�
    // ����2�F�g����������X���W
    //=================================================================
    public void waveEnd(int waveNum,float endPosX)
    {
        endPosArray[waveNum] = endPosX;
        endPosBuffer.SetData(endPosArray);
        effect.SetGraphicsBuffer("endPosBuffer",endPosBuffer);
    }

    //=================================================================
    //�g�����p�̊֐�
    //�߂�l����
    //�����F�����������g�̔z��ԍ�
    //=================================================================
    public void waveDelete(int waveNum)
    {
        waveHeightArray[waveNum] = 0;
        waveHeightBuffer.SetData(waveHeightArray);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
    }

    private void OnApplicationQuit()
    {

        waveSpeedBuffer.Release();
        waveStartPosBuffer.Release();
        waveStartTimeBuffer.Release();
        waveHeightBuffer.Release();
        waveWidthBuffer.Release();
        endPosBuffer.Release();
        EnemyFlgBuffer.Release();

    }
}
