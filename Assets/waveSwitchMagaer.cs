using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSwitchMagaer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]vfxManager vfxManager;
     float startPosY;
    [Header("trueなら下から上に、falseなら上から下に波発生")]
    [SerializeField] bool Direction = true;
    [Header("波の速度・数値は0以上にする")]
    [SerializeField]float waveSpeed=2;
    [SerializeField] float waveHeight=2;

    //左から侵入してきたらd\widthを正の数値、右から侵入してきたら負の数値としておく
    [SerializeField] float waveWidth=1;
    void Start()
    {
        //向きが下なら速度をマイナスにする
        if (Direction == false)
            waveSpeed *= -1;
        if(waveSpeed<0)
        {
            Debug.Log("波の速度が0未満に設定されています");
        }    
        //vfxManager=this.transform.parent.GetComponent<vfxManager>();    
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Wave"))
        {
            //waveSpeed=other.GetComponent<vfxManager>


            vfxManager.WaveSpawn(startPosY, waveSpeed, waveHeight, waveWidth,0);
        }
    }
}
