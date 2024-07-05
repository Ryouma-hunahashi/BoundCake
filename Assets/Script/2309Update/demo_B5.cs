using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

#if UNITY_EDITOR

// ボスクラスを継承する
[CustomEditor(typeof(Enemy_Boss))]

#endif

public class demo_B5 : Enemy_Boss
{
    // 移動位置の列挙型
    private enum POSITION
    {
        MID,    // 中央
        LEFT,   //  左
        RIGHT,  //  右
    }

    [System.Serializable]
    private struct DIRECTION_DATA
    {
        [SerializeField] private Vector2 pos;
        [SerializeField] private float min, max;

        public Vector2 GetPosition() { return pos; }
        public float GetDistanceMin() { return min; }
        public float GetDistanceMax() { return max; }
    }

    [System.Serializable]
    private struct POSITION_DATA
    {
        // 移動地点の[ X ]座標の値
        [SerializeField] private float mid, left, right;

        // ゲットセッター
        public float GetMidPos() { return mid; }
        public float GetLeftPos() { return left; }
        public float GetRightPos() { return right; }
    }

    [System.Serializable]
    private struct SUMMON_DATA
    {
        [SerializeField] private GameObject mob;        // オブジェクト
        [SerializeField] private DIRECTION_DATA left, right;

        // ゲットセッター
        public GameObject GetMobObject() { return mob; }
        public DIRECTION_DATA GetLeftData() { return left; }
        public DIRECTION_DATA GetRightData() { return right; }
    }

    private Vector3 scale;

    // 行動の開始用のブーリアン型
    public bool start;
    private bool inMoving;
    private bool inAction;

    [SerializeField] private float stanTime;        // 混乱解除までの時間

    // 自身の移動位置の設定
    [Header("＊移動位置の設定")]
    [SerializeField] private POSITION[] standPoint = new POSITION[3];   // 降下位置の設定
    [SerializeField] private POSITION_DATA posData;                     // 移動地点の座標設定

    [Header("＊跳躍関係の設定")]
    [SerializeField] private float chargeTime;  // 溜める時間
    [SerializeField] private double jumpPower;  // 素の跳躍力
    [SerializeField] private float jumpPowMag;  // 跳躍力倍率
    [SerializeField] private float endFall;     // どこまで落ちるか

    [Header("＊召喚関係の設定")]
    [SerializeField] private SUMMON_DATA summon;
    [SerializeField] private float summonTime;      // 召喚までの時間

    private GameObject summonedMob;     // 召喚したオブジェクト

    [Header("＊ボス演出関連のスクリプト＊")]
    [SerializeField] private Bossdirection bossCamera;

    private void Start()
    {
        scale = this.transform.localScale;
    }

    private void Update()
    {
        Vector3 plPos = GetTarget().transform.position;
        Vector3 myPos = this.transform.position;

        if(plPos.x < myPos.x)
        {
            this.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
        }
        else
        {
            this.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
        }
    }

    private void FixedUpdate()
    {
        // 行動を開始しない状態のときや移動中は処理を抜ける
        if (!start || inMoving) return;

        if (Input.GetKeyDown(KeyCode.W)) SetState(STATE.DAMAGE);

        // ダメージを受けたなら専用の行動に移行する
        if (GetState() == STATE.DAMAGE) DamageReaction();

        // 行動中は以下の処理を実行しない
        if (inAction) return;

        // 状態に合わせて関数を実行する
        FlexibleResponse();
    }

    private void FlexibleResponse()
    {
        // 待機中なら行動しない
        if (GetState() == STATE.IDLE) return;

        // 行動を開始する
        inAction = true;

        switch (GetState())
        {
            // 出現時の処理
            case STATE.WARNING:
                {
                    // 開始時の処理用の関数を開始する
                    StartCoroutine(Warning());
                }
                break;
            // モブ召喚攻撃処理
            case STATE.ATK01:
                {
                    // モブ召喚用の関数を開始する
                    StartCoroutine(SummonEnemies());
                }
                break;
        }
    }

    private void DamageReaction()
    {
        Debug.Log("* Start damage reaction.");

        // 行動を開始する
        inAction = true;
        inMoving = true;

        // 使用している行動をすべて止める
        StopAllCoroutines();

        // フェイズ変化を実施
        StartCoroutine(PhaseChange());
    }

