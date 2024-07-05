using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;


//=======================4/20　更新　中村==========================---
//update関数内でのposYの取得に条件追加
//移動床を使うときはvfxManagerの新規変数moveFlgをtrueにする
//デフォルトは固定床（false）
//====================================================================


//=======================4/22　更新　中村==========================---
//敵の波の波形をギザギザにする機能追加
//それに伴いWaveSpawn関数（176行目付近）に引数を新しく追加したので、
//MovePlayerやEnemyの波生成時の呼び出し時にfloat型の引数を加える
//ギザギザにするときは1、通常の波の時は0を渡す
//====================================================================

public class vfxManager : MonoBehaviour
{
    // Start is called before the first frame update
    //動かしたい波のエフェクトの設定
    //波のVFXオブジェクト一つにつきこのスクリプトを一つ持たせる
    VisualEffect effect;
    //波のエフェクトと連動させる地面
    //[Header("波と連動させる地面（当たり判定）のオブジェクト")]
    //[SerializeField] GameObject ground;
    [SerializeField] float testWaveHeight = 0;
    [SerializeField] float testWaveWidth = 0;
    //waveClash2関数での計算に使用する変数
    float clashSpeed = 0;
    float clashTime = 0;
    float clashDistance = 0;
    float dampingDistance = 0;
    float dampingTime = 0;

    public float chargeShift = 0;
    //配列？みたいななにか。
    //Bufferの中身を上で設定したエフェクトに引き渡す。

    GraphicsBuffer waveSpeedBuffer; //波のスピードのバッファ
    GraphicsBuffer waveStartPosBuffer;　//波の開始位置のバッファ
    GraphicsBuffer waveStartTimeBuffer;　//波の開始時間のバッファ
    GraphicsBuffer waveHeightBuffer;//波の高さを格納するバッファ
    GraphicsBuffer waveWidthBuffer;//波長（波の横幅）を格納するバッファ
    GraphicsBuffer waveFrequencyBuffer;//波の振動回数格納するバッファ
    [System.NonSerialized] public GraphicsBuffer endPosBuffer;
    GraphicsBuffer EnemyFlgBuffer;//波が敵のものか（波形をギザギザにするか）を格納するバッファ

    float endPos = 0;

    //スクリプトでwaveSpeedBuffer等のグラフィックバッファに引き渡す用の配列
    //波を発生させたとき等に書き換える
    [System.NonSerialized] public float[] waveSpeedArray = new float[10];//波の速度、数値が正なら右向き、負なら左向き
    [System.NonSerialized] public float[] waveStartTimeArray = new float[10];//波の開始時間
    [System.NonSerialized] public float[] waveStartPosArray = new float[10];//波の開始位置
    [System.NonSerialized] public float[] waveHeightArray = new float[10];//振幅（波の高さ）
    [System.NonSerialized] public float[] waveWidthArray = new float[10];//波長（波の横幅）
    [System.NonSerialized] public float[] endPosArray = new float[10];
    [System.NonSerialized] public float[] enemyFlgArray = new float[10];//1ならギザギザ、0なら通常の波

    //trueなら縦糸
    [Header("縦糸の場合チェックを入れる")]
    public bool warpWave = false;

    //int waveCnt = 0;

    [SerializeField] private float rideShiftScale = 1.6f;
    [SerializeField] private float grabShiftScale = 1.75f;
    float landScaleY = 0;


    [System.NonSerialized] public sbyte waveSpawnCnt = 0;//現在発生している波の数
    //======================4月19日・中村編集==============================================
    [Header("スイッチを使うときはチェックを入れる")]
    [SerializeField] private bool useSwitch = false;
    [SerializeField] private VariousSwitches_2 VariousSwitches_2;
    [SerializeField] private GameObject playerObj;
    [SerializeField] private string playerTag = "Player";
    //=============================編集終了================================================
    //======================4月20日・中村編集==============================================
    [Header("移動床の場合はチェック入れる")]
    [SerializeField] private bool moveFlg = false;
    [Header("天井糸の場合チェックを入れる")]
    [SerializeField] private bool topFg = false;
    //=============================編集終了================================================

