using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class OtherEffectManager : MonoBehaviour
{
    [Header("�p�t�F�������̉������G�t�F�N�g")]
    [SerializeField]VisualEffect ef_pafe;
    [Header("�S�[�����H�̃N�b�L�[���U��΂�G�t�F�N�g")]
    [SerializeField]VisualEffect ef_cooky;
    [Header ("�p�t�F�������̃L���L��")]
    [SerializeField]VisualEffect ef_flash;
    //[SerializeField]VisualEffect ef_1;
    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    public void StartPafe()
    {
        ef_pafe.SendEvent("Start");
    }
    public void StopPafe()
    {
        ef_pafe.SendEvent("Stop");
    }
    public void StartCooky()
    {
        ef_cooky.SendEvent("Start");
    }
    public void StopCooky()
    {
        ef_cooky.SendEvent("Stop");
    }
    public void StartFlash()
    {
        ef_flash.SendEvent("Start");
    }
    public void StopFlash()
    {
        ef_flash.SendEvent("Stop");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
