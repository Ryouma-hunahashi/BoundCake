using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �Ή���2023/04/11
// �{��
public class Enemy_A_Rush : MonoBehaviour
{
    [SerializeField]
    private GameObject myParent;    // �����̐e���擾

    // ���g�̏����擾
    private Rigidbody rb;
    [SerializeField]
    private Vector3 parentScale;    // �e�̑傫��
    private IEnumerator rushAct;

    public bool rushNow;
    public bool rushEnd;

    // �����ʒu���擾
    [Tooltip("�ːi�̊J�n�n�_��ێ�����")]
    [SerializeField] private Vector3 startPosition;      // �ːi�J�n�n�_��ێ�
    [Tooltip("�ːi�̏I���n�_��\������")]
    [SerializeField] private Vector3 stopPosition;       // �ːi�I���n�_��\��

    // ���W���擾�������ǂ���
    private bool positionSaved;

    [Tooltip("�ړ����x�̐ݒ�")]
    [SerializeField] private float rushSpeed;           // ���x�̎w��
    [Tooltip("�ړ������̐ݒ�")]
    [SerializeField] private float rushDirection;       // �����̎w��
    [Tooltip("�����x�̐ݒ�"), Range(0f, 1f)]
    [SerializeField] private float rushAcceleration;    // �����̎w��
    [Tooltip("�����x�̐ݒ�"), Range(0f, 1f)]
    [SerializeField] private float rushDiceleration;    // �����̎w��

    //==================================================
    //      �ːi�U��
    // ���G�L�����N�^�[�̓ːi�U���p�̊֐�
    // ���ʊ֐�����Ăяo���Ďg�p���Ă�������
    //==================================================
    // �����2023/04/03    �X�V��2023/04/08
    // �{��
    public IEnumerator RushAction()
    {
        rushNow = true;
        rushEnd = false;

        if(!positionSaved)
        {//----- if_start -----

            // �ːi�J�n���ɂ��̍��W���擾���Ă���
            startPosition = myParent.transform.position;

            // �ːi�̏I���n�_��\��
            stopPosition = myParent.transform.position;
            stopPosition.x += rushDirection;

            // ���W�̕ێ��󋵂�'��'�ɂ���
            positionSaved = true;

        }//----- if_stop -----

        // �ړ��������E��[X+]�����Ȃ�
        if(rushDirection > 0)
        {//----- if_start-----

            while (stopPosition.x > myParent.transform.position.x)
            {//----- while_start -----

                yield return null;

                // ���x���X�V����
                rb.velocity = new Vector3(rushSpeed, 0, 0);

            }//----- while_stop -----

            // ������␳����
            // ���g���E�������Ă���̂Ȃ�
            if (myParent.transform.localScale.x > 0)
            {
                // ���g���������֌�������
                myParent.transform.localScale = new Vector3(-parentScale.x, parentScale.y, parentScale.z);
                Debug.Log("���ɂނ����܂����I");


                // �ړ�������ύX����
                rushDirection *= -1;

            }//----- if_stop -----
        }//----- if_stop -----
        // �ړ�����������[X-]�����Ȃ�
        else if (rushDirection < 0)
        {//----- if_start-----

            while (stopPosition.x < myParent.transform.position.x)
            {//----- while_start -----

                yield return null;

                // ���x���X�V����
                rb.velocity = new Vector3(-rushSpeed, 0, 0);

            }//----- while_stop -----

            // �������C������
            // ���g�����������Ă���̂Ȃ�
            if (myParent.transform.localScale.x < 0)
            {
                // ���g���E�����֌�������
                myParent.transform.localScale = new Vector3(parentScale.x, parentScale.y, parentScale.z);
                Debug.Log("�E�ɂނ����܂����I");

                // �ړ�������ύX����
                rushDirection *= -1;

            }//----- if_stop -----
        }//----- else if_stop -----

        // �ːi�I���n�_�������݈ʒu�̒l���Ⴂ�Ƃ�

        // �ʒu��␳����
        rb.velocity = Vector3.zero;
        rb.position = stopPosition;

        rushNow = false;
        rushEnd = true;

        positionSaved = false;

    }

    private void Start()
    {
        // ���g�̖��O��"rush"�ɂ���
        this.gameObject.name = "rush";

        // �����̐e���擾
        myParent = transform.parent.gameObject;

        // �e�̑傫�����擾
        parentScale = myParent.transform.localScale;
        if(parentScale.x < 0)
        {
            parentScale.x *= -1;
        }

        // [Rigidbody]�̎擾
        rb = myParent.GetComponent<Rigidbody>();

        // ���܂��擾�ł��Ȃ������ꍇ
        if (rb == null)
        {
            Debug.LogError("[Rigidbody]��������܂���I");

        }//----- if_stop -----
    }
}
