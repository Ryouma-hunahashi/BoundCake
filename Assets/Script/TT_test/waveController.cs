using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ì¬“ú2023/2/16  2023/2/27XV“ú
// ‚“c
public class waveController : MonoBehaviour
{
    public enum WAVE_VELOCITY
    {
        RIGHT,
        LEFT,
        BOTH,
    }
    [Header("”g‚ÌU•")]
    public float waveAmplitude = 5.0f;
    [Header("”g‚Ì‘¬“x")]
    public float waveSpeed = 3.0f;
    [Header("”g‚ÌU“®”")]
    public float waveLength = 0.25f;
    [Header("”g‚ÌkŒ¹‚ÌˆÊ’u")]
    public List<float> l_waveOrigin = new List<float>(); // ”g‚ÌkŒ¹‚ÌXÀ•W
    [Header("”g‚Ì•ûŒü")]
    public List<WAVE_VELOCITY> l_waveVelocity = new List<WAVE_VELOCITY>(); 


    [Header("”g‚Ì”½ËˆÊ’u")]
    public List<float> l_waveReflectionPoint = new List<float>(); // ”g‚ª”½Ë‚·‚é’n“_
    //// Start is called before the first frame update
    //void Start()
    //{

    //}

    //// Update is called once per frame
    //void Update()
    //{

    //}
}
