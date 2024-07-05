using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//======================================================
//          version ： 1.1   概要
// ・ボス専用ステータスを追加しました
//======================================================
// 対応日2023/04/08
// 宮﨑
[System.Serializable]
public class BossStatusManager
{
    // ボスの名前
    [Tooltip("ボスの名前")]
    public string name = "";
    // 攻撃の状態
    [Tooltip("攻撃中かどうか")]
    public bool nowAttack;
    // 各ステータス
    [Tooltip("ボスのHP設定")]
    public sbyte hitPoint;
}

public class StatusManager : MonoBehaviour
{
    static public int maxHitPoint = 6;      // 最大体力
    static public int nowHitPoint = 3;      // 今の体力
    static public int coinCount = 0;        // コインの取得枚数
    static public int gameScore = 0;        // ゲーム全体のスコア
}
