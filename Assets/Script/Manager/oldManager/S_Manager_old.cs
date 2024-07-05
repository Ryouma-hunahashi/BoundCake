using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;

//==================================================
// ※このスクリプトが入っているゲームオブジェクトの呼び方
//   このスクリプト内では"自分"と書きます
//==================================================

// 作成日2023/02/17    更新日2023/02/21
// 宮﨑
public class S_Manager_old : MonoBehaviour
{
    // コントローラー押し込み時の真偽値=================
    [Header("----- ボタン状態確認用 -----"), Space(5)]  // 以下状態の確認をインスペクタ上で可能
    [Header("左スティックの状態確認")]

    // 左スティックを左へ倒している間のみ[true]になる
    [Tooltip("左スティックが左に倒されている状態のときのみ[true]になる")]
    [SerializeField] private bool Lstick_Left = false;

    // 左スティックを右へ倒している間のみ[true]になる
    [Tooltip("左スティックが右に倒されている状態のときのみ[true]になる")]
    [SerializeField] private bool Lstick_Right = false;

    // コントローラー押し込み時の真偽値=================
    [Header("十字ボタンの状態確認")]

    // 左スティックを左へ倒している間のみ[true]になる
    [Tooltip("十字ボタンが左に押されている状態のときのみ[true]になる")]
    [SerializeField] private bool dpad_Left = false;

    // 左スティックを右へ倒している間のみ[true]になる
    [Tooltip("十字ボタンが右に押されている状態のときのみ[true]になる")]
    [SerializeField] private bool dpad_Right = false;

    //==================================================

    // キーボード用操作
    private KeyCode leftArrow = KeyCode.LeftArrow;      // 左矢印キー
    private KeyCode rightArrow = KeyCode.RightArrow;    // 右矢印キー
    private KeyCode leftKey = KeyCode.A;    // Aキーを左とする
    private KeyCode rightKey = KeyCode.D;   // Dキーを右とする

    [SerializeField]
    private KeyCode decisionKey = KeyCode.Space;    // スペースキーを決定ボタンとする

    [Header("----- ※ シーン名情報 -----"), Space(5)]  // 必ずインスペクタ上で指定

    [Header("※各シーンの名前")]
    [Tooltip("タイトルにするシーンの名前に変更があったら書き換える")]
    [SerializeField] private string titleSceneName;

    [Tooltip("ステージ選択にするシーンの名前に変更があったら書き換える")]
    [SerializeField] private string selectSceneName;

    [Tooltip("リザルトにするシーンの名前に変更があったら書き換える")]
    [SerializeField] private string resultSceneName;

    [Header("※ステージシーンの名前")]
    [Tooltip(
        "(※必須項目)\n" +
        "ステージ番号以外のシーン名を入力\n" +
        "シーンの名前に変更があったら書き換える")]
    [SerializeField] private string stageName;

    [Tooltip("存在している移動したいステージ番号を入力")]
    [SerializeField] private int stageNum;

    // 開始時にメモリを確保する
    public static S_Manager_old instance = null;

    // ゴールの判定を取得する
    public static bool gameClear;

    //==================================================
    // 　　　　　 (シングルトンの生成)
    // ※自分が存在していないときに１つ出現させて
    // ※逆に２つ以上存在するときは１つだけ残すようにする
    //==================================================
    // 作成日2023/02/17
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

    //==================================================
    // ※ステージ番号が最大または最低値になったとき
    // 　繰り返すか,最大,最低値で止めるかの選択用列挙型
    // ==================================================
    private enum StageNum
    {//----- enum_start -----
        // 止めるか繰り返すかの設定
        stop,   // 数値の端で止める
        repeat, // 数値の端を超えるとループする
        
        // 未設定の状態
        none,
    }//----- enum_stop -----

    // シーンリストを作成
    private enum SceneList
    {//----- enum_start -----
        // タイトル画面
        title = 0,
        select = 1,
        //==================================================
        // 　　　　（ゲームプレイ画面シーンリスト）
        // ※ステージ追加ごとに番号順に記入してください
        // ※ワールドごとにworld_[n]
        //   [n] x 10の数値を設定します
        //==================================================
        // world_1
        stage1 = 10,
        stage2,
        stage3,
        stage4,
        stage5,
        // world_2
        stage6 = 20,
        stage7,
        // リザルト画面
        result = 300,

