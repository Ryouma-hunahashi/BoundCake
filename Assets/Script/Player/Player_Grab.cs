using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// 掴み処理
//==================================================
// 作成日2023/05/02    更新日2023/05/04
// 宮﨑
public class Player_Grab : MonoBehaviour
{
    // 親(プレイヤー)情報を取得
    private Rigidbody parentRb;
    private Player_Main parentScript;

    // 親の子にあるスクリプトを取得
    private Player_Fall pl_Fall;

    // ジャンプチャージの設定 ----------
    public bool chargeNow;          // チャージ中
    public byte chargeStartPoint;   // チャージ開始時の量
    public byte chargePoint;        // 現在チャージ量
    public byte chargeLimit = 20;   // チャージ量上限

    public float chargeLandDis = 0.25f;
    public float chargeLandSpd = 10f;
    // ジャンプ時の設定 ----------

    private void Start()
    {
        // 親の情報を取得
        parentRb = transform.parent.GetComponent<Rigidbody>();
        parentScript = transform.parent.GetComponent<Player_Main>();

        

        // 親の子にある情報を取得
        pl_Fall = transform.parent.GetComponentInChildren<Player_Fall>();
    }

    //==================================================
    //          ジャンプチャージ実行
    // ※ボタン押し込み時間によってジャンプ力が変化
    // ※ジャンプ待機のアニメーションはここに書く
    // 戻り値 : なし
    //  引数  : なし
    //==================================================
    // 作成日2023/05/02    更新日2023/05/04
    // 宮﨑
    public void ChargePower()
    {
        // ポイントが上限を超えていなければ
        if (chargePoint < chargeLimit)
        {
            // ぐらっぶチャージあり時のアニメーション
            //if(!chargeNow)
            //{
            //    parentScript.pl_Anim.SetBool("grabJumping", false);
            //    parentScript.pl_Anim.Play("grabCharge");
            //}
            // チャージ量を上昇させる
            chargePoint++;
            chargeNow = true;
            if (!parentScript.groundLand.nowCharge)
            {
                if (parentScript.groundLand != null)
                {
                    parentScript.groundLand.ChargeLand(chargeLandDis, chargeLandSpd, true);
                }
                parentScript.pl_Audio.playerSource.loop = false;
                parentScript.pl_Audio.ChargeSound();
                parentScript.ef_Manager.PlayAura();
            }
           

        }//----- if_stop -----
        //else
        //{
        //    chargeNow = false;

        //}//----- else_stop -----
    }

    //==================================================
    //          チャージジャンプ実行
    // ※移動を支持する項目があるのでコルーチンになってます
    // ※保持されたチャージ量によってジャンプ力が変化
    // ※ジャンプ中等のアニメーションはここに書く
    // 戻り値 : なし
    //  引数  : _point : チャージ値
    //==================================================
    // 作成日2023/05/02    更新日2023/05/04
    // 宮﨑
    public IEnumerator ChargeJump(float _point)
    {
        Debug.Log("チャージ値[ " + _point + " ]で掴みジャンプを開始しますっ！");
        
        parentScript.pl_Col.isTrigger = true;

        parentScript.inRayRide = true;

        parentScript.nowJump = false;

        if (parentScript.groundLand != null)
        {
            StartCoroutine(parentScript.groundLand.ChargeEnd());
        }
        if(_point>chargeLimit/2)
        {
            parentScript.ef_Manager.PlayJumpWind();
        }

        

        float i = (60 / Test_FPS.instance.m_fps);

        // チャージ値が０になるまで続ける
        while (_point > 0)
        {
            // １フレーム毎に
            // チャージ値を減少させていく
            yield return null;
            _point-=i;
            
            // ジャンプ時の移動速度を更新する
            parentRb.velocity = new Vector3(parentRb.velocity.x, (float)_point, parentRb.velocity.y);

        }//----- while_stop -----

        parentScript.pl_Col.isTrigger = false;

        parentScript.nowJump = true;

        //chargePoint = chargeStartPoint;

        // 落下を開始する
        StartCoroutine(pl_Fall.FallTheAfterJump());
    }
}
