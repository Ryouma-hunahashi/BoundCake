using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボス透明化行動
// ※自身、分身両者を不可視状態にします。
// 　既に倒されている分身は再度召喚されるまでは表示されません
//==================================================
// 制作日2023/05/24    更新日2023/05/30
// 宮﨑
public class Boss4_Invisible : MonoBehaviour
{
    // 指令の情報を取得
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // 親の情報を取得
    private Boss4_Main parMain;
    private SpriteRenderer parMesh;

    // 分身の情報を取得
    private SpriteRenderer[] avaMesh;

    // 状態変化までの時間
    [SerializeField] private byte changeDelay = 180;
    [SerializeField] private byte animationDelay = 60;

    // 現在の状態
    public bool nowInvisibleAnim;
    public bool nowVisualizeAnim;
    public bool nowInvisible;

    private void Start()
    {
        // 指令の情報を取得
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();

        // 分身の情報を取得
        int avatarCnt = avatarOperator.transform.childCount;
        avaMesh = new SpriteRenderer[avatarCnt];

        // 召喚地点の数繰り返す
        for (int i = 0; i < avatarCnt; i++)
        {
            // 分身のメッシュ情報を取得
            avaMesh[i] = avatarOperator.transform.GetChild(i).GetComponentInChildren<SpriteRenderer>();

            // 一度透明化しておく
            avaMesh[i].enabled = false;

        }//----- for_stop -----

        // 親の情報を取得
        GameObject parObj = this.transform.parent.gameObject;
        parMain = parObj.GetComponent<Boss4_Main>();
        parMesh = parObj.GetComponent<SpriteRenderer>();
    }

    public IEnumerator AnimationDelay()
    {
        // 待機時間終了後、可視化、不可視化状態へ移行
        // 不可視状態なら
        if(nowInvisible)
        {
            // 待機時間を開始
            for (int i = 0; i < changeDelay; i++)
            {
                // １フレーム遅延させる
                yield return null;

                // ダメージリアクションが行われたなら処理を抜ける
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // 無敵状態を解除する
            parMain.invincibility = false;

            // 自身と倒されていない分身を表示する
            VisualizationObjects();

            // 出現アニメーションの開始 -----------
            parMain.anim.SetBool("Spawning", true);
            nowVisualizeAnim = true;

            // 描画待機時間を開始
            for (int i = 0; i < animationDelay; i++)
            {
                // １フレーム遅延させる
                yield return null;

                // ダメージリアクションが行われたなら処理を抜ける
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // 出現アニメーションの終了 -----------
            parMain.anim.SetBool("Spawning", false);
            nowVisualizeAnim = false;

        }//----- if_stop -----
        else
        {
            // 待機時間を開始
            for (int i = 0; i < changeDelay; i++)
            {
                // １フレーム遅延させる
                yield return null;

                // ダメージリアクションが行われたなら処理を抜ける
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // 消滅アニメーションの開始 -----------
            parMain.anim.Play("Hide");
            nowInvisibleAnim = true;

            // 描画待機時間を開始
            for (int i = 0; i < animationDelay; i++)
            {
                // １フレーム遅延させる
                yield return null;

                // ダメージリアクションが行われたなら処理を抜ける
                if (parMain.nowDmgReAct) yield break;

            }//----- for_stop -----

            // 消滅アニメーションの終了 -----------
            nowInvisibleAnim = false;

            // 自身と分身を不可視状態にする
            InvisibleObjects();

        }//----- else_stop -----
    }

    //==================================================
    //          不可視状態へ移行する関数
    // ※自身と分身全体を不可視化する関数です
    // 戻り値：なし
    //  引数 ：なし
    //==================================================
    // 制作日2023/05/30
    // 宮﨑
    public void InvisibleObjects()
    {
        Debug.Log("自身と分身を不可視化します");
        nowInvisible = true;

        // 自身を非表示にする
        parMesh.enabled = false;

        // 格納されたメッシュの数繰り返す
        for(int i = 0; i < avaMesh.Length; i++)
        {
            // 分身を非表示にする
            avaMesh[i].enabled = false;

        }//----- for_stop -----

        if (parMain.nowDmgReAct) return;

        // ランダムな位置に転移を開始する
        parMain.actTP.Teleportation();

        // 魔法攻撃の待機状態へ移行する
        StartCoroutine(parMain.actMB.AttackDelay());
    }

    //==================================================
    //          可視状態へ移行する関数
    // ※自身と分身全体を可視化する関数です
    // ※すでに倒されている分身や、
    // 　自身の位置に存在する分身は表示しません
    // 戻り値：なし
    //  引数 ：なし
    //==================================================
    // 制作日2023/05/30
    // 宮﨑
    public void VisualizationObjects()
    {
        Debug.Log("自身と分身を可視化します");
        nowInvisible = false;

        // 自身を表示する
        parMesh.enabled = true;

        parMain.actTP.Teleportation();

        // 格納されたメッシュの数繰り返す
        for (int i = 0; i < avaMesh.Length; i++)
        {
            // 分身が存在していないなら処理を飛ばす
            if (parMain.avaMain[i].active == false) continue;

            // フェイズ進行状態によって表示する数を変更する
            if (parMain.setAvatarPhase[parMain.nowPhase] < i) continue;

            // 分身を表示する
            avaMesh[i].enabled = true;

        }//----- for_stop -----

        if (parMain.nowDmgReAct) return;

        StartCoroutine(AnimationDelay());
    }
}
