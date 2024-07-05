using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// すり抜け処理
//==================================================
// 作成日2023/05/01
// 宮﨑
public class Player_Ride : MonoBehaviour
{
    // 親(プレイヤー)情報を取得する
    private Rigidbody parentRb;
    private Player_Main parentScript;

    // 親の子にあるスクリプトを取得
    private Player_Grab pl_Grab;    // 掴み用スクリプト
    private Player_Jump pl_Jump;    // 跳躍用スクリプト
    private Player_Fall pl_Fall;    // 落下用スクリプト
    private Player_Wave pl_Wave;    // 波発生スクリプト

    private void Start()
    {
        // 親の情報を取得
        parentRb = transform.parent.GetComponent<Rigidbody>();
        parentScript = transform.parent.GetComponent<Player_Main>();

        // 親の子にある情報を取得
        pl_Grab = transform.parent.GetComponentInChildren<Player_Grab>();
        pl_Jump = transform.parent.GetComponentInChildren<Player_Jump>();
        pl_Fall = transform.parent.GetComponentInChildren<Player_Fall>();
    }

    private void FixedUpdate()
    {
        if(parentScript.inRayRide)
        {
            parentScript.pl_Col.isTrigger = true;

        }//----- if_stop -----
        else if(!parentScript.inRayRide && (parentRb.velocity.y < 0 && parentScript.onRayGround))
        {
            parentScript.pl_Col.isTrigger = false;

        }//----- elseif_stop -----
    }
}
