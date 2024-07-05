using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

//======================= ver2.0 ============================
// ステージセレクト時に、選んだステージの画像をズームするようにしました。
// それ関連で、WorldStatus構造体の要素を追加しました。
//===========================================================
// 実行日　2023/05/03   更新日2023/05/21
// 高田　宮﨑

//[System.Serializable]
//public struct WorldStatus
//{
//    [System.Serializable]
//    public struct StageStatus
//    {
//        // ズームを行うか
//        public enum ZoomSet
//        {
//            ON,
//            OFF,
//        }

//        [Tooltip("場所の名前")]
//        public string stageName;
//        [Tooltip("シーン名")]
//        public string sceneName;
//        [Tooltip("カメラを固定する座標\nImageからの相対座標")]
//        public Vector3 cameraZoomPos;
//        [Tooltip("対応するImageにズームを行うか否か")]
//        public ZoomSet zoomSet;
//    }


//    [Tooltip("地図の名前")]
//    public string worldName;            // 地図名を設定する
//    [Tooltip("場所の情報")]
//    public List<StageStatus> stages;    // 場所の情報を設定する

//}

// WorldStatusに対応するImageの情報格納用
[System.Serializable]
public struct MapImages
{
    public Image image;         // Image格納用
    public Animator animator;   // ImageのAnimator格納用
    public Vector3 transPos;    // Imageのposition格納用
    public testMSelect mSelect;
}

[System.Serializable]
public struct SelectorImages
{
    public Image image;
    public Animator animator;
    public Vector3 transPos;
}

//==============================
//      ステージセレクト
//==============================
// 作成日2023/04/27    更新日2023/05/21
// 宮﨑　高田
public class StageSelector : MonoBehaviour
{
    // データ送り先の設定
    //[SerializeField] private S_Manager s_Manager;

    // 選択位置の情報
    public byte worldNum = 1;
    public byte stageNum = 0;

    // 位置変更速度の設定
    [SerializeField] private byte selectDelayTime = 20;
    [SerializeField] private float zoomSpeed = 30.0f;
    private bool nowDelayTime;  // 待機状態

    // 地図の情報
    //public List<WorldStatus> worlds = new List<WorldStatus>();

    public List<Stage_Manager.WorldInfo> mapInfo;

    [Header("一つ前のシーンの名前")]
    [Tooltip("ズームしていない状態で東ボタンを押すとシーン遷移")]
    [SerializeField] private string backSceneName = "";
    [Header("次のセレクト画面")]
    [SerializeField] private string rightSceneName = "";
    [Header("前のセレクト画面")]
    [SerializeField] private string leftSceneName = "";

    // このオブジェのRectTransform格納用
    private RectTransform canvasTransform = null;

    // stageImage関連の情報格納用
    [SerializeField] private List<MapImages> mapImages = new List<MapImages>();
    [SerializeField] private List<SelectorImages> selectorImages = new List<SelectorImages>();

    // グレースケール変換用カーブ
    [SerializeField] private int grayColor = 25;
    private GameObject cameraObj = null;

    private Vector3 defaultCameraPos;

    private bool cameraZoomFlag = false;
    private bool nowZoomOut = false;
    private bool nowZoom = false;
    private bool sceneMoveSoundSet = true;

    [SerializeField] private AnimationCurve fadeOutCurve;
    private Image fadeOutImage;
    private float elapsedTime;
    [SerializeField] private float fadeOutTime = 1.0f;
    private bool nowfadeOut = false;
    private bool nowStageSet = false;
    Image stageSettingImage;
    private byte settingNum = 0;

    [SerializeField] private Color defaultColor;
    [SerializeField] private Color selectColor;


    UIAudio audio;

    //==============================
    //      選択位置変更のディレイ
    // 戻り値 : 無し
    //　引数  : 無し
    //==============================
    // 作成日2023/04/27
    // 宮﨑
    private IEnumerator ChangeDelay()
    {
        // 待機状態に変更する
        nowDelayTime = true;

        // 設定されたフレーム数待機する
        for (byte i = 0; i < selectDelayTime; i++)
        {
            // 1フレーム遅延させる
            yield return null;
        }//----- for_stop -----

        // 非待機状態に変更する
        nowDelayTime = false;
    }