    void Start()
    {
        //このスクリプトが管理する波のVFXコンポーネントを取得する
        effect = GetComponent<VisualEffect>();

        //グラフィックバッファの宣言
        //引数（不明、バッファ（配列？）の要素数、sizeof(バッファで扱う変数の型)）
        waveSpeedBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveStartPosBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveStartTimeBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveHeightBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        waveWidthBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        endPosBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        EnemyFlgBuffer = new GraphicsBuffer(GraphicsBuffer.Target.Raw, 10, sizeof(float));
        effect.SetFloat("posY", transform.position.y);
        effect.SetFloat("posX", transform.position.x);
        effect.SetFloat("startPosY", transform.position.y);
        
        if(warpWave==true)
        {
            effect.SetFloat("posY", transform.position.x);
            effect.SetFloat("posX", transform.position.y);
            //seffect.SetFloat("ShiftScale", 0);
            //effect.SetFloat("waveLong", transform.parent.gameObject.transform.localScale.y);
        }
        if(topFg)
        {
            effect.SetFloat("DownScaleY2", -0.45f);
            effect.SetFloat("const", 1.2f);
            effect.SetFloat("ShiftScale", -0.45f);

        }
        effect.SetFloat("waveLong", transform.parent.gameObject.transform.localScale.x);
        effect.SendEvent("OnPlay");

        //======================4月19日・中村編集==============================================
        if(useSwitch)
        {
            if(VariousSwitches_2==null)
            {
                Debug.Log("Swicth設定してない");
            }
        }
        playerObj = GameObject.FindWithTag(playerTag).gameObject;
        //=============================編集終了================================================

    }

    // Update is called once per frame
    void Update()
    {
        
        
        effect.SetFloat("waveLong", transform.parent.localScale.x);
        effect.SetVector3("playerPos", playerObj.transform.position);
        if(moveFlg)
        {
            if(warpWave)
            {
                effect.SetFloat("posY", transform.position.x);
                effect.SetFloat("posX", transform.position.y);
            }
            else
            {
                effect.SetFloat("posX", transform.position.x);
                effect.SetFloat("posY", transform.parent.parent.position.y);
                effect.SetFloat("startPosY", transform.parent.position.y);
                landScaleY = transform.parent.parent.position.y-transform.parent.position.y;
            }
        }
        if (!topFg)
        {
            if (playerObj.transform.position.x > transform.position.x - 1.5f &&
                playerObj.transform.position.x < transform.position.x + transform.parent.localScale.x + 1.5f)
            {
                if (!warpWave)
                {
                    if (playerObj.transform.position.y > transform.position.y)
                    {
                        effect.SetFloat("DownScaleY2", rideShiftScale+landScaleY);
                    }
                    else
                    {
                        effect.SetFloat("DownScaleY2", grabShiftScale+landScaleY);
                        effect.SetFloat("range", -0.45f - chargeShift);
                    }
                }
            }
        }
        

        ////スペースキーを押したとき
        //if(Input.GetKeyDown(KeyCode.Space))
        //{
        //    //開始時間の取得
        //    waveStartTimeArray[pushCnt]=Time.time;
        //    pushCnt++;
        //    waveStartTimeArray[pushCnt] = Time.time;

        //    //グラフィックバッファに配列の値を代入する
        //    waveSpeedBuffer.SetData(waveSpeedArray);
        //    waveStartPosBuffer.SetData(waveStartPosArray);
        //    waveStartTimeBuffer.SetData(waveStartTimeArray);

        //    //配列を代入したグラフィックバッファをVFXのパラメータに送信する
        //    effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        //    effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        //    effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);


        //}
        //effect.SetFloat("posY", transform.position.y);
        //effect.SetFloat("posX", transform.position.x);
        //if (warpWave == true)
        //{
        //    effect.SetFloat("posY", transform.position.x);
        //    effect.SetFloat("posX", transform.position.y);
        //    //effect.SetFloat("waveLong", transform.parent.gameObject.transform.localScale.y);
        //}
    }

