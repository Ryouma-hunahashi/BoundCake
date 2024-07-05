using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobingblock : MonoBehaviour
{
    public float moveX = 0.0f;          //X�ړ�����
    public float moveY = 0.0f;          //Y�ړ�����
    public float moveZ = 0.0f;          //Z�ړ�����
    public float times = 0.0f;          //�ړ�����
    public float weight = 0.0f;         //��~����
    [SerializeField,Header("�v���C���[�����Ɠ����t���O")]
    public bool isMoveWhenOn = false;   //������Ƃ��ɓ����t���O
    [SerializeField, Header("����ɓ����t���O")]
    public bool isCanMove = true;       //�����t���O
    float perDX;                        //1�t���[����X�̈ړ��l
    float perDY;                        //1�t���[����Y�̈ړ��l
    float perDZ;                        //1�t���[����Z�̈ړ��l
    Vector3 defPos;                     //�����ʒu
    bool isReverse = false;             //���]�t���O

    // Start is called before the first frame update
    void Start()
    {
        //�����ʒu
        defPos = transform.position;
        //1�t���[���̈ړ����Ԏ擾
        float timestep = Time.fixedDeltaTime;
        // 1�t���[����x�ړ��l
        perDX = moveX / (1.0f / timestep * times);
        // 1�t���[����y�ړ��l
        perDY = moveY / (1.0f / timestep * times);
        // 1�t���[����z�ړ��l
        perDZ = moveZ / (1.0f / timestep * times);


        if (isMoveWhenOn)
        {
            // ������Ƃ��ɓ������׍ŏ��͒�~����
            isCanMove = false;
        }
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void FixedUpdate()
    {
        if (isCanMove)
        {
            // �ړ���
            float x = transform.position.x;
            float y = transform.position.y;
            float z = transform.position.z;
            bool endX = false;
            bool endY = false;
            bool endZ = false;
            if (isReverse)
            {
                // �t�����Ɉړ���
                if ((perDX >= 0.0f && x <= defPos.x) || (perDX < 0.0f && x >= defPos.x))
                {
                    endX = true; // X�����̈ړ��I��
                }
                if ((perDY >= 0.0f && y <= defPos.y) || (perDY < 0.0f && y >= defPos.y))
                {
                    endY = true; // Y�����̈ړ��I��
                }
                if ((perDZ >= 0.0f && z <= defPos.z) || (perDZ < 0.0f && z >= defPos.z))
                {
                    endZ = true; // Z�����̈ړ��I��
                }
                // �����ړ�������
                transform.Translate(new Vector3(-perDX, -perDY, -perDZ));
            }
            else
            {
                // �������Ɉړ���
                if ((perDX >= 0.0f && x >= defPos.x + moveX) || (perDX < 0.0f && x <= defPos.x + moveX))
                {
                    endX = true; // X�����̈ړ��I��
                }
                if ((perDY >= 0.0f && y >= defPos.y + moveY) || (perDY < 0.0f && y <= defPos.y + moveY))
                {
                    endY = true; // Y�����̈ړ��I��
                }
                if ((perDZ >= 0.0f && z >= defPos.z + moveZ) || (perDZ < 0.0f && z <= defPos.z + moveZ))
                {
                    endZ = true; // Z�����̈ړ��I��
                }
                // �����ړ�������
                Vector3 v = new Vector3(perDX, perDY, perDZ);
                transform.Translate(v);
            }
            if (endX && endY && endZ)
            {
                if (isReverse)
                {
                    // �������ڑ��ɖ߂�O�ɏ����ʒu�ɖ߂��@�i�ʒu�������̂Łj
                    transform.position = defPos;
                }
                isReverse = !isReverse; // �t���O�𔽓]������
                isCanMove = false;      // �ړ��t���O�����낷

                if (isMoveWhenOn == false)
                {
                    Invoke("Move", weight); // �ړ��t���O�𗧂Ă�x�����s
                }
            }
        }
    }
    // �ړ��t���O�𗧂Ă�
    public void Move()
    {
        isCanMove = true;
    }
    // �ړ��t���O�����낷
    public void Stop()
    {
        isCanMove = false;
    }

    // �ڐG�J�n
    private void OnCollisionEnter(Collision collision)
    {
        // �v���C���[�����Ă���^�O�ɐG�ꂽ��
        if (collision.gameObject.CompareTag("Player"))
        {
            // �ڐG�����̂��v���C���[�Ȃ�ړ����̎q�I�u�W�F�N�g�ɂ���
            collision.transform.SetParent(transform);
            if (isMoveWhenOn)
            {
                // ������Ƃ��Ƀt���OON
                isCanMove = true;
            }

        }

        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    Invoke("Fall", 2);
        //}
    }

    // �ڐG�I��
    private void OnCollisionExit(Collision collision)
    {
        // �v���C���[�����Ă���^�O�ɐG�ꂽ��
        if (collision.gameObject.CompareTag("Player"))
        {
            // �ڐG����O��
            collision.transform.SetParent(null);
        }
    }



}