        // 未設定の状態
        none = 500,
    }//----- enum_stop -----

    // ステージ番号の最小値,最大値======================
    // ステージ追加や削除など番号の
    // 変更に合わせてここの数値を変更してください
    [Header("※ステージ番号の最小,最大値の入力")]

    // インスペクタ上から必ず１以上の値で設定してください
    [Tooltip("移動したい最小ステージ番号を１以上の値で設定")]
    [SerializeField] private int minStageNum;

    // ステージ追加や削除の度にインスペクタから書き換えてください
    [Tooltip("移動したい最大ステージ番号を存在する数値内で設定")]
    [SerializeField] private int maxStageNum;
    //==================================================

    // タイトル,リザルトから飛ぶシーン =================
    // 次に遷移するシーンのリスト
    private enum NextSceneList
    {//----- enum_start -----
        title,
        select,
        result,
        stage,
        none,
    }//----- enum_stop -----

    // タイトルシーンから遷移するシーンを選択
    [Header("※各シーンから移動するシーンを選択")]
    [Tooltip("タイトルシーンから次に移行するシーンを設定")]
    [SerializeField] private NextSceneList titleNextScene = NextSceneList.none;

    // リザルトシーンから遷移するシーンを選択
    [Tooltip("リザルトシーンから次に移行するシーンを設定")]
    [SerializeField] private NextSceneList resultNextScene = NextSceneList.none;

    //==================================================

    // ステージ番号が最大,最小値になったときどういった動作をとるか
    [SerializeField, Header("※ステージ番号の設定")]
    [Tooltip("番号の値が限界に達したときの動作の設定")]
    private StageNum stageNumStatus = StageNum.none;

    // シーン位置の確認=================================
    // インスペクター上で今いるシーンを確認できる
    [Header("----- 状態確認用 -----"),Space(5)]  // 以下状態の確認をインスペクタ上で可能
    [SerializeField, Header("現在のシーン")]
    [Tooltip("現在アクティブになっているシーンがここに表示されます")]
    private string sceneNameNow = "None";

    // インスペクター上で１つ前にいたシーンを確認できる
    [SerializeField,Header("１つ前のシーン")]
    [Tooltip("１つ前にアクティブになっていたシーンがここに表示されます")]
    private string sceneNameOld = "None";
    //==================================================

    // GameManagerの仕様まで非表示---------------------------------------------
    // インスペクター上から現在選択しているシーンを確認
    //[SerializeField, Header("選択中のシーン")]
    private SceneList sceneSelect = SceneList.none;
    // インスペクター上から1つ前にいたシーンを確認できる
    //[SerializeField, Header("前回のシーン")]
    //private SceneList sceneLog = SceneList.none;

    // 選択中のステージ番号
    [SerializeField,Header("選択中のステージ番号")]
    [Tooltip("現在選択されているステージ番号がここに表示されます")]
    private int m_selectStageNum;

    // コマンド用(後で消します)

    // 各入力後の制限時間
    private int LD, BW;

    // 作成日2023/02/17
    // 宮﨑
    //サンプルシーンに戻る
    private void HideCommandRoom()
    {
        // ゲームパッドが接続されていなければnull
        if (Gamepad.current == null) return;

        // Lスティックの下を離したあと60の猶予
        if (Gamepad.current.leftStick.down.wasReleasedThisFrame)
        {//----- if_start -----
            LD = 60;
        }//----- if_stop -----
         // Xボタンを押したあと60の猶予
        if (Gamepad.current.buttonWest.wasPressedThisFrame && (LD > 0))
        {//----- if_start -----
            BW = 60;
        }//----- if_stop -----
         // Bボタンを押したあとサンプルシーンに戻る
        if (Gamepad.current.buttonEast.wasPressedThisFrame && (BW > 0))
        {//----- if_start -----
         SceneManager.LoadScene("SampleScene");
        }//----- if_stop -----

        // 各操作の制限時間を減らす
        if(LD >= 0)
        {//----- if_start -----
            LD--;
        }//----- if_stop -----

        if (BW >= 0)
        {//----- if_start -----
            BW--;
        }//----- if_stop -----
    }

