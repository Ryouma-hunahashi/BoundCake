using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 作成日2023/03/11    更新日2023/03/12
// 宮﨑
public class Test_FPS : MonoBehaviour
{
    // フレームレート計測管理
    // フレームレート表示の設定
    [Tooltip("フレームレートを表示するか")]
    [SerializeField] private bool displayFrameRate;

    // 表示フレームを更新する間隔
    [Tooltip("表示間隔を変更")]
    [SerializeField] private float frameInterval = 0.5f;

    // フレームレートの指定
    [Tooltip("フレームレートの設定")]
    [SerializeField] private int frameRate = 60;

    // フレームレート変更専用の変数
    private int frameRateRevision;

    public static Test_FPS instance;

    // 
    private float m_timeCount;
    private int m_frame;

    private float m_time_mn;
    public float m_fps;

    //いつか使うかもしれないシングルトン
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {

        // フレームレートを一時的に保持
        frameRateRevision = frameRate;

        // フレームレートの指定
        Application.targetFrameRate = frameRate;
    }

    private void Update()
    {
        // フレームレートが更新されたときの処理
        if (frameRate != frameRateRevision)
        {//----- if_start -----

            // 修正後のフレームレートを一時的に保持
            frameRateRevision = frameRate;

            // フレームレートの指定
            Application.targetFrameRate = frameRate;

            // フレームレート更新のログ
            Debug.Log("フレームレートを(" + frameRate + "Frame)に変更しました！");

        }//----- if_stop -----

        // 時間の計測
        m_time_mn -= Time.deltaTime;
        m_timeCount += Time.timeScale / Time.deltaTime;
        m_frame++;

        // 経過時間内なら処理を抜ける
        if (0 < m_time_mn)
        {//----- if_start -----

            return;

        }//----- if_stop -----

        // fpsの計算
        m_fps = m_timeCount / m_frame;

        // fpsカウントの初期化
        m_time_mn = frameInterval;
        m_timeCount = 0;
        m_frame = 0;

    }

    // フレームレートの表示処理
    // 作成日2023/03/11
    // 宮﨑
    private void OnGUI()
    {
        // フレームレートを表示するか
        if (displayFrameRate)
        {//----- if_start -----

            // 画面内にFPSを表示する(例：FPS:60)
            GUILayout.Label("FPS : " + m_fps.ToString("f0"));

        }//----- if_stop -----
    }
}
