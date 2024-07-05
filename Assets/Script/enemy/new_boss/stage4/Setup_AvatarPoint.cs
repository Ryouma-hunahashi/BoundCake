using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          分身の地点を開始時に設定する
// ※別スクリプトで[AvatarPoint]系統が必要です
// ※ゲーム開始時に分身を自動設定します
//==================================================
// 制作日2023/05/24
// 宮﨑
public class Setup_AvatarPoint : MonoBehaviour
{
    // 転移先オペレーター設定
    public string apo = "AvatarPointOperator";  // 親オブジェクトの名前(固定)
    public string pointName = "AvatarPoint";    // 子の名前(名前の後にID使用)

    // 分身のプレハブ
    public GameObject avatarPrefab;

    // 自身で設定オブジェクトを指定する
    public bool auto = true;    // 自動でポイントを設定する

    private void Awake()
    {
        // 自動で設定しないなら
        if(auto)
        {
            // 自動で設定を開始します
            AutoSetup();

        }//----- if_stop -----
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
        this.gameObject.name = apo;

        // 自身の子の数を取得
        int children = this.transform.childCount;

        // 自身の子の数繰り返す
        for (int i = 0; i < children; i++)
        {
            // 召喚地点を一時的に格納
            GameObject pointObj = transform.GetChild(i).gameObject;

            // 召喚地点にIDを振り分ける
            pointObj.name = pointName + i;

            // 分身が設定されていないなら処理を抜ける
            if (avatarPrefab == null) continue;

            // 分身を召喚しIDを名前に追記する
            GameObject avatarObj = Instantiate(avatarPrefab);
            avatarObj.name = "Avatar" + i;

            // 分身の挙動を取得する
            Boss4_Avatar_Main avatarMain = avatarObj.GetComponent<Boss4_Avatar_Main>();

            // 分身にIDを割り当てる
            avatarMain.myID = i;
            
            // 分身を召喚地点の子オブジェクトに指定する
            avatarObj.transform.SetParent(pointObj.transform, false);

        }//----- for_stop -----
    }
}
