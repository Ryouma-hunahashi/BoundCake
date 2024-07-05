using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 対応日2023/04/11
// 宮﨑
public class Enemy_A_Rush : MonoBehaviour
{
    [SerializeField]
    private GameObject myParent;    // 自分の親を取得

    // 自身の情報を取得
    private Rigidbody rb;
    [SerializeField]
    private Vector3 parentScale;    // 親の大きさ
    private IEnumerator rushAct;

    public bool rushNow;
    public bool rushEnd;

    // 発動位置を取得
    [Tooltip("突進の開始地点を保持する")]
    [SerializeField] private Vector3 startPosition;      // 突進開始地点を保持
    [Tooltip("突進の終了地点を予測する")]
    [SerializeField] private Vector3 stopPosition;       // 突進終了地点を予測

    // 座標を取得したかどうか
    private bool positionSaved;

    [Tooltip("移動速度の設定")]
    [SerializeField] private float rushSpeed;           // 速度の指定
    [Tooltip("移動距離の設定")]
    [SerializeField] private float rushDirection;       // 距離の指定
    [Tooltip("加速度の設定"), Range(0f, 1f)]
    [SerializeField] private float rushAcceleration;    // 加速の指定
    [Tooltip("減速度の設定"), Range(0f, 1f)]
    [SerializeField] private float rushDiceleration;    // 減速の指定

    //==================================================
    //      突進攻撃
    // ※敵キャラクターの突進攻撃用の関数
    // ※別関数から呼び出して使用してください
    //==================================================
    // 制作日2023/04/03    更新日2023/04/08
    // 宮﨑
    public IEnumerator RushAction()
    {
        rushNow = true;
        rushEnd = false;

        if(!positionSaved)
        {//----- if_start -----

            // 突進開始時にその座標を取得しておく
            startPosition = myParent.transform.position;

            // 突進の終了地点を予測
            stopPosition = myParent.transform.position;
            stopPosition.x += rushDirection;

            // 座標の保持状況を'済'にする
            positionSaved = true;

        }//----- if_stop -----

        // 移動方向が右側[X+]方向なら
        if(rushDirection > 0)
        {//----- if_start-----

            while (stopPosition.x > myParent.transform.position.x)
            {//----- while_start -----

                yield return null;

                // 速度を更新する
                rb.velocity = new Vector3(rushSpeed, 0, 0);

            }//----- while_stop -----

            // 向きを補正する
            // 自身が右を向いているのなら
            if (myParent.transform.localScale.x > 0)
            {
                // 自身を左方向へ向かせる
                myParent.transform.localScale = new Vector3(-parentScale.x, parentScale.y, parentScale.z);
                Debug.Log("左にむかせました！");


                // 移動方向を変更する
                rushDirection *= -1;

            }//----- if_stop -----
        }//----- if_stop -----
        // 移動方向が左側[X-]方向なら
        else if (rushDirection < 0)
        {//----- if_start-----

            while (stopPosition.x < myParent.transform.position.x)
            {//----- while_start -----

                yield return null;

                // 速度を更新する
                rb.velocity = new Vector3(-rushSpeed, 0, 0);

            }//----- while_stop -----

            // 向きを修正する
            // 自身が左を向いているのなら
            if (myParent.transform.localScale.x < 0)
            {
                // 自身を右方向へ向かせる
                myParent.transform.localScale = new Vector3(parentScale.x, parentScale.y, parentScale.z);
                Debug.Log("右にむかせました！");

                // 移動方向を変更する
                rushDirection *= -1;

            }//----- if_stop -----
        }//----- else if_stop -----

        // 突進終了地点よりも現在位置の値が低いとき

        // 位置を補正する
        rb.velocity = Vector3.zero;
        rb.position = stopPosition;

        rushNow = false;
        rushEnd = true;

        positionSaved = false;

    }

    private void Start()
    {
        // 自身の名前を"rush"にする
        this.gameObject.name = "rush";

        // 自分の親を取得
        myParent = transform.parent.gameObject;

        // 親の大きさを取得
        parentScale = myParent.transform.localScale;
        if(parentScale.x < 0)
        {
            parentScale.x *= -1;
        }

        // [Rigidbody]の取得
        rb = myParent.GetComponent<Rigidbody>();

        // うまく取得できなかった場合
        if (rb == null)
        {
            Debug.LogError("[Rigidbody]が見つかりません！");

        }//----- if_stop -----
    }
}
