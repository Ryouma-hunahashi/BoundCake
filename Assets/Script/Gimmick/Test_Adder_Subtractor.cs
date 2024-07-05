using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Adder_Subtractor : MonoBehaviour
{
    //// �g�̏��
    //private Vector3 searchDirection;    // �g�̌������擾

    //private waveCollition getWaveScript;        // �U���̃X�N���v�g���擾
    //private vfxManager getVFXManagerScript;     // VFX�}�l�[�W���[���擾

    // ���u���ǂ̏�Ԃœ�������
    public enum AdderSubtractor
    {//----- enum_start -----
        [Tooltip("�󂯎�����U�������ɓo�^�������ɋ���")]
        adder,          
        [Tooltip("����������Ă����g�݂̂�����")]
        adderLeft,          
        [Tooltip("�E��������Ă����g�݂̂�����")]
        adderRight,

        [Tooltip("�����Ă����g���������A\n[repeatCount]�Őݒ肵�����̎��ɏo��")]
        adderBoth,         
        [Tooltip("�󂯎�����U�������ɓo�^�������Ɏ㉻")]
        subtractor,     
        [Tooltip("����������Ă����g�݂̂��㉻")]
        subtractorLeft,     
        [Tooltip("�E��������Ă����g�݂̂��㉻")]
        subtractorRight,
        [Tooltip("�����Ă����g���㉻���A\n[repeatCount]�Őݒ肵�����̎��ɏo��")]
        subtractorBoth,     // �����̕����Ɏ㉻�����U��������

        [Tooltip("�c�Ɖ��̐ڑ��p")]
        VtoH_Connect,       // �c�Ɖ���ڑ�����

        [Tooltip("�g���I��������")]
        none,
    }//----- enum_stop -----

    // ���u�̏�Ԃ�ύX����
    [Header("----- ���u�̐ݒ� -----"), Space(5)]
    [Tooltip("--- �������u�̏�Ԃ�ύX ---\n" + "adder : ���Z���ĐU�����N����\n" + "subtractor : ���Z���ĐU�����N����")]
    public AdderSubtractor machineMode = AdderSubtractor.none;
    
    enum SpringSetUP
    {
        ON,
        OFF,
    }
    [Header("���̏����ݒ���s�����ǂ���")]
    [SerializeField] private SpringSetUP InitialSet = SpringSetUP.ON;
    [Header("�Q�[�����ɐV��������o�^���邩�ۂ�")]
    [SerializeField] private SpringSetUP setUP = SpringSetUP.OFF;
    private bool setUpFg = false;
    private byte startSetCheckTime = 0;
    private byte endSetCheckTime = 30;

    // ���̃I�u�W�F�N�g�̃R���C�_�[
    private Collider col;

    // ��M���ȊO����U�����󂯎�����Ƃ��ɔ��˂���
    [Header("----- ���˂̐ݒ� -----"), Space(5)]
    [Tooltip("��M���ȊO����U�����󂯎�����Ƃ��ɔ��˂���")]
    [SerializeField] private bool reflect;              // ���ʂɔ��˂���
    [Tooltip("��M���ȊO����U�����󂯎�����Ƃ��ɋ������Ă��甽�˂���")]
    [SerializeField] private bool adderReflect;         // �������Ĕ��˂���
    [Tooltip("��M���ȊO����U�����󂯎�����Ƃ��Ɏ㉻���Ă��甽�˂���")]
    [SerializeField] private bool subtracterReflect;    // �㉻���Ĕ��˂���

    [Tooltip("Both�ݒ�p�B���ɓo�^����Ă��鎅���牽�ڂ܂Ŕg�𔭐������邩")]
    [SerializeField, Range(0, 4)] private byte repeatCount;

    // �������u
    [Header("----- �����̐ݒ� -----"), Space(5)]
    [SerializeField, Range(0f, 1f)] private float addPower;        // �g�����Z����
    [SerializeField, Range(0f, 1f)] private float subtractPower;   // �g�����Z����
    [SerializeField, Range(0f, 1f)] private float VtoH_subtractPower;

    [Header("----- �����Ă���g�̐��� -----"),Space(5)]
    [SerializeField] private bool hightLimitationFg = false;
    [SerializeField] private float hightLimit;

    // ���������g�̐��l
    [SerializeField] private float adderWavePower;

    // �㉻�����g�̐��l
    [SerializeField] private float SubtractorWavePower;

    [Header("���s�[�^�[����p���鎅�̃I�u�W�F�N�g")]

    [SerializeField] private List<GameObject> groundObj = new List<GameObject>();

    [Header("�f�o�b�O�pvfx�m�F")]
    // �o�^���ꂽ�I�u�W�F����vfxManager
    public List<vfxManager> vfxManagers = new List<vfxManager>();

    

    // �I�u�W�F�N�g�̌�
    public byte vfxCount = 0;

    // �\���̃��X�g���̎c�[�B�C���X�y�N�^�[�ɕ\������Ȃ��S�~�J�X
    //public struct GROUND
    //{
    //    vfxManager vfxManager;
    //    waveCollition.WAVE_VELOCITY input;
    //    waveCollition.WAVE_VELOCITY output;
    //}
    //[Header("�\��")]
    //[SerializeField] public GROUND[] grounds = new GROUND[2];


    // �����Ă���g�̕����B����ȊO�œ���Ɣ������Ȃ��B
    public List<waveCollition.WAVE_VELOCITY> waveInputVelocity = new List<waveCollition.WAVE_VELOCITY>();
    // �o�Ă����g�̕���
    public List<waveCollition.WAVE_VELOCITY> waveOutPutVelocity = new List<waveCollition.WAVE_VELOCITY>();
    // �ݒ肵��vfx������Ă��邩�ۂ�
    public List<bool> enterFg = new List<bool>();

    // �Ԃ������g��groundObj�̂ǂꂩ������Ă������𔻒f����Y�����B
    // �����Ă����ꏊ����ˏo�������ꍇ�͂��̕ϐ����g�p����
    [SerializeField] private byte inputVFXNumber;
    // �g���ǂ�groundObj����ˏo���邩�𔻒f����Y����
    [SerializeField] private byte outPutVFXNumber;
    [Header("�擾�����R���W�����X�N���v�g")]
    [SerializeField] private waveCollition waveCollision;
    [SerializeField] private GameObject waveColliderObj;
    [SerializeField] private bool repeatSetFg = false;
    // ���X�g���ɏd������I�u�W�F�N�g�����݂��邩�𔻕ʂ���
    private bool groundSetFg = true;

    private float repeatWavePower;

    private bool repeatGoFg;
    // private int modeSetFg = 0;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.transform.childCount == 3)
    //    {
    //        if (other.gameObject.transform.GetChild(2).GetComponent<vfxManager>() != null)
    //        {
    //            for (int i = 0; i < 4; i++)
    //            {

    //                if (vfxManagers[i] == null)
    //                {
    //                    vfxManagers[i] = other.gameObject.transform.GetChild(2).GetComponent<vfxManager>();
    //                    groundPos[i] = other.gameObject.transform.position;
    //                    vfxCount++;
    //                    for (int j = 0; j < i; j++)
    //                    {
    //                        if (i == 0)
    //                        {
    //                            break;
    //                        }
    //                        if (vfxManagers[i] == vfxManagers[j])
    //                        {
    //                            vfxManagers[i] = null;
    //                            groundPos[i] = Vector3.zero;
    //                            vfxCount--;
    //                        }
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {


        if (InitialSet == SpringSetUP.ON)
        {
            if (other.gameObject.layer == 6 || other.gameObject.layer == 8)
            {
                groundSetFg = true;
                if (other.transform.childCount == 1)
                {
                    if (other.gameObject.transform.GetChild(0).GetComponent<vfxManager>() != null)
                    {
                        for (int i = 0; i < groundObj.Count; i++)
                        {
                            if (groundObj[i] == other.gameObject)
                            {
                                groundSetFg = false;
                                if (setUpFg)
                                {
                                    enterFg[i] = true;
                                }
                                break;
                            }//-----if_stop-----
                            else
                            {
                                groundSetFg = true;
                            }//-----else_stop-----

                        }//-----for_syop-----
                        if (groundSetFg == true)
                        {
                            groundObj.Add(other.gameObject);
                            vfxManagers.Add(other.gameObject.transform.GetChild(0).GetComponent<vfxManager>());
                            if (setUpFg)
                            {
                                enterFg.Add(true);
                            }
                            else
                            {
                                enterFg.Add(false);
                            }

                            vfxCount++;
                            // �c���̏ꍇ
                            if (vfxManagers[vfxCount - 1].warpWave == true)
                            {
                                // �I�u�W�F�N�g����ɂ����
                                if (groundObj[vfxCount - 1].transform.position.y > transform.position.y)
                                {
                                    // ���͂̉\��������
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                                    // �o�͂���ɐݒ�
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                                }//-----if_stop-----
                                // ���ɂ����
                                else
                                {
                                    // ���͂̉\�������
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                                    // �o�͂����ɐݒ�
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                                }//-----else_stop-----
                            }//-----if_stop-----
                            // �����̏ꍇ
                            else
                            {
                                // �I�u�W�F�N�g���E�ɂ����
                                if (groundObj[vfxCount - 1].transform.position.x > transform.position.x)
                                {
                                    // ���͂̉\��������
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                                    // �o�͂��E�ɐݒ�
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                                }//-----if_stop-----
                                // ���ł����
                                else
                                {
                                    // ���͂̉\�����E��
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                                    // �o�͂����ɐݒ�
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                                }//-----else_stop-----
                            }//-----else_stop-----
                        }//-----if_stop----
                    }//-----if_stop----
                }//-----if_stop----

            }//-----if_stop----

        }//-----if_stop----



        // [Wave]�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�Ȃ�
        if (other.gameObject.CompareTag("Wave"))
        {//----- if_start -----

            // �G�ꂽ����̌������擾
            //searchDirection = other.gameObject.transform.localScale;
            // �G�ꂽ�g�̃R���W�����X�N���v�g���擾
            waveCollision = other.GetComponent<waveCollition>();
            if(waveCollision.repeater != null)
            {
                return;
            }
            if (hightLimitationFg && waveCollision.nowHight / 2 < hightLimit)
            {
                waveCollision.CheckWaveEndPoint(col);
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                waveCollision.ResetElapsedTime();
                return;
            }
            switch (machineMode)
            {
                case AdderSubtractor.adder:

                    //Debug.Log("Adder");
                    //// �Ԃ������g���擾
                    //waveColliderObj = other.gameObject;

                    //// �Ԃ������g�����s�[�^�[�ɓo�^���Ă���n�ʂŋN�������g�����ׂ�
                    //for (byte i = 0; i < groundObj.Count; i++)
                    //{
                    //    // �g�̕��������s�[�^�[�ɐڐG����\���̂���g�Ɠ�������
                    //    // �o�^���Ă���n�ʂŋN���������̂ł������ꍇ
                    //    if (waveCollision.waveVelocity == waveInputVelocity[i] && waveCollision.vfxManager == vfxManagers[i])
                    //    {
                    //        // �����Ă����n�ʂ̓Y������o�^
                    //        inputVFXNumber = i;
                    //        // �o�Ă����n�ʂ����ɓo�^����Ă�����̂ɕύX
                    //        outPutVFXNumber = (byte)(i + 1);
                    //        // ���E�ɓ��B������0�ɂ���B
                    //        if (outPutVFXNumber == groundObj.Count)
                    //        {
                    //            outPutVFXNumber = 0;
                    //        }
                    //        // �R���W�����̃��s�[�^�[�ڐG�t���O��ture�ɂ���B
                    //        waveCollision.repeatFg = true;
                    //        // �R���W���������s�[�g����t���O��true�ɂ���B
                    //        repeatSetFg = true;
                    //        // for�����甲����
                    //        break;
                    //    }
                    //    // �ő�ɓ��B���Ă�������vfx�������łȂ���Ώ����𒆒f
                    //    else if (i == groundObj.Count - 1)
                    //    {
                    //        if (waveCollision.repeatFg == false)
                    //        {
                    //            //waveCollition.waveEndFg = 0;
                    //            //waveCollition.waveFg = 2;
                    //        }
                    //        return;
                    //    }
                    //}
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {

                        Adder_InputCheck();
                    }

                    break;
                case AdderSubtractor.adderLeft:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        AdderLeft_InputCheck();
                    }
                    break;
                case AdderSubtractor.adderRight:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        AdderRight_InputCheck();
                    }

                    break;
                case AdderSubtractor.adderBoth:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        AdderBoth_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractor:

                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        Subtractor_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractorLeft:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        SubtractorLeft_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractorRight:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        SubtractorRight_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractorBoth:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        SubtractorBoth_InputCheck();
                    }
                    break;
                // �c�Ɖ����q�������̏ꍇ
                case AdderSubtractor.VtoH_Connect:

                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        VtoH_InputCheck();
                    }

                    break;
                case AdderSubtractor.none:
                    RepeatGroundSet(other);
                    break;
                default:

                    break;
            }
            
            waveCollision = null;
            waveColliderObj = null;

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        if(setUpFg&&InitialSet == SpringSetUP.ON)
        {
            if(other.gameObject.layer == 6 || other.gameObject.layer == 8)
            {
                for (int i = 0; i < groundObj.Count; i++)
                {
                    if (groundObj[i] == other.gameObject)
                    {
                        enterFg[i] = false;

                        break;
                    }//-----if_stop-----
                    

                }//-----for_syop-----
            }
        }
    }

    //=================================================
    // ���������g�R���W�����𔻒f���Ainput,output�𔻒f
    // �߂�l����
    // �����F
    //=================================================
    private void RepeatGroundSet(Collider other)
    {
        // �Ԃ������g���擾
        waveColliderObj = other.gameObject;

        // �Ԃ������g�����s�[�^�[�ɓo�^���Ă���n�ʂŋN�������g�����ׂ�
        for (byte i = 0; i < groundObj.Count; i++)
        {
            // �g�̕��������s�[�^�[�ɐڐG����\���̂���g�Ɠ�������
            // �o�^���Ă���n�ʂŋN���������̂���
            // �ڐG���Ă��鎅�ł���ꍇ
            if (waveCollision.CheckVelocity(waveInputVelocity[i]) && waveCollision.vfxManager == vfxManagers[i])
            {
                waveCollision.repeater = this;
                // �����Ă����n�ʂ̓Y������o�^
                inputVFXNumber = i;
                // �o�Ă����n�ʂ����ɓo�^����Ă�����̂ɕύX
                outPutVFXNumber = (byte)(i + 1);
                // ���E�ɓ��B������0�ɂ���B
                if (outPutVFXNumber == groundObj.Count)
                {
                    outPutVFXNumber = 0;
                }
                while (!enterFg[outPutVFXNumber] || Mathf.Abs(groundObj[outPutVFXNumber].transform.localScale.x) < 0.35f)
                {
                    outPutVFXNumber++;
                    // ���E�ɓ��B������0�ɂ���B
                    if (outPutVFXNumber >= groundObj.Count)
                    {
                        outPutVFXNumber = 0;
                    }
                    if (outPutVFXNumber == inputVFXNumber)
                    {
                        break;
                    }
                }
                
                // �R���W�����̃��s�[�^�[�ڐG�t���O��ture�ɂ���B
                //waveCollision.SetMode(waveCollition.WAVE_MODE.REPEAT);
                // �R���W���������s�[�g����t���O��true�ɂ���B
                //repeatSetFg = true;
                // for�����甲����
                repeatGoFg = true;
                break;
            }
            // �ő�ɓ��B���Ă�������vfx�������łȂ���Ώ����𒆒f
            else if (i == groundObj.Count - 1)
            {
                //if (waveCollision.repeatFg == false)
                //{
                //    waveCollition.waveEndFg = 0;
                //    waveCollition.waveFg = 2;
                //}
                
                //waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                waveCollision.repeater = null;
                repeatGoFg = false;
                waveCollision = null;
                waveColliderObj = null;
                return;
            }
        }
    }



    //=================================================
    // �����Ă����g�̌����ɉ����āA�A�E�g�v�b�g�̏�����ύX����
    // �߂�l����
    // ��������
    //=================================================
    // �쐬���@2023/04/06   �X�V��2023/04/14
    // �쐬�ҁ@���c�@�{��
    private void Adder_InputCheck()
    {
        // �A�E�g�v�b�g�𓝈�̂��̂ɕύX�������ߍ폜�B
        // ���^�̕����ǂ���΂�����𕜌�����B
        // �����Ă����g�̕����𔻒f
        //switch (waveInputVelocity[inputVFXNumber])
        //{

        //    // ���̏ꍇ
        //    case waveCollition.WAVE_VELOCITY.LEFT:

        //        // �g��o�^�������̒n�ʂɏo��������B
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }
        //        break;

        //    // �E�̏ꍇ
        //    case waveCollition.WAVE_VELOCITY.RIGHT:

        //        // �g��o�^�������̒n�ʂɏo��������B
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }
        //        break;

        //    // ��̏ꍇ
        //    case waveCollition.WAVE_VELOCITY.UP:

        //        // �g��o�^�������̒n�ʂɏo��������B
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }
        //        break;

        //    // ���̏ꍇ
        //    case waveCollition.WAVE_VELOCITY.DOWN:

        //        // �g��o�^�������̒n�ʂɏo��������B
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }

        //        break;
        //}
        // �����Ă����g�̕����𔻒f
        switch (waveInputVelocity[inputVFXNumber])
        {
            // ���̏ꍇ
            case waveCollition.WAVE_VELOCITY.LEFT:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // �E�̏ꍇ
            case waveCollition.WAVE_VELOCITY.RIGHT:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    { 
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                           WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // ��̏ꍇ
            case waveCollition.WAVE_VELOCITY.UP:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;
            // ���̏ꍇ
            case waveCollition.WAVE_VELOCITY.DOWN:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }

                break;

        }

    }

    //=================================================
    // �g�̏o�Ă��������ɉ����āA�����ꏊ�A������ύX
    // �߂�l����
    // �����F�g�̍ő�T�C�Y
    //=================================================
    // �쐬���@2023/04/06   �X�V��2023/04/14
    // �쐬�ҁ@���c�@�{��
    private void Adder_Output(float waveMaxSize)
    {
        // ���ˏ�ԂłȂ����
        if (!reflect)
        {
            // �o�Ă����g�̕����𔻒f
            switch (waveOutPutVelocity[outPutVFXNumber])
            {
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.LEFT:
                    {
                        Debug.Log("���ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̍����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
                // �E�̏ꍇ
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    {
                        Debug.Log("�E�ɐ���");
                        // �����ʒu�����̃I�u�W�F�N�g�̉E���ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
                // ��̏ꍇ
                case waveCollition.WAVE_VELOCITY.UP:
                    {
                        Debug.Log("��ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̏㑤�ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.DOWN:
                    {
                        Debug.Log("���ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̉����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
            }//----- switch_stop -----

        }//----- if_stop -----
        else
        {
            Debug.Log("Adder_Reflect");
            // �o�Ă����g�̕����𔽓]
            switch (waveOutPutVelocity[inputVFXNumber])
            {
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.LEFT:
                    {
                        Debug.Log("���ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̍����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, inputVFXNumber);
                        break;
                    }
                // �E�̏ꍇ
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    {
                        Debug.Log("�E�ɐ���");
                        // �����ʒu�����̃I�u�W�F�N�g�̉E���ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, inputVFXNumber);
                        break;
                    }
                // ��̏ꍇ
                case waveCollition.WAVE_VELOCITY.UP:
                    {
                        Debug.Log("��ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̏㑤�ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, inputVFXNumber);
                        break;
                    }
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.DOWN:
                    {
                        Debug.Log("���ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̉����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, inputVFXNumber);
                        break;
                    }
            }//----- switch_stop -----
        }//----- else_stop -----
    }

    //=================================================
    // ������E�ɂ̂ݔg�𑝕������A����ȊO�͔��ˁA�����͂�����waveEnd�Ƃ��ċ@�\������
    // �߂�l����
    // ��������
    //=================================================
    private void AdderLeft_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                if(adderReflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight()+addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                if (inputVFXNumber != outPutVFXNumber)
                {
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                // �g��o�^�������̒n�ʂɏo��������B
                
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // �E���獶�ɂ̂ݔg�𑝕������A����ȊO�͔��ˁA�����͂�����waveEnd�Ƃ��ċ@�\������
    // �߂�l����
    // ��������
    // ================================================
    private void AdderRight_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                if (inputVFXNumber != outPutVFXNumber)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                if (adderReflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // �����Ă����g���AOutputVFXNumber����ݒ���̎��ɑ������ă��s�[�g����
    // �߂�l����
    // �����Ȃ�
    //=================================================
    private void AdderBoth_InputCheck()
    {
        // �g��o�^�������̒n�ʂɏo��������B
        if (waveCollision.nowHightIndex < 1)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i)%vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.GetMaxHight() + addPower);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.GetMaxHight() + addPower);
                    
                }
            }


        }
        else if (waveCollision.nowHightIndex < 2)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i) % vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.nowHight / 2);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.nowHight / 2);
                    
                }
            }

        }

        // �R���W���������s�[�g����t���O��؂�
        repeatSetFg = false;

        // Update���Ŕg�����s�[�g������ꍇ�̏����B
        // �Ԃ������g�R���W�����̏������ׂĔj������B
        waveColliderObj = null;
        waveCollision = null;
    }

    //===================================
    // �����Ă����g�̌����ɉ����āA�A�E�g�v�b�g�̏�����ύX����
    // �߂�l����
    // ��������
    //===================================
    // �쐬���@2023/04/06   �X�V��2023/04/14
    // �쐬�ҁ@���c�@�{��
    private void Subtractor_InputCheck()
    {
        // �����Ă����g�̕����𔻒f
        switch (waveInputVelocity[inputVFXNumber])
        {
            // ���̏ꍇ
            case waveCollition.WAVE_VELOCITY.LEFT:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // �E�̏ꍇ
            case waveCollition.WAVE_VELOCITY.RIGHT:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // ��̏ꍇ
            case waveCollition.WAVE_VELOCITY.UP:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;
            // ���̏ꍇ
            case waveCollition.WAVE_VELOCITY.DOWN:

                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);

                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }

                break;

        }

    }

    //=================================================
    // ������E�ɂ̂ݔg�����������A����ȊO�͔��ˁA�����͂�����waveEnd�Ƃ��ċ@�\������
    // �߂�l����
    // ��������
    //=================================================
    private void SubtractorLeft_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                if (subtracterReflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                }
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // �E���獶�ɂ̂ݔg�𑝕������A����ȊO�͔��ˁA�����͂�����waveEnd�Ƃ��ċ@�\������
    // �߂�l����
    // ��������
    // ================================================
    private void SubtractorRight_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                // �g��o�^�������̒n�ʂɏo��������B
                if (waveCollision.nowHightIndex < 1)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                }
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                if (subtracterReflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // �g��o�^�������̒n�ʂɏo��������B
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // �g�����ł�����B
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // �����Ă����g���AOutputVFXNumber����ݒ���̎��ɑ������ă��s�[�g����
    // �߂�l����
    // �����Ȃ�
    //=================================================
    private void SubtractorBoth_InputCheck()
    {
        // �g��o�^�������̒n�ʂɏo��������B
        if (waveCollision.nowHightIndex < 1)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i)%vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.GetMaxHight() - subtractPower);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.GetMaxHight() + addPower);
                    
                }
            }


        }
        else if (waveCollision.nowHightIndex < 2)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i)%vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.nowHight / 2);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.nowHight / 2);
                }
            }

        }

        // �R���W���������s�[�g����t���O��؂�
        repeatSetFg = false;

        // Update���Ŕg�����s�[�g������ꍇ�̏����B
        // �Ԃ������g�R���W�����̏������ׂĔj������B
        waveColliderObj = null;
        waveCollision = null;
    }

    //=================================================
    // �g�̏o�Ă��������ɉ����āA�����ꏊ�A������ύX
    // �߂�l����
    // �����F�g�̍ő�T�C�Y
    // 1     �o�������鎅�̔ԍ�
    //=================================================
    // �쐬���@2023/04/06   �X�V��2023/04/14
    // �쐬�ҁ@���c�@�{��
    private void WaveOutput(byte vfxNumber, float waveMaxSize)
    {
        
        if (waveMaxSize > 0&&enterFg[vfxNumber])
        {
            // �o�Ă����g�̕����𔻒f
            switch (waveOutPutVelocity[vfxNumber])
            {
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.LEFT:
                    {
                        Debug.Log("���ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̍����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                        break;
                    }
                // �E�̏ꍇ
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    {
                        Debug.Log("�E�ɐ���");
                        // �����ʒu�����̃I�u�W�F�N�g�̉E���ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                        break;
                    }
                // ��̏ꍇ
                case waveCollition.WAVE_VELOCITY.UP:
                    {
                        Debug.Log("��ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̏㑤�ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                        break;
                    }
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.DOWN:
                    {
                        Debug.Log("���ɐ����I");
                        // �����ʒu�����̃I�u�W�F�N�g�̉����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                        V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                        break;
                    }
            }//----- switch_stop -----
        }
        else
        {
            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
        }


    }

    private void AdderBoth_OutputCheck(GameObject Collision, waveCollition CollisionScript, byte vfxNumber, float waveMaxSize)
    {
        // �o�Ă����g�̕����𔻒f
        switch (waveOutPutVelocity[vfxNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                H_DivideWaveSpawn(Collision, CollisionScript, transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                H_DivideWaveSpawn(Collision, CollisionScript, transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                V_DivideWaveSpawn(Collision, CollisionScript, transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                V_DivideWaveSpawn(Collision, CollisionScript, transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                break;
        }
    }

    //===================================
    // �����Ă����g�̌����ɉ����āA�A�E�g�v�b�g�̏�����ύX����
    // �߂�l����
    // ��������
    //===================================
    // �쐬���@2023/04/06
    // �쐬�ҁ@���c
    private void VtoH_InputCheck()
    {
        // �����Ă����g�̕����𔻒f
        switch (waveInputVelocity[inputVFXNumber])
        {
            // ���̏ꍇ
            case waveCollition.WAVE_VELOCITY.LEFT:
                // �g�̕���0�ȏ�ɂȂ�����(�}�C�i�X�����ɔg���傫���Ȃ��Ă�������)
                //if (waveColliderObj.transform.localScale.x >= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("�������̎��̔g�̐������Ⴀ�I");
                        // �g��o�^�������̒n�ʂɏo��������B
                        if (waveCollision.nowHightIndex < 1)
                        {

                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);

                        }
                        else if (waveCollision.nowHightIndex < 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
            // �E�̏ꍇ
            case waveCollition.WAVE_VELOCITY.RIGHT:
                //if (waveColliderObj.transform.localScale.x <= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("�E�����̎��̔g�̐������Ⴀ�I");
                        // �g��o�^�������̒n�ʂɏo��������B
                        if (waveCollision.nowHightIndex < 1)
                        {
                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                        else if (waveCollision.nowHightIndex < 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
            // ��̏ꍇ
            case waveCollition.WAVE_VELOCITY.UP:
                //if (waveColliderObj.transform.localScale.y <= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("������̎��̔g�̐������Ⴀ�I");
                        // �g��o�^�������̒n�ʂɏo��������B
                        if (waveCollision.nowHightIndex < 1)
                        {
                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                        else if (waveCollision.nowHightIndex <= 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
            // ���̏ꍇ
            case waveCollition.WAVE_VELOCITY.DOWN:
                //if (waveColliderObj.transform.localScale.y >= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("�������̎��̔g�̐������Ⴀ�I");
                        // �g��o�^�������̒n�ʂɏo��������B
                        if (waveCollision.nowHightIndex < 1)
                        {
                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                        else if (waveCollision.nowHightIndex < 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
        }

    }

    //=================================================
    // �g�̏o�Ă��������ɉ����āA�����ꏊ�A������ύX
    // �߂�l����
    // �����F�g�̍ő�T�C�Y
    //=================================================
    // �쐬���@2023/04/06
    // �쐬�ҁ@���c
    private void VtoH_Output(float waveMaxSize)
    {
        if (waveMaxSize > 0&&enterFg[outPutVFXNumber])
        {
            // �o�Ă����g�̕����𔻒f
            switch (waveOutPutVelocity[outPutVFXNumber])
            {
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.LEFT:
                    Debug.Log("���ɐ����I");
                    // �����ʒu�����̃I�u�W�F�N�g�̍����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                    H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2-0.01f, -1, waveMaxSize, outPutVFXNumber);

                    break;
                // �E�̏ꍇ
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    Debug.Log("�E�ɐ���");
                    // �����ʒu�����̃I�u�W�F�N�g�̉E���ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                    H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2+ 0.01f, 1, waveMaxSize, outPutVFXNumber);

                    break;
                // ��̏ꍇ
                case waveCollition.WAVE_VELOCITY.UP:
                    Debug.Log("��ɐ����I");
                    // �����ʒu�����̃I�u�W�F�N�g�̏㑤�ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                    V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2+ 0.01f, 1, waveMaxSize, outPutVFXNumber);

                    break;
                // ���̏ꍇ
                case waveCollition.WAVE_VELOCITY.DOWN:
                    Debug.Log("���ɐ����I");
                    // �����ʒu�����̃I�u�W�F�N�g�̉����ɐݒ�B�g�̕����A�g�̍ő�T�C�Y���w�肵�Đ���
                    V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2- 0.01f, -1, waveMaxSize, outPutVFXNumber);

                    break;
            }
        }
        else
        {
            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
        }
    }

    //==============================================
    // ���E�ɔg�𔭐�������֐�
    // �߂�l����
    // �����F�g�̔����ʒu�A�g�̌����A�g�̍ő�T�C�Y�A�o���������̔z��ԍ�
    //==============================================
    // �쐬���@2023/04/07
    // �쐬�ҁ@���c
    private void H_RepeatWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // �g�̌������o�Ă��������ɐݒ�
        waveCollision.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // �g�̃R���W�����X�N���v�g�ɏo�Ă����g��vfxManager����
        waveCollision.vfxManager = vfxManagers[vfxNumber];
        // vfx�̔g�𐶐����A�g�̔ԍ����ꎞ�I�ɕۑ�
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]),
                                                waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()],
                                                vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);
        
        
        // �n�ʂ��ړ����̏ꍇ�A���̒n�ʂ̐e�̎q�ɔg��ݒ肷��B
        if ((groundObj[vfxNumber].CompareTag("Floor") ||
            (groundObj[vfxNumber].transform.parent != null &&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor"))) &&
            !vfxManagers[vfxNumber].warpWave)
        {
            waveColliderObj.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        { 
            waveColliderObj.transform.SetParent(null); 
        }

        // �R���W������vfx��̔ԍ��𐶐������g�̂��̂ɐݒ�
        waveCollision.SetVFXNum(num);
        // �R���W�����̃X�P�[����������
        waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
        // �R���W�����̏����ʒuX��repeatPos�ɕύX
        var trans = waveColliderObj.transform.position;
        trans.x = repeatPos;
        // Y�����̃I�u�W�F�̈ʒu�ɕύX
        trans.y = groundObj[vfxNumber].transform.position.y;
        // �ύX�������ʂ���
        waveColliderObj.transform.position = trans;
        // �g�̊J�n�n�_��ݒ�
        waveCollision.SetStartPos(trans);
        // �g��1�����̑傫����ݒ�
        waveCollision.SetMaxSize();
        // �g�̍ő�̍�����ݒ�
        waveCollision.SetMaxHight(waveMaxSize);
        // ��������̌o�ߎ��Ԃ�������
        waveCollision.ResetElapsedTime();
        // �g�𓮂���
        waveCollision.SetMode(waveCollition.WAVE_MODE.PLAY);
        // �R���W���������s�[�g����t���O��؂�
        repeatSetFg = false;

        

    }
    //==============================================
    // �㉺�ɔg�𔭐�������֐�
    // �߂�l����
    // �����F�g�̔����ʒu�A�g�̌����A�g�̍ő�T�C�Y�A�o���������̔z��ԍ�
    //==============================================
    // �쐬���@2023/04/07
    // �쐬�ҁ@���c
    private void V_RepeatWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // �g�̌������o�Ă��������ɐݒ�
        waveCollision.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // �g�̃R���W�����X�N���v�g�ɏo�Ă����g��vfxManager����
        waveCollision.vfxManager = vfxManagers[vfxNumber];
        // vfx�̔g�𐶐����A�g�̔ԍ����ꎞ�I�ɕۑ�
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]), 
                                                   waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()], 
                                                   vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);

        
        // �n�ʂ��ړ����̏ꍇ�A���̒n�ʂ̐e�̎q�ɔg��ݒ肷��B
        if ((groundObj[vfxNumber].CompareTag("Floor") ||
            (groundObj[vfxNumber].transform.parent != null &&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor")))&&
            !vfxManagers[vfxNumber].warpWave)
        {
            waveColliderObj.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        {
            waveColliderObj.transform.SetParent(null);
        }

        // �R���W������vfx��̔ԍ��𐶐������g�̂��̂ɐݒ�
        waveCollision.SetVFXNum(num);
        // �R���W�����̃X�P�[����������
        waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
        // �R���W�����̏����ʒuY��repeatPos�ɕύX
        var trans = waveColliderObj.transform.position;
        trans.y = repeatPos;
        // X��U�����o���I�u�W�F�̈ʒu�ɕύX
        trans.x = groundObj[vfxNumber].transform.position.x;
        // �ύX�������ʂ���
        waveColliderObj.transform.position = trans;
        // �g�̊J�n�n�_��ݒ�
        waveCollision.SetStartPos(trans);
        // �g��1�����̑傫����ݒ�
        waveCollision.SetMaxSize();
        // �g�̍ő�̍�����ݒ�
        waveCollision.SetMaxHight(waveMaxSize);
        // ��������̌o�ߎ��Ԃ�������
        waveCollision.ResetElapsedTime();
        // �g�𓮂���
        waveCollision.SetMode(waveCollition.WAVE_MODE.PLAY);

        
        repeatSetFg = false;
        

    }

    private void H_DivideWaveSpawn(GameObject Collision, waveCollition CollitionScript, float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // �g�̌������o�Ă��������ɐݒ�
        CollitionScript.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // �g�̃R���W�����X�N���v�g�ɏo�Ă����g��vfxManager����
        CollitionScript.vfxManager = vfxManagers[vfxNumber];
        // vfx�̔g�𐶐����A�g�̔ԍ����ꎞ�I�ɕۑ�
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]),
                                                   waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()], 
                                                   vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);


        // �n�ʂ��ړ����̏ꍇ�A���̒n�ʂ̐e�̎q�ɔg��ݒ肷��B
        if ((groundObj[vfxNumber].CompareTag("Floor") ||
            (groundObj[vfxNumber].transform.parent != null &&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor"))) &&
            !vfxManagers[vfxNumber].warpWave)
        {
            Collision.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        { 
            Collision.transform.SetParent(null); 
        }

        // �R���W������vfx��̔ԍ��𐶐������g�̂��̂ɐݒ�
        CollitionScript.SetVFXNum(num);
        // �R���W�����̃X�P�[����������
        Collision.transform.localScale = new Vector3(0, 0, 1);
        // �R���W�����̏����ʒuX��repeatPos�ɕύX
        var trans = Collision.transform.position;
        trans.x = repeatPos;
        // Y�����̃I�u�W�F�̈ʒu�ɕύX
        trans.y = groundObj[vfxNumber].transform.position.y;
        // �ύX�������ʂ���
        Collision.transform.position = trans;
        // �g�̊J�n�n�_��ݒ�
        CollitionScript.SetStartPos(trans);
        // �g��1�����̑傫����ݒ�
        CollitionScript.SetMaxSize();
        // �g�̍ő�̍�����ݒ�
        CollitionScript.SetMaxHight(waveMaxSize);
        // ��������̌o�ߎ��Ԃ�������
        CollitionScript.ResetElapsedTime();
        // �g�𓮂���
        CollitionScript.SetMode(waveCollition.WAVE_MODE.PLAY);

    }

    private void V_DivideWaveSpawn(GameObject Collision, waveCollition CollitionScript, float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // �g�̌������o�Ă��������ɐݒ�
        CollitionScript.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // �g�̃R���W�����X�N���v�g�ɏo�Ă����g��vfxManager����
        CollitionScript.vfxManager = vfxManagers[vfxNumber];
        // vfx�̔g�𐶐����A�g�̔ԍ����ꎞ�I�ɕۑ�
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]), 
                                                   waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()],
                                                   vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);

        
        // �n�ʂ��ړ����̏ꍇ�A���̒n�ʂ̐e�̎q�ɔg��ݒ肷��B
        if ((groundObj[vfxNumber].CompareTag("Floor")||
            (groundObj[vfxNumber].transform.parent!=null&&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor")))&&
            !vfxManagers[vfxNumber].warpWave)
        {
            Collision.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        {
            Collision.transform.SetParent(null);
        }

        // �R���W������vfx��̔ԍ��𐶐������g�̂��̂ɐݒ�
        CollitionScript.SetVFXNum(num);
        // �R���W�����̃X�P�[����������
        Collision.transform.localScale = new Vector3(0, 0, 1);
        // �R���W�����̏����ʒuX��repeatPos�ɕύX
        var trans = Collision.transform.position;
        trans.y = repeatPos;
        // Y�����̃I�u�W�F�̈ʒu�ɕύX
        trans.x = groundObj[vfxNumber].transform.position.x;
        // �ύX�������ʂ���
        Collision.transform.position = trans;
        // �g�̊J�n�n�_��ݒ�
        CollitionScript.SetStartPos(trans);
        // �g��1�����̑傫����ݒ�
        CollitionScript.SetMaxSize();
        // �g�̍ő�̍�����ݒ�
        CollitionScript.SetMaxHight(waveMaxSize);
        // ��������̌o�ߎ��Ԃ�������
        CollitionScript.ResetElapsedTime();
        // �g�𓮂���
        CollitionScript.SetMode(waveCollition.WAVE_MODE.PLAY);

    }

    // ���˂̎c�[�B�o������vfx�̔ԍ������ύX����Γ������߁A�Q�l�p�ɕۑ��B
    //private void H_ReflectWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize)
    //{

    //    // �g�̌������o�Ă��������ɐݒ�
    //    waveCollision.waveVelocity = waveOutPutVelocity[inputVFXNumber];

    //    // vfx�̔g�𐶐����A�g�̔ԍ����ꎞ�I�ɕۑ�
    //    var num = vfxManagers[inputVFXNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.waveNum]), waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.waveNum]);
    //    // �g�̃R���W�����X�N���v�g�ɏo�Ă����g��vfxManager����
    //    waveCollision.vfxManager = vfxManagers[inputVFXNumber];
    //    // �R���W������vfx��̔ԍ��𐶐������g�̂��̂ɐݒ�
    //    waveCollision.waveNum = num;
    //    // �R���W�����̃X�P�[����������
    //    waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
    //    // �R���W�����̏����ʒuX��repeatPos�ɕύX
    //    var trans = waveCollision.transform.position;
    //    trans.x = repeatPos;
    //    // Y�����̃I�u�W�F�̈ʒu�ɕύX
    //    trans.y = transform.position.y;
    //    // �ύX�������ʂ���
    //    waveColliderObj.transform.position = trans;
    //    // �g�̊J�n�n�_��ݒ�
    //    waveCollision.waveStartPosition = trans;
    //    // �g��1�����̑傫����ݒ�
    //    waveCollision.waveMaxSize = Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[num] * vfxManagers[inputVFXNumber].waveWidthArray[num]);
    //    // �g�̍ő�̍�����ݒ�
    //    waveCollision.maxWaveHight = waveMaxSize;
    //    // ��������̌o�ߎ��Ԃ�������
    //    waveCollision.waveElapsedTime = 0;
    //    // �g�̏I���t���O��؂�
    //    waveCollision.waveEndFg = 0;
    //    // �g���ړ�������
    //    waveCollision.waveFg = 2;
    //    // ���s�[�g�t���O����
    //    waveCollision.repeatFg = false;
    //    // �R���W���������s�[�g����t���O��؂�
    //    repeatSetFg = false;

    //}
    //private void V_ReflectWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize)
    //{

    //    // �g�̌������o�Ă��������ɐݒ�
    //    waveCollision.waveVelocity = waveOutPutVelocity[inputVFXNumber];

    //    // vfx�̔g�𐶐����A�g�̔ԍ����ꎞ�I�ɕۑ�
    //    var num = vfxManagers[inputVFXNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.waveNum]), waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.waveNum]);
    //    // �g�̃R���W�����X�N���v�g�ɏo�Ă����g��vfxManager����
    //    waveCollision.vfxManager = vfxManagers[inputVFXNumber];
    //    // �R���W������vfx��̔ԍ��𐶐������g�̂��̂ɐݒ�
    //    waveCollision.waveNum = num;
    //    // �R���W�����̃X�P�[����������
    //    waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
    //    // �R���W�����̏����ʒuY��repeatPos�ɕύX
    //    var trans = waveCollision.transform.position;
    //    trans.y = repeatPos;
    //    // X�����̃I�u�W�F�̈ʒu�ɕύX
    //    trans.x = transform.position.x;
    //    // �ύX�������ʂ���
    //    waveColliderObj.transform.position = trans;
    //    // �g�̊J�n�n�_��ݒ�
    //    waveCollision.waveStartPosition = trans;
    //    // �g��1�����̑傫����ݒ�
    //    waveCollision.waveMaxSize = Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[num] * vfxManagers[inputVFXNumber].waveWidthArray[num]);
    //    // �g�̍ő�̍�����ݒ�
    //    waveCollision.maxWaveHight = waveMaxSize;
    //    // ��������̌o�ߎ��Ԃ�������
    //    waveCollision.waveElapsedTime = 0;
    //    // �g�̏I���t���O��؂�
    //    waveCollision.waveEndFg = 0;
    //    // �g���ړ�������
    //    waveCollision.waveFg = 2;
    //    // ���s�[�g�t���O����
    //    waveCollision.repeatFg = false;
    //    repeatSetFg = false;
    //    waveColliderObj = null;
    //    waveCollision = null;
    //}

    void RepeatorSetUp()
    {
        //if (groundObj == null)
        //{
        //    Debug.LogError("��p�����鎅��ݒ肵�Ă�������");
        //}
        // �ݒ肵����p���鎅�̃I�u�W�F�N�g�̐��AvfxManager���擾����
        for (int i = 0; i < groundObj.Count; i++)
        {
            // �O���E���h�I�u�W�F�����̔z��ԍ��ɐݒ肳��Ă���ꍇ
            if (groundObj[i] != null)
            {
                // ���̎q�I�u�W�F�N�g�ɑ��݂���vfxManager���擾
                vfxManagers.Add(groundObj[i].transform.GetChild(0).GetComponent<vfxManager>());
                // ���s�[�^�[�Ǝ����G��Ă����Ԃɂ���B
                enterFg.Add(true);
                // ���݂���vfx�̐���ۑ�
                vfxCount++;
            }
        }



        // �ݒ肵�����ɂ���āA�g�������Ă�������A�o�Ă����������m�肷��B
        for (int i = 0; i < groundObj.Count; i++)
        {
            // �c���̏ꍇ
            if (vfxManagers[i].warpWave == true)
            {
                // �I�u�W�F�N�g����ɂ����
                if (groundObj[i].transform.position.y > transform.position.y)
                {
                    // ���͂̉\��������
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                    // �o�͂���ɐݒ�
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                }
                // ���ɂ����
                else
                {
                    // ���͂̉\�������
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                    // �o�͂����ɐݒ�
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                }
            }
            // �����̏ꍇ
            else
            {
                // �I�u�W�F�N�g���E�ɂ����
                if (groundObj[i].transform.position.x > transform.position.x)
                {
                    // ���͂̉\��������
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                    // �o�͂��E�ɐݒ�
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                }
                // ���ł����
                else
                {
                    // ���͂̉\�����E��
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                    // �o�͂����ɐݒ�
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                }

            }
        }
    }

    private void Start()
    {
        col = GetComponent<Collider>();

        vfxCount = 0;
        for(byte i = 0;i<groundObj.Count;i++)
        {
            if(groundObj[i] == null)
            {
                groundObj.RemoveAt(i);
                i--;
            }
        }
        vfxManagers.Clear();
        waveInputVelocity.Clear();
        waveOutPutVelocity.Clear();
        enterFg.Clear();

        switch (machineMode)
        {
            case AdderSubtractor.adder:

                RepeatorSetUp();
                break;

            case AdderSubtractor.adderRight:

                RepeatorSetUp();
                break;

            case AdderSubtractor.adderLeft:

                RepeatorSetUp();
                break;

            case AdderSubtractor.adderBoth:

                
                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractor:

                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractorRight:

                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractorLeft:

                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractorBoth:

                
                RepeatorSetUp();
                break;

            case AdderSubtractor.VtoH_Connect:

                RepeatorSetUp();
                break;

            case AdderSubtractor.none:

                RepeatorSetUp();
                break;

            default:
                break;

        }

       // if(setUP == SpringSetUP.ON)
        {
            setUpFg = true;
        }
        //else if(setUP == SpringSetUP.OFF)
        //{
        //    setUpFg = false;
        //}
        //// ���[�h���c���̐ڑ��̏ꍇ
        //if (machineMode == AdderSubtractor.VtoH_Connect)
        //{
        //    RepeatorSetUp();
        //}

        //// ���[�h���c���̐ڑ��̏ꍇ
        //if (machineMode == AdderSubtractor.adder)
        //{
        //    RepeatorSetUp();
        //    //if (groundObj == null)
        //    //{
        //    //    Debug.LogError("��p�����鎅��ݒ肵�Ă�������");
        //    //}
        //    //// �ݒ肵����p���鎅�̃I�u�W�F�N�g�̐��AvfxManager���擾����
        //    //for (int i = 0; i < groundObj.Count; i++)
        //    //{
        //    //    // �O���E���h�I�u�W�F�����̔z��ԍ��ɐݒ肳��Ă���ꍇ
        //    //    if (groundObj[i] != null)
        //    //    {
        //    //        // ���̎q�I�u�W�F�N�g�ɑ��݂���vfxManager���擾
        //    //        vfxManagers.Add(groundObj[i].transform.GetChild(0).GetComponent<vfxManager>());
        //    //        // ���݂���vfx�̐���ۑ�
        //    //        vfxCount++;
        //    //    }
        //    //}



        //    //// �ݒ肵�����ɂ���āA�g�������Ă�������A�o�Ă����������m�肷��B
        //    //for (int i = 0; i < groundObj.Count; i++)
        //    //{
        //    //    // �c���̏ꍇ
        //    //    if (vfxManagers[i].warpWave == true)
        //    //    {
        //    //        // �I�u�W�F�N�g����ɂ����
        //    //        if (groundObj[i].transform.position.y > transform.position.y)
        //    //        {
        //    //            // ���͂̉\��������
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
        //    //            // �o�͂���ɐݒ�
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
        //    //        }
        //    //        // ���ɂ����
        //    //        else
        //    //        {
        //    //            // ���͂̉\�������
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
        //    //            // �o�͂����ɐݒ�
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
        //    //        }
        //    //    }
        //    //    // �����̏ꍇ
        //    //    else
        //    //    {
        //    //        // �I�u�W�F�N�g���E�ɂ����
        //    //        if (groundObj[i].transform.position.x > transform.position.x)
        //    //        {
        //    //            // ���͂̉\��������
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
        //    //            // �o�͂��E�ɐݒ�
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
        //    //        }
        //    //        // ���ł����
        //    //        else
        //    //        {
        //    //            // ���͂̉\�����E��
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
        //    //            // �o�͂����ɐݒ�
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
        //    //        }

        //    //    }
        //    //}
        //}

        //if (machineMode == AdderSubtractor.adderBoth)
        //{

        //    playerScript = GameObject.FindWithTag("Player").GetComponent<MovePlayer3_3>();
        //    if (playerScript == null)
        //    {
        //        Debug.LogError("�v���C���[�X�N���v�g��������܂���");
        //    }
        //    RepeatorSetUp();
        //}
    }

    //private void Update()
    //{
    //if(modeSetFg == 0)
    //{
    //    for (int i = 0; i < vfxManagers.Length; i++)
    //    {
    //        if (vfxManagers[i].warpWave == true)
    //        {
    //            machineMode = AdderSubtractor.VtoH_Connect;
    //            modeSetFg = 1;
    //        }
    //    }
    //}

    //if (repeatSetFg == true)
    //{
    // �����Ă����g�̌����ɉ������A�E�g�v�b�g�������s���B
    //VtoH_InputCheck();
    //}

    //}

    

    private void FixedUpdate()
    {
        if(setUP == SpringSetUP.OFF)
        {
            if (startSetCheckTime < endSetCheckTime)
            {
                startSetCheckTime++;
            }
            else
            {
                setUpFg = false;
            }
        }
        for(int i = 0;i<vfxCount;i++)
        {
            // �c���̏ꍇ
            if (vfxManagers[i].warpWave == true)
            {
                // �I�u�W�F�N�g����ɂ����
                if (groundObj[i].transform.position.y > transform.position.y)
                {
                    // ���͂̉\��������
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.DOWN);
                    // �o�͂���ɐݒ�
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.UP);
                }//-----if_stop-----
                 // ���ɂ����
                else
                {
                    // ���͂̉\�������
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.UP);
                    // �o�͂����ɐݒ�
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.DOWN);
                }//-----else_stop-----
            }//-----if_stop-----
             // �����̏ꍇ
            else
            {
                // �I�u�W�F�N�g���E�ɂ����
                if (groundObj[i].transform.position.x > transform.position.x)
                {
                    // ���͂̉\��������
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.LEFT);
                    // �o�͂��E�ɐݒ�
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.RIGHT);
                }//-----if_stop-----
                 // ���ł����
                else
                {
                    // ���͂̉\�����E��
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.RIGHT);
                    // �o�͂����ɐݒ�
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.LEFT);
                }//-----else_stop-----
            }//-----else_stop-----
        }
    }


    private void OnApplicationQuit()
    {
        groundObj.Clear();
        vfxManagers.Clear();
        waveInputVelocity.Clear();
        waveOutPutVelocity.Clear();
        enterFg.Clear();
    }
}

