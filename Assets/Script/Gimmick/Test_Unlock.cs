using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Unlock : MonoBehaviour
{
    // ���g�̏����擾
    private Vector3 m_Position;     // ���g�̍��W
    private Vector3 m_Scale;        // ���g�̑傫��

    [SerializeField] private LayerMask waveLayer;

    // �ǂ̖ʂŔ�����Ƃ邩
    private enum SurfaceDistanceType
    {//----- enum_start -----

        top,    // ���
        left,   // ����
        right,  // �E��
        bottom, // ����

        none,   // �Ȃ�

    }//----- enum_stop -----

    // ����̎擾���������߂�
    [Header("----- �ڐG�ʂ̐ݒ� -----"),Space(5)]
    [Tooltip("�ݒ肳�ꂽ�ʂ�[Wave]��������Ɣj�󂳂�܂�")]
    [SerializeField] private SurfaceDistanceType blockSurfaceDistance = SurfaceDistanceType.none;
    private SurfaceDistanceType m_DistanceType;     // �ڒn�ʂ̐ݒ�̕ێ�
    private bool changeDistanceType = false;        // �ڒn�ʕύX�̋���

    [Header("----- ���C�̐ݒ� -----")]
    [Tooltip("�ڐG�\������ݒ�")]
    [SerializeField] private float rayDirection;    // ���C�̋�����ݒ�
    private float changeRayDirectionX;              // ���C�̌�����[X]���ύX
    private float changeRayDirectionY;              // ���C�̌�����[Y]���ɕύX

    private void Start()
    {
        // ���g�̏��̕ێ�
        m_Position = transform.position;
        m_Scale = transform.localScale;
    }

    private void Update()
    {
        // ���g�̍��W�ɕύX��������ꂽ�Ƃ�
        if(m_Position != this.transform.position)
        {//----- if_start -----

            // ���g�̕ێ����Ă�����W���X�V����
            m_Position = this.transform.position;

        }//----- if_stop -----

        // �ݒ肳��Ă���j��\�Ȗʂŏ�����ύX
        switch(blockSurfaceDistance)
        {//----- switch_start -----

            case SurfaceDistanceType.top:

                // ���C�̌�����ύX����
                changeRayDirectionX = 0;
                changeRayDirectionY = 1;

                // ���C���o��������
                RaySetting();

                break;
            case SurfaceDistanceType.left:

                // ���C�̌�����ύX����
                changeRayDirectionX = -1;
                changeRayDirectionY = 0;

                // ���C���o��������
                RaySetting();

                break;
            case SurfaceDistanceType.right:

                // ���C�̌�����ύX����
                changeRayDirectionX = 1;
                changeRayDirectionY = 0;

                // ���C���o��������
                RaySetting();

                break;
            case SurfaceDistanceType.bottom:

                // ���C�̌�����ύX����
                changeRayDirectionX = 0;
                changeRayDirectionY = -1;

                // ���C���o��������
                RaySetting();

                break;
            // �˂��ݒ肳��Ă��Ȃ������ꍇ�̃G���[���O
            case SurfaceDistanceType.none:
            default:
                // �G���[��\��������
                Debug.LogError("�j��\�Ȗʂ��ݒ肳��Ă��܂���I");
                break;

        }//----- switch_stop -----
    }

    private void RaySetting()
    {
        // ���C�̈ʒu,������ݒ肷��
        Ray hitRay = new Ray(new Vector3(m_Position.x, m_Position.y, m_Position.z), new Vector3(changeRayDirectionX, changeRayDirectionY, 0));
        RaycastHit hitWave;

        // ���C����ʏォ�猩����悤�ɂ���
        Debug.DrawRay(hitRay.origin, hitRay.direction * rayDirection, Color.red, 1, false);

        // ���C��[Wave]�ɓ��������Ƃ��̏���
        if(Physics.Raycast(hitRay, out hitWave, rayDirection, waveLayer))
        {//----- if_start -----

            // ���̂�����͐U���̕����Ƃ��擾���Ă������Ă���������

            // ���̃I�u�W�F�N�g��j�󂷂�
            Destroy(this.gameObject);

        }//----- if_stop -----
    }
}
