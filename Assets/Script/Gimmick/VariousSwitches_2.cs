using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==================================================
//          �V�K�X�C�b�`�̃X�N���v�g�ł�
// �����X�C�b�`�X�N���v�g�ƕʂ̖��O�ɂȂ��Ă��܂���
// �@���̐V�X�C�b�`�X�N���v�g��[ Ver 2 ]�Ƃ��������ł��B
// ���ߋ��X�C�b�`�X�N���v�g�ɓ�������Ă����u���b�N�̔j��
// �@�����ł͔j��,�܂��͕ύX�����\��������܂��B
// ���V���ȋ@�\�Ƃ��ĘA���X�C�b�`,XOR��H���ǉ��\��ł�
//==================================================
// �����2023/04/12
// �{��
public class VariousSwitches_2 : MonoBehaviour
{
    [Tooltip("�X�C�b�`�̏��")]
    public bool switchStatus;
    private bool saveSwitchStatus;

    [Header("----- �A�����̐ݒ� -----"), Space(5)]
    [SerializeField] private GameObject myParent;                                   // �e�I�u�W�F�N�g�̎擾
    [SerializeField] private VariousSwitches_2 parentScript;
    [SerializeField] private SwitchConditionChenge parentChenger;
    [SerializeField] private bool parentActive;

    [SerializeField] private List<GameObject> myChildren = new List<GameObject>();  // �q�I�u�W�F�N�g�̎擾
    [SerializeField] private List<VariousSwitches_2> childScripts = new List<VariousSwitches_2>();
    [SerializeField] private List<SwitchConditionChenge> childChengers = new List<SwitchConditionChenge>();
    [SerializeField] private bool childrenActive;

    [Header("XOR�A���̐ݒ�")]
    [SerializeField] private GameObject versusSwitch;
    [SerializeField] private VariousSwitches_2 versusScript;

    //private Renderer test;

    [System.NonSerialized] public waveCollition collisionScript;

    private void Start()
    {
        //test = this.GetComponent<Renderer>();

        // �e�����݂��Ă���Ȃ�
        if (this.transform.parent != null)
        {
            // �e�I�u�W�F�N�g���擾����
            myParent = this.transform.parent.gameObject;
            parentScript = this.transform.parent.GetComponent<VariousSwitches_2>();
            parentChenger = this.transform.parent.GetComponent<SwitchConditionChenge>();
            if (parentScript != null)
            {
                // �e�I�u�W�F�N�g�����݂��Ă���
                parentActive = true;

                // ���g�̖��O��ύX����
                this.gameObject.name = "childSwitch";
            }
        }//----- if_stop -----

        // �q�����݂��Ă���Ȃ�
        if (this.transform.childCount != 0)
        {
            // �q�I�u�W�F�N�g�̐����擾
            int childCount = this.transform.childCount;

            // ���X�g����x����������
            myChildren.Clear();
            childScripts.Clear();
            childChengers.Clear();

            // ���g�ɂ��Ă���q�I�u�W�F�N�g���擾����
            for (int i = 0; i < childCount; i++)
            {
                // �q�I�u�W�F�N�g�����X�g���Ɋi�[
                myChildren.Add(transform.GetChild(i).gameObject);
                childScripts.Add(transform.GetChild(i).GetComponent<VariousSwitches_2>());
                childChengers.Add(transform.GetChild(i).GetComponent<SwitchConditionChenge>());

            }//----- for_stop -----

            // �q�I�u�W�F�N�g�����݂��Ă���
            childrenActive = true;

            // ���g�̖��O��ύX����
            this.gameObject.name = "parentSwitch";

        }//----- if_stop -----

        // �΂����݂��Ă���Ȃ�
        if (this.versusSwitch != null)
        {
            // �΂̃X�N���v�g�����擾
            versusScript = this.versusSwitch.GetComponent<VariousSwitches_2>();
        }//----- if_stop -----

        // �ŏ��ɕێ����Ă���l�Ɠ��l�̏ꍇ���s
        if (switchStatus == saveSwitchStatus)
        {
            // �ێ����Ă���l��ύX����
            saveSwitchStatus = !saveSwitchStatus;

        }//----- if_stop -----

    }

