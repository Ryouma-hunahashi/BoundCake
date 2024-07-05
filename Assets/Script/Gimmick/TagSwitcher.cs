using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct RotateSetting
{
    public GameObject rotateObj;
    public ObjectRotation rotationScript;
    public float rotateAngle;
    public float rotateSpeed;
}

public class TagSwitcher : MonoBehaviour
{
    enum RotationSet
    {
        ON,
        OFF,
    }


    [Header("���̃I�u�W�F�����ʂ���X�C�b�`�̃I�u�W�F�N�g")]
    [SerializeField] private GameObject switchObj;
    private VariousSwitches_2 switchScript; // �X�C�b�`�̃X�N���v�g
    private GameObject targetObj;   // �^�O��ύX����I�u�W�F�N�g
    [Header("�X�C�b�`���I���̎��ݒ肵�����^�O��")]
    [SerializeField] private string tagName_On;
    [Header("�X�C�b�`���I�t�̎��ݒ肵�����^�O��")]
    [SerializeField] private string tagName_Off;

    [Space(5),Header("�^�O��ύX�����ۂɃI�u�W�F�N�g����]")]
    [SerializeField] private RotationSet rotationSet = RotationSet.ON;
    
    
    [Header("ON�̏ꍇ�ݒ�")]
    [SerializeField] private List<RotateSetting> l_rotateSettings = new List<RotateSetting>();
    
    


    private bool switchLog;
    // Start is called before the first frame update
    void Start()
    {
        // �^�O��ύX����I�u�W�F�N�g�����̃I�u�W�F��
        targetObj = gameObject;
        // �X�C�b�`�̃X�N���v�g���擾
        switchScript = switchObj.GetComponent<VariousSwitches_2>();
        // �X�C�b�`�̃��O���擾
        switchLog = switchScript.switchStatus;

        // �����ݒ肳��Ă��Ȃ����Unity�̉����^�O���t���Ă��Ȃ���Ԃɂ���B
        if(tagName_On== "")
        {
            tagName_On = "Untagged";
        }
        if (tagName_Off == "")
        {
            tagName_Off = "Untagged";
        }
        // �����̃X�C�b�`��Ԃɉ����ă^�O��ύX
        if (switchLog)
        {
            
            targetObj.tag = tagName_On;
        }
        else
        {
            targetObj.tag = tagName_Off;
        }

        if(rotationSet == RotationSet.ON)
        {
            for(byte i = 0; i < l_rotateSettings.Count; i++)
            {
                if(l_rotateSettings[i].rotateObj != null)
                {
                    RotateSetting set = l_rotateSettings[i];
                    set.rotationScript = l_rotateSettings[i].rotateObj.GetComponent<ObjectRotation>();
                    l_rotateSettings[i] = set;
                    
                }
                else
                {
                    Debug.LogError("�I�u�W�F�N�g�̐ݒ�����Ă�������");
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���O�ƌ��݂̃X�C�b�`��Ԃ��قȂ�ꍇ
        if (switchLog != switchScript.switchStatus)
        {
            // �^�O��ύX����
            if (switchScript.switchStatus)
            {
                targetObj.tag = tagName_On;
                
            }
            else
            {
                targetObj.tag = tagName_Off;
            }

            if (rotationSet == RotationSet.ON)
            {
                for (byte i = 0; i < l_rotateSettings.Count; i++)
                {
                    l_rotateSettings[i].rotationScript.RotateStart(l_rotateSettings[i].rotateAngle, l_rotateSettings[i].rotateSpeed);
                }
            }

            // ���O���X�V
            switchLog = switchScript.switchStatus;
        }
    }

    private void OnApplicationQuit()
    {
        l_rotateSettings.Clear();
    }
}
