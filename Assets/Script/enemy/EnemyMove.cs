using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class EnemyMove : MonoBehaviour
{
    [Header("移動速度")] public float speed;
    [SerializeField, Header("大きさ指定")]
    private float EnemyScale;   // Enemyの大きさを指定する
    

    [SerializeField,TagField]private string[] TagChanger;  // タグを入力する
    private Vector3 pos = new Vector3(1, 0, 0); // enemyのVectorを保存
    private Vector3 defpos;
    public int moveindex = 1; // 左右の切り替え用数字
    [SerializeField,Header("左に何マス進むか")]
    public float LeftEnd = 0;  // 左端の限界
    [SerializeField,Header("右に何マス進むか")]
    public float RightEnd = 10; // 右端の限界

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
        defpos= this.transform.position; // 現在の位置を取得
        // サイズ指定
        transform.localScale = new Vector3(EnemyScale, EnemyScale, EnemyScale);
        
        anim = GetComponent<Animator>();
        if(anim == null)
        {
            Debug.LogError("Animatorが見つかりません");
        }
        knockBackScript = GetComponent<Enemy_KB>();
        if(knockBackScript == null)
        {
            Debug.LogError("Enemy_KBが見つかりません");
        }
        audio = GetComponent<EnemyAudio>();
        if(audio == null)
        {
            Debug.LogError("EnemyAudioが見つかりません");
        }
        damageLog = knockBackScript.hitPlayerWave;

        
    }

    // Update is called once per frame
    void Update()
    {
        pos = this.transform.position; // trasformを保存

        if (!knockBackScript.hitPlayerWave)
        {
            // trasformを移動させてる
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
        // x座標がRightEndを超えたら
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
        // 選択したタグにあたったら
        for (int i = 0; i < TagChanger.Length; i++)
        {
            if (other.gameObject.tag == TagChanger[i])
            {
                // 反転する
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
        // 選択したタグにあたったら
        for (int i = 0; i < TagChanger.Length; i++)
        {
            if (other.gameObject.tag == TagChanger[i])
            {
                // 反転する
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