    //=========================================================================s
    //波発生処理の関数()
    //戻り値：発生させた波の番号
    //引数：（波の開始位置、波の速度）
    //補足・波の速度が正なら右方向、負なら左方向に移動する
    //
    //=========================================================================
    public sbyte WaveSpawn(float waveStartPosX, float waveStartSpeed, float waveHeight, float waveWidth,float enemyFlg)
    {
        waveSpawnCnt++;
        if (waveSpawnCnt >= waveSpeedArray.Length)
        {
            waveSpawnCnt = 0;
        }
        //======================4月14日・中村編集==============================================
        if (useSwitch)
        {
            if(VariousSwitches_2==null)
            {
                Debug.Log("Switch設定してない");
            }
            if (VariousSwitches_2.switchStatus == false)
            {
                return -1;
            }
        }
        //=============================編集終了================================================



        //波の開始時間の取得
        waveStartTimeArray[waveSpawnCnt] = Time.time;
        //波の速度の設定
        waveSpeedArray[waveSpawnCnt] = waveStartSpeed;
        //波の開始位置の設定
        waveStartPosArray[waveSpawnCnt] = waveStartPosX;
        //振幅の設定
        waveHeightArray[waveSpawnCnt] = waveHeight;
        //波長の設定
        waveWidthArray[waveSpawnCnt] = waveWidth;

        endPosArray[waveSpawnCnt] = 0;

        //波形の設定
        enemyFlgArray[waveSpawnCnt] = enemyFlg;

        //各グラフィックバッファに配列の値を代入する
        waveSpeedBuffer.SetData(waveSpeedArray);
        waveStartPosBuffer.SetData(waveStartPosArray);
        waveStartTimeBuffer.SetData(waveStartTimeArray);
        waveHeightBuffer.SetData(waveHeightArray);
        waveWidthBuffer.SetData(waveWidthArray);
        endPosBuffer.SetData(endPosArray);
        EnemyFlgBuffer.SetData(enemyFlgArray);

        //配列を代入したグラフィックバッファをVFXのパラメータに送信する
        effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
        effect.SetGraphicsBuffer("waveWidthBuffer", waveWidthBuffer);
        effect.SetGraphicsBuffer("endPosBuffer",endPosBuffer);
        effect.SetGraphicsBuffer("EnemyFlgBuffer",EnemyFlgBuffer);
        //発生した波の番号を返す
        return waveSpawnCnt;



    }

    
    //=============================================================
    //波同士の衝突時の処理関数()
    //戻り値：なし
    //引数：（大きい方の波（波Ａ）の配列の添字、小さい方の波（波Ｂ）の配列の添字）
    //補足・
    //

    //=============================================================
    public void WaveClash(int waveNumA, int waveNumB)
    {
        //大きい波を波Ａ、小さい波を波Ｂとする

        //波の速度の設定
        //ぶつかった時に波を加速・減速させる
        //waveSpeedArray[waveSpawnCnt] = waveStartSpeed;

        //波の開始位置の設定
        //ぶつかった時に波を移動させる
        //waveStartPosArray[waveSpawnCnt] = waveStartPosX;

        //振幅の設定
        //大きい波の振幅（高さ）に小さい方の波の振幅の数値を加算する
        //小さい方の波を消す（高さを０にする）
        //waveHeightArray[waveNumA] += waveHeightArray[waveNumB] / 2;
        waveHeightArray[waveNumB] = 0;
        //波長の設定
        //ぶつかった時に波長を変更させる
        //waveWidthArray[waveSpawnCnt] = testWaveWidth;

        //各グラフィックバッファに配列の値を代入する
        //waveSpeedBuffer.SetData(waveSpeedArray);
        //waveStartPosBuffer.SetData(waveStartPosArray);
        //waveStartTimeBuffer.SetData(waveStartTimeArray);
        waveHeightBuffer.SetData(waveHeightArray);
        //waveWidthBuffer.SetData(waveWidthArray);

        //配列を代入したグラフィックバッファをVFXのパラメータに送信する
        //effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        //effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        //effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
        //effect.SetGraphicsBuffer("waveWidthBuffer", waveWidthBuffer);



    }
    //=============================================================
    //波同士の衝突時の処理関数()
    //戻り値：なし
    //引数：（大きい方の波（波Ａ）の配列の添字、小さい方の波（波Ｂ）の配列の添字）
    //補足・
    //

