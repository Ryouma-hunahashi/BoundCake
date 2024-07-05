using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_Main : MonoBehaviour
{
    //==================================================
    //          ５ボスの挙動全般
    // ※これ以外にフェイズ参照出現スクリプトが存在しますが
    // 　ボス単体ではこれのみで作動します。
    //==================================================
    // 制作日2023/05/26    更新日2023/05/28
    // 宮﨑
    [System.Serializable]
    public struct LR_Position
    {
        public bool left, right;
    }

    // 動きを開始する
    public bool startAction = true;

    // 自身の情報を取得
    private Rigidbody rb;
    private Animator anim;
    private BossAudio audio;
    private Vector3 scale;

    [SerializeField] private GoalCamera g_camera;
    [SerializeField] private BgmCon bgm;

    private EnemyEffectManager ef_Manager;

    // 自身の詳細設定
    public BossStatusManager status;
    public byte nowPhase = 0;

    [SerializeField] private LayerMask groundLayer = 1 << 6;
    [SerializeField] private float rayDistance = 1.5f;
    RaycastHit underRayHit;

    // 被ダメージ時の設定
    [SerializeField]
    private bool invincibility = false;     // 無敵状態

    [SerializeField] private byte atkDelayFrame = 180;
    [SerializeField] private byte stanFrame = 90;
    [SerializeField] private sbyte knockBackSpeed = 30;

    // ヒットストップの設定
    [SerializeField] private byte hitStopFrame = 30;
    [SerializeField, Range(0f, 1f)] private float hitStop = 0.5f;

    // コルーチンの格納
    public IEnumerator cor_Damage;

    // 移動位置の設定
    [SerializeField] private LR_Position[] lr_Pos = new LR_Position[3];
    [SerializeField] private float leftPosition;
    [SerializeField] private float rightPosition;
    private bool moved = false;

    // 特殊ジャンプの設定
    [SerializeField] private byte specialChargeFrame = 90;
    [SerializeField] private byte specialJumpSpeed = 26;

    [SerializeField] private GameObject switchObj;
    private VariousSwitches_2 switchScript;

    // 現在の状態
    public bool nowCharge;
    public bool nowJumpAtk;
    public bool nowPhaseChange;

    public bool nowAtkDelay;
    public bool nowDmgReAct;
    public bool nowStan;
    public bool nowHitStop;

    // 波当たり判定
    private GameObject waveHit;
    private GameObject waveLog;

    private bool startLog = false;
    private bool startSet = false;

    // 波の設定
    [Header("----- 波の設定 -----")]
    private WavePool pool;
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

    private void OnTriggerEnter(Collider other)
    {
        // 無敵状態,ダメージアクション中なら処理を抜ける
        if (invincibility || nowDmgReAct || nowJumpAtk) return;

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
                }//-----if_stop-----
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
        // 自身の情報を取得
        GameObject thisObj = this.gameObject;
        rb = thisObj.GetComponent<Rigidbody>();
        scale = thisObj.transform.localScale;
        anim = GetComponent<Animator>();
        audio = GetComponent<BossAudio>();
        pool = GetComponent<WavePool>();
        pool.AddEtoPDamage();

        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        switchScript = switchObj.GetComponent<VariousSwitches_2>();

        startLog = g_camera.BossStart;

        //// 波の各情報を取得
        //for (int i = 0; i < waveCount; i++)
        //{
        //    l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
        //    l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
        //    l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        //}//----- for_stop -----

        // コルーチンを格納
        cor_Damage = DamageReaction();

        // フェイズごとの移動位置を再設定する
        PositionSelector();

        status.hitPoint = 3;
    }

    private void FixedUpdate()
    {
        Ray underRay = new Ray(transform.position, Vector3.down);
        bool underRayFg = Physics.Raycast(underRay,out underRayHit,rayDistance,groundLayer);
        // 自身の体力が０になった
        // ヒットストップ状態に入っていないなら
        if ((status.hitPoint <= 0) && !nowHitStop)
        {
            StartCoroutine(KnockOut());
            nowHitStop = true;

        }//----- if_stop -----


        if (!startSet && startLog != g_camera.BossStart)
        {
            startAction = true;
            startSet = true;
            bgm.BossBattleStart();
        }
        if (startAction)
        {
            StartCoroutine(JumpPhaseChange(nowPhase));
            startAction = false;
        }
    }

    //==================================================
    //          フェイズごとの位置変更の修正
    // ※設定された値に不備がある場合に自動で調整します
    //==================================================
    // 制作日2023/05/28
    // 宮﨑
    private void PositionSelector()
    {
        for(int i = 0; i < lr_Pos.Length; i++)
        {
            // 選択方向が確定していないなら
            if ((lr_Pos[i].left && lr_Pos[i].right) ||
                (!lr_Pos[i].left && !lr_Pos[i].right))
            {
                // ランダムで左右を選択する
                int random = Random.Range(0, 2);

                // 格納された数値によって選択を変更する
                switch (random)
                {
                    case 0:
                        {
                            lr_Pos[i].left = true;
                            lr_Pos[i].right = false;
                        }// 左を優先する
                        break;
                    case 1:
                        {
                            lr_Pos[i].left = false;
                            lr_Pos[i].right = true;
                        }// 右を優先する
                        break;
                }//----- switch_stop -----

                Debug.Log(i + "の情報を" + random + "に変更しました");

            }//----- if_stop -----

        }//----- for_stop -----
    }

    //==================================================
    //          攻撃までの待機時間
    // ※攻撃を開始するまでの待機時間をフレーム時間でとります
    //==================================================
    // 制作日2023/05/28
    // 宮﨑
    private IEnumerator AttackDelayTime()
    {
        Debug.Log("攻撃の待機時間に入りましたっ！");
        nowAtkDelay = true;
        ef_Manager.StopSummon();

        // 攻撃までの待機を開始する
        for (int i = 0; i < atkDelayFrame; i++)
        {
            if (nowPhaseChange) yield break;

            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        // 特殊ジャンプチャージへ移行する
        nowAtkDelay = false;
        StartCoroutine(SpecialJumpCharge());
    }

    //==================================================
    //          特殊ジャンプのチャージ
    // ※プレイヤーの動きに似せたジャンプチャージ時間です
    //==================================================
    // 制作日2023/05/28
    // 宮﨑
    public IEnumerator SpecialJumpCharge()
    {
        // ジャンプチャージを開始する
        Debug.Log("特殊チャージ状態に入りましたっ！");
        nowCharge = true;

        // チャージアニメーション開始 ----------
        anim.Play("Charge");

        // チャージ時間の間繰り返す
        for (int i = 0; i < specialChargeFrame; i++)
        {
            if (nowDmgReAct)
            {
                nowCharge = false;
                yield break;

            }//----- if_stop -----

            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        if (nowDmgReAct)
        {
            nowCharge = false;
            yield break;

        }//----- if_stop -----

        // ジャンプチャージを終了する
        nowCharge = false;

        // チャージアニメーション終了 ----------

        // 特殊ジャンプ攻撃へ移行する
        StartCoroutine(SpecialJumpAttack());
    }

    //==================================================
    //          特殊ジャンプ本体
    // ※チャージ時間を通過した後に呼び出され
    //   大きくジャンプを行い振動を発生させます
    //==================================================
    // 制作日2023/05/28
    // 宮﨑
    public IEnumerator SpecialJumpAttack()
    {
        Debug.Log("特殊ジャンプを開始しましたっ！");
        nowJumpAtk = true;

        // ジャンプ開始地点を保持
        Vector3 holdJumpPos = transform.position;

        sbyte fallSpeed_LL = (sbyte)-specialJumpSpeed;
        float speed = specialJumpSpeed;

        float i = (60 / Test_FPS.instance.m_fps);

        // ジャンプアニメーションを開始する ----------
        anim.SetBool("JumpEnding", false);
        anim.SetBool("Jumping",true);
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.Boss5_SuperJumpSound();
        ef_Manager.StartJump();
        // 落下速度が下限を下回るまで繰り返す
        while (rb.velocity.y >= fallSpeed_LL)
        {
            // 自身に速度を付与する
            rb.velocity = new Vector3(rb.velocity.x, speed, 0);
            speed-=i;

            // 速度が０以下になったなら
            if(rb.velocity.y < 0)
            {
                // 落下系の何かが入るならここです
               // ef_Manager.StopJump();

            }//----- if_stop -----

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        // 着地処理
        rb.velocity = Vector3.zero; // 速度を０にする
        rb.position = holdJumpPos;  // 開始地点へ補正する
        nowJumpAtk = false;

        // 落下系の何かはここで終了させます
        anim.SetBool("Jumping", false);
        anim.SetBool("JumpEnding", true);

        yield return null;
        if (underRayHit.transform != null)
        {
            pool.WaveCreate(waveSpeed*waveAngle,waveWidth,waveHight,waveCollition.WAVE_TYPE.ENEMY
            ,transform.position.x, underRayHit.transform);
        }
        

        // 攻撃待機時間に移行する
        StartCoroutine(AttackDelayTime());
    }

    //==================================================
    //          フェイズごとの位置変更
    // ※攻撃された後に指定された方向へ大きく跳躍して移動を行います
    //==================================================
    // 制作日2023/05/28
    // 宮﨑
    public IEnumerator JumpPhaseChange(byte _phase)
    {
        Debug.Log("フェイズ変更後特殊ジャンプを実施しましたっ！");
        nowPhaseChange = true;
        while (nowJumpAtk) yield return null;

        // ジャンプ開始地点を保持
        float holdJumpPosY = transform.position.y;

        sbyte fallSpeed_LL = (sbyte)(-specialJumpSpeed * 2);
        float speed = (specialJumpSpeed * 2);

        float i = (60 / Test_FPS.instance.m_fps);

        // ジャンプアニメーションを開始する ----------
        anim.SetBool("JumpEnding", false);
        anim.Play("Jump");
        audio.bossSource.Stop();
        audio.bossSource.loop = false;
        audio.Boss5_SuperJumpSound();
        ef_Manager.StartJump();

        // 落下速度が下限を下回るまで繰り返す
        while (rb.velocity.y >= fallSpeed_LL)
        {
            // 自身に速度を付与する
            rb.velocity = new Vector3(rb.velocity.x, speed, 0);
            speed-=i;

            // 速度が０以下になったなら
            if (rb.velocity.y < 0)
            {
                // 落下系の何かが入るならここです
                if(!moved)
                {
                    if (lr_Pos[_phase].left)
                    {
                        rb.position = new Vector3(leftPosition, rb.position.y, 0);
                        this.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
                        waveAngle = 1;
                    }//----- if_stop -----
                    else if (lr_Pos[_phase].right)
                    {
                        rb.position = new Vector3(rightPosition, rb.position.y, 0);
                        this.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
                        waveAngle = -1;
                    }//----- if_stop -----
                    //ef_Manager.StopJump();
                    moved = true;

                }//----- if_stop -----

            }//----- if_stop -----

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        // 着地処理
        anim.SetBool("JumpEnding", true);
        rb.velocity = Vector3.zero; // 速度を０にする
        rb.position = new Vector3(rb.position.x, holdJumpPosY, 0);  // 開始地点へ補正する
        for (byte j = 0; j < 3; j++)
        {
            yield return null;
        }
        
        if (underRayHit.transform != null)
        {
            vfxManager = underRayHit.transform.GetChild(0).GetComponent<vfxManager>();
            //WaveCreate(transform.position.x, underRayHit.transform.position.y);
        }
        
        // 落下系の何かはここで終了させます
        moved = false;

        // エリア移動の終了
        nowJumpAtk = false;
        nowPhaseChange = false;
        invincibility = false;

        // フェイズを変更
        nowPhase++;

        if (status.hitPoint == 1)
        {
            switchScript.switchStatus = false;
        }


        // 雑魚敵召喚アニメーション
        anim.Play("Command");
        ef_Manager.StartSummon();

        // 攻撃待機時間に移行する
        StartCoroutine(AttackDelayTime());
    }

    //==================================================
    //          被ダメージ時のリアクション
    // ※プレイヤーからの振動を受けた時に大きく跳ね上がります
    // ※跳ね上がった後はしばらくの間スタンします
    //==================================================
    // 制作日2023/05/28
    // 宮﨑
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
            // 自身に速度をつける
            rb.velocity = new Vector3(0, knockBackDistHold, 0);
            knockBackDistHold--;
            if(rb.velocity.y < 0)
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
        anim.SetBool("Falling",false);
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
        audio.bossSource.loop = false;

        nowStan = false;

        // ダメージリアクション状態を解除
        nowDmgReAct = false;

        // エリア移動ジャンプへ移行する
        StartCoroutine(JumpPhaseChange(nowPhase));
    }

    //==================================================
    //          討伐時のリアクション
    // ※最終攻撃を行った時に大きく跳ね上がり、
    // 　少しの間時間が遅くなって画面外まで落下していきます
    //==================================================
    // 制作日2023/05/28
    // 宮﨑
    private IEnumerator KnockOut()
    {
        // ボスを倒したときの処理を実行します
        Debug.Log(this.name + "を倒しましたっ！");

        // ダメージリアクション状態にする
        nowDmgReAct = true;
        anim.Play("Damage");

        // リアクション開始地点を保持
        Vector3 holdReactPos = transform.position;
        sbyte knockOut_LL = (sbyte)-knockBackSpeed;

        // 落下速度が下限を超えた後としばらくの間繰り返す
        while (rb.velocity.y > knockOut_LL)
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
            if (rb.velocity.y < 0)
            {
                anim.SetBool("Falling", true);
            }

            // 自身に速度をつける
            rb.velocity = new Vector3(0, knockBackSpeed, 0);
            knockBackSpeed--;

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        // 着地処理
        rb.velocity = Vector3.zero;     // 速度を０にする
        rb.position = holdReactPos;     // 開始地点で固定する

        for(byte i= 0; i < 45; i++)
        {
            yield return null;
        }
        anim.SetBool("WakeUping", true);
        for(byte i= 0;i<50;i++)
        {
            yield return null;
        }
        // ここでエンディング！！！！！！！！！！！！！！！！！！！！

        g_camera.BossEnd = true;



        //// 自身を非表示にする
        //Debug.Log(this.gameObject.name + "を非表示にしましたっ！");
        //this.gameObject.SetActive(false);
    }
    //public void WaveCreate(float startPosX, float startPosY)
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
