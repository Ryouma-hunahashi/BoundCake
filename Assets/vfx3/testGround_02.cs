using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testGround_02 : MonoBehaviour
{

    [Header("����������g�̃G�t�F�N�g")]
    [SerializeField]vfxManager m_manager;
    //WaveSpawn�֐��ɓn���X�s�[�h
    public float m_speed = 0;
    //WaveSpawn�֐��ɓn���J�n�ʒu
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
            //���Ƀv���C���[��X���W��g�̊J�n�ʒu�Ƃ���
            m_pos = collision.gameObject.transform.position.x;
            //�g�̑��x�ݒ�
            //m_speed = Random.Range(-20,20);s
            //m_speed = -10;
            m_manager.WaveSpawn(m_pos,m_speed,5,1,0);
        }
    }
}
