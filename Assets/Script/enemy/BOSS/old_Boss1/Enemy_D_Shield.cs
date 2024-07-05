using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//              バグ報告 - 書置き
// もし一度でも破壊が破壊された判定が入ると盾振り下ろしの際に
// 耐久値の状態関係なくフェイズが切り替わって回復してしまいます
//==================================================
// 制作日2023/04/10    更新日2023/04/11
// 宮﨑
public class Enemy_D_Shield : MonoBehaviour
{
    [SerializeField]
    private GameObject myParent;    // 自分の親を取得
    private Animator parentAnim;    // 親のアニメーター

    private IEnumerator guardWaitTime;

    private Vector3 fixPosition;

    // 一度のみ処理
    public bool startup = false;

    public bool notRepiar;          // 修復をしない設定

    // 盾構えの状態
    [Header("盾の構え状態")]
    [SerializeField] private ushort rotateTime;    // 回転待機時間
    public bool rotateNow;      // 回転中の状態
    public bool rotateEnd;      // 回転後の状態

    [Tooltip("盾を振り下ろすまでの硬直時間")]
    [SerializeField] private ushort impactWaitTime;    // 防御解除までの時間
    public bool shieldImpactNow;        // 盾構え中
    public bool shieldImpactEnd;        // 盾構え終了

    [Header("盾の防御状態")]
    [Tooltip("盾を構えている間の硬直時間")]
    [SerializeField] private ushort guardTime;         // 防御解除までの時間
    public bool guardNow;           // 防御中の状態
    public bool guardEnd;           // 防御終了の状態

    [Header("盾の段階設定")]
    [Tooltip("現在のフェイズ状態")]
    public byte phaseNow;                        // 現在のフェイズ
    public byte phaseLog;                        // 変更前のフェイズ
    public bool lastPhase;
    [Tooltip("フェイズ毎耐久値")]
    public byte[] shieldPhase = { 1, 2, 3 };     // 盾の各耐久値

    [Header("盾の耐久設定")]
    [Tooltip("現在の耐久値")]
    public byte shieldHitPoint;      // 盾の耐久値
    [Tooltip("盾の破壊状態")]
    public bool shieldBreak;        // 盾の破損状態

    [Header("盾の修復状態")]
    [Tooltip("盾が破壊された後の硬直時間")]
    [SerializeField] private int shieldRepairTime;  // 盾修復までの時間
    public bool shieldRepairNow;        // 盾修復中の状態
    public bool shieldRepairEnd;        // 盾修復完了の状態

    [Tooltip("耐久値減少を抑える")]
    public bool stopDamage;                     // 耐久値減少を抑える
    [Tooltip("破壊不可能な状態")]
    [SerializeField] private bool unbreakable;  // 破壊不能状態

    [Header("盾の設定")]
    [Tooltip("盾の当たり判定")]
    public CapsuleCollider shield;          // 盾の当たり判定
    [Tooltip("盾の落下速度")]
    public float shieldSpeed;               // 盾振り下ろし攻撃速度

    public Vector3 shieldPositionStart;     // 盾振り下ろし開始地点
    public Vector3 shieldPositionEnd;       // 盾振り下ろし終了地点

    public vfxManager vfxManager;          // 盾が作用する糸のvfxManager
    private sbyte arrayNum = 0;//波を生成するときに戻り値を受け取りクローンに渡す
    [Header("波の設定")]
    [SerializeField] private float waveSpeed = 7.5f;        // 波の速度
    [SerializeField] private float waveWidth = 0.225f;      // 波の振動数
    [SerializeField] private float waveHight = 2.0f;        // 波の高さ
    [SerializeField] GameObject waveObj;                    // 波のコリジョンプレファブ
    public int waveCount = 3;             // プールに生成する波の数
    private int waveAngle = 1;                              // 波の方向
    // 波コリジョンのオブジェクトプール
    [System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();
    // 波コリジョンにコンポーネントされている waveCollition のプール。
    // 添え字は波コリジョンのプールに対応。
    private List<waveCollition> l_waveCollitions = new List<waveCollition>();
    //==================================================
    //          盾を振り下ろすまでの硬直時間
    // ※振り下ろし開始までの時間を硬直時間として空けておきます
    //==================================================
    // 制作日2023/04/11
    // 宮﨑
    public IEnumerator RotateWaitTime()
    {
#if UNITY_EDITOR
        Debug.Log("回転を開始しましたっ！");
#endif
        // 回転状態にする
        rotateNow = true;
        rotateEnd = false;

        // 回転が解除されるまでの時間
        for (int i = 0; i < rotateTime; i++)
        {
            yield return null;

        }//----- for_stop -----

        // 回転状態を解除する
        rotateNow = false;
        rotateEnd = true;
    }


    //==================================================
    //          防御中の硬直時間
    // ※振り下ろした盾が攻撃を代わりに受けます
    //==================================================
    // 制作日2023/04/10    更新日2023/04/11
    // 宮﨑
    public IEnumerator WaveGuardTime()
    {
#if UNITY_EDITOR
        Debug.Log("防御を開始しましたっ！");
#endif
        // 防御状態にする
        guardNow = true;
        guardEnd = false;

        // 防御が解除されるまでの時間
        for (int i = 0; i < guardTime; i++)
        {
            yield return null;

            // 盾が破壊されたなら
            if (shieldBreak)
            {
#if UNITY_EDITOR
                Debug.Log("盾が破壊されましたっ！");
#endif
                // 盾の当たり判定を破棄する
                shield.enabled = false;

                // 盾は修復用の処理に移行する
                StartCoroutine(ShieldBreakTime());

                // 処理を抜ける
                yield break;

            }//----- if_stop -----

        }//----- for_stop -----

        // 防御状態を解除する
        guardNow = false;
        guardEnd = true;
    }

    //==================================================
    //          盾が破壊された後の硬直時間
    // ※攻撃によって盾が破壊されたときに盾の機能を一時的に止める
    //==================================================
    // 制作日2023/04/10    更新日2023/04/11
    // 宮﨑
    public IEnumerator ShieldBreakTime()
    {
#if UNITY_EDITOR
        Debug.Log("盾を修復していますっ！");
#endif

        // 防御状態を解除する
        guardNow = false;
        guardEnd = true;

        // 盾が破壊できないようにする
        unbreakable = true;

        // 修復中の状態にする
        shieldRepairNow = true;
        shieldRepairEnd = false;

        // 盾を修復するまでの時間
        for (int i = 0; i < shieldRepairTime; i++)
        {
            yield return null;

        }//----- for_stop -----

        // 修復不可ではない場合
        if (!notRepiar)
        {
            // 修復を完了する
            shieldRepairNow = false;
            shieldRepairEnd = true;

            // 盾を破壊できる状態にする
            shieldBreak = false;
            unbreakable = false;

            // 盾の当たり判定を付与する
            shield.enabled = true;
        }

        // スタートアップを開始する
        startup = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        // [Wave]タグがついているオブジェクトに触れた時 &
        // ダメージが与えられる状態なら
        if (other.CompareTag("Wave") && !stopDamage && !unbreakable && guardNow)
        {
            for (int i = 0; i < waveCount; i++)
            {
                if (other.gameObject == l_waveCollisionObj[i])
                {
                    return;
                }
            }
            // 盾の耐久値を減少させる
            shieldHitPoint--;

            // ダメージを与えられない状態にする
            stopDamage = true;

        }//----- if_stop -----
        if ((1 << other.gameObject.layer) == 1 << 6)
        {
            if (other.transform.childCount == 1)
            {
                vfxManager = other.transform.GetChild(0).GetComponent<vfxManager>();
            }

        }
    }

    private void OnTriggerExit(Collider other)
    {
        // [Wave]タグがついているオブジェクトが離れた時
        if (other.CompareTag("Wave"))
        {
            // ダメージを与えられる状態にする
            stopDamage = false;

        }//----- if_stop -----
    }

    //==================================================
    //          盾振り下ろし攻撃
    // ※地面に向かって盾を振り下ろして振動を起こし攻撃する
    //==================================================
    // 制作日2023/04/10    更新日2023/04/11
    // 宮﨑
    public void ShieldImpact()
    {
        // 処理がスタートしたときに一度だけ実行する
        if (!startup)
        {
            // 盾を構え始める
            shieldImpactNow = true;
            shieldImpactEnd = false;

            if (notRepiar)
            {
                shield.enabled = true;
            }

            // 盾の着地地点を初期設置位置で設定する
            shieldPositionEnd = shield.center;

            // 盾振り下ろし開始
            shield.center = new Vector3(shield.center.x, shieldPositionStart.y, shield.center.z);
            parentAnim.SetBool("Impact", true);
            // 一度のみの実行終了
            startup = true;

        }//----- if_stop -----

        // 盾の振り下ろし地点にまだ到達していないなら
        if (shieldPositionEnd.y < shield.center.y)
        {
            // 振り下ろし終了地点まで動かす
            shield.center = new Vector3(shield.center.x, shield.center.y - shieldSpeed, shield.center.z);

        }//----- if_stop -----

        // 盾の着地地点を超えたなら
        else if (shieldPositionEnd.y > shield.center.y)
        {
#if UNITY_EDITOR
            Debug.Log("盾の位置を修正しましたっ！");
#endif
            // 着地点を修正する
            shield.center = shieldPositionEnd;

            parentAnim.SetBool("Impact", false);

            // 盾が破壊できるようにする
            unbreakable = false;

            // ここで盾の攻撃が終了
            shieldImpactNow = false;
            shieldImpactEnd = true;

            // この関数を初期化しておく
            guardWaitTime = WaveGuardTime();

            // 防御状態に移行する
            StartCoroutine(guardWaitTime);
            if (transform.parent.localScale.x > 0)
            {
                waveAngle = 1;
            }
            else
            {
                waveAngle = -1;
            }
            //WaveCreate(transform.position.x + ((Mathf.Abs(transform.lossyScale.x) * (0.5f + shield.radius * 2) + 0.05f) * waveAngle), waveHight, 0);


        }//----- elseif_stop -----
    }
    //private void WaveCreate(float startPosX, float waveHight, float startPosY)
    //{
    //    arrayNum = vfxManager.WaveSpawn(startPosX, waveSpeed * waveAngle, waveHight, waveWidth, 1);
    //    for (int i = 0; i < waveCount; i++)
    //    {
    //        if (l_waveCollisionObj[i].transform.position.z != 0)
    //        {
    //            //当たり判定用のクローンを生成
    //            //cloneObj = Instantiate(waveObj, new Vector3(groundHit.point.x, waveSpornPosY), Quaternion.identity);
    //            //waveCollition wave = l_waveCollisionObj[i].gameObject.GetComponent<waveCollition>();
    //            // 当たり判定に番号を指定
    //            l_waveCollitions[i].waveNum = arrayNum;
    //            // クローンに対応させる vfxManager を設定
    //            l_waveCollitions[i].vfxManager = vfxManager;
    //            // 波をプレイヤーが発生させた波に設定
    //            l_waveCollitions[i].waveType = waveCollition.WAVE_TYPE.ENEMY;
    //            // コリジョンを動かす
    //            l_waveCollitions[i].waveMode = waveCollition.WAVE_MODE.PLAY;
    //            // コリジョンの高さを波の高さに設定
    //            l_waveCollisionObj[i].transform.localScale = new Vector3(0, 0, 1);
    //            // コリジョンの発生位置を波の発生位置に設定
    //            l_waveCollitions[i].waveStartPosition = new Vector3(startPosX, startPosY, 0.0f);

    //            //l_waveCollisionObj[i].transform.position = new Vector3(groundHit.point.x, waveSpornPosY, 0.0f);
    //            // コリジョンの最大の高さを設定
    //            l_waveCollitions[i].maxWaveHight = waveHight;
    //            break;
    //        }
    //        else if (i == waveCount - 1)
    //        {
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(0, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }
    //    }
    //}
    private void Start()
    {
        // 自身の名前を"shield"にする
        this.gameObject.name = "shield";

        // 自分の親を取得
        myParent = transform.parent.gameObject;
        parentAnim = myParent.GetComponent<Animator>();

        // コルーチンを初期化する
        guardWaitTime = WaveGuardTime();

        // 開始時のフェイズをログとして格納
        phaseLog = phaseNow;

        // 初期の盾の親から見た相対座標を取得
        fixPosition = myParent.transform.position-transform.position;

        // 最初のフェイズ設定通りの耐久値を設定
        shieldHitPoint = shieldPhase[0];

        for (int i = 0; i < waveCount; i++)
        {
            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(3, 0, 50), Quaternion.identity));
            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
            l_waveCollisionObj[i].AddComponent<EtoP_Damage>();
        }
    }

    private void FixedUpdate()
    {
        // 盾の耐久値が0以下になったとき
        if (shieldHitPoint <= 0)
        {
            // 盾破壊状態にする
            shieldBreak = true;
            parentAnim.SetBool("staning", true);
        }

        // 盾の位置を本体から乖離しないようにする
        this.transform.position = myParent.transform.position 
                            + new Vector3(fixPosition.x * Mathf.Sign(transform.parent.localScale.x),
                              -fixPosition.y,fixPosition.z);

        // フェイズが変更されたなら
        if ((phaseNow != phaseLog) && !lastPhase)
        {
            if (phaseNow != shieldPhase.Length)
            {
                Debug.Log("盾修復完了");
                // フェイズに合わせて盾の耐久値を変更する
                shieldHitPoint = shieldPhase[phaseNow];
            }

            parentAnim.SetBool("staning", false);

            // ログを更新する
            phaseLog = phaseNow;

            startup = false;

            // 修復不可の場合
            if (notRepiar)
            {
                // 修復を完了する
                shieldRepairNow = false;
                shieldRepairEnd = true;

                // 盾を破壊できる状態にする
                shieldBreak = false;
                unbreakable = false;

                // 盾の当たり判定を付与する
                shield.enabled = true;
            }

            // フェイズが進行不能になったら
            if (phaseNow == shieldPhase.Length)
            {
                // 最終フェイズ状態にする
                lastPhase = true;

            }//----- if_stop -----

        }//----- if_stop -----
    }
}