    private void Start()
    {
        // データの送り先を探す
        //s_Manager = GetComponent<S_Manager>();

        mapInfo = Stage_Manager.instanse.worldInformation;

        worldNum = 1;
        stageNum = 0;

        // 子オブジェクトの数を取得
        byte childCnt = (byte)this.transform.childCount;

        // 一度リストを開放する
        mapImages.Clear();

        canvasTransform = GetComponent<RectTransform>();
        if (canvasTransform == null)
        {
            Debug.LogError("RectTransformがコンポーネントされていません。");
        }

        cameraObj = GameObject.FindWithTag("MainCamera");
        if (cameraObj == null)
        {
            Debug.LogError("カメラが見つかりません");
        }

        // 子オブジェクトの<Image>をリスト内に格納
        for (byte i = 0; i < childCnt; i++)
        {
            // リスト内に子オブジェクトの<Image>を追加格納する。
            mapImages.Add(SetImages(i));
            if (mapInfo[i].worldLock)
            {
                mapImages[i].image.color = new Color(grayColor/255f,grayColor/255f,grayColor/255f,1);
            }


        }//----- for_stop -----
        for (byte i = 0; i < cameraObj.transform.GetChild(0).childCount; i++)
        {
            // リスト内にカメラ孫オブジェクトの<Image>を追加格納する。
            selectorImages.Add(SetSelectorImages(i));
        }

        audio = GetComponent<UIAudio>();



        // 初期カメラ座標を代入
        if (defaultCameraPos == null)
        {
            defaultCameraPos = cameraObj.transform.position;
        }

        if (mapImages[worldNum].mSelect != null)
        {
            mapImages[worldNum].mSelect.Enter();
        }
        else
        {
            mapImages[worldNum].image.color = selectColor;
        }

        fadeOutImage = cameraObj.transform.GetChild(1).GetChild(0).GetComponent<Image>();


    }




