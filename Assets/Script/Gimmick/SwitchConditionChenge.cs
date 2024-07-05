using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//===================================
// �X�C�b�`�̐؂�ւ����s�������B
// �쓮���������X�C�b�`�X�N���v�g�Ɠ����I�u�W�F�N�g�ɃR���|�[�l���g���Ă��������B
//===================================
// �쐬�� 2023/05/13
// ���c
public class SwitchConditionChenge : MonoBehaviour
{

    
    enum Switch_Type
    { 
        ENTER,
        STAY,
        EXIT,
    }

    enum Active_Parson
    {
        PLAYER,
        ENEMY,
        WAVE,
        COIN,
    }

    enum Default_Status
    {
        ON,
        OFF,
    }

    enum Switch_Rule
    {
        OnToOff,
        OffToOn,
        none,
    }

    enum Switch_Times
    {
        LIMIT,
        UNLIMIT,
    }

    

    private VariousSwitches_2 varSwich;     // �X�C�b�`�̃X�N���v�g
    [Header("�X�C�b�`�̃^�C�v")]
    [Tooltip("TRIGGER�F����������\n" +
             "STAY   �F�������Ă����\n" +
             "EXIT   �F���ꂽ��")]
    [SerializeField] private Switch_Type type = Switch_Type.ENTER;
    
    [Header("�X�C�b�`���I���ɂ���I�u�W�F�N�g")]
    [SerializeField] private Active_Parson[] activeParson = { Active_Parson.WAVE };
    [Header("�X�C�b�`�̏������")]
    [SerializeField] private Default_Status defaultStatus;
    private bool defaultSwitch = false;
    [Header("�X�C�b�`�̋�������(Stay�̏ꍇ�͎g�p���Ȃ��ł�������)")]
    [Tooltip("ONtoOFF�FON����OFF�̂ݕύX��\n" +
             "OFFtoON�FOFF����ON�̂ݕύX��\n" +
             "none   �F�����邽�тɐ؂�ւ�")]
    [SerializeField] private Switch_Rule rule = Switch_Rule.none;



    [Header("�X�C�b�`�̍쓮��")]
    [Tooltip("LIMIT  �F���E����\n" +
             "UNLIMIT�F������")]
    [SerializeField] private Switch_Times switchTimes = Switch_Times.UNLIMIT;
    [Tooltip("�쓮�񐔂�[ LIMIT ]�̏ꍇ�A�񐔂��w��")]
    [SerializeField] private byte limitTimes = 1;
    private byte nowTime = 0;        // ���݂̃X�C�b�`�쓮��
    [Header("�X�C�b�`�̃N�[���^�C��")]
    [SerializeField] private Default_Status coolTimeCheck = Default_Status.ON;
    [SerializeField] private float coolTime = 30;
    private byte nowCoolTime = 0;
    private bool nowCool = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // ���̃I�u�W�F�N�g���̃X�C�b�`���i�[
        varSwich = GetComponent<VariousSwitches_2>();
        
        // ���E�l��0�ȉ��̎���1�ɌŒ�B(�o�O��U�炩������)
        if(limitTimes <= 0)
        {
            limitTimes = 1;
        }

        // Stay�̎��̓N�[���^�C���𔭐������Ȃ��悤�ɂ���B
        if(type == Switch_Type.STAY)
        {
            coolTimeCheck = Default_Status.OFF;
        }

