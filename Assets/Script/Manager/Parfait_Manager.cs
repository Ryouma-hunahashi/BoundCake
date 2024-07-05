using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//========================================
//          ステージ中の所持更新
//  ※ゲーム中にパフェが回収されたら実行
//========================================
// 作成日2023/05/19    更新日2023/05/21
// 宮﨑
public class Parfait_Manager : MonoBehaviour
{
    // シングルトンの作成
    public static Parfait_Manager instanse;

    private void Awake()
    {
        // 自身が存在していないなら
        if (instanse == null)
        {
            // 自身をインスタンス化
            instanse = this;

            // シーン変更時に破棄されないようにする
            DontDestroyOnLoad(this.gameObject);
        }//----- if_stop -----
        else
        {
            // すでに自身が存在しているなら破棄
            Destroy(this.gameObject);

        }//----- else_stop -----
    }

    // ゲーム中のパフェ保持状況を格納
    [System.Serializable]
    public struct ParfaitHolder
    {
        public bool top;
        public bool mid;
        public bool btm;
    }

    // ゲーム中の取得情報
    public ParfaitHolder getParfait;

    // 選択されたステージ情報を取得
    private byte nowWorld;  // ワールド番号
    private byte nowStage;  // ステージ番号

    // ステージ情報を格納
    public List<Stage_Manager.WorldInfo> mapInfo;

    private void Start()
    {
        // ステージ情報を取得
        mapInfo = Stage_Manager.instanse.worldInformation;
    }

    //========================================
    //          パフェ取得情報の更新
    //  ※ゴール後にパフェの状態を更新する
    //========================================
    // 作成日2023/05/19
    // 宮﨑
    public void ParfaitUpdate()
    {
        // 追記待機
    }

    private void OnApplicationQuit()
    {
        mapInfo.Clear();
    }
}
