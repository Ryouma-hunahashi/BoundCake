using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボスの盾挙動スクリプト
// ※盾を使った攻撃を関数として配置します
//==================================================
// 制作日2023/05/16    更新日2023/05/18
// 宮﨑
public class Boss1_Shield : MonoBehaviour
{
    // 親の情報を取得
    private GameObject par_Obj;
    private Animator par_Anim;

    // 親の挙動情報を取得
    private Boss1_Main par_Main;
    private Boss1_Rush par_Rush;
    private WavePool wavePool;

    // 本体とのズレ
    private Vector3 fixPosition;

    // コルーチンの格納
    [System.NonSerialized] public IEnumerator cor_Rotate;   // 回転
    [System.NonSerialized] public IEnumerator cor_Impact;   // 衝撃
    [System.NonSerialized] public IEnumerator cor_Shield;   // 防御
    [System.NonSerialized] public IEnumerator cor_Repair;   // 修復

    // 待機時間の設定
    [Header("----- 待機の設定 -----")]
    [SerializeField] private ushort breakFrame = 10;    // 破壊時の無敵時間
    [SerializeField] private ushort rotateFrame = 60;   // 回転待機時間
    [SerializeField] private ushort impact_CT = 120;    // 防御開始までの時間
    [SerializeField] private ushort shieldFrame = 480;  // 防御解除までの時間
    [SerializeField] private ushort repairFrame = 45;   // 修復までの時間

    // 盾状態の設定
    [Header("----- 現在の状態 -----")]
    public bool nowStan;   // 混乱中
    private bool nowBreak;  // 待機中
    public bool nowRotate;  // 回転中
    public bool nowImpact;  // 攻撃中
    public bool nowShield;  // 防御中
    public bool nowRepair;  // 修復中
    public bool nowDamage;  // 無敵中

    // 盾の詳細設定
    [Header("----- 盾詳細設定 -----")]
    private CapsuleCollider shieldCol;  // 盾の当たり判定
    public sbyte durability;            // 耐久力
    public float downSpeed = 0.1f;      // 落下速度
    public Vector3 startPos;            // 落下開始地点
    public Vector3 finishPos;           // 落下終了地点
    public bool shieldBreak = false;    // 盾の破損状態
    public bool unbreakable = false;    // 盾破壊不可状態

    // フェイズ状態
    [Header("----- 段階の設定 -----")]
    public byte nowPhase;                   // 現在のフェイズ
    private byte logPhase;                  // 過去のフェイズ
    public byte[] vitPhase = new byte[3];   // フェイズ毎の耐久値
    public bool lastPhase;                  // 最終フェイズ状態
    private bool stanSetFg = false;

    // 波の設定
    [Header("----- 波の設定 -----")]
    public vfxManager vfxManager;   // 盾が作用する糸のvfxManager
    private sbyte arrayNum = 0;     // 波を生成するとき戻り値をクローンへ
    [SerializeField] private float waveSpeed = 7.5f;    // 波の速度
    [SerializeField] private float waveWidth = 0.225f;  // 波の振動数
    [SerializeField] private float waveHight = 2.0f;    // 波の高さ
    [SerializeField] private GameObject waveObj;        // 波の判定プレハブ
    public int waveCount = 3;   // プールに生成する波の数
    private int waveAngle = 1;  // 波の方向

