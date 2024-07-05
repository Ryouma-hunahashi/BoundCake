using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// 落下処理
//==================================================
// 作成日2023/04/29    更新日2023/05/01
// 宮﨑
public class Player_Fall : MonoBehaviour
{
    // 親(プレイヤー)情報を取得
    private Rigidbody parentRb;
    private Player_Main parentScript;

    // 親の子にあるスクリプトを取得
    private Player_Jump pl_Jump;

    // 落下速度の設定 ----------
    public sbyte maxFallPow = 30;
    // 現在の落下速度
    public sbyte fallPowLog;

    private void Start()
    {
        // 親の情報を取得
        parentRb = transform.parent.GetComponent<Rigidbody>();
        parentScript = transform.parent.GetComponent<Player_Main>();

        // 親の子にある情報を取得
        pl_Jump = transform.parent.GetComponentInChildren<Player_Jump>();
    }

    //==================================================
    //          ジャンプ後落下実行
    // ※移動を支持する項目があるのでコルーチンになってます
    // ※落下中等のアニメーションはここに書く
    // 戻り値 : なし
    //  引数  : なし
    //==================================================
    // 作成日2023/04/30    更新日2023/05/04
    // 宮﨑
    public IEnumerator FallTheAfterJump()
    {
        sbyte fallPow = 0;

        

        // 親オブジェクトが接地していない間繰り返す
        while(!parentScript.onRayGround)
        {
            //Debug.Log("落下中");

            if(parentScript.underRayGrab)
            {
                fallPowLog = fallPow;
                // ループを抜ける
                yield break;

            }//----- if_stop -----

            // 落下加速度の上限値まで速度を上昇させる
            if(fallPow > -maxFallPow)
            {
                fallPow--;

            }//----- if_stop -----

            // １フレームずらす
            yield return null;

            fallPowLog = fallPow;

            // 落下の速度を更新する
            parentRb.velocity = new Vector3(parentRb.velocity.x, fallPow, parentRb.velocity.z);

        }//----- while_stop -----
    }
}
