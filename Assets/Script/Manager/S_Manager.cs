using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

// 制作日2023/02/17    更新日2023/04/25
// 宮﨑
public class S_Manager : MonoBehaviour
{
    // 遷移前五に情報を取得する
    private string sceneNow;
    private string sceneLog;
    
    // 開始時にメモリを確保する
    public static S_Manager instance = null;

    //==================================================
    // 　　　　　 (シングルトンの生成)
    // ※自分が存在していないときに１つ出現させて
    // ※逆に２つ以上存在するときは１つだけ残すようにする
    //==================================================
    // 作成日2023/04/28
    // 宮﨑
    private void Awake()
    {
        // 自分が存在していなければ追加する
        if (instance == null)
        {//----- if_start -----
            // 自分を追加
            instance = this;
            // シーン切り替え時に自分が破棄されないようにする
            DontDestroyOnLoad(this.gameObject);
        }//----- if_stop -----
        else
        {//----- else_start -----
            // すでに自分が存在している場合自分を破棄する
            Destroy(this.gameObject);
        }//----- else_stop -----
    }

    private void FixedUpdate()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            SceneChange("ContentsSelect");
        }
    }

    //==================================================
    // シーン遷移を行う関数
    // 戻り値 : なし
    //  引数  : string シーンの名前
    //==================================================
    // 制作日2023/04/25    更新日2023/04/28
    // 宮﨑
    public void SceneChange(string sceneName)
    {
        // ログを取得する
        sceneLog = SceneManager.GetActiveScene().name;

        // 現在のシーンを取得
        sceneNow = sceneName;

        // 送られてきた名前のシーンに遷移する
        SceneManager.LoadScene(sceneName);
    }
}
