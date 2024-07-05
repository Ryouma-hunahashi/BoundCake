using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Get_Parfait : MonoBehaviour
{
    // 層の指定
    private enum ParfaitLayer
    {
        top,    // 上層
        mid,    // 中層
        btm,    // 下層

        none,   // 無指定
    }

    // パフェの設定
    [SerializeField] private ParfaitLayer parfait = ParfaitLayer.none;

    [SerializeField] private bool test;

    public byte worldNum;
    public byte stageNum;

    SpriteRenderer spriteRenderer;
    StageSelector stageSelector;

    private void OnTriggerEnter(Collider other)
    {
        // プレイヤーに触れた時
        if(other.CompareTag("Player"))
        {
            // パフェの取得情報を更新する
            parfaitUpdate();

        }//----- if_stop -----
    }

    private void Start()
    {
        worldNum = Result_Manager.instance.nowWorld;
        stageNum = Result_Manager.instance.nowStage;
    }
    private void Update()
    {
        if(test)
        {
            parfaitUpdate();
            test = false;
        }
       // parfaitSecondGet();

    }

    //========================================
    //          パフェの所持更新
    //  ※ゲーム中にパフェが回収されたら実行
    //========================================
    // 作成日2023/05/20    更新日2023/05/21
    // 宮﨑
    private void parfaitUpdate()
    {
        switch(parfait)
        {
            case ParfaitLayer.top:
                Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.top = true;
                //Result_Manager.instance.getParfait.top = true;
                Debug.Log("toptrue");
                break;
            case ParfaitLayer.mid:
                Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.mid = true;
                //Result_Manager.instance.getParfait.mid = true;
                Debug.Log("middletrue");
                break;
            case ParfaitLayer.btm:
                Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.btm = true;
                //Result_Manager.instance.getParfait.btm = true;
                Debug.Log("botoomtrue");
                break;
            case ParfaitLayer.none:
            default:
                break;

        }//----- switch_stop -----
    }

    void parfaitSecondGet()
    {
        switch (parfait)
        {
            case ParfaitLayer.top:
                if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.top)
                {
                    this.gameObject.GetComponentsInChildren<SpriteRenderer>();

                    //spriteRenderer.color = new Color(1.0f, 1.0f, 1.0f, 0.35f);
                }
                break;
            case ParfaitLayer.mid:
                if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.mid)
                {

                }
                break;
            case ParfaitLayer.btm:
                if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.btm)
                {

                }
                break;
            default:
                break;
        }
    }
}
