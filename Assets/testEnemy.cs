using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//======================================
//�֐��̓��e�F�G���n�ʂƐڐG�����Ƃ���Y���W���Œ肳����
//�����F�Ȃ�
//�߂�l�F�Ȃ�
//�쐬�ҁF����
//�쐬���F4/16
//======================================

public class testEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    //�G�ƒn�ʂ̐ڐG��ԁB�O�Ȃ�G��Ă��Ȃ�0�ȊO�Ȃ�ڐG���Ă���
    public int touchGround = 0;
    [Header("�n�ʂ̃^�O��")]
    [SerializeField] LayerMask groundLayer = 1<<6;
    //���g��Rigidbody�i�[�p
    private Rigidbody rb;
    void Start()
    {
        //������
        touchGround = 0;
        //���g��Rigidbody���i�[����
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}


    private void OnCollisionEnter(Collision collision)
    {
        //�n�ʂƐڐG�����Ƃ��A���A1�x�ڂ̐ڐG�Ȃ�Ώ������s
        if(1<<collision.gameObject.layer == groundLayer&&touchGround==0)
        {
            //�ڐG�t���O��ύX����
            touchGround = 1;
            //�ڐG�����n�ʂ̃X�N���v�g�̃��X�g�Ɏ��g��ǉ�����
            //collision.gameObject.GetComponent<LandingGround>().EnemyObjList.Add(this.gameObject);
            
            //Y���W���Œ肷��O�Ɉʒu�𒲐�����(���������K�v�Ȃ�ǋL����)

            //�S�Ẳ�]��Y�EZ���W���Œ肷��
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            
        }
    }
}
