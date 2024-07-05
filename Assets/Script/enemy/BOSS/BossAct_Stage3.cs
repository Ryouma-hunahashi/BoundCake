using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAct_Stage3 : MonoBehaviour
{
    // 遅延関数の取得
    IEnumerator bossAtk;
    IEnumerator bossCT;

    [SerializeField] private bool actTest = true;

    // ダメージ判定
    private bool damage;        // 被ダメージ時
    private bool invincibilitu; // 無敵状態

    // ステータスクラスの取得
    [SerializeField] public BossStatusManager bossStatusManager;

    // フェイズ状態
    [SerializeField] private byte phase;

    [SerializeField] private ushort[] shockTime;

    // 床生成
    private bool makeGround;    // 自身の休憩用床
    private bool[] shockGround; // 敵振動床の生成

    // 攻撃の状態
    [SerializeField] private bool nowAttack;
    [SerializeField] private bool nowCT;

    [SerializeField] private byte coolTime;

    private IEnumerator BossCoolTime()
    {
        nowCT = true;

        for(int i = 0; i < coolTime; i++)
        {
            yield return null;
        }

        nowCT = false;
    }

    private IEnumerator BossAttack()
    {
        nowAttack = true;

        for(int i = 0; i < shockTime[phase]; i++)
        {
            yield return null;
        }

        nowAttack = false;

        bossCT = BossCoolTime();
        StartCoroutine(bossCT);
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void FixedUpdate()
    {
        if(actTest || (!nowCT && !nowAttack))
        {
            bossAtk = BossAttack();
            StartCoroutine(bossAtk);

            actTest = false;
        }
    }
}