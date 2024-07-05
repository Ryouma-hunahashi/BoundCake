using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Wave : MonoBehaviour
{


    // �e�̎q�ɂ���X�N���v�g���擾
    private Player_Fall pl_Fall;    // �����p�X�N���v�g
    private WavePool pool;

    [Header("----- �g�̐ݒ� -----"), Space(5)]
    
    [System.NonSerialized] public sbyte waveAngle = 1;//�v���C���[�̌����A���Ȃ獶�����A���Ȃ�E����

    [Header("�N�̔g��")]
    [SerializeField] private waveCollition.WAVE_TYPE waveType = waveCollition.WAVE_TYPE.PLAYER;

    [Header("�g�̑��x")]
    [SerializeField] private float waveSpeed = 7.5f;
    [Header("�g�̐U����")]
    [SerializeField] private float waveWidth = 0.225f;
    //private int waveStartPos = 0;
    [Header("�Œ�g�̍���")]
    public float waveHight = 2.0f;
    [Header("�g�̍����w��")]
    [Tooltip("���n���̃G�l���M�[���牽�{��")]
    public float waveHightIndex = 0.06f;

    [Header("----- ���𒾂܂���ݒ� -----"), Space(5)]
    [SerializeField] private float maxLandDistance = 0.5f;
    [SerializeField] private float landSpeed = 15;


    // Start is called before the first frame update
    void Start()
    {
        //for (byte i = 0; i < waveCount; i++)
        //{
        //    // �����̔g�R���W�����̐��A�\���̃��X�g�ɒǉ�
        //    l_collisions.Add(WaveCollisionSet());

        //}//-----for_atop-----
        pool = GetComponent<WavePool>();
        pl_Fall = transform.parent.GetComponentInChildren<Player_Fall>();
    }





    //==================================================
    // �g�̏���VFX�ɑ�����A�R���W�����ƂƂ��ɔg�𔭐�������B
    // ������ : �g�𔭐������������RaycastHit
    // ������ : ����������g�̍���
    //==================================================
    // �����2023/03/21    �X�V��2023/04/14
    // ���c�@����
    public void WaveCreate(RaycastHit groundHit, float waveHight, float waveSpornPosY)
    {
        if (pool.WaveCreate(waveSpeed*waveAngle, waveWidth, waveHight, waveType, groundHit.point.x, groundHit.transform))
        {
            groundHit.transform.GetComponent<LandingGround>().lnadGround(Mathf.Abs(maxLandDistance * pl_Fall.fallPowLog / pl_Fall.maxFallPow), landSpeed);
            if (pl_Fall.fallPowLog < -24)
            {
                string a = "strong";
                Vibration_Manager.instanse.VibrationSelect(a);
            }
            else
            {
                string a = "weak";
                Vibration_Manager.instanse.VibrationSelect(a);
            }
        }
        
    }


    //// �Q�[���I�����Ƀ��X�g��j������B
    //private void OnApplicationQuit()
    //{
    //    l_collisions.Clear();
    //}
}
