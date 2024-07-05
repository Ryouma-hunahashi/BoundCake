using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

//===========================================
// リザルトの処理
//===========================================

public class UI_Result : MonoBehaviour
{
    public string resultStage; // 現在のリザルトステージを取得

    // ============================
    // 別スクリプト参照
    [SerializeField] OtherEffectManager effect; // effectをつけるため
    [SerializeField] FadeCamera fadeCamera; // カメラを暗転させるため
    [SerializeField] private UIAudio audio;
    // ============================

    //============================================
    // 必要なImage画像（ここがNULLだとエラーが出る）
    [SerializeField] private Image UI_parfait_top; // パフェの一番上
    [SerializeField] private Image UI_parfait_mid; // パフェ中間
    [SerializeField] private Image UI_parfait_btm; // パフェ一番下
    [SerializeField] private Image UI_nextStage;   // 次進むための画像
    [SerializeField] private Image UI_selectStage; // ステージセレクト入れる用の画像
    [SerializeField] private Image ParfaitImage;   // パフェを作るようの画像
    [SerializeField] private Image UI_backimage; // ステージによって背景が変わるときに用いる変数
    [SerializeField] private Image UI_blackoutimage; // シーン切り替えの時に使う暗転用画像
    [SerializeField] private Image UI_panel_top;    // リザルトパネル上の部分
    [SerializeField] private Image UI_panel_mid;    // リザルトパネル真ん中の部分
    [SerializeField] private Image UI_panel_btm;    // リザルトパネル下の部分
    [SerializeField] private Image UI_star_left;    // 星の左画像
    [SerializeField] private Image UI_star_mid;     // 星の真ん中画像
    [SerializeField] private Image UI_star_right;   // 星の右画像
    //===========================================

    // アニメーション
    [SerializeField] private Animator nextStageAnim;
    [SerializeField] private Animator selectStageAnim;

    // リザルトの時にステージの番号取得
    public byte worldnum;
    public byte stagenum;

    // パフェの座標位置取得
    private RectTransform top;
    private RectTransform mid;
    private RectTransform btm;

    // 選択されているときに透明度を変更する
    [SerializeField, Range(0f, 1f)]
    private float alphaimage;

    // パフェアイテムの落下速度
    [SerializeField] private float p_move = 5.0f;
    [SerializeField] private float move_x = 0.1f;
    [SerializeField] private float P_change = 10f;
    [SerializeField] private float weight = 3.0f; // effectを一定時間でけす
    private float time;


    // 
    bool nextcheck = true;
    bool selectcheck = false;
    bool btm_effect = true;
    bool mid_effect = true;
    bool top_effect = true;
    bool shine_effect = true;
    bool nextstagecheck = false;
    bool selectstagecheck = false;
    bool audiocheck;

    // 連打対策用
    private WaitForSeconds wait = new WaitForSeconds(0.8f);
    private bool buttonEnabled = true;

