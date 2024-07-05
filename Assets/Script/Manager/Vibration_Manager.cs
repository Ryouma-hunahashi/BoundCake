using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//==================================================
//          コントローラー振動のマネージャー
// ※別スクリプトの条件がそろった時に振動させます
//==================================================
// 制作日2023/05/22    更新日2023/05/26
// 宮﨑
public class Vibration_Manager : MonoBehaviour
{
    // シングルトンの作成
    public static Vibration_Manager instanse;

    private void Awake()
    {
        // 自身が存在していないなら
        if (instanse == null)
        {
            // 自身をインスタンス化
            instanse = this;

            // シーン変更時に破棄されないようにする
            DontDestroyOnLoad(this.gameObject);
        }//----- if_stop -----
        else
        {
            // すでに自身が存在しているなら破棄
            Destroy(this.gameObject);

        }//----- else_stop -----
    }

    // 各コントローラー振動の設定用構造体
    [System.Serializable]
    private struct BothVibrationValue
    {
        public string vibName;    // 振動の名前

        // 振動継続の設定
        [Header("----- 継続の設定 -----")]
        public int loopFrame;       // 振動持続時間

        // コントローラー振動の設定
        [Header("----- 振動の設定 -----")]
        
        [Range(0f, 1f)] public float leftValue;     // 左側の振動
        [Range(0f, 1f)] public float rightValue;    // 右側の振動
    }

    // 振動の状態
    public List<bool> nowVibration;
    public bool stopVibration;      // 振動を強制的に止める

    // 各コントローラー振動の設定
    [SerializeField] private List<BothVibrationValue> vibValues = new List<BothVibrationValue>();

    // 以下編集後消滅 -----
    [SerializeField] private bool test;
    [SerializeField] private string testName;

    private void Start()
    {
        // コントローラー振動の設定がある分繰り返す
        for(int i = 0; i < vibValues.Count; i++)
        {
            // コントローラー振動状況を種類別で取得
            nowVibration.Add(false);

        }//----- for_stop -----
    }

    private void FixedUpdate()
    {
        if(stopVibration)
        {
            for(int i = 0; i < nowVibration.Count; i++)
            {
                nowVibration[i] = false;
            }

            // 振動を終了させる
            Gamepad.current.SetMotorSpeeds(0, 0);
            stopVibration = false;
        }
    }

    //==================================================
    //          コントローラー振動設定の選択
    // ※どのコンロトーラー振動設定を使用するかを
    // 　送られてきた引数を使用して調べます
    // 戻り値 ：なし
    //　引数　：(_VibName)設定の名前
    //==================================================
    // 制作日2023/05/22
    // 宮﨑
    public void VibrationSelect(string _VibName)
    {
        // ゲームパッドが接続されていないなら処理を抜ける
        if (Gamepad.current == null) return;

        // 設定されているコントローラー振動の数繰り返す
        for(int i = 0; i < vibValues.Count; i++)
        {
            // 指定された設定が実行中ならスキップ
            if (nowVibration[i]) continue;

            // 送られた名前が一致しているなら
            if (vibValues[i].vibName == _VibName)
            {
                // コントローラー振動をスタートさせる
                nowVibration[i] = true;
                StartCoroutine(VibrationUpdate(i, vibValues[i].leftValue, vibValues[i].rightValue, vibValues[i].loopFrame));

            }//----- if_stop -----

        }//----- for_stop -----
    }

    //==================================================
    //          コントローラー振動の実行
    // ※選択された設定を使用してコントローラーを振動させる
    // 戻り値 ：なし
    //　引数　：(_num)設定番号,
    //　        (_left)左振動強さ, (_right)右振動強さ
    //          (_count)振動を継続させるフレーム
    //==================================================
    // 制作日2023/05/22
    // 宮﨑
    public IEnumerator VibrationUpdate(int _num, float _left, float _right, int _count)
    {
        // コントローラーを指定の強さで振動させる
        Gamepad.current.SetMotorSpeeds(_left, _right);

        for(int i = 0; i < _count; i++)
        {
            // 途中で振動を止められたなら処理を抜ける
            if (!nowVibration[_num]) break;

            // １フレーム遅延させる
            yield return null;

        }//----- for_stop -----

        // 現在の振動を終了させる
        nowVibration[_num] = false;

        // 各設定で振動していない
        bool allFalse = true;

        // 各設定の振動状態を調べる
        for (int i = 0; i < nowVibration.Count; i++)
        {
            // １つでも振動していたなら
            if(nowVibration[i])
            {
                // なにかは振動している
                allFalse = false;

            }//----- if_stop -----

        }//----- for_stop -----

        // 各設定で振動していない状態なら
        if(allFalse)
        {
            // 振動を終了させる
            Gamepad.current.SetMotorSpeeds(0, 0);

        }//----- if_stop -----
    }

    private void OnApplicationQuit()
    {
        // 振動を終了させる
        Gamepad.current.SetMotorSpeeds(0, 0);

        // 振動中の情報を開放
        nowVibration.Clear();
        vibValues.Clear();
    }
}