    private void Update()
    {
        // �X�C�b�`�̏�ԂɕύX���������ꍇ
        if (switchStatus != saveSwitchStatus)
        {
            // ���O�Ɍ��݂̃X�C�b�`�̏�Ԃ�ۑ�����
            saveSwitchStatus = switchStatus;

            // �e�I�u�W�F�N�g�݂̂����݂��Ă���Ȃ�
            if (parentActive && !childrenActive)
            {
                Debug.Log("�e�̂ݑ���");

                // ���g���ύX���ꂽ�Ƃ��ɐe�̏�Ԃ�ύX����
                parentScript.switchStatus = switchStatus;

            }//----- if_stop -----
            // �q�I�u�W�F�N�g�݂̂����݂��Ă���Ȃ�
            else if (childrenActive && !parentActive)
            {
                Debug.Log("�q�̂ݑ���");

                // ���g���ύX���ꂽ�Ƃ��Ɏq�̏�Ԃ�ύX����
                for (int i = 0; i < childScripts.Count; i++)
                {
                    Debug.Log("�ύX���ꂽ��");

                    childScripts[i].switchStatus = switchStatus;
                }//----- for_stop -----

            }//----- elseif_stop -----
            // �e�q���ɃI�u�W�F�N�g�����݂��Ă��Ȃ��Ȃ�
            else if (!parentActive && !childrenActive)
            {
                Debug.Log("�e�q���݂��Ă��Ȃ�");

            }//----- elseif_stop -----

            //// �e�X�g�ŐF��ύX����
            //if (switchStatus)
            //{
            //    test.material.color = Color.blue;

            //}//----- if_stop -----
            //else
            //{
            //    test.material.color = Color.red;

            //}//----- else_stop -----

        }//----- if_stop -----

        // �΂����݂��Ă���Ȃ�
        if (versusSwitch != null)
        {
            // �΂Ɠ�����ԂɂȂ����Ȃ�
            if (versusScript.switchStatus == switchStatus)
            {
                Debug.Log("�Δ���");

                // ���̏�Ԃ𔽓]������
                switchStatus = !switchStatus;

            }//----- if_stop -----
        }//----- if_stop -----

        //// �e�I�u�W�F�N�g�݂̂����݂��Ă���Ȃ�
        //if (parentActive && !childrenActive)
        //{
        //    Debug.Log("�e�������܁`��");

        //    // ���g�̏�Ԃ��ύX���ꂽ�Ȃ�
        //    if(switchStatus != saveSwitchStatus)
        //    {
        //        // �e�̏�Ԃ��㏑������
        //        parentScript.switchStatus = switchStatus;

        //    }//----- if_stop -----
        //}
        //// �q�I�u�W�F�N�g�݂̂����݂��Ă���Ȃ�
        //else if(childrenActive && !parentActive)
        //{
        //    Debug.Log("�q�������܁`��");

        //    if (switchStatus != saveSwitchStatus)
        //    {
        //        saveSwitchStatus = switchStatus;

        //        if (switchStatus)
        //        {
        //            GetComponent<Renderer>().material.color = Color.blue;
        //        }
        //        else
        //        {
        //            GetComponent<Renderer>().material.color = Color.red;
        //        }
        //    }
        //}
        //// �e,�q�I�u�W�F�N�g�����ɑ��݂��Ă��Ȃ�
        //else if(!parentActive && !childrenActive)
        //{
        //    Debug.Log("�������܂��`��");

        //    if (switchStatus != saveSwitchStatus)
        //    {
        //        saveSwitchStatus = switchStatus;

        //        if (switchStatus)
        //        {
        //            GetComponent<Renderer>().material.color = Color.blue;
        //        }
        //        else
        //        {
        //            GetComponent<Renderer>().material.color = Color.red;
        //        }
        //    }
        //}
    }

    public void AddFamilyChangeCount()
    {
        if(parentActive)
        {
            parentChenger.AddChangeCount();
        }
        if(childrenActive)
        {
            for(byte i =0; i < childChengers.Count; i++)
            {
                if(childChengers[i] != null)
                {
                    childChengers[i].AddChangeCount();
                }
            }
        }
    }

    public void SetFamilyCoolTime()
    {
        if (parentActive)
        {
            parentChenger.SetCoolTime();
        }
        if (childrenActive)
        {
            for (byte i = 0; i < childChengers.Count; i++)
            {
                if (childChengers[i] != null)
                {
                    childChengers[i].SetCoolTime();
                }
            }
        }
    }
   

    private void OnApplicationQuit()
    {
        myChildren.Clear();
        childScripts.Clear();
        childChengers.Clear();
    }

}
