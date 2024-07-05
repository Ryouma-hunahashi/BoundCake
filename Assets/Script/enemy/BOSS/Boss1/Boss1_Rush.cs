using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボスの突進挙動スクリプト
// ※指定した方向,距離,速度で突進を開始します
//==================================================
// 制作日2023/05/16
// 宮﨑
public class Boss1_Rush : MonoBehaviour
{
    // 親の情報を取得
    private GameObject par_Obj;
    private Animator par_Anim;
    private Rigidbody par_Rb;
    private Vector3 par_Scale;

    // 親の挙動情報を取得
    private Boss1_Main par_Main;
    private Boss1_Shield par_Shield;

    // コルーチンの格納
    [System.NonSerialized] public IEnumerator cor_Rush; // 突進

    // 突進状態
    public bool nowRush;

    // 突進の各地点を取得
    public Vector3 startPos;    // 開始地点の保持
    public Vector3 finishPos;   // 終了地点の予測

    public Vector3 fallPos;  // 着地時の位置を保持

    // 座標を取得したかどうか
    private bool savedPos;

    // 突進の設定
    [SerializeField] private float speed = 16;      // 速度
    [SerializeField] private float direction = -26; // 距離
    [SerializeField] private float acceleration;    // 加速度
    [SerializeField] private float diceleration;    // 減速度

    private void Start()
    {
        // 自身の名前を"rush"にする
        this.gameObject.name = "rush";

        // 親の情報を取得
        par_Obj = transform.parent.gameObject;
        par_Anim = par_Obj.GetComponent<Animator>();
        par_Rb = par_Obj.GetComponent<Rigidbody>();

        // 親の挙動情報を取得
        par_Main = transform.parent.GetComponent<Boss1_Main>();
        par_Shield = transform.parent.GetComponentInChildren<Boss1_Shield>();

        cor_Rush = RushAction();

        // 親の大きさを取得
        par_Scale = par_Obj.transform.localScale;

        // 反対を向いているなら
        if (par_Scale.x < 0)
        {
            // 大きさを補正する
            par_Scale.x *= -1;

        }//----- if_stop -----

        // 初期位置を取得
        fallPos = par_Obj.transform.localPosition;
    }

    //==================================================
    //          盾構え突進攻撃
    // ※防御行動後、盾を構えながら突進する
    //==================================================
    // 制作日2023/05/16
    // 宮﨑
    public IEnumerator RushAction()
    {
        if (par_Shield.nowStan) yield break;
        

        Debug.Log("突進を開始しましたっ！");

        // 突進状態にする
        nowRush = true;
        if (par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.Stop();
        }
        par_Main.audio.bossSource.loop = true;
        par_Main.audio.BossDashSound();
        // 突進アニメーションを開始
        par_Anim.SetBool("rushing", true);

        // 突進開始時の座標を取得しておく
        startPos = par_Obj.transform.position;

        // 突進終了地点を予測する
        finishPos = par_Obj.transform.position;
        finishPos.x += direction;

        // 移動方向が右側(X++)方向なら
        if (direction > 0)
        {
            while (finishPos.x > par_Obj.transform.position.x)
            {
                // １フレーム遅延させる
                yield return null;

                // 速度を更新する
                par_Rb.velocity = new Vector3(speed, 0, 0);

                
            }//----- while_stop -----

            // 向きを補正する
            // 自身が右を向いているなら
            if (par_Obj.transform.localScale.x > 0)
            {
                // 自身を左方向へ向かせる
                par_Obj.transform.localScale = new Vector3(-par_Scale.x, par_Scale.y, par_Scale.z);

                Debug.Log("左に向かせましたっ！");

                // 移動方向を変更する
                direction *= -1;

            }//----- if_stop -----

        }//----- if_stop -----
        // 移動方向が左側(X--)方向なら
        else if (direction < 0)
        {
            while (finishPos.x < par_Obj.transform.position.x)
            {
                // １フレーム遅延させる
                yield return null;

                // 速度を更新する
                par_Rb.velocity = new Vector3(-speed, 0, 0);

            }//----- while_stop -----

            // 向きを修正する
            // 自身が左を向いているなら
            if (par_Obj.transform.localScale.x < 0)
            {
                // 自身を右方向へ向かせる
                par_Obj.transform.localScale = new Vector3(par_Scale.x, par_Scale.y, par_Scale.z);

                Debug.Log("右に向かせましたっ！");

                // 移動方向を変更する
                direction *= -1;

            }//----- if_stop -----

        }//----- elseif_stop -----

        // 位置を補正する
        par_Rb.velocity = Vector3.zero;
        par_Rb.position = finishPos;

        fallPos = par_Obj.transform.localPosition;

        // 突進状態を解除する
        nowRush = false;
        if (par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.Stop();
        }
        par_Main.audio.bossSource.loop = false;

        // 突進アニメーションを終了
        par_Anim.SetBool("rushing", false);

        // 次の動作に移行する
        StopCoroutine(par_Shield.cor_Rotate);
        par_Shield.cor_Rotate = par_Shield.Rotate();
        StartCoroutine(par_Shield.cor_Rotate);
    }
}
