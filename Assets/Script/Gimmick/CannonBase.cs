using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CannonBase : MonoBehaviour
{
    // Start is called before the first frame update

    public enum VECTOR
    { 
        RIGHT,
        LEFT,
        UP,
        DOWN,
    }
    
    [System.Serializable] public struct BulletData
    {
        public GameObject obj;
        public CannonBullet sqript;
    }

    private Dictionary<VECTOR, float> vec2Rad = new Dictionary<VECTOR, float>(); 
    
    



    [Header("-----弾のゲームオブジェクト-----\n")]
    [SerializeField] private GameObject bulletObj;
    // オブジェクトプール保存用
    [SerializeField]private List<BulletData> l_bulletPool = new List<BulletData>();
    // 弾のオブジェクトプールの保管場所
    private Vector3 stayPos = new Vector3(20, 0, -50);
    [Header("プール内の初期オブジェクト数")]
    [SerializeField] private byte defPoolCnt = 2;
    private byte nowPoolCnt = 0;

    
    [Header("\n-----弾の発射設定-----\n")]
    [Header("発射のディレイタイム(秒)")]
    [SerializeField] private float shotDelay = 5.0f;
    [Header("弾の速度")]
    [SerializeField] private float speed = 5.0f;
    // 弾の発射ベクトル
    private Vector3 shotVec;
    [Header("弾の発射方向")]
    [SerializeField] private VECTOR vec = VECTOR.RIGHT;
    [Header("弾が自然消滅するまでの時間(秒)")]
    [SerializeField] private float destroyTIme = 8.0f;
    [Header("-----弾を発射する位置の補正値-----\n")]
    [Header("発射方向にこの値分の補正を掛けます")]
    [SerializeField] private float shotShiftVal = 0.0f;
    // 実際の補正値
    private Vector3 shiftVec;

    // 内部処理用変数
    private float nowDelayTime = 0.0f;
    private Vector3 pos;

    // カメラに写っているか
    private bool cameraVisibling = false;

    void Start()
    {
        if(bulletObj == null)
        {
            Debug.LogError("弾のプレハブを登録してください");
        }
        // このポジションを保存
        pos = this.transform.position;

        // 各方向に対応した角度をラジアン角で登録
        vec2Rad.Add(VECTOR.RIGHT, Mathf.Deg2Rad * 0.0f);
        vec2Rad.Add(VECTOR.LEFT, Mathf.Deg2Rad * 180.0f);
        vec2Rad.Add(VECTOR.UP, Mathf.Deg2Rad * 90.0f);
        vec2Rad.Add(VECTOR.DOWN, Mathf.Deg2Rad * 270.0f);
        
        // 発射位置の補正値を計算
        SetShiftValue();

        // オブジェクトプールの登録
        for(byte i = 0;i<defPoolCnt;i++)
        {
            l_bulletPool.Add(InitNewBulletPool());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // カメラに写っていないとき処理を止める。
        if(!cameraVisibling)
        {
            return;
        }
        nowDelayTime += Time.deltaTime;
        if(nowDelayTime >= shotDelay)
        {
            GetBulletPool().sqript.Shot(speed, shiftVec + pos, shotVec, destroyTIme,vec);
            nowDelayTime = 0.0f;
        }
    }


    private BulletData InitNewBulletPool()
    {
        BulletData newData;

        newData.obj = Instantiate(bulletObj, stayPos, Quaternion.identity);
        newData.sqript = newData.obj.GetComponent<CannonBullet>();
        if(newData.sqript == null)
        {
            Debug.LogError("バレットスクリプトの取得に失敗");
        }
        nowPoolCnt++;

        return newData;
    }

    private BulletData GetBulletPool()
    {
        byte i;
        for (i = 0;i<nowPoolCnt;i++)
        {
            if(l_bulletPool[i].obj.activeSelf == false)
            {
                l_bulletPool[i].obj.SetActive(true);
                l_bulletPool[i].sqript.End();
            }
            if(l_bulletPool[i].sqript.CheckState(CannonBullet.STATE.STAY))
            {
                break;
            }
            else if(i==nowPoolCnt-1)
            {
                l_bulletPool.Add(InitNewBulletPool());
            }
        }
        return l_bulletPool[i];
    }

    private void SetShiftValue()
    {
        var vec3 = Vector3.zero;
        vec3.x = Mathf.Cos(vec2Rad[vec]);
        vec3.y = Mathf.Sin(vec2Rad[vec]);
        vec3.z = 0.0f;
        shotVec = vec3;
        shiftVec = vec3*shotShiftVal;
        
    }

    public void SetShotVec(VECTOR _vec)
    {
        vec = _vec;
        SetShiftValue();
    }

    private void OnBecameVisible()
    {
        cameraVisibling = true;
    }
    private void OnBecameInvisible()
    {
        cameraVisibling = false;
        nowDelayTime = 0.0f;
    }

    private void OnApplicationQuit()
    {
        vec2Rad.Clear();
        l_bulletPool.Clear();
    }
}
