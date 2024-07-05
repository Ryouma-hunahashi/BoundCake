using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveManager : MonoBehaviour
{
    public static WaveManager instance = null;

    [Header("‘å”g‚ÌŠî€’l")]
    [SerializeField] private float strongWaveBase = 2.5f;
    public bool CheckStrongWave(float _hignt) { return strongWaveBase <= _hignt; }
    [Header("”g‚ÌTag–¼")]
    public const string waveTag = "Wave";


    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }



    //// Start is called before the first frame update
    //void Start()
    //{
        
    //}

    //// Update is called once per frame
    //void Update()
    //{
        
    //}
}
