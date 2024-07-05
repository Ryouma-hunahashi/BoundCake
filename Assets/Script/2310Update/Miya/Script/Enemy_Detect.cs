using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Detect : MonoBehaviour
{
    private demo_Enemy_A enemyA;

    [SerializeField]
    private float searchDistance;   // 検知距離

    // 状態用変数
    private bool isDetect;          // 検知状態

    // ゲッター
    public bool GetDetect() { return isDetect; }    // 検知状態

    private void Start()
    {
        enemyA = GetComponent<demo_Enemy_A>();
    }

    private void Update()
    {
        // 自身の位置座標を取得する
        Vector3 myPos = this.transform.position;

        // プレイヤーの位置座標を取得する
        Vector3 plPos = GameObject.Find("player").transform.position;

        // プレイヤーと自身の距離を取得する
        float dist = Vector3.Distance(plPos, transform.position);

        // プレイヤーまでの距離が検知距離内なら
        if (dist < searchDistance)
        {
            // プレイヤーの位置が自分の左なら
            if(myPos.x > plPos.x)
            {
                // 自分の向きを左にする
                enemyA.SetDirection(demo_Enemy_A.DIRECTION.LEFT);
            }
            else if(myPos.x < plPos.x)
            {
                // 自分の向きを右にする
                enemyA.SetDirection(demo_Enemy_A.DIRECTION.RIGHT);
            }

            // 検知状態にする
            isDetect = true;

        }//----- if_stop -----
        else
        {
            // 非検知状態にする
            isDetect = false;

        }//----- else_stop -----
    }

}
