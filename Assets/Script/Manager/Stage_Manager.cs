using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//========================================
//          ステージの情報
//========================================
public class Stage_Manager : MonoBehaviour
{
    // シングルトンの作成
    public static Stage_Manager instanse;

    private void Awake()
    {
        
        // 自身が存在していないなら
        if(instanse == null)
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

        for(byte i = 0;i<worldInformation.Count;i++)
        {
            worldInformation[i].worldLockLog = worldInformation[i].worldLock;
        }
    }

    // ワールド情報の設定
    [System.Serializable]
    public class WorldInfo
    {

        // ズームの設定
        public enum ZoomSet
        {
            ON,
            OFF,
        }
        // ワールド情報
        [Tooltip("地図片の名前")]
        public string worldName;
        [Tooltip("ワールドの開放状況")]
        public bool worldLock;
        public bool worldLockLog;
        // ステージ情報のクラスを取得
        [Tooltip("ステージの情報")]
        public List<StageInfo> stageInformation = new List<StageInfo>();
        [Tooltip("対応するImageにズームを行う")]
        public ZoomSet zoomSet;
        // カメラ情報
        [Tooltip("カメラを固定する座標\n" + "Imageからの相対座標")]
        public Vector3 cameraZoomPos;
    }

    // ステージ情報の設定
    [System.Serializable]
    public class StageInfo
    {
        // パフェの所持状態
        [System.Serializable]
        public struct parfaitInfo
        {
            public bool top;    // 上層
            public bool mid;    // 中層
            public bool btm;    // 下層
        }

        

        // ステージ情報
        [Tooltip("ステージの名前")]
        public string stageName;
        [Tooltip("シーンの名前")]
        public string sceneName;
        [Tooltip("ステージのロック情報")]
        public bool stageLock = false;

        // 収集アイテムの状況
        [Tooltip("パフェの所持状況")]
        public parfaitInfo parfait;

        
        
    }

    // インスペクターに表示 -----
    // 地図情報の設定
    public List<WorldInfo> worldInformation = new List<WorldInfo>();

    private void OnApplicationQuit()
    {
        worldInformation.Clear();
       
    }
}