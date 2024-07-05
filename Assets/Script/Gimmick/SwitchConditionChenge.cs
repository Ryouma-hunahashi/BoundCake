using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//===================================
// スイッチの切り替えを行う処理。
// 作動させたいスイッチスクリプトと同じオブジェクトにコンポーネントしてください。
//===================================
// 作成日 2023/05/13
// 高田
public class SwitchConditionChenge : MonoBehaviour
{

    
    enum Switch_Type
    { 
        ENTER,
        STAY,
        EXIT,
    }

    enum Active_Parson
    {
        PLAYER,
        ENEMY,
        WAVE,
        COIN,
    }

    enum Default_Status
    {
        ON,
        OFF,
    }

    enum Switch_Rule
    {
        OnToOff,
        OffToOn,
        none,
    }

    enum Switch_Times
    {
        LIMIT,
        UNLIMIT,
    }

    

    private VariousSwitches_2 varSwich;     // スイッチのスクリプト
    [Header("スイッチのタイプ")]
    [Tooltip("TRIGGER：当たった時\n" +
             "STAY   ：当たっている間\n" +
             "EXIT   ：離れた時")]
    [SerializeField] private Switch_Type type = Switch_Type.ENTER;
    
    [Header("スイッチをオンにするオブジェクト")]
    [SerializeField] private Active_Parson[] activeParson = { Active_Parson.WAVE };
    [Header("スイッチの初期状態")]
    [SerializeField] private Default_Status defaultStatus;
    private bool defaultSwitch = false;
    [Header("スイッチの挙動制限(Stayの場合は使用しないでください)")]
    [Tooltip("ONtoOFF：ONからOFFのみ変更可\n" +
             "OFFtoON：OFFからONのみ変更可\n" +
             "none   ：当たるたびに切り替え")]
    [SerializeField] private Switch_Rule rule = Switch_Rule.none;



    [Header("スイッチの作動回数")]
    [Tooltip("LIMIT  ：限界あり\n" +
             "UNLIMIT：無制限")]
    [SerializeField] private Switch_Times switchTimes = Switch_Times.UNLIMIT;
    [Tooltip("作動回数が[ LIMIT ]の場合、回数を指定")]
    [SerializeField] private byte limitTimes = 1;
    private byte nowTime = 0;        // 現在のスイッチ作動回数
    [Header("スイッチのクールタイム")]
    [SerializeField] private Default_Status coolTimeCheck = Default_Status.ON;
    [SerializeField] private float coolTime = 30;
    private byte nowCoolTime = 0;
    private bool nowCool = false;
    
    // Start is called before the first frame update
    void Start()
    {
        // このオブジェクト内のスイッチを格納
        varSwich = GetComponent<VariousSwitches_2>();
        
        // 限界値が0以下の時は1に固定。(バグり散らかすため)
        if(limitTimes <= 0)
        {
            limitTimes = 1;
        }

        // Stayの時はクールタイムを発生させないようにする。
        if(type == Switch_Type.STAY)
        {
            coolTimeCheck = Default_Status.OFF;
        }

        // 初期のスイッチ状態を格納
        switch(defaultStatus)
        {
            case Default_Status.ON:
                varSwich.switchStatus = true;
                defaultSwitch = true;
                break;
            case Default_Status.OFF:
                varSwich.switchStatus= false;
                defaultSwitch = false;
                break;
        }
        
    }

    // Update is called once per frame
    //void Update()
    //{

    //}

    private void FixedUpdate()
    {
        if(nowCool)
        {
            if(nowCoolTime<coolTime)
            {
                nowCoolTime++;
            }
            else
            {
                nowCoolTime = 0;
                nowCool = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!nowCool)
        {
            // 回数制限がないか、回数制限に至っていないとき
            if (switchTimes == Switch_Times.UNLIMIT || nowTime < limitTimes)
            {
                // 当たった時、当たっている間のスイッチを切り替える。
                switch (type)
                {
                    case Switch_Type.ENTER:
                        ActiveCheck(other);
                        break;
                        //case Switch_Type.STAY:
                        //    ActiveCheck(other);
                        //    break;

                }
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (varSwich.switchStatus == defaultSwitch)
        {
            // 回数制限がないか、回数制限に至っていないとき
            if (switchTimes == Switch_Times.UNLIMIT || nowTime <= limitTimes)
            {
                // 当たっている間、離れた時のスイッチを切り替える。
                switch (type)
                {
                    case Switch_Type.STAY:
                        ActiveCheck(other);
                        break;
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        // 回数制限がないか、回数制限に至っていないとき
        if (switchTimes == Switch_Times.UNLIMIT || nowTime < limitTimes)
        {
            // 当たっている間、離れた時のスイッチを切り替える。
            switch (type)
            {
                case Switch_Type.STAY:
                    ActiveCheck(other);
                    break;
                case Switch_Type.EXIT:
                    if (!nowCool)
                    {
                        ActiveCheck(other);
                    }
                    break;
            }
        }
    }

    //===============================================
    // スイッチを切り替えられる対象に応じて、切り替えるかの判別を行う
    // 引数：ぶつかったオブジェクトのCollider
    // 戻り値無し
    //===============================================
    // 作成日 2023/05/13
    // 高田
    private void ActiveCheck(Collider other)
    {
        // 作動させられる対象の数繰り返す
        for (byte i = 0; i < activeParson.Length; i++)
        {
            switch (activeParson[i])
            {
                case Active_Parson.PLAYER:
                    if(other.CompareTag("Player"))
                    {
                        switchChange();
                    }
                    break;
                case Active_Parson.ENEMY:
                    if (other.CompareTag("Enemy"))
                    {
                        switchChange();
                    }
                    break;
                case Active_Parson.WAVE:
                    if (other.CompareTag("Wave"))
                    {
                        switchChange();
                        varSwich.collisionScript = other.GetComponent<waveCollition>();
                    }
                    break;
                case Active_Parson.COIN:
                    if (other.CompareTag("Coin"))
                    {
                        switchChange();
                    }
                    break;
            }
            
        }
    }
    //======================================
    // スイッチを切り替える処理
    // 引数、戻り値無し
    //======================================
    // 作成日 2023/05/13
    // 高田
    private void switchChange()
    {
        
        switch (rule)
        {
            case Switch_Rule.OnToOff:
                if (varSwich.switchStatus)
                {
                    varSwich.switchStatus = false;
                    // 切り替え限界が存在すれば、切り替え回数を加算
                    if (switchTimes == Switch_Times.LIMIT)
                    {
                        AddChangeCount();
                        varSwich.AddFamilyChangeCount();

                    }
                    if(coolTimeCheck == Default_Status.ON)
                    {
                        SetCoolTime();
                        varSwich.SetFamilyCoolTime();
                    }
                }
                break;
            case Switch_Rule.OffToOn:
                if (!varSwich.switchStatus)
                {
                    varSwich.switchStatus = true;
                    // 切り替え限界が存在すれば、切り替え回数を加算
                    if (switchTimes == Switch_Times.LIMIT)
                    {
                        AddChangeCount();
                        varSwich.AddFamilyChangeCount();

                    }
                    if (coolTimeCheck == Default_Status.ON)
                    {
                        SetCoolTime();
                        varSwich.SetFamilyCoolTime();
                    }
                }
                break;
            case Switch_Rule.none:
                // switchStatusを逆転させる
                if (varSwich.switchStatus)
                {
                    varSwich.switchStatus = false;
                    
                }
                else
                {
                    varSwich.switchStatus = true;
                }
                // 切り替え限界が存在すれば、切り替え回数を加算
                if (switchTimes == Switch_Times.LIMIT)
                {
                    AddChangeCount();
                    varSwich.AddFamilyChangeCount();

                }
                if (coolTimeCheck == Default_Status.ON)
                {
                    SetCoolTime();
                    varSwich.SetFamilyCoolTime();
                }

                break;
        }        
        
    }
    public void SetCoolTime()
    {
        nowCool = true;
    }
    public void AddChangeCount()
    {
        nowTime++;
    }
    
}
