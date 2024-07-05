using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// �����̃X�N���v�g�̓t���[�����[�g60�O���z�肵�č���Ă��܂�
// �@�g�p����ۂ̓t���[����60FPS�ŌŒ肷�邩�A
// �@�X�N���v�g�����𒲐����Ă���g�p���Ă�������
//==================================================
// �쐬��2023/03/23
// �{��
public class Test_EnemyDown : MonoBehaviour
{
    // �����擾���
    private Rigidbody rb;           // ���g��[Rigidbody]���擾
    private BoxCollider boxCol;     // ���g��[BoxCollider]���擾

    // �_���[�W����
    [Header("----- �ڐG�����Ƃ��̏�� -----"),Space(5)]
    [Tooltip("�v���C���[�̐U���ɐG�ꂽ�Ȃ�")]
    [SerializeField] private bool hitPlayerWave;        // �U���ɐG�ꂽ����
    [Tooltip("�G�ꂽ�U���̌������擾")]
    [SerializeField] private Vector3 searchDirection;   // �U���̌������擾

    [Header("----- ��s�����̐ݒ� -----"),Space(5)]
    [Tooltip("��΂������̐ݒ�")]
    [SerializeField] private Vector3 blowOfDistance;        // ��s�����̐ݒ�
    [Tooltip("��֔�΂������̍ő�l")]
    [SerializeField] private float blowOfDistanceLimitY;    // �㏸�����̐ݒ�

    [Header("----- ���Ŏ��Ԃ̐ݒ� -----"),Space(5)]
    [Tooltip("���ł܂ł̎��Ԃ̐ݒ�")]
    [SerializeField] private float destroyFrame;        // �I�u�W�F�N�g���ł܂ł̎���
    [Tooltip("�v�����̏��")]
    [SerializeField] private bool nowDestroyCount;      // ���ł܂ł̎��Ԍv�����̔���

    [Header("----- ��Ԋm�F�p -----"),Space(5),Header("�������")]
    [Tooltip("�������")]
    [SerializeField] private bool nowFall;      // �������̔���

    private IEnumerator TimeToDestroy()
    {
        // �v�����J�n����
        nowDestroyCount = true;

        for(int i = 0; i < destroyFrame; i++)
        {//----- for_start -----

            yield return null;

        }//----- for_stop -----

        // �v���I���ケ�̃I�u�W�F�N�g��j�󂷂�
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        // �v���C���[�ŗL�Ƃ����̂��ł���Βǉ����Ă������� -----
        // [Wave]�^�O�̃I�u�W�F�N�g�ɐG�ꂽ�Ȃ�
        if (other.gameObject.tag == "Wave")
        {//----- if_start -----

            // �G�ꂽ����̌������擾
            searchDirection = other.gameObject.transform.localScale;
            hitPlayerWave = true;

        }//----- if_stop -----
    }

    private void Start()
    {
        // ���g��[Rigidbody]���擾����
        rb = this.GetComponent<Rigidbody>();

        // [Rigidbody]���擾�ł��Ă��Ȃ������ꍇ
        if(rb == null)
        {//----- if_start -----

            // �G���[���O��\��������
            Debug.LogError("[Rigidbody]���擾�ł��Ă��܂���I");

        }//----- if_stop -----

        // ���g��[BoxCollider]���擾����
        boxCol = this.GetComponent<BoxCollider>();

        // [BoxCollider]���擾�ł��Ă��Ȃ������ꍇ
        if (boxCol == null)
        {//----- if_start -----

            // �G���[���O��\��������
            Debug.LogError("[BoxCollider]���擾�ł��Ă��܂���I");

        }//----- if_stop -----
    }

    private void Update()
    {
        // �v���C���[�̐U���ɐG�ꂽ�Ȃ�
        if(hitPlayerWave)
        {//----- if_start -----

            // �|���ꂽ�Ƃ��̓������J�n����

            WasKnockedDown();

        }//----- if_stop -----
    }

    //==================================================
    //      �|���ꂽ�Ƃ��̏���
    // ���U���ɓ���������Ɏ��g�����ł���܂ł̏����ł�
    //==================================================
    // �����   2023/03/23
    // �{��
    private void WasKnockedDown()
    {
        // ���g�𐁂���΂�
        rb.velocity = new Vector3(blowOfDistance.x * searchDirection.x, blowOfDistance.y, blowOfDistance.z);

        // �n�ʂ����蔲�������邽�߂ɔ�����g���K�[�ɕύX
        boxCol.isTrigger = true;

        // �������łȂ����
        if(!nowFall)
        {//----- if_start -----

            // �㏸�l������������
            blowOfDistance.y++;

        }//----- if_stop -----
        // �㏸�l�������𒴂������@�܂���
        // �㏸��̗������̏ꍇ
        if((blowOfDistanceLimitY < blowOfDistance.y) || nowFall)
        {//----- if_start -----

            // ������Ԃɂ���
            //nowFall = true;

            // �������x���㏸�����Ă���
            blowOfDistance.y--;

        }//----- if_stop -----

        // ���ł܂ł̌v�����łȂ����
        if(!nowDestroyCount)
        {//----- if_start -----

            // ���ł܂ł̎��Ԃ��v�����n�߂�
            StartCoroutine(TimeToDestroy());

        }//----- if_stop -----
    }
}