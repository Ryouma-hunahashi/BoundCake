using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          爆弾を召喚するスクリプト
// ※召喚地点の情報をもとにそこへ物体を召喚する
// ※[Setup_BombPointer]の付いているオブジェクトが必要
//==================================================
// 制作日2023/05/26    更新日2023/05/28
// 宮﨑
public class Boss3_Bomb : MonoBehaviour
{
    [SerializeField] private bool testSpawn;

    // 指令の情報
    private GameObject bombOperator;

    // 親の情報
    private GameObject parObj;

    // 爆弾のプレハブを格納
    public GameObject bombObj;
    [SerializeField] private byte bombCount = 4;
    private List<GameObject> l_bombPool = new List<GameObject>();
    private List<Boss3_Bomb_Action> l_bombScripts = new List<Boss3_Bomb_Action>();

    // 挙動情報の取得
    private Boss3_Main parMain;

    private void Start()
    {
        // 爆弾が格納されていないなら
        if(bombObj == null)
        {
            // 警告だけ表示する
            Debug.LogWarning(this.transform.name + "の爆弾が格納されていませんっ！");

        }//----- if_stop -----
        for(byte i = 0; i < bombCount; i++)
        {
            l_bombPool.Add(Instantiate(bombObj, new Vector3(20, 0, 50), Quaternion.identity));
            l_bombScripts.Add(l_bombPool[i].GetComponent<Boss3_Bomb_Action>());
        }

        // 指令の情報を取得
        bombOperator = GameObject.Find("BombPointOperator").gameObject;

        // 親情報の取得
        parObj = transform.parent.gameObject;

        // 挙動情報の取得
        parMain = parObj.GetComponent<Boss3_Main>();
    }

    private void FixedUpdate()
    {
        // 爆弾が格納されていないなら処理を飛ばす
        if (bombObj == null) return;

        if(testSpawn)
        {
            StartCoroutine(SpawnBomb());
            testSpawn = false;
        }
    }

    //==================================================
    //          召喚地点に各オブジェクトを召喚する
    // ※召喚地点の情報をもとにそこへ物体を召喚する
    // 安全地帯、ボス本体以外には爆弾を配置します
    //==================================================
    // 制作日2023/05/26    更新日2023/05/28
    // 宮﨑
    public IEnumerator SpawnBomb()
    {
        // 指令の子オブジェクトの数を取得する
        int opChildCnt = bombOperator.transform.childCount;

        // ボスの出現地点を設定
        int randomBossPos = Random.Range(0,opChildCnt);

        // プレイヤーが乗れる地点を設定
        int randomSafePos = Random.Range(0, opChildCnt);

        // 乗れる床がボス出現地点と同じなら再抽選を行う
        while(randomBossPos == randomSafePos)
        {
            randomSafePos = Random.Range(0, opChildCnt);
        }//----- if_stop -----

        // 親が子オブジェクトになっていたなら解除する
        if (parObj.transform.parent != null) parObj.transform.parent = null;

        // 自身の親オブジェクトを床の子オブジェクトに指定
        parObj.transform.position = Vector3.zero+new Vector3(0,1,0);
        parObj.transform.SetParent(GameObject.Find("BombPoint" + randomBossPos).transform,false);

        // 本体出現時のアニメーションはここ...? ----------
        parMain.anim.SetBool("Grawing",true);
        parMain.audio.bossSource.Stop();
        parMain.audio.bossSource.loop = false;
        parMain.audio.Boss3_SpawnSound();

        // 指令の子オブジェクトの数繰り返す
        for (int i = 0; i < opChildCnt; i++)
        {
            // 指定の子オブジェクトを格納
            GameObject thisObj = GameObject.Find("BombPoint" + i).gameObject;
            
            Vector3 thisObj_Pos = thisObj.transform.position;

            // 格納したオブジェクトが親オブジェクトなら処理を抜ける
            if (thisObj == parObj) yield break;

            // 召喚地点にボスの出現する予定なら処理を飛ばす
            if (i == randomBossPos) continue;

            // 召喚地点に安全地帯が出現する予定なら処理を飛ばす
            if (i == randomSafePos) continue;

            Debug.Log("BombPoint" + i);
            Debug.Log("randomBossPos="+randomBossPos);
            Debug.Log("randomSafePos=" + randomSafePos);

            for (byte j = 0; j < bombCount; j++)
            {
                if (l_bombPool[j].transform.position.z != thisObj_Pos.z+1)
                {
                    // 召喚した爆弾を床の子オブジェクトに変更する
                    l_bombPool[j].transform.SetParent(thisObj.transform, false);
                    l_bombPool[j].transform.position = new Vector3(thisObj_Pos.x, thisObj_Pos.y+0.1f, thisObj_Pos.z + 1);
                    StartCoroutine(l_bombScripts[j].BombCountDown());
                    l_bombScripts[j].bombAnim.Play("BombGrawUp");
                    //for (byte k = 0; k < 3; k++)
                    //{
                    //    yield return null;
                    //}
                    break;
                }
                else if (j == bombCount - 1)
                {
                    l_bombPool.Add(Instantiate(bombObj, new Vector3(20, 0, 50), Quaternion.identity));
                    l_bombScripts.Add(l_bombPool[j].GetComponent<Boss3_Bomb_Action>());
                    bombCount++;
                }
            }
            //// 爆弾を出現させる
            //GameObject spawnObj = Instantiate(bombObj);
            //spawnObj.GetComponent<Boss3_Bomb_Action>().bossMain = parMain;
            
        }
        
    }
    private void OnApplicationQuit()
    {
        l_bombPool.Clear();
        l_bombScripts.Clear();
    }
}