        // �����̃X�C�b�`��Ԃ��i�[
        switch(defaultStatus)
        {
            case Default_Status.ON:
                varSwich.switchStatus = true;
                defaultSwitch = true;
                break;
            case Default_Status.OFF:
                varSwich.switchStatus= false;
                defaultSwitch = false;
                break;
        }
        
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void FixedUpdate()
    {
        if(nowCool)
        {
            if(nowCoolTime<coolTime)
            {
                nowCoolTime++;
            }
            else
            {
                nowCoolTime = 0;
                nowCool = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!nowCool)
        {
            // �񐔐������Ȃ����A�񐔐����Ɏ����Ă��Ȃ��Ƃ�
            if (switchTimes == Switch_Times.UNLIMIT || nowTime < limitTimes)
            {
                // �����������A�������Ă���Ԃ̃X�C�b�`��؂�ւ���B
                switch (type)
                {
                    case Switch_Type.ENTER:
                        ActiveCheck(other);
                        break;
                        //case Switch_Type.STAY:
                        //    ActiveCheck(other);
                        //    break;

                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (varSwich.switchStatus == defaultSwitch)
        {
            // �񐔐������Ȃ����A�񐔐����Ɏ����Ă��Ȃ��Ƃ�
            if (switchTimes == Switch_Times.UNLIMIT || nowTime <= limitTimes)
            {
                // �������Ă���ԁA���ꂽ���̃X�C�b�`��؂�ւ���B
                switch (type)
                {
                    case Switch_Type.STAY:
                        ActiveCheck(other);
                        break;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // �񐔐������Ȃ����A�񐔐����Ɏ����Ă��Ȃ��Ƃ�
        if (switchTimes == Switch_Times.UNLIMIT || nowTime < limitTimes)
        {
            // �������Ă���ԁA���ꂽ���̃X�C�b�`��؂�ւ���B
            switch (type)
            {
                case Switch_Type.STAY:
                    ActiveCheck(other);
                    break;
                case Switch_Type.EXIT:
                    if (!nowCool)
                    {
                        ActiveCheck(other);
                    }
                    break;
            }
        }
    }

    //===============================================
    // �X�C�b�`��؂�ւ�����Ώۂɉ����āA�؂�ւ��邩�̔��ʂ��s��
    // �����F�Ԃ������I�u�W�F�N�g��Collider
    // �߂�l����
    //===============================================
    // �쐬�� 2023/05/13
    // ���c
    private void ActiveCheck(Collider other)
    {
        // �쓮��������Ώۂ̐��J��Ԃ�
        for (byte i = 0; i < activeParson.Length; i++)
        {
            switch (activeParson[i])
            {
                case Active_Parson.PLAYER:
                    if(other.CompareTag("Player"))
                    {
                        switchChange();
                    }
                    break;
                case Active_Parson.ENEMY:
                    if (other.CompareTag("Enemy"))
                    {
                        switchChange();
                    }
                    break;
                case Active_Parson.WAVE:
                    if (other.CompareTag("Wave"))
                    {
                        switchChange();
                        varSwich.collisionScript = other.GetComponent<waveCollition>();
                    }
                    break;
                case Active_Parson.COIN:
                    if (other.CompareTag("Coin"))
                    {
                        switchChange();
                    }
                    break;
            }
            
        }
    }
    //======================================
    // �X�C�b�`��؂�ւ��鏈��
    // �����A�߂�l����
    //======================================
    // �쐬�� 2023/05/13
    // ���c
    private void switchChange()
    {
        
        switch (rule)
        {
            case Switch_Rule.OnToOff:
                if (varSwich.switchStatus)
                {
                    varSwich.switchStatus = false;
                    // �؂�ւ����E�����݂���΁A�؂�ւ��񐔂����Z
                    if (switchTimes == Switch_Times.LIMIT)
                    {
                        AddChangeCount();
                        varSwich.AddFamilyChangeCount();

                    }
                    if(coolTimeCheck == Default_Status.ON)
                    {
                        SetCoolTime();
                        varSwich.SetFamilyCoolTime();
                    }
                }
                break;
            case Switch_Rule.OffToOn:
                if (!varSwich.switchStatus)
                {
                    varSwich.switchStatus = true;
                    // �؂�ւ����E�����݂���΁A�؂�ւ��񐔂����Z
                    if (switchTimes == Switch_Times.LIMIT)
                    {
                        AddChangeCount();
                        varSwich.AddFamilyChangeCount();

                    }
                    if (coolTimeCheck == Default_Status.ON)
                    {
                        SetCoolTime();
                        varSwich.SetFamilyCoolTime();
                    }
                }
                break;
            case Switch_Rule.none:
                // switchStatus���t�]������
                if (varSwich.switchStatus)
                {
                    varSwich.switchStatus = false;
                    
                }
                else
                {
                    varSwich.switchStatus = true;
                }
                // �؂�ւ����E�����݂���΁A�؂�ւ��񐔂����Z
                if (switchTimes == Switch_Times.LIMIT)
                {
                    AddChangeCount();
                    varSwich.AddFamilyChangeCount();

                }
                if (coolTimeCheck == Default_Status.ON)
                {
                    SetCoolTime();
                    varSwich.SetFamilyCoolTime();
                }

                break;
        }        
        
    }
    public void SetCoolTime()
    {
        nowCool = true;
    }
    public void AddChangeCount()
    {
        nowTime++;
    }
    
}
