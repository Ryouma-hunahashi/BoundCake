using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CameraVibration : MonoBehaviour
{
    // カメラ操作スクリプト取得
    private CameraController controller;


    [Header("揺れる時間")]
    [SerializeField] private float vibeTime = 0.3f;
    [Header("背景の揺れる時間")]
    [SerializeField] private float backGroundvibeTime = 0.5f;
    [Tooltip("一周期の時間")]
    [SerializeField] private float onePeriodTime = 0.05f;

    [Header("揺れの強さ")]
    [Tooltip("カメラの振幅")]
    public float cameraAmplitude = 0.2f;
    [Tooltip("背景の振幅")]
    public float backAmplitude = -0.2f;

    // 背景のトランスフォーム格納
    private Transform backGroundTrans;
    // 背景の基本Y座標取得
    private float defaultBackPosY;

    // 揺れる加速度
    private float vibeAccelaration;
    // 揺れの速度
    private float vibeVelocity = 0;
    //private float defaultRandomoOffset;

    // カメラの基本ポジション
    private float defaultTargetPosY;
    //全体の揺れフラグ
    [System.NonSerialized] public bool vibeFg = false;
    // 背景の揺れフラグ
    bool backVibeFg = false;
    // カメラの揺れフラグ
    bool cameraVibeFg = false;
    // 揺れ始めからの時間
    private float vibeElapsedTime = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // このオブジェクト内のカメラコントローラーを取得
        controller = GetComponent<CameraController>();
        // 無ければエラー
        if(controller == null)
        {
            Debug.LogError("[CameraController] が見つかりません");
        }

        // 背景のタグからトランスフォームを取得
        backGroundTrans = GameObject.FindWithTag("BackGround").transform;

        // 背景の基本ポジションを取得
        defaultBackPosY = backGroundTrans.position.y;

        // カメラの基本ポジションを取得
        defaultTargetPosY = controller.cameraTargetPosY;

        //nowAmplitude = maxAmplitude;

        

    }

    // Update is called once per frame
    void FixedUpdate()
    {
//#if UNITY_EDITOR
//        // デバッグ用。スペースが押されれば揺らす
//        if (Input.GetKeyDown(KeyCode.Space))
//        {
//            vibeFg = true;
//            //nowAmplitude = maxAmplitude;

//        }
//#endif
        // 揺らす指示が出たとき
        if(vibeFg == true)
        {
            // 背景、カメラ両方を揺らす指示を出す
            if(backVibeFg == false && cameraVibeFg == false)
            {
                backVibeFg = true;
                cameraVibeFg = true;
            }
            
            // 雑に360度を一周期速度をフレーム化して分割して加速度を出す
            vibeAccelaration = (2 * Mathf.PI) / (onePeriodTime * 60);
            // 速度を加算
            vibeVelocity += vibeAccelaration;
            // 発生からの時間を加算
            vibeElapsedTime += Time.deltaTime;
            // 背景を揺らす指示が出ていれば
            if (backVibeFg == true)
            {
                // 背景のポジションを一時格納
                var backPos = backGroundTrans.position;
                // Yを設定した振幅を最大として動かす
                backPos.y = defaultBackPosY + (backAmplitude * Mathf.Sin(vibeVelocity));
                backGroundTrans.position = backPos;
                // 時間が経過すれば
                if (vibeElapsedTime > backGroundvibeTime)
                {
                    // 全部初期化
                    vibeElapsedTime = 0f;
                    vibeVelocity = 0;
                    backPos.y = defaultBackPosY;
                    backGroundTrans.position = backPos;
                    backVibeFg = false;
                    vibeFg = false;
                }
            }
            // カメラを揺らす指示が出ていれば
            if(cameraVibeFg == true)
            {
                // カメラのポジションを一時格納
                var cameraPos = transform.position;
                cameraPos.y = defaultTargetPosY + (cameraAmplitude * Mathf.Sin(vibeVelocity));
                transform.position = cameraPos;


                //ゴミの残骸。乱数で動かそうとした名残。使わないと思う
                //controller.cameraTargetPosY = GetVibrationPosition(maxAmplitude, randomOffset, vibeElapsedTime, defaultTargetPosY);
                //transform.position = new Vector3(transform.position.x, GetVibrationPosition(maxAmplitude, randomOffset, vibeElapsedTime, defaultTargetPosY), transform.position.z);

                // 時間が経過すれば
                if (vibeElapsedTime > vibeTime)
                {
                    // カメラの揺れを初期化
                    cameraVibeFg = false;

                    //nowAmplitude = 0.0f;
                    controller.cameraTargetPosY = defaultTargetPosY;
                    cameraPos.y = defaultTargetPosY;
                    transform.position = cameraPos;
                }
            }
        }
        

        


        
        
        
        
        
    }

    //private float GetVibrationPosition(float strength,float offset,float elapsedTime,float targetPosY)
    //{
    //    var localStrength = strength;
    //    var localOffset = offset;
    //    var random = GetRandomNoise(localOffset, localStrength, elapsedTime);

    //    random *= localStrength;
    //    nowAmplitude = maxAmplitude;
    //    var ratio = 1.0f - elapsedTime / vibeTime;

    //    nowAmplitude *= ratio;

    //    random = Mathf.Clamp(random, -nowAmplitude, nowAmplitude);

    //    return (defaultTargetPosY + random);
    //}

    //private float GetRandomNoise(float offset, float speed, float time)
    //{
    //    var vibeNoise = Mathf.PerlinNoise(offset + speed * time, 0.0f);

    //    return ((vibeNoise - 0.5f) * 2.0f);
    //}

}
