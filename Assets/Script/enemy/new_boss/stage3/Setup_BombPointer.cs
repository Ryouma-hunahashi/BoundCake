using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          爆弾召喚地点のセットアップ
// ※回転床の親に設置すると床の数自動で
// 　召喚地点を設定します。
//==================================================
// 制作日2023/05/26    更新日2023/05/28
// 宮﨑
public class Setup_BombPointer : MonoBehaviour
{
    // 転移先オペレーター設定
    public string bpo = "BombPointOperator";
    public string pointName = "BombPoint";

    // 自分で設定オブジェクトを指定する
    public bool auto = true;

    public bool blasted;    // 爆発したかどうか

    private void Awake()
    {
        if(auto)
        {
            AutoSetup();
        }
    }

    //==================================================
    //          自動で分身を設定する
    // 戻り値 ：なし
    //　引数　：なし
    //==================================================
    // 制作日2023/05/24
    // 宮﨑
    private void AutoSetup()
    {
        // 自身の名前を変更する
        this.gameObject.name = bpo;

        // 自身の子の数を取得
        int children = this.transform.childCount;

        // 自身の子の数繰り返す
        for (int i = 0; i < children; i++)
        {
            // 子オブジェクトを一時的に格納
            GameObject childObj = transform.GetChild(i).gameObject;
            Vector3 childObj_Pos = childObj.transform.position;

            // 床の数に応じて子オブジェクトを生成
            // 子オブジェクト毎にIDを振り分ける
            GameObject bombObj = new GameObject(pointName + i);

            // 床の子オブジェクトに設定する
            bombObj.transform.SetParent(childObj.transform, false);

            // 床のオブジェクトの少し上にスポーンポイントを設定する
            bombObj.transform.position = new Vector3(childObj_Pos.x, childObj_Pos.y + 1, childObj_Pos.z);

        }//----- for_stop -----
    }
}