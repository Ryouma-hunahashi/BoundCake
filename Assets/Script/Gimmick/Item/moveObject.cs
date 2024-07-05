using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class moveObject : MonoBehaviour
{
    [SerializeField] GameObject target;        // 親のゲームオブジェクト取得
    private ItemDeta item;                      // 別のスクリプトを参照
    public float deceleration;                 // 減速値
    [SerializeField, Header("移動距離 1～7 距離に応じて数値が入る")]
    public float distance;                    // 移動する距離( 1～7 の数値を入れる)
    public bool Hitflg = false;                 // 波が当たったかどうかを判定するフラグ trueの時はアイテムが取れない
    public bool fallflg = false;                // 落下中か判定するフラグ
    private float defdistance = 0.0f;           // 移動する数値を保存する
    private Vector3 defPos;                      // 最初の場所を保持する座標
    private Vector3 ParentPos;                   // 最初の場所を保持する座標
    public float weight = 0.0f;                 // 上で待機する時間
    private float up = 0.1f;


    [SerializeField] private LayerMask groundLayer = 1 << 6;    // グラウンドレイヤーを6番目のレイヤーとして初期化

    private GameObject test_obj;
    private Vector3 nowPos;
    // Start is called before the first frame update
    void Start()
    {
        // 現在の位置を保存する
        defPos = transform.position;
        ParentPos = target.transform.position;
        // ItemDetaスクリプトを取得
        item = GetComponent<ItemDeta>();
        // ２点間の距離を求める
        distance = Vector3.Distance(this.transform.position, target.transform.position);
        // 今のスピードを別変数に保存する
        defdistance = distance;
        // 減産する値を求める( 2点間の距離 / 2 )
        deceleration = distance / 2;
        //distance = distance * 2;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 波が当たったとき
        if (Hitflg == true)
        {
            // 目的地に到着した時
            if (this.transform.position == target.transform.position)
            {
                // 落下するとき用に数値を0にする
                if (distance > 0 || distance < 0)
                {
                    distance = 0;
                }
                // あたったときのフラグをfalseにする
                Hitflg = false;

                // 待機時間
                Invoke("Move", weight);
            }
            // 親オブジェクトに移動する
            transform.position = Vector3.MoveTowards
                (this.transform.position, target.transform.position, distance * 2 * Time.deltaTime);
            // 減速値をフレームと掛けて減算する
            distance -= Time.deltaTime * deceleration * 2;
            nowPos = this.transform.position;
            // 上昇中でもアイテムを取れるようにする
            item.getFg = true;
        }
        // 落下中(元の場所に戻る)とき
        else if (Hitflg == false && fallflg == true)
        {
            // 元居た場所に移動する
            transform.position = Vector3.MoveTowards
                (this.transform.position, defPos, distance * Time.deltaTime);
            // 減速値をフレームと掛けて減算する
            distance += Time.deltaTime * distance + 0.1f/*(deceleration + up)*/;
            nowPos = this.transform.position;
            // 落下中でもアイテムを取れるようにする
            item.getFg = true;
            // 元の場所に戻ったとき
            if (this.transform.position.y <= defPos.y)
            {
                // 元のスピードに戻す
                distance = defdistance;
                // 落下中フラグをfalseにする
                fallflg = false;
            }
        }
    }
    private void Move()
    {
        // 元の場所に戻る
        Hitflg = false;
        fallflg = true;
        distance = 0;

    }
    private void OnTriggerEnter(Collider other)
    {
        // 地面に接触していれば
        if(1 << other.gameObject.layer == groundLayer&&transform.position == defPos)
        {
           // Debug.Log(gameObject.transform.GetInstanceID);
            // アイテムを取得できないようにする
            //item.getFg = false;
        }
        // Waveタグに当たったら
        else if (other.gameObject.tag == "Wave" && Hitflg == false && !fallflg)
        {
            // ヒットフラグをtrue
            Hitflg = true;
        }
    }
}
