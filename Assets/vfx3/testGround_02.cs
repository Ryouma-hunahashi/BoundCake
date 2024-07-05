using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGround_02 : MonoBehaviour
{

    [Header("同期させる波のエフェクト")]
    [SerializeField]vfxManager m_manager;
    //WaveSpawn関数に渡すスピード
    public float m_speed = 0;
    //WaveSpawn関数に渡す開始位置
    float m_pos = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            //仮にプレイヤーのX座標を波の開始位置とする
            m_pos = collision.gameObject.transform.position.x;
            //波の速度設定
            //m_speed = Random.Range(-20,20);s
            //m_speed = -10;
            m_manager.WaveSpawn(m_pos,m_speed,5,1,0);
        }
    }
}
