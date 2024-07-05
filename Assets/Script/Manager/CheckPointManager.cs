using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

//============================
// あさわ
// リスポーンに伴う処理まとめ
//============================



public class CheckPointManager : MonoBehaviour
{
    private Vector3 playerPosition;
    public int respawnCnt = 0;
    const float MAX_IMAGE_SCALE = 25.0f;

    [SerializeField] private float nowImageScale = MAX_IMAGE_SCALE;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Vector3[] enemiesPos;
    [SerializeField] private List<ItemDeta> l_cookies = new List<ItemDeta>();
    [SerializeField] private GameObject boss;
    [SerializeField] private Vector3 bossPosition;

    [SerializeField] private RectTransform image;
    [SerializeField] private Vector3 respawnpoint;
    [SerializeField] static public bool isNowRespawning;

    [SerializeField] private Transform playerTrans;

    // Start is called before the first frame update
    void Start()
    {
        // シーン上の敵を取得
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("このシーンのエネミーの数："+enemies.Length);

        // 敵の初期位置を保存
        enemiesPos = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemiesPos[i] = enemies[i].transform.position;
        }

        // ボスの初期値を保存
        if(boss != null)
        {
            bossPosition = boss.transform.position;
        }

        // シーン上のクッキーを取得
        var cookiesBuf = GameObject.FindGameObjectsWithTag("Coin");
        for(byte i = 0; i < cookiesBuf.Length; i++)
        {
            l_cookies.Add(cookiesBuf[i].GetComponent<ItemDeta>());
        }
        

        Debug.Log("このシーンのクッキーの数：" + l_cookies.Count);

        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        if(playerTrans == null)
        {
            Debug.LogError("プレイヤーが見つかりません");
        }

        

        // isNowRespawning初期化
        isNowRespawning = false;
    }


    //Update is called once per frame
    void Update()
    {
        // 毎ループ暗幕イメージをプレイヤーに吸いつかせる
        InitImage();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(MakeSmallerImageScale());
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(MakeLargerImageScale());
        }
#endif
    }


    //// リスポーン地点保存
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Respawn"))
    //    {
    //        respawnpoint = other.gameObject.GetComponent<RespornPoint>().respornPosition;

    //    }
    //    //if(other.gameObject.tag == "GameOver")
    //    //{
    //    //	Resporn();
    //    //}
    //}


    /// <summary>
    /// リスポーン時のステージ初期化をまとめた関数
    /// </summary>
    public void Reset()
    {
        if(boss != null) 
        {
            ResetBoss();
        }
        ResetCookies();
        ResetEnemies();
    }


    // ボス戦闘時に死んだ時の処理
    //ボスの左壁を上げる
    private void ResetBoss()
    {
        //boss.bossleftcheck = true;
        boss.transform.position = bossPosition;
    }

    // クッキーの初期化
    private void ResetCookies()
    {
        for(int i = 0; i < l_cookies.Count; i++)
        {
            l_cookies[i].gameObject.SetActive(true);
            l_cookies[i].MakeGettableCoin();
        }
    }

    // 敵の初期化
    private void ResetEnemies()
    {
        for(int i = 0; i  < enemies.Length; i++)
        {
            // 位置情報初期化
            enemies[i].transform.position = enemiesPos[i];
        }
    }

    // 暗幕の位置を設定する
    private void InitImage()
    {
        float posZ = image.transform.position.z;
        playerPosition = playerTrans.position;
        playerPosition.z = posZ;
        image.transform.position = playerPosition;
    }

    // 暗幕のスケールを縮小させる
    public IEnumerator MakeSmallerImageScale()
    {
        var wait = new WaitForSeconds(0.01f);
        while (nowImageScale > 0.03)
        {
            yield return wait;
            nowImageScale  = nowImageScale * 15.0f / 16.0f;
            image.localScale = new(nowImageScale, nowImageScale, nowImageScale); ;
        }
    }


    // 暗幕のスケールを拡大させる
    public IEnumerator MakeLargerImageScale()
    {
        var wait = new WaitForSeconds(0.01f);
        while (nowImageScale < MAX_IMAGE_SCALE)
        {
            yield return wait;
            nowImageScale = nowImageScale * 16.0f / 15.0f;
            image.localScale = new(nowImageScale, nowImageScale, nowImageScale); ;
        }
    }

    // リスポーンした回数を返す関数
    public int GetRespawnCnt()
    {
        return respawnCnt;
    }

    // 現在リスポーン中かどうかを返す関数
    public bool GetIsNowRespawning()
    {
        return isNowRespawning;
    }

    // 現在リスポーン中可を設定する関数
    public void SetIsNowRespawning(bool nextState)
    {
        isNowRespawning = nextState;
    }

    // アプリケーション終了時にリストを破棄
    private void OnApplicationQuit()
    {
        enemies.Free();
        enemiesPos.Free();
        l_cookies.Clear();
    }
}