    // 作成日2023/02/18
    // 宮﨑
    private void SelectSceneCommandRoom()
    {
        // ゲームパッドが接続されていなければnull
        if (Gamepad.current == null) return;

        if(Gamepad.current.leftTrigger.wasPressedThisFrame &&
            Gamepad.current.rightTrigger.wasPressedThisFrame)
        {
            SceneManager.LoadScene("select");
        }
    }
    // ここまでコマンド

    //==================================================
    // 　　　ステージ番号の停止と繰り返し処理
    // ※[stageNumStatus]によって設定されている変数を
    // 　使用してステージ番号の数値を以下の動きに変動させる
    //==================================================
    // 制作日2023/02/17
    // 宮﨑
    private void StopOrRepeatOfTheStageSelectNum()
    {
        // [minStageNum]/[maxStageNum]が
        // 正しく設定されていない場合エラーを出す
        if (minStageNum < 1)
        {//----- if_start -----
            Debug.LogError("[minStageNum]が正しく設定されていません！");
        }//----- if_stop -----
        //if (maxStageNum)
        //{
        //    Debug.LogError("[maxStageNum]が正しく設定されていません！");
        //}

        // [stageNumStatus]の状態によって動きを変更する
        switch (stageNumStatus)
        {//----- switch_start -----
            // 数値が最大,最小を超えないようにする
            case StageNum.stop:
                if (m_selectStageNum < minStageNum)
                {//----- if_start -----
                    m_selectStageNum = minStageNum;
                }//----- if_stop -----
                if (m_selectStageNum > maxStageNum)
                {//----- if_start -----
                    m_selectStageNum = maxStageNum;
                }//----- if_stop -----
                break;
            // 数値が最大,最小を超えた時,反転して繰り返す
            case StageNum.repeat:
                if (m_selectStageNum < minStageNum)
                {//----- if_start -----
                    m_selectStageNum = maxStageNum;
                }//----- if_stop -----
                if (m_selectStageNum > maxStageNum)
                {//----- if_start -----
                    m_selectStageNum = minStageNum;
                }//----- if_stop -----
                break;

            // [stageNumStatus]が正しく設定されていない場合エラー表記
            case StageNum.none:
            default:
                Debug.LogError("[stageNumStatus]が正しく設定されていません！");
                break;
        }//----- switch_stop -----

    }

    //==================================================
    // コントローラー操作
    // ※スティックを倒している間に(Lstick_Left,Right)に真を出力する
    // ※真が出力されている間選択中のステージ番号を変更する
    //==================================================
    // 制作日2023/02/17
    // 宮﨑
    private void StageSelectWithController()
    {
        // ゲームパッドが接続されていなければnull
        if (Gamepad.current == null) return;

        // Lスティックが左にたおされている間
        // Lstick_Leftをtrueにする
        if (Gamepad.current.leftStick.left.wasPressedThisFrame)
        {//----- if_start -----
            Lstick_Left = true;
        }//----- if_stop -----
        else if(Gamepad.current.leftStick.left.wasReleasedThisFrame)
        {//----- elseif_start -----
            Lstick_Left = false;
        }//----- elseif_stop -----

        // Lスティックが右にたおされている間
        // Lstick_Rightをtrueにする
        if (Gamepad.current.leftStick.right.wasPressedThisFrame)
        {//----- if_start -----
            Lstick_Right = true;
        }//----- if_stop -----
        else if (Gamepad.current.leftStick.right.wasReleasedThisFrame)
        {//----- elseif_start -----
            Lstick_Right = false;
        }//----- elseif_stop -----

        // 十字ボタンが左におされている間
        // dpad_Leftをtrueにする
        if (Gamepad.current.dpad.left.wasPressedThisFrame)
        {//----- if_start -----
            dpad_Left = true;
        }//----- if_stop -----
        else if (Gamepad.current.dpad.left.wasReleasedThisFrame)
        {//----- elseif_start -----
            dpad_Left = false;
        }//----- elseif_stop -----

        // 十字ボタンが右におされている間
        // dpad_Rightをtrueにする
        if (Gamepad.current.dpad.right.wasPressedThisFrame)
        {//----- if_start -----
            dpad_Right = true;
        }//----- if_stop -----
        else if (Gamepad.current.dpad.right.wasReleasedThisFrame)
        {//----- elseif_start -----
            dpad_Right = false;
        }//----- elseif_stop -----
    }

