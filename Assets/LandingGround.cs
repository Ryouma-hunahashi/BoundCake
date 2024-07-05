using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LandingGround : MonoBehaviour
{
    //自身のRigidbody
    Rigidbody rb;
    //振動するのにかかる時間
    //[SerializeField] float landTime = 0.5f;
    [SerializeField] Vector3 landingSpeed = new Vector3(0, 2, 0);
    //振動の影響が及ぶ範囲
    //[SerializeField] float landRange = 2f;

    //ゲーム開始時の糸オブジェクトの座標
     Vector3 startPos;
    //ゲーム開始時の糸の高さ
    private float startPosY;
    //振動発生時に糸がどこまで沈むか
    [Header("糸が沈む量")]
    [SerializeField] float targetPosY=1;
    [Header("糸が沈むスピード(フレーム)")]
    [Tooltip("往復の速度")]
    [SerializeField] float landSpeed = 15.0f;
    private float defaultLandSpeed;
    //糸が沈む限界の座標・
    Vector3 targetPos;
    //糸が上昇する際に
    [SerializeField] float upRate = 1;
    [SerializeField] string EnemyTag="Enemy";
    //地面と接触した敵オブジェクトを格納する用のリスト
    //public List<GameObject> EnemyObjList = new List<GameObject>();
    public bool moveFlg = false;

    //=====================================
    private bool AlphaFlg = false;
    //=====================================

    private float landAccel;
    private float landVelocity = 0;

    public bool nowCharge = false;
    private float chargeAccel = 0;
    private float chargeVelocity = 0;

    private const float LandingIndex = Mathf.PI / 2;
    //子オブジェクトのVFXコンポーネント
    private VisualEffect ChildrenEffect;
    private vfxManager vfxManager;

    private bool vfxShiftFg = false;
    private float shiftScale;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ChildrenEffect = GetComponentInChildren<VisualEffect>();
        vfxManager = GetComponentInChildren<vfxManager>();
        if(ChildrenEffect == null||vfxManager==null)
        {
            Debug.LogError("VFXがねえ！");
        }
        startPos =transform.position;
        startPosY=transform.position.y;
        targetPos=new Vector3(startPos.x, startPosY-targetPosY, startPos.z);

        landAccel = LandingIndex / landSpeed;

        //=====================================
        AlphaFlg = ChildrenEffect.GetBool("AlphaFlg");
        if(GetComponent<alphaZeroSpring>() != null)
        {
            AlphaFlg = false;
        }
        //=====================================

        defaultLandSpeed = landSpeed;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    private void FixedUpdate()
    {
        if (gameObject.CompareTag("Floor"))
        {
            startPos = transform.parent.position;
            startPosY = startPos.y;
            targetPos = new Vector3(startPos.x, startPosY - targetPosY, startPos.z);
        }
        else if (transform.parent != null && transform.parent.CompareTag("Floor"))
        {
            startPos = transform.parent.position;
            startPosY = startPos.y;
            targetPos = new Vector3(startPos.x, startPosY - targetPosY, startPos.z);
        }
            // 動く許可が出ていれば

            if (nowCharge)
            {
                if (landVelocity <= LandingIndex)
                {
                    var pos = transform.position;
                    landVelocity += landAccel;
                    shiftScale = targetPosY * (Mathf.Sin(landVelocity));
                    pos.y = startPosY - shiftScale;
                    if (pos.y <= transform.position.y)
                    {
                        transform.position = pos;
                        if (vfxShiftFg)
                        {
                            vfxManager.chargeShift = shiftScale;
                        }
                    }
                }
            }
            else if (moveFlg)
            {
                var pos = transform.position;
                landVelocity += landAccel;
                pos.y = startPosY - targetPosY * (Mathf.Cos(landVelocity));
                transform.position = pos;
                //rb.AddForce(landingSpeed*upRate);
                //if (transform.position.y > startPosY)
                //{
                //    moveFlg = 0;
                //    rb.velocity = Vector3.zero;
                //    transform.position = startPos;

                //}
                if (landVelocity > LandingIndex)
                {
                    pos.y = startPosY;
                    transform.position = pos;
                    landVelocity = 0;
                    moveFlg = false;
                }
            }
            else
            {

                landVelocity = 0;
                var pos = transform.position;
                pos.y = startPosY;
                transform.position = pos;
                vfxManager.chargeShift = 0;
                shiftScale = 0;
            }
        
        


#if UNITY_EDITOR
        if(defaultLandSpeed!=landSpeed)
        {
            defaultLandSpeed = landSpeed;
            landAccel = LandingIndex / landSpeed;
        }
#endif
        //else if(moveFlg < 0)
        //{
        //    //rb.AddForce(-landingSpeed);

        //}

        //地面が沈む際の上限・下限の設定
        //if (transform.position.y > startPosY)
        //{
        //    moveFlg = 0;
        //    rb.velocity = Vector3.zero;
        //    transform.position = startPos;

        //}
        //if (transform.position.y < targetPos.y)
        //{
        //    rb.velocity = Vector3.zero;
        //    transform.position = targetPos;
        //    moveFlg = 1;

        //}

    }

    //糸を揺らし始める処理
    public void lnadGround(float _landDis,float _landSpeed)
    {
        //波の発生地点から一定範囲内に敵が存在しているかをリストで確認する
        //foreach(GameObject obj in EnemyObjList)
        //{
        //    //存在していればY座標の固定を解除する
        //    if (Mathf.Abs(obj.transform.position.x-pointX) < landRange)
        //    {
        //        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        //    }
        //}

        // 動いている途中でなければ
        if (!moveFlg)
        {
            // スピードを更新
            landSpeed = _landSpeed;
            // 下げる距離を更新
            targetPosY = _landDis;
            // ターゲットの最終決定
            targetPos = new Vector3(transform.position.x, startPosY - targetPosY, transform.position.z);
            // 下がる角加速度を更新
            landAccel = LandingIndex / landSpeed;
            // ポジションを目標地点に変更
            transform.position = targetPos;
            // 動かす
            moveFlg = true;
            //landingtimeプロパティに開始時間を送信する
            ChildrenEffect.SetFloat("LandingTime", Time.time);

            //=====================================
            if (AlphaFlg == true)
            {
                ChildrenEffect.SendEvent("Start");
            }
            //=====================================
            //rb.velocity = -landingSpeed;
        }
        //rb.velocity=landingSpeed*upRate;

    }

    public void ChargeLand(float _maxLandDis, float _landSpeed,bool _shiftFg = false)
    {
        if (!nowCharge)
        {

            landSpeed = _landSpeed;
            landAccel = LandingIndex / landSpeed;
            targetPosY = _maxLandDis;
            targetPos = new Vector3(transform.position.x, startPosY - targetPosY, transform.position.z);
            nowCharge = true;
            vfxShiftFg = _shiftFg;
        }

        //// 角加速度を計算
        //// 0～1の間で遷移させたいので90°をチャージの限界数分で割る事で算出
        //chargeAccel = LandingIndex / _chargeLimit;
        //// 現在のチャージ分角加速度に乗算
        //chargeVelocity = chargeAccel*_chargePoint;
        //// ポジションを一時格納
        //var pos = transform.position;
        //// 初期位置から現在のチャージ値で算出した現在角度をsinに放り込み下げる。
        //pos.y = startPosY - _maxLandDis * (Mathf.Sin(chargeVelocity));
        //// ポジションを更新
        //transform.position = pos;
        
        //// 地面をチャージ中にする。
        //nowCharge = true;
        //// 限界値にチャージ度が到達していなければ
        //if (_chargeLimit != _chargePoint)
        //{
        //    // 一度で下がる距離の近似値を返す。(厳密には一緒でないため)
        //    return _maxLandDis * (Mathf.Sin(chargeAccel));
        //}
        //// 到達していれば
        //else
        //{
        //    // 0を返し、それ以上の減少をなくす。
        //    return 0;
        //}

        
    }

    public IEnumerator ChargeEnd()
    {
        if(nowCharge)
        {
            for (byte i = 0; i < 3; i++)
            {
                //if(nowCharge)
                //{
                //    yield break;
                //}
                yield return null;
            }
            nowCharge = false;
        }
    }

    

    //敵が地面に最初に触れたときにY座標を固定させる
    //private void OnCollisionEnter(Collision collision)
    //{
        ////衝突したオブジェクトのタグが敵
        //if(collision.transform.CompareTag(EnemyTag))
        //{
        //    //衝突したオブジェクトの位置を調整する

        //    //衝突したオブジェクトのRigidbodyを取得する
        //    rb = collision.rigidbody;
        //    //オブジェクトの全回転・Y座標・Z座標を固定する
        //    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;


        //}
        
        //if(collision.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log("沈め！");
        //    lnadGround(collision.contacts[0].point.x);
        //}
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if(collision.gameObject.CompareTag("Player"))
    //    {
    //        moveFlg = -1;
    //        rb.velocity = -landingSpeed;
    //    }
    //}
    
}
