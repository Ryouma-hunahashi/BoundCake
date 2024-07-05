using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Wave : MonoBehaviour
{


    // 親の子にあるスクリプトを取得
    private Player_Fall pl_Fall;    // 落下用スクリプト
    private WavePool pool;

    [Header("----- 波の設定 -----"), Space(5)]
    
    [System.NonSerialized] public sbyte waveAngle = 1;//プレイヤーの向き、正なら左向き、負なら右向き

    [Header("誰の波か")]
    [SerializeField] private waveCollition.WAVE_TYPE waveType = waveCollition.WAVE_TYPE.PLAYER;

    [Header("波の速度")]
    [SerializeField] private float waveSpeed = 7.5f;
    [Header("波の振動数")]
    [SerializeField] private float waveWidth = 0.225f;
    //private int waveStartPos = 0;
    [Header("固定波の高さ")]
    public float waveHight = 2.0f;
    [Header("波の高さ指数")]
    [Tooltip("着地時のエネルギーから何倍か")]
    public float waveHightIndex = 0.06f;

    [Header("----- 床を沈ませる設定 -----"), Space(5)]
    [SerializeField] private float maxLandDistance = 0.5f;
    [SerializeField] private float landSpeed = 15;


    // Start is called before the first frame update
    void Start()
    {
        //for (byte i = 0; i < waveCount; i++)
        //{
        //    // 初期の波コリジョンの数、構造体リストに追加
        //    l_collisions.Add(WaveCollisionSet());

        //}//-----for_atop-----
        pool = GetComponent<WavePool>();
        pl_Fall = transform.parent.GetComponentInChildren<Player_Fall>();
    }





    //==================================================
    // 波の情報をVFXに代入し、コリジョンとともに波を発生させる。
    // 第一引数 : 波を発生させる方向のRaycastHit
    // 第二引数 : 発生させる波の高さ
    //==================================================
    // 制作日2023/03/21    更新日2023/04/14
    // 高田　中村
    public void WaveCreate(RaycastHit groundHit, float waveHight, float waveSpornPosY)
    {
        if (pool.WaveCreate(waveSpeed*waveAngle, waveWidth, waveHight, waveType, groundHit.point.x, groundHit.transform))
        {
            groundHit.transform.GetComponent<LandingGround>().lnadGround(Mathf.Abs(maxLandDistance * pl_Fall.fallPowLog / pl_Fall.maxFallPow), landSpeed);
            if (pl_Fall.fallPowLog < -24)
            {
                string a = "strong";
                Vibration_Manager.instanse.VibrationSelect(a);
            }
            else
            {
                string a = "weak";
                Vibration_Manager.instanse.VibrationSelect(a);
            }
        }
        
    }


    //// ゲーム終了時にリストを破棄する。
    //private void OnApplicationQuit()
    //{
    //    l_collisions.Clear();
    //}
}
