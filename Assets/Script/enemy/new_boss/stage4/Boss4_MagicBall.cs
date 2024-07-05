using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ボス魔法攻撃
// ※召喚地点をもとに存在している分身と本体から攻撃
//==================================================
// 制作日2023/05/24    更新日2023/05/30
// 宮﨑
public class Boss4_MagicBall : MonoBehaviour
{
    // 指令の情報を取得
    private GameObject avatarOperator;
    private Setup_AvatarPoint opAvatarMain;

    // 親の情報を取得
    private Boss4_Main parMain;

    // 待機時間
    [SerializeField] private byte atkDelay = 240;

    // 現在の状態
    public bool shot;

    private void Start()
    {
        // 指令の情報を取得
        avatarOperator = GameObject.Find("AvatarPointOperator").gameObject;
        opAvatarMain = avatarOperator.GetComponent<Setup_AvatarPoint>();

        // 親の情報を取得
        GameObject parObj = this.transform.parent.gameObject;
        parMain = parObj.GetComponent<Boss4_Main>();
    }

    public IEnumerator AttackDelay()
    {
        // 待機時間を開始
        for(int i = 0; i < atkDelay; i++)
        {
            // １フレーム遅延させる
            yield return null;

            // ダメージリアクションが行われたなら処理を抜ける
            if (parMain.nowDmgReAct) yield break;
        }

        // 待機時間を抜け攻撃へ移行
        MagicAttack(parMain.magicBall);
    }

    //==================================================
    //          格納されたオブジェクトを射出する関数
    // ※自身と倒されていない分身の地点から
    // 　渡されたオブジェクトを出現させます
    // 戻り値：なし
    //  引数 ：_magicPrefab(メインに格納したオブジェクトを使用)
    //==================================================
    // 制作日2023/05/30
    // 宮﨑
    public void MagicAttack(GameObject _magicPrefab)
    {
        Debug.Log("魔法球召喚！！");

        // 親の情報を取得
        GameObject parObj = this.transform.parent.gameObject;

        // 魔力球を本体から召喚する
        GameObject magic = Instantiate(_magicPrefab);
        magic.transform.position = parObj.transform.position;

        // 召喚地点の数を取得
        int pointCnt = avatarOperator.transform.childCount;

        parMain.audio.bossSource.Stop();
        parMain.audio.Boss4_BulletAttackSound();

        // 召喚地点の回数繰り返す
        for (int i = 0; i < pointCnt; i++)
        {
            // 分身が存在していないなら処理を飛ばす
            if (parMain.avaMain[i].active == false) continue;

            // フェイズ進行状態によって数を変更する
            if (parMain.setAvatarPhase[parMain.nowPhase] < i) continue;

            // 魔力球を召喚する
            magic = Instantiate(_magicPrefab);
            magic.transform.position = avatarOperator.transform.GetChild(i).GetChild(0).transform.position;

        }//----- for_stop -----

        shot = true;

        // 魔力弾射出後召喚待機へ移行する
        StartCoroutine(parMain.actInv.AnimationDelay());
    }
}
