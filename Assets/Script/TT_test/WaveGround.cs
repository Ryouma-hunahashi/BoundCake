using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveGround : MonoBehaviour
{
    [Header("�g�R���g���[���[�̖���")]
    [SerializeField] private string waveConObjName = "Controller";

    // �g�R���g���[���̃I�u�W�F�N�g�i�[�p
    private GameObject waveControllerObj;

    // �g�̐ݒ�p�X�N���v�g�i�[�p
    private waveController waveConScript;

   
    private float waveAmplitude; // �g�̐U��
    private float waveSpeed;     // �g�̑��x
    private float waveLength;    // �g�̐U����

    private float waveStartTime;    // �U���J�n����
    private float waveElapsedTime;  // �U���J�n����̌o�ߎ���

    private float thisNomalPointX; // ���̃I�u�W�F�N�g�̕W��X�ʒu
    private float thisNomalPointZ; // ���̃I�u�W�F�N�g�̕W��Z�ʒu
    private float thisNomalPointY; // ���̃I�u�W�F�N�g�̕W��Y���W
    private List<float> l_thisNomalHight = new List<float>(); // �g�̑傫��
    private float thisPointHight;  // ���̃I�u�W�F�N�g�ɔg���������̍��v�̍���

    [SerializeField] private List<float> l_waveOrigin; // �g�̐k����X���W
    [SerializeField] private List<float> l_waveReflectionPoint;
    
    [SerializeField] private List<float> l_toOriginDistanse = new List<float>(); // �����n�_����̋���
    // ���̃I�u�W�F�N�g�̒P�U�����J�n��������
    [SerializeField] private List<float> l_thisPointWaveStartTime = new List<float>(); 
    // ���̃I�u�W�F�N�g�̒P�U���J�n����̌o�ߎ���
    [SerializeField] private List<float> l_thisPointWaveElapsedTime = new List<float>();

    enum WAVE_VELOCITY
    {
        RIGHT,
        LEFT,
        BOTH,
    }
    
    //���˒n�_���o�R�����ꍇ�̋���
    [SerializeField] private List<float> l_thisPointWaveReflectionDistance = new List<float>();
    // private float waveOrigin = 10.0f;       // �g�̐k����X���W
    // private float toOriginDistanse = 0.0f; // �����n�_����̋���

    private Transform thisPointTransform; // ���̃I�u�W�F�N�g��Transform

    private const float PI = Mathf.PI; // �~����


    private int waveMoveFlg = 0;
    private List<int> l_waveMoveFlg = new List<int>();

    private int waveOriginCount = 0;

    private int x = 1;

    // �쐬��2023/2/16  2023/2/27�X�V��
    // ���c
    // Start is called before the first frame update
    void Start()
    {
        // ���̃I�u�W�F�N�g��Transform���i�[
        thisPointTransform = this.transform;

        // �g�̐ݒ�p�X�N���v�g���A�^�b�`�����I�u�W�F�N�g��������
        waveControllerObj = GameObject.Find(waveConObjName);
        // �g�̐ݒ�p�X�N���v�g���擾
        waveConScript = waveControllerObj.GetComponent<waveController>();
        // �g�̐U�����擾
        waveAmplitude = waveConScript.waveAmplitude;
        // �g�̑��x���擾
        waveSpeed = waveConScript.waveSpeed;
        // �g�̐U�������擾
        waveLength = waveConScript.waveLength;
        // ��{���W���擾
        thisNomalPointX = thisPointTransform.position.x; // �I�u�W�F�N�gX���W
        thisNomalPointY = thisPointTransform.position.y; // �I�u�W�F�N�gY���W
        thisNomalPointZ = thisPointTransform.position.z; // �I�u�W�F�N�gZ���W
        l_waveOrigin = waveConScript.l_waveOrigin;
        l_waveReflectionPoint = waveConScript.l_waveReflectionPoint;
        for(int i = 0; i < l_waveOrigin.Count; i++)
        {
            l_thisNomalHight.Add(0.0f);
            l_toOriginDistanse.Add(0.0f);
            l_thisPointWaveStartTime.Add(0.0f);
            l_thisPointWaveElapsedTime.Add(0.0f);
            l_waveMoveFlg.Add(0);
            
        }
        for(int i = 0; i < l_waveReflectionPoint.Count; i++)
        {
            l_thisPointWaveReflectionDistance.Add(0.0f);
        }
    }

    // �쐬��2023/2/16  2023/2/27�X�V��
    // ���c
    // Update is called once per frame
    void Update()
    {
        thisPointHight = 0;

        if (Input.GetKeyDown(KeyCode.Space) && waveMoveFlg == 0)
        {
            waveStartTime = Time.time;
            waveMoveFlg = 1;
            for(int i = 0;i<l_waveMoveFlg.Count;i++)
            {
                l_waveMoveFlg[i] = 1;
            }
        }
        


        if (waveMoveFlg == 1)
        {
           
            waveElapsedTime = Time.time - waveStartTime;
            // �g��X���W��ł̋������v�Z
            // Mathf.Abs�Ő�Βl���擾���Čv�Z
            for (int i = 0; i < l_waveOrigin.Count; i++)
            {
                if (waveConScript.l_waveVelocity[i] == waveController.WAVE_VELOCITY.RIGHT)
                {
                    if(thisNomalPointX<l_waveOrigin[i])
                    {
                        l_waveMoveFlg[i] = 0;
                    }
                }
                else if (waveConScript.l_waveVelocity[i] == waveController.WAVE_VELOCITY.LEFT)
                {
                    if (thisNomalPointX > l_waveOrigin[i])
                    {
                        l_waveMoveFlg[i] = 0;
                    }
                }
                
                l_toOriginDistanse[i] = Mathf.Abs(thisNomalPointX - (l_waveOrigin[i]));

                //if(i%2 == 0)
                //{
                //    x = -1;
                //}
                //else
                //{
                //    x = 1;
                //}


                if (l_waveMoveFlg[i] == 1)
                {
                    // (�U�� - ���� / 10)�ŐU���̌������v�Z�B0�ȉ��ł���΍����̍X�V�����Ȃ�
                    if (waveAmplitude - l_toOriginDistanse[i] / 10 > 0
                        && (l_toOriginDistanse[i] <= waveSpeed * waveElapsedTime))
                    {

                        // �I�u�W�F�N�g��P�U��������
                        // y = Asin2��(t - x / ��)�̎���p����
                        // weveLength����Z���邱�ƂŐU�����𑝉�������
                        l_thisNomalHight[i] = x * (waveAmplitude - l_toOriginDistanse[i] / 10)
                            * Mathf.Sin(2.0f * PI * waveLength * (waveElapsedTime - (l_toOriginDistanse[i] / waveSpeed)));



                    }

                    if (waveElapsedTime > (1 / waveLength + l_toOriginDistanse[i] / waveSpeed ))
                    {
                        l_waveMoveFlg[i] = 0;
                        l_thisNomalHight[i] = 0;
                    }
                    if (!l_waveMoveFlg.Contains(1))
                    {

                        waveMoveFlg = 0;
                    }

                    thisPointHight += l_thisNomalHight[i];


                }
            }
            //for (int i = 0; i < l_waveOrigin.Count; i++)
            //{
            //    // ���x�Ɍo�ߎ��Ԃ��������������A�k������̋����ɒB����Γ�����
            //    if (l_toOriginDistanse[i] <= waveSpeed * waveElapsedTime)
            //    {//----- if_start -----
                 // ���W�̍X�V
            thisPointTransform.position = new Vector3(thisNomalPointX, thisPointHight+thisNomalPointY, thisNomalPointZ);

            //}//----- if_stop -----
            //else
            //{//----- else_start -----
            // // ���W�̌Œ�
            //    thisPointTransform.position = new Vector3(thisNomalPointX, thisNomalPointY, thisNomalPointZ);

            //}//----- else_stop -----
            //}


            
        }
        
        if(waveMoveFlg == 0)
        {
            // ���W�̌Œ�
            thisPointTransform.position = new Vector3(thisNomalPointX, thisNomalPointY, thisNomalPointZ);
        }



        
    }

    //==================================================
    // �Q�[���I�����ɑ��݂��郊�X�g��j������B
    // ��������
    // �߂�l����
    //==================================================
    // �쐬��2023/2/24
    // ���c
    private void OnApplicationQuit()
    {
        for (int i = l_waveOrigin.Count-1; i < 0; i--)
        {
            l_thisNomalHight.RemoveAt(i);
            l_toOriginDistanse.RemoveAt(i);
            l_waveOrigin.RemoveAt(i);
            l_thisPointWaveStartTime.RemoveAt(i);
            l_thisPointWaveElapsedTime.RemoveAt(i);
        }
    }

    //�����蔻��
    //private void OnCollisionEnter(Collision collision)
    //{
    //    if(collision.gameObject.tag == "Enemy"&&waveMoveFlg==1)
    //    {
    //        collision.rigidbody.AddForce(new Vector3(0, 1, 0),ForceMode.Impulse);
    //    }
    //}
}
