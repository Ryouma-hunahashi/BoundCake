using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_MobSpawn : MonoBehaviour
{
    public GameObject boss5Obj;
    // ボスのメインスクリプトを格納
    private Boss5_Main boss5Main;

    // 自身の子の情報
    private GameObject childObj;

    [Tooltip("指定されたフェイズで召喚します")]
    [SerializeField] private byte phaseSpawn;
    [SerializeField] private byte smakeFrame = 30;
    private bool spawnFlag = true;
    [SerializeField] private bool started;

    private void Start()
    {
        boss5Main = boss5Obj.GetComponent<Boss5_Main>();
        // 自身の子オブジェクトを取得する
        childObj = this.transform.GetChild(0).gameObject;
    }

    private void FixedUpdate()
    {
        // ボスの動きが開始されているとき
        if(boss5Main.startAction || started)
        {

            // ボスのフェイズが設定したフェイズと一致したときに
            if (spawnFlag && boss5Main.nowPhase == phaseSpawn)
            {
                //------------スポーンエフェクト------------
                // 自身の子オブジェクトをアクティブにする
                childObj.SetActive(true);
                spawnFlag = false;
                StartCoroutine(EffectStop());

            }//----- if_stop -----
        }

        if(boss5Main.status.hitPoint <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator EffectStop()
    {
        for (byte i = 0;i<smakeFrame;i++)
        {
            yield return null;
        }
        // 靄エフェクト止める
    }
}
