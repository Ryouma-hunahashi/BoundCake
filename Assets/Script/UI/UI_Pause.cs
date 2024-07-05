using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UI_Pause : MonoBehaviour
{
    // ポーズ状態
    private bool nowPause = false;
    private bool outPause = false;

    // ポーズ切り替え後一度のみ
    private bool setPause = false;

    // 現在の選択地点
    private byte selectNum = 0;

    // 数値に変更があった際に変更
    private bool selectChanged = false;

    // 表示する画面を格納
    [SerializeField] private List<Image> buttonImages = new List<Image>();
    [SerializeField] private List<Text> textNames = new List<Text>();

    // 子の数を数値で格納
    private byte childCount;

    [SerializeField] private Animator goSelectAnim;
    [SerializeField] private Animator ContinueAnim;

    private void Start()
    {
        // 子オブジェクトの数を取得
        childCount = (byte)this.transform.childCount;

        for (byte i = 0; i < buttonImages.Count; i++)
        {
            // UIを非表示にする
            buttonImages[i].enabled = false;

        }//----- for_stop -----

        goSelectAnim = buttonImages[0].gameObject.GetComponent<Animator>();
        ContinueAnim = buttonImages[1].gameObject.GetComponent<Animator>();

        for (byte i = 0; i < textNames.Count; i++)
        {
            // UIを非表示にする
            textNames[i].enabled = false;

        }//----- for_stop -----
        goSelectAnim.SetBool("lockOn", false);
        ContinueAnim.SetBool("lockOn", true);
    }

    void Update()
    {
        // ゲームパッドが接続されていないなら抜ける
        if (Gamepad.current == null) return;

        // ポーズ関係のボタンの設定
        bool ActButton = Gamepad.current.buttonSouth.wasPressedThisFrame;
        bool pauseButton = Gamepad.current.startButton.wasPressedThisFrame;

        // スティックの上下たおし具合を取得する
        bool Lstick_Right = Gamepad.current.leftStick.right.wasPressedThisFrame;
        bool Lstick_Left = Gamepad.current.leftStick.left.wasPressedThisFrame;

        // ポーズボタンが押されたとき
        // または、ポーズを抜けるとき
        if (pauseButton || outPause)
        {
            // ポーズ状態を切り替えます
            nowPause = !nowPause;

            setPause = true;
            outPause = false;

            Debug.Log("ポーズ状態が[ " + nowPause + " ]になりました。");

            // ポーズ状態ではなくなったとき
            if(!nowPause)
            {
                // 動作を再開する
                Time.timeScale = 1.0f;

                // 変更後一度のみ実行
                if (setPause)
                {
                    for (byte i = 0; i < buttonImages.Count; i++)
                    {
                        // UIを非表示にする
                        buttonImages[i].enabled = false;

                    }//----- for_stop -----


                    for (byte i = 0; i < textNames.Count; i++)
                    {
                        // UIを非表示にする
                        textNames[i].enabled = false;

                    }//----- for_stop -----

                    // 上記実行移行変更まで待機
                    setPause = false;

                }//----- if_stop -----

            }//----- if_stop -----

        }//----- if_stop -----

        // ポーズ状態のとき
        if(nowPause)
        {
            // 動作が止まっていないとき
            if (Time.timeScale != 0)
            {
                // Fixed等の動作をストップさせる
                Time.timeScale = 0.000001f;
            
            }//----- if_stop -----

            // 変更後一度のみ実行
            if(setPause)
            {
                // 表示したときに指定位置を初期位置に戻す
                selectNum = 0;
                for (byte i = 0; i < buttonImages.Count; i++)
                {
                    // UIを非表示にする
                    buttonImages[i].enabled = true;
                    // 指定番号に沿って色を変更する
                    if (selectNum == i)
                    {
                        // 選択されているウィンドウの色を変更する
                        //buttonImages[i].color = Color.gray;
                        goSelectAnim.SetBool("lockOn", false);

                    }//----- if_stop -----
                    else if (selectNum != i)
                    {
                        // 選択されているウィンドウの色を変更する
                        //buttonImages[i].color = Color.white; 
                        ContinueAnim.SetBool("lockOn", true);

                    }
                }//----- for_stop -----


                for (byte i = 0; i < textNames.Count; i++)
                {
                    // UIを非表示にする
                    textNames[i].enabled = true;

                }//----- for_stop -----
                
                    // 上記実行移行変更まで待機
                    setPause = false;

            }//----- if_stop -----

            // 上下どちらかの入力を受けとったとき
            // 角度の許容範囲を上方向へ超えた場合
            if (Lstick_Left)
            {
                // 指定位置が'0'より上なら
                if (selectNum > 0)
                {
                    // 選択地点を上へずらす
                    selectNum--;

                }//----- if_stop -----

                // 変更後の状態を送る
                selectChanged = true;

            }//----- if_stop -----
             // 角度の許容範囲を下方向へ超えた場合
            else if (Lstick_Right)
            {
                // 指定位置が'子の数 - 配列調整'の値以下なら
                if (selectNum < childCount - 1)
                {
                    // 選択地点を下へずらす
                    selectNum++;

                }//----- if_stop -----

                // 変更後の状態を送る
                selectChanged = true;

            }//----- elseif_stop -----

            // 指定位置に変更があった場合
            if (selectChanged)
            {
                selectChanged = false;
                if (selectNum == 0)
                {
                    ContinueAnim.SetBool("lockOn", true);
                    goSelectAnim.SetBool("lockOn", false);
                }
                else if (selectNum == 1)
                {
                    ContinueAnim.SetBool("lockOn", false);
                    goSelectAnim.SetBool("lockOn", true);
                }

                //for (byte i = 0; i <buttonImages.Count; i++)
                //{
                //    if(selectNum == i)
                //    {
                //        // 選択されているウィンドウの色を変更する
                //        //buttonImages[i].color = Color.gray;
                //        //ContinueAnim.SetBool("lockOn", true);
                        


                //    }//----- if_stop -----
                //    else if(selectNum != i)
                //    {
                //        // 選択されているウィンドウの色を変更する
                //        //buttonImages[i].color = Color.white;
                //        //goSelectAnim.SetBool("lockOn", false);
                //        //if (selectNum == 0)
                //        //{
                //        //    ContinueAnim.SetBool("lockOn", true);
                //        //    goSelectAnim.SetBool("lockOn", true);
                //        //}
                //        //else if (selectNum == 1)
                //        //{
                //        //    ContinueAnim.SetBool("lockOn", true);
                //        //    goSelectAnim.SetBool("lockOn", false);
                //        //}

                //    }//----- elseif_stop -----

               // }//----- for_stop -----

            }//----- if_stop -----

            // 決定ボタンが押されたとき
            if(ActButton)
            {
                // 選択されているUIによって変化
                switch (selectNum)
                {
                    case 0:
                        {
                            // ポーズ状態を抜ける
                            outPause = true;

                        }// 再開を選択
                        break;
                    case 1:
                        {
                            Time.timeScale = 1.0f;
                            // ステージセレクトに送ってください
                            SceneManager.LoadScene("ContentsSelect");
                        }// 終了を選択
                        break;
                }//----- switch_stop -----
            }//----- if_stop -----

        }//----- if_stop -----
    }
    private void OnApplicationQuit()
    {
        buttonImages.Clear();
        textNames.Clear();
    }
}
