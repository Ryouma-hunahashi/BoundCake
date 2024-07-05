using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==================================================
//          新規スイッチのスクリプトです
// ※旧スイッチスクリプトと別の名前になっていますが
// 　この新スイッチスクリプトで[ Ver 2 ]という扱いです。
// ※過去スイッチスクリプトに同梱されていたブロックの破壊が
// 　ここでは破棄,または変更される可能性があります。
// ※新たな機能として連動スイッチ,XOR回路等追加予定です
//==================================================
// 制作日2023/04/12
// 宮﨑
public class VariousSwitches_2 : MonoBehaviour
{
    [Tooltip("スイッチの状態")]
    public bool switchStatus;
    private bool saveSwitchStatus;

    [Header("----- 連動時の設定 -----"), Space(5)]
    [SerializeField] private GameObject myParent;                                   // 親オブジェクトの取得
    [SerializeField] private VariousSwitches_2 parentScript;
    [SerializeField] private SwitchConditionChenge parentChenger;
    [SerializeField] private bool parentActive;

    [SerializeField] private List<GameObject> myChildren = new List<GameObject>();  // 子オブジェクトの取得
    [SerializeField] private List<VariousSwitches_2> childScripts = new List<VariousSwitches_2>();
    [SerializeField] private List<SwitchConditionChenge> childChengers = new List<SwitchConditionChenge>();
    [SerializeField] private bool childrenActive;

    [Header("XOR連動の設定")]
    [SerializeField] private GameObject versusSwitch;
    [SerializeField] private VariousSwitches_2 versusScript;

    //private Renderer test;

    [System.NonSerialized] public waveCollition collisionScript;

    private void Start()
    {
        //test = this.GetComponent<Renderer>();

        // 親が存在しているなら
        if (this.transform.parent != null)
        {
            // 親オブジェクトを取得する
            myParent = this.transform.parent.gameObject;
            parentScript = this.transform.parent.GetComponent<VariousSwitches_2>();
            parentChenger = this.transform.parent.GetComponent<SwitchConditionChenge>();
            if (parentScript != null)
            {
                // 親オブジェクトが存在している
                parentActive = true;

                // 自身の名前を変更する
                this.gameObject.name = "childSwitch";
            }
        }//----- if_stop -----

        // 子が存在しているなら
        if (this.transform.childCount != 0)
        {
            // 子オブジェクトの数を取得
            int childCount = this.transform.childCount;

            // リストを一度初期化する
            myChildren.Clear();
            childScripts.Clear();
            childChengers.Clear();

            // 自身についている子オブジェクトを取得する
            for (int i = 0; i < childCount; i++)
            {
                // 子オブジェクトをリスト内に格納
                myChildren.Add(transform.GetChild(i).gameObject);
                childScripts.Add(transform.GetChild(i).GetComponent<VariousSwitches_2>());
                childChengers.Add(transform.GetChild(i).GetComponent<SwitchConditionChenge>());

            }//----- for_stop -----

            // 子オブジェクトが存在している
            childrenActive = true;

            // 自身の名前を変更する
            this.gameObject.name = "parentSwitch";

        }//----- if_stop -----

        // 対が存在しているなら
        if (this.versusSwitch != null)
        {
            // 対のスクリプト情報を取得
            versusScript = this.versusSwitch.GetComponent<VariousSwitches_2>();
        }//----- if_stop -----

        // 最初に保持している値と同値の場合実行
        if (switchStatus == saveSwitchStatus)
        {
            // 保持している値を変更する
            saveSwitchStatus = !saveSwitchStatus;

        }//----- if_stop -----

    }

    private void Update()
    {
        // スイッチの状態に変更があった場合
        if (switchStatus != saveSwitchStatus)
        {
            // ログに現在のスイッチの状態を保存する
            saveSwitchStatus = switchStatus;

            // 親オブジェクトのみが存在しているなら
            if (parentActive && !childrenActive)
            {
                Debug.Log("親のみ存在");

                // 自身が変更されたときに親の状態を変更する
                parentScript.switchStatus = switchStatus;

            }//----- if_stop -----
            // 子オブジェクトのみが存在しているなら
            else if (childrenActive && !parentActive)
            {
                Debug.Log("子のみ存在");

                // 自身が変更されたときに子の状態を変更する
                for (int i = 0; i < childScripts.Count; i++)
                {
                    Debug.Log("変更された数");

                    childScripts[i].switchStatus = switchStatus;
                }//----- for_stop -----

            }//----- elseif_stop -----
            // 親子共にオブジェクトが存在していないなら
            else if (!parentActive && !childrenActive)
            {
                Debug.Log("親子存在していない");

            }//----- elseif_stop -----

            //// テストで色を変更する
            //if (switchStatus)
            //{
            //    test.material.color = Color.blue;

            //}//----- if_stop -----
            //else
            //{
            //    test.material.color = Color.red;

            //}//----- else_stop -----

        }//----- if_stop -----

        // 対が存在しているなら
        if (versusSwitch != null)
        {
            // 対と同じ状態になったなら
            if (versusScript.switchStatus == switchStatus)
            {
                Debug.Log("対判定");

                // 今の状態を反転させる
                switchStatus = !switchStatus;

            }//----- if_stop -----
        }//----- if_stop -----

        //// 親オブジェクトのみが存在しているなら
        //if (parentActive && !childrenActive)
        //{
        //    Debug.Log("親だけいま～す");

        //    // 自身の状態が変更されたなら
        //    if(switchStatus != saveSwitchStatus)
        //    {
        //        // 親の状態を上書きする
        //        parentScript.switchStatus = switchStatus;

        //    }//----- if_stop -----
        //}
        //// 子オブジェクトのみが存在しているなら
        //else if(childrenActive && !parentActive)
        //{
        //    Debug.Log("子だけいま～す");

        //    if (switchStatus != saveSwitchStatus)
        //    {
        //        saveSwitchStatus = switchStatus;

        //        if (switchStatus)
        //        {
        //            GetComponent<Renderer>().material.color = Color.blue;
        //        }
        //        else
        //        {
        //            GetComponent<Renderer>().material.color = Color.red;
        //        }
        //    }
        //}
        //// 親,子オブジェクトが共に存在していない
        //else if(!parentActive && !childrenActive)
        //{
        //    Debug.Log("両方いませ～ん");

        //    if (switchStatus != saveSwitchStatus)
        //    {
        //        saveSwitchStatus = switchStatus;

        //        if (switchStatus)
        //        {
        //            GetComponent<Renderer>().material.color = Color.blue;
        //        }
        //        else
        //        {
        //            GetComponent<Renderer>().material.color = Color.red;
        //        }
        //    }
        //}
    }

    public void AddFamilyChangeCount()
    {
        if(parentActive)
        {
            parentChenger.AddChangeCount();
        }
        if(childrenActive)
        {
            for(byte i =0; i < childChengers.Count; i++)
            {
                if(childChengers[i] != null)
                {
                    childChengers[i].AddChangeCount();
                }
            }
        }
    }

    public void SetFamilyCoolTime()
    {
        if (parentActive)
        {
            parentChenger.SetCoolTime();
        }
        if (childrenActive)
        {
            for (byte i = 0; i < childChengers.Count; i++)
            {
                if (childChengers[i] != null)
                {
                    childChengers[i].SetCoolTime();
                }
            }
        }
    }
   

    private void OnApplicationQuit()
    {
        myChildren.Clear();
        childScripts.Clear();
        childChengers.Clear();
    }

}
