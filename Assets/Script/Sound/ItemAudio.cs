using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAudio : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource ItemSource;

    [Header("�A�C�e������")]
    [SerializeField] AudioClip itemGetSound;
    [Header("�X�^�[�R�C������")]
    [SerializeField] AudioClip itemGetStarCoinSound;

    //���ʒ���
    [Header("�A�C�e������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float itemGetVolume;
    [Header("�X�^�[�R�C������̃T�E���h����")]
    [SerializeField, Range(0f, 1f)] float itemGetStarCoinVolume;

    // Start is called before the first frame update
    void Start()
    {
        ItemSource = GetComponent<AudioSource>();
    }

    public void GetSound()
    {
        ItemSource.volume = itemGetVolume;
        ItemSource.PlayOneShot(itemGetSound);
    }
    public void GetStarCoinSound()
    {
        ItemSource.volume = itemGetStarCoinVolume;
        ItemSource.PlayOneShot(itemGetStarCoinSound);
    }
}
