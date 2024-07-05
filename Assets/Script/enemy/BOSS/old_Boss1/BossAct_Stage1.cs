using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAct_Stage1 : MonoBehaviour
{
    // 自身の情報を取得
    private Rigidbody rb;
    // 自身のアニメーター
    private Animator anim;




    [SerializeField] private bool actTest = true;

    [SerializeField] private bool damge;
    [SerializeField] private bool invincibility;    // 無敵状態

    // クラスの取得
    [SerializeField] public BossStatusManager bossStatusManager;

    // ボスの行動
    [Header("ボス挙動")]
    [SerializeField] private Enemy_A_Rush rushAtk;  // 突進攻撃の格納
    [SerializeField] private Enemy_D_Shield guard;  // 防御状態の格納

    [SerializeField] private List<GameObject> childObjects = new List<GameObject>();

    [SerializeField] private GameObject waveHit;
    [SerializeField] private GameObject waveLog;

    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Wave") && guard.guardNow)
        {
            Debug.Log("４");
            waveHit = other.gameObject;
        }

        if (other.CompareTag("Wave") && guard.shieldBreak && !invincibility && !guard.guardNow && !rushAtk.rushNow)
        {
            if (waveHit == null)
            {
                Debug.Log("１");
                waveHit = other.gameObject;

                for (int i = 0; i < guard.waveCount; i++)
                {
                    if (guard.l_waveCollisionObj[i] == waveHit)
                    {
                        return;
                    }
                }

                bossStatusManager.hitPoint--;

                Debug.Log("ダメージ！");
                anim.Play("Damage");
                anim.SetBool("staning",false);

                // フェイズを進行する
                if (guard.phaseNow < guard.shieldPhase.Length)
                {
                    Debug.Log("フェイズ変更！");
                    guard.phaseNow++;
                }

                // 自身にダメージを与えられない状態にする
                invincibility = true;

                if (waveHit != waveLog)
                {
                    Debug.Log("２");
                    waveLog = waveHit;
                }
            }
        }

        //if (1<<other.gameObject.layer == 1 << 6)
        //{
        //    guard.vfxManager = other.transform.GetChild(0).GetComponent<vfxManager>();
        //}

        //// [Wave]タグがついているオブジェクトに触れた時 &
        //// 盾が破壊されているとき
        //// 無敵状態ではないなら
        //if (other.CompareTag("Wave") && guard.shieldBreak && !invincibility && !guard.stopDamage && !guard.guardNow && !rushAtk.rushNow && guard.notRepiar)
        //{
        //    // 自身の体力を減少させる
        //    bossStatusManager.status.hitPoint--;

        //    // フェイズを進行する
        //    if (guard.phaseNow < guard.shieldPhase.Length)
        //    {
        //        guard.phaseNow++;
        //    }

        //    // 自身にダメージを与えられない状態にする
        //    invincibility = true;

        //}//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Wave"))
        {

            if (waveHit != null)
            {
                Debug.Log("３");
                waveHit = null;

                // 自身にダメージを与えられる状態にする
                invincibility = false;
            }
        }
    }

    private void Start()
    {
        // ボスの各挙動を格納
        rushAtk = this.GetComponentInChildren<Enemy_A_Rush>();
        guard = this.GetComponentInChildren<Enemy_D_Shield>();

        // [Rigidbody]の取得
        rb = GetComponent<Rigidbody>();

        // うまく取得できなかった場合
        if (rb == null)
        {
            Debug.LogError("[Rigidbody]が見つかりません！");

        }//----- if_stop -----

        anim = GetComponent<Animator>();
        if(anim == null)
        {
            Debug.LogError("[Animator]が見つかりません");
        }

        // ボスの名前が設定されているなら
        if (bossStatusManager.name != "")
        {
            // 設定されている名前に変更する
            this.gameObject.name = bossStatusManager.name;

        }//----- if_stop -----

        // 子オブジェクトの数を取得
        int childCount = this.transform.childCount;

        // リストを一度初期化する
        childObjects.Clear();

        // 自身についている子オブジェクトを取得する
        for (int i = 0; i < childCount; i++)
        {
            // 子オブジェクトをリスト内に格納
            childObjects.Add(transform.GetChild(i).gameObject);

        }//----- for_stop -----
    }

    private void FixedUpdate()
    {
        if (!guard.rotateNow && !rushAtk.rushNow && guard.rotateEnd && guard.shieldImpactEnd && guard.guardEnd && !guard.shieldRepairNow && !guard.shieldBreak && guard.startup)
        {
            // 突進開始 ----- この辺にあにめしょん
            anim.SetBool("rushing", true);
            rushAtk.StartCoroutine(rushAtk.RushAction());

            guard.startup = false;
        }

        if (!guard.rotateNow && !rushAtk.rushNow && !guard.shieldImpactNow && 
            guard.shieldImpactEnd && !guard.guardNow && rushAtk.rushEnd && 
            !guard.shieldRepairNow && !guard.shieldBreak || (guard.shieldRepairEnd && 
            !guard.shieldBreak) || actTest)
        {
            // 振り向き開始 ----- この辺にあにめしょん
            anim.SetBool("rushing",false);
            anim.Play("ShieldSet");

            // 振り向きと盾構えはほぼ同時です

            // 盾の位置を変更する
            guard.ShieldImpact();

            // 回転待機を開始する
            guard.StartCoroutine(guard.RotateWaitTime());

            guard.shieldRepairEnd = false;
            actTest = false;
        }
        else if (!guard.rotateNow && !rushAtk.rushNow && !guard.guardNow && guard.rotateEnd && guard.shieldImpactNow && !guard.shieldImpactEnd && !guard.shieldRepairNow)
        {
            // 盾振り下ろし ----- この辺にあにめしょん
            

            // 盾を設定位置まで振り下ろす
            guard.ShieldImpact();
        }

        if (guard.lastPhase && (bossStatusManager.hitPoint <= 0))
        {
            // 格納しているオブジェクトの破棄
            childObjects.Clear();
            waveHit = null;
            waveLog = null;

            // このオブジェクトを破壊する
            gameObject.SetActive(false);
        }
    }
}