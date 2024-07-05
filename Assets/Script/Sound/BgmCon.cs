using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BgmCon : MonoBehaviour
{
    private AudioSource audio;
    [SerializeField] private AudioClip introBGM;
    [SerializeField] private AudioClip mainBGM;
    [SerializeField] private AudioClip bossIntroBGM;
    [SerializeField] private AudioClip bossMainBGM;



    private bool mainIntro;
    private bool bossIntro;

    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
        if (audio == null)
        {
            Debug.LogError("AudioSource‚ª‚ ‚è‚Ü‚¹‚ñ");
        }
        FieldBGMStart();
    }

    // Update is called once per frame
    void Update()
    {
        if (mainIntro && !audio.isPlaying)
        {
            audio.loop = true;
            audio.clip = mainBGM;
            audio.Play();
            mainIntro = false;
        }
        else if (bossIntro && !audio.isPlaying)
        {
            audio.loop = true;
            audio.clip = bossMainBGM;
            audio.Play();
            bossIntro = false;
        }
    }

    public void BossBattleStart()
    {
        audio.clip = bossIntroBGM;
        audio.loop = false;
        audio.Play();
        bossIntro = true;
        mainIntro = false;
    }

    public void FieldBGMStart()
    {
        audio.clip = introBGM;
        audio.loop = false;
        audio.Play();
        bossIntro = false;
        mainIntro = true;
    }
}
