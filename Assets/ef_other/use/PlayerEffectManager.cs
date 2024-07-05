using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX.Utility;
using UnityEngine.VFX;

public class PlayerEffectManager : MonoBehaviour
{
    [Header("�W�����v�����u�Ԃɔ���������G�t�F�N�g")]
    [SerializeField] VisualEffect JumpWind;
    [Header("�`���[�W�s��������Ƃ��ɔ���������G�t�F�N�g")]
    [SerializeField] VisualEffect Aura;
    // Start is called before the first frame update
    void Start()
    {
        

    }
    // Update is called once per frame
    void Update()
    {
        
    }
    
    //�W�����v�������̎��ɌĂяo��
    public void PlayJumpWind()
    {
        JumpWind.SetFloat("StartTime",Time.time);
        JumpWind.SendEvent("Start");
    }
    //�`���[�W�s���J�n���ɌĂяo��
    public void PlayAura()
    {
        Aura.SendEvent("Start");
    }
    //�`���[�W�s���I�����ɌĂяo��
    public void StopAura()
    {
        Aura.SendEvent("Stop");
    }
}
