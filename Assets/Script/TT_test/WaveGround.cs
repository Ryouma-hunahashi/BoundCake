using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGround : MonoBehaviour
{
    [Header("波コントローラーの名称")]
    [SerializeField] private string waveConObjName = "Controller";

    // 波コントローラのオブジェクト格納用
    private GameObject waveControllerObj;

    // 波の設定用スクリプト格納用
    private waveController waveConScript;

   
    private float waveAmplitude; // 波の振幅
    private float waveSpeed;     // 波の速度
    private float waveLength;    // 波の振動数

    private float waveStartTime;    // 振動開始時間
    private float waveElapsedTime;  // 振動開始からの経過時間

    private float thisNomalPointX; // このオブジェクトの標準X位置
    private float thisNomalPointZ; // このオブジェクトの標準Z位置
    private float thisNomalPointY; // このオブジェクトの標準Y座標
    private List<float> l_thisNomalHight = new List<float>(); // 波の大きさ
    private float thisPointHight;  // このオブジェクトに波が来た時の合計の高さ

    [SerializeField] private List<float> l_waveOrigin; // 波の震源のX座標
    [SerializeField] private List<float> l_waveReflectionPoint;
    
    [SerializeField] private List<float> l_toOriginDistanse = new List<float>(); // 発生地点からの距離
    // このオブジェクトの単振動が開始した時間
    [SerializeField] private List<float> l_thisPointWaveStartTime = new List<float>(); 
    // このオブジェクトの単振動開始からの経過時間
    [SerializeField] private List<float> l_thisPointWaveElapsedTime = new List<float>();

    enum WAVE_VELOCITY
    {
        RIGHT,
        LEFT,
        BOTH,
    }
    
    //反射地点を経由した場合の距離
    [SerializeField] private List<float> l_thisPointWaveReflectionDistance = new List<float>();
    // private float waveOrigin = 10.0f;       // 波の震源のX座標
    // private float toOriginDistanse = 0.0f; // 発生地点からの距離

    private Transform thisPointTransform; // このオブジェクトのTransform

    private const float PI = Mathf.PI; // 円周率


    private int waveMoveFlg = 0;
    private List<int> l_waveMoveFlg = new List<int>();

    private int waveOriginCount = 0;

    private int x = 1;

    // 作成日2023/2/16  2023/2/27更新日
    // 高田
    // Start is called before the first frame update
    void Start()
    {
        // このオブジェクトのTransformを格納
        thisPointTransform = this.transform;

        // 波の設定用スクリプトをアタッチしたオブジェクトを見つける
        waveControllerObj = GameObject.Find(waveConObjName);
        // 波の設定用スクリプトを取得
        waveConScript = waveControllerObj.GetComponent<waveController>();
        // 波の振幅を取得
        waveAmplitude = waveConScript.waveAmplitude;
        // 波の速度を取得
        waveSpeed = waveConScript.waveSpeed;
        // 波の振動数を取得
        waveLength = waveConScript.waveLength;
        // 基本座標を取得
        thisNomalPointX = thisPointTransform.position.x; // オブジェクトX座標
        thisNomalPointY = thisPointTransform.position.y; // オブジェクトY座標
        thisNomalPointZ = thisPointTransform.position.z; // オブジェクトZ座標
        l_waveOrigin = waveConScript.l_waveOrigin;
        l_waveReflectionPoint = waveConScript.l_waveReflectionPoint;
        for(int i = 0; i < l_waveOrigin.Count; i++)
        {
            l_thisNomalHight.Add(0.0f);
            l_toOriginDistanse.Add(0.0f);
            l_thisPointWaveStartTime.Add(0.0f);
            l_thisPointWaveElapsedTime.Add(0.0f);
            l_waveMoveFlg.Add(0);
            
        }
        for(int i = 0; i < l_waveReflectionPoint.Count; i++)
        {
            l_thisPointWaveReflectionDistance.Add(0.0f);
        }
    }

    // 作成日2023/2/16  2023/2/27更新日
    // 高田
    // Update is called once per frame
    void Update()
    {
        thisPointHight = 0;

        if (Input.GetKeyDown(KeyCode.Space) && waveMoveFlg == 0)
        {
            waveStartTime = Time.time;
            waveMoveFlg = 1;
            for(int i = 0;i<l_waveMoveFlg.Count;i++)
            {
                l_waveMoveFlg[i] = 1;
            }
        }
        


        if (waveMoveFlg == 1)
        {
           
            waveElapsedTime = Time.time - waveStartTime;
            // 波のX座標上での距離を計算
            // Mathf.Absで絶対値を取得して計算
            for (int i = 0; i < l_waveOrigin.Count; i++)
            {
                if (waveConScript.l_waveVelocity[i] == waveController.WAVE_VELOCITY.RIGHT)
                {
                    if(thisNomalPointX<l_waveOrigin[i])
                    {
                        l_waveMoveFlg[i] = 0;
                    }
                }
                else if (waveConScript.l_waveVelocity[i] == waveController.WAVE_VELOCITY.LEFT)
                {
                    if (thisNomalPointX > l_waveOrigin[i])
                    {
                        l_waveMoveFlg[i] = 0;
                    }
                }
                
                l_toOriginDistanse[i] = Mathf.Abs(thisNomalPointX - (l_waveOrigin[i]));

                //if(i%2 == 0)
                //{
                //    x = -1;
                //}
                //else
                //{
                //    x = 1;
                //}


                if (l_waveMoveFlg[i] == 1)
                {
                    // (振幅 - 距離 / 10)で振幅の減衰を計算。0以下であれば高さの更新をしない
                    if (waveAmplitude - l_toOriginDistanse[i] / 10 > 0
                        && (l_toOriginDistanse[i] <= waveSpeed * waveElapsedTime))
                    {

                        // オブジェクトを単振動させる
                        // y = Asin2π(t - x / λ)の式を用いる
                        // weveLengthを乗算することで振動数を増加させる
                        l_thisNomalHight[i] = x * (waveAmplitude - l_toOriginDistanse[i] / 10)
                            * Mathf.Sin(2.0f * PI * waveLength * (waveElapsedTime - (l_toOriginDistanse[i] / waveSpeed)));



                    }

                    if (waveElapsedTime > (1 / waveLength + l_toOriginDistanse[i] / waveSpeed ))
                    {
                        l_waveMoveFlg[i] = 0;
                        l_thisNomalHight[i] = 0;
                    }
                    if (!l_waveMoveFlg.Contains(1))
                    {

                        waveMoveFlg = 0;
                    }

                    thisPointHight += l_thisNomalHight[i];


                }
            }
            //for (int i = 0; i < l_waveOrigin.Count; i++)
            //{
            //    // 速度に経過時間をかけた距離が、震源からの距離に達すれば動かす
            //    if (l_toOriginDistanse[i] <= waveSpeed * waveElapsedTime)
            //    {//----- if_start -----
                 // 座標の更新
            thisPointTransform.position = new Vector3(thisNomalPointX, thisPointHight+thisNomalPointY, thisNomalPointZ);

            //}//----- if_stop -----
            //else
            //{//----- else_start -----
            // // 座標の固定
            //    thisPointTransform.position = new Vector3(thisNomalPointX, thisNomalPointY, thisNomalPointZ);

            //}//----- else_stop -----
            //}


            
        }
        
        if(waveMoveFlg == 0)
        {
            // 座標の固定
            thisPointTransform.position = new Vector3(thisNomalPointX, thisNomalPointY, thisNomalPointZ);
        }



        
    }

    //==================================================
    // ゲーム終了時に存在するリストを破棄する。
    // 引数無し
    // 戻り値無し
    //==================================================
    // 作成日2023/2/24
    // 高田
    private void OnApplicationQuit()
    {
        for (int i = l_waveOrigin.Count-1; i < 0; i--)
        {
            l_thisNomalHight.RemoveAt(i);
            l_toOriginDistanse.RemoveAt(i);
            l_waveOrigin.RemoveAt(i);
            l_thisPointWaveStartTime.RemoveAt(i);
            l_thisPointWaveElapsedTime.RemoveAt(i);
        }
    }

    //当たり判定
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Enemy"&&waveMoveFlg==1)
    //    {
    //        collision.rigidbody.AddForce(new Vector3(0, 1, 0),ForceMode.Impulse);
    //    }
    //}
}
