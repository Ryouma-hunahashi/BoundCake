using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボスのメインスクリプト
// ※各挙動を組み合わせてボスの行動を変化させます
//==================================================
// 制作日2023/05/16
// 宮﨑
public class Boss1_Main : MonoBehaviour
{
    // 自身の情報を取得
    private Rigidbody rb;

    // 自身のアニメーター
    private Animator anim;

    public BossStatusManager bossStatus;   // ボスのステータス

    // ボスの行動
    private Boss1_Rush boss_Rush;       // 突進攻撃
    private Boss1_Shield boss_Shield;   // 防御行動


    public BossAudio audio;
    [SerializeField]private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;
    
    // コルーチンの格納
    public IEnumerator cor_DmgReaction;

    public bool nowDmgAct;

    [Header("ボス稼働設定")]
    [Tooltip("行動を開始させます")]
    public bool startAction;    // 行動開始
    [Tooltip("無敵状態")]
    public bool invincibility;  // 無敵状態
    [Tooltip("ボスラストヒット時の吹き飛び速度")]
    [SerializeField] private sbyte distance_KB = 35;
    [Tooltip("着地時スタンフレーム")]
    [SerializeField] private byte stanFrame = 60;

    [Header("ヒットストップの設定")]
    [Tooltip("どれくらいのストップをかけるか")]
    [SerializeField, Range(0f, 1f)] private float stopSpeed = 0.5f;
    [Tooltip("ヒットストップ時間")]
    [SerializeField] private ushort stopFrame = 45;

    // ヒットストップ稼働状態
    private bool activeHitStop;

    // 消去フラグ
    private bool deleteFg = false;

    private GameObject waveHit;
    private GameObject waveLog;

    //[SerializeField] private GameObject bossCamera;
    //private GoalCamera goalCamera;
    private bool startLog = false;
    private bool startSet = false;

    private void OnTriggerEnter(Collider other)
    {
        // 無敵状態なら処理を抜ける
        if (invincibility) return;

        // ダメージアクション中なら処理を抜ける
        if (nowDmgAct) return;

        // 振動に触れた時
        if (other.CompareTag("Wave"))
        {
            // 防御している状態なら
            if (boss_Shield.nowShield)
            {
                waveHit = other.gameObject;

            }//----- if_stop -----

            // 盾が破壊されているなら
            if (boss_Shield.shieldBreak && !invincibility)
            {
                if (waveHit == null)
                {
                    // 自身にダメージを与えられない状態にする
                    invincibility = true;

                    waveHit = other.gameObject;

                    for (int i = 0; i < boss_Shield.waveCount; i++)
                    {
                        if (boss_Shield.l_waveCollisionObj[i] == waveHit)
                        {
                            return;

                        }//----- if_stop -----

                    }//----- for_stop -----
                    
                    audio.bossSource.Stop();
                    
                    audio.bossSource.loop = false;
                    audio.BossCrushing_1Sound();
                    // ダメージアニメーションを開始
                    anim.Play("DamageUp");
                    anim.SetBool("staning", false);

                    bossStatus.hitPoint--;

                    // フェイズを進行させる
                    if (boss_Shield.nowPhase < boss_Shield.vitPhase.Length - 1)
                    {
                        boss_Shield.nowPhase++;

                    }//----- if_stop -----

                    // 最終フェイズではないとき
                    if (!boss_Shield.lastPhase)
                    {
                        StopCoroutine(cor_DmgReaction);
                        cor_DmgReaction = DamageAction();
                        StartCoroutine(cor_DmgReaction);

                    }//----- if_stop -----

                    // 更新した波がログと一致しないなら
                    if (waveHit != waveLog)
                    {
                        // 波のログを更新する
                        waveLog = waveHit;

                    }//----- if_stop -----

                }//----- if_stop -----

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // 振動に触れていた時
        if (other.CompareTag("Wave"))
        {
            // 触れた波が格納されてるなら
            if (waveHit != null)
            {
                waveHit = null;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void Start()
    {
        // 自身の情報を取得
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        audio = GetComponent<BossAudio>();
        // 自身の挙動情報を取得
        boss_Rush = GetComponentInChildren<Boss1_Rush>();
        boss_Shield = GetComponentInChildren<Boss1_Shield>();
        

        //goalCamera = bossCamera.GetComponent<GoalCamera>();
        startLog = g_camera.BossStart;

        // コルーチンの格納
        cor_DmgReaction = DamageAction();

        bossStatus.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        if(!startSet&&startLog!=g_camera.BossStart)
        {
            bgm.BossBattleStart();
            startAction = true;
            startSet = true;
        }
        if (startAction)
        {
            // 回転を開始する
            StartCoroutine(boss_Shield.Rotate());

            // 開始後状態を切る
            startAction = false;
        }//----- if_stop -----

        if (boss_Shield.lastPhase && (bossStatus.hitPoint <= 0) && !activeHitStop)
        {
            StartCoroutine(BossKnockOut());
            activeHitStop = true;
        }
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

    private IEnumerator HitStop()
    {
        Debug.Log("ヒットストップを開始しますっ！");

        Time.timeScale = stopSpeed;

        for (int i = 0; i < stopFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        Time.timeScale = 1f;

        // 彼方へ吹き飛ばす
        StartCoroutine(BossKnockOut());
    }

    private IEnumerator BossKnockOut()
    {
        sbyte KO_LowerLimit = (sbyte)-distance_KB;

        while (rb.velocity.y > KO_LowerLimit * 1.5)
        {
            // １フレーム遅延させる
            yield return null;

            if (distance_KB == 0)
            {
                Debug.Log("ヒットストップを開始しますっ！");

                Time.timeScale = stopSpeed;

                for (int i = 0; i < stopFrame; i++)
                {
                    // １フレーム遅延させる
                    yield return null;

                }//----- for_stop -----

                Time.timeScale = 1f;

            }//----- if_stop -----

            // 加速させる
            rb.velocity = new Vector3(0, distance_KB, 0);
            distance_KB--;

        }//----- while_stop -----
        deleteFg = true;
    }

    public IEnumerator DamageAction()
    {
        Debug.Log("ダメージリアクション開始");

        nowDmgAct = true;

        sbyte KB_LowerLimit = (sbyte)-distance_KB;
        sbyte KB_distanceHold = (sbyte)distance_KB;

        while (rb.velocity.y >= KB_LowerLimit)
        {
            // １フレーム遅延させる
            yield return null;
            if(rb.velocity.y<0)
            {
                anim.SetBool("DamageFall", true);
            }
            // 加速させる
            rb.velocity = new Vector3(0, KB_distanceHold, 0);
            KB_distanceHold--;

        }//----- while_stop -----


        // 着地処理
        anim.SetBool("DamageFall", false);
        rb.velocity = Vector3.zero;
        rb.position = boss_Rush.fallPos;
        audio.bossSource.Stop();
        audio.BossStunSound();
        audio.bossSource.loop = true;
        anim.SetBool("staning", true);
        boss_Shield.nowStan = true;

        // 指定した時間スタン状態にする
        for (int i = 0; i < stanFrame; i++)
        {
            yield return null;

        }//----- for_stop -----

        // スタンアニメーション終了
        anim.SetBool("staning", false);
        audio.bossSource.loop = false;
        if (audio.bossSource.isPlaying)
        {
            audio.bossSource.Stop();
        }
        boss_Shield.nowStan = false;

        nowDmgAct = false;

        StopCoroutine(boss_Shield.cor_Repair);
        boss_Shield.cor_Repair = boss_Shield.ShieldRepair();
        StartCoroutine(boss_Shield.cor_Repair);
    }
}
