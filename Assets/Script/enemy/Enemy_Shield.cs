using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �G���u�̏��h��
//==================================================
// �쐬��2023/05/19
// �{��
public class Enemy_Shield : MonoBehaviour
{
    // �e�̏����擾
    private GameObject par_Obj;
    private Vector3 par_Scale;

    [Header("----- ���̐ݒ� -----")]
    private CapsuleCollider shieldCol;      // ���̓����蔻��
    public bool nowBreak;     // �j����
    public sbyte durability;  // �ϋv�l
    public bool unbreakable;
    [SerializeField] private byte unbreakableFrame; // �j��s����

    Animator enemyAinim;

    private void OnTriggerEnter(Collider other)
    {
        // "Wave"����̕t�����I�u�W�F�N�g���G�ꂽ��
        if(other.CompareTag("Wave"))
        {
            // �������݂��Ă���Ƃ�
            // �ϋv�l�����炷���Ƃ��ł���Ȃ�
            if (!nowBreak && !unbreakable)
            {
                durability--;

                // �A���Ń_���[�W��^�����Ȃ��悤�ɂ���
                unbreakable = true;

                // ���G���Ԃ�݂���
                StartCoroutine(ShieldUnbreakable());
            }
        }
        enemyAinim = transform.parent.GetComponent<Animator>();
        if(enemyAinim == null)
        {
            enemyAinim = transform.parent.parent.GetComponent<Animator>();
            if(enemyAinim == null)
            {
                Debug.LogError("Animator������܂���");
            }
        }
    }

    private void Start()
    {
        // �e�̏����擾
        par_Obj = transform.parent.gameObject;
        par_Scale = par_Obj.transform.localScale;

        // ���̓����蔻����擾
        shieldCol = this.GetComponent<CapsuleCollider>();

        // ����L��������
        shieldCol.enabled = true;
    }

    private void FixedUpdate()
    {
        // �ϋv�l���O�ȉ��ɂȂ����Ȃ�
        if((durability <= 0) && !nowBreak)
        {
            // �����j�󂳂��
            nowBreak = true;
            enemyAinim.SetBool("shieldBreak", true);
            StartCoroutine(ShieldUnbreakable());

        }//----- if_stop -----
    }

    private IEnumerator ShieldUnbreakable()
    {
        // �����j�󂳂�Ă���Ȃ�
        if (nowBreak)
        {
            // �����蔻������ł�����
            shieldCol.enabled = false;

        }//----- if_stop -----

        // �ݒ肳�ꂽ���G���Ԃ̊Ԃ��肩����
        for (int i = 0; i < unbreakableFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----

        // ���G���Ԃ𔲂���̂ŉ���
        unbreakable = false;
    }
}