using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Pafait : MonoBehaviour
{
    // ステージ内でのパフェのUI

    // パフェの取得
    public Image P_top;
    public Image P_mid;
    public Image P_btm;

    public byte worldNum;
    public byte stageNum;
    
    StageSelector StageSelector;

    // Start is called before the first frame update
    void Start()
    {
        // ステージ番号を取得
        worldNum = Result_Manager.instance.nowWorld;
        stageNum = Result_Manager.instance.nowStage;
        // 初期化
        InitParfait();
    }

    // Update is called once per frame
    void Update()
    {
        if(Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.top)
        {
            P_top.sprite = Resources.Load<Sprite>("P-top");
        }
        if(Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.mid)
        {
            P_mid.sprite = Resources.Load<Sprite>("P-mid");
        }
        if(Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.btm)
        {
            P_btm.sprite = Resources.Load<Sprite>("P-btm");
        }
    }
    private void InitParfait()
    {
        // 初期設定
        // パフェは最初点線だけにする
        if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.btm)
        {
            P_top.sprite = Resources.Load<Sprite>("P-btm");
        }
        else
        {
            P_top.sprite = Resources.Load<Sprite>("NotParfait");
        }
        if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.mid)
        {
            P_top.sprite = Resources.Load<Sprite>("P-mid");
        }
        else
        {
            P_top.sprite = Resources.Load<Sprite>("NotParfait");
        }
        
        if (Stage_Manager.instanse.worldInformation[worldNum].stageInformation[stageNum].parfait.top)
        {
            P_top.sprite = Resources.Load<Sprite>("P-top");
        }
        else
        {
            P_top.sprite = Resources.Load<Sprite>("NotParfait");
        }
    }
}
