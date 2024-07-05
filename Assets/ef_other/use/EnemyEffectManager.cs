using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class EnemyEffectManager : MonoBehaviour
{
    [Header("�`���[�W�s��������Ƃ��ɔ���������G�t�F�N�g")]
    [SerializeField] VisualEffect Charge;
    [Header("�_�b�V�����ȂǂɎg���G�t�F�N�g")]
    [SerializeField] VisualEffect Dash;
    [Header("�K�[�h�p�H�G�t�F�N�g")]
    [SerializeField] VisualEffect Guard;
    [Header("�{�X�W�����v�G�t�F�N�g")]
    [SerializeField] VisualEffect Jump;
    //�J�n���̊֐��ɂ͈�������K�v
    //�����P�F�v���C���[�̍��W(�ڕW)
    //�����Q�F�G���g�̍��W�i�J�n�ʒu�j
    [Header("���@�̒e�G�t�F�N�g")]
    [SerializeField] VisualEffect Bullet;
    //Stop�֐��̎g�p�K�{�H
    [Header("�G���u�o���G�t�F�N�g_1")]
    [SerializeField] VisualEffect Summon_1;
    [Header("�G���u�o���G�t�F�N�g_2")]
    [SerializeField] VisualEffect Summon_2;
    [Header("�L�m�R����")]
    [SerializeField] VisualEffect Explosion;
    [Header("�u�Ԉړ��G�t�F�N�g")]
    [SerializeField] VisualEffect TelePort;
    [Header("�����������G�t�F�N�g")]
    [SerializeField] VisualEffect RemoveInvisible;


    GameObject playerObj;
    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //�f�o�b�N�p
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

    //�����Ƃ��Ĕ��ˎ��̃^�[�Q�b�g�i�v���C���[�j�̈ʒu�Ɣ��ˊJ�n�i�{�X�j�̈ʒu��n��
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