    private void FixedUpdate()
    {
        // ゲームパッドが接続されていないなら処理を抜ける
        if (Gamepad.current == null) return;

        // ローカル変数の作成 & 初期化開始 ----------------

        // 移動方向の入力情報を取得する
        float inputVertical = Input.GetAxisRaw("Vertical");
        float inputHorizontal = Input.GetAxisRaw("Horizontal");

        //// スティックの入力情報を取得する ----- Vertical
        //bool Lstick_Up = Gamepad.current.leftStick.up.wasPressedThisFrame;
        //bool Lstick_Down = Gamepad.current.leftStick.down.wasPressedThisFrame;

        //// スティックの入力情報を取得する ----- Horizontal
        //bool Lstick_Left = Gamepad.current.leftStick.left.wasPressedThisFrame;
        //bool Lstick_Right = Gamepad.current.leftStick.right.wasPressedThisFrame;

        // ボタンの入力情報を取得する
        bool southButton = Gamepad.current.buttonSouth.wasPressedThisFrame; // 南ボタンの入力情報
        bool eastButton = Gamepad.current.buttonEast.wasPressedThisFrame;  // 東ボタンの入力情報

        bool leftButton = Gamepad.current.leftShoulder.wasPressedThisFrame || Gamepad.current.leftTrigger.wasPressedThisFrame;
        bool rightButton = Gamepad.current.rightShoulder.wasPressedThisFrame || Gamepad.current.rightTrigger.wasPressedThisFrame;

        // ローカル変数の作成 & 初期化完了 ----------------

        //==============================
        // ワールド位置をずらす
        //==============================
        // 待機状態ではないときに[L]ボタンが押されたなら
        if (leftButton && !nowDelayTime && !cameraZoomFlag && !nowZoomOut && !nowZoom)
        {
            // シーンの名前が登録されていれば
            if (leftSceneName != "")
            {
                // シーン遷移する
                S_Manager.instance.SceneChange(leftSceneName);

            }//-----if_stop-----

            //// 地図番号が最低値ではないなら
            //if (worldNum != 0)
            //{
            //    // 現在のアニメーションを切る
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", false);

            //    worldNum--;
            //    stageNum = 0;

            //    // 次の番号のアニメーションを起動
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", true);
            //    // 位置変更を一時的に停止する
            //    StartCoroutine(ChangeDelay());

            //}//----- if_stop -----

        }//----- if_stop -----
        // 待機状態ではないときに[R]ボタンが押されたなら
        else if (rightButton && !nowDelayTime && !cameraZoomFlag && !nowZoomOut && !nowZoom)
        {
            // シーンの名前が登録されていれば
            if (rightSceneName != "")
            {
                // シーン遷移する
                S_Manager.instance.SceneChange(rightSceneName);

            }//-----if_stop-----



            //// 地図番号が最大値ではないなら
            //if (worldNum < (byte)worlds.Count - 1)
            //{
            //    // 現在のアニメーションを切る
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", false);

            //    worldNum++;
            //    stageNum = 0;

            //    // 次の番号のアニメーションを起動
            //    stageImages[GetImageNumber()].animator.SetBool("lockOn", true);

            //}//----- if_stop -----

            //// 位置変更を一時的に停止する
            //StartCoroutine(ChangeDelay());

        }//----- elseif_stop -----

        //==============================
        // ステージ,ワールド位置をずらす
        //==============================
        // 待機状態ではないときに横方向の入力がされたなら
        if (!nowDelayTime && (inputHorizontal != 0) && !nowZoom && !nowfadeOut)
        {
            // 指定位置の変更
            ChangeHorizontalPosition(inputHorizontal);    // 横入力を送る

        }//----- if_stop -----
        //else if(!nowDelayTime && (inputVertical != 0) && !nowZoom)
        //{
        //    // 指定位置の変更
        //    ChangeVerticalPosition(inputVertical);        // 縦入力を送る
        //}

        // 決定ボタンが入力されたなら実行
        if (southButton)
        {
            switch (mapInfo[worldNum].zoomSet)
            {
                case Stage_Manager.WorldInfo.ZoomSet.ON:
                    if (cameraZoomFlag && !nowZoom && !nowfadeOut)
                    {
                        audio.StartSound();
                        // --------------------------ゲーム開始時のサウンド--------------
                        nowfadeOut = true;

                    }//-----if_stop-----
                    else
                    {
                        // カメラを動かす
                        audio.BookMarkSound();
                        nowZoom = true;
                        cameraZoomFlag = true;

                    }//-----else_stop-----
                    break;
                case Stage_Manager.WorldInfo.ZoomSet.OFF:
                    if (!nowfadeOut)
                    {
                        nowfadeOut = true;
                    }
                    break;
            }

        }//----- if_stop -----

        if (eastButton)
        {
            if (!nowZoom && cameraZoomFlag)
            {
                //--------------------------栞のサウンド--------------------------
                audio.BookMarkSound();
                for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                {
                    selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", false);
                }
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                stageNum = 0;
                cameraZoomFlag = false;
                nowZoomOut = true;
            }
            else if (!cameraZoomFlag && !nowZoom && !nowZoomOut)
            {
                if (backSceneName != "")
                {
                    S_Manager.instance.SceneChange(backSceneName);
                }
            }
        }

        if (nowZoom)
        {
            cameraObj.transform.position = Vector3.MoveTowards(cameraObj.transform.position, mapImages[worldNum].transPos + mapInfo[worldNum].cameraZoomPos, zoomSpeed);

            // カメラが目標に達したとき、ズーム中フラグを切る。
            if (cameraObj.transform.position == mapImages[worldNum].transPos + mapInfo[worldNum].cameraZoomPos)
            {
                for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                {
                    if (!mapInfo[worldNum].stageInformation[i].stageLock)
                    {
                        selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", true);
                    }
                }
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                nowZoom = false;

            }//-----if_stop-----

        }//-----if_stop-----
        else if (nowZoomOut)
        {
            cameraObj.transform.position = Vector3.MoveTowards(cameraObj.transform.position, defaultCameraPos, 30);

            // カメラが目標に達したとき、ズームアウト中フラグを切る。
            if (cameraObj.transform.position == defaultCameraPos)
            {

                nowZoomOut = false;
            }
        }

        if (nowfadeOut)
        {
            var color = fadeOutImage.color;
            color.a = fadeOutCurve.Evaluate(elapsedTime / fadeOutTime);
            fadeOutImage.color = color;

            elapsedTime += Time.deltaTime;
            if (elapsedTime >= fadeOutTime)
            {
                elapsedTime = fadeOutTime;
                if (sceneMoveSoundSet)
                {
                    //----------------シーン遷移の音
                    audio.ChangeSceneSound();
                    sceneMoveSoundSet = false;
                }
                // 音が終わってから
                if (!audio.UISource.isPlaying)
                {
                    sceneMoveSoundSet = true;
                    nowfadeOut = false;
                    elapsedTime = 0;
                    Debug.Log((worldNum + 1) + " - " + (stageNum + 1) + " に飛びますっ！");

                    Result_Manager.instance.nowWorld = worldNum; // ワールド番号を保存
                    Result_Manager.instance.nowStage = stageNum; // ステージ番号を保存

                    // シーン遷移を行う
                    S_Manager.instance.SceneChange(mapInfo[worldNum].stageInformation[stageNum].sceneName);
                }
            }
        }


        if (nowStageSet)
        {
            var color = stageSettingImage.color;
            color.r += 5/255f;
            color.g += 5 / 255f;
            color.b += 5 / 255f;
            if (color.r >=1)
            {
                audio.MapSetSound();
                color.r = 1;
                color.g = 1;
                color.b = 1;
                nowStageSet = false;
            }
            stageSettingImage.color = color;
        }
        else
        {
            for (byte i = 0; i < Stage_Manager.instanse.worldInformation.Count; i++)
            {
                if (Stage_Manager.instanse.worldInformation[i].worldLock !=
                    Stage_Manager.instanse.worldInformation[i].worldLockLog)
                {
                    if (Stage_Manager.instanse.worldInformation[i].worldLockLog)
                    {
                        Stage_Manager.instanse.worldInformation[i].worldLockLog = Stage_Manager.instanse.worldInformation[i].worldLock;
                        mapInfo[i].worldLock = Stage_Manager.instanse.worldInformation[i].worldLockLog;
                        mapInfo[i].worldLockLog = mapInfo[i].worldLock;
                        Debug.Log("通れやぁ！！");
                        mapImages[i].image.enabled = true;
                        stageSettingImage = mapImages[i].image;
                        var color = stageSettingImage.color;
                        color.r = grayColor/255f;
                        color.g = grayColor/255f;
                        color.b = grayColor/255f;
                        stageSettingImage.color = color;

                        nowStageSet = true;
                        settingNum = i;
                        break;
                    }
                }
            }
        }

    }

    //==============================
    //      ステージセレクト
    // ※横移動のみの実装
    // 戻り値 : 無し
    //　引数  : _inputDirection 横入力情報
    //==============================
    // 作成日2023/04/27
    // 宮﨑
    private void ChangeHorizontalPosition(float _inputDirection)
    {
        // 入力方向が左なら
        if (_inputDirection < -0.55f)
        {
            if (cameraZoomFlag)
            {
                Debug.Log("左方向へ位置の変更を開始します");

                // ステージ番号が最低値未満ではないなら
                if (stageNum > 0)
                {
                    if (!mapInfo[worldNum].stageInformation[stageNum - 1].stageLock)
                    {
                        //------------------------------------------栞サウンド---------------
                        audio.BookMarkSound();
                        // 現在のアニメーションを切る

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                        // ステージ番号を減少させる
                        stageNum--;
                        // 次の番号のアニメーションを起動

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                    }
                }//----- if_stop -----
                else
                {
                    // 地図番号が最低値ではないなら
                    if (worldNum != 0)
                    {
                        if (!mapInfo[worldNum - 1].worldLock)
                        {
                            //------------------------------------------栞サウンド---------------
                            audio.BookMarkSound();
                            // 現在のアニメーションを切る
                            for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            {
                                selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", false);
                            }
                            mapImages[worldNum].animator.SetBool("lockOn", false);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Exit();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = defaultColor;
                            }
                            // 地図番号を減少させ,ステージ番号を最大値にする
                            worldNum--;
                            stageNum = (byte)(mapInfo[worldNum].stageInformation.Count - 1);
                            // 次の番号のアニメーションを起動
                            //for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            //{
                            //    selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", true);
                            //}
                            mapImages[worldNum].animator.SetBool("lockOn", true);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Enter();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = selectColor;
                            }
                            ZoomMove();
                        }
                    }//----- if_stop -----

                }//----- else_stop -----
            }
            else
            {
                if (worldNum > 0)
                {
                    if (!mapInfo[worldNum - 1].worldLock)
                    {
                        //------------------------------------カーソル移動サウンド----------------------
                        audio.UISource.Stop();
                        audio.MoveCursorSound();
                        mapImages[worldNum].animator.SetBool("lockOn", false);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Exit();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = defaultColor;
                        }
                        worldNum--;
                        mapImages[worldNum].animator.SetBool("lockOn", true);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Enter();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = selectColor;
                        }
                    }
                }
            }
            // カメラ位置を移動させる。



            // 位置変更を一時的に停止する
            StartCoroutine(ChangeDelay());

        }//----- if_stop -----
        // 入力方向が右なら
        else if (_inputDirection > 0.55f)
        {
            Debug.Log("右方向へ位置の変更を開始します");
            if (cameraZoomFlag)
            {
                // ステージ番号が最大値未満なら
                if (stageNum < (byte)mapInfo[worldNum].stageInformation.Count - 1)
                {
                    if (!mapInfo[worldNum].stageInformation[stageNum + 1].stageLock)
                    {
                        //------------------------------------------栞サウンド---------------
                        audio.BookMarkSound();

                        // 現在のアニメーションを切る

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                        // ステージ番号を上昇させる
                        stageNum++;
                        // 次の番号のアニメーションを起動

                        selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                    }
                }//----- if_stop -----
                else
                {
                    // 地図番号が最大値ではないなら
                    if (worldNum < (byte)mapInfo.Count - 1)
                    {
                        if (!mapInfo[worldNum + 1].worldLock)
                        {
                            //------------------------------------------栞サウンド---------------
                            audio.BookMarkSound();

                            // 現在のアニメーションを切る
                            for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            {
                                selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", false);
                            }
                            mapImages[worldNum].animator.SetBool("lockOn", false);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Exit();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = defaultColor;
                            }

                            // 地図番号を上昇させ,ステージ番号を最低値にする
                            worldNum++;
                            stageNum = 0;
                            // 次の番号のアニメーションを起動
                            //for (byte i = 0; i < mapInfo[worldNum].stageInformation.Count; i++)
                            //{
                            //    selectorImages[GetImageNumber() - stageNum + i].animator.SetBool("worldLockOn", true);
                            //}
                            mapImages[worldNum].animator.SetBool("lockOn", true);
                            selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
                            if (mapImages[worldNum].mSelect != null)
                            {
                                mapImages[worldNum].mSelect.Enter();
                            }
                            else
                            {
                                mapImages[worldNum].image.color = selectColor;
                            }
                            ZoomMove();
                        }
                    }//----- if_stop -----

                }//----- else_stop -----
            }
            else
            {
                if (worldNum < (byte)mapInfo.Count - 1)
                {
                    if (!mapInfo[worldNum + 1].worldLock)
                    {
                        //-----------------------------------カーソル移動サウンド-------------
                        audio.UISource.Stop();
                        audio.MoveCursorSound();
                        mapImages[worldNum].animator.SetBool("lockOn", false);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Exit();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = defaultColor;
                        }
                        worldNum++;
                        mapImages[worldNum].animator.SetBool("lockOn", true);
                        if (mapImages[worldNum].mSelect != null)
                        {
                            mapImages[worldNum].mSelect.Enter();
                        }
                        else
                        {
                            mapImages[worldNum].image.color = selectColor;
                        }
                    }
                }
            }
            // カメラ位置を移動させる。


            // 位置変更を一時的に停止する
            StartCoroutine(ChangeDelay());

        }//----- elseif_stop -----

    }

    //==============================
    //      ステージセレクト
    // ※縦移動のみの実装
    // 戻り値 : 無し
    //　引数  : _inputDirection 縦入力情報
    //==============================
    // 作成日2023/04/27
    // 宮﨑
    private void ChangeVerticalPosition(float _inputDirection)
    {
        // 入力方向が左なら
        if (_inputDirection < -0.55f)
        {
            Debug.Log("下方向へ位置の変更を開始します");

            // ステージ番号が最低値未満ではないなら
            if (worldNum < (byte)mapInfo.Count - 1)
            {
                // 現在のアニメーションを切る
                mapImages[GetImageNumber()].animator.SetBool("lockOn", false);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                // ステージ番号を減少させる
                worldNum++;
                if (stageNum > (byte)mapInfo[worldNum].stageInformation.Count - 1)
                {
                    stageNum = (byte)(mapInfo[worldNum].stageInformation.Count - 1);
                }
                // 次の番号のアニメーションを起動
                mapImages[GetImageNumber()].animator.SetBool("lockOn", true);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);

            }//----- if_stop -----
            //else
            //{
            //    // 地図番号が最低値ではないなら
            //    if (worldNum != 0)
            //    {
            //        // 現在のアニメーションを切る
            //        animators[GetImageNumber()].SetBool("lockOn", false);
            //        // 地図番号を減少させ,ステージ番号を最大値にする
            //        worldNum--;
            //        stageNum = (byte)(worlds[worldNum].stages.Count - 1);
            //        // 次の番号のアニメーションを起動
            //        animators[GetImageNumber()].SetBool("lockOn", true);
            //    }//----- if_stop -----

            //}//----- else_stop -----

            // 現在位置を変更する
            // カメラ位置を移動させる。
            ZoomMove();


            // 位置変更を一時的に停止する
            StartCoroutine(ChangeDelay());

        }//----- if_stop -----
        // 入力方向が右なら
        else if (_inputDirection > 0.55f)
        {
            Debug.Log("上方向へ位置の変更を開始します");

            // ステージ番号が最大値未満なら
            if (worldNum > 0)
            {
                // 現在のアニメーションを切る
                mapImages[GetImageNumber()].animator.SetBool("lockOn", false);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", false);
                // ステージ番号を上昇させる
                worldNum--;
                if (stageNum > (byte)mapInfo[worldNum].stageInformation.Count - 1)
                {
                    stageNum = (byte)(mapInfo[worldNum].stageInformation.Count - 1);
                }
                // 次の番号のアニメーションを起動
                mapImages[GetImageNumber()].animator.SetBool("lockOn", true);
                selectorImages[GetImageNumber()].animator.SetBool("lockOn", true);
            }//----- if_stop -----
            //else
            //{
            //    // 地図番号が最大値ではないなら
            //    if (worldNum < (byte)worlds.Count - 1)
            //    {
            //        // 現在のアニメーションを切る
            //        animators[GetImageNumber()].SetBool("lockOn", false);
            //        // 地図番号を上昇させ,ステージ番号を最低値にする
            //        worldNum++;
            //        stageNum = 0;
            //        // 次の番号のアニメーションを起動
            //        animators[GetImageNumber()].SetBool("lockOn", true);
            //    }//----- if_stop -----

            //}//----- else_stop -----

            // カメラ位置を移動させる。
            ZoomMove();


            // 位置変更を一時的に停止する
            StartCoroutine(ChangeDelay());

        }//----- elseif_stop -----

    }

    //===========================================
    // 現在のステージ、ワールドの番号からImageのリストの番号を算出する
    // 戻り値：Imageリストの番号
    // 引数無し
    //===========================================
    // 2023/05/01
    // 作成者　高田
    private byte GetImageNumber()
    {
        byte stageCount = 0;

        // ワールド内に存在するステージをひとつ前のワールドまで加算
        for (byte i = 0; i < worldNum; i++)
        {
            stageCount += (byte)mapInfo[i].stageInformation.Count;
        }
        // 現在のステージ番号を加算することでImageの番号を決定
        stageCount += stageNum;

        return stageCount;
    }

    //==========================================
    // ImagesのリストをAddする際に、情報を取得する。
    // 戻り値：登録する情報を持ったImages
    // 引数  ：登録したいImageの番号
    //==========================================
    // 作成日　2023/05/03
    // 作成者　高田
    private MapImages SetImages(byte _i)
    {
        // 一先ず登録したいImageを取得
        var trans = transform.GetChild(_i);

        // リストを仮作成
        MapImages Images;
        // 子オブジェのImageを取得
        Images.image = trans.GetComponent<Image>();
        // 子オブジェのアニメーターを取得
        Images.animator = trans.GetComponent<Animator>();
        // 子オブジェのワールド座標を取得。親(このオブジェ)の座標に子オブジェのRectTrans座標を
        Images.transPos = trans.GetComponent<RectTransform>().position;
        // 子オブジェのマテリアル制御スクリプトを取得
        Images.mSelect = trans.GetComponent<testMSelect>();

        // Imagesを返す。
        return Images;
    }

    private SelectorImages SetSelectorImages(byte _i)
    {
        // カメラの孫オブジェに存在する_i番目のImageを取得
        var trans = cameraObj.transform.GetChild(0).GetChild(_i);
        SelectorImages Images;

        Images.image = trans.GetComponent<Image>();
        Images.animator = trans.GetComponent<Animator>();
        Images.transPos = trans.GetComponent<RectTransform>().position;

        return Images;
    }

    private void ZoomMove()
    {
        switch (mapInfo[worldNum].zoomSet)
        {
            case Stage_Manager.WorldInfo.ZoomSet.ON:
                // 現在位置を変更する
                if (cameraZoomFlag)
                {
                    nowZoom = true;
                }
                break;
            case Stage_Manager.WorldInfo.ZoomSet.OFF:
                if (cameraZoomFlag)
                {
                    nowZoomOut = true;
                    cameraZoomFlag = false;
                }
                break;
        }
    }

    //===========================================
    // ゲーム終了時にリストを破棄する。
    //===========================================
    private void OnApplicationQuit()
    {
        mapInfo.Clear();
        mapImages.Clear();
        selectorImages.Clear();

    }
}