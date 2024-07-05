using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public struct RotateSetting
{
    public GameObject rotateObj;
    public ObjectRotation rotationScript;
    public float rotateAngle;
    public float rotateSpeed;
}

public class TagSwitcher : MonoBehaviour
{
    enum RotationSet
    {
        ON,
        OFF,
    }


    [Header("このオブジェが判別するスイッチのオブジェクト")]
    [SerializeField] private GameObject switchObj;
    private VariousSwitches_2 switchScript; // スイッチのスクリプト
    private GameObject targetObj;   // タグを変更するオブジェクト
    [Header("スイッチがオンの時設定したいタグ名")]
    [SerializeField] private string tagName_On;
    [Header("スイッチがオフの時設定したいタグ名")]
    [SerializeField] private string tagName_Off;

    [Space(5),Header("タグを変更した際にオブジェクトを回転")]
    [SerializeField] private RotationSet rotationSet = RotationSet.ON;
    
    
    [Header("ONの場合設定")]
    [SerializeField] private List<RotateSetting> l_rotateSettings = new List<RotateSetting>();
    
    


    private bool switchLog;
    // Start is called before the first frame update
    void Start()
    {
        // タグを変更するオブジェクトをこのオブジェに
        targetObj = gameObject;
        // スイッチのスクリプトを取得
        switchScript = switchObj.GetComponent<VariousSwitches_2>();
        // スイッチのログを取得
        switchLog = switchScript.switchStatus;

        // 何も設定されていなければUnityの何もタグが付いていない状態にする。
        if(tagName_On== "")
        {
            tagName_On = "Untagged";
        }
        if (tagName_Off == "")
        {
            tagName_Off = "Untagged";
        }
        // 初期のスイッチ状態に応じてタグを変更
        if (switchLog)
        {
            
            targetObj.tag = tagName_On;
        }
        else
        {
            targetObj.tag = tagName_Off;
        }

        if(rotationSet == RotationSet.ON)
        {
            for(byte i = 0; i < l_rotateSettings.Count; i++)
            {
                if(l_rotateSettings[i].rotateObj != null)
                {
                    RotateSetting set = l_rotateSettings[i];
                    set.rotationScript = l_rotateSettings[i].rotateObj.GetComponent<ObjectRotation>();
                    l_rotateSettings[i] = set;
                    
                }
                else
                {
                    Debug.LogError("オブジェクトの設定をしてください");
                }
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ログと現在のスイッチ状態が異なる場合
        if (switchLog != switchScript.switchStatus)
        {
            // タグを変更する
            if (switchScript.switchStatus)
            {
                targetObj.tag = tagName_On;
                
            }
            else
            {
                targetObj.tag = tagName_Off;
            }

            if (rotationSet == RotationSet.ON)
            {
                for (byte i = 0; i < l_rotateSettings.Count; i++)
                {
                    l_rotateSettings[i].rotationScript.RotateStart(l_rotateSettings[i].rotateAngle, l_rotateSettings[i].rotateSpeed);
                }
            }

            // ログを更新
            switchLog = switchScript.switchStatus;
        }
    }

    private void OnApplicationQuit()
    {
        l_rotateSettings.Clear();
    }
}
