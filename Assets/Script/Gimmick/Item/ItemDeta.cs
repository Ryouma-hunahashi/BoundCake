using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDeta : MonoBehaviour
{
    // コインにつける用
    // 取得したらスコアを得る
    public int value = 0;   //整数値の設定
    [SerializeField] private LayerMask groundLayer = 1 << 6;    // グラウンドレイヤーを6番目のレイヤーとして初期化
    public bool getFg = true;   // 触れて取得できるかのフラグ
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
                //アイテム(コイン)を削除する
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
        // プレイヤータグに触れた時
        if (other.gameObject.tag == "Player")
        {// -----if start -----
            
            if (getFg == true)
            {
                // ItemDetaからのオブジェクトを受け取る
                if(StatusManager.nowHitPoint < StatusManager.maxHitPoint)
                {
                    StatusManager.coinCount++;
                }
                // スコアを加算する
                
                StatusManager.gameScore += value;
                getFg = false;
                deleatFg = true;
                childObj.SetActive(false);
                itemCol.enabled = false;
                itemAudio.GetSound();
            }

        }// -----if stop -----
    }


    // あさわ
    // この関数追加
    /// <summary>
    /// クッキーを取得可能状態にする関数
    /// </summary>
    public void MakeGettableCoin()
    {
        getFg = true;
        deleatFg = false;
        childObj.SetActive(true);
        itemCol.enabled = true;
    }
}
