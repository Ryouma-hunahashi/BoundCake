using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AItest : MonoBehaviour
{
    [Header("�ړ����x")] public float speed;

    private Vector3 pos = new Vector3(1,0,0); // enemy��Vector��ۑ�
    public int num = 1; // ���E�̐؂�ւ��p����
    public float LeftEnd = 0;  // ���[�̌��E
    public float RightEnd = 10; // �E�[�̌��E
   
   

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        pos = transform.position; // trasform��ۑ�
         
        // trasform���ړ������Ă�
        transform.Translate(transform.right * Time.deltaTime * speed * num);

        // x���W��RightEnd�𒴂�����
        if (pos.x > RightEnd)
        {
            num = -1;
            // �T�C�Y�w��
            transform.localScale = new Vector3((float)1.5, (float)1.5, (float)1.5);
        }
        if (pos.x < LeftEnd)
        {
            num = 1;
            // �T�C�Y�w��
            transform.localScale = new Vector3((float)-1.5, (float)1.5, (float)1.5);
        }

    }

    //private void FixedUpdate()
    //{
    //    if (mr.isVisible || nonVisibleAct)
    //    {
    //        int xVector = -1;
    //        if (rightreturn)
    //        {
    //            xVector = 1;

    //            transform.localScale = new Vector3(-(float)size, (float)1.5, (float)1.5);

    //            if (pos.x > RightEnd)
    //            {
    //                rightreturn = true;
    //            }

    //        }
    //        else
    //        {
    //            transform.localScale = new Vector3((float)1.5, (float)1.5, (float)1.5);
    //            if (pos.x < LeftEnd)
    //            {
    //                rightreturn = false;
    //            }
    //        }
    //        rb.velocity = new Vector3(xVector * speed, -gravity, 0);
    //    }
    //    else
    //    {
    //        rb.Sleep();
    //    }


    //}
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "WaveEnd")
        {
            if (num == 1)
            {
                num = -1;
            }
            else
            {
                num = 1;
            }
        }
    }
    //private void OnTriggerEnter(Collider other)
    //{
    //    if(other.gameObject.tag=="WaveEnd")
    //    {
    //        if(num==1)
    //        {
    //            num = -1;
    //        }
    //        else
    //        {
    //            num = 1;
    //        }
    //    }
    //}
}
