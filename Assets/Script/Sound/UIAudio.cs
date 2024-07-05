using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIAudio : MonoBehaviour
{
    //新規関数・スクリプト
    //======================================
    //作成者：中村
    //作成日：05/20
    //======================================

    // Start is called before the first frame update
    //UIのaudioSourceを保持する変数
    public AudioSource UISource;

    //UI操作時のSEを変数に格納していく
    [Header("カーソル移動のサウンド")]
    [SerializeField] private AudioClip UIMoveCursorSound;
    [Header("決定のサウンド")]
    [SerializeField] private AudioClip UIDecisionSound;
    [Header("キャンセルのサウンド")]
    [SerializeField] private AudioClip UICancelSound;
    [Header("ポーズ画面のサウンド")]
    [SerializeField] private AudioClip UIPauseSound;
    [Header("ゲーム開始時のサウンド")]
    [SerializeField] private AudioClip UIStartSound;
    [Header("シーン切り替えのサウンド")]
    [SerializeField] private AudioClip UIChangeSceneSound;
    [Header("風に舞う紙のサウンド")]
    [SerializeField] private AudioClip UIWindSound;
    [Header("しおりのサウンド")]
    [SerializeField] private AudioClip UIBookMarkSound;
    [Header("地図がはまる音")]
    [SerializeField] private AudioClip UIMapSetSound;

    //SEの音量を調整する用の変数を作る
    [Header("カーソル移動時のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UIMoveCursorVolume;
    [Header("決定時のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UIDecisionVolume;
    [Header("キャンセル時のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UICancelVolume;
    [Header("ポーズ画面のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UIPauseVolume;
    [Header("ゲーム開始時のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UIStartVolume;
    [Header("シーン切り替えのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UIChangeSceneVolume;
    [Header("風に舞う紙のサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UIWindVolume;
    [Header("しおりのサウンド音量")]
    [SerializeField, Range(0f, 1f)] float UIBookMarkVolume;
    [Header("地図がはまる音")]
    [SerializeField, Range(0f, 1f)] float UIMapSetVolume;
    void Start()
    {
        //UIのAudioSourceコンポーネントを取得する
        UISource = GetComponent<AudioSource>();
    }

    public void MoveCursorSound()
    {
        UISource.volume = UIMoveCursorVolume;
        UISource.PlayOneShot(UIMoveCursorSound);
    }
    public void DecisionSound()
    {
        UISource.volume = UIDecisionVolume;
        UISource.PlayOneShot(UIDecisionSound);
    }
    public void CancelSound()
    {
        UISource.volume = UICancelVolume;
        UISource.PlayOneShot(UICancelSound);
    }
    public void PauseSound()
    {
        UISource.volume = UIPauseVolume;
        UISource.PlayOneShot(UIPauseSound);
    }
    public void StartSound()
    {
        UISource.volume = UIStartVolume;
        UISource.PlayOneShot(UIStartSound);
    }
    public void ChangeSceneSound()
    {
        UISource.volume = UIChangeSceneVolume;
        UISource.PlayOneShot(UIChangeSceneSound);
    }
    public void WindSound()
    {
        UISource.volume = UIWindVolume;
        UISource.PlayOneShot(UIWindSound);
    }
    public void BookMarkSound()
    {
        UISource.volume = UIBookMarkVolume;
        UISource.PlayOneShot(UIBookMarkSound);
    }
    public void MapSetSound()
    {
        UISource.volume = UIMapSetVolume;
        UISource.PlayOneShot(UIMapSetSound);
    }




    // Update is called once per frame
    //void Update()
    //{

    //}
}
