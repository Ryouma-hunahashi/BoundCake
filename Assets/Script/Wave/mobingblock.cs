using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class mobingblock : MonoBehaviour
{
    public float moveX = 0.0f;          //X移動距離
    public float moveY = 0.0f;          //Y移動距離
    public float moveZ = 0.0f;          //Z移動距離
    public float times = 0.0f;          //移動時間
    public float weight = 0.0f;         //停止時間
    [SerializeField,Header("プレイヤーが乗ると動くフラグ")]
    public bool isMoveWhenOn = false;   //乗ったときに動くフラグ
    [SerializeField, Header("勝手に動くフラグ")]
    public bool isCanMove = true;       //動くフラグ
    float perDX;                        //1フレームのXの移動値
    float perDY;                        //1フレームのYの移動値
    float perDZ;                        //1フレームのZの移動値
    Vector3 defPos;                     //初期位置
    bool isReverse = false;             //反転フラグ

    // Start is called before the first frame update
    void Start()
    {
        //初期位置
        defPos = transform.position;
        //1フレームの移動時間取得
        float timestep = Time.fixedDeltaTime;
        // 1フレームのx移動値
        perDX = moveX / (1.0f / timestep * times);
        // 1フレームのy移動値
        perDY = moveY / (1.0f / timestep * times);
        // 1フレームのz移動値
        perDZ = moveZ / (1.0f / timestep * times);


        if (isMoveWhenOn)
        {
            // 乗ったときに動かす為最初は停止する
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
            // 移動中
            float x = transform.position.x;
            float y = transform.position.y;
            float z = transform.position.z;
            bool endX = false;
            bool endY = false;
            bool endZ = false;
            if (isReverse)
            {
                // 逆方向に移動中
                if ((perDX >= 0.0f && x <= defPos.x) || (perDX < 0.0f && x >= defPos.x))
                {
                    endX = true; // X方向の移動終了
                }
                if ((perDY >= 0.0f && y <= defPos.y) || (perDY < 0.0f && y >= defPos.y))
                {
                    endY = true; // Y方向の移動終了
                }
                if ((perDZ >= 0.0f && z <= defPos.z) || (perDZ < 0.0f && z >= defPos.z))
                {
                    endZ = true; // Z方向の移動終了
                }
                // 床を移動させる
                transform.Translate(new Vector3(-perDX, -perDY, -perDZ));
            }
            else
            {
                // 正方向に移動中
                if ((perDX >= 0.0f && x >= defPos.x + moveX) || (perDX < 0.0f && x <= defPos.x + moveX))
                {
                    endX = true; // X方向の移動終了
                }
                if ((perDY >= 0.0f && y >= defPos.y + moveY) || (perDY < 0.0f && y <= defPos.y + moveY))
                {
                    endY = true; // Y方向の移動終了
                }
                if ((perDZ >= 0.0f && z >= defPos.z + moveZ) || (perDZ < 0.0f && z <= defPos.z + moveZ))
                {
                    endZ = true; // Z方向の移動終了
                }
                // 床を移動させる
                Vector3 v = new Vector3(perDX, perDY, perDZ);
                transform.Translate(v);
            }
            if (endX && endY && endZ)
            {
                if (isReverse)
                {
                    // 正方向移送に戻る前に初期位置に戻す　（位置がずれるので）
                    transform.position = defPos;
                }
                isReverse = !isReverse; // フラグを反転させる
                isCanMove = false;      // 移動フラグを下ろす

                if (isMoveWhenOn == false)
                {
                    Invoke("Move", weight); // 移動フラグを立てる遅延実行
                }
            }
        }
    }
    // 移動フラグを立てる
    public void Move()
    {
        isCanMove = true;
    }
    // 移動フラグを下ろす
    public void Stop()
    {
        isCanMove = false;
    }

    // 接触開始
    private void OnCollisionEnter(Collision collision)
    {
        // プレイヤーがついているタグに触れたら
        if (collision.gameObject.CompareTag("Player"))
        {
            // 接触したのがプレイヤーなら移動床の子オブジェクトにする
            collision.transform.SetParent(transform);
            if (isMoveWhenOn)
            {
                // 乗ったときにフラグON
                isCanMove = true;
            }

        }

        //if (collision.gameObject.CompareTag("Player"))
        //{
        //    Invoke("Fall", 2);
        //}
    }

    // 接触終了
    private void OnCollisionExit(Collision collision)
    {
        // プレイヤーがついているタグに触れたら
        if (collision.gameObject.CompareTag("Player"))
        {
            // 接触から外す
            collision.transform.SetParent(null);
        }
    }



}
