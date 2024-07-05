using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X�̓ːi�����X�N���v�g
// ���w�肵������,����,���x�œːi���J�n���܂�
//==================================================
// �����2023/05/16
// �{��
public class Boss1_Rush : MonoBehaviour
{
    // �e�̏����擾
    private GameObject par_Obj;
    private Animator par_Anim;
    private Rigidbody par_Rb;
    private Vector3 par_Scale;

    // �e�̋��������擾
    private Boss1_Main par_Main;
    private Boss1_Shield par_Shield;

    // �R���[�`���̊i�[
    [System.NonSerialized] public IEnumerator cor_Rush; // �ːi

    // �ːi���
    public bool nowRush;

    // �ːi�̊e�n�_���擾
    public Vector3 startPos;    // �J�n�n�_�̕ێ�
    public Vector3 finishPos;   // �I���n�_�̗\��

    public Vector3 fallPos;  // ���n���̈ʒu��ێ�

    // ���W���擾�������ǂ���
    private bool savedPos;

    // �ːi�̐ݒ�
    [SerializeField] private float speed = 16;      // ���x
    [SerializeField] private float direction = -26; // ����
    [SerializeField] private float acceleration;    // �����x
    [SerializeField] private float diceleration;    // �����x

    private void Start()
    {
        // ���g�̖��O��"rush"�ɂ���
        this.gameObject.name = "rush";

        // �e�̏����擾
        par_Obj = transform.parent.gameObject;
        par_Anim = par_Obj.GetComponent<Animator>();
        par_Rb = par_Obj.GetComponent<Rigidbody>();

        // �e�̋��������擾
        par_Main = transform.parent.GetComponent<Boss1_Main>();
        par_Shield = transform.parent.GetComponentInChildren<Boss1_Shield>();

        cor_Rush = RushAction();

        // �e�̑傫�����擾
        par_Scale = par_Obj.transform.localScale;

        // ���΂������Ă���Ȃ�
        if (par_Scale.x < 0)
        {
            // �傫����␳����
            par_Scale.x *= -1;

        }//----- if_stop -----

        // �����ʒu���擾
        fallPos = par_Obj.transform.localPosition;
    }

    //==================================================
    //          ���\���ːi�U��
    // ���h��s����A�����\���Ȃ���ːi����
    //==================================================
    // �����2023/05/16
    // �{��
    public IEnumerator RushAction()
    {
        if (par_Shield.nowStan) yield break;
        

        Debug.Log("�ːi���J�n���܂������I");

        // �ːi��Ԃɂ���
        nowRush = true;
        if (par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.Stop();
        }
        par_Main.audio.bossSource.loop = true;
        par_Main.audio.BossDashSound();
        // �ːi�A�j���[�V�������J�n
        par_Anim.SetBool("rushing", true);

        // �ːi�J�n���̍��W���擾���Ă���
        startPos = par_Obj.transform.position;

        // �ːi�I���n�_��\������
        finishPos = par_Obj.transform.position;
        finishPos.x += direction;

        // �ړ��������E��(X++)�����Ȃ�
        if (direction > 0)
        {
            while (finishPos.x > par_Obj.transform.position.x)
            {
                // �P�t���[���x��������
                yield return null;

                // ���x���X�V����
                par_Rb.velocity = new Vector3(speed, 0, 0);

                
            }//----- while_stop -----

            // ������␳����
            // ���g���E�������Ă���Ȃ�
            if (par_Obj.transform.localScale.x > 0)
            {
                // ���g���������֌�������
                par_Obj.transform.localScale = new Vector3(-par_Scale.x, par_Scale.y, par_Scale.z);

                Debug.Log("���Ɍ������܂������I");

                // �ړ�������ύX����
                direction *= -1;

            }//----- if_stop -----

        }//----- if_stop -----
        // �ړ�����������(X--)�����Ȃ�
        else if (direction < 0)
        {
            while (finishPos.x < par_Obj.transform.position.x)
            {
                // �P�t���[���x��������
                yield return null;

                // ���x���X�V����
                par_Rb.velocity = new Vector3(-speed, 0, 0);

            }//----- while_stop -----

            // �������C������
            // ���g�����������Ă���Ȃ�
            if (par_Obj.transform.localScale.x < 0)
            {
                // ���g���E�����֌�������
                par_Obj.transform.localScale = new Vector3(par_Scale.x, par_Scale.y, par_Scale.z);

                Debug.Log("�E�Ɍ������܂������I");

                // �ړ�������ύX����
                direction *= -1;

            }//----- if_stop -----

        }//----- elseif_stop -----

        // �ʒu��␳����
        par_Rb.velocity = Vector3.zero;
        par_Rb.position = finishPos;

        fallPos = par_Obj.transform.localPosition;

        // �ːi��Ԃ���������
        nowRush = false;
        if (par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.Stop();
        }
        par_Main.audio.bossSource.loop = false;

        // �ːi�A�j���[�V�������I��
        par_Anim.SetBool("rushing", false);

        // ���̓���Ɉڍs����
        StopCoroutine(par_Shield.cor_Rotate);
        par_Shield.cor_Rotate = par_Shield.Rotate();
        StartCoroutine(par_Shield.cor_Rotate);
    }
}
