using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchStats : MonoBehaviour
{
    // オブジェクトの種類の設定
    private enum ObjectType
    {
        breakObject,
        itemObject,
        switchObject,

        none,   // なにも設定していない
    }

    [Header("----- 基本設定 -----"), Space(5)]
    [Tooltip("※対応オブジェクトの設定")]
    [SerializeField] private ObjectType type = ObjectType.none;

    [Header("----- ブロック設定 -----")]
    // プレハブの格納
    [Tooltip("※ブロックから出現させるPrefabを設定")]
    [SerializeField] private GameObject itemPrefab;

    [Header("----- スイッチ設定 -----"), Space(5)]
    // アクティブ状態を外部へ渡す
    [Tooltip(
        "現在のスイッチの状態( ON / OFF )\n" +
        "この状態は外部へ渡します")]
    public bool activeSwitch;

    

    // このスクリプト内のみのスイッチの状態
    private bool switchSituation;

    // スイッチの種類の設定
    private enum SwitchModeList
    {//----- enum_start -----

        // press,      // 押している間のみ[true]
        flipflop,   // 押しなおすまで[true]
        none,

    }//----- enum_stop -----

    [Header("----- 破壊設定 -----")]
    [SerializeField] private string[] tagName;

    // インスペクター上でスイッチの動きを変更する
    [Tooltip("※スイッチの起動方法を設定")]
    [SerializeField] SwitchModeList switchMode = SwitchModeList.none;


    // オブジェクトが何かに触れているときの判定
    [Header("----- 状態確認用 -----"), Space(5)]
    [Tooltip("オブジェクトが何かに触れたかの判定")]
    [SerializeField] private bool hitObject;

    // 衝突時の判定を取得
    private void OnTriggerEnter(Collider collider)
    {
        switch (type)
        {
            case ObjectType.breakObject:
                // [Wave]タグのオブジェクトに触れたなら
                if (CheckTag(collider.gameObject.tag))
                {//----- if_start -----

                    // ブロック破壊のアニメーション
                    // 予定地

                    // デバッグログ
                    Debug.Log("[Bullet]タグの付いたオブジェクトによって破壊されました！");

                    // 自分自身を破壊する
                    gameObject.SetActive(false);

                }//----- if_stop -----
                break;
            case ObjectType.itemObject:
                // [Wave]タグのオブジェクトに触れたなら
                if (collider.gameObject.CompareTag("Wave"))
                {//----- if_start -----

                    // デバッグログ
                    Debug.Log("[Wave]タグの付いたオブジェクトがぶつかりました！");

                    // 何かが当たった判定を出す
                    hitObject = true;

                }//----- if_stop -----
                break;
            case ObjectType.switchObject:
                hitObject = true;
                break;
            default:
                break;
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        switch (type)
        {
            case ObjectType.breakObject:
                // [Wave]タグのオブジェクトに触れたなら
                if (CheckTag(other.gameObject.tag))
                {//----- if_start -----

                    // ブロック破壊のアニメーション
                    // 予定地

                    // デバッグログ
                    Debug.Log("[Bullet]タグの付いたオブジェクトによって破壊されました！");

                    // 自分自身を破壊する
                    gameObject.SetActive(false);

                }//----- if_stop -----
                break;
            case ObjectType.itemObject:
                // [Wave]タグのオブジェクトに触れたなら
                if (other.gameObject.CompareTag("Wave"))
                {//----- if_start -----

                    // デバッグログ
                    Debug.Log("[Wave]タグの付いたオブジェクトがぶつかりました！");

                    // 何かが当たった判定を出す
                    hitObject = true;

                }//----- if_stop -----
                break;
            case ObjectType.switchObject:
                hitObject = true;
                break;
            default:
                break;
        }
    }

    // 乖離時の判定を取得
    private void OnCollisionExit(Collision collision)
    {
        hitObject = false;
    }

    private bool CheckTag(string _objTag)
    {
        for(int i = 0; i < tagName.Length; i++)
        {
            if(_objTag == tagName[i])
            {
                return true;
            }
        }

        return false;
    }

    //==================================================
    //      アイテム出現ブロック
    // ※[Wave]ヒット時に指定されたPrefabを出現させる関数です
    //==================================================
    // 制作日2023/03/12
    // 宮﨑
    public void HitItemBlock()
    {
        // 生成位置
        Vector3 pos = new Vector3(
            this.transform.position.x,
            this.transform.position.y + 1,
            this.transform.position.z);

        Instantiate(itemPrefab,pos,Quaternion.identity);

        // アイテム出現に関して
        // ジャンプバグあるので一時的にこれで、
        // すぐに対処可能なので時間ある人できれば知恵かしてください
        // この関数を[bool]で渡すようにして
        // 取得した[bool]で出し方変更してください
        
        hitObject = false;
    }

    //==================================================
    //      フリップフロップ回路
    // ※オン、オフの切り替えに押しなおしが必要になるスイッチです
    //==================================================
    // 制作日2023/03/9
    // 宮﨑
    public bool FlipFlopSwitch()
    {
        // 何かオブジェクトが自分自身に触れたとき
        if(hitObject)
        {//----- if_start -----

            // スイッチの状態が
            // [true]のときは[false]に切り替わる
            // [false]のときは[true]に切り替わる
            if(activeSwitch && !switchSituation )
            {//----- if_start -----

                activeSwitch = false;

            }//----- if_stop -----
            else if(!activeSwitch && !switchSituation)
            {//----- elseif_start -----

                activeSwitch = true;

            }//----- elseif_stop -----

            // 何かが当たっている状態なら
            // スイッチの状態をオンにする
            switchSituation = true;

        }//----- if_stop -----
        else
        {//----- else_start -----

            // 何も当たっていない状態なら
            // スイッチの状態をオフにする
            switchSituation = false;

        }//----- else_stop -----

        // [activeSwitch]の状態を送る
        return activeSwitch;
    }

    private void Update()
    {
        switch (type)
        {
            case ObjectType.breakObject:
                break;
            case ObjectType.itemObject:
                // プレハブが格納されていないときにエラーを表示
                if (itemPrefab == null)
                {
                    Debug.LogError("[itemPrefab]内にオブジェクトが格納されていません！");
                }

                // オブジェクトが当たったときの処理
                if (hitObject)
                {
                    HitItemBlock();
                }
                break;
            case ObjectType.switchObject:
                switch (switchMode)
                {//----- switch_start -----

                    //case SwitchModeList.press:
                    //    Debug.LogError("制作されていない状態が設定されています");
                    //    break;
                    case SwitchModeList.flipflop:
                        FlipFlopSwitch();
                        break;
                    case SwitchModeList.none:
                    default:
                        Debug.LogError("スイッチの動きが設定されていません！");
                        break;

                }//----- switch_stop -----
                break;
            case ObjectType.none:
            default:
                Debug.LogError("オブジェクトの種類が設定されていません！");
                break;
        }
    }

}
