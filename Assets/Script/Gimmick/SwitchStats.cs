using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchStats : MonoBehaviour
{
    // �I�u�W�F�N�g�̎�ނ̐ݒ�
    private enum ObjectType
    {
        breakObject,
        itemObject,
        switchObject,

        none,   // �Ȃɂ��ݒ肵�Ă��Ȃ�
    }

    [Header("----- ��{�ݒ� -----"), Space(5)]
    [Tooltip("���Ή��I�u�W�F�N�g�̐ݒ�")]
    [SerializeField] private ObjectType type = ObjectType.none;

    [Header("----- �u���b�N�ݒ� -----")]
    // �v���n�u�̊i�[
    [Tooltip("���u���b�N����o��������Prefab��ݒ�")]
    [SerializeField] private GameObject itemPrefab;

    [Header("----- �X�C�b�`�ݒ� -----"), Space(5)]
    // �A�N�e�B�u��Ԃ��O���֓n��
    [Tooltip(
        "���݂̃X�C�b�`�̏��( ON / OFF )\n" +
        "���̏�Ԃ͊O���֓n���܂�")]
    public bool activeSwitch;

    

    // ���̃X�N���v�g���݂̂̃X�C�b�`�̏��
    private bool switchSituation;

    // �X�C�b�`�̎�ނ̐ݒ�
    private enum SwitchModeList
    {//----- enum_start -----

        // press,      // �����Ă���Ԃ̂�[true]
        flipflop,   // �����Ȃ����܂�[true]
        none,

    }//----- enum_stop -----

    [Header("----- �j��ݒ� -----")]
    [SerializeField] private string[] tagName;

    // �C���X�y�N�^�[��ŃX�C�b�`�̓�����ύX����
    [Tooltip("���X�C�b�`�̋N�����@��ݒ�")]
    [SerializeField] SwitchModeList switchMode = SwitchModeList.none;


    // �I�u�W�F�N�g�������ɐG��Ă���Ƃ��̔���
    [Header("----- ��Ԋm�F�p -----"), Space(5)]
    [Tooltip("�I�u�W�F�N�g�������ɐG�ꂽ���̔���")]
    [SerializeField] private bool hitObject;

    // �Փˎ��̔�����擾
    private void OnTriggerEnter(Collider collider)
    {
        switch (type)
        {
            case ObjectType.breakObject:
                // [Wave]�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�Ȃ�
                if (CheckTag(collider.gameObject.tag))
                {//----- if_start -----

                    // �u���b�N�j��̃A�j���[�V����
                    // �\��n

                    // �f�o�b�O���O
                    Debug.Log("[Bullet]�^�O�̕t�����I�u�W�F�N�g�ɂ���Ĕj�󂳂�܂����I");

                    // �������g��j�󂷂�
                    gameObject.SetActive(false);

                }//----- if_stop -----
                break;
            case ObjectType.itemObject:
                // [Wave]�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�Ȃ�
                if (collider.gameObject.CompareTag("Wave"))
                {//----- if_start -----

                    // �f�o�b�O���O
                    Debug.Log("[Wave]�^�O�̕t�����I�u�W�F�N�g���Ԃ���܂����I");

                    // ��������������������o��
                    hitObject = true;

                }//----- if_stop -----
                break;
            case ObjectType.switchObject:
                hitObject = true;
                break;
            default:
                break;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (type)
        {
            case ObjectType.breakObject:
                // [Wave]�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�Ȃ�
                if (CheckTag(other.gameObject.tag))
                {//----- if_start -----

                    // �u���b�N�j��̃A�j���[�V����
                    // �\��n

                    // �f�o�b�O���O
                    Debug.Log("[Bullet]�^�O�̕t�����I�u�W�F�N�g�ɂ���Ĕj�󂳂�܂����I");

                    // �������g��j�󂷂�
                    gameObject.SetActive(false);

                }//----- if_stop -----
                break;
            case ObjectType.itemObject:
                // [Wave]�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�Ȃ�
                if (other.gameObject.CompareTag("Wave"))
                {//----- if_start -----

                    // �f�o�b�O���O
                    Debug.Log("[Wave]�^�O�̕t�����I�u�W�F�N�g���Ԃ���܂����I");

                    // ��������������������o��
                    hitObject = true;

                }//----- if_stop -----
                break;
            case ObjectType.switchObject:
                hitObject = true;
                break;
            default:
                break;
        }
    }

    // �������̔�����擾
    private void OnCollisionExit(Collision collision)
    {
        hitObject = false;
    }

    private bool CheckTag(string _objTag)
    {
        for(int i = 0; i < tagName.Length; i++)
        {
            if(_objTag == tagName[i])
            {
                return true;
            }
        }

        return false;
    }

    //==================================================
    //      �A�C�e���o���u���b�N
    // ��[Wave]�q�b�g���Ɏw�肳�ꂽPrefab���o��������֐��ł�
    //==================================================
    // �����2023/03/12
    // �{��
    public void HitItemBlock()
    {
        // �����ʒu
        Vector3 pos = new Vector3(
            this.transform.position.x,
            this.transform.position.y + 1,
            this.transform.position.z);

        Instantiate(itemPrefab,pos,Quaternion.identity);

        // �A�C�e���o���Ɋւ���
        // �W�����v�o�O����̂ňꎞ�I�ɂ���ŁA
        // �����ɑΏ��\�Ȃ̂Ŏ��Ԃ���l�ł���Βm�b�����Ă�������
        // ���̊֐���[bool]�œn���悤�ɂ���
        // �擾����[bool]�ŏo�����ύX���Ă�������
        
        hitObject = false;
    }

    //==================================================
    //      �t���b�v�t���b�v��H
    // ���I���A�I�t�̐؂�ւ��ɉ����Ȃ������K�v�ɂȂ�X�C�b�`�ł�
    //==================================================
    // �����2023/03/9
    // �{��
    public bool FlipFlopSwitch()
    {
        // �����I�u�W�F�N�g���������g�ɐG�ꂽ�Ƃ�
        if(hitObject)
        {//----- if_start -----

            // �X�C�b�`�̏�Ԃ�
            // [true]�̂Ƃ���[false]�ɐ؂�ւ��
            // [false]�̂Ƃ���[true]�ɐ؂�ւ��
            if(activeSwitch && !switchSituation )
            {//----- if_start -----

                activeSwitch = false;

            }//----- if_stop -----
            else if(!activeSwitch && !switchSituation)
            {//----- elseif_start -----

                activeSwitch = true;

            }//----- elseif_stop -----

            // �������������Ă����ԂȂ�
            // �X�C�b�`�̏�Ԃ��I���ɂ���
            switchSituation = true;

        }//----- if_stop -----
        else
        {//----- else_start -----

            // �����������Ă��Ȃ���ԂȂ�
            // �X�C�b�`�̏�Ԃ��I�t�ɂ���
            switchSituation = false;

        }//----- else_stop -----

        // [activeSwitch]�̏�Ԃ𑗂�
        return activeSwitch;
    }

    private void Update()
    {
        switch (type)
        {
            case ObjectType.breakObject:
                break;
            case ObjectType.itemObject:
                // �v���n�u���i�[����Ă��Ȃ��Ƃ��ɃG���[��\��
                if (itemPrefab == null)
                {
                    Debug.LogError("[itemPrefab]���ɃI�u�W�F�N�g���i�[����Ă��܂���I");
                }

                // �I�u�W�F�N�g�����������Ƃ��̏���
                if (hitObject)
                {
                    HitItemBlock();
                }
                break;
            case ObjectType.switchObject:
                switch (switchMode)
                {//----- switch_start -----

                    //case SwitchModeList.press:
                    //    Debug.LogError("���삳��Ă��Ȃ���Ԃ��ݒ肳��Ă��܂�");
                    //    break;
                    case SwitchModeList.flipflop:
                        FlipFlopSwitch();
                        break;
                    case SwitchModeList.none:
                    default:
                        Debug.LogError("�X�C�b�`�̓������ݒ肳��Ă��܂���I");
                        break;

                }//----- switch_stop -----
                break;
            case ObjectType.none:
            default:
                Debug.LogError("�I�u�W�F�N�g�̎�ނ��ݒ肳��Ă��܂���I");
                break;
        }
    }

}