    //=============================
    // private RectTransform pafait;
    // Start is called before the first frame update
    void Start()
    {

        // 現在のリザルトの名前を取得
        resultStage = SceneManager.GetActiveScene().name;
        // リザルトシーンならば、このキャンバスを表示する
        if (resultStage == "result1" || resultStage == "result2" ||
            resultStage == "result3" || resultStage == "result4" ||
            resultStage == "result5")
        {
            Result_Manager.instance.Canvas.enabled = true;
        }
        else
        {
            Result_Manager.instance.Canvas.enabled = false;
        }

        // ここにステージ番号を保存
        worldnum = Result_Manager.instance.nowWorld;
        stagenum = Result_Manager.instance.nowStage;

        // リザルトのステージによって背景を切り替える
        Check_backGround();

        // 前のステージでのパフェ取得をチェックする
        CheckParfait();

        // 効果音取得
        audio = GetComponent<UIAudio>();
        // 現在のパフェのアイテムの座標位置取得
        top = UI_parfait_top.rectTransform;
        mid = UI_parfait_mid.rectTransform;
        btm = UI_parfait_btm.rectTransform;
        // 今のリザルトのscene名を取得
        //resultStage = Stage_Manager.StageInfo.stageName;

        

        // ================初期設定==================

        // パフェの中身をリセットする
        ParfaitImage.sprite = Resources.Load<Sprite>("glass");

        // 次のステージへとセレクトステージへの画像をリセットする
        UI_nextStage.enabled = false;
        UI_selectStage.enabled = false;
        //UI_blackoutimage.color = new Color(1.0f, 1.0f, 1.0f, 0f);

        btm_effect = true;
        mid_effect = true;
        top_effect = true;
        shine_effect = true;
        audiocheck = false;
        
        nextStageAnim = UI_nextStage.gameObject.GetComponent<Animator>();
        selectStageAnim = UI_selectStage.gameObject.GetComponent<Animator>();

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // 縦方向の入力を受けつける
        float inputHorizontal = Input.GetAxisRaw("Horizontal");
        bool downButton = Gamepad.current.dpad.down.wasPressedThisFrame; // 十字キー下の入力情報
        bool upButton = Gamepad.current.dpad.up.wasPressedThisFrame; // 十字キー上の入力情報
        bool eastButton = Gamepad.current.buttonEast.wasPressedThisFrame;  // 東ボタンの入力情報
        bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame; // 南ボタンの入力情報

        // ========================================
        // 一番下のアイテムがあるとき
        if (UI_parfait_btm.enabled)
        {
            if (btm_effect)
            {
                effect.StartPafe();
                btm_effect = false;
            }
            //　座標を少しずつ下に持っていく
            btm.position -= new Vector3(-move_x, p_move, 0);


            if (btm.position.y <= P_change)
            {
                ParfaitImage.sprite = Resources.Load<Sprite>("p2");
                UI_panel_top.sprite = Resources.Load<Sprite>("P1_board");
                UI_star_left.sprite = Resources.Load<Sprite>("star");

                Invoke("btmfalse", 0.5f);
                effect.StopPafe();
            }
        }
        // 真ん中のアイテムがあるとき
        if (!UI_parfait_btm.enabled && UI_parfait_mid.enabled)
        {
            if (mid_effect)
            {
                effect.StartPafe();
                mid_effect = false;
            }

            // アイテムを下に誘導する
            mid.position -= new Vector3(0, p_move, 0);

            // 一定の下の位置にいったら
            if (mid.position.y <= P_change)
            {
                // パフェの画像を切り替える
                Debug.Log(ParfaitImage.sprite.name);
                //　下のアイテムが入っている画像の時
                if (ParfaitImage.sprite.name == "p2")
                {
                    Debug.Log("btm true mid true");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p5");
                    UI_panel_mid.sprite = Resources.Load<Sprite>("P2_board");
                    UI_star_mid.sprite = Resources.Load<Sprite>("star");

                }
                // 下のアイテムが入っていない画像の時
                else if (ParfaitImage.sprite.name == "glass")
                {
                    Debug.Log("btm false mid true");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p3");
                    UI_panel_mid.sprite = Resources.Load<Sprite>("P2_board");
                    UI_star_left.sprite = Resources.Load<Sprite>("star");

                }
                effect.StopPafe();
                Invoke("midfalse", 0.5f);
            }
        }
        // top部分が動くとき
        if (!UI_parfait_btm.enabled && !UI_parfait_mid.enabled && UI_parfait_top.enabled)
        {
            if (top_effect)
            {
                effect.StartPafe();
                top_effect = false;
            }

            top.position -= new Vector3(move_x, p_move, 0);

            if (top.position.y <= P_change)
            {
                // 全部取得している
                if (ParfaitImage.sprite.name == "p5")
                {
                    Debug.Log("b&m true top true");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p8");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_right.sprite = Resources.Load<Sprite>("star");

                }
                // btm false mid trueのとき
                else if (ParfaitImage.sprite.name == "p3")
                {
                    ParfaitImage.sprite = Resources.Load<Sprite>("p7");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_mid.sprite = Resources.Load<Sprite>("star");

                }
                // btm true  mid false のとき 
                else if (ParfaitImage.sprite.name == "p2")
                {
                    ParfaitImage.sprite = Resources.Load<Sprite>("p6");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_mid.sprite = Resources.Load<Sprite>("star");

                }
                // btm midがfalseのとき
                else if (ParfaitImage.sprite.name == "glass")
                {
                    Debug.Log("btm mid false");
                    ParfaitImage.sprite = Resources.Load<Sprite>("p4");
                    UI_panel_btm.sprite = Resources.Load<Sprite>("P3_board");
                    UI_star_left.sprite = Resources.Load<Sprite>("star");

                }
                effect.StopPafe();
                Invoke("topfalse", 0.5f);
            }
        }
        // パフェ全部移動し終わったとき
        if (!UI_parfait_btm.enabled && !UI_parfait_mid.enabled && !UI_parfait_top.enabled)
        {
            if (shine_effect)
            {
                audio.MapSetSound();
                effect.StartFlash();
                shine_effect = false;
            }
            UI_nextStage.enabled = true;
            UI_selectStage.enabled = true;
           
        }
        //=========================================

        Debug.Log(fadeCamera.isFadeOut);

        // 両方の画像が出ているとき
        if (UI_selectStage.enabled && UI_nextStage.enabled)
        {
            // 上にカーソルがあるとき
            if (nextcheck)
            {
                // 下の画像を少し透明にする
                //UI_nextStage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                //UI_selectStage.color = new Color(1.0f, 1.0f, 1.0f, alphaimage);
                nextStageAnim.SetBool("lockOn", true);
                selectStageAnim.SetBool("lockOn", false);
                // したボタン押したとき 連打禁止する為二つの条件でしか反応しないようにする
                if (southButton && buttonEnabled)
                {
                    // フェードアウト
                    fadeCamera.isFadeOut = true;
                    nextstagecheck = true;
                    if (buttonEnabled)
                    {
                        // 次の番号の添え字に1加算する
                        stagenum++;
                        // ステージの端っこに行ったら、次のステージの初めからにする
                        if (stagenum > 3)
                        {
                            worldnum++;
                            stagenum = 0;
                        }
                        // 他の所にパフェを取得したかどうかを保存したのでゲットしたのをいったんリセットする
                        CheckParfaitInit();
                        // 次のステージの添え字を保存する
                        Result_Manager.instance.nowWorld = worldnum;
                        Result_Manager.instance.nowStage = stagenum;
                        Debug.Log(worldnum);
                        Debug.Log(stagenum);
                        // ワールドロックをfalseにする
                        Stage_Manager.instanse.worldInformation[worldnum].worldLock = false;
                        // ボタンを押せないようにする（二段階で押せないようにする）
                        buttonEnabled = false;
                    }
                }
                // フェードアウトがおわったとき
                if (!fadeCamera.isFadeOut && nextstagecheck)
                {
                    // シーン切り替え音
                    audio.ChangeSceneSound();
                    // 一瞬だけフレーム遅延をする
                    StartCoroutine("momentFlame");
                    // 次のステージへシーン遷移する
                    S_Manager.instance.SceneChange(Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].sceneName);
                }
            }
            // 下にカーソルがあるとき
            if (selectcheck)
            {
                // 上の画像を少し透明にする
                //UI_selectStage.color = new Color(1.0f, 1.0f, 1.0f, 1.0f);
                //UI_nextStage.color = new Color(1.0f, 1.0f, 1.0f, alphaimage);
                nextStageAnim.SetBool("lockOn", false);
                selectStageAnim.SetBool("lockOn", true);

                // ×ボタンを押したとき
                if (southButton)
                {
                    // フェードアウトする
                    selectstagecheck = true;
                    fadeCamera.isFadeOut = true;

                }
                // フェードアウトが終わったら
                if (!fadeCamera.isFadeOut && selectstagecheck)
                {
                    // シーン切り替え音
                    audio.ChangeSceneSound();
                    // 一瞬だけフレーム遅延をする
                    StartCoroutine("momentFlame");
                    StartCoroutine("momentFlame");
                    // ステージセレクトへ
                    SceneManager.LoadScene("ContentsSelect");
                }
            }
            // 上にスティックを傾ける＆十字上キーを押したとき
            if (inputHorizontal > 0.55f || upButton)
            {
                if (audiocheck)
                {
                    // ここに効果音
                    audio.MoveCursorSound();
                    audiocheck = false;
                }
                // 上の画像にカーソルを合わせる
                nextcheck = true;    
                selectcheck = false;
            }
            else if (inputHorizontal < -0.55f || downButton)
            {
                if (!audiocheck)
                {
                    // ここに効果音
                    audio.MoveCursorSound();
                    audiocheck = true;
                }
                // 下の画像にカーソルを合わせる
                selectcheck = true;
                nextcheck = false;
            }
            //時間計測を開始する
            time += Time.deltaTime;

            // 一定時間がたったら
            if (time > weight)
            {
                // キラキラのエフェクトを消す
                effect.StopFlash();
            }
        }
    }

    
    // 各リザルトに応じて背景の画像を切り替える
    void Check_backGround()
    {
        Debug.Log("test");
        if (resultStage == "result1")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back1");
        }
        if (resultStage == "result2")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back2");
        }
        if (resultStage == "result3")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back3");
        }
        if (resultStage == "result4")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back4");
        }
        if (resultStage == "result5")
        {
            UI_backimage.sprite = Resources.Load<Sprite>("back5");
        }
    }

    // 各ステージでどのアイテムを取ったか調べる関数
    void CheckParfait()
    {
        if (Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].parfait.top)
        {
            UI_parfait_top.enabled = true;
        }
        else
        {
            UI_parfait_top.enabled = false;
        }
        if (Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].parfait.mid)
        {
            UI_parfait_mid.enabled = true;
        }
        else
        {
            UI_parfait_mid.enabled = false;
        }
        if (Stage_Manager.instanse.worldInformation[worldnum].stageInformation[stagenum].parfait.btm)
        {
            UI_parfait_btm.enabled = true;
        }
        else
        {
            UI_parfait_btm.enabled = false;
        }

    }

    // リセットする関数
    void CheckParfaitInit()
    {
        Result_Manager.instance.getParfait.top = false;
        Result_Manager.instance.getParfait.mid = false;
        Result_Manager.instance.getParfait.btm = false;
    }

    void btmfalse()
    {
        UI_parfait_btm.enabled = false;
    }
    void midfalse()
    {
        UI_parfait_mid.enabled = false;
    }
    void topfalse()
    {
        UI_parfait_top.enabled = false;
    }
  IEnumerable momentFlame()
    {
        yield return null;
    }
}