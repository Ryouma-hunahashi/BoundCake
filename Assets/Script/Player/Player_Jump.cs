using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// ジャンプ処理
//==================================================
// 作成日2023/04/24    更新日2023/05/10
// 宮﨑
public class Player_Jump : MonoBehaviour
{
    //==============================
    // このスクリプト内の略称
    // pow : power
    // UL : upperLimit
    //==============================

    // 親(プレイヤー)情報を取得
    private Rigidbody mainRb;
    private Player_Main mainScript;

    // 親の子にあるスクリプトを取得
    private Player_Fall pl_Fall;

    // ジャンプ別の設定 ----------
    public byte[] jumpPow = new byte[2]
    {
        20,28
    };    // ジャンプ力(チャージ別)

    // ジャンプチャージの設定 ----------
    public bool chargeNow;          // チャージ中
    public byte chargeStartPoint;   // チャージ開始時の量
    public byte chargePoint;        // 現在チャージ量
    public byte chargeUL = 28;   // チャージ量上限

    public byte changeJump;     // ジャンプ力の変化状態
    public byte changePoint;    // ジャンプを変化させるチャージ値

    public float chargeLandDis = 0.25f;
    public float chargeLandSpd = 10f;

    //public bool nowJump;

    // ジャンプ時の設定 ----------

    // 必須ではないかも -----
    public byte chargeLog;      // チャージ値の保持


    private void Start()
    {
        // 親の情報を取得
        mainRb = transform.parent.GetComponent<Rigidbody>();
        mainScript = transform.parent.GetComponent<Player_Main>();

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
    // 作成日2023/04/29    更新日2023/05/14
    // 宮﨑
    public void ChargePower()
    {
        // ポイントが上限を超えていなければ
        if (chargePoint < chargeUL)
        {
            if (!chargeNow && chargePoint < chargeUL&&mainScript.pl_Col.isTrigger == false&&!mainScript.nowJump)
            {
                mainScript.pl_Anim.SetBool("weakJumping", false);
                mainScript.pl_Anim.SetBool("strongJumping", false);
                mainScript.pl_Anim.SetBool("JumpEnding", false);
                mainScript.pl_Anim.Play("JumpCharge");
                mainScript.ef_Manager.PlayAura();
                
            }
            // チャージ量を上昇させる
            chargePoint++;
            chargeNow = true;
            if(chargePoint == 5)
            {
                mainScript.pl_Audio.playerSource.loop = false;
                mainScript.pl_Audio.ChargeSound();
            }

            // 親のポジションを一時格納
            //var parentPos = transform.parent.GetComponent<Rigidbody>();
            // 床のコリジョンを徐々に下げる。下がる分を戻り値とし親も下げる。
            //parentPos.y-=mainScript.groundLand.ChargeLand(chargePoint,chargeUL,0.25f);
            // ポジションを更新
            //transform.parent.position = parentPos;
            if (mainScript.groundLand!=null&&!mainScript.groundLand.nowCharge)
            {
                mainScript.groundLand.ChargeLand(chargeLandDis, chargeLandSpd,false);
            }
            mainRb.useGravity = true;

        }//----- if_stop -----
    }

    //==================================================
    //          チャージジャンプ実行
    // ※移動を支持する項目があるのでコルーチンになってます
    // ※保持されたチャージ量によってジャンプ力が変化
    // ※ジャンプ中等のアニメーションはここに書く
    // 戻り値 : なし
    //  引数  : _point : チャージ値
    //==================================================
    // 作成日2023/04/30    更新日2023/05/01
    // 宮﨑
    public IEnumerator ChargeJump(float _point)
    {
        Debug.Log("チャージ値[ " + _point + " ]でジャンプを開始しますっ！");

        mainScript.nowJump = true;
        mainRb.useGravity = false;
        chargeNow = false;
        
        
        if(_point>changePoint)
        {
            mainScript.ef_Manager.PlayJumpWind();
        }

        float i = (60 / Test_FPS.instance.m_fps);
        if (mainScript.groundLand != null)
        {
            // 地面のチャージ中フラグを切る。
            StartCoroutine(mainScript.groundLand.ChargeEnd());
        }
        // チャージ値が０になるまで続ける
        while (_point > 0)
        {
            //if (parentScript.underRayGrab)
            //{
            //    // 落下を開始する
            //    StartCoroutine(pl_Fall.FallTheAfterJump());
            //    yield break;

            //}//----- if_stop -----

            // １フレーム毎に
            // チャージ値を減少させていく
            yield return null;
            _point-=i;

            // ジャンプ時の移動速度を更新する
            mainRb.velocity = new Vector3(mainRb.velocity.x, (float)_point, mainRb.velocity.z);

        }//----- while_stop -----

        // 落下を開始する
        StartCoroutine(pl_Fall.FallTheAfterJump());
    }
}