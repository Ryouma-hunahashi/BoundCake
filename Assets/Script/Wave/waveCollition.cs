using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveCollition : MonoBehaviour
{
    [Header("強弱の判別"), Space(5)]
    [Tooltip("この数値以上で強波判定")]
    [SerializeField] private float strongChangePoint = 1.6f;

    [Header("以下デバッグ用"), Space(5)]
    //GameObject waveControllerObj; // 波コントロールオブジェクト格納用
    //waveController waveController;  //波コントローラ―スクリプト格納用
    private float waveMaxSize = 20.0f;  // 波のサイズ

    //private int waveDirection = 1;
    //private float waveReflectPointX = 0; // 波が反射エネミーに当たった時の位置

    private float waveEndPoint;

    private float waveElapsedTime = 0f;

    //[Header("波コントローラオブジェクトの名前")]
    //[SerializeField] private string waveConObjName = "Controller";

    private GameObject hitCollision;
    private WaveAudio audio;


    public enum WAVE_VELOCITY // 波の方向制御用
    {
        RIGHT,
        LEFT,
        UP,
        DOWN,

    }
    [Header("波の進む方向(X軸方向に進む)")]
    private WAVE_VELOCITY waveVelocity;

    public enum WAVE_TYPE // 誰の波か
    {
        PLAYER,
        PLAYER_ENEMY,
        PLAYER_POWERUP,
        ENEMY,
        GIMMICK,
        none,
    }
    private WAVE_TYPE waveType = WAVE_TYPE.PLAYER;
    // このオブジェクトの Transform 格納用
    private Transform waveColliderTrans;
    // 波の震源
    private Vector3 waveStartPosition;

    // 反射中判定用フラグ
    //int waveReflectFg = 0;
    // 波が終わるかのフラグ
    //public byte waveEndFg = 0;

    // 波が何に影響を及ぼすかの判定(1の場合プレイヤーを吹き飛ばしている)
    //int waveType = 0;
    // 波が発生しているかのフラグ
    //public byte waveFg = 0;

    public enum WAVE_MODE
    {
        STANDBY,
        SETUP,
        PLAY,
        END,
        REPEAT,

    }

    private WAVE_MODE waveMode = WAVE_MODE.STANDBY;



    // このオブジェクトが対応するvfxManager
    public vfxManager vfxManager;
    // vfxManager上の波の番号
    private sbyte waveNum = 0;


    private sbyte waveNumB = 0;//衝突した波の番号を保持する
    //=================================編集開始===============================================

    // 波の最大の高さ。
    private float maxWaveHight;
    // 波の生存時間
    private float waveLifeTime;
    // 現在の波の高さ
    public float nowHight;
    // 波の増減を判断するための指数
    public float nowHightIndex;

    //=================================編集終了===============================================



    // リピータースクリプトを格納。現在未使用
    public Test_Adder_Subtractor repeater;

    // リピーターが起動しているかを判断。リピーターから変更
    //public bool repeatFg = false;

    // このコリジョンを管理しているオブジェクトプール
    private WavePool pool = null;


    // 2023/2/18 高田
    // Start is called before the first frame update
    void Start()
    {
        // Transformを格納
        waveColliderTrans = this.transform;

        // 波開始位置の設定
        waveStartPosition = waveColliderTrans.position;
        // 波発生からの経過時間を保存
        waveElapsedTime = 0;

        audio = GetComponent<WaveAudio>();
        if (audio == null)
        {
            Debug.LogError("WaveAudioが見つかりません");
        }
    }

    // 作成日2023/2/18     2023/3/1 更新日
    // 高田
    // Update is called once per frame
    void FixedUpdate()
    {



        //========================================編集開始===============================================

        // 波の初期設定
        if (waveMode == WAVE_MODE.SETUP)
        {
            WaveSetup();
        }


        //========================================編集終了============================================
        // スペースが押された時に波を発生
        //if (Input.GetKeyDown(KeyCode.Space))
        //{//----- if_start -----

        //    waveFg = 2;

        //}//----- if_stop -----
        //=======================================編集開始================================================





        // 少しずつ振幅を小さくする。
        //waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, waveColliderTrans.localScale.y - 0.1f * Time.deltaTime - waveController.waveSpeed * 0.1f * Time.deltaTime, waveColliderTrans.localScale.z);
        //waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, /*waveColliderTrans.localScale.y*/defaultScale.y - 0.2f * waveElapsedTime - (Mathf.Abs(vfxManager.waveSpeedArray[waveNum]) * waveElapsedTime /** Time.deltaTime*/) * 0.2f, waveColliderTrans.localScale.z);
        //=======================================編集終了===============================================
        // 波が発生していて、その波が反射エネミーに触れていないとき、指定方向へ波を動かす。

        switch (waveMode)
        {
            case WAVE_MODE.PLAY:
                // 波を更新する
                WavePlay();
                break;
            case WAVE_MODE.END:
                // 波を終了させる処理を行う
                WaveEnd(waveEndPoint);
                break;
            case WAVE_MODE.REPEAT:
                WaveEnd(waveEndPoint);
                break;
            default:
                break;
        }
        /*
        if (waveFg == 2 && waveReflectFg == 0)

        {//----- if_start -----
         // 波発生からの時間を計算
            waveElapsedTime += Time.deltaTime;
            // 波の生存時間を計算。
            // 基本を波の大きさをそのまま秒数とし、それに補数を掛けることで計算。
            waveLifeTime = 1 / maxWaveHight * 3;
            // 現在の波の高さの指数。
            // 波の生存時間に発生からの経過時間を掛けることで計算。
            nowHightIndex = waveLifeTime * waveElapsedTime;

            // 波が増幅しているか減衰しているかを判断
            // 生存時間は波の最大サイズ*何分の一秒かの逆数で計算している。
            // 1以下の時(一度最大に到達するまで)
            if (nowHightIndex <= 1)
            {
                // 現在の大きさを計算。
                // ((1 - nowHightIndex) ^ 2)で現在の大きさの指数を取得する。
                // このままでは最大から小さくなるので、1から引くことで最小から最大に変換。
                // そこから出た結果に最大サイズの2倍を掛ける(コリジョンは山から谷のため)
                nowHight = (1 - Mathf.Pow(1 - nowHightIndex, 2)) * maxWaveHight * 2;
            }
            // 1より大きく、2以下の時(最大に到達した後、最小になるまで)
            else if (nowHightIndex <= 2)
            {
                // 上記と同じように現在の指数を取得。
                // 1 < nowHightindex <= 2のため、2から除算する。
                // 最大から最小にしたいため、1から除算はしない。
                // そこから出た結果に最大サイズの2倍を掛ける(コリジョンは山から谷のため)
                nowHight = (Mathf.Pow(2 - nowHightIndex, 2)) * maxWaveHight * 2;
            }
            // 2より大きくなった場合
            else
            {
                // 波を消す。
                // Destroyをした場合、消えた後に他オブジェから参照がかかりエラーを吐くのでSetActive(false)を行っている。
                transform.position = new Vector3(0, 0, 50);
                transform.SetParent(null);
                waveElapsedTime = 0;
                waveFg = 0;
                return;
                //gameObject.SetActive(false);
                //Destroy(gameObject);
            }


            // 波の方向を判断
            switch (waveVelocity)
            {
                // 右に向かっているとき
                case WAVE_VELOCITY.RIGHT:
                    // Yの大きさを波の高さに変換
                    waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                    // 大きさが最大でない間
                    if (waveColliderTrans.localScale.x < waveMaxSize)
                    {//----- if_start -----

                        // 波の速度と同じ速さで当たり判定を大きくする。
                        //========================================編集開始===================================-
                        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                        //=====================================編集終了======================================
                        // オブジェクトの左右に大きくなるので、現在の大きさの半分だけポジションを右にズラす。
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                    }//----- if_stop ----

                    // 大きさが最大になった時
                    else
                    {//----- else_start -----

                        // ポジションを波の速さで移動させる。
                        //========================================編集開始===================================-
                        //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                        //=====================================編集終了======================================

                    }//----- elseif_stop -----
                    break;
                case WAVE_VELOCITY.LEFT:
                    // Yの大きさを波の高さに変換
                    waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                    // 大きさが最大でない間(左に大きくするので大きさがマイナスの値になっている)
                    if (waveColliderTrans.localScale.x > -waveMaxSize)
                    {//----- if_start -----

                        // 波の速度と同じ速さで当たり判定を大きくする。
                        waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                        // オブジェクトの左右に大きくなるので、現在の大きさの半分だけポジションを左にズラす。
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                    }//----- if_stop -----
                    else
                    {//----- else_start -----

                        // ポジションを波の速さで移動させる。
                        waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                    }//----- else_stop -----
                    break;
                case WAVE_VELOCITY.UP:
                    // Xの大きさを波の高さに変換
                    waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                    // 大きさが最大でない間
                    if (waveColliderTrans.localScale.y < waveMaxSize)
                    {//----- if_start -----

                        // 波の速度と同じ速さで当たり判定を大きくする。
                        //========================================編集開始===================================-
                        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================編集終了======================================
                        // オブジェクトの上下に大きくなるので、現在の大きさの半分だけポジションを上にズラす。
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x, waveStartPosition.y + waveColliderTrans.localScale.y / 2, waveStartPosition.z);

                    }//----- if_stop -----

                    // 大きさが最大になった時
                    else
                    {//----- else_start -----

                        // ポジションを波の速さで移動させる。
                        //========================================編集開始===================================-
                        //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================編集終了======================================

                    }//----- elseif_stop -----
                    break;
                case WAVE_VELOCITY.DOWN:
                    // Xの大きさを波の高さに変換
                    waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                    // 大きさが最大でない間(下に大きくするのでマイナスの値になっている)
                    if (waveColliderTrans.localScale.y > -waveMaxSize)
                    {//----- if_start -----

                        // 波の速度と同じ速さで当たり判定を大きくする。
                        //========================================編集開始===================================-
                        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================編集終了======================================
                        // オブジェクトの上下に大きくなるので、現在の大きさの半分だけポジションを下にズラす。
                        waveColliderTrans.position =
                            new Vector3(waveStartPosition.x, waveStartPosition.y + waveColliderTrans.localScale.y / 2, waveStartPosition.z);

                    }//----- if_stop -----

                    // 大きさが最大になった時
                    else
                    {//----- else_start -----

                        // ポジションを波の速さで移動させる。
                        //========================================編集開始===================================-
                        //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                        waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                        //=====================================編集終了======================================

                    }//----- elseif_stop -----
                    break;
            }


             // 右に進んでいる時
            if (waveVelocity == WAVE_VELOCITY.RIGHT)
            {//----- if_start -----

                // 大きさが最大でない間
                if (waveColliderTrans.localScale.x < waveMaxSize)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を大きくする。
                    //========================================編集開始===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=====================================編集終了======================================
                    // オブジェクトの左右に大きくなるので、現在の大きさの半分だけポジションを右にズラす。
                    waveColliderTrans.position =
                        new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----

                // 大きさが最大になった時
                else
                {//----- else_start -----

                    // ポジションを波の速さで移動させる。
                    //========================================編集開始===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=====================================編集終了======================================

                }//----- elseif_stop -----
            }//----- if_stop -----

            // 左に進んでいるとき
            else if (waveVelocity == WAVE_VELOCITY.LEFT)
            {//----- elseif_start -----

                // 大きさが最大でない間(左に大きくするので大きさがマイナスの値になっている)
                if (waveColliderTrans.localScale.x > -waveMaxSize)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を大きくする。
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    // オブジェクトの左右に大きくなるので、現在の大きさの半分だけポジションを左にズラす。
                    waveColliderTrans.position =
                        new Vector3(waveStartPosition.x + waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----
                else
                {//----- else_start -----

                    // ポジションを波の速さで移動させる。
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                }//----- else_stop -----
            }//----- elseif_stop -----


    }//----- if_stop -----

    波が発生していて、その波が反射エネミーに吸収されている間
        else if (waveFg == 2 && waveReflectFg == 1)
        {//----- elseif_start -----

            // 右に進んでいる時
            if (waveVelocity == WAVE_VELOCITY.RIGHT)
            {//----- if_start -----

                // 波が吸収しきられるまでの間
                if (waveColliderTrans.localScale.x > 0)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を小さくする。
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
    // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを右にズラす。
    waveColliderTrans.position =
                    new Vector3(waveStartPosition.x - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

}//----- if_stop -----

                // 波が吸収しきられたら
else
{//----- else_start -----

    // 大きさの更新を止めて、逆方向に波を進める。
    waveReflectFg = 0;
    waveVelocity = WAVE_VELOCITY.LEFT;

}//----- else_stop -----
            }//----- if_stop -----

            // 左に進んでいる時
else if (waveVelocity == WAVE_VELOCITY.LEFT)
{//----- elseif_start -----

    // 波が吸収しきられるまでの間
    if (waveColliderTrans.localScale.x < 0)
    {//----- if_start -----

        // 波の速度と同じ速さで当たり判定を小さくする。
        //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
        waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
        // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを左にズラす。
        waveColliderTrans.position =
        new Vector3(waveStartPosition.x - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

    }//----- if_stop -----

    // 波が吸収しきられたら
    else
    {//----- else_start -----

        // 大きさの更新を止めて、逆方向に波を進める。
        waveReflectFg = 0;
        waveVelocity = WAVE_VELOCITY.RIGHT;

    }//----- else_stop -----
}//----- elseif_stop -----
        }//----- elseif_stop -----

         波の終了処理が掛けられた場合
        if (waveFg == 0 && waveEndFg == 1)
{
    // 波を終了させる処理を行う
    WaveEnd(waveEndPoint);
}

if (waveColliderTrans.localScale.y <= 0.2)
{
    Destroy(gameObject);
}
*/
    }

    // 作成日2023/3/1
    // 高田
    private void OnTriggerEnter(Collider other)
    {
        if (waveMode!=WAVE_MODE.STANDBY)
        {
            // Fixedにした影響でコリジョンのバグが発生したためコルーチンで止めた後に行う。
            StartCoroutine(TriggerDelayEnter(other));
        }

    }
    private void OnTriggerExit(Collider other)
    {
        if(other.CompareTag("Repeater"))
        {
            repeater = null;
        }
    }

    private IEnumerator TriggerDelayEnter(Collider other)
    {
        // 1フレーム止める
        yield return null;

        //// Reflect タグを持ったエネミーに触れたとき
        //if (other.gameObject.CompareTag("Reflect"))
        //{//----- if_start -----

        //    Debug.Log("HIT");
        //    // 波が吸収されるようにする。
        //    waveReflectFg = 1;

        //    // 波がプレイヤーに作用するようにする
        //    waveType = 1;

        //    if (waveVelocity == WAVE_VELOCITY.RIGHT)
        //    {//----- if_start -----

        //        // 波の震源を、当たった位置に更新
        //        // 左からあたったので左側の位置を取得している
        //        waveStartPosition.x = other.gameObject.transform.position.x - other.gameObject.transform.localScale.x / 2;

        //    }//----- if_stop -----
        //    else
        //    {//----- else_start -----

        //        // 波の震源を、当たった位置に更新
        //        // 右からあたったので右側の位置を取得している
        //        waveStartPosition.x = other.gameObject.transform.position.x + other.gameObject.transform.localScale.x / 2;

        //    }//----- else_stop -----
        //}//----- if_stop -----

        //// プレイヤーに作用出来る状態でプレイヤーに触れた時
        //else if (other.gameObject.tag == "Player" && waveType == 1)
        //{//----- if_start -----

        //    // プレイヤーの Rigidbody を取得
        //    var playerRigidbody = other.gameObject.GetComponent<Rigidbody>();

        //    // プレイヤーを上に飛ばす。
        //    playerRigidbody.AddForce(0, 20, 0, ForceMode.Impulse);

        //}//----- if_stop -----

        //===================================================編集開始====================================================
        // 波と衝突した場合
        if (other.gameObject.tag == "Wave")
        {
            // ぶつかった波のオブジェクトを取得
            hitCollision = other.gameObject;
            var collitionScript = hitCollision.GetComponent<waveCollition>();
            //Debug.Log("vfx取る");
            // ぶつかった波スクリプトのvfxManagerがこのオブジェクトのものと同じ場合
            //      ※別の糸のコリジョンと干渉し合わないため
            if (collitionScript.vfxManager == vfxManager)
            {

                //小さい方の波の添字を保持する
                waveNumB = collitionScript.waveNum;
                //Debug.Log("vfx取れた");
                // 波の方向を判断
                switch (waveVelocity)
                {
                    // 右の場合
                    case WAVE_VELOCITY.RIGHT:
                        //仮にぶつかった方の波のうち添字が大きい方を波Aとする
                        if (waveColliderTrans.localScale.y < hitCollision.transform.localScale.y)
                        {
                            // Debug.Log("Fixedは糞");
                            //互いの波長が等しいとき
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //波の管理スクリプトに波同士が衝突した時の処理を実行させる
                            //引数１：大きい方の波の添字
                            //引数２：小さい方の波の添字
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //もし波の大きさが同じときは両方消す
                        else if (waveColliderTrans.localScale.y == hitCollision.transform.localScale.y)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                            
                        }
                        break;
                    // 左の場合
                    case WAVE_VELOCITY.LEFT:
                        //仮にぶつかった方の波のうち添字が大きい方を波Aとする
                        if (waveColliderTrans.localScale.y < hitCollision.transform.localScale.y)
                        {
                            // Debug.Log("Fixedは糞");
                            //互いの波長が等しいとき
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //波の管理スクリプトに波同士が衝突した時の処理を実行させる
                            //引数１：大きい方の波の添字
                            //引数２：小さい方の波の添字
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //もし波の大きさが同じときは両方消す
                        else if (waveColliderTrans.localScale.y == hitCollision.transform.localScale.y)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                        }
                        break;
                    // 上の場合
                    case WAVE_VELOCITY.UP:
                        //仮にぶつかった方の波のうち添字が大きい方を波Aとする
                        if (waveColliderTrans.localScale.x < hitCollision.transform.localScale.x)
                        {
                            // Debug.Log("Fixedは糞");
                            //互いの波長が等しいとき
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //波の管理スクリプトに波同士が衝突した時の処理を実行させる
                            //引数１：大きい方の波の添字
                            //引数２：小さい方の波の添字
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //もし波の大きさが同じときは両方消す
                        else if (waveColliderTrans.localScale.x == hitCollision.transform.localScale.x)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                        }
                        break;
                    // 下の場合
                    case WAVE_VELOCITY.DOWN:
                        //仮にぶつかった方の波のうち添字が大きい方を波Aとする
                        if (waveColliderTrans.localScale.x > hitCollision.transform.localScale.x)
                        {
                            // Debug.Log("Fixedは糞");
                            //互いの波長が等しいとき
                            //vfxManager.WaveClash2(waveNum,waveNumB);
                            waveColliderTrans.position = new Vector3(0, 0, 50);
                            if (waveColliderTrans.parent != null)
                            {
                                waveColliderTrans.SetParent(null);
                            }
                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            //Destroy(hitCollision);
                            //波の管理スクリプトに波同士が衝突した時の処理を実行させる
                            //引数１：大きい方の波の添字
                            //引数２：小さい方の波の添字
                            vfxManager.WaveClash(waveNumB, waveNum);
                        }
                        //もし波の大きさが同じときは両方消す
                        else if (waveColliderTrans.localScale.x == hitCollision.transform.localScale.x)
                        {
                            waveColliderTrans.position = new Vector3(0, 0, 50);

                            waveElapsedTime = 0;
                            waveMode = WAVE_MODE.STANDBY;
                            if (transform.parent != null)
                            {
                                transform.SetParent(null);
                            }
                            vfxManager.waveDelete(waveNum);
                        }
                        break;
                }

            }

        }

        // WaveEndタグにぶつかった時、終了処理がなされていなければ
        if (other.gameObject.tag == "WaveEnd"/*&&waveEndFg == 0*/)
        {
            //// WaveEndに親が存在する場合
            //if (other.gameObject.transform.parent != null)
            //{
            //    // 親がvfxを持っているかを調べる。
            //    var hitVFXSaver = other.transform.parent.GetComponent<vfxSaver>();
            //    // 持っていなければ
            //    if (hitVFXSaver == null)
            //    {
            //        // 波を終了させる
            //        waveFg = 0;
            //        waveEndFg = 1;

            //        // 波の終了地点を調べる
            //        CheckWaveEndPoint(other);
            //        // 保存したvfx上の番号の波をwaveEndPointで終了させる。
            //        vfxManager.waveEnd(waveNum, waveEndPoint);
            //    }
            //    // 持っている場合(当たったのがただの糸の端の場合)
            //    else if (hitVFXSaver.vfxManager == vfxManager)
            //    {
            //        // 波を終了させる。
            //        waveFg = 0;
            //        waveEndFg = 1;
            //        // 念のため、終了地点を保存する。現在未使用
            //        CheckWaveEndPoint(other);




            //    }



            //}
            //// 親がいない場合(ただの終了地点の場合)
            //else
            //{
            // 波を終了させる。
            waveMode = WAVE_MODE.END;
            // 波の終了地点を調べる。
            CheckWaveEndPoint(other);
            // 保存したvfx上の番号の波をwaveEndPointで終了させる。
            vfxManager.waveEnd(waveNum, waveEndPoint);


            //}

        }

        if (other.gameObject.CompareTag("Repeater"))
        {
            // ぶつかったオブジェクトのリピータースクリプトを取得
            var repeaterScript = other.gameObject.GetComponent<Test_Adder_Subtractor>();
            //if(repeaterScript.machineMode!=Test_Adder_Subtractor.AdderSubtractor.none)
            //{

            //}
            //if (repeaterScript.machineMode != Test_Adder_Subtractor.AdderSubtractor.none)
            if (repeaterScript != null&&repeaterScript==repeater)
            {
                
                // リピーターに設定されているvfxの数、参照を行う
                for (int i = 0; i < repeaterScript.vfxCount; i++)
                {
                    // 波のvfxManagerとリピーターに設定されているものが同じ、かつ
                    // リピーターに吸収される可能性のある向きの波であれば
                    if (vfxManager == repeaterScript.vfxManagers[i] && waveVelocity == repeaterScript.waveInputVelocity[i])
                    {
                        if (repeater.machineMode == Test_Adder_Subtractor.AdderSubtractor.none)
                        {
                            waveMode = WAVE_MODE.END;
                        }
                        else
                        {
                            // 波を終了させる。
                            waveMode = WAVE_MODE.REPEAT;
                        }
                        // 波の終了地点を調べる。
                        CheckWaveEndPoint(other);
                        // 保存したvfx上の番号の波をwaveEndPointで終了させる。
                        vfxManager.waveEnd(waveNum, waveEndPoint);
                    }

                }

            }

        }
        //===================================================編集終了====================================================


    }

    private void WavePlay()
    {
        // 波発生からの時間を計算
        waveElapsedTime += Time.deltaTime;
        // 波の生存時間を計算。
        // 基本を波の大きさをそのまま秒数とし、それに補数を掛けることで計算。
        waveLifeTime = 1 / maxWaveHight * 3;
        // 現在の波の高さの指数。
        // 波の生存時間に発生からの経過時間を掛けることで計算。
        nowHightIndex = waveLifeTime * waveElapsedTime;

        // 波が増幅しているか減衰しているかを判断
        // 生存時間は波の最大サイズ*何分の一秒かの逆数で計算している。
        // 1以下の時(一度最大に到達するまで)
        if (nowHightIndex < 1)
        {
            // 現在の大きさを計算。
            // ((1 - nowHightIndex) ^ 2)で現在の大きさの指数を取得する。
            // このままでは最大から小さくなるので、1から引くことで最小から最大に変換。
            // そこから出た結果に最大サイズの2倍を掛ける(コリジョンは山から谷のため)
            nowHight = (1 - Mathf.Pow(1 - nowHightIndex, 2)) * maxWaveHight * 2;
        }
        // 1より大きく、2以下の時(最大に到達した後、最小になるまで)
        else if (nowHightIndex < 2)
        {
            // 上記と同じように現在の指数を取得。
            // 1 < nowHightindex <= 2のため、2から除算する。
            // 最大から最小にしたいため、1から除算はしない。
            // そこから出た結果に最大サイズの2倍を掛ける(コリジョンは山から谷のため)
            nowHight = (Mathf.Pow(2 - nowHightIndex, 2)) * maxWaveHight * 2;
        }
        // 2より大きくなった場合
        else
        {
            // 波を消す。
            // Destroyをした場合、消えた後に他オブジェから参照がかかりエラーを吐くのでSetActive(false)を行っている。
            transform.position = new Vector3(0, 0, 50);
            transform.SetParent(null);
            waveElapsedTime = 0;
            waveMode = WAVE_MODE.STANDBY;
            return;
            //gameObject.SetActive(false);
            //Destroy(gameObject);
        }


        // 波の方向を判断
        switch (waveVelocity)
        {
            // 右に向かっているとき
            case WAVE_VELOCITY.RIGHT:
                // Yの大きさを波の高さに変換
                waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                // 大きさが最大でない間
                if (waveColliderTrans.localScale.x < waveMaxSize)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を大きくする。
                    //========================================編集開始===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                    //=====================================編集終了======================================
                    // オブジェクトの左右に大きくなるので、現在の大きさの半分だけポジションを右にズラす。
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0, 0);

                }//----- if_stop -----

                // 大きさが最大になった時
                else
                {//----- else_start -----

                    // ポジションを波の速さで移動させる。
                    //========================================編集開始===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=====================================編集終了======================================

                }//----- elseif_stop -----
                break;
            case WAVE_VELOCITY.LEFT:
                // Yの大きさを波の高さに変換
                waveColliderTrans.localScale = new Vector3(waveColliderTrans.localScale.x, nowHight, waveColliderTrans.localScale.z);
                // 大きさが最大でない間(左に大きくするので大きさがマイナスの値になっている)
                if (waveColliderTrans.localScale.x > -waveMaxSize)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を大きくする。
                    waveColliderTrans.localScale += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    // オブジェクトの左右に大きくなるので、現在の大きさの半分だけポジションを左にズラす。
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0, 0);

                }//----- if_stop -----
                else
                {//----- else_start -----

                    // ポジションを波の速さで移動させる。
                    waveColliderTrans.position += new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);

                }//----- else_stop -----
                break;
            case WAVE_VELOCITY.UP:
                // Xの大きさを波の高さに変換
                waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                // 大きさが最大でない間
                if (waveColliderTrans.localScale.y < waveMaxSize)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を大きくする。
                    //========================================編集開始===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================編集終了======================================
                    // オブジェクトの上下に大きくなるので、現在の大きさの半分だけポジションを上にズラす。
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0);

                }//----- if_stop -----

                // 大きさが最大になった時
                else
                {//----- else_start -----

                    // ポジションを波の速さで移動させる。
                    //========================================編集開始===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================編集終了======================================

                }//----- elseif_stop -----
                break;
            case WAVE_VELOCITY.DOWN:
                // Xの大きさを波の高さに変換
                waveColliderTrans.localScale = new Vector3(nowHight, waveColliderTrans.localScale.y, waveColliderTrans.localScale.z);
                // 大きさが最大でない間(下に大きくするのでマイナスの値になっている)
                if (waveColliderTrans.localScale.y > -waveMaxSize)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を大きくする。
                    //========================================編集開始===================================-
                    //waveColliderTrans.localScale += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================編集終了======================================
                    // オブジェクトの上下に大きくなるので、現在の大きさの半分だけポジションを下にズラす。
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime/2, 0);

                }//----- if_stop -----

                // 大きさが最大になった時
                else
                {//----- else_start -----

                    // ポジションを波の速さで移動させる。
                    //========================================編集開始===================================-
                    //waveColliderTrans.position += new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.position += new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=====================================編集終了======================================

                }//----- elseif_stop -----
                break;
        }
    }

    //================================
    // 波の終了地点を調べる処理
    // 戻り値無し
    // 引数：ぶつかったCollider
    //================================
    // 作成日 2023/04/05
    // 作成者 高田
    public void CheckWaveEndPoint(Collider other)
    {
        // 波の方向を判断
        switch (waveVelocity)
        {
            // 右の場合
            case WAVE_VELOCITY.RIGHT:
                // 終了ポイントをぶつかったオブジェクトの右側にする。
                waveEndPoint = other.gameObject.transform.position.x - Mathf.Abs(other.gameObject.transform.lossyScale.x / 2+0.01f);
                break;
            // 左の場合
            case WAVE_VELOCITY.LEFT:
                // 終了ポイントをぶつかったオブジェクトの左側にする。
                waveEndPoint = other.gameObject.transform.position.x + Mathf.Abs(other.gameObject.transform.lossyScale.x / 2- 0.01f);
                break;
            // 上の場合
            case WAVE_VELOCITY.UP:
                // 終了ポイントをぶつかったオブジェクトの下側にする。
                waveEndPoint = other.gameObject.transform.position.y - Mathf.Abs(other.gameObject.transform.lossyScale.y / 2+ 0.01f);
                break;
            // 下の場合
            case WAVE_VELOCITY.DOWN:
                // 終了ポイントをぶつかったオブジェクトの上側にする。
                waveEndPoint = other.gameObject.transform.position.y + Mathf.Abs(other.gameObject.transform.lossyScale.y / 2- 0.01f);
                break;
        }
    }

    private void WaveEnd(float EndPos)
    {
        switch (waveVelocity)
        {
            case WAVE_VELOCITY.RIGHT:
                // 波が吸収しきられるまでの間
                if (waveColliderTrans.localScale.x > 0)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を小さくする。
                    //=========================編集開始========================================
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=============================編集終了=======================================
                    // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを右にズラす。
                    waveColliderTrans.position =
                    new Vector3(EndPos - Mathf.Abs(waveColliderTrans.localScale.x) / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----

                // 波が吸収しきられたら
                else
                {//----- else_start -----

                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }
                }//----- else_stop -----

                break;
            case WAVE_VELOCITY.LEFT:
                // 波が吸収しきられるまでの間
                if (waveColliderTrans.localScale.x < 0)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を小さくする。
                    //===================================編集開始======================================
                    waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
                    //=============================編集終了=======================================
                    // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを左にズラす。
                    waveColliderTrans.position =
                    new Vector3(EndPos + Mathf.Abs(waveColliderTrans.localScale.x) / 2, waveStartPosition.y, waveStartPosition.z);

                }//----- if_stop -----

                // 波が吸収しきられたら
                else
                {//----- else_start -----

                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }

                }//----- else_stop -----

                break;
            case WAVE_VELOCITY.UP:
                // 波が吸収しきられるまでの間
                if (waveColliderTrans.localScale.y > 0)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を小さくする。
                    //=========================編集開始========================================
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=============================編集終了=======================================
                    // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを右にズラす。
                    waveColliderTrans.position =
                    new Vector3(waveStartPosition.x, EndPos - Mathf.Abs(waveColliderTrans.localScale.y) / 2, waveStartPosition.z);

                }//----- if_stop -----

                // 波が吸収しきられたら
                else
                {//----- else_start -----

                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }
                }//----- else_stop -----

                break;
            case WAVE_VELOCITY.DOWN:
                // 波が吸収しきられるまでの間
                if (waveColliderTrans.localScale.y < 0)
                {//----- if_start -----

                    // 波の速度と同じ速さで当たり判定を小さくする。
                    //=========================編集開始========================================
                    //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
                    waveColliderTrans.localScale -= new Vector3(0, vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0);
                    //=============================編集終了=======================================
                    // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを右にズラす。
                    waveColliderTrans.position =
                    new Vector3(waveStartPosition.x, EndPos + Mathf.Abs(waveColliderTrans.localScale.y) / 2, waveStartPosition.z);

                }//----- if_stop -----

                // 波が吸収しきられたら
                else
                {//----- else_start -----
                    if (waveMode != WAVE_MODE.REPEAT)
                    {

                        transform.position = new Vector3(0, 0, 50);

                        if (transform.parent != null)
                        {
                            transform.SetParent(null);
                        }
                        waveElapsedTime = 0;
                        waveMode = WAVE_MODE.STANDBY;
                        //gameObject.SetActive(false);
                        //Destroy(gameObject);
                    }
                    else
                    {
                        //repeatFg = false;
                        //waveFg = 1;
                        waveMode = WAVE_MODE.PLAY;
                        transform.position = waveStartPosition;
                        waveElapsedTime = 0;
                    }
                }//----- else_stop -----

                break;
        }
        //// 右に進んでいる時
        //if (waveVelocity == WAVE_VELOCITY.RIGHT)
        //{//----- if_start -----

        //    // 波が吸収しきられるまでの間
        //    if (waveColliderTrans.localScale.x > 0)
        //    {//----- if_start -----

        //        // 波の速度と同じ速さで当たり判定を小さくする。
        //        //=========================編集開始========================================
        //        //waveColliderTrans.localScale -= new Vector3(waveController.waveSpeed * Time.deltaTime, 0, 0);
        //        waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
        //        //=============================編集終了=======================================
        //        // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを右にズラす。
        //        waveColliderTrans.position =
        //        new Vector3(EndPos - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

        //    }//----- if_stop -----

        //    // 波が吸収しきられたら
        //    else
        //    {//----- else_start -----

        //        if (repeatFg == false)
        //        {
        //            Destroy(gameObject);
        //        }
        //        else
        //        {
        //            //waveFg = 1;
        //            waveEndFg = 0;
        //            waveElapsedTime = 0;
        //        }
        //    }//----- else_stop -----
        //}//----- if_stop -----

        //// 左に進んでいる時
        //else if (waveVelocity == WAVE_VELOCITY.LEFT)
        //{//----- elseif_start -----

        //    // 波が吸収しきられるまでの間
        //    if (waveColliderTrans.localScale.x < 0)
        //    {//----- if_start -----

        //        // 波の速度と同じ速さで当たり判定を小さくする。
        //        //===================================編集開始======================================
        //        waveColliderTrans.localScale -= new Vector3(vfxManager.waveSpeedArray[waveNum] * Time.deltaTime, 0, 0);
        //        //=============================編集終了=======================================
        //        // オブジェクトの左右に小さくなるので、現在の大きさの半分だけポジションを左にズラす。
        //        waveColliderTrans.position =
        //        new Vector3(EndPos - waveColliderTrans.localScale.x / 2, waveStartPosition.y, waveStartPosition.z);

        //    }//----- if_stop -----

        //    // 波が吸収しきられたら
        //    else
        //    {//----- else_start -----

        //        if (repeatFg == false)
        //        {
        //            Destroy(gameObject);
        //        }
        //        else
        //        {
        //            //waveFg = 1;
        //            waveEndFg = 0;
        //            waveElapsedTime = 0;
        //        }

        //    }//----- else_stop -----
        //}//----- elseif_stop -----
    }

    public void WaveSetup()
    {

        // 波の速さのベクトルに応じて波の方向を変更
        if (vfxManager.waveSpeedArray[waveNum] < 0)
        {
            waveVelocity = WAVE_VELOCITY.LEFT;
        }
        else
        {
            waveVelocity = WAVE_VELOCITY.RIGHT;
        }
        SetMaxSize();
        //waveMaxSize = vfxManager.waveSpeedArray[waveNum] * vfxManager.waveWidthArray[waveNum] * 2;
        // 波を動かす

        waveElapsedTime = 0;

        if (maxWaveHight >= strongChangePoint)
        {
            audio.BigSound();
            //Debug.Log("強波サウンド再生！犯人は"+gameObject.GetInstanceID());
        }
        else
        {
            audio.SmallSound();
            //Debug.Log("弱波サウンド再生！犯人は" + gameObject.GetInstanceID());
        }
        waveMode = WAVE_MODE.PLAY;
    }
    //=====================
    // 波の最大の高さを返す関数
    //=====================
    // 作成日 2023/9/10
    // 高田
    public float GetMaxHight()
    {
        return maxWaveHight;
    }
    //=====================
    // 波の最大の高さを登録する関数
    //=====================
    // 作成日 2023/9/10
    // 高田
    public void SetMaxHight(float _hight)
    {
        maxWaveHight = _hight;
    }
    //=====================
    // 誰の波かを返す関数
    //=====================
    // 作成日 2023/9/10
    // 高田
    public bool CheckMode(WAVE_MODE _mode)
    {
        return waveMode == _mode;
    }
    //=====================
    // 誰の波かを登録する関数
    //=====================
    // 作成日 2023/9/10
    // 高田
    public void SetMode(WAVE_MODE _mode)
    {
        waveMode = _mode;
    }
    //=====================
    // この波のvfx上の番号を返す関数
    //=====================
    // 作成日 2023/9/10
    // 高田
    public sbyte GetVFXNum()
    {
        return waveNum;
    }

    //=====================
    // この波のvfx上の番号を登録する関数
    //=====================
    // 作成日 2023/9/10 
    // 高田
    public void SetVFXNum(sbyte _num)
    {
        waveNum = _num;
    }

    //=====================
    // 波の開始地点を登録する関数
    //=====================
    // 作成日 2023/9/10 
    // 高田
    public void SetStartPos(Vector3 _pos)
    {
        waveStartPosition = _pos;
    }

    //=====================
    // 波のタイプか引数と同じか否かを返す関数
    //=====================
    // 作成日 2023/9/10 
    // 高田
    public bool CheckType(WAVE_TYPE _type)
    {
        return waveType == _type;
    }

    //=====================
    // 波のタイプを登録する関数
    //=====================
    // 作成日 2023/9/10 
    // 高田
    public void SetType(WAVE_TYPE _type)
    {
        waveType = _type;
    }

    //=====================
    // 波の進む方向が引数と同じか否かを返す関数
    //=====================
    // 作成日 2023/9/10 
    // 高田
    public bool CheckVelocity(WAVE_VELOCITY _vel)
    {
        return waveVelocity == _vel;
    }

    //=====================
    // 波の進む方向を返す関数
    //=====================
    // 作成日 2023/9/10 
    // 高田
    public WAVE_VELOCITY GetVelocity()
    {
        return waveVelocity;
    }

    //=====================
    // 波の進む方向を登録する関数
    //=====================
    // 作成日 2023/9/10 
    // 高田
    public void SetVelocity(WAVE_VELOCITY _vel)
    {
        waveVelocity = _vel;
    }

    //=====================
    // 波の周波数を計算
    //=====================
    // 作成日 2023/9/11
    // 高田
    public void SetMaxSize()
    {
        // 速度*振動数で1周期の大きさを計算
        waveMaxSize = Mathf.Abs(vfxManager.waveSpeedArray[waveNum] * vfxManager.waveWidthArray[waveNum]);
    }

    //=====================
    // オブジェクトプールのスクリプト情報を取得
    //=====================
    // 作成日 2023/9/11
    // 高田
    public WavePool GetPool()
    {
        return pool;
    }

    //=====================
    // オブジェクトプールを登録
    //=====================
    // 作成日 2023/9/11
    // 高田
    public void SetPool(WavePool _pool)
    {
        if(pool == null)
        {
            pool = _pool;
        }
        else if(pool != _pool)
        {
            Debug.LogError("違うプールをセットしてたらバグが出るぞ");
        }
        
    }



    //======================
    // 波の経過時間をリセットする関数
    //======================
    // 作成日 2023/9/10
    // 高田
    public void ResetElapsedTime()
    {
        waveElapsedTime = 0;
    }
}



