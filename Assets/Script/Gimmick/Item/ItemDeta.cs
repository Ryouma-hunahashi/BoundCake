using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDeta : MonoBehaviour
{
    // �R�C���ɂ���p
    // �擾������X�R�A�𓾂�
    public int value = 0;   //�����l�̐ݒ�
    [SerializeField] private LayerMask groundLayer = 1 << 6;    // �O���E���h���C���[��6�Ԗڂ̃��C���[�Ƃ��ď�����
    public bool getFg = true;   // �G��Ď擾�ł��邩�̃t���O
    private ItemAudio itemAudio;
    private AudioSource audioSource;
    private SphereCollider itemCol;

    private bool deleatFg = false;
    private GameObject childObj;

    // Start is called before the first frame update
    void Start()
    {
        this.gameObject.name = "Coin";
        itemAudio = GetComponent<ItemAudio>();
        audioSource = GetComponent<AudioSource>();
        itemCol = GetComponent<SphereCollider>();
        childObj = transform.GetChild(0).gameObject;
    }

    void FixedUpdate()
    {
        if(deleatFg)
        {
            if(!audioSource.isPlaying)
            {
                //�A�C�e��(�R�C��)���폜����
                gameObject.SetActive(false);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (1 << other.gameObject.layer == groundLayer)
        {
            getFg = false;
        }
        
    }
    private void OnTriggerStay(Collider other)
    {
        // �v���C���[�^�O�ɐG�ꂽ��
        if (other.gameObject.tag == "Player")
        {// -----if start -----
            
            if (getFg == true)
            {
                // ItemDeta����̃I�u�W�F�N�g���󂯎��
                if(StatusManager.nowHitPoint < StatusManager.maxHitPoint)
                {
                    StatusManager.coinCount++;
                }
                // �X�R�A�����Z����
                
                StatusManager.gameScore += value;
                getFg = false;
                deleatFg = true;
                childObj.SetActive(false);
                itemCol.enabled = false;
                itemAudio.GetSound();
            }

        }// -----if stop -----
    }


    // ������
    // ���̊֐��ǉ�
    /// <summary>
    /// �N�b�L�[���擾�\��Ԃɂ���֐�
    /// </summary>
    public void MakeGettableCoin()
    {
        getFg = true;
        deleatFg = false;
        childObj.SetActive(true);
        itemCol.enabled = true;
    }
}
