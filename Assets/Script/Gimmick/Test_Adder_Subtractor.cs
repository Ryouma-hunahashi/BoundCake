using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Adder_Subtractor : MonoBehaviour
{
    //// 波の情報
    //private Vector3 searchDirection;    // 波の向きを取得

    //private waveCollition getWaveScript;        // 振動のスクリプトを取得
    //private vfxManager getVFXManagerScript;     // VFXマネージャーを取得

    // 装置をどの状態で動かすか
    public enum AdderSubtractor
    {//----- enum_start -----
        [Tooltip("受け取った振動を次に登録した糸に強化")]
        adder,          
        [Tooltip("左から入ってきた波のみを強化")]
        adderLeft,          
        [Tooltip("右から入ってきた波のみを強化")]
        adderRight,

        [Tooltip("入ってきた波を強化し、\n[repeatCount]で設定した数の糸に出力")]
        adderBoth,         
        [Tooltip("受け取った振動を次に登録した糸に弱化")]
        subtractor,     
        [Tooltip("左から入ってきた波のみを弱化")]
        subtractorLeft,     
        [Tooltip("右から入ってきた波のみを弱化")]
        subtractorRight,
        [Tooltip("入ってきた波を弱化し、\n[repeatCount]で設定した数の糸に出力")]
        subtractorBoth,     // 両方の方向に弱化した振動をだす

        [Tooltip("縦と横の接続用")]
        VtoH_Connect,       // 縦と横を接続する

        [Tooltip("波を終了させる")]
        none,
    }//----- enum_stop -----

    // 装置の状態を変更する
    [Header("----- 装置の設定 -----"), Space(5)]
    [Tooltip("--- 加減装置の状態を変更 ---\n" + "adder : 加算して振動を起こす\n" + "subtractor : 減算して振動を起こす")]
    public AdderSubtractor machineMode = AdderSubtractor.none;
    
    enum SpringSetUP
    {
        ON,
        OFF,
    }
    [Header("糸の初期設定を行うかどうか")]
    [SerializeField] private SpringSetUP InitialSet = SpringSetUP.ON;
    [Header("ゲーム中に新しく糸を登録するか否か")]
    [SerializeField] private SpringSetUP setUP = SpringSetUP.OFF;
    private bool setUpFg = false;
    private byte startSetCheckTime = 0;
    private byte endSetCheckTime = 30;

    // このオブジェクトのコライダー
    private Collider col;

    // 受信側以外から振動を受け取ったときに反射する
    [Header("----- 反射の設定 -----"), Space(5)]
    [Tooltip("受信側以外から振動を受け取ったときに反射する")]
    [SerializeField] private bool reflect;              // 普通に反射する
    [Tooltip("受信側以外から振動を受け取ったときに強化してから反射する")]
    [SerializeField] private bool adderReflect;         // 強化して反射する
    [Tooltip("受信側以外から振動を受け取ったときに弱化してから反射する")]
    [SerializeField] private bool subtracterReflect;    // 弱化して反射する

    [Tooltip("Both設定用。次に登録されている糸から何個目まで波を発生させるか")]
    [SerializeField, Range(0, 4)] private byte repeatCount;

    // 加減装置
    [Header("----- 加減の設定 -----"), Space(5)]
    [SerializeField, Range(0f, 1f)] private float addPower;        // 波を加算する
    [SerializeField, Range(0f, 1f)] private float subtractPower;   // 波を減算する
    [SerializeField, Range(0f, 1f)] private float VtoH_subtractPower;

    [Header("----- 入ってくる波の制限 -----"),Space(5)]
    [SerializeField] private bool hightLimitationFg = false;
    [SerializeField] private float hightLimit;

    // 強化した波の数値
    [SerializeField] private float adderWavePower;

    // 弱化した波の数値
    [SerializeField] private float SubtractorWavePower;

    [Header("リピーターが作用する糸のオブジェクト")]

    [SerializeField] private List<GameObject> groundObj = new List<GameObject>();

    [Header("デバッグ用vfx確認")]
    // 登録されたオブジェ毎のvfxManager
    public List<vfxManager> vfxManagers = new List<vfxManager>();

    

    // オブジェクトの個数
    public byte vfxCount = 0;

    // 構造体リスト化の残骸。インスペクターに表示されないゴミカス
    //public struct GROUND
    //{
    //    vfxManager vfxManager;
    //    waveCollition.WAVE_VELOCITY input;
    //    waveCollition.WAVE_VELOCITY output;
    //}
    //[Header("表示")]
    //[SerializeField] public GROUND[] grounds = new GROUND[2];


    // 入ってくる波の方向。これ以外で入ると反応しない。
    public List<waveCollition.WAVE_VELOCITY> waveInputVelocity = new List<waveCollition.WAVE_VELOCITY>();
    // 出ていく波の方向
    public List<waveCollition.WAVE_VELOCITY> waveOutPutVelocity = new List<waveCollition.WAVE_VELOCITY>();
    // 設定したvfxが離れているか否か
    public List<bool> enterFg = new List<bool>();

    // ぶつかった波がgroundObjのどれから入ってきたかを判断する添え字。
    // 入ってきた場所から射出したい場合はこの変数を使用する
    [SerializeField] private byte inputVFXNumber;
    // 波をどのgroundObjから射出するかを判断する添え字
    [SerializeField] private byte outPutVFXNumber;
    [Header("取得したコリジョンスクリプト")]
    [SerializeField] private waveCollition waveCollision;
    [SerializeField] private GameObject waveColliderObj;
    [SerializeField] private bool repeatSetFg = false;
    // リスト内に重複するオブジェクトが存在するかを判別する
    private bool groundSetFg = true;

    private float repeatWavePower;

    private bool repeatGoFg;
    // private int modeSetFg = 0;

    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.transform.childCount == 3)
    //    {
    //        if (other.gameObject.transform.GetChild(2).GetComponent<vfxManager>() != null)
    //        {
    //            for (int i = 0; i < 4; i++)
    //            {

    //                if (vfxManagers[i] == null)
    //                {
    //                    vfxManagers[i] = other.gameObject.transform.GetChild(2).GetComponent<vfxManager>();
    //                    groundPos[i] = other.gameObject.transform.position;
    //                    vfxCount++;
    //                    for (int j = 0; j < i; j++)
    //                    {
    //                        if (i == 0)
    //                        {
    //                            break;
    //                        }
    //                        if (vfxManagers[i] == vfxManagers[j])
    //                        {
    //                            vfxManagers[i] = null;
    //                            groundPos[i] = Vector3.zero;
    //                            vfxCount--;
    //                        }
    //                    }
    //                    break;
    //                }
    //            }
    //        }
    //    }
    //}
    private void OnTriggerEnter(Collider other)
    {


        if (InitialSet == SpringSetUP.ON)
        {
            if (other.gameObject.layer == 6 || other.gameObject.layer == 8)
            {
                groundSetFg = true;
                if (other.transform.childCount == 1)
                {
                    if (other.gameObject.transform.GetChild(0).GetComponent<vfxManager>() != null)
                    {
                        for (int i = 0; i < groundObj.Count; i++)
                        {
                            if (groundObj[i] == other.gameObject)
                            {
                                groundSetFg = false;
                                if (setUpFg)
                                {
                                    enterFg[i] = true;
                                }
                                break;
                            }//-----if_stop-----
                            else
                            {
                                groundSetFg = true;
                            }//-----else_stop-----

                        }//-----for_syop-----
                        if (groundSetFg == true)
                        {
                            groundObj.Add(other.gameObject);
                            vfxManagers.Add(other.gameObject.transform.GetChild(0).GetComponent<vfxManager>());
                            if (setUpFg)
                            {
                                enterFg.Add(true);
                            }
                            else
                            {
                                enterFg.Add(false);
                            }

                            vfxCount++;
                            // 縦糸の場合
                            if (vfxManagers[vfxCount - 1].warpWave == true)
                            {
                                // オブジェクトが上にあれば
                                if (groundObj[vfxCount - 1].transform.position.y > transform.position.y)
                                {
                                    // 入力の可能性を下に
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                                    // 出力を上に設定
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                                }//-----if_stop-----
                                // 下にあれば
                                else
                                {
                                    // 入力の可能性を上に
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                                    // 出力を下に設定
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                                }//-----else_stop-----
                            }//-----if_stop-----
                            // 横糸の場合
                            else
                            {
                                // オブジェクトが右にあれば
                                if (groundObj[vfxCount - 1].transform.position.x > transform.position.x)
                                {
                                    // 入力の可能性を左に
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                                    // 出力を右に設定
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                                }//-----if_stop-----
                                // 左であれば
                                else
                                {
                                    // 入力の可能性を右に
                                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                                    // 出力を左に設定
                                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                                }//-----else_stop-----
                            }//-----else_stop-----
                        }//-----if_stop----
                    }//-----if_stop----
                }//-----if_stop----

            }//-----if_stop----

        }//-----if_stop----



        // [Wave]タグのオブジェクトに触れたなら
        if (other.gameObject.CompareTag("Wave"))
        {//----- if_start -----

            // 触れた相手の向きを取得
            //searchDirection = other.gameObject.transform.localScale;
            // 触れた波のコリジョンスクリプトを取得
            waveCollision = other.GetComponent<waveCollition>();
            if(waveCollision.repeater != null)
            {
                return;
            }
            if (hightLimitationFg && waveCollision.nowHight / 2 < hightLimit)
            {
                waveCollision.CheckWaveEndPoint(col);
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                waveCollision.ResetElapsedTime();
                return;
            }
            switch (machineMode)
            {
                case AdderSubtractor.adder:

                    //Debug.Log("Adder");
                    //// ぶつかった波を取得
                    //waveColliderObj = other.gameObject;

                    //// ぶつかった波がリピーターに登録している地面で起こった波か調べる
                    //for (byte i = 0; i < groundObj.Count; i++)
                    //{
                    //    // 波の方向がリピーターに接触する可能性のある波と同じかつ
                    //    // 登録してある地面で起こったものであった場合
                    //    if (waveCollision.waveVelocity == waveInputVelocity[i] && waveCollision.vfxManager == vfxManagers[i])
                    //    {
                    //        // 入ってきた地面の添え字を登録
                    //        inputVFXNumber = i;
                    //        // 出ていく地面を次に登録されているものに変更
                    //        outPutVFXNumber = (byte)(i + 1);
                    //        // 限界に到達したら0にする。
                    //        if (outPutVFXNumber == groundObj.Count)
                    //        {
                    //            outPutVFXNumber = 0;
                    //        }
                    //        // コリジョンのリピーター接触フラグをtureにする。
                    //        waveCollision.repeatFg = true;
                    //        // コリジョンをリピートするフラグをtrueにする。
                    //        repeatSetFg = true;
                    //        // for文から抜ける
                    //        break;
                    //    }
                    //    // 最大に到達しても方向とvfxが同じでなければ処理を中断
                    //    else if (i == groundObj.Count - 1)
                    //    {
                    //        if (waveCollision.repeatFg == false)
                    //        {
                    //            //waveCollition.waveEndFg = 0;
                    //            //waveCollition.waveFg = 2;
                    //        }
                    //        return;
                    //    }
                    //}
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {

                        Adder_InputCheck();
                    }

                    break;
                case AdderSubtractor.adderLeft:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        AdderLeft_InputCheck();
                    }
                    break;
                case AdderSubtractor.adderRight:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        AdderRight_InputCheck();
                    }

                    break;
                case AdderSubtractor.adderBoth:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        AdderBoth_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractor:

                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        Subtractor_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractorLeft:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        SubtractorLeft_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractorRight:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        SubtractorRight_InputCheck();
                    }
                    break;
                case AdderSubtractor.subtractorBoth:
                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        SubtractorBoth_InputCheck();
                    }
                    break;
                // 縦と横を繋ぐ処理の場合
                case AdderSubtractor.VtoH_Connect:

                    RepeatGroundSet(other);
                    if (repeatGoFg)
                    {
                        VtoH_InputCheck();
                    }

                    break;
                case AdderSubtractor.none:
                    RepeatGroundSet(other);
                    break;
                default:

                    break;
            }
            
            waveCollision = null;
            waveColliderObj = null;

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        if(setUpFg&&InitialSet == SpringSetUP.ON)
        {
            if(other.gameObject.layer == 6 || other.gameObject.layer == 8)
            {
                for (int i = 0; i < groundObj.Count; i++)
                {
                    if (groundObj[i] == other.gameObject)
                    {
                        enterFg[i] = false;

                        break;
                    }//-----if_stop-----
                    

                }//-----for_syop-----
            }
        }
    }

    //=================================================
    // 当たった波コリジョンを判断し、input,outputを判断
    // 戻り値無し
    // 引数：
    //=================================================
    private void RepeatGroundSet(Collider other)
    {
        // ぶつかった波を取得
        waveColliderObj = other.gameObject;

        // ぶつかった波がリピーターに登録している地面で起こった波か調べる
        for (byte i = 0; i < groundObj.Count; i++)
        {
            // 波の方向がリピーターに接触する可能性のある波と同じかつ
            // 登録してある地面で起こったものかつ
            // 接触している糸である場合
            if (waveCollision.CheckVelocity(waveInputVelocity[i]) && waveCollision.vfxManager == vfxManagers[i])
            {
                waveCollision.repeater = this;
                // 入ってきた地面の添え字を登録
                inputVFXNumber = i;
                // 出ていく地面を次に登録されているものに変更
                outPutVFXNumber = (byte)(i + 1);
                // 限界に到達したら0にする。
                if (outPutVFXNumber == groundObj.Count)
                {
                    outPutVFXNumber = 0;
                }
                while (!enterFg[outPutVFXNumber] || Mathf.Abs(groundObj[outPutVFXNumber].transform.localScale.x) < 0.35f)
                {
                    outPutVFXNumber++;
                    // 限界に到達したら0にする。
                    if (outPutVFXNumber >= groundObj.Count)
                    {
                        outPutVFXNumber = 0;
                    }
                    if (outPutVFXNumber == inputVFXNumber)
                    {
                        break;
                    }
                }
                
                // コリジョンのリピーター接触フラグをtureにする。
                //waveCollision.SetMode(waveCollition.WAVE_MODE.REPEAT);
                // コリジョンをリピートするフラグをtrueにする。
                //repeatSetFg = true;
                // for文から抜ける
                repeatGoFg = true;
                break;
            }
            // 最大に到達しても方向とvfxが同じでなければ処理を中断
            else if (i == groundObj.Count - 1)
            {
                //if (waveCollision.repeatFg == false)
                //{
                //    waveCollition.waveEndFg = 0;
                //    waveCollition.waveFg = 2;
                //}
                
                //waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                waveCollision.repeater = null;
                repeatGoFg = false;
                waveCollision = null;
                waveColliderObj = null;
                return;
            }
        }
    }



    //=================================================
    // 入ってきた波の向きに応じて、アウトプットの条件を変更する
    // 戻り値無し
    // 引数無し
    //=================================================
    // 作成日　2023/04/06   更新日2023/04/14
    // 作成者　高田　宮﨑
    private void Adder_InputCheck()
    {
        // アウトプットを統一のものに変更したため削除。
        // 旧型の方が良ければこちらを復元する。
        // 入ってきた波の方向を判断
        //switch (waveInputVelocity[inputVFXNumber])
        //{

        //    // 左の場合
        //    case waveCollition.WAVE_VELOCITY.LEFT:

        //        // 波を登録した次の地面に出現させる。
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }
        //        break;

        //    // 右の場合
        //    case waveCollition.WAVE_VELOCITY.RIGHT:

        //        // 波を登録した次の地面に出現させる。
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }
        //        break;

        //    // 上の場合
        //    case waveCollition.WAVE_VELOCITY.UP:

        //        // 波を登録した次の地面に出現させる。
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }
        //        break;

        //    // 下の場合
        //    case waveCollition.WAVE_VELOCITY.DOWN:

        //        // 波を登録した次の地面に出現させる。
        //        if (waveCollision.nowHightIndex < 1)
        //        {
        //            Adder_Output(waveCollision.maxWaveHight + addPower);
        //        }
        //        else if (waveCollision.nowHightIndex < 2)
        //        {
        //            Adder_Output(waveCollision.nowHight / 2);
        //        }

        //        break;
        //}
        // 入ってきた波の方向を判断
        switch (waveInputVelocity[inputVFXNumber])
        {
            // 左の場合
            case waveCollition.WAVE_VELOCITY.LEFT:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // 右の場合
            case waveCollition.WAVE_VELOCITY.RIGHT:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    { 
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                           WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // 上の場合
            case waveCollition.WAVE_VELOCITY.UP:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;
            // 下の場合
            case waveCollition.WAVE_VELOCITY.DOWN:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }

                break;

        }

    }

    //=================================================
    // 波の出ていく向きに応じて、生成場所、向きを変更
    // 戻り値無し
    // 引数：波の最大サイズ
    //=================================================
    // 作成日　2023/04/06   更新日2023/04/14
    // 作成者　高田　宮﨑
    private void Adder_Output(float waveMaxSize)
    {
        // 反射状態でなければ
        if (!reflect)
        {
            // 出ていく波の方向を判断
            switch (waveOutPutVelocity[outPutVFXNumber])
            {
                // 左の場合
                case waveCollition.WAVE_VELOCITY.LEFT:
                    {
                        Debug.Log("左に生成！");
                        // 発生位置をこのオブジェクトの左側に設定。波の方向、波の最大サイズを指定して生成
                        H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
                // 右の場合
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    {
                        Debug.Log("右に生成");
                        // 発生位置をこのオブジェクトの右側に設定。波の方向、波の最大サイズを指定して生成
                        H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
                // 上の場合
                case waveCollition.WAVE_VELOCITY.UP:
                    {
                        Debug.Log("上に生成！");
                        // 発生位置をこのオブジェクトの上側に設定。波の方向、波の最大サイズを指定して生成
                        V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
                // 下の場合
                case waveCollition.WAVE_VELOCITY.DOWN:
                    {
                        Debug.Log("下に生成！");
                        // 発生位置をこのオブジェクトの下側に設定。波の方向、波の最大サイズを指定して生成
                        V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, outPutVFXNumber);
                        break;
                    }
            }//----- switch_stop -----

        }//----- if_stop -----
        else
        {
            Debug.Log("Adder_Reflect");
            // 出ていく波の方向を反転
            switch (waveOutPutVelocity[inputVFXNumber])
            {
                // 左の場合
                case waveCollition.WAVE_VELOCITY.LEFT:
                    {
                        Debug.Log("左に生成！");
                        // 発生位置をこのオブジェクトの左側に設定。波の方向、波の最大サイズを指定して生成
                        H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, inputVFXNumber);
                        break;
                    }
                // 右の場合
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    {
                        Debug.Log("右に生成");
                        // 発生位置をこのオブジェクトの右側に設定。波の方向、波の最大サイズを指定して生成
                        H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, inputVFXNumber);
                        break;
                    }
                // 上の場合
                case waveCollition.WAVE_VELOCITY.UP:
                    {
                        Debug.Log("上に生成！");
                        // 発生位置をこのオブジェクトの上側に設定。波の方向、波の最大サイズを指定して生成
                        V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, inputVFXNumber);
                        break;
                    }
                // 下の場合
                case waveCollition.WAVE_VELOCITY.DOWN:
                    {
                        Debug.Log("下に生成！");
                        // 発生位置をこのオブジェクトの下側に設定。波の方向、波の最大サイズを指定して生成
                        V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, inputVFXNumber);
                        break;
                    }
            }//----- switch_stop -----
        }//----- else_stop -----
    }

    //=================================================
    // 左から右にのみ波を増幅させ、それ以外は反射、或いはただのwaveEndとして機能させる
    // 戻り値無し
    // 引数無し
    //=================================================
    private void AdderLeft_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                if(adderReflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight()+addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                if (inputVFXNumber != outPutVFXNumber)
                {
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                // 波を登録した次の地面に出現させる。
                
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // 右から左にのみ波を増幅させ、それ以外は反射、或いはただのwaveEndとして機能させる
    // 戻り値無し
    // 引数無し
    // ================================================
    private void AdderRight_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                if (inputVFXNumber != outPutVFXNumber)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                if (adderReflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() + addPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // 入ってきた波を、OutputVFXNumberから設定個数の糸に増幅してリピートする
    // 戻り値無し
    // 引数なし
    //=================================================
    private void AdderBoth_InputCheck()
    {
        // 波を登録した次の地面に出現させる。
        if (waveCollision.nowHightIndex < 1)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i)%vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.GetMaxHight() + addPower);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.GetMaxHight() + addPower);
                    
                }
            }


        }
        else if (waveCollision.nowHightIndex < 2)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i) % vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.nowHight / 2);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.nowHight / 2);
                    
                }
            }

        }

        // コリジョンをリピートするフラグを切る
        repeatSetFg = false;

        // Update内で波をリピートさせる場合の処理。
        // ぶつかった波コリジョンの情報をすべて破棄する。
        waveColliderObj = null;
        waveCollision = null;
    }

    //===================================
    // 入ってきた波の向きに応じて、アウトプットの条件を変更する
    // 戻り値無し
    // 引数無し
    //===================================
    // 作成日　2023/04/06   更新日2023/04/14
    // 作成者　高田　宮﨑
    private void Subtractor_InputCheck()
    {
        // 入ってきた波の方向を判断
        switch (waveInputVelocity[inputVFXNumber])
        {
            // 左の場合
            case waveCollition.WAVE_VELOCITY.LEFT:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // 右の場合
            case waveCollition.WAVE_VELOCITY.RIGHT:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;

            // 上の場合
            case waveCollition.WAVE_VELOCITY.UP:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }
                break;
            // 下の場合
            case waveCollition.WAVE_VELOCITY.DOWN:

                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }

                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    if (!reflect)
                    {
                        if (inputVFXNumber != outPutVFXNumber)
                        {
                            WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);

                        }
                        else
                        {
                            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                        }
                    }
                    else
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }

                }

                break;

        }

    }

    //=================================================
    // 左から右にのみ波を減衰させ、それ以外は反射、或いはただのwaveEndとして機能させる
    // 戻り値無し
    // 引数無し
    //=================================================
    private void SubtractorLeft_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                if (subtracterReflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                }
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // 右から左にのみ波を増幅させ、それ以外は反射、或いはただのwaveEndとして機能させる
    // 戻り値無し
    // 引数無し
    // ================================================
    private void SubtractorRight_InputCheck()
    {
        switch (waveInputVelocity[inputVFXNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                // 波を登録した次の地面に出現させる。
                if (waveCollision.nowHightIndex < 1)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                }
                else if (waveCollision.nowHightIndex < 2)
                {
                    WaveOutput(outPutVFXNumber, waveCollision.nowHight / 2);
                }
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                if (subtracterReflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight() - subtractPower);
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else if (reflect)
                {
                    // 波を登録した次の地面に出現させる。
                    if (waveCollision.nowHightIndex < 1)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.GetMaxHight());
                    }
                    else if (waveCollision.nowHightIndex < 2)
                    {
                        WaveOutput(inputVFXNumber, waveCollision.nowHight / 2);
                    }
                }
                else
                {
                    waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                }
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                // 波を消滅させる。
                waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                break;

        }
    }

    //=================================================
    // 入ってきた波を、OutputVFXNumberから設定個数の糸に増幅してリピートする
    // 戻り値無し
    // 引数なし
    //=================================================
    private void SubtractorBoth_InputCheck()
    {
        // 波を登録した次の地面に出現させる。
        if (waveCollision.nowHightIndex < 1)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i)%vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.GetMaxHight() - subtractPower);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.GetMaxHight() + addPower);
                    
                }
            }


        }
        else if (waveCollision.nowHightIndex < 2)
        {
            for (byte i = 0; i < repeatCount; i++)
            {
                byte outPutNumber = (byte)((outPutVFXNumber + i)%vfxCount);
                //if (outPutVFXNumber == vfxCount)
                //{
                //    outPutNumber -= vfxCount;
                //}
                if (i == 0)
                {
                    AdderBoth_OutputCheck(waveColliderObj, waveCollision, outPutNumber, waveCollision.nowHight / 2);
                }
                else
                {
                    var colData = waveCollision.GetPool().GetWaveCollision();
                    AdderBoth_OutputCheck(colData.waveObj, colData.collision, outPutNumber, waveCollision.nowHight / 2);
                }
            }

        }

        // コリジョンをリピートするフラグを切る
        repeatSetFg = false;

        // Update内で波をリピートさせる場合の処理。
        // ぶつかった波コリジョンの情報をすべて破棄する。
        waveColliderObj = null;
        waveCollision = null;
    }

    //=================================================
    // 波の出ていく向きに応じて、生成場所、向きを変更
    // 戻り値無し
    // 引数：波の最大サイズ
    // 1     出現させる糸の番号
    //=================================================
    // 作成日　2023/04/06   更新日2023/04/14
    // 作成者　高田　宮﨑
    private void WaveOutput(byte vfxNumber, float waveMaxSize)
    {
        
        if (waveMaxSize > 0&&enterFg[vfxNumber])
        {
            // 出ていく波の方向を判断
            switch (waveOutPutVelocity[vfxNumber])
            {
                // 左の場合
                case waveCollition.WAVE_VELOCITY.LEFT:
                    {
                        Debug.Log("左に生成！");
                        // 発生位置をこのオブジェクトの左側に設定。波の方向、波の最大サイズを指定して生成
                        H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                        break;
                    }
                // 右の場合
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    {
                        Debug.Log("右に生成");
                        // 発生位置をこのオブジェクトの右側に設定。波の方向、波の最大サイズを指定して生成
                        H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                        break;
                    }
                // 上の場合
                case waveCollition.WAVE_VELOCITY.UP:
                    {
                        Debug.Log("上に生成！");
                        // 発生位置をこのオブジェクトの上側に設定。波の方向、波の最大サイズを指定して生成
                        V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                        break;
                    }
                // 下の場合
                case waveCollition.WAVE_VELOCITY.DOWN:
                    {
                        Debug.Log("下に生成！");
                        // 発生位置をこのオブジェクトの下側に設定。波の方向、波の最大サイズを指定して生成
                        V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                        break;
                    }
            }//----- switch_stop -----
        }
        else
        {
            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
        }


    }

    private void AdderBoth_OutputCheck(GameObject Collision, waveCollition CollisionScript, byte vfxNumber, float waveMaxSize)
    {
        // 出ていく波の方向を判断
        switch (waveOutPutVelocity[vfxNumber])
        {
            case waveCollition.WAVE_VELOCITY.LEFT:
                H_DivideWaveSpawn(Collision, CollisionScript, transform.position.x - Mathf.Abs(transform.localScale.x) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                break;
            case waveCollition.WAVE_VELOCITY.RIGHT:
                H_DivideWaveSpawn(Collision, CollisionScript, transform.position.x + Mathf.Abs(transform.localScale.x) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                break;
            case waveCollition.WAVE_VELOCITY.UP:
                V_DivideWaveSpawn(Collision, CollisionScript, transform.position.y + Mathf.Abs(transform.localScale.y) / 2 + 0.01f, 1, waveMaxSize, vfxNumber);
                break;
            case waveCollition.WAVE_VELOCITY.DOWN:
                V_DivideWaveSpawn(Collision, CollisionScript, transform.position.y - Mathf.Abs(transform.localScale.y) / 2 - 0.01f, -1, waveMaxSize, vfxNumber);
                break;
        }
    }

    //===================================
    // 入ってきた波の向きに応じて、アウトプットの条件を変更する
    // 戻り値無し
    // 引数無し
    //===================================
    // 作成日　2023/04/06
    // 作成者　高田
    private void VtoH_InputCheck()
    {
        // 入ってきた波の方向を判断
        switch (waveInputVelocity[inputVFXNumber])
        {
            // 左の場合
            case waveCollition.WAVE_VELOCITY.LEFT:
                // 波の幅が0以上になったら(マイナス方向に波が大きくなっていたため)
                //if (waveColliderObj.transform.localScale.x >= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("左方向の次の波の生成じゃあ！");
                        // 波を登録した次の地面に出現させる。
                        if (waveCollision.nowHightIndex < 1)
                        {

                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);

                        }
                        else if (waveCollision.nowHightIndex < 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
            // 右の場合
            case waveCollition.WAVE_VELOCITY.RIGHT:
                //if (waveColliderObj.transform.localScale.x <= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("右方向の次の波の生成じゃあ！");
                        // 波を登録した次の地面に出現させる。
                        if (waveCollision.nowHightIndex < 1)
                        {
                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                        else if (waveCollision.nowHightIndex < 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
            // 上の場合
            case waveCollition.WAVE_VELOCITY.UP:
                //if (waveColliderObj.transform.localScale.y <= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("上方向の次の波の生成じゃあ！");
                        // 波を登録した次の地面に出現させる。
                        if (waveCollision.nowHightIndex < 1)
                        {
                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                        else if (waveCollision.nowHightIndex <= 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
            // 下の場合
            case waveCollition.WAVE_VELOCITY.DOWN:
                //if (waveColliderObj.transform.localScale.y >= 0)
                {
                    if (inputVFXNumber != outPutVFXNumber)
                    {
                        Debug.Log("下方向の次の波の生成じゃあ！");
                        // 波を登録した次の地面に出現させる。
                        if (waveCollision.nowHightIndex < 1)
                        {
                            repeatWavePower = waveCollision.GetMaxHight() - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                        else if (waveCollision.nowHightIndex < 2)
                        {
                            repeatWavePower = waveCollision.nowHight / 2 - VtoH_subtractPower;
                            if (repeatWavePower < 0)
                            {
                                repeatWavePower = 0;
                            }
                            VtoH_Output(repeatWavePower);
                        }
                    }
                    else
                    {
                        waveCollision.SetMode(waveCollition.WAVE_MODE.END);
                    }
                }
                break;
        }

    }

    //=================================================
    // 波の出ていく向きに応じて、生成場所、向きを変更
    // 戻り値無し
    // 引数：波の最大サイズ
    //=================================================
    // 作成日　2023/04/06
    // 作成者　高田
    private void VtoH_Output(float waveMaxSize)
    {
        if (waveMaxSize > 0&&enterFg[outPutVFXNumber])
        {
            // 出ていく波の方向を判断
            switch (waveOutPutVelocity[outPutVFXNumber])
            {
                // 左の場合
                case waveCollition.WAVE_VELOCITY.LEFT:
                    Debug.Log("左に生成！");
                    // 発生位置をこのオブジェクトの左側に設定。波の方向、波の最大サイズを指定して生成
                    H_RepeatWaveSpawn(transform.position.x - Mathf.Abs(transform.localScale.x) / 2-0.01f, -1, waveMaxSize, outPutVFXNumber);

                    break;
                // 右の場合
                case waveCollition.WAVE_VELOCITY.RIGHT:
                    Debug.Log("右に生成");
                    // 発生位置をこのオブジェクトの右側に設定。波の方向、波の最大サイズを指定して生成
                    H_RepeatWaveSpawn(transform.position.x + Mathf.Abs(transform.localScale.x) / 2+ 0.01f, 1, waveMaxSize, outPutVFXNumber);

                    break;
                // 上の場合
                case waveCollition.WAVE_VELOCITY.UP:
                    Debug.Log("上に生成！");
                    // 発生位置をこのオブジェクトの上側に設定。波の方向、波の最大サイズを指定して生成
                    V_RepeatWaveSpawn(transform.position.y + Mathf.Abs(transform.localScale.y) / 2+ 0.01f, 1, waveMaxSize, outPutVFXNumber);

                    break;
                // 下の場合
                case waveCollition.WAVE_VELOCITY.DOWN:
                    Debug.Log("下に生成！");
                    // 発生位置をこのオブジェクトの下側に設定。波の方向、波の最大サイズを指定して生成
                    V_RepeatWaveSpawn(transform.position.y - Mathf.Abs(transform.localScale.y) / 2- 0.01f, -1, waveMaxSize, outPutVFXNumber);

                    break;
            }
        }
        else
        {
            waveCollision.SetMode(waveCollition.WAVE_MODE.END);
        }
    }

    //==============================================
    // 左右に波を発生させる関数
    // 戻り値無し
    // 引数：波の発生位置、波の向き、波の最大サイズ、出したい糸の配列番号
    //==============================================
    // 作成日　2023/04/07
    // 作成者　高田
    private void H_RepeatWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // 波の向きを出ていく向きに設定
        waveCollision.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // 波のコリジョンスクリプトに出ていく波のvfxManagerを代入
        waveCollision.vfxManager = vfxManagers[vfxNumber];
        // vfxの波を生成し、波の番号を一時的に保存
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]),
                                                waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()],
                                                vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);
        
        
        // 地面が移動床の場合、その地面の親の子に波を設定する。
        if ((groundObj[vfxNumber].CompareTag("Floor") ||
            (groundObj[vfxNumber].transform.parent != null &&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor"))) &&
            !vfxManagers[vfxNumber].warpWave)
        {
            waveColliderObj.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        { 
            waveColliderObj.transform.SetParent(null); 
        }

        // コリジョンのvfx上の番号を生成した波のものに設定
        waveCollision.SetVFXNum(num);
        // コリジョンのスケールを初期化
        waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
        // コリジョンの初期位置XをrepeatPosに変更
        var trans = waveColliderObj.transform.position;
        trans.x = repeatPos;
        // Yをこのオブジェの位置に変更
        trans.y = groundObj[vfxNumber].transform.position.y;
        // 変更した結果を代入
        waveColliderObj.transform.position = trans;
        // 波の開始地点を設定
        waveCollision.SetStartPos(trans);
        // 波の1周期の大きさを設定
        waveCollision.SetMaxSize();
        // 波の最大の高さを設定
        waveCollision.SetMaxHight(waveMaxSize);
        // 発生からの経過時間を初期化
        waveCollision.ResetElapsedTime();
        // 波を動かす
        waveCollision.SetMode(waveCollition.WAVE_MODE.PLAY);
        // コリジョンをリピートするフラグを切る
        repeatSetFg = false;

        

    }
    //==============================================
    // 上下に波を発生させる関数
    // 戻り値無し
    // 引数：波の発生位置、波の向き、波の最大サイズ、出したい糸の配列番号
    //==============================================
    // 作成日　2023/04/07
    // 作成者　高田
    private void V_RepeatWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // 波の向きを出ていく向きに設定
        waveCollision.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // 波のコリジョンスクリプトに出ていく波のvfxManagerを代入
        waveCollision.vfxManager = vfxManagers[vfxNumber];
        // vfxの波を生成し、波の番号を一時的に保存
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]), 
                                                   waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()], 
                                                   vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);

        
        // 地面が移動床の場合、その地面の親の子に波を設定する。
        if ((groundObj[vfxNumber].CompareTag("Floor") ||
            (groundObj[vfxNumber].transform.parent != null &&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor")))&&
            !vfxManagers[vfxNumber].warpWave)
        {
            waveColliderObj.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        {
            waveColliderObj.transform.SetParent(null);
        }

        // コリジョンのvfx上の番号を生成した波のものに設定
        waveCollision.SetVFXNum(num);
        // コリジョンのスケールを初期化
        waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
        // コリジョンの初期位置YをrepeatPosに変更
        var trans = waveColliderObj.transform.position;
        trans.y = repeatPos;
        // Xを振動を出すオブジェの位置に変更
        trans.x = groundObj[vfxNumber].transform.position.x;
        // 変更した結果を代入
        waveColliderObj.transform.position = trans;
        // 波の開始地点を設定
        waveCollision.SetStartPos(trans);
        // 波の1周期の大きさを設定
        waveCollision.SetMaxSize();
        // 波の最大の高さを設定
        waveCollision.SetMaxHight(waveMaxSize);
        // 発生からの経過時間を初期化
        waveCollision.ResetElapsedTime();
        // 波を動かす
        waveCollision.SetMode(waveCollition.WAVE_MODE.PLAY);

        
        repeatSetFg = false;
        

    }

    private void H_DivideWaveSpawn(GameObject Collision, waveCollition CollitionScript, float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // 波の向きを出ていく向きに設定
        CollitionScript.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // 波のコリジョンスクリプトに出ていく波のvfxManagerを代入
        CollitionScript.vfxManager = vfxManagers[vfxNumber];
        // vfxの波を生成し、波の番号を一時的に保存
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]),
                                                   waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()], 
                                                   vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);


        // 地面が移動床の場合、その地面の親の子に波を設定する。
        if ((groundObj[vfxNumber].CompareTag("Floor") ||
            (groundObj[vfxNumber].transform.parent != null &&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor"))) &&
            !vfxManagers[vfxNumber].warpWave)
        {
            Collision.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        { 
            Collision.transform.SetParent(null); 
        }

        // コリジョンのvfx上の番号を生成した波のものに設定
        CollitionScript.SetVFXNum(num);
        // コリジョンのスケールを初期化
        Collision.transform.localScale = new Vector3(0, 0, 1);
        // コリジョンの初期位置XをrepeatPosに変更
        var trans = Collision.transform.position;
        trans.x = repeatPos;
        // Yをこのオブジェの位置に変更
        trans.y = groundObj[vfxNumber].transform.position.y;
        // 変更した結果を代入
        Collision.transform.position = trans;
        // 波の開始地点を設定
        CollitionScript.SetStartPos(trans);
        // 波の1周期の大きさを設定
        CollitionScript.SetMaxSize();
        // 波の最大の高さを設定
        CollitionScript.SetMaxHight(waveMaxSize);
        // 発生からの経過時間を初期化
        CollitionScript.ResetElapsedTime();
        // 波を動かす
        CollitionScript.SetMode(waveCollition.WAVE_MODE.PLAY);

    }

    private void V_DivideWaveSpawn(GameObject Collision, waveCollition CollitionScript, float repeatPos, float waveAngle, float waveMaxSize, int vfxNumber)
    {
        // 波の向きを出ていく向きに設定
        CollitionScript.SetVelocity(waveOutPutVelocity[vfxNumber]);
        // 波のコリジョンスクリプトに出ていく波のvfxManagerを代入
        CollitionScript.vfxManager = vfxManagers[vfxNumber];
        // vfxの波を生成し、波の番号を一時的に保存
        var num = vfxManagers[vfxNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.GetVFXNum()]), 
                                                   waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.GetVFXNum()],
                                                   vfxManagers[inputVFXNumber].enemyFlgArray[waveCollision.GetVFXNum()]);

        
        // 地面が移動床の場合、その地面の親の子に波を設定する。
        if ((groundObj[vfxNumber].CompareTag("Floor")||
            (groundObj[vfxNumber].transform.parent!=null&&
            groundObj[vfxNumber].transform.parent.CompareTag("Floor")))&&
            !vfxManagers[vfxNumber].warpWave)
        {
            Collision.transform.SetParent(groundObj[vfxNumber].transform.parent);
        }
        else
        {
            Collision.transform.SetParent(null);
        }

        // コリジョンのvfx上の番号を生成した波のものに設定
        CollitionScript.SetVFXNum(num);
        // コリジョンのスケールを初期化
        Collision.transform.localScale = new Vector3(0, 0, 1);
        // コリジョンの初期位置XをrepeatPosに変更
        var trans = Collision.transform.position;
        trans.y = repeatPos;
        // Yをこのオブジェの位置に変更
        trans.x = groundObj[vfxNumber].transform.position.x;
        // 変更した結果を代入
        Collision.transform.position = trans;
        // 波の開始地点を設定
        CollitionScript.SetStartPos(trans);
        // 波の1周期の大きさを設定
        CollitionScript.SetMaxSize();
        // 波の最大の高さを設定
        CollitionScript.SetMaxHight(waveMaxSize);
        // 発生からの経過時間を初期化
        CollitionScript.ResetElapsedTime();
        // 波を動かす
        CollitionScript.SetMode(waveCollition.WAVE_MODE.PLAY);

    }

    // 反射の残骸。出したいvfxの番号さえ変更すれば動くため、参考用に保存。
    //private void H_ReflectWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize)
    //{

    //    // 波の向きを出ていく向きに設定
    //    waveCollision.waveVelocity = waveOutPutVelocity[inputVFXNumber];

    //    // vfxの波を生成し、波の番号を一時的に保存
    //    var num = vfxManagers[inputVFXNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.waveNum]), waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.waveNum]);
    //    // 波のコリジョンスクリプトに出ていく波のvfxManagerを代入
    //    waveCollision.vfxManager = vfxManagers[inputVFXNumber];
    //    // コリジョンのvfx上の番号を生成した波のものに設定
    //    waveCollision.waveNum = num;
    //    // コリジョンのスケールを初期化
    //    waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
    //    // コリジョンの初期位置XをrepeatPosに変更
    //    var trans = waveCollision.transform.position;
    //    trans.x = repeatPos;
    //    // Yをこのオブジェの位置に変更
    //    trans.y = transform.position.y;
    //    // 変更した結果を代入
    //    waveColliderObj.transform.position = trans;
    //    // 波の開始地点を設定
    //    waveCollision.waveStartPosition = trans;
    //    // 波の1周期の大きさを設定
    //    waveCollision.waveMaxSize = Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[num] * vfxManagers[inputVFXNumber].waveWidthArray[num]);
    //    // 波の最大の高さを設定
    //    waveCollision.maxWaveHight = waveMaxSize;
    //    // 発生からの経過時間を初期化
    //    waveCollision.waveElapsedTime = 0;
    //    // 波の終了フラグを切る
    //    waveCollision.waveEndFg = 0;
    //    // 波を移動させる
    //    waveCollision.waveFg = 2;
    //    // リピートフラグ解除
    //    waveCollision.repeatFg = false;
    //    // コリジョンをリピートするフラグを切る
    //    repeatSetFg = false;

    //}
    //private void V_ReflectWaveSpawn(float repeatPos, float waveAngle, float waveMaxSize)
    //{

    //    // 波の向きを出ていく向きに設定
    //    waveCollision.waveVelocity = waveOutPutVelocity[inputVFXNumber];

    //    // vfxの波を生成し、波の番号を一時的に保存
    //    var num = vfxManagers[inputVFXNumber].WaveSpawn(repeatPos, waveAngle * Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[waveCollision.waveNum]), waveMaxSize, vfxManagers[inputVFXNumber].waveWidthArray[waveCollision.waveNum]);
    //    // 波のコリジョンスクリプトに出ていく波のvfxManagerを代入
    //    waveCollision.vfxManager = vfxManagers[inputVFXNumber];
    //    // コリジョンのvfx上の番号を生成した波のものに設定
    //    waveCollision.waveNum = num;
    //    // コリジョンのスケールを初期化
    //    waveColliderObj.transform.localScale = new Vector3(0, 0, 1);
    //    // コリジョンの初期位置YをrepeatPosに変更
    //    var trans = waveCollision.transform.position;
    //    trans.y = repeatPos;
    //    // Xをこのオブジェの位置に変更
    //    trans.x = transform.position.x;
    //    // 変更した結果を代入
    //    waveColliderObj.transform.position = trans;
    //    // 波の開始地点を設定
    //    waveCollision.waveStartPosition = trans;
    //    // 波の1周期の大きさを設定
    //    waveCollision.waveMaxSize = Mathf.Abs(vfxManagers[inputVFXNumber].waveSpeedArray[num] * vfxManagers[inputVFXNumber].waveWidthArray[num]);
    //    // 波の最大の高さを設定
    //    waveCollision.maxWaveHight = waveMaxSize;
    //    // 発生からの経過時間を初期化
    //    waveCollision.waveElapsedTime = 0;
    //    // 波の終了フラグを切る
    //    waveCollision.waveEndFg = 0;
    //    // 波を移動させる
    //    waveCollision.waveFg = 2;
    //    // リピートフラグ解除
    //    waveCollision.repeatFg = false;
    //    repeatSetFg = false;
    //    waveColliderObj = null;
    //    waveCollision = null;
    //}

    void RepeatorSetUp()
    {
        //if (groundObj == null)
        //{
        //    Debug.LogError("作用させる糸を設定してください");
        //}
        // 設定した作用する糸のオブジェクトの数、vfxManagerを取得する
        for (int i = 0; i < groundObj.Count; i++)
        {
            // グラウンドオブジェがその配列番号に設定されている場合
            if (groundObj[i] != null)
            {
                // 糸の子オブジェクトに存在するvfxManagerを取得
                vfxManagers.Add(groundObj[i].transform.GetChild(0).GetComponent<vfxManager>());
                // リピーターと糸が触れている状態にする。
                enterFg.Add(true);
                // 存在するvfxの数を保存
                vfxCount++;
            }
        }



        // 設定した糸によって、波が入ってくる向き、出ていく向きを確定する。
        for (int i = 0; i < groundObj.Count; i++)
        {
            // 縦糸の場合
            if (vfxManagers[i].warpWave == true)
            {
                // オブジェクトが上にあれば
                if (groundObj[i].transform.position.y > transform.position.y)
                {
                    // 入力の可能性を下に
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                    // 出力を上に設定
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                }
                // 下にあれば
                else
                {
                    // 入力の可能性を上に
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
                    // 出力を下に設定
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
                }
            }
            // 横糸の場合
            else
            {
                // オブジェクトが右にあれば
                if (groundObj[i].transform.position.x > transform.position.x)
                {
                    // 入力の可能性を左に
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                    // 出力を右に設定
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                }
                // 左であれば
                else
                {
                    // 入力の可能性を右に
                    waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
                    // 出力を左に設定
                    waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
                }

            }
        }
    }

    private void Start()
    {
        col = GetComponent<Collider>();

        vfxCount = 0;
        for(byte i = 0;i<groundObj.Count;i++)
        {
            if(groundObj[i] == null)
            {
                groundObj.RemoveAt(i);
                i--;
            }
        }
        vfxManagers.Clear();
        waveInputVelocity.Clear();
        waveOutPutVelocity.Clear();
        enterFg.Clear();

        switch (machineMode)
        {
            case AdderSubtractor.adder:

                RepeatorSetUp();
                break;

            case AdderSubtractor.adderRight:

                RepeatorSetUp();
                break;

            case AdderSubtractor.adderLeft:

                RepeatorSetUp();
                break;

            case AdderSubtractor.adderBoth:

                
                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractor:

                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractorRight:

                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractorLeft:

                RepeatorSetUp();
                break;

            case AdderSubtractor.subtractorBoth:

                
                RepeatorSetUp();
                break;

            case AdderSubtractor.VtoH_Connect:

                RepeatorSetUp();
                break;

            case AdderSubtractor.none:

                RepeatorSetUp();
                break;

            default:
                break;

        }

       // if(setUP == SpringSetUP.ON)
        {
            setUpFg = true;
        }
        //else if(setUP == SpringSetUP.OFF)
        //{
        //    setUpFg = false;
        //}
        //// モードが縦横の接続の場合
        //if (machineMode == AdderSubtractor.VtoH_Connect)
        //{
        //    RepeatorSetUp();
        //}

        //// モードが縦横の接続の場合
        //if (machineMode == AdderSubtractor.adder)
        //{
        //    RepeatorSetUp();
        //    //if (groundObj == null)
        //    //{
        //    //    Debug.LogError("作用させる糸を設定してください");
        //    //}
        //    //// 設定した作用する糸のオブジェクトの数、vfxManagerを取得する
        //    //for (int i = 0; i < groundObj.Count; i++)
        //    //{
        //    //    // グラウンドオブジェがその配列番号に設定されている場合
        //    //    if (groundObj[i] != null)
        //    //    {
        //    //        // 糸の子オブジェクトに存在するvfxManagerを取得
        //    //        vfxManagers.Add(groundObj[i].transform.GetChild(0).GetComponent<vfxManager>());
        //    //        // 存在するvfxの数を保存
        //    //        vfxCount++;
        //    //    }
        //    //}



        //    //// 設定した糸によって、波が入ってくる向き、出ていく向きを確定する。
        //    //for (int i = 0; i < groundObj.Count; i++)
        //    //{
        //    //    // 縦糸の場合
        //    //    if (vfxManagers[i].warpWave == true)
        //    //    {
        //    //        // オブジェクトが上にあれば
        //    //        if (groundObj[i].transform.position.y > transform.position.y)
        //    //        {
        //    //            // 入力の可能性を下に
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
        //    //            // 出力を上に設定
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
        //    //        }
        //    //        // 下にあれば
        //    //        else
        //    //        {
        //    //            // 入力の可能性を上に
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.UP);
        //    //            // 出力を下に設定
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.DOWN);
        //    //        }
        //    //    }
        //    //    // 横糸の場合
        //    //    else
        //    //    {
        //    //        // オブジェクトが右にあれば
        //    //        if (groundObj[i].transform.position.x > transform.position.x)
        //    //        {
        //    //            // 入力の可能性を左に
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
        //    //            // 出力を右に設定
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
        //    //        }
        //    //        // 左であれば
        //    //        else
        //    //        {
        //    //            // 入力の可能性を右に
        //    //            waveInputVelocity.Add(waveCollition.WAVE_VELOCITY.RIGHT);
        //    //            // 出力を左に設定
        //    //            waveOutPutVelocity.Add(waveCollition.WAVE_VELOCITY.LEFT);
        //    //        }

        //    //    }
        //    //}
        //}

        //if (machineMode == AdderSubtractor.adderBoth)
        //{

        //    playerScript = GameObject.FindWithTag("Player").GetComponent<MovePlayer3_3>();
        //    if (playerScript == null)
        //    {
        //        Debug.LogError("プレイヤースクリプトが見つかりません");
        //    }
        //    RepeatorSetUp();
        //}
    }

    //private void Update()
    //{
    //if(modeSetFg == 0)
    //{
    //    for (int i = 0; i < vfxManagers.Length; i++)
    //    {
    //        if (vfxManagers[i].warpWave == true)
    //        {
    //            machineMode = AdderSubtractor.VtoH_Connect;
    //            modeSetFg = 1;
    //        }
    //    }
    //}

    //if (repeatSetFg == true)
    //{
    // 入ってきた波の向きに応じたアウトプット処理を行う。
    //VtoH_InputCheck();
    //}

    //}

    

    private void FixedUpdate()
    {
        if(setUP == SpringSetUP.OFF)
        {
            if (startSetCheckTime < endSetCheckTime)
            {
                startSetCheckTime++;
            }
            else
            {
                setUpFg = false;
            }
        }
        for(int i = 0;i<vfxCount;i++)
        {
            // 縦糸の場合
            if (vfxManagers[i].warpWave == true)
            {
                // オブジェクトが上にあれば
                if (groundObj[i].transform.position.y > transform.position.y)
                {
                    // 入力の可能性を下に
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.DOWN);
                    // 出力を上に設定
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.UP);
                }//-----if_stop-----
                 // 下にあれば
                else
                {
                    // 入力の可能性を上に
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.UP);
                    // 出力を下に設定
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.DOWN);
                }//-----else_stop-----
            }//-----if_stop-----
             // 横糸の場合
            else
            {
                // オブジェクトが右にあれば
                if (groundObj[i].transform.position.x > transform.position.x)
                {
                    // 入力の可能性を左に
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.LEFT);
                    // 出力を右に設定
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.RIGHT);
                }//-----if_stop-----
                 // 左であれば
                else
                {
                    // 入力の可能性を右に
                    waveInputVelocity[i] = (waveCollition.WAVE_VELOCITY.RIGHT);
                    // 出力を左に設定
                    waveOutPutVelocity[i] = (waveCollition.WAVE_VELOCITY.LEFT);
                }//-----else_stop-----
            }//-----else_stop-----
        }
    }


    private void OnApplicationQuit()
    {
        groundObj.Clear();
        vfxManagers.Clear();
        waveInputVelocity.Clear();
        waveOutPutVelocity.Clear();
        enterFg.Clear();
    }
}