    //==================================================
    // ※コントローラ―やキーボード操作に合わせて
    // 　選択位置の変更を行う
    //==================================================
    // 制作日2023/02/17
    // 宮﨑
    private void ChangeSelectPosition()
    {
        // ゲームパッドが接続されていなければnull
        if (Gamepad.current == null) return;

        // world_1の空きステージを飛ばす処理
        // stage1以上,stage5以下のときに実施
        // ※不備あり
        //=テスト実施=======================================

        if ((sceneSelect >= SceneList.stage1) || (sceneSelect <= SceneList.stage5))
        {//----- if_start -----
            if (Lstick_Left)
            {//----- if_start -----
                sceneSelect--;
            }//----- if_stop -----
            if (Lstick_Right)
            {//----- if_start -----
                sceneSelect++;
            }//----- if_stop -----
        }//----- if_stop -----
        if ((sceneSelect >= SceneList.stage6) || (sceneSelect <= SceneList.stage7))
        {//----- if_start -----
            if (Lstick_Left)
            {//----- if_start -----
                sceneSelect--;
            }//----- if_stop -----
            if (Lstick_Right)
            {//----- if_start -----
                sceneSelect++;
            }//----- if_stop -----
        }//----- if_stop -----

        //==================================================
    }

    //==================================================
    // コントローラー操作
    // ※決定ボタンを押したときにシーンを変更する
    //==================================================
    // 制作日2023/02/17
    // 宮﨑
    private void ChangeSceneWithControllerDecisionButton()
    {
        // ゲームパッドが接続されていなければnull
        if (Gamepad.current == null) return;

        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {//----- if_start -----
            // シーンを[selectStageNum]で指定したステージに変更する
            SceneManager.LoadScene("stage" + m_selectStageNum);
            // 現在使用しているシーンを記録する
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    //==================================================
    // キーボード操作
    // ※決定キーを押したときにシーンを変更する
    //==================================================
    // 制作日2023/02/17
    // 宮﨑
    private void ChangeSceneWithDecisionKey()
    {
        if (Input.GetKeyDown(decisionKey))
        {//----- if_start -----
            // シーンを[selectStageNum]で指定したステージに変更する
            SceneManager.LoadScene("stage" + m_selectStageNum);
            // 現在使用しているシーンを記録する
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    //==================================================
    // コントローラー操作
    // ※スティック,十字ボタン操作によって指定ステージ番号を変更する
    //==================================================
    // 制作日2023/02/17
    // 宮﨑
    private void ChangeStageNumberWithController()
    {
        // 左スティックを左に倒したときや十字ボタンの左が押されたときに実行
        if (Gamepad.current.leftStick.left.wasPressedThisFrame ||
            Gamepad.current.dpad.left.wasPressedThisFrame)
        {//----- if_start -----
            m_selectStageNum--;
        }//----- if_stop -----
        // 左スティックを右に倒したときや十字ボタンの右が押されたときに実行
        if (Gamepad.current.leftStick.right.wasPressedThisFrame ||
            Gamepad.current.dpad.right.wasPressedThisFrame)
        {//----- if_start -----
            m_selectStageNum++;
        }//----- if_stop -----
    }

    //==================================================
    // キーボード操作
    // ※A,Dキー,左右アローキー操作によって指定ステージ番号を変更する
    //==================================================
    // 制作日2023/02/17
    // 宮﨑
    private void ChangeStageNumberWithKeyboard()
    {
        // 左入力がされたときに実行
        if (Input.GetKeyDown(leftArrow) || Input.GetKeyDown(leftKey))
        {//----- if_start -----
            m_selectStageNum--;
        }//----- if_stop -----
        // 右入力がされたときに実行
        if (Input.GetKeyDown(rightArrow) || Input.GetKeyDown(rightKey))
        {//----- if_start -----
            m_selectStageNum++;
        }//----- if_stop -----
    }

    //==================================================
    //      [NextScene]から飛べるシーンを選ぶ
    //==================================================
    // 作成日2023/02/20    更新日2023/02/21
    // 宮﨑
    private void EnumSceneList()
    {
        if (SceneManager.GetActiveScene().name == titleSceneName)
        {
            switch (titleNextScene)
            {//----- switcj_start -----
             // タイトルからタイトルに移動しようとするとエラーを出す
                case NextSceneList.title:
                    Debug.LogError("同じ名前のシーンに飛ぼうとしています！");
                    break;
                case NextSceneList.select:
                    SceneManager.LoadScene("" + selectSceneName);
                    break;
                case NextSceneList.result:
                    SceneManager.LoadScene("" + resultSceneName);
                    break;
                case NextSceneList.stage:
                    SceneManager.LoadScene(stageName + stageNum);
                    break;
                case NextSceneList.none:
                default:
                    Debug.LogError("[titleNextScene]が正しく設定されていません！");
                    break;
            }//----- switcj_stop -----
        }

        if (SceneManager.GetActiveScene().name == resultSceneName)
        {
            switch (resultNextScene)
            {//----- switcj_start -----
             // リザルトからリザルトに移動しようとするとエラーを出す
                case NextSceneList.title:
                    SceneManager.LoadScene("" + titleSceneName);
                    break;
                case NextSceneList.select:
                    SceneManager.LoadScene("" + selectSceneName);
                    break;
                case NextSceneList.result:
                    Debug.LogError("同じ名前のシーンに飛ぼうとしています！");
                    break;
                case NextSceneList.stage:
                    SceneManager.LoadScene(stageName + stageNum);
                    break;
                case NextSceneList.none:
                default:
                    Debug.LogError("[resultNextScene]が正しく設定されていません！");
                    break;
            }//----- switcj_stop -----
        }
    }

    //=テスト実施=======================================

    // 作成日2023/02/18
    // 宮﨑
    private void ControlForDecisionWithController()
    {
        // ゲームパッドが接続されていなければnull
        if (Gamepad.current == null) return;

        // 決定ボタンが押されたらステージ選択へ
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {//----- if_start -----
            EnumSceneList();
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    // 作成日2023/02/17
    // 宮﨑
    private void ControlForDecisionWithKeyboard()
    {
        // 決定キーが押されたらステージ選択へ
        if (Input.GetKeyDown(decisionKey))
        {//----- if_start -----
            EnumSceneList();
            sceneNameOld = SceneManager.GetActiveScene().name;
        }//----- if_stop -----
    }

    //==================================================

    //==================================================
    //      下記エラーログを表示
    // ※必要項目のstring欄に不備がある場合
    // ※必要項目の次に移動するシーンに不備がある場合
    //==================================================
    // 作成日2023/02/21
    // 宮﨑
    private void ErrorLog()
    {
        // シーンの名前が記入されていない場合にエラーを表示させる
        // タイトルシーン未記入エラー
        if (titleSceneName == "")
        {//----- if_start -----
            Debug.LogError("[titleSceneName]が記入されていません！");
        }//----- if_stop -----

        // ステージセレクト未記入エラー
        if (selectSceneName == "")
        {//----- if_start -----
            Debug.LogError("[selectSceneName]が記入されていません！");
        }//----- if_stop -----

        // リザルトシーン未記入エラー
        if (resultSceneName == "")
        {//----- if_start -----
            Debug.LogError("[resultSceneName]が記入されていません！");
        }

        // 同じシーンに移動するように設定されていた場合エラーを表示させる
        if (titleNextScene == NextSceneList.title ||
            resultNextScene == NextSceneList.result)
        {//----- if_start -----
            Debug.LogError("同じ名前のシーンに飛ぼうとしています！");
        }//----- if_stop -----

        if (titleNextScene == NextSceneList.none)
        {//----- if_start -----
            Debug.LogError("[titleNextScene]が移動するシーンが設定されていません！");
        }//----- if_stop -----

        if (resultNextScene == NextSceneList.none)
        {//----- if_start -----
            Debug.LogError("[resultNextScene]が設定されていません！");
        }//----- if_stop -----
    }

    private void Update()
    {
        //=テスト実施=======================================

        // 今いるシーンの名前が[title]または[result]なら専用の操作に変更
        if (SceneManager.GetActiveScene().name == "title" ||
            SceneManager.GetActiveScene().name == "result")
        {//----- if_start -----
            // ゲームパッドが接続されていない場合はキーボード操作にする
            // ゲームパッドが接続されている場合はコントローラー操作にする
            if (Gamepad.current == null)
            {//----- if_start -----
                ControlForDecisionWithKeyboard();
            }//----- if_stop -----
            else
            {//----- else_start -----
                ControlForDecisionWithController();
            }//----- else_stop -----
        }//----- if_stop -----


        // ※[m_selectStageNum]の操作と同時に入力すると
        // 　ステージシーンから離脱できないバグが発生します
        // 今いるシーンの名前が[stage(selectStageNum)]なら専用の操作に変更
        if (SceneManager.GetActiveScene().name == stageName + m_selectStageNum)
        {//----- if_start -----

            // 決定ボタンが押されたらステージ選択へ
            if (gameClear)
            {//----- if_start -----
                SceneManager.LoadScene(resultSceneName);
                sceneNameOld = SceneManager.GetActiveScene().name;
            }//----- if_stop -----

        }//----- if_stop -----

        //==================================================

        // 今いるシーンの名前が[select]または[SampleScene]なら専用の操作に変更
        if (SceneManager.GetActiveScene().name == "select" ||
            SceneManager.GetActiveScene().name == "SampleScene")
        {//----- if_start -----
            // ゲームパッドが接続されていない場合はキーボード操作にする
            // ゲームパッドが接続されている場合はコントローラー操作にする
            if (Gamepad.current == null)
            {//----- if_start -----
                // 決定キーでシーンを切り替える
                ChangeSceneWithDecisionKey();
                // 左右の矢印,ADキー操作でステージ番号を変更する
                ChangeStageNumberWithKeyboard();
            }//----- if_stop -----
            else
            {//----- else_start -----
                // 決定ボタンでシーンを切り替える
                ChangeSceneWithControllerDecisionButton();
                // スティック,十字キー操作でステージ番号を変更する
                ChangeStageNumberWithController();
                // スティックが押されている状態を確認する
                StageSelectWithController();
            }//----- else_stop -----
        }//----- if_stop -----

        // 現在使用しているシーン名を書き換える
        sceneNameNow = SceneManager.GetActiveScene().name;

        // コントローラ用サンプルシーン遷移コマンド
        HideCommandRoom();

        //=テスト実施=======================================

        // 現在のシーン名が[select]ではないとき
        // ZR,ZLが同時に押されたならステージ選択画面に遷移
        if (SceneManager.GetActiveScene().name != "select")
        {//----- if_start -----
            SelectSceneCommandRoom();
        }//----- if_stop -----

        // 現在のシーン名が[select]のとき
        // Bボタン,Sキーが押されたなら[title]へ遷移
        if (SceneManager.GetActiveScene().name == "select")
        {//----- if_start -----
            if (Gamepad.current == null)
            {//----- if_start -----
                if (Input.GetKeyDown(KeyCode.S))
                {
                    SceneManager.LoadScene("title");
                    sceneNameOld = SceneManager.GetActiveScene().name;
                }
            }//----- if_stop -----
            else
            {//----- else_start -----
                if(Gamepad.current.buttonEast.wasPressedThisFrame)
                {
                    SceneManager.LoadScene("title");
                    sceneNameOld = SceneManager.GetActiveScene().name;
                }
            }//----- else_stop -----
        }//----- if_stop -----

        //==================================================

        // ステージ番号選択時の設定
        StopOrRepeatOfTheStageSelectNum();

        //----- テスト段階で一時停止中 -----
        //StageSelectWithController();
        //ChangeSelectPosition();

        // エラーの表示
        ErrorLog();
    }
}
