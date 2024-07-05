using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyEffectManager : MonoBehaviour
{
    [Header("チャージ行動をするときに発生させるエフェクト")]
    [SerializeField] VisualEffect Charge;
    [Header("ダッシュ時などに使うエフェクト")]
    [SerializeField] VisualEffect Dash;
    [Header("ガード用？エフェクト")]
    [SerializeField] VisualEffect Guard;
    [Header("ボスジャンプエフェクト")]
    [SerializeField] VisualEffect Jump;
    //開始時の関数には引数が二つ必要
    //引数１：プレイヤーの座標(目標)
    //引数２：敵自身の座標（開始位置）
    [Header("魔法の弾エフェクト")]
    [SerializeField] VisualEffect Bullet;
    //Stop関数の使用必須？
    [Header("敵モブ出現エフェクト_1")]
    [SerializeField] VisualEffect Summon_1;
    [Header("敵モブ出現エフェクト_2")]
    [SerializeField] VisualEffect Summon_2;
    [Header("キノコ爆発")]
    [SerializeField] VisualEffect Explosion;
    [Header("瞬間移動エフェクト")]
    [SerializeField] VisualEffect TelePort;
    [Header("透明化解除エフェクト")]
    [SerializeField] VisualEffect RemoveInvisible;


    GameObject playerObj;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //デバック用
        if(Input.GetKeyDown(KeyCode.Space))
        {

        }
    }
    public void PlayCharge()
    {
        Charge.SendEvent("Start");
    }
    public void StopCharge()
    {
        Charge.SendEvent("Stop");
    }
    public void PlayDash()
    {
        Dash.SendEvent("Start");
    }
    public void StopDash()
    {
        Dash.SendEvent("Stop");
    }
    public void StartGuard()
    {
        Guard.SendEvent("Start");
    }
    public void StopGuard()
    {
        Guard.SendEvent("Stop");
    }
    public void StartJump()
    {
        Jump.SendEvent("Start");
    }
    public void StopJump()
    {
        Jump.SendEvent("Stop");
    }

    //引数として発射時のターゲット（プレイヤー）の位置と発射開始（ボス）の位置を渡す
    public void StartBullet(Vector3 TargetPos,Vector3 StartPos)
    {
        
        Bullet.SetVector3("TargetPos",TargetPos);
        Bullet.SetVector3("StartPos",StartPos);
        Bullet.SendEvent("Start");
    }
    public void StopBullet()
    {
        Bullet.SendEvent("Stop");
    }
    public void StartSummon()
    {

        Summon_1.SendEvent("Start");
        Summon_2.SendEvent("Start");
    }
    public void StopSummon()
    {
        Summon_1.SendEvent("Stop");
        Summon_2.SendEvent("Stop");
    }
    public void StartExplosion()
    {
        Explosion.SendEvent("Start");
    }
    public void StopExplosion()
    {
        Explosion.SendEvent("Stop");
    }
    public void StartTeleport()
    {
        TelePort.SendEvent("Start");
    }
    public void StopTelePort()
    {
        TelePort.SendEvent("Stop");
    }
    public void StartRemoveInvisible()
    {
        RemoveInvisible.SendEvent("Start");
    }
    public void StopRemoveInvisible()
    {
        RemoveInvisible.SendEvent("Stop");
    }
    
}
