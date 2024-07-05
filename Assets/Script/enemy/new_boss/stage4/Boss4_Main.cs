using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボスのメインスクリプト
// ※各挙動を組み合わせてボスの行動を変化させます
// ※[Setup_AvatarPoint]スクリプトが付与された
// 　オブジェクトが存在しているシーン内でのみ行動可能です
//==================================================
// 制作日2023/05/24    更新日2023/05/30
// 宮﨑
public class Boss4_Main : MonoBehaviour
{
    [SerializeField] private bool startAction;
    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    // 指令の情報
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // 自身の情報
    private Rigidbody rb;
    public Animator anim;
    private EnemyEffectManager ef_Manager;
    public BossAudio audio;

    // 分身の情報
    public GameObject[] avaObj;
    public Boss4_Avatar_Main[] avaMain;

    // 挙動情報
    public Boss4_MagicBall actMB;
    public Boss4_Invisible actInv;
    public Boss4_Teleportation actTP;

    // コルーチンの格納
    private IEnumerator cor_Damage;
    private IEnumerator cor_AtkDelay;

    // ステータス
    public BossStatusManager status;

    // 魔力球のプレハブ
    public GameObject magicBall;

    // ランダム情報を取得
    public int[] randomPoint;

    // フェイズ設定
    public byte nowPhase = 0;
    public byte[] setAvatarPhase = new byte[3];

    // 待機の設定
    [SerializeField] private byte stanFrame = 90;
    [SerializeField] private byte hitStopFrame = 30;

    // 被ダメージ時の設定
    public bool invincibility = false;  // 無敵状態
    [SerializeField] private sbyte knockBackSpeed = 30;
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f;

    // 現在の状態
    public bool nowAtkDelay;
    public bool nowStan;
    public bool nowDmgReAct;
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
                // 自身にダメージを与えられない状態にする
                invincibility = true;

                waveHit = other.gameObject;

                // ダメージアニメーションを開始 ----------

                status.hitPoint--;

                if (nowPhase < 2) nowPhase++;

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
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();



        // 分身の情報を取得
        int avatarCnt = avatarOperator.transform.childCount;
        avaObj = new GameObject[avatarCnt];
        avaMain = new Boss4_Avatar_Main[avatarCnt];

        // 召喚地点の数繰り返す
        for (int i = 0; i < avatarCnt; i++)
        {
            // 分身を格納
            avaObj[i] = avatarOperator.transform.GetChild(i).GetChild(0).gameObject;

            // 分身のメイン挙動を取得
            avaMain[i] = avaObj[i].GetComponent<Boss4_Avatar_Main>();

        }//----- for_stop -----

        // 自身の情報を取得
        GameObject thisObj = this.gameObject;
        rb = thisObj.GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        audio = GetComponent<BossAudio>();
        
        // 挙動情報の取得
        actMB = thisObj.GetComponentInChildren<Boss4_MagicBall>();
        actInv = thisObj.GetComponentInChildren<Boss4_Invisible>();
        actTP = thisObj.GetComponentInChildren<Boss4_Teleportation>();

        startLog = g_camera.BossStart;

        // コルーチンの格納
        cor_Damage = DamageReaction();
        //cor_AtkDelay = AttackDelay(0);

        // ボスの体力設定
        status.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        if (!startSet && startLog != g_camera.BossStart)
        {
            startAction = true;
            startSet = true;
            bgm.BossBattleStart();
        }
        // 攻撃を開始する
        if (startAction)
        {
            if (nowDmgReAct) return;
            startAction = false;

            // 乱数で召喚位置を指定
            //Randomizer();

            // 表示状態を変更する
            StartCoroutine(actInv.AnimationDelay());

            // 透明化の後に位置を変更する
            //actInv.InvisibleObjects();
            //actTP.Teleportation();

            //StopCoroutine(cor_AtkDelay);
            //cor_AtkDelay = AttackDelay(magicBallDelay);
            //StartCoroutine(cor_AtkDelay);

        }//----- if_stop -----

