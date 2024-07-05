using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX.Utility;
using UnityEngine.VFX;

public class PlayerEffectManager : MonoBehaviour
{
    [Header("ジャンプした瞬間に発生させるエフェクト")]
    [SerializeField] VisualEffect JumpWind;
    [Header("チャージ行動をするときに発生させるエフェクト")]
    [SerializeField] VisualEffect Aura;
    // Start is called before the first frame update
    void Start()
    {
        

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    //ジャンプ処理等の時に呼び出す
    public void PlayJumpWind()
    {
        JumpWind.SetFloat("StartTime",Time.time);
        JumpWind.SendEvent("Start");
    }
    //チャージ行動開始時に呼び出す
    public void PlayAura()
    {
        Aura.SendEvent("Start");
    }
    //チャージ行動終了時に呼び出す
    public void StopAura()
    {
        Aura.SendEvent("Stop");
    }
}
