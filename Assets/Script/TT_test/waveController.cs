using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �쐬��2023/2/16  2023/2/27�X�V��
// ���c
public class waveController : MonoBehaviour
{
    public enum WAVE_VELOCITY
    {
        RIGHT,
        LEFT,
        BOTH,
    }
    [Header("�g�̐U��")]
    public float waveAmplitude = 5.0f;
    [Header("�g�̑��x")]
    public float waveSpeed = 3.0f;
    [Header("�g�̐U����")]
    public float waveLength = 0.25f;
    [Header("�g�̐k���̈ʒu")]
    public List<float> l_waveOrigin = new List<float>(); // �g�̐k����X���W
    [Header("�g�̕���")]
    public List<WAVE_VELOCITY> l_waveVelocity = new List<WAVE_VELOCITY>(); 


    [Header("�g�̔��ˈʒu")]
    public List<float> l_waveReflectionPoint = new List<float>(); // �g�����˂���n�_
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
