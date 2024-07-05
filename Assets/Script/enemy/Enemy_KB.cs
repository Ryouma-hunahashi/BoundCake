using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_KB : MonoBehaviour
{
    public bool waveTest;

    // �g�̏��擾
    public float hitWaveSpeed; // �g�̑��x
    public float hitWaveHight; // �g�̍���

    private Rigidbody rb;
    private waveCollition hitWave;
    private Vector3 me_Pos;
    private Vector3 me_Scale;

    

    

    [SerializeField] private bool big_Enemy;
    [SerializeField] private bool fly_Enemy;

    private bool nowFall;

    // �g�̔����̐ݒ� ----- ���l�͍����ق��D��
    [Header("----- ���� -----")]
    public float judgeSpeed;  // ���x
    public float judgeHight;  // ����

    public byte speedLevel;
    public byte hightLevel;

    // �Փˌ�̏���
    public float[] blowOfSpeed = new float[2];      // ��s���x�̐ݒ�
    public float[] blowOfHight = new float[2];      // �㏸���x�̐ݒ�
    public float[] blowOfHightLimit = new float[2]; // �㏸�����̐ݒ�

    // �m�b�N�o�b�N�̕���
    public sbyte knockBackVelocity = 1;
    public float fallPow;

    public bool triggerSty;
    public bool hitPlayerWave;

    // ���C���[�̐ݒ�
    [SerializeField] private LayerMask groundLayer = 1 << 6;

    // ���C�̐ݒ�
    [Header("----- ���C�̐ݒ� -----")]
    [SerializeField] private float rayDistance;

    [SerializeField] private bool onGround;

    [Header("----- ���ɓ�����������\n�g�̐ݒ� -----")]
    [Header("���S����Ƃ��Ɉړ�����Z���W")]
    [SerializeField] private float deadPosZ = -4f;
    [Header("�g�̑��x")]
    [SerializeField] private float waveSpeed = 0.75f;
    [Header("�g�̐U����")]
    [SerializeField] private float waveWidth = 0.225f;
    [Header("�g�̍���")]
    [SerializeField] private float waveHight = 2.0f;
    [Header("�N�̔g��")]
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


    //            // ���x�̔���
    //            if (waveSpeed >= judgeSpeed)
    //            {
    //                speedLevel = 1;

    //            }//----- if_stop -----
    //            else
    //            {
    //                hightLevel = 0;

    //            }//----- else_stop -----

    //            // �����̔���
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
        // ���g�̏����擾����
        rb = GetComponent<Rigidbody>();
        
        
    }

    private void FixedUpdate()
    {
        if(waveTest)
        {
            hitPlayerWave = true;

            // ���x�̔���
            if (waveSpeed >= judgeSpeed)
            {
                speedLevel = 1;

            }//----- if_stop -----
            else
            {
                speedLevel = 0;

            }//----- else_stop -----

            // �����̔���
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
        Debug.Log("�n�ʐڐG");
        if(hightLevel == 1&&(collision.transform.gameObject.layer == 6||collision.transform.gameObject.layer == 8))
        {
            Debug.Log("�n�ʌ��m");
            if(fallPow>0)
            {
                Debug.Log("������ѐ�Ŕg����");
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
    //      �|���ꂽ�Ƃ��̏���
    // ���U���ɓ���������Ɏ��g�����ł���܂ł̏����ł�
    //==================================================
    // �����   2023/05/11
    // �{��
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
            // ���g�𐁂���΂�
            rb.velocity = new Vector3(knockBackVelocity*blowOfSpeed[speedLevel], fallPow, 0);

        }//----- if_stop -----

        // �������łȂ����
        if (!nowFall)
        {
            // �㏸���x������������
            fallPow++;

        }//----- if_stop -----

        // �㏸�ʂ������𒴂����Ƃ�
        // �㏸��̗������̏ꍇ
        if ((blowOfHightLimit[hightLevel] < fallPow) || nowFall)
        {
            // ������Ԃɂ���
            nowFall = true;

            // �������x������������
            fallPow--;

        }//----- if_stop -----

        // �ڒn���Ă����ԂȂ�
        if (onGround)
        {
            hitPlayerWave = false;
            nowFall = false;

        }//----- if_stop -----

    }

    //==============
    // �m�b�N�o�b�N�������g�̃X�N���v�g����o�^
    //==============
    // 2023/9/11
    // ���c
    public void SetHitWave(waveCollition _col)
    {
        hitWave = _col;
    }
}