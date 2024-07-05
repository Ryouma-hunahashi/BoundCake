using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionArea : MonoBehaviour
{
    // 索敵範囲の大きさ
    [SerializeField] private GameObject setObject;
    [SerializeField] private float detectRange;

    // 検知したか
    private bool isDetect;

    // 検知状態のゲッター
    public bool GetDetection() { return isDetect; }

    private void OnTriggerStay(Collider other)
    {
        // 索敵範囲内にプレイヤーが入ったとき
        if(other.gameObject.CompareTag("Player"))
        {
            // 検知状態を取得する
            isDetect = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // 索敵範囲外にプレイヤーが出た時
        if (other.gameObject.CompareTag("Player"))
        {
            // 検知状態を取得する
            isDetect = false;
        }
    }

    private void Awake()
    {
        // 丸形の当たり判定を作成して格納する
        SphereCollider detectCollision = this.gameObject.AddComponent<SphereCollider>();

        // 当たり判定の設定を変更する
        detectCollision.radius = detectRange;   // 範囲初期化
        detectCollision.isTrigger = true;       // トリガー化
    }

    private void Update()
    {
        // オブジェクトが格納されていなければ処理を抜ける
        if (setObject == null) return;

        Vector3 objPos = setObject.transform.position;

        // このオブジェクトを格納されたオブジェクトに移動
        this.transform.position = new Vector3(objPos.x, objPos.y, 0);
    }
}
