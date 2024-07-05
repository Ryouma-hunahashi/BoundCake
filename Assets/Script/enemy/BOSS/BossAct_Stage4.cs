using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAct_Stage4 : MonoBehaviour
{
    public bool testAct;
    // 自身の情報を取得

    // 本体出現ポイントの設定
    public List<Vector3> spawnPointPosition = new List<Vector3>();

    private byte children;

    private void Start()
    {
        children = (byte)this.transform.childCount;

        for(int i = 0; i < children; i++)
        {
            spawnPointPosition.Add(transform.GetChild(i).position);

        }//----- for_stop -----
    }

    private void FixedUpdate()
    {
        if(testAct)
        {
            this.transform.position = spawnPointPosition[Random.Range(0,children)];

            testAct = false;
        }
    }
}