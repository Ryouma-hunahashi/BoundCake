using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_Main : MonoBehaviour
{
    public bool startAction;

    private Rigidbody rb;
    public Animator anim;

    [SerializeField] BossStatusManager status;

    public BossAudio audio;

    private Boss2_Ball bossBall;

    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    // コルーチンの格納
    public IEnumerator cor_Damage;

    public bool nowDmgReAct;

    private bool invincibility = false;
    private byte invincibilityFrame = 120;

    // ボス吹き飛び速度
    [SerializeField] private sbyte knockBackSpeed = 30;

    // スタン状態の時間
    [SerializeField] private byte stanFrame = 90;

    // ヒットストップ
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f;
    [SerializeField] private sbyte hitStopFrame = 30;

    // 待機時間
    [SerializeField] private byte atkDelayFrame = 150;

    // 現在の状態
    public bool nowStan;
    public bool nowHitStop;

    // 波当たり判定
    private GameObject waveHit;
    private GameObject waveLog;

    // 波の設定
    [Header("----- 波の設定 -----")]
    public vfxManager vfxManager;   // 爆弾が作用する糸のvfxManager
    private sbyte arrayNum = 0;     // 波を生成するとき戻り値をクローンへ
    [SerializeField] private float waveSpeed = 7.5f;    // 波の速度
    [SerializeField] private float waveWidth = 0.225f;  // 波の振動数
    [SerializeField] private float waveHight = 2.0f;    // 波の高さ
    [SerializeField] private GameObject waveObj;        // 波の判定プレハブ
    public int waveCount = 3;   // プールに生成する波の数
    [SerializeField] public int waveAngle = -1;  // 波の方向

    // 波のコリジョンのオブジェクトプール
    [System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();

    // 波コリジョンにコンポーネントされている波判定のプール
    // 添え字は波判定のプールに対応
    private List<waveCollition> l_waveCollitions = new List<waveCollition>();

    bool deleteFg = false;

    private bool startLog = false;
    private bool startSet = false;

    [System.NonSerialized] public WavePool pool;

    private void OnTriggerEnter(Collider other)
    {
        // 無敵状態,ダメージアクション中なら処理を抜ける
        if (invincibility || nowDmgReAct) return;

        // 振動に触れた時
        if (other.CompareTag("Wave"))
        {
            if (waveHit == null)
            {
                if (!other.GetComponent<waveCollition>().CheckType(waveCollition.WAVE_TYPE.ENEMY))
                {
                    // 自身にダメージを与えられない状態にする
                    invincibility = true;

                    waveHit = other.gameObject;

                    // ダメージアニメーションを開始 ----------
                    anim.Play("Damage");
                    anim.SetBool("Damaging", true);
                    audio.bossSource.loop = false;
                    audio.bossSource.Stop();
                    audio.BossCrushing_1Sound();

                    status.hitPoint--;

                    // 最終フェイズではないとき
                    if (status.hitPoint > 0)
                    {
                        StopCoroutine(cor_Damage);
                        cor_Damage = DamageReaction();
                        StartCoroutine(cor_Damage);

                    }//----- if_stop ------

                    // 更新した波がログと一致しないなら
                    if (waveHit != waveLog)
                    {
                        // 波のログを更新する
                        waveLog = waveHit;

                    }//----- if_stop -----
                }
            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // 振動に触れていた時
        if (other.CompareTag("Wave"))
        {
            // 触れた波が格納されているなら
            if (waveHit != null)
            {
                waveHit = null;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        bossBall = GetComponentInChildren<Boss2_Ball>();
        anim = GetComponent<Animator>();
        cor_Damage = DamageReaction();
        audio = GetComponent<BossAudio>();
        pool = GetComponent<WavePool>();
        pool.AddEtoPDamage();

        startLog = g_camera.BossStart;

        // 波の各情報を取得
        for (int i = 0; i < waveCount; i++)
        {
            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
            l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        }//----- for_stop -----

        status.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        if (!startSet && startLog != g_camera.BossStart)
        {
            bgm.BossBattleStart();
            startAction = true;
            startSet = true;
        }
        if (startAction)
        {
            anim.SetBool("ballSetting",true);
            audio.bossSource.Stop();
            audio.bossSource.loop = false;
            audio.BossTearBallSound();
            StartCoroutine(bossBall.SpawnBallDelay());
            startAction = false;
        }

        // 自身の体力が０になった
        // ヒットストップ状態に入っていないなら
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            audio.bossSource.Stop();
            audio.bossSource.loop=false;
            audio.BossCrushing_1Sound();
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----

        if(deleteFg)
        {
            if (!audio.bossSource.isPlaying)
            {
                // 自身を非表示にする
                Debug.Log(this.gameObject.name + "を非表示にしましたっ！");
                g_camera.BossEnd = true;
                this.gameObject.SetActive(false);
            }
        }
    }

    public IEnumerator AttackDelayTime()
    {
        for (int i = 0; i < atkDelayFrame; i++)
        {
            yield return null;
        }
        anim.SetBool("ballSetting", true);
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossTearBallSound();
        // 玉の射出を行う
        StartCoroutine(bossBall.SpawnBallDelay());
    }

    private IEnumerator DamageReaction()
    {
        Debug.Log("ダメージリアクションを開始しましたっ！");

        // ダメージリアクション状態にする
        nowDmgReAct = true;

        // リアクション開始地点を保持
        Vector3 holdReactPos = transform.position;

        sbyte knockBack_LL = (sbyte)-knockBackSpeed;
        sbyte knockBackDistHold = (sbyte)knockBackSpeed;

        // 落下速度が下限を下回るまで繰り返す
        while (rb.velocity.y >= knockBack_LL)
        {
            // 自身に速度をつける
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            if (rb.velocity.y < 0)
            {
                anim.SetBool("Falling", true);
            }
            knockBackDistHold--;

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        // 着地処理
        rb.velocity = Vector3.zero;     // 速度を０にする
        rb.position = holdReactPos;     // 開始地点で固定する

        // スタンアニメーション開始 ----------
        anim.SetBool("Falling",false);
        anim.SetBool("Damaging", false);
        audio.bossSource.Stop();

        nowStan = true;

        // 指定した時間の間スタン状態にする
        for (int i = 0; i < stanFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        // スタンアニメーション終了 ----------

        nowStan = false;

        // ダメージリアクション状態を解除
        nowDmgReAct = false;

        for(int i = 0; i < invincibilityFrame; i++)
        {
            yield return null;
        }

        invincibility = false;
    }

    private IEnumerator KnockOut()
    {
        sbyte knockOut_LL = (sbyte)-knockBackSpeed;

        // 落下速度が下限を超えた後としばらくの間繰り返す
        while (rb.velocity.y > knockOut_LL * 1.5)
        {
            // 上昇最高地点くらいに到達したなら
            if (knockBackSpeed == 0)
            {
                Debug.Log("ヒットストップを開始しますっ！");

                // ゲームスピードを指定した値に変更する
                Time.timeScale = hitStop;

                // ヒットストップの継続時間の間繰り返す
                for (int i = 0; i < hitStopFrame; i++)
                {
                    // １フレーム遅延させる
                    yield return null;

                }//----- for_stop -----

                // ゲームスピードを通常速度に戻す
                Time.timeScale = 1f;

            }//----- if_stop -----

            // 自身に速度をつける
            rb.velocity = new Vector3(0, knockBackSpeed, 0);
            knockBackSpeed--;

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----
        deleteFg = true;
        
    }

    public void WaveCreate(float startPosX, Transform rayTrans)
    {
        pool.WaveCreate(waveSpeed * waveAngle, waveWidth, waveHight,
            waveCollition.WAVE_TYPE.ENEMY, startPosX, rayTrans);
    }
    //{
    //    // 配列番号を指定する
    //    arrayNum = vfxManager.WaveSpawn(startPosX, waveSpeed * waveAngle, waveHight, waveWidth, 1);

    //    for (int i = 0; i < waveCount; i++)
    //    {
    //        if (l_waveCollisionObj[i].transform.position.z != 0)
    //        {
    //            // 当たり判定に番号を指定
    //            l_waveCollitions[i].waveNum = arrayNum;
    //            // クローンに対応させる vfxManager を設定
    //            l_waveCollitions[i].vfxManager = vfxManager;
    //            // 波をプレイヤーが発生させた波に設定
    //            l_waveCollitions[i].waveType = waveCollition.WAVE_TYPE.ENEMY;
    //            // コリジョンを動かす
    //            l_waveCollitions[i].waveMode = waveCollition.WAVE_MODE.PLAY;
    //            // コリジョンの高さを波の高さに設定
    //            l_waveCollitions[i].transform.localScale = new Vector3(0, 0, 1);
    //            // コリジョンの発生位置を波の発生位置に設定
    //            l_waveCollitions[i].waveStartPosition = new Vector3(startPosX, startPosY, 0);
    //            // コリジョンの最大高度を設定
    //            l_waveCollitions[i].maxWaveHight = waveHight;

    //            break;
    //        }//----- if_stop -----
    //        else if (i == waveCount - 1)
    //        {
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }//----- elseif_stop -----

    //    }//----- for_stop -----
    //}

    private void OnApplicationQuit()
    {
        l_waveCollisionObj.Clear();
        l_waveCollitions.Clear();
    }
}
