using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class Goal : MonoBehaviour
{
    [SerializeField] OtherEffectManager effect;
    [SerializeField, TagField]
    private string ContactTag; // 指定のタグ設定

    [SerializeField, Tooltip("指定時間で画面遷移")]
    private float weight;

    private bool goalflg; // ゴール判定ふらぐ


    Player_Main pl_main;
    [System.Serializable]
    public enum GoalType
    {
        result1, // ステージ１のリザルト
        result2, // ステージ２のリザルト
        result3, // ステージ３のリザルト
        result4, // ステージ４のリザルト　
        result5, // ステージ５のリザルト
    }

    public GoalType resultkinds = GoalType.result1;

    // Start is called before the first frame update
    void Start()
    {
        goalflg = false;
    }
    // Update is called once per frame
    void Update()
    {
        // ゴール条件を満たしたとき
        if (goalflg)
        {
            effect.StopCooky();

            // 各リザルトシーンに移動する
            switch (resultkinds)
            {
                case GoalType.result1:
                    SceneManager.LoadScene("result1");
                    break;
                case GoalType.result2:
                    SceneManager.LoadScene("result2");
                    break;
                case GoalType.result3:
                    SceneManager.LoadScene("result3");
                    break;
                case GoalType.result4:
                    SceneManager.LoadScene("result4");
                    break;
                case GoalType.result5:
                    SceneManager.LoadScene("result5");
                    break;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // 指定タグに触れたら
        if (other.gameObject.tag == ContactTag) 
        {
            Debug.Log(Result_Manager.instance.nowStage);
            // pl_main.jumpButton = false;
            effect.StartCooky();
            // コルーチンを発生
            StartCoroutine("Stop");
        }
    }
    IEnumerator Stop()
    {
        //----------------
        // エフェクト再生
        //---------------
       

        // この時間の間処理を一旦停止する
        yield return new WaitForSeconds(weight);
        // ゴール判定フラグをtrueにする
        goalflg = true;
    }
}
