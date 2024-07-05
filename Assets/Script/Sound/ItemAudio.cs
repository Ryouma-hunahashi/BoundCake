using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAudio : MonoBehaviour
{
    // Start is called before the first frame update
    private AudioSource ItemSource;

    [Header("アイテム入手")]
    [SerializeField] AudioClip itemGetSound;
    [Header("スターコイン入手")]
    [SerializeField] AudioClip itemGetStarCoinSound;

    //音量調整
    [Header("アイテム入手のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float itemGetVolume;
    [Header("スターコイン入手のサウンド音量")]
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
