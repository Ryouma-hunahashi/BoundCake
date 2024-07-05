using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// 回転床のスクリプト。中心点に

public class Rotation : MonoBehaviour
{
    public float angularVelocity = 1;

    private List<GameObject> l_childrenObj = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // 一度リストをクリア
        l_childrenObj.Clear();

        // 子オブジェを全て格納
        for (byte i = 0; i < transform.childCount; i++)
        {
            l_childrenObj.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // このオブジェを回転させる。
        transform.eulerAngles += new Vector3(0,0,angularVelocity);
        
        // 子オブジェを逆方向に回転させる。
        for(byte i = 0; i < l_childrenObj.Count; i++)
        {
            l_childrenObj[i].transform.eulerAngles -= new Vector3(0,0,angularVelocity);
        }
    }

    // ゲーム終了時にリストを破棄
    private void OnApplicationQuit()
    {
        l_childrenObj.Clear();
    }
}