    //=============================================================
    public void WaveClash2(int waveNumA, int waveNumB)
    {
        //大きい波を波Ａ、小さい波を波Ｂとする


        //波Aと波Bの相対速度を求める
        clashSpeed = Mathf.Abs(waveSpeedArray[waveNumA] - waveSpeedArray[waveNumB]);
        //飲み込まれるまでの距離は互いの波長と等しい
        clashDistance = Mathf.Abs(waveSpeedArray[waveNumA] * waveWidthArray[waveNumA] * 2);

        clashTime = clashDistance / clashSpeed;

        StartCoroutine(WaitWave(waveNumA, waveNumB));
    }
    IEnumerator WaitWave(int waveNumA, int waveNumB)
    {
        yield return new WaitForSeconds(clashTime + 0.1f);
        //波の速度の設定
        //ぶつかった時に波を加速・減速させる
        //waveSpeedArray[waveSpawnCnt] = waveStartSpeed;

        //波の開始位置の設定
        //ぶつかった時に波を移動させる
        //waveStartPosArray[waveSpawnCnt] = waveStartPosX;

        //振幅の設定
        //大きい波の振幅（高さ）に小さい方の波の振幅の数値を加算する
        //小さい方の波を消す（高さを０にする）
        waveHeightArray[waveNumA] += waveHeightArray[waveNumB] / 2;
        waveHeightArray[waveNumB] = 0;
        //波長の設定
        //ぶつかった時に波長を変更させる
        //waveWidthArray[waveSpawnCnt] = testWaveWidth;

        //各グラフィックバッファに配列の値を代入する
        //waveSpeedBuffer.SetData(waveSpeedArray);
        //waveStartPosBuffer.SetData(waveStartPosArray);
        //waveStartTimeBuffer.SetData(waveStartTimeArray);
        waveHeightBuffer.SetData(waveHeightArray);
        //waveWidthBuffer.SetData(waveWidthArray);

        //配列を代入したグラフィックバッファをVFXのパラメータに送信する
        //effect.SetGraphicsBuffer("waveSpeedBuffer", waveSpeedBuffer);
        //effect.SetGraphicsBuffer("waveStartPosBuffer", waveStartPosBuffer);
        //effect.SetGraphicsBuffer("waveStartTimeBuffer", waveStartTimeBuffer);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
        //effect.SetGraphicsBuffer("waveWidthBuffer", waveWidthBuffer);

    }

    //=================================================================
    // 波吸収関数。送った位置で波を終了
    // 戻り値無し
    // 引数1：消去したい波の配列番号
    // 引数2：波を消去するX座標
    //=================================================================
    public void waveEnd(int waveNum,float endPosX)
    {
        endPosArray[waveNum] = endPosX;
        endPosBuffer.SetData(endPosArray);
        effect.SetGraphicsBuffer("endPosBuffer",endPosBuffer);
    }

    //=================================================================
    //波消去用の関数
    //戻り値無し
    //引数：消去したい波の配列番号
    //=================================================================
    public void waveDelete(int waveNum)
    {
        waveHeightArray[waveNum] = 0;
        waveHeightBuffer.SetData(waveHeightArray);
        effect.SetGraphicsBuffer("waveHeightBuffer", waveHeightBuffer);
    }

    private void OnApplicationQuit()
    {

        waveSpeedBuffer.Release();
        waveStartPosBuffer.Release();
        waveStartTimeBuffer.Release();
        waveHeightBuffer.Release();
        waveWidthBuffer.Release();
        endPosBuffer.Release();
        EnemyFlgBuffer.Release();

    }
}
