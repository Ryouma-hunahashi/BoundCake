using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_Unlock : MonoBehaviour
{
    // 自身の情報を取得
    private Vector3 m_Position;     // 自身の座標
    private Vector3 m_Scale;        // 自身の大きさ

    [SerializeField] private LayerMask waveLayer;

    // どの面で判定をとるか
    private enum SurfaceDistanceType
    {//----- enum_start -----

        top,    // 上面
        left,   // 左面
        right,  // 右面
        bottom, // 下面

        none,   // なし

    }//----- enum_stop -----

    // 判定の取得方向を決める
    [Header("----- 接触面の設定 -----"),Space(5)]
    [Tooltip("設定された面に[Wave]が当たると破壊されます")]
    [SerializeField] private SurfaceDistanceType blockSurfaceDistance = SurfaceDistanceType.none;
    private SurfaceDistanceType m_DistanceType;     // 接地面の設定の保持
    private bool changeDistanceType = false;        // 接地面変更の許可

    [Header("----- レイの設定 -----")]
    [Tooltip("接触可能距離を設定")]
    [SerializeField] private float rayDirection;    // レイの距離を設定
    private float changeRayDirectionX;              // レイの向きを[X]軸変更
    private float changeRayDirectionY;              // レイの向きを[Y]軸に変更

    private void Start()
    {
        // 自身の情報の保持
        m_Position = transform.position;
        m_Scale = transform.localScale;
    }

    private void Update()
    {
        // 自身の座標に変更が加えられたとき
        if(m_Position != this.transform.position)
        {//----- if_start -----

            // 自身の保持している座標を更新する
            m_Position = this.transform.position;

        }//----- if_stop -----

        // 設定されている破壊可能な面で処理を変更
        switch(blockSurfaceDistance)
        {//----- switch_start -----

            case SurfaceDistanceType.top:

                // レイの向きを変更する
                changeRayDirectionX = 0;
                changeRayDirectionY = 1;

                // レイを出現させる
                RaySetting();

                break;
            case SurfaceDistanceType.left:

                // レイの向きを変更する
                changeRayDirectionX = -1;
                changeRayDirectionY = 0;

                // レイを出現させる
                RaySetting();

                break;
            case SurfaceDistanceType.right:

                // レイの向きを変更する
                changeRayDirectionX = 1;
                changeRayDirectionY = 0;

                // レイを出現させる
                RaySetting();

                break;
            case SurfaceDistanceType.bottom:

                // レイの向きを変更する
                changeRayDirectionX = 0;
                changeRayDirectionY = -1;

                // レイを出現させる
                RaySetting();

                break;
            // 麺が設定されていなかった場合のエラーログ
            case SurfaceDistanceType.none:
            default:
                // エラーを表示させる
                Debug.LogError("破壊可能な面が設定されていません！");
                break;

        }//----- switch_stop -----
    }

    private void RaySetting()
    {
        // レイの位置,方向を設定する
        Ray hitRay = new Ray(new Vector3(m_Position.x, m_Position.y, m_Position.z), new Vector3(changeRayDirectionX, changeRayDirectionY, 0));
        RaycastHit hitWave;

        // レイを画面上から見えるようにする
        Debug.DrawRay(hitRay.origin, hitRay.direction * rayDirection, Color.red, 1, false);

        // レイが[Wave]に当たったときの処理
        if(Physics.Raycast(hitRay, out hitWave, rayDirection, waveLayer))
        {//----- if_start -----

            // このあたりは振動の方向とか取得してからやってもいいかも

            // このオブジェクトを破壊する
            Destroy(this.gameObject);

        }//----- if_stop -----
    }
}
