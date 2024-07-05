using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          召喚地点に移動する
// ※[Setup_AvatarPointer]にて生成された召喚地点へ
// 　ランダムで移動するスクリプトです
// ※シーン上に"AvatarPointOperator"が必要です
//==================================================
// 制作日2023/05/24    // 更新日2023/05/30
// 宮﨑
public class Boss4_Teleportation : MonoBehaviour
{
    // 指令の情報を取得
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // 親の情報を取得
    private Boss4_Main parMain;

    // 現在の状態
    public bool teleported;

    private void Start()
    {
        // 指令の情報を取得
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();

        // 親の情報を取得
        GameObject parObj = this.transform.parent.gameObject;
        parMain = parObj.GetComponent<Boss4_Main>();
    }

    //==================================================
    //          指定された地点へランダムに移動を行う関数
    // ※自身のメイン挙動から格納されたランダムな値を利用して
    // 　自身、分身の移動地点を決定、そこへ移動を行います
    // 戻り値：なし
    //  引数 ：なし
    //==================================================
    // 制作日2023/05/30
    // 宮﨑
    public void Teleportation()
    {
        if (parMain.nowDmgReAct) return;

        Debug.Log("転移します");

        // 移動位置を乱数で指定する
        parMain.Randomizer();

        // 親の情報を取得
        GameObject parObj = this.transform.parent.gameObject;

        // 親の位置を変更する
        parObj.transform.position = avatarOperator.transform.GetChild(parMain.randomPoint[0]).transform.position;

        // 格納された分身の数繰り返す
        for (int i = 0; i < parMain.avaObj.Length; i++)
        {
            // 分身が存在していないなら処理を飛ばす
            if (parMain.avaMain[i].active == false) continue;

            // 分身の位置を変更する
            parMain.avaObj[i].transform.position = avatarOperator.transform.GetChild(parMain.randomPoint[i]).transform.position;

        }//----- for_stop -----

        teleported = true;
    }
}