    private IEnumerator PhaseChange()
    {
        // フェイズを変化させる
        NextPhase();

        // 現在のフェイズが３未満なら
        if(GetPhase() < 3)
        {
            // スタン状態に移行
            GetAnimator().SetBool("Staning", true);
            GetAudio().bossSource.Stop();
            GetAudio().bossSource.loop = true;
            GetAudio().BossStunSound();

            // スタン用待機時間
            yield return StartCoroutine(IdleTime(stanTime));

            // スタン終了
            GetAnimator().SetBool("Staning", false);
            GetAudio().bossSource.Stop();
            GetAudio().bossSource.loop = false;

            // フェイズ変化用のジャンプを開始
            yield return StartCoroutine(Jump());

            // 召喚攻撃を開始する
            SetState(STATE.ATK01);

            // 行動を終了する
            inAction = false;
        }
        else
        {
            // 倒したとき用のジャンプを開始
            yield return StartCoroutine(Jump(true));

            // 待機状態にする
            SetState(STATE.IDLE);

            // 非表示にする
            this.gameObject.SetActive(false);
        }

    }

    private IEnumerator Warning()
    {

        // ジャンプ処理が終了するまで待つ
        yield return StartCoroutine(Jump());

        // 待機状態が終了するまで待つ
        yield return StartCoroutine(IdleTime());

        // 攻撃状態に移行
        SetState(STATE.ATK01);

        // 行動を終了する
        inAction = false;
    }

    private IEnumerator SummonEnemies()
    {
        yield return StartCoroutine(Summon());

        // 待機状態に移行
        SetState(STATE.IDLE);

        // 行動を終了する
        inAction = false;
    }

    private IEnumerator Jump(bool _KO = false)
    {
        Debug.Log("* Use jump function.");

        // 移動を開始する
        inMoving = true;

        // FPSによるジャンプのずれを軽減
        // 式：( 指定FPS値 ) / ( 現在のFPS値 )
        double fixFPS = ((60) / Test_FPS.instance.m_fps);

        // ジャンプに使用する各変数の作成
        float startJumpPos = this.transform.position.y;     // 跳躍開始地点の座標を取得
        double limFallSpeed = (-jumpPower * jumpPowMag);    // 跳躍速度の制限
        double spJumpPower = (jumpPower * jumpPowMag);      // 反映跳躍速度

        // ジャンプ用アニメーションを開始する
        GetAnimator().SetBool("JumpEnding",false);
        GetAnimator().Play("Jump");

        // ジャンプ用オーディオの再生
        GetAudio().bossSource.Stop();
        GetAudio().bossSource.loop = false;
        GetAudio().Boss5_SuperJumpSound();

        // ジャンプ用のエフェクトを開始する
        GetEffectManager().StartJump();

        // 跳躍処理本体 ------------------------------*
        // 落下速度が下限を下回るまで繰り返す
        while(GetRigidbody().velocity.y >= limFallSpeed)
        {
            // 自身に速度を付与する
            GetRigidbody().velocity = new Vector3(GetRigidbody().velocity.x, (float)spJumpPower, 0);

            // 速度の値をFPSに合わせて減少させていく
            spJumpPower -= fixFPS;

            // 落下速度が０以下になったなら
            if(GetRigidbody().velocity.y < 0)
            {
                // ノックアウトしていないなら
                if (!_KO)
                {
                    // どこに着地するのかを判定する
                    switch (standPoint[GetPhase()])
                    {
                        // 中央に着地するときの処理
                        case POSITION.MID:
                            {
                                // 降りる位置を指定して移動する
                                GetRigidbody().position = new Vector3(posData.GetMidPos(), this.transform.position.y, 0);
                            }
                            break;
                        // 左側に着地するときの処理
                        case POSITION.LEFT:
                            {
                                // 降りる位置を指定して移動する
                                GetRigidbody().position = new Vector3(posData.GetLeftPos(), this.transform.position.y, 0);
                            }
                            break;
                        // 右側に着地するときの処理
                        case POSITION.RIGHT:
                            {
                                // 降りる位置を指定して移動する
                                GetRigidbody().position = new Vector3(posData.GetRightPos(), this.transform.position.y, 0);
                            }
                            break;
                    }
                }
                
            }//----- if_stop -----

            // １フレーム遅延させる
            yield return null;

        }//----- while_stop -----

        // ノックアウト処理のとき
        if(_KO)
        {
            // 自身が最終落下地点より上に存在しているなら
            while(this.transform.position.y > (startJumpPos - endFall))
            {
                // 自身に速度を付与する
                GetRigidbody().velocity = new Vector3(GetRigidbody().velocity.x, (float)spJumpPower, 0);

                // １フレーム遅延させる
                yield return null;

            }//----- while_stop -----

            bossCamera.bossrightcheck = true;

        }//----- if_stop -----

        // 着地処理本体 ------------------------------*
        // 自身の速度を無くす
        GetRigidbody().velocity = Vector3.zero;

        if(_KO)
        {
            // 通り過ぎる状態で自身の位置を調整する
            GetRigidbody().position = new Vector3(GetRigidbody().position.x, startJumpPos - endFall, 0);
        }
        else
        {
            // 通り過ぎないように自身の位置を調整する
            GetRigidbody().position = new Vector3(GetRigidbody().position.x, startJumpPos, 0);
        }

        // 着地用アニメーションを開始する
        GetAnimator().SetBool("JumpEnding",true);

        // 移動を終了する
        inMoving = false;

        Debug.Log("* Used jump function.");
    }

