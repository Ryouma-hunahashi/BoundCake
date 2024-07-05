using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnemyMove : MonoBehaviour
{
    [Header("�ړ����x")] public float speed;
    [SerializeField, Header("�傫���w��")]
    private float EnemyScale;   // Enemy�̑傫�����w�肷��
    

    [SerializeField,TagField]private string[] TagChanger;  // �^�O����͂���
    private Vector3 pos = new Vector3(1, 0, 0); // enemy��Vector��ۑ�
    private Vector3 defpos;
    public int moveindex = 1; // ���E�̐؂�ւ��p����
    [SerializeField,Header("���ɉ��}�X�i�ނ�")]
    public float LeftEnd = 0;  // ���[�̌��E
    [SerializeField,Header("�E�ɉ��}�X�i�ނ�")]
    public float RightEnd = 10; // �E�[�̌��E

    private Animator anim;
    private Enemy_KB knockBackScript;
    private EnemyAudio audio;

    private bool damageLog = false;

    //===================================================
    public float const1=0.1f;
    public int const2=0;
    private Vector3 prvPos=new Vector3(0,0,0);
    private float R=0;
    private int testCnt = 0;

    // Start is called before the first frame update
    void Start()
    {
        defpos= this.transform.position; // ���݂̈ʒu���擾
        // �T�C�Y�w��
        transform.localScale = new Vector3(EnemyScale, EnemyScale, EnemyScale);
        
        anim = GetComponent<Animator>();
        if(anim == null)
        {
            Debug.LogError("Animator��������܂���");
        }
        knockBackScript = GetComponent<Enemy_KB>();
        if(knockBackScript == null)
        {
            Debug.LogError("Enemy_KB��������܂���");
        }
        audio = GetComponent<EnemyAudio>();
        if(audio == null)
        {
            Debug.LogError("EnemyAudio��������܂���");
        }
        damageLog = knockBackScript.hitPlayerWave;

        
    }

    // Update is called once per frame
    void Update()
    {
        pos = this.transform.position; // trasform��ۑ�

        if (!knockBackScript.hitPlayerWave)
        {
            // trasform���ړ������Ă�
            transform.Translate(transform.right * Time.deltaTime * speed * moveindex);
            transform.localScale = new Vector3(moveindex * EnemyScale, EnemyScale, 1);

            anim.SetBool("running", true);
            if(!audio.enemySource.isPlaying)
            {
                audio.WalkSound();
            }
        }
        if (damageLog != knockBackScript.hitPlayerWave)
        {
            if (knockBackScript.hitPlayerWave)
            {
                
                audio.enemySource.Stop();
                audio.KnockBackSound();
            }
            anim.SetBool("damaging", knockBackScript.hitPlayerWave);
            damageLog = knockBackScript.hitPlayerWave;

        }
        // x���W��RightEnd�𒴂�����
        if (pos.x > (pos.x+RightEnd))
        {
            moveindex = -1;
            
        }
        if (pos.x < pos.x-LeftEnd)
        {
            moveindex = 1;
      
        }
        prvPos = this.transform.position;
        R=Mathf.Pow(pos.x-prvPos.x, 2)+Mathf.Pow(pos.y-prvPos.y,2);
        if(R < const1)
        {
            testCnt++;
            if(testCnt>const2)
            {
                testCnt=0;
                moveindex *= -1;
            }
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        // �I�������^�O�ɂ���������
        for (int i = 0; i < TagChanger.Length; i++)
        {
            if (other.gameObject.tag == TagChanger[i])
            {
                // ���]����
                if (moveindex == 1)
                {
                    moveindex = -1;
                }
                else
                {
                    moveindex = 1;
                }
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        // �I�������^�O�ɂ���������
        for (int i = 0; i < TagChanger.Length; i++)
        {
            if (other.gameObject.tag == TagChanger[i])
            {
                // ���]����
                if (moveindex == 1)
                {
                    moveindex = -1;
                }
                else
                {
                    moveindex = 1;
                }
            }
        }
    }
}
