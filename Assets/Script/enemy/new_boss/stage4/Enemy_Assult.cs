using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          敵モブの直進攻撃
//==================================================
// 作成日2023/05/17    更新日2023/05/19
// 宮﨑
public class Enemy_Assult : MonoBehaviour
{
    // 親の情報を取得
    private Rigidbody par_Rb;
    private EnemyEffectManager ef_Manager;

    // 標的を設定
    public GameObject target;
    private bool setTarget;     // 標的の有無

    // どう破壊するか
    [SerializeField] private bool hitDestroy;       // 何かに当たったとき
    [SerializeField] private bool finishDestroy;    // フレーム終了時

    [SerializeField] private bool startAction;

    // 直進の設定
    [Header("----- 直進の設定 -----")]
    [SerializeField] private float speed;           // 速度
    [SerializeField] private ushort distanceFrame;  // 距離(指定フレーム間)
    [SerializeField] private bool speedup;          // 加速の有無
    [SerializeField] private float upperLimit;      // 加速の上限
    private float addSpeed = 0;                     // 加速度

    //private void OnTriggerEnter(Collider other)
    //{
    //    // 何かに触れた時に破壊するなら
    //    if (hitDestroy)
    //    {
    //        // 自身を破壊する
    //        Destroy(this.transform.parent.gameObject);

    //    }//----- if_stop -----
    //}

    private void Start()
    {
        // 親の情報を取得
        target = GameObject.Find("player");
        par_Rb = transform.parent.GetComponent<Rigidbody>();
        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        // 標的が設定されているなら
        if(target != null)
        {
            // 標的指定状態にする
            setTarget = true;

        }//----- if_stop -----
        else
        {
            // 標的未定状態にする
            setTarget = false;

        }//----- else_stop -----
    }

    private void Update()
    {
        if(startAction)
        {
            StartCoroutine(Assult());
            startAction = false;

        }//----- if_stop -----
    }

    private IEnumerator Assult()
    {
        // 標的が決まっているなら向きを設定する
        if (setTarget)
        {
            transform.parent.LookAt(target.transform.position);
            ef_Manager.StartBullet(target.transform.position,transform.position);
        }


        for(int i = 0; i < distanceFrame; i++)
        {
            // １フレーム遅延させる
            yield return null;

            // 加速するなら
            if(speedup)
            {
                // 指定の上限値まで加速
                if (addSpeed < upperLimit)
                {
                    addSpeed++;

                }//----- if_stop -----

            }//----- if_stop -----

            // 移動速度の更新
            par_Rb.velocity = new Vector3(transform.parent.forward.x * (speed + addSpeed), transform.parent.forward.y * (speed + addSpeed), 0);

        }//----- for_stop -----

        // 速度を無くす
        par_Rb.velocity = Vector3.zero;

        if(finishDestroy)
        {
            // 自身を破壊する
            Destroy(this.transform.parent.gameObject);

        }//----- if_stop -----
    }
}