    private IEnumerator Summon()
    {
        Debug.Log("* Use summon function.");

        // 召喚モブの情報を取得
        demo_Enemy_A demoEnemy;

        // 召喚アニメーション
        GetAnimator().Play("Command");
        //GetEffectManager().StartSummon();

        // 召喚までの待機時間
        yield return StartCoroutine(IdleTime(summonTime));

        // 召喚する位置を自身の位置から指定する
        switch(standPoint[GetPhase()])
        {
            // 自身の位置が中央になっているなら
            case POSITION.MID:
                {
                    // 乱数の取得
                    int random = Random.Range(0, 2);

                    if (random == 0)
                    {
                        summonedMob = Instantiate(summon.GetMobObject(), summon.GetLeftData().GetPosition(), Quaternion.identity);

                        // 召喚したモンスターのスクリプトを取得
                        demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                        demoEnemy.SetDistanceMin(summon.GetLeftData().GetDistanceMin());
                        demoEnemy.SetDistanceMax(summon.GetLeftData().GetDistanceMax());
                        demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.RIGHT);
                    }
                    else if (random == 1)
                    {
                        summonedMob = Instantiate(summon.GetMobObject(), summon.GetRightData().GetPosition(), Quaternion.identity);

                        // 召喚したモンスターのスクリプトを取得
                        demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                        demoEnemy.SetDistanceMin(summon.GetRightData().GetDistanceMin());
                        demoEnemy.SetDistanceMax(summon.GetRightData().GetDistanceMax());
                        demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.LEFT);
                    }
                }
                break;
            // 自身の位置が左になっているなら
            case POSITION.LEFT:
                {
                    // モブの召喚を行う
                    summonedMob = Instantiate(summon.GetMobObject(), summon.GetRightData().GetPosition(), Quaternion.identity);

                    

                    // 召喚したモンスターのスクリプトを取得
                    demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                    demoEnemy.SetDistanceMin(summon.GetRightData().GetDistanceMin());
                    demoEnemy.SetDistanceMax(summon.GetRightData().GetDistanceMax());
                    demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.LEFT);
                }
                break;
            // 自身の位置が右になっているなら
            case POSITION.RIGHT:
                {
                    // モブの召喚を行う
                    summonedMob = Instantiate(summon.GetMobObject(), summon.GetLeftData().GetPosition(), Quaternion.identity);

                    

                    // 召喚したモンスターのスクリプトを取得
                    demoEnemy = summonedMob.GetComponent<demo_Enemy_A>();
                    demoEnemy.SetDistanceMin(summon.GetLeftData().GetDistanceMin());
                    demoEnemy.SetDistanceMax(summon.GetLeftData().GetDistanceMax());
                    demoEnemy.SetDirection(demo_Enemy_A.DIRECTION.RIGHT);
                }
                break;
        }

        //GetEffectManager().StopSummon();

        Debug.Log("* Used summon function.");
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Wave"))
        {
            var col = other.gameObject.GetComponent<waveCollition>();
            if(col.CheckType(waveCollition.WAVE_TYPE.PLAYER_ENEMY))
            {
                SetState(STATE.DAMAGE);
            }

        }
    }
}
