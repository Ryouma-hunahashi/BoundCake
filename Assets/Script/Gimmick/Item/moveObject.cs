using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveObject : MonoBehaviour
{
    [SerializeField] GameObject target;        // �e�̃Q�[���I�u�W�F�N�g�擾
    private ItemDeta item;                      // �ʂ̃X�N���v�g���Q��
    public float deceleration;                 // �����l
    [SerializeField, Header("�ړ����� 1�`7 �����ɉ����Đ��l������")]
    public float distance;                    // �ړ����鋗��( 1�`7 �̐��l������)
    public bool Hitflg = false;                 // �g�������������ǂ����𔻒肷��t���O true�̎��̓A�C�e�������Ȃ�
    public bool fallflg = false;                // �����������肷��t���O
    private float defdistance = 0.0f;           // �ړ����鐔�l��ۑ�����
    private Vector3 defPos;                      // �ŏ��̏ꏊ��ێ�������W
    private Vector3 ParentPos;                   // �ŏ��̏ꏊ��ێ�������W
    public float weight = 0.0f;                 // ��őҋ@���鎞��
    private float up = 0.1f;


    [SerializeField] private LayerMask groundLayer = 1 << 6;    // �O���E���h���C���[��6�Ԗڂ̃��C���[�Ƃ��ď�����

    private GameObject test_obj;
    private Vector3 nowPos;
    // Start is called before the first frame update
    void Start()
    {
        // ���݂̈ʒu��ۑ�����
        defPos = transform.position;
        ParentPos = target.transform.position;
        // ItemDeta�X�N���v�g���擾
        item = GetComponent<ItemDeta>();
        // �Q�_�Ԃ̋��������߂�
        distance = Vector3.Distance(this.transform.position, target.transform.position);
        // ���̃X�s�[�h��ʕϐ��ɕۑ�����
        defdistance = distance;
        // ���Y����l�����߂�( 2�_�Ԃ̋��� / 2 )
        deceleration = distance / 2;
        //distance = distance * 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // �g�����������Ƃ�
        if (Hitflg == true)
        {
            // �ړI�n�ɓ���������
            if (this.transform.position == target.transform.position)
            {
                // ��������Ƃ��p�ɐ��l��0�ɂ���
                if (distance > 0 || distance < 0)
                {
                    distance = 0;
                }
                // ���������Ƃ��̃t���O��false�ɂ���
                Hitflg = false;

                // �ҋ@����
                Invoke("Move", weight);
            }
            // �e�I�u�W�F�N�g�Ɉړ�����
            transform.position = Vector3.MoveTowards
                (this.transform.position, target.transform.position, distance * 2 * Time.deltaTime);
            // �����l���t���[���Ɗ|���Č��Z����
            distance -= Time.deltaTime * deceleration * 2;
            nowPos = this.transform.position;
            // �㏸���ł��A�C�e��������悤�ɂ���
            item.getFg = true;
        }
        // ������(���̏ꏊ�ɖ߂�)�Ƃ�
        else if (Hitflg == false && fallflg == true)
        {
            // �������ꏊ�Ɉړ�����
            transform.position = Vector3.MoveTowards
                (this.transform.position, defPos, distance * Time.deltaTime);
            // �����l���t���[���Ɗ|���Č��Z����
            distance += Time.deltaTime * distance + 0.1f/*(deceleration + up)*/;
            nowPos = this.transform.position;
            // �������ł��A�C�e��������悤�ɂ���
            item.getFg = true;
            // ���̏ꏊ�ɖ߂����Ƃ�
            if (this.transform.position.y <= defPos.y)
            {
                // ���̃X�s�[�h�ɖ߂�
                distance = defdistance;
                // �������t���O��false�ɂ���
                fallflg = false;
            }
        }
    }
    private void Move()
    {
        // ���̏ꏊ�ɖ߂�
        Hitflg = false;
        fallflg = true;
        distance = 0;

    }
    private void OnTriggerEnter(Collider other)
    {
        // �n�ʂɐڐG���Ă����
        if(1 << other.gameObject.layer == groundLayer&&transform.position == defPos)
        {
           // Debug.Log(gameObject.transform.GetInstanceID);
            // �A�C�e�����擾�ł��Ȃ��悤�ɂ���
            //item.getFg = false;
        }
        // Wave�^�O�ɓ���������
        else if (other.gameObject.tag == "Wave" && Hitflg == false && !fallflg)
        {
            // �q�b�g�t���O��true
            Hitflg = true;
        }
    }
}
