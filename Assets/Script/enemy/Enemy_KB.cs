using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_KB : MonoBehaviour
{
    public bool waveTest;

    // 波の情報取得
    public float hitWaveSpeed; // 波の速度
    public float hitWaveHight; // 波の高さ

    private Rigidbody rb;
    private waveCollition hitWave;
    private Vector3 me_Pos;
    private Vector3 me_Scale;

    

    

    [SerializeField] private bool big_Enemy;
    [SerializeField] private bool fly_Enemy;

    private bool nowFall;

    // 波の判定基準の設定 ----- 同値は高いほう優先
    [Header("----- 判定基準 -----")]
    public float judgeSpeed;  // 速度
    public float judgeHight;  // 高さ

    public byte speedLevel;
    public byte hightLevel;

    // 衝突後の処理
    public float[] blowOfSpeed = new float[2];      // 飛行速度の設定
    public float[] blowOfHight = new float[2];      // 上昇高度の設定
    public float[] blowOfHightLimit = new float[2]; // 上昇制限の設定

    // ノックバックの方向
    public sbyte knockBackVelocity = 1;
    public float fallPow;

    public bool triggerSty;
    public bool hitPlayerWave;

    // レイヤーの設定
    [SerializeField] private LayerMask groundLayer = 1 << 6;

    // レイの設定
    [Header("----- レイの設定 -----")]
    [SerializeField] private float rayDistance;

    [SerializeField] private bool onGround;

    [Header("----- 糸に当たった時の\n波の設定 -----")]
    [Header("死亡するときに移動するZ座標")]
    [SerializeField] private float deadPosZ = -4f;
    [Header("波の速度")]
    [SerializeField] private float waveSpeed = 0.75f;
    [Header("波の振動数")]
    [SerializeField] private float waveWidth = 0.225f;
    [Header("波の高さ")]
    [SerializeField] private float waveHight = 2.0f;
    [Header("誰の波か")]
    [SerializeField] private waveCollition.WAVE_TYPE waveType = waveCollition.WAVE_TYPE.PLAYER_ENEMY;
    //private void OnTriggerStay(Collider other)
    //{
    //    if(other.CompareTag("Wave") && !hitPlayerWave)
    //    {
    //        var waveScript = other.gameObject.GetComponent<waveCollition>();
    //        if (waveScript.waveType == waveCollition.WAVE_TYPE.PLAYER&&
    //            (waveScript.waveVelocity == waveCollition.WAVE_VELOCITY.RIGHT||
    //            waveScript.waveVelocity == waveCollition.WAVE_VELOCITY.LEFT))
    //        {
    //            hitPlayerWave = true;
    //            waveSpeed = waveScript.vfxManager.waveSpeedArray[waveScript.waveNum];
    //            waveHight = waveScript.vfxManager.waveHeightArray[waveScript.waveNum];
    //            switch(waveScript.waveVelocity)
    //            {
    //                case waveCollition.WAVE_VELOCITY.RIGHT:
    //                    knockBackVelocity = 1;
    //                    break;
    //                case waveCollition.WAVE_VELOCITY.LEFT:
    //                    knockBackVelocity = -1;
    //                    break;
    //            }


    //            // 速度の判定
    //            if (waveSpeed >= judgeSpeed)
    //            {
    //                speedLevel = 1;

    //            }//----- if_stop -----
    //            else
    //            {
    //                hightLevel = 0;

    //            }//----- else_stop -----

    //            // 高さの判定
    //            if (waveHight >= judgeHight)
    //            {
    //                hightLevel = 1;

    //            }//----- if_stop -----
    //            else
    //            {
    //                hightLevel = 0;

    //            }//----- else_stop -----

    //            fallPow = blowOfHight[hightLevel];

    //            audio.KnockBackSound();
    //            waveTest = false;
    //        }
    //    }
    //}

    private void Start()
    {
        // 自身の情報を取得する
        rb = GetComponent<Rigidbody>();
        
        
    }

    private void FixedUpdate()
    {
        if(waveTest)
        {
            hitPlayerWave = true;

            // 速度の判定
            if (waveSpeed >= judgeSpeed)
            {
                speedLevel = 1;

            }//----- if_stop -----
            else
            {
                speedLevel = 0;

            }//----- else_stop -----

            // 高さの判定
            if (WaveManager.instance.CheckStrongWave(hitWaveHight))
            {
                hightLevel = 1;

            }//----- if_stop -----
            else
            {
                hightLevel = 0;

            }//----- else_stop -----

            fallPow = blowOfHight[hightLevel];

            waveTest = false;
        }

        me_Pos = this.transform.position;
        me_Scale = this.transform.localScale;

        if (!fly_Enemy)
        {
            Ray underRay = new Ray(new Vector3(me_Pos.x, me_Pos.y, me_Pos.z), new Vector3(0.0f, -1.0f, 0.0f));
            if (me_Scale.x > 0)
            {
                underRay = new Ray(new Vector3(me_Pos.x, me_Pos.y, me_Pos.z), new Vector3(0.0f, -1.0f, 0.0f));
            }
            else if (me_Scale.x < 0)
            {
                underRay = new Ray(new Vector3(me_Pos.x, me_Pos.y, me_Pos.z), new Vector3(0.0f, -1.0f, 0.0f));
            }

            RaycastHit underRayHit;
            bool underRayFlag = Physics.Raycast(underRay, out underRayHit, rayDistance, groundLayer);

            if (underRayFlag)
            {
                onGround = true;
                Debug.DrawRay(underRay.origin, underRay.direction * rayDistance, Color.blue, 1, false);
            }
            else
            {
                onGround = false;
                Debug.DrawRay(underRay.origin, underRay.direction * rayDistance, Color.red, 1, false);
            }

            if (onGround)
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - 1, 0);
            }
            else
            {
                rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y - 1, 0);
            }
        }
        if(hitPlayerWave)
        {
            WasKnockedDown();

        }//----- if_stop -----

        if(transform.position.y<-50.0f)
        {
            End();

            gameObject.SetActive(false);
        }
    }

    public void End()
    {
        fallPow = 0;
        hitPlayerWave = false;
        nowFall = false;
        rb.velocity = Vector3.zero;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("地面接触");
        if(hightLevel == 1&&(collision.transform.gameObject.layer == 6||collision.transform.gameObject.layer == 8))
        {
            Debug.Log("地面検知");
            if(fallPow>0)
            {
                Debug.Log("吹き飛び先で波発生");
                rb.velocity = new Vector3(rb.velocity.x,0,0);
                nowFall = true;
                fallPow = 0;
                

                hitWave.GetPool().WaveCreate( waveSpeed, waveWidth, waveHight, waveType, transform.position.x+0.1f, collision.transform);
                hitWave.GetPool().WaveCreate(-waveSpeed, waveWidth, waveHight, waveType, transform.position.x-0.1f, collision.transform);
                
            }
            transform.position = new Vector3(transform.position.x, transform.position.y, deadPosZ);
        }
    }



    //==================================================
    //      倒されたときの処理
    // ※振動に当たった後に自身が消滅するまでの処理です
    //==================================================
    // 制作日   2023/05/11
    // 宮﨑
    private void WasKnockedDown()
    {
        if (big_Enemy && !onGround)
        {
            switch (speedLevel)
            {
                case 0:
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
                    break;
                case 1:
                    rb.velocity = new Vector3(knockBackVelocity*blowOfSpeed[speedLevel], rb.velocity.y, 0);
                    break;
            }

            switch (hightLevel)
            {
                case 0:
                    rb.velocity = new Vector3(rb.velocity.x, rb.velocity.y, 0);
                    break;
                case 1:
                    rb.velocity = new Vector3(rb.velocity.x, fallPow, 0);
                    break;
            }
        }
        else
        {
            // 自身を吹き飛ばす
            rb.velocity = new Vector3(knockBackVelocity*blowOfSpeed[speedLevel], fallPow, 0);

        }//----- if_stop -----

        // 落下中でなければ
        if (!nowFall)
        {
            // 上昇速度を加速させる
            fallPow++;

        }//----- if_stop -----

        // 上昇量が制限を超えたとき
        // 上昇後の落下中の場合
        if ((blowOfHightLimit[hightLevel] < fallPow) || nowFall)
        {
            // 落下状態にする
            nowFall = true;

            // 落下速度を加速させる
            fallPow--;

        }//----- if_stop -----

        // 接地している状態なら
        if (onGround)
        {
            hitPlayerWave = false;
            nowFall = false;

        }//----- if_stop -----

    }

    //==============
    // ノックバックさせた波のスクリプト情報を登録
    //==============
    // 2023/9/11
    // 高田
    public void SetHitWave(waveCollition _col)
    {
        hitWave = _col;
    }
}