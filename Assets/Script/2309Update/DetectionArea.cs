using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{
    // ���G�͈͂̑傫��
    [SerializeField] private GameObject setObject;
    [SerializeField] private float detectRange;

    // ���m������
    private bool isDetect;

    // ���m��Ԃ̃Q�b�^�[
    public bool GetDetection() { return isDetect; }

    private void OnTriggerStay(Collider other)
    {
        // ���G�͈͓��Ƀv���C���[���������Ƃ�
        if(other.gameObject.CompareTag("Player"))
        {
            // ���m��Ԃ��擾����
            isDetect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // ���G�͈͊O�Ƀv���C���[���o����
        if (other.gameObject.CompareTag("Player"))
        {
            // ���m��Ԃ��擾����
            isDetect = false;
        }
    }

    private void Awake()
    {
        // �ی`�̓����蔻����쐬���Ċi�[����
        SphereCollider detectCollision = this.gameObject.AddComponent<SphereCollider>();

        // �����蔻��̐ݒ��ύX����
        detectCollision.radius = detectRange;   // �͈͏�����
        detectCollision.isTrigger = true;       // �g���K�[��
    }

    private void Update()
    {
        // �I�u�W�F�N�g���i�[����Ă��Ȃ���Ώ����𔲂���
        if (setObject == null) return;

        Vector3 objPos = setObject.transform.position;

        // ���̃I�u�W�F�N�g���i�[���ꂽ�I�u�W�F�N�g�Ɉړ�
        this.transform.position = new Vector3(objPos.x, objPos.y, 0);
    }
}
