using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//======================================
//関数の内容：敵が地面と接触したときにY座標を固定させる
//引数：なし
//戻り値：なし
//作成者：中村
//作成日：4/16
//======================================

public class testEnemy : MonoBehaviour
{
    // Start is called before the first frame update
    //敵と地面の接触状態。０なら触れていない0以外なら接触している
    public int touchGround = 0;
    [Header("地面のタグ名")]
    [SerializeField] LayerMask groundLayer = 1<<6;
    //自身のRigidbody格納用
    private Rigidbody rb;
    void Start()
    {
        //初期化
        touchGround = 0;
        //自身のRigidbodyを格納する
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}


    private void OnCollisionEnter(Collision collision)
    {
        //地面と接触したとき、かつ、1度目の接触ならば処理実行
        if(1<<collision.gameObject.layer == groundLayer&&touchGround==0)
        {
            //接触フラグを変更する
            touchGround = 1;
            //接触した地面のスクリプトのリストに自身を追加する
            //collision.gameObject.GetComponent<LandingGround>().EnemyObjList.Add(this.gameObject);
            
            //Y座標を固定する前に位置を調整する(微調整が必要なら追記する)

            //全ての回転とY・Z座標を固定する
            rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;
            
        }
    }
}
