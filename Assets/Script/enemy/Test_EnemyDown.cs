using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// ※このスクリプトはフレームレート60前後を想定して作っています
// 　使用する際はフレームを60FPSで固定するか、
// 　スクリプト内部を調整してから使用してください
//==================================================
// 作成日2023/03/23
// 宮﨑
public class Test_EnemyDown : MonoBehaviour
{
    // 初期取得情報
    private Rigidbody rb;           // 自身の[Rigidbody]を取得
    private BoxCollider boxCol;     // 自身の[BoxCollider]を取得

    // ダメージ判定
    [Header("----- 接触したときの情報 -----"),Space(5)]
    [Tooltip("プレイヤーの振動に触れたなら")]
    [SerializeField] private bool hitPlayerWave;        // 振動に触れた判定
    [Tooltip("触れた振動の向きを取得")]
    [SerializeField] private Vector3 searchDirection;   // 振動の向きを取得

    [Header("----- 飛行距離の設定 -----"),Space(5)]
    [Tooltip("飛ばす距離の設定")]
    [SerializeField] private Vector3 blowOfDistance;        // 飛行距離の設定
    [Tooltip("上へ飛ばす距離の最大値")]
    [SerializeField] private float blowOfDistanceLimitY;    // 上昇制限の設定

    [Header("----- 消滅時間の設定 -----"),Space(5)]
    [Tooltip("消滅までの時間の設定")]
    [SerializeField] private float destroyFrame;        // オブジェクト消滅までの時間
    [Tooltip("計測中の状態")]
    [SerializeField] private bool nowDestroyCount;      // 消滅までの時間計測中の判定

    [Header("----- 状態確認用 -----"),Space(5),Header("落下状態")]
    [Tooltip("落下状態")]
    [SerializeField] private bool nowFall;      // 落下中の判定

    private IEnumerator TimeToDestroy()
    {
        // 計測を開始する
        nowDestroyCount = true;

        for(int i = 0; i < destroyFrame; i++)
        {//----- for_start -----

            yield return null;

        }//----- for_stop -----

        // 計測終了後このオブジェクトを破壊する
        Destroy(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        // プレイヤー固有というのもできれば追加してください -----
        // [Wave]タグのオブジェクトに触れたなら
        if (other.gameObject.tag == "Wave")
        {//----- if_start -----

            // 触れた相手の向きを取得
            searchDirection = other.gameObject.transform.localScale;
            hitPlayerWave = true;

        }//----- if_stop -----
    }

    private void Start()
    {
        // 自身の[Rigidbody]を取得する
        rb = this.GetComponent<Rigidbody>();

        // [Rigidbody]が取得できていなかった場合
        if(rb == null)
        {//----- if_start -----

            // エラーログを表示させる
            Debug.LogError("[Rigidbody]が取得できていません！");

        }//----- if_stop -----

        // 自身の[BoxCollider]を取得する
        boxCol = this.GetComponent<BoxCollider>();

        // [BoxCollider]が取得できていなかった場合
        if (boxCol == null)
        {//----- if_start -----

            // エラーログを表示させる
            Debug.LogError("[BoxCollider]が取得できていません！");

        }//----- if_stop -----
    }

    private void Update()
    {
        // プレイヤーの振動に触れたなら
        if(hitPlayerWave)
        {//----- if_start -----

            // 倒されたときの動きを開始する

            WasKnockedDown();

        }//----- if_stop -----
    }

    //==================================================
    //      倒されたときの処理
    // ※振動に当たった後に自身が消滅するまでの処理です
    //==================================================
    // 制作日   2023/03/23
    // 宮﨑
    private void WasKnockedDown()
    {
        // 自身を吹き飛ばす
        rb.velocity = new Vector3(blowOfDistance.x * searchDirection.x, blowOfDistance.y, blowOfDistance.z);

        // 地面をすり抜けさせるために判定をトリガーに変更
        boxCol.isTrigger = true;

        // 落下中でなければ
        if(!nowFall)
        {//----- if_start -----

            // 上昇値を加速させる
            blowOfDistance.y++;

        }//----- if_stop -----
        // 上昇値が制限を超えた時　または
        // 上昇後の落下中の場合
        if((blowOfDistanceLimitY < blowOfDistance.y) || nowFall)
        {//----- if_start -----

            // 落下状態にする
            //nowFall = true;

            // 落下速度を上昇させていく
            blowOfDistance.y--;

        }//----- if_stop -----

        // 消滅までの計測中でなければ
        if(!nowDestroyCount)
        {//----- if_start -----

            // 消滅までの時間を計測し始める
            StartCoroutine(TimeToDestroy());

        }//----- if_stop -----
    }
}