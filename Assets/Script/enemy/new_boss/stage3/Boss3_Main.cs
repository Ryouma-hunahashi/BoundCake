using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボスのメインスクリプト
// ※攻撃や召喚、ダメージなどの主軸を司ります　
//==================================================
// 制作日2023/05/26    更新日2023/05/28
// 宮﨑
public class Boss3_Main : MonoBehaviour
{
    [SerializeField] private bool startAction;

    // 指令の情報
    private GameObject bombOperator;
    private Setup_BombPointer bompSetup;
    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    // 回転床の保持値
    public Rotation groundRotation;         // 回転床の取得
    private float holdRotateValue;          // 回転床の初期値
    public float stanRotateValue = 0.2f;    // スタン中の値

    // 自身の情報
    private Rigidbody rb;
    public Animator anim;
    public BossAudio audio;

    // 挙動の情報
    private Boss3_Bomb boss_Bomb;

    // コルーチンの格納
    public IEnumerator cor_Damage;

    // ボスの設定
    public BossStatusManager status;

    // 出現地点の設定

    // 無敵状態
    [SerializeField] private bool invincibility = true;

    // 待機時間の設定
    [SerializeField] private byte atkDelayFrame = 60;

    // 被ダメージ持の設定
    [SerializeField] private sbyte knockBackSpeed = 30;

    // スタン状態の設定
    [SerializeField] private byte stanFrame = 30;
    [SerializeField] private byte blastedStanFrame = 240;

    // ヒットストップ
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f; 
    [SerializeField] private byte hitStopFrame = 30;

    // 現在の状態

    public bool nowAtkDelay;
    public bool nowDmgReAct;
    public bool nowStan;
    public bool nowHitStop;

    // 波当たり判定
    private GameObject waveHit;
    private GameObject waveLog;

    private bool startLog = false;
    private bool startSet = false;


    private void OnTriggerEnter(Collider other)
    {
        // 無敵状態,ダメージアクション中なら処理を抜ける
        if (invincibility || nowDmgReAct) return;

        // 振動に触れた時
        if (other.CompareTag("Wave"))
        {
            if (waveHit == null)
            {
                if (!other.GetComponent<waveCollition>().CheckType( waveCollition.WAVE_TYPE.ENEMY))
                {
                    // 自身にダメージを与えられない状態にする
                    invincibility = true;

                    waveHit = other.gameObject;

                    // ダメージアニメーションを開始 ----------

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
        // 指令の情報を取得
        bombOperator = GameObject.Find("BombPointOperator").gameObject;
        bompSetup = bombOperator.GetComponent<Setup_BombPointer>();

        // 回転の初期値を保持しておく
        holdRotateValue = groundRotation.angularVelocity;

        // 自身の情報を取得
        GameObject thisObj = this.transform.gameObject;
        rb = thisObj.GetComponent<Rigidbody>();
        
        // 自身の挙動を取得
        boss_Bomb = thisObj.GetComponentInChildren<Boss3_Bomb>();
        anim = GetComponent<Animator>();
        audio = GetComponent<BossAudio>();

        // コルーチンの格納
        cor_Damage = DamageReaction();

        // HPの設定
        status.hitPoint = 3;
        startLog = g_camera.BossStart;

        // 爆弾を生成した床の子オブジェのvfxManagerを取得
        //vfxManager = transform.parent.transform.parent.GetChild(0).GetComponentInChildren<vfxManager>();

        // 自身を回転床(Operator)の子オブジェクトにする ----- 出現タイミングに毎回呼び出しも可
        //GameObject bomb_OP = GameObject.Find("BombPointOperator").gameObject;
        //this.transform.SetParent(bomb_OP.transform, false);
    }

    private void FixedUpdate()
    {

        if (!startSet && startLog != g_camera.BossStart)
        {
            bgm.BossBattleStart();
            startAction = true;
            startSet = true;
        }
        // 自身の体力が０になった
        // ヒットストップ状態に入っていないなら
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----

        // キノコ爆弾が爆発したなら
        if (bompSetup.blasted)
        {
            // スタン状態に移行する
            StartCoroutine(StanAfterBlast());
            bompSetup.blasted = false;
        }
    }

    private IEnumerator AttackDelayTime()
    {
        // 攻撃開始までの待機状態を開始
        nowAtkDelay = true;

        // 潜るアニメーション開始 ----------
        anim.SetBool("Grawing", false);
        anim.Play("Dive");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.Boss3_MoveSound();

        for(int i = 0; i < atkDelayFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;
        }

        // 攻撃を開始する
        nowAtkDelay = false;

        // キノコ爆弾を召喚する
        StartCoroutine(boss_Bomb.SpawnBomb());
    }

    private IEnumerator StanAfterBlast()
    {
        // 爆発した後のスタン
        invincibility = false;

        // 床の回転速度を設定した値に変更する
        groundRotation.angularVelocity = stanRotateValue;

        // スタンアニメーションを開始 -----------
        anim.SetBool("Staning", true);
        audio.bossSource.Stop();
        audio.bossSource.loop = true;
        audio.BossStunSound();

        nowStan = true;

        for (int i = 0; i < blastedStanFrame; i++)
        {
            // 攻撃を受けているなら処理を抜ける
            if (nowDmgReAct) yield break;

            // １フレーム遅延させる
            yield return null;
        }

        // 攻撃を受けているなら処理を抜ける
        if (nowDmgReAct) yield break;

        // スタンアニメーション終了 ----------
        anim.SetBool("Staning", false);
        audio.bossSource.Stop();
        audio.bossSource.loop=false;


        nowStan = false;

        // 床の回転速度を初期値に戻す
        groundRotation.angularVelocity = holdRotateValue;

        // 攻撃待機状態に移行する
        invincibility = true;
        StartCoroutine(AttackDelayTime());

    }

    private IEnumerator DamageReaction()
    {
        Debug.Log("ダメージリアクションを開始しましたっ！");

        // 足場の回転を止める
        groundRotation.angularVelocity = 0;

        // ダメージリアクション状態にする
        nowDmgReAct = true;
        anim.Play("Damage");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossCrushing_1Sound();

        // リアクション開始地点を保持
        Vector3 holdReactPos = transform.position;

        sbyte knockBack_LL = (sbyte)-knockBackSpeed;
        sbyte knockBackDistHold = (sbyte)knockBackSpeed;

        // 落下速度が下限を下回るまで繰り返す
        while (rb.velocity.y >= knockBack_LL)
        {
            // 自身に速度をつける
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            knockBackDistHold--;
            if(knockBackDistHold<0)
            {
                anim.SetBool("Falling", true);
            }

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        // 着地処理
        rb.velocity = Vector3.zero;     // 速度を０にする
        rb.position = holdReactPos;     // 開始地点で固定する
        
        // スタンアニメーション開始 ----------
        anim.SetBool("Falling", false);
        anim.SetBool("Staning", true);
        audio.bossSource.Stop();
        audio.bossSource.loop = true;
        audio.BossStunSound();
        nowStan = true;

        // 指定した時間の間スタン状態にする
        for (int i = 0; i < stanFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        // スタンアニメーション終了 ----------
        anim.SetBool("Staning", false);
        audio.bossSource.Stop();
        audio.bossSource.loop= false;

        nowStan = false;

        // ダメージリアクション状態を解除
        nowDmgReAct = false;

        // 足場の回転を開始させる
        groundRotation.angularVelocity = holdRotateValue;

        // 攻撃待機状態に移行する
        StartCoroutine(AttackDelayTime());
    }

    private IEnumerator KnockOut()
    {
        // 床の回転を停止させる
        groundRotation.angularVelocity = 0;
        sbyte knockOut_LL = (sbyte)-knockBackSpeed;
        
        anim.Play("Damage");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossCrushing_1Sound();
        // 落下速度が下限を超えた後としばらくの間繰り返す
        while (rb.velocity.y > knockOut_LL * 2)
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

        

        // 自身を非表示にする
        Debug.Log(this.gameObject.name + "を非表示にしましたっ！");
        g_camera.BossEnd = true;
        this.gameObject.SetActive(false);
    }

    
}
