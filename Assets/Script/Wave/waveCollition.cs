using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveCollition : MonoBehaviour
{
    [Header("����̔���"), Space(5)]
    [Tooltip("���̐��l�ȏ�ŋ��g����")]
    [SerializeField] private float strongChangePoint = 1.6f;

    [Header("�ȉ��f�o�b�O�p"), Space(5)]
    //GameObject waveControllerObj; // �g�R���g���[���I�u�W�F�N�g�i�[�p
    //waveController waveController;  //�g�R���g���[���\�X�N���v�g�i�[�p
    private float waveMaxSize = 20.0f;  // �g�̃T�C�Y

    //private int waveDirection = 1;
    //private float waveReflectPointX = 0; // �g�����˃G�l�~�[�ɓ����������̈ʒu

    private float waveEndPoint;

    private float waveElapsedTime = 0f;

    //[Header("�g�R���g���[���I�u�W�F�N�g�̖��O")]
    //[SerializeField] private string waveConObjName = "Controller";

    private GameObject hitCollision;
    private WaveAudio audio;


    public enum WAVE_VELOCITY // �g�̕�������p
    {
        RIGHT,
        LEFT,
        UP,
        DOWN,

    }
    [Header("�g�̐i�ޕ���(X�������ɐi��)")]
    private WAVE_VELOCITY waveVelocity;

    public enum WAVE_TYPE // �N�̔g��
    {
        PLAYER,
        PLAYER_ENEMY,
        PLAYER_POWERUP,
        ENEMY,
        GIMMICK,
        none,
    }
    private WAVE_TYPE waveType = WAVE_TYPE.PLAYER;
    // ���̃I�u�W�F�N�g�� Transform �i�[�p
    private Transform waveColliderTrans;
    // �g�̐k��
    private Vector3 waveStartPosition;

    // ���˒�����p�t���O
    //int waveReflectFg = 0;
    // �g���I��邩�̃t���O
    //public byte waveEndFg = 0;

    // �g�����ɉe�����y�ڂ����̔���(1�̏ꍇ�v���C���[�𐁂���΂��Ă���)
    //int waveType = 0;
    // �g���������Ă��邩�̃t���O
    //public byte waveFg = 0;

    public enum WAVE_MODE
    {
        STANDBY,
        SETUP,
        PLAY,
        END,
        REPEAT,

    }

    private WAVE_MODE waveMode = WAVE_MODE.STANDBY;



    // ���̃I�u�W�F�N�g���Ή�����vfxManager
    public vfxManager vfxManager;
    // vfxManager��̔g�̔ԍ�
    private sbyte waveNum = 0;


    private sbyte waveNumB = 0;//�Փ˂����g�̔ԍ���ێ�����
    //=================================�ҏW�J�n===============================================

    // �g�̍ő�̍����B
    private float maxWaveHight;
    // �g�̐�������
    private float waveLifeTime;
    // ���݂̔g�̍���
    public float nowHight;
    // �g�̑����𔻒f���邽�߂̎w��
    public float nowHightIndex;

    //=================================�ҏW�I��===============================================



    // ���s�[�^�[�X�N���v�g���i�[�B���ݖ��g�p
    public Test_Adder_Subtractor repeater;

    // ���s�[�^�[���N�����Ă��邩�𔻒f�B���s�[�^�[����ύX
    //public bool repeatFg = false;

    // ���̃R���W�������Ǘ����Ă���I�u�W�F�N�g�v�[��
    private WavePool pool = null;


    // 2023/2/18 ���c
    // Start is called before the first frame update
    void Start()
    {
        // Transform���i�[
        waveColliderTrans = this.transform;

        // �g�J�n�ʒu�̐ݒ�
        waveStartPosition = waveColliderTrans.position;
        // �g��������̌o�ߎ��Ԃ�ۑ�
        waveElapsedTime = 0;

        audio = GetComponent<WaveAudio>();
        if (audio == null)
        {
            Debug.LogError("WaveAudio��������܂���");
        }
    }

    // �쐬��2023/2/18     2023/3/1 �X�V��
    // ���c
    // Update is called once per frame
    void FixedUpdate()
    {



        //========================================�ҏW�J�n===============================================

        // �g�̏����ݒ�
        if (waveMode == WAVE_MODE.SETUP)
        {
            WaveSetup();
        }


        //========================================�ҏW�I��============================================
        // �X�y�[�X�������ꂽ���ɔg�𔭐�
        //if (Input.GetKeyDown(KeyCode.Space))
        //{//----- if_start -----

        //    waveFg = 2;

        //}//----- if_stop -----
        //=======================================�ҏW�J�n================================================





        // �������U��������������B
        //waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, waveColliderTrans.localScale.y - 0.1f * Time.deltaTime - waveController.waveSpeed * 0.1f * Time.deltaTime, waveColliderTrans.localScale.z);
        //waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, /*waveColliderTrans.localScale.y*/defaultScale.y - 0.2f * waveElapsedTime - (Mathf.Abs(vfxManager.waveSpeedArray[waveNum]) * waveElapsedTime /** Time.deltaTime*/) * 0.2f, waveColliderTrans.localScale.z);
        //=======================================�ҏW�I��===============================================
        // �g���������Ă��āA���̔g�����˃G�l�~�[�ɐG��Ă��Ȃ��Ƃ��A�w������֔g�𓮂����B

        switch (waveMode)
        {
            case WAVE_MODE.PLAY:
                // �g���X�V����
                WavePlay();
                break;
            case WAVE_MODE.END:
                // �g���I�������鏈�����s��
                WaveEnd(waveEndPoint);
                break;
            case WAVE_MODE.REPEAT:
                WaveEnd(waveEndPoint);
                break;
            default:
                break;
        }
        /*
        if (waveFg == 2 && waveReflectFg == 0)

        {//----- if_start -----
         // �g��������̎��Ԃ��v�Z
            waveElapsedTime += Time.deltaTime;
            // �g�̐������Ԃ��v�Z�B
            // ��{��g�̑傫�������̂܂ܕb���Ƃ��A����ɕ␔���|���邱�ƂŌv�Z�B
            waveLifeTime = 1 / maxWaveHight * 3;
            // ���݂̔g�̍����̎w���B
            // �g�̐������Ԃɔ�������̌o�ߎ��Ԃ��|���邱�ƂŌv�Z�B
            nowHightIndex = waveLifeTime * waveElapsedTime;

            // �g���������Ă��邩�������Ă��邩�𔻒f
            // �������Ԃ͔g�̍ő�T�C�Y*�����̈�b���̋t���Ōv�Z���Ă���B
            // 1�ȉ��̎�(��x�ő�ɓ��B����܂�)
            if (nowHightIndex <= 1)
            {
                // ���݂̑傫�����v�Z�B
                // ((1 - nowHightIndex) ^ 2)�Ō��݂̑傫���̎w�����擾����B
                // ���̂܂܂ł͍ő傩�珬�����Ȃ�̂ŁA1����������Ƃōŏ�����ő�ɕϊ��B
                // ��������o�����ʂɍő�T�C�Y��2�{���|����(�R���W�����͎R����J�̂���)
                nowHight = (1 - Mathf.Pow(1 - nowHightIndex, 2)) * maxWaveHight * 2;
            }
            // 1���傫���A2�ȉ��̎�(�ő�ɓ��B������A�ŏ��ɂȂ�܂�)
            else if (nowHightIndex <= 2)
            {
                // ��L�Ɠ����悤�Ɍ��݂̎w�����擾�B
                // 1 < nowHightindex <= 2�̂��߁A2���珜�Z����B
                // �ő傩��ŏ��ɂ��������߁A1���珜�Z�͂��Ȃ��B
                // ��������o�����ʂɍő�T�C�Y��2�{���|����(�R���W�����͎R����J�̂���)
                nowHight = (Mathf.Pow(2 - nowHightIndex, 2)) * maxWaveHight * 2;
            }
            // 2���傫���Ȃ����ꍇ
            else
            {
                // �g�������B
                // Destroy�������ꍇ�A��������ɑ��I�u�W�F����Q�Ƃ�������G���[��f���̂�SetActive(false)���s���Ă���B
                transform.position = new Vector3(0, 0, 50);
                transform.SetParent(null);
                waveElapsedTime = 0;
                waveFg = 0;
                return;
                //gameObject.SetActive(false);
                //Destroy(gameObject);
            }


            // �g�̕����𔻒f
            switch (waveVelocity)
            {
                // �E�Ɍ������Ă���Ƃ�
                case WAVE_VELOCITY.RIGHT:
                    // Y�̑傫����g�̍����ɕϊ�
                    waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                    // �傫�����ő�łȂ���
                    if (waveColliderTrans.localScale.x < waveMaxSize)
                    {//----- if_start -----

                        // �g�̑��x�Ɠ��������œ����蔻���傫������B
                        //========================================�ҏW�J�n===================================-
                        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                        //=====================================�ҏW�I��======================================
                        // �I�u�W�F�N�g�̍��E�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                    }//----- if_stop ----

                    // �傫�����ő�ɂȂ�����
                    else
                    {//----- else_start -----

                        // �|�W�V������g�̑����ňړ�������B
                        //========================================�ҏW�J�n===================================-
                        //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                        //=====================================�ҏW�I��======================================

                    }//----- elseif_stop -----
                    break;
                case WAVE_VELOCITY.LEFT:
                    // Y�̑傫����g�̍����ɕϊ�
                    waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                    // �傫�����ő�łȂ���(���ɑ傫������̂ő傫�����}�C�i�X�̒l�ɂȂ��Ă���)
                    if (waveColliderTrans.localScale.x > -waveMaxSize)
                    {//----- if_start -----

                        // �g�̑��x�Ɠ��������œ����蔻���傫������B
                        waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                        // �I�u�W�F�N�g�̍��E�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                    }//----- if_stop -----
                    else
                    {//----- else_start -----

                        // �|�W�V������g�̑����ňړ�������B
                        waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                    }//----- else_stop -----
                    break;
                case WAVE_VELOCITY.UP:
                    // X�̑傫����g�̍����ɕϊ�
                    waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                    // �傫�����ő�łȂ���
                    if (waveColliderTrans.localScale.y < waveMaxSize)
                    {//----- if_start -----

                        // �g�̑��x�Ɠ��������œ����蔻���傫������B
                        //========================================�ҏW�J�n===================================-
                        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================�ҏW�I��======================================
                        // �I�u�W�F�N�g�̏㉺�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V��������ɃY�����B
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x, waveStartPosition.y + waveColliderTrans.localScale.y / 2, waveStartPosition.z);

                    }//----- if_stop -----

                    // �傫�����ő�ɂȂ�����
                    else
                    {//----- else_start -----

                        // �|�W�V������g�̑����ňړ�������B
                        //========================================�ҏW�J�n===================================-
                        //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================�ҏW�I��======================================

                    }//----- elseif_stop -----
                    break;
                case WAVE_VELOCITY.DOWN:
                    // X�̑傫����g�̍����ɕϊ�
                    waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                    // �傫�����ő�łȂ���(���ɑ傫������̂Ń}�C�i�X�̒l�ɂȂ��Ă���)
                    if (waveColliderTrans.localScale.y > -waveMaxSize)
                    {//----- if_start -----

                        // �g�̑��x�Ɠ��������œ����蔻���傫������B
                        //========================================�ҏW�J�n===================================-
                        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================�ҏW�I��======================================
                        // �I�u�W�F�N�g�̏㉺�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x, waveStartPosition.y + waveColliderTrans.localScale.y / 2, waveStartPosition.z);

                    }//----- if_stop -----

                    // �傫�����ő�ɂȂ�����
                    else
                    {//----- else_start -----

                        // �|�W�V������g�̑����ňړ�������B
                        //========================================�ҏW�J�n===================================-
                        //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================�ҏW�I��======================================

                    }//----- elseif_stop -----
                    break;
            }


             // �E�ɐi��ł��鎞
            if (waveVelocity == WAVE_VELOCITY.RIGHT)
            {//----- if_start -----

                // �傫�����ő�łȂ���
                if (waveColliderTrans.localScale.x < waveMaxSize)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻���傫������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=====================================�ҏW�I��======================================
                    // �I�u�W�F�N�g�̍��E�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
                    waveColliderTrans.position =
                        new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----

                // �傫�����ő�ɂȂ�����
                else
                {//----- else_start -----

                    // �|�W�V������g�̑����ňړ�������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=====================================�ҏW�I��======================================

                }//----- elseif_stop -----
            }//----- if_stop -----

            // ���ɐi��ł���Ƃ�
            else if (waveVelocity == WAVE_VELOCITY.LEFT)
            {//----- elseif_start -----

                // �傫�����ő�łȂ���(���ɑ傫������̂ő傫�����}�C�i�X�̒l�ɂȂ��Ă���)
                if (waveColliderTrans.localScale.x > -waveMaxSize)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻���傫������B
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    // �I�u�W�F�N�g�̍��E�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
                    waveColliderTrans.position =
                        new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----
                else
                {//----- else_start -----

                    // �|�W�V������g�̑����ňړ�������B
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                }//----- else_stop -----
            }//----- elseif_stop -----


    }//----- if_stop -----

    �g���������Ă��āA���̔g�����˃G�l�~�[�ɋz������Ă����
        else if (waveFg == 2 && waveReflectFg == 1)
        {//----- elseif_start -----

            // �E�ɐi��ł��鎞
            if (waveVelocity == WAVE_VELOCITY.RIGHT)
            {//----- if_start -----

                // �g���z����������܂ł̊�
                if (waveColliderTrans.localScale.x > 0)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻�������������B
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
    // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
    waveColliderTrans.position =
                    new Vector3(waveStartPosition.x - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

}//----- if_stop -----

                // �g���z��������ꂽ��
else
{//----- else_start -----

    // �傫���̍X�V���~�߂āA�t�����ɔg��i�߂�B
    waveReflectFg = 0;
    waveVelocity = WAVE_VELOCITY.LEFT;

}//----- else_stop -----
            }//----- if_stop -----

            // ���ɐi��ł��鎞
else if (waveVelocity == WAVE_VELOCITY.LEFT)
{//----- elseif_start -----

    // �g���z����������܂ł̊�
    if (waveColliderTrans.localScale.x < 0)
    {//----- if_start -----

        // �g�̑��x�Ɠ��������œ����蔻�������������B
        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
        waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
        // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
        waveColliderTrans.position =
        new Vector3(waveStartPosition.x - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

    }//----- if_stop -----

    // �g���z��������ꂽ��
    else
    {//----- else_start -----

        // �傫���̍X�V���~�߂āA�t�����ɔg��i�߂�B
        waveReflectFg = 0;
        waveVelocity = WAVE_VELOCITY.RIGHT;

    }//----- else_stop -----
}//----- elseif_stop -----
        }//----- elseif_stop -----

         �g�̏I���������|����ꂽ�ꍇ
        if (waveFg == 0 && waveEndFg == 1)
{
    // �g���I�������鏈�����s��
    WaveEnd(waveEndPoint);
}

if (waveColliderTrans.localScale.y <= 0.2)
{
    Destroy(gameObject);
}
*/
    }

    // �쐬��2023/3/1
    // ���c
    private void OnTriggerEnter(Collider other)
    {
        if (waveMode!=WAVE_MODE.STANDBY)
        {
            // Fixed�ɂ����e���ŃR���W�����̃o�O�������������߃R���[�`���Ŏ~�߂���ɍs���B
            StartCoroutine(TriggerDelayEnter(other));
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Repeater"))
        {
            repeater = null;
        }
    }

    private IEnumerator TriggerDelayEnter(Collider other)
    {
        // 1�t���[���~�߂�
        yield return null;

        //// Reflect �^�O���������G�l�~�[�ɐG�ꂽ�Ƃ�
        //if (other.gameObject.CompareTag("Reflect"))
        //{//----- if_start -----

        //    Debug.Log("HIT");
        //    // �g���z�������悤�ɂ���B
        //    waveReflectFg = 1;

        //    // �g���v���C���[�ɍ�p����悤�ɂ���
        //    waveType = 1;

        //    if (waveVelocity == WAVE_VELOCITY.RIGHT)
        //    {//----- if_start -----

        //        // �g�̐k�����A���������ʒu�ɍX�V
        //        // �����炠�������̂ō����̈ʒu���擾���Ă���
        //        waveStartPosition.x = other.gameObject.transform.position.x - other.gameObject.transform.localScale.x / 2;

        //    }//----- if_stop -----
        //    else
        //    {//----- else_start -----

        //        // �g�̐k�����A���������ʒu�ɍX�V
        //        // �E���炠�������̂ŉE���̈ʒu���擾���Ă���
        //        waveStartPosition.x = other.gameObject.transform.position.x + other.gameObject.transform.localScale.x / 2;

        //    }//----- else_stop -----
        //}//----- if_stop -----

        //// �v���C���[�ɍ�p�o�����ԂŃv���C���[�ɐG�ꂽ��
        //else if (other.gameObject.tag == "Player" && waveType == 1)
        //{//----- if_start -----

        //    // �v���C���[�� Rigidbody ���擾
        //    var playerRigidbody = other.gameObject.GetComponent<Rigidbody>();

        //    // �v���C���[����ɔ�΂��B
        //    playerRigidbody.AddForce(0, 20, 0, ForceMode.Impulse);

        //}//----- if_stop -----

        //===================================================�ҏW�J�n====================================================
        // �g�ƏՓ˂����ꍇ
        if (other.gameObject.tag == "Wave")
        {
            // �Ԃ������g�̃I�u�W�F�N�g���擾
            hitCollision = other.gameObject;
            var collitionScript = hitCollision.GetComponent<waveCollition>();
            //Debug.Log("vfx���");
            // �Ԃ������g�X�N���v�g��vfxManager�����̃I�u�W�F�N�g�̂��̂Ɠ����ꍇ
            //      ���ʂ̎��̃R���W�����Ɗ�������Ȃ�����
            if (collitionScript.vfxManager == vfxManager)
            {

                //���������̔g�̓Y����ێ�����
                waveNumB = collitionScript.waveNum;
                //Debug.Log("vfx��ꂽ");
                // �g�̕����𔻒f
                switch (waveVelocity)
                {
                    // �E�̏ꍇ
                    case WAVE_VELOCITY.RIGHT:
                        //���ɂԂ��������̔g�̂����Y�����傫������gA�Ƃ���
                        if (waveColliderTrans.localScale.y < hitCollision.transform.localScale.y)
                        {
                            // Debug.Log("Fixed�͕�");
                            //�݂��̔g�����������Ƃ�
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //�g�̊Ǘ��X�N���v�g�ɔg���m���Փ˂������̏��������s������
                            //�����P�F�傫�����̔g�̓Y��
                            //�����Q�F���������̔g�̓Y��
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //�����g�̑傫���������Ƃ��͗�������
                        else if (waveColliderTrans.localScale.y == hitCollision.transform.localScale.y)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                            
                        }
                        break;
                    // ���̏ꍇ
                    case WAVE_VELOCITY.LEFT:
                        //���ɂԂ��������̔g�̂����Y�����傫������gA�Ƃ���
                        if (waveColliderTrans.localScale.y < hitCollision.transform.localScale.y)
                        {
                            // Debug.Log("Fixed�͕�");
                            //�݂��̔g�����������Ƃ�
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //�g�̊Ǘ��X�N���v�g�ɔg���m���Փ˂������̏��������s������
                            //�����P�F�傫�����̔g�̓Y��
                            //�����Q�F���������̔g�̓Y��
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //�����g�̑傫���������Ƃ��͗�������
                        else if (waveColliderTrans.localScale.y == hitCollision.transform.localScale.y)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                        }
                        break;
                    // ��̏ꍇ
                    case WAVE_VELOCITY.UP:
                        //���ɂԂ��������̔g�̂����Y�����傫������gA�Ƃ���
                        if (waveColliderTrans.localScale.x < hitCollision.transform.localScale.x)
                        {
                            // Debug.Log("Fixed�͕�");
                            //�݂��̔g�����������Ƃ�
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //�g�̊Ǘ��X�N���v�g�ɔg���m���Փ˂������̏��������s������
                            //�����P�F�傫�����̔g�̓Y��
                            //�����Q�F���������̔g�̓Y��
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //�����g�̑傫���������Ƃ��͗�������
                        else if (waveColliderTrans.localScale.x == hitCollision.transform.localScale.x)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                        }
                        break;
                    // ���̏ꍇ
                    case WAVE_VELOCITY.DOWN:
                        //���ɂԂ��������̔g�̂����Y�����傫������gA�Ƃ���
                        if (waveColliderTrans.localScale.x > hitCollision.transform.localScale.x)
                        {
                            // Debug.Log("Fixed�͕�");
                            //�݂��̔g�����������Ƃ�
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //�g�̊Ǘ��X�N���v�g�ɔg���m���Փ˂������̏��������s������
                            //�����P�F�傫�����̔g�̓Y��
                            //�����Q�F���������̔g�̓Y��
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //�����g�̑傫���������Ƃ��͗�������
                        else if (waveColliderTrans.localScale.x == hitCollision.transform.localScale.x)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                        }
                        break;
                }

            }

        }

        // WaveEnd�^�O�ɂԂ��������A�I���������Ȃ���Ă��Ȃ����
        if (other.gameObject.tag == "WaveEnd"/*&&waveEndFg == 0*/)
        {
            //// WaveEnd�ɐe�����݂���ꍇ
            //if (other.gameObject.transform.parent != null)
            //{
            //    // �e��vfx�������Ă��邩�𒲂ׂ�B
            //    var hitVFXSaver = other.transform.parent.GetComponent<vfxSaver>();
            //    // �����Ă��Ȃ����
            //    if (hitVFXSaver == null)
            //    {
            //        // �g���I��������
            //        waveFg = 0;
            //        waveEndFg = 1;

            //        // �g�̏I���n�_�𒲂ׂ�
            //        CheckWaveEndPoint(other);
            //        // �ۑ�����vfx��̔ԍ��̔g��waveEndPoint�ŏI��������B
            //        vfxManager.waveEnd(waveNum, waveEndPoint);
            //    }
            //    // �����Ă���ꍇ(���������̂������̎��̒[�̏ꍇ)
            //    else if (hitVFXSaver.vfxManager == vfxManager)
            //    {
            //        // �g���I��������B
            //        waveFg = 0;
            //        waveEndFg = 1;
            //        // �O�̂��߁A�I���n�_��ۑ�����B���ݖ��g�p
            //        CheckWaveEndPoint(other);




            //    }



            //}
            //// �e�����Ȃ��ꍇ(�����̏I���n�_�̏ꍇ)
            //else
            //{
            // �g���I��������B
            waveMode = WAVE_MODE.END;
            // �g�̏I���n�_�𒲂ׂ�B
            CheckWaveEndPoint(other);
            // �ۑ�����vfx��̔ԍ��̔g��waveEndPoint�ŏI��������B
            vfxManager.waveEnd(waveNum, waveEndPoint);


            //}

        }

        if (other.gameObject.CompareTag("Repeater"))
        {
            // �Ԃ������I�u�W�F�N�g�̃��s�[�^�[�X�N���v�g���擾
            var repeaterScript = other.gameObject.GetComponent<Test_Adder_Subtractor>();
            //if(repeaterScript.machineMode!=Test_Adder_Subtractor.AdderSubtractor.none)
            //{

            //}
            //if (repeaterScript.machineMode != Test_Adder_Subtractor.AdderSubtractor.none)
            if (repeaterScript != null&&repeaterScript==repeater)
            {
                
                // ���s�[�^�[�ɐݒ肳��Ă���vfx�̐��A�Q�Ƃ��s��
                for (int i = 0; i < repeaterScript.vfxCount; i++)
                {
                    // �g��vfxManager�ƃ��s�[�^�[�ɐݒ肳��Ă�����̂������A����
                    // ���s�[�^�[�ɋz�������\���̂�������̔g�ł����
                    if (vfxManager == repeaterScript.vfxManagers[i] && waveVelocity == repeaterScript.waveInputVelocity[i])
                    {
                        if (repeater.machineMode == Test_Adder_Subtractor.AdderSubtractor.none)
                        {
                            waveMode = WAVE_MODE.END;
                        }
                        else
                        {
                            // �g���I��������B
                            waveMode = WAVE_MODE.REPEAT;
                        }
                        // �g�̏I���n�_�𒲂ׂ�B
                        CheckWaveEndPoint(other);
                        // �ۑ�����vfx��̔ԍ��̔g��waveEndPoint�ŏI��������B
                        vfxManager.waveEnd(waveNum, waveEndPoint);
                    }

                }

            }

        }
        //===================================================�ҏW�I��====================================================


    }

    private void WavePlay()
    {
        // �g��������̎��Ԃ��v�Z
        waveElapsedTime += Time.deltaTime;
        // �g�̐������Ԃ��v�Z�B
        // ��{��g�̑傫�������̂܂ܕb���Ƃ��A����ɕ␔���|���邱�ƂŌv�Z�B
        waveLifeTime = 1 / maxWaveHight * 3;
        // ���݂̔g�̍����̎w���B
        // �g�̐������Ԃɔ�������̌o�ߎ��Ԃ��|���邱�ƂŌv�Z�B
        nowHightIndex = waveLifeTime * waveElapsedTime;

        // �g���������Ă��邩�������Ă��邩�𔻒f
        // �������Ԃ͔g�̍ő�T�C�Y*�����̈�b���̋t���Ōv�Z���Ă���B
        // 1�ȉ��̎�(��x�ő�ɓ��B����܂�)
        if (nowHightIndex < 1)
        {
            // ���݂̑傫�����v�Z�B
            // ((1 - nowHightIndex) ^ 2)�Ō��݂̑傫���̎w�����擾����B
            // ���̂܂܂ł͍ő傩�珬�����Ȃ�̂ŁA1����������Ƃōŏ�����ő�ɕϊ��B
            // ��������o�����ʂɍő�T�C�Y��2�{���|����(�R���W�����͎R����J�̂���)
            nowHight = (1 - Mathf.Pow(1 - nowHightIndex, 2)) * maxWaveHight * 2;
        }
        // 1���傫���A2�ȉ��̎�(�ő�ɓ��B������A�ŏ��ɂȂ�܂�)
        else if (nowHightIndex < 2)
        {
            // ��L�Ɠ����悤�Ɍ��݂̎w�����擾�B
            // 1 < nowHightindex <= 2�̂��߁A2���珜�Z����B
            // �ő傩��ŏ��ɂ��������߁A1���珜�Z�͂��Ȃ��B
            // ��������o�����ʂɍő�T�C�Y��2�{���|����(�R���W�����͎R����J�̂���)
            nowHight = (Mathf.Pow(2 - nowHightIndex, 2)) * maxWaveHight * 2;
        }
        // 2���傫���Ȃ����ꍇ
        else
        {
            // �g�������B
            // Destroy�������ꍇ�A��������ɑ��I�u�W�F����Q�Ƃ�������G���[��f���̂�SetActive(false)���s���Ă���B
            transform.position = new Vector3(0, 0, 50);
            transform.SetParent(null);
            waveElapsedTime = 0;
            waveMode = WAVE_MODE.STANDBY;
            return;
            //gameObject.SetActive(false);
            //Destroy(gameObject);
        }


        // �g�̕����𔻒f
        switch (waveVelocity)
        {
            // �E�Ɍ������Ă���Ƃ�
            case WAVE_VELOCITY.RIGHT:
                // Y�̑傫����g�̍����ɕϊ�
                waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                // �傫�����ő�łȂ���
                if (waveColliderTrans.localScale.x < waveMaxSize)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻���傫������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                    //=====================================�ҏW�I��======================================
                    // �I�u�W�F�N�g�̍��E�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0, 0);

                }//----- if_stop -----

                // �傫�����ő�ɂȂ�����
                else
                {//----- else_start -----

                    // �|�W�V������g�̑����ňړ�������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=====================================�ҏW�I��======================================

                }//----- elseif_stop -----
                break;
            case WAVE_VELOCITY.LEFT:
                // Y�̑傫����g�̍����ɕϊ�
                waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                // �傫�����ő�łȂ���(���ɑ傫������̂ő傫�����}�C�i�X�̒l�ɂȂ��Ă���)
                if (waveColliderTrans.localScale.x > -waveMaxSize)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻���傫������B
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    // �I�u�W�F�N�g�̍��E�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0, 0);

                }//----- if_stop -----
                else
                {//----- else_start -----

                    // �|�W�V������g�̑����ňړ�������B
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                }//----- else_stop -----
                break;
            case WAVE_VELOCITY.UP:
                // X�̑傫����g�̍����ɕϊ�
                waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                // �傫�����ő�łȂ���
                if (waveColliderTrans.localScale.y < waveMaxSize)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻���傫������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================�ҏW�I��======================================
                    // �I�u�W�F�N�g�̏㉺�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V��������ɃY�����B
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0);

                }//----- if_stop -----

                // �傫�����ő�ɂȂ�����
                else
                {//----- else_start -----

                    // �|�W�V������g�̑����ňړ�������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================�ҏW�I��======================================

                }//----- elseif_stop -----
                break;
            case WAVE_VELOCITY.DOWN:
                // X�̑傫����g�̍����ɕϊ�
                waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                // �傫�����ő�łȂ���(���ɑ傫������̂Ń}�C�i�X�̒l�ɂȂ��Ă���)
                if (waveColliderTrans.localScale.y > -waveMaxSize)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻���傫������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================�ҏW�I��======================================
                    // �I�u�W�F�N�g�̏㉺�ɑ傫���Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0);

                }//----- if_stop -----

                // �傫�����ő�ɂȂ�����
                else
                {//----- else_start -----

                    // �|�W�V������g�̑����ňړ�������B
                    //========================================�ҏW�J�n===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================�ҏW�I��======================================

                }//----- elseif_stop -----
                break;
        }
    }

    //================================
    // �g�̏I���n�_�𒲂ׂ鏈��
    // �߂�l����
    // �����F�Ԃ�����Collider
    //================================
    // �쐬�� 2023/04/05
    // �쐬�� ���c
    public void CheckWaveEndPoint(Collider other)
    {
        // �g�̕����𔻒f
        switch (waveVelocity)
        {
            // �E�̏ꍇ
            case WAVE_VELOCITY.RIGHT:
                // �I���|�C���g���Ԃ������I�u�W�F�N�g�̉E���ɂ���B
                waveEndPoint = other.gameObject.transform.position.x - Mathf.Abs(other.gameObject.transform.lossyScale.x / 2+0.01f);
                break;
            // ���̏ꍇ
            case WAVE_VELOCITY.LEFT:
                // �I���|�C���g���Ԃ������I�u�W�F�N�g�̍����ɂ���B
                waveEndPoint = other.gameObject.transform.position.x + Mathf.Abs(other.gameObject.transform.lossyScale.x / 2- 0.01f);
                break;
            // ��̏ꍇ
            case WAVE_VELOCITY.UP:
                // �I���|�C���g���Ԃ������I�u�W�F�N�g�̉����ɂ���B
                waveEndPoint = other.gameObject.transform.position.y - Mathf.Abs(other.gameObject.transform.lossyScale.y / 2+ 0.01f);
                break;
            // ���̏ꍇ
            case WAVE_VELOCITY.DOWN:
                // �I���|�C���g���Ԃ������I�u�W�F�N�g�̏㑤�ɂ���B
                waveEndPoint = other.gameObject.transform.position.y + Mathf.Abs(other.gameObject.transform.lossyScale.y / 2- 0.01f);
                break;
        }
    }

    private void WaveEnd(float EndPos)
    {
        switch (waveVelocity)
        {
            case WAVE_VELOCITY.RIGHT:
                // �g���z����������܂ł̊�
                if (waveColliderTrans.localScale.x > 0)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻�������������B
                    //=========================�ҏW�J�n========================================
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=============================�ҏW�I��=======================================
                    // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
                    waveColliderTrans.position =
                    new Vector3(EndPos - Mathf.Abs(waveColliderTrans.localScale.x) / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----

                // �g���z��������ꂽ��
                else
                {//----- else_start -----

                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }
                }//----- else_stop -----

                break;
            case WAVE_VELOCITY.LEFT:
                // �g���z����������܂ł̊�
                if (waveColliderTrans.localScale.x < 0)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻�������������B
                    //===================================�ҏW�J�n======================================
                    waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=============================�ҏW�I��=======================================
                    // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
                    waveColliderTrans.position =
                    new Vector3(EndPos + Mathf.Abs(waveColliderTrans.localScale.x) / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----

                // �g���z��������ꂽ��
                else
                {//----- else_start -----

                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }

                }//----- else_stop -----

                break;
            case WAVE_VELOCITY.UP:
                // �g���z����������܂ł̊�
                if (waveColliderTrans.localScale.y > 0)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻�������������B
                    //=========================�ҏW�J�n========================================
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=============================�ҏW�I��=======================================
                    // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
                    waveColliderTrans.position =
                    new Vector3(waveStartPosition.x, EndPos - Mathf.Abs(waveColliderTrans.localScale.y) / 2, waveStartPosition.z);

                }//----- if_stop -----

                // �g���z��������ꂽ��
                else
                {//----- else_start -----

                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }
                }//----- else_stop -----

                break;
            case WAVE_VELOCITY.DOWN:
                // �g���z����������܂ł̊�
                if (waveColliderTrans.localScale.y < 0)
                {//----- if_start -----

                    // �g�̑��x�Ɠ��������œ����蔻�������������B
                    //=========================�ҏW�J�n========================================
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=============================�ҏW�I��=======================================
                    // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
                    waveColliderTrans.position =
                    new Vector3(waveStartPosition.x, EndPos + Mathf.Abs(waveColliderTrans.localScale.y) / 2, waveStartPosition.z);

                }//----- if_stop -----

                // �g���z��������ꂽ��
                else
                {//----- else_start -----
                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }
                }//----- else_stop -----

                break;
        }
        //// �E�ɐi��ł��鎞
        //if (waveVelocity == WAVE_VELOCITY.RIGHT)
        //{//----- if_start -----

        //    // �g���z����������܂ł̊�
        //    if (waveColliderTrans.localScale.x > 0)
        //    {//----- if_start -----

        //        // �g�̑��x�Ɠ��������œ����蔻�������������B
        //        //=========================�ҏW�J�n========================================
        //        //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
        //        waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
        //        //=============================�ҏW�I��=======================================
        //        // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V�������E�ɃY�����B
        //        waveColliderTrans.position =
        //        new Vector3(EndPos - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

        //    }//----- if_stop -----

        //    // �g���z��������ꂽ��
        //    else
        //    {//----- else_start -----

        //        if (repeatFg == false)
        //        {
        //            Destroy(gameObject);
        //        }
        //        else
        //        {
        //            //waveFg = 1;
        //            waveEndFg = 0;
        //            waveElapsedTime = 0;
        //        }
        //    }//----- else_stop -----
        //}//----- if_stop -----

        //// ���ɐi��ł��鎞
        //else if (waveVelocity == WAVE_VELOCITY.LEFT)
        //{//----- elseif_start -----

        //    // �g���z����������܂ł̊�
        //    if (waveColliderTrans.localScale.x < 0)
        //    {//----- if_start -----

        //        // �g�̑��x�Ɠ��������œ����蔻�������������B
        //        //===================================�ҏW�J�n======================================
        //        waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
        //        //=============================�ҏW�I��=======================================
        //        // �I�u�W�F�N�g�̍��E�ɏ������Ȃ�̂ŁA���݂̑傫���̔��������|�W�V���������ɃY�����B
        //        waveColliderTrans.position =
        //        new Vector3(EndPos - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

        //    }//----- if_stop -----

        //    // �g���z��������ꂽ��
        //    else
        //    {//----- else_start -----

        //        if (repeatFg == false)
        //        {
        //            Destroy(gameObject);
        //        }
        //        else
        //        {
        //            //waveFg = 1;
        //            waveEndFg = 0;
        //            waveElapsedTime = 0;
        //        }

        //    }//----- else_stop -----
        //}//----- elseif_stop -----
    }

    public void WaveSetup()
    {

        // �g�̑����̃x�N�g���ɉ����Ĕg�̕�����ύX
        if (vfxManager.waveSpeedArray[waveNum] < 0)
        {
            waveVelocity = WAVE_VELOCITY.LEFT;
        }
        else
        {
            waveVelocity = WAVE_VELOCITY.RIGHT;
        }
        SetMaxSize();
        //waveMaxSize = vfxManager.waveSpeedArray[waveNum] * vfxManager.waveWidthArray[waveNum] * 2;
        // �g�𓮂���

        waveElapsedTime = 0;

        if (maxWaveHight >= strongChangePoint)
        {
            audio.BigSound();
            //Debug.Log("���g�T�E���h�Đ��I�Ɛl��"+gameObject.GetInstanceID());
        }
        else
        {
            audio.SmallSound();
            //Debug.Log("��g�T�E���h�Đ��I�Ɛl��" + gameObject.GetInstanceID());
        }
        waveMode = WAVE_MODE.PLAY;
    }
    //=====================
    // �g�̍ő�̍�����Ԃ��֐�
    //=====================
    // �쐬�� 2023/9/10
    // ���c
    public float GetMaxHight()
    {
        return maxWaveHight;
    }
    //=====================
    // �g�̍ő�̍�����o�^����֐�
    //=====================
    // �쐬�� 2023/9/10
    // ���c
    public void SetMaxHight(float _hight)
    {
        maxWaveHight = _hight;
    }
    //=====================
    // �N�̔g����Ԃ��֐�
    //=====================
    // �쐬�� 2023/9/10
    // ���c
    public bool CheckMode(WAVE_MODE _mode)
    {
        return waveMode == _mode;
    }
    //=====================
    // �N�̔g����o�^����֐�
    //=====================
    // �쐬�� 2023/9/10
    // ���c
    public void SetMode(WAVE_MODE _mode)
    {
        waveMode = _mode;
    }
    //=====================
    // ���̔g��vfx��̔ԍ���Ԃ��֐�
    //=====================
    // �쐬�� 2023/9/10
    // ���c
    public sbyte GetVFXNum()
    {
        return waveNum;
    }

    //=====================
    // ���̔g��vfx��̔ԍ���o�^����֐�
    //=====================
    // �쐬�� 2023/9/10 
    // ���c
    public void SetVFXNum(sbyte _num)
    {
        waveNum = _num;
    }

    //=====================
    // �g�̊J�n�n�_��o�^����֐�
    //=====================
    // �쐬�� 2023/9/10 
    // ���c
    public void SetStartPos(Vector3 _pos)
    {
        waveStartPosition = _pos;
    }

    //=====================
    // �g�̃^�C�v�������Ɠ������ۂ���Ԃ��֐�
    //=====================
    // �쐬�� 2023/9/10 
    // ���c
    public bool CheckType(WAVE_TYPE _type)
    {
        return waveType == _type;
    }

    //=====================
    // �g�̃^�C�v��o�^����֐�
    //=====================
    // �쐬�� 2023/9/10 
    // ���c
    public void SetType(WAVE_TYPE _type)
    {
        waveType = _type;
    }

    //=====================
    // �g�̐i�ޕ����������Ɠ������ۂ���Ԃ��֐�
    //=====================
    // �쐬�� 2023/9/10 
    // ���c
    public bool CheckVelocity(WAVE_VELOCITY _vel)
    {
        return waveVelocity == _vel;
    }

    //=====================
    // �g�̐i�ޕ�����Ԃ��֐�
    //=====================
    // �쐬�� 2023/9/10 
    // ���c
    public WAVE_VELOCITY GetVelocity()
    {
        return waveVelocity;
    }

    //=====================
    // �g�̐i�ޕ�����o�^����֐�
    //=====================
    // �쐬�� 2023/9/10 
    // ���c
    public void SetVelocity(WAVE_VELOCITY _vel)
    {
        waveVelocity = _vel;
    }

    //=====================
    // �g�̎��g�����v�Z
    //=====================
    // �쐬�� 2023/9/11
    // ���c
    public void SetMaxSize()
    {
        // ���x*�U������1�����̑傫�����v�Z
        waveMaxSize = Mathf.Abs(vfxManager.waveSpeedArray[waveNum] * vfxManager.waveWidthArray[waveNum]);
    }

    //=====================
    // �I�u�W�F�N�g�v�[���̃X�N���v�g�����擾
    //=====================
    // �쐬�� 2023/9/11
    // ���c
    public WavePool GetPool()
    {
        return pool;
    }

    //=====================
    // �I�u�W�F�N�g�v�[����o�^
    //=====================
    // �쐬�� 2023/9/11
    // ���c
    public void SetPool(WavePool _pool)
    {
        if(pool == null)
        {
            pool = _pool;
        }
        else if(pool != _pool)
        {
            Debug.LogError("�Ⴄ�v�[�����Z�b�g���Ă���o�O���o�邼");
        }
        
    }



    //======================
    // �g�̌o�ߎ��Ԃ����Z�b�g����֐�
    //======================
    // �쐬�� 2023/9/10
    // ���c
    public void ResetElapsedTime()
    {
        waveElapsedTime = 0;
    }
}



