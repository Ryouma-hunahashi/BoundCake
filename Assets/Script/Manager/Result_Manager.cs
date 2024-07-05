using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Cinemachine;

//=================================
// ステージのリザルト情報
//=================================

public class Result_Manager : MonoBehaviour
{
    // シングルトンの作成
    public static Result_Manager instance;

    private void Awake()
    {
        // 自身が存在していないなら
        if (instance == null)
        {
            // 自信をインスタンス化
            instance = this;

            // シーン変更時に破棄されないようにする
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            // 既に存在しているなら破棄
            Destroy(this.gameObject);
        }
    }
    
    //  Stage_Manager stage_Manager;
    StageSelector stageSelector;
    Parfait_Manager parfait_Manager;
    Get_Parfait get_Parfait;

    [Tooltip("さっきやったステージの情報")]
    public string stagename;
   public Canvas Canvas; // 自身のキャンバス
   
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
    public byte nowWorld;  // ワールド番号
    public byte nowStage;  // ステージ番号
    // ステージ情報を格納
    public List<Stage_Manager.WorldInfo> mapInfo;

    bool nextcheck = true;
    public bool selectercheck = false;

    private void OnApplicationQuit()
    {
        mapInfo.Clear();
    }

}