    // 波のコリジョンのオブジェクトプール
    [System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();

    // 波コリジョンにコンポーネントされている波判定のプール
    // 添え字は波判定のプールに対応
    private List<waveCollition> l_waveCollitions = new List<waveCollition>();
    

    private void OnTriggerEnter(Collider other)
    {
        // 盾を構えている間
        // ダメージが与えられる状態のとき
        // "Wave"タグが付いたオブジェクトに触れたなら
        if (nowShield && !nowDamage && !unbreakable && other.CompareTag("Wave"))
        {
            for (int i = 0; i < waveCount; i++)
            {
                if (other.gameObject == l_waveCollisionObj[i])
                {
                    return;
                }//----- if_stop -----

            }//----- for_stop -----
            if (par_Main.audio.bossSource.isPlaying)
            {
                par_Main.audio.bossSource.Stop();
            }

            par_Main.audio.bossSource.loop = false;
            par_Main.audio.Boss1_GuardSound();
            // 盾の耐久値を減少させる
            durability--;

            // ダメージを与えられない状態にする
            nowDamage = true;

        }//----- if_stop -----

        if ((1 << other.gameObject.layer) == 1 << 6)
        {
            if (other.transform.childCount == 1)
            {
                vfxManager = other.transform.GetChild(0).GetComponent<vfxManager>();

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // "Wave"タグがついたオブジェクトが離れた時
        if (other.CompareTag("Wave"))
        {
            // ダメージを与えられるようにする
            nowDamage = false;

        }//----- if_stop -----
    }

    private void Start()
    {
        // 自身の名前を"shield"にする
        this.gameObject.name = "shield";

        // 親の情報を取得
        par_Obj = transform.parent.gameObject;
        par_Anim = par_Obj.GetComponent<Animator>();

        // 親の挙動情報を取得
        par_Main = transform.parent.GetComponent<Boss1_Main>();
        par_Rush = transform.parent.GetComponentInChildren<Boss1_Rush>();
        wavePool = GetComponent<WavePool>();

        // 盾の当たり判定を取得
        shieldCol = this.GetComponent<CapsuleCollider>();

        // コルーチンを初期化
        cor_Rotate = Rotate();
        cor_Impact = ShieldImpact();
        cor_Shield = ShieldGuald();
        cor_Repair = ShieldRepair();

        // 開始時のフェイズをログとして格納
        logPhase = nowPhase;

        // 初期の盾の親から見た相対座標を取得
        fixPosition = par_Obj.transform.position - transform.position;

        // 最初のフェイズ設定どおりの耐久値を設定
        durability = (sbyte)vitPhase[0];

        // 波の各情報を取得
        for (int i = 0; i < waveCount; i++)
        {
            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(3, 0, 50), Quaternion.identity));
            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
            l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        }//----- for_stop -----
    }

    private void FixedUpdate()
    {
        // 盾の耐久値が０以下になったとき
        if (durability <= 0)
        {
            if (!nowBreak)
            {
                
                // 本体攻撃可能にする関数
                StartCoroutine(ShieldBreak());

                nowBreak = true;

            }//----- if_stop -----

            
            nowStan = true;

        }//----- if_stop -----

        // 盾の位置を本体から乖離しないようにする
        this.transform.position = par_Obj.transform.position
                              + new Vector3(fixPosition.x * Mathf.Sign(transform.parent.localScale.x),
                              -fixPosition.y, fixPosition.z);

        // 最終フェイズではないときに
        // フェイズが変更されたなら
        if (!lastPhase && (nowPhase != logPhase))
        {
            // ログを更新する
            logPhase = nowPhase;

            // フェイズに合わせて盾の耐久値を変更する
            durability = (sbyte)vitPhase[nowPhase];

            // スタンアニメーションを終了
            par_Anim.SetBool("staning", false);
            stanSetFg = false;
            nowStan = false;
            if (par_Main.audio.bossSource.isPlaying)
            {
                //par_Main.audio.bossSource.Stop();
            }

            par_Main.audio.bossSource.loop = false;

            // フェイズが進行不能になったら
            if (nowPhase == vitPhase.Length - 1)
            {
                // 最終フェイズの状態を送る
                lastPhase = true;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    // 制作日2023/05/18
    public IEnumerator ShieldBreak()
    {
        shieldCol.enabled = false;
        
        
        if (!nowStan)
        {
            Debug.Log("盾破壊するぜ！");

            par_Main.audio.bossSource.loop = false;
            par_Main.audio.Boss1_ShieldBreakSound();
        }
        // 本体攻撃可能になるまでの無敵時間
        for (int i = 0; i < breakFrame; i++)
        {
            yield return null;

        }//----- if_stop -----


        // スタンアニメーションを開始
        par_Anim.SetBool("staning", true);
        
        if (!stanSetFg&&!par_Main.nowDmgAct)
        {

            Debug.Log("スタン～");
            par_Main.audio.bossSource.loop = true;

            par_Main.audio.BossStunSound();
            stanSetFg = true;
        }
        else if(!par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.loop = true;

            par_Main.audio.BossStunSound();
        }
        shieldBreak = true;

        nowBreak = false;
    }

    //==================================================
    //          盾を振り下ろし始めるまでの待機時間
    // ※盾振り下ろし攻撃を開始するまでの待機用関数、初期動作
    //==================================================
    // 制作日2023/05/16
    // 宮﨑
    public IEnumerator Rotate()
    {
        while (nowStan || par_Main.nowDmgAct)
        {
            yield return null;
        }

        Debug.Log("回転を開始しましたっ！");

        // 回転状態を送る
        nowRotate = true;

        // 回転が解除されるまでの時間
        for (int i = 0; i < rotateFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        // 回転状態を解除
        nowRotate = false;

        // 次の操作に移行する
        StopCoroutine(cor_Impact);
        cor_Impact = ShieldImpact();
        StartCoroutine(cor_Impact);
    }


    //==================================================
    //          盾振り下ろし攻撃
    // ※盾を糸に叩きつけて振動を飛ばします
    //==================================================
    // 制作日2023/05/16
    // 宮﨑
    public IEnumerator ShieldImpact()
    {
        if (nowStan || par_Main.nowDmgAct) yield break;
        // 盾構え状態を送る
        nowImpact = true;

        // 盾構えのアニメーションを開始
        par_Anim.Play("ShieldSet");

        // 盾の当たり判定を出現させる
        shieldCol.enabled = true;

        // 盾の着地地点を初期位置に設定
        finishPos = shieldCol.center;

        // 自身を振り下ろし開始地点に移動
        shieldCol.center = new Vector3(shieldCol.center.x, startPos.y, shieldCol.center.z);

        // 盾振り下ろしのアニメーション開始
        par_Anim.SetBool("Impact", true);
        if (par_Main.audio.bossSource.isPlaying)
        {
            par_Main.audio.bossSource.Stop();
        }
        par_Main.audio.bossSource.loop = false;
        par_Main.audio.Boss1_AttackSound();
        // 振り下ろし地点に到達していないなら
        while (finishPos.y < shieldCol.center.y)
        {
            // １フレーム遅延させる
            yield return null;

            // 振り下ろす
            shieldCol.center = new Vector3(shieldCol.center.x, shieldCol.center.y - downSpeed, shieldCol.center.z);

        }//----- while_stop -----
        
        // 着地地点を修正する
        shieldCol.center = finishPos;

        // 盾振り下ろしのアニメーション終了
        par_Anim.SetBool("Impact", false);


        // 盾が破壊できるようにする
        unbreakable = false;

        // 盾構えを解除
        nowImpact = false;

        // 波を出現させる
        if (transform.parent.localScale.x > 0)
        {
            waveAngle = 1;

        }//----- if_stop -----
        else
        {
            waveAngle = -1;

        }//----- else_stop -----
        //WaveCreate(transform.position.x + ((Mathf.Abs(transform.lossyScale.x) * (0.5f + shieldCol.radius * 2) + 0.05f) * waveAngle), waveHight, vfxManager.transform.parent.position.y);
        wavePool.WaveCreate(waveSpeed, waveWidth, waveHight, waveCollition.WAVE_TYPE.ENEMY,
            transform.position.x + ((Mathf.Abs(transform.lossyScale.x) * (0.5f + shieldCol.radius * 2) + 0.05f) * waveAngle), 
            vfxManager.transform.parent);

        // 防御状態に移行する
        StopCoroutine(cor_Shield);
        cor_Shield = ShieldGuald();
        StartCoroutine(cor_Shield);
    }

    //==================================================
    //          防御行動の関数
    // ※振り下ろした盾が攻撃を代わりに受けます
    //==================================================
    // 制作日2023/05/16       更新日2023/05/18
    // 宮﨑
    public IEnumerator ShieldGuald()
    {
        Debug.Log("防御を開始しましたっ！");

        // 防御状態にする
        nowShield = true;

        // 防御が解除させるまでの時間
        for (int i = 0; i < shieldFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

            // 盾が破壊されているとき
            // 修復中でなければ
            if (shieldBreak && !nowRepair)
            {
                // 盾修復状態に移行する
                StopCoroutine(cor_Repair);
                cor_Repair = ShieldRepair();
                StartCoroutine(cor_Repair);

                // 処理を抜ける
                yield break;

            }//----- if_stop -----

        }//----- for_stop -----

        // 防御を解除する
        nowShield = false;

        // 突進動作に移行する
        StopCoroutine(par_Rush.cor_Rush);
        par_Rush.cor_Rush = par_Rush.RushAction();
        StartCoroutine(par_Rush.cor_Rush);
    }

    //==================================================
    //          盾が修復されるまでの待機時間
    // ※波攻撃によって盾が破壊されたときに盾の機能を一時的に止める
    //==================================================
    // 制作日2023/05/16
    // 宮﨑
    public IEnumerator ShieldRepair()
    {
        Debug.Log("盾を修復していますっ！");

        // 盾の当たり判定を破棄
        shieldCol.enabled = false;

        // 防御状態を解除する
        nowShield = false;

        // 盾を破壊不可にする
        unbreakable = true;

        // 修復状態にする
        nowRepair = true;

        // 盾を修復するまでの時間
        for (int i = 0; i < repairFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        // 修復が完了
        nowRepair = false;

        // 盾を破壊可能にする
        unbreakable = false;

        // 無敵状態を解除
        par_Main.invincibility = false;

        // 盾が破壊されていない状態にする
        shieldBreak = false;

        // 盾の当たり判定を付与する
        shieldCol.enabled = true;

        // 次の動作に移行する
        StopCoroutine(cor_Rotate);
        cor_Rotate = Rotate();
        StartCoroutine(cor_Rotate);
    }

    private void OnApplicationQuit()
    {
        l_waveCollisionObj.Clear();
        l_waveCollitions.Clear();
    }

    //private void WaveCreate(float startPosX, float waveHight, float startPosY)
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
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(0, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }//----- elseif_stop -----

    //    }//----- for_stop -----
    //}
}