        // 自身の体力が０になった
        // ヒットストップ状態に入っていないなら
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----
    }

    public void Randomizer()
    {
        // 指令の子オブジェクトの数を取得する
        int opChildCnt = avatarOperator.transform.childCount;

        // 指令の子オブジェクトの数配列を作成する
        randomPoint = new int[opChildCnt];

        // 指令の子オブジェクトの数繰り返す
        for (int i = 0; i < opChildCnt; i++)
        {
            // 配列内に０から子オブジェクトの数までの乱数を格納する
            randomPoint[i] = Random.Range(0, opChildCnt);

            // 乱数がしっかり振り分けられるまで繰り返す
            for (int j = 0; j < i; j++)
            {
                // 乱数が既に登録されているものなら
                if (randomPoint[i] == randomPoint[j])
                {
                    // 再抽選を行う
                    randomPoint[i] = Random.Range(0, opChildCnt);

                    // 再抽選を行っても一致したときのために
                    // 再度繰り返されるように設定する
                    j = -1;

                }//----- if_stop -----

            }//----- for_stop -----

        }//----- for_stop -----
    }

    //private IEnumerator AttackDelay(byte _delay)
    //{
    //    // 攻撃待機時間を開始する
    //    nowAtkDelay = true;

    //    for(int i = 0; i < _delay; i++)
    //    {
    //        // １フレーム遅延させる
    //        yield return null;

    //        if (nowDmgReAct) yield break;

    //    }//----- for_stop -----

    //    // 攻撃待機時間を抜ける
    //    nowAtkDelay = false;


    //    // 可視化状態のとき
    //    if (!actInv.nowInvisible)
    //    {
    //        startAction = true;
    //    }

    //    // 不可視状態のときに移動後なら
    //    if (actInv.nowInvisible && actTP.teleported)
    //    {
    //        actTP.teleported = false;

    //        // 魔法攻撃を行う
    //        actMB.MagicAttack(magicBall);

    //        StopCoroutine(cor_AtkDelay);
    //        cor_AtkDelay = AttackDelay(magicBallDelay);
    //        StartCoroutine(cor_AtkDelay);
    //    }

    //    // 不可視状態のときに魔力球を放った後なら
    //    if(actInv.nowInvisible && actMB.shot)
    //    {
    //        actMB.shot = false;

    //        // 可視化状態にする
    //        actInv.VisualizationObjects();

    //        // 分身の当たり判定を復活させる
    //        for (int i = 0; i < avaMain.Length; i++)
    //        {
    //            // ０は本体になるため表示しない
    //            if (i == 0) continue;

    //            avaMain[i].damage = false;
    //        }

    //        // 無敵状態を解除する
    //        invincibility = false;

    //        StopCoroutine(cor_AtkDelay);
    //        cor_AtkDelay = AttackDelay(teleportDelay);
    //        StartCoroutine(cor_AtkDelay);
    //    }
    //}

    private IEnumerator StanStatus()
    {
        // 自身がみえるようにする
        MeshRenderer myMesh = this.gameObject.GetComponent<MeshRenderer>();
        myMesh.enabled = true;

        // スタンアニメーション開始 ----------
        nowStan = true;

        // 無敵状態ではない間繰り返す
        while (!invincibility) yield return null;

        // スタンアニメーション終了 ----------
        nowStan = false;
    }

    private IEnumerator DamageReaction()
    {
        Debug.Log("ダメージリアクションを開始しましたっ！");

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
            // 頂点に近くなっているなら
            if(rb.velocity.y < 5)
            {
                // 自身がみえるようにする
                SpriteRenderer myMesh = this.gameObject.GetComponent<SpriteRenderer>();
                myMesh.enabled = true;

                anim.SetBool("Falling", true);

                actInv.nowInvisible = false;
            }

            // 自身に速度をつける
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            knockBackDistHold--;

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        // 着地処理
        rb.velocity = Vector3.zero;     // 速度を０にする
        rb.position = holdReactPos;     // 開始地点で固定する

        // スタンアニメーション開始 ----------
        audio.bossSource.Stop();
        audio.bossSource.loop = true;
        audio.BossStunSound();
        anim.SetBool("Falling",false);
        anim.SetBool("Staning", true);

        nowStan = true;

        // 指定した時間の間スタン状態にする
        for (int i = 0; i < stanFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        // スタンアニメーション終了 ----------
        audio.bossSource.Stop();
        audio.bossSource.loop=false;
        anim.SetBool("Staning", false);
        nowStan = false;

        // ダメージリアクション状態を解除
        nowDmgReAct = false;

        // 分身を復活させる
        for(int i = 0; i < avaMain.Length; i++)
        {
            // ０は本体になるため表示しない
            if (i == 0) continue;

            avaMain[i].active = true;
            avaMain[i].damage = false;
        }

        // 攻撃を開始する
        startAction = true;
    }

    private IEnumerator KnockOut()
    {
        // ダメージリアクション状態にする
        nowDmgReAct = true;

        // 分身を消滅させる
        actInv.InvisibleObjects();

        // 自身がみえるようにする
        SpriteRenderer myMesh = this.gameObject.GetComponent<SpriteRenderer>();
        myMesh.enabled = true;
        actInv.nowInvisible = false;

        sbyte knockOut_LL = (sbyte)-knockBackSpeed;

        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.BossCrushing_1Sound();

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

        // 自身を非表示にする
        Debug.Log(this.gameObject.name + "を非表示にしましたっ！");
        g_camera.BossEnd = true;
        this.gameObject.SetActive(false);
    }
}
