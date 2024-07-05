using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    public enum STATE
    {
        // 演出系統
        WARNING,    // 登場
        ENDING,     // 討伐
        PHASE,      // フェイズ変更

        // 攻撃以外の状態
        IDLE,
        MOVE,
        DAMAGE,
        STAN,

        // 攻撃状態
        ATK01,
        ATK02, 
        ATK03, 
        ATK04,
        ATK05,
    }

    // 自身の状態
    private STATE state;

    // ゲットセッタ
    public STATE GetState() { return state; }
    public void SetState(STATE _state) { this.state = _state; }

    // オブジェクトについているデータ
    private Rigidbody rb;
    private Animator anim;
    private BossAudio audio;

    // 付属マネージャー
    private EnemyEffectManager fxManager;

    // ゲットセッタ
    public Rigidbody GetRigidbody() { return rb; }
    public Animator GetAnimator() { return anim; }
    public BossAudio GetAudio() { return audio; }
    public EnemyEffectManager GetEffectManager() { return fxManager; }

    // ターゲットの情報
    [SerializeField] private GameObject target;

    // ゲットセッタ
    public GameObject GetTarget() { return target; }

    // フェイズ状態のデータ
    private byte phase;

    public void NextPhase() { phase++; }    // フェイズを上昇させる
    public int GetPhase() { return phase; } // 今のフェイズを取得する

    // 待機時間の設定
    [SerializeField] private float idleTime;

    private void Awake()
    {
        // 自身の情報を取得
        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();
        audio = this.GetComponent<BossAudio>();
        fxManager = this.GetComponentInChildren<EnemyEffectManager>();

        // ターゲットが設定されていないなら
        if(!target)
        {
            // プレイヤーを検索して格納する
            target = GameObject.Find("player").gameObject;
        }
    }

    /// <summary>
    /// 引数や変数の値によって待機時間が変化する関数
    /// 引数の設定：
    /// (-1)待機しない
    /// (０)変数を使用する
    /// (１)引数を利用する
    /// </summary>
    /// 戻り値：なし
    ///  引数 ：(1)待機する時間
    /// </summary>
    public IEnumerator IdleTime(float _wait = 0)
    {
        // 引数の数値が"０未満"なら処理を抜ける
        if (_wait < 0) yield break;

        // 待機開始のログを表示
        Debug.Log("< " + this.name + " >：待機状態に移行しました。");

        // 引数の数値が"0ではない"なら
        if (_wait != 0)
        {
            // 引数を使用するログを表示
            Debug.Log("< " + this.name + " >：[ 待機状態 ]に引数を使用して（" + _wait + "）秒待機します");

            // 待機処理を指定した引数で行う
            yield return new WaitForSeconds(_wait);

        }//----- if_stop -----
        else
        {
            // 変数を使用するログを表示
            Debug.Log("< " + this.name + " >：[ 待機状態 ]に変数を使用して（" + idleTime + "）秒待機します");

            // 待機処理を指定した変数で行う
            yield return new WaitForSeconds(idleTime);

        }//----- else_stop -----
    }
}
