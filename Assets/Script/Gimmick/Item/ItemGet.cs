using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGet : MonoBehaviour
{
    // プレイヤーにつける用
    private void OnTriggerEnter(Collider other)
    {
        // コインタグに触れた時
        if (other.gameObject.tag == "Coin")
        {// -----if start -----
            // ItemDetaからのオブジェクトを受け取る
            ItemDeta item = other.gameObject.GetComponent<ItemDeta>();
            if (item.getFg == true)
            {
                // ItemDetaからのオブジェクトを受け取る

                // スコアを加算する
                StatusManager.coinCount++;
                StatusManager.gameScore += item.value;

                //アイテム(コイン)を削除する
                other.gameObject.SetActive(false);
            }

        }// -----if stop -----
    }
}
