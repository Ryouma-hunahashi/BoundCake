using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class waveSwitchMagaer : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]vfxManager vfxManager;
     float startPosY;
    [Header("true�Ȃ牺�����ɁAfalse�Ȃ�ォ�牺�ɔg����")]
    [SerializeField] bool Direction = true;
    [Header("�g�̑��x�E���l��0�ȏ�ɂ���")]
    [SerializeField]float waveSpeed=2;
    [SerializeField] float waveHeight=2;

    //������N�����Ă�����d\width�𐳂̐��l�A�E����N�����Ă����畉�̐��l�Ƃ��Ă���
    [SerializeField] float waveWidth=1;
    void Start()
    {
        //���������Ȃ瑬�x���}�C�i�X�ɂ���
        if (Direction == false)
            waveSpeed *= -1;
        if(waveSpeed<0)
        {
            Debug.Log("�g�̑��x��0�����ɐݒ肳��Ă��܂�");
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
