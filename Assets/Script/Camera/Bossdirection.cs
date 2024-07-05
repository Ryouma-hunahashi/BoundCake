using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Bossdirection : MonoBehaviour
{
    // ボスの所にたどりついたときに起こる処理
    private struct BossWall
    {
        public Vector3 left;
        public Vector3 right;
    };
    [SerializeField]private BossWall dif_wall;
    [SerializeField]private BossWall now_wall;
    
    private BossWall updownUi;
    [SerializeField]private BossWall def_posUi;

    [SerializeField]private float des_y;

    // ステージの範囲に入ったら縦糸を少し下げてボスステージからでなくさせる
    [SerializeField] private GameObject leftwallobj;
    [SerializeField] private GameObject rightwallobj;
    // ボスの演出中はUIを一時的に消去する
    [SerializeField]private Canvas mainUi;
    // ボスステージに入ったときに少し外枠を黒ぶちで埋める
    [SerializeField] private Canvas bossUi;
    [SerializeField] private Image topImage;
    [SerializeField] private Image downImage;

    [SerializeField]private float blocktime;
    [SerializeField] private float time = 0.0f;

    private float test = 5.0f;
    [SerializeField]private bool playercheck = false;
    bool bossend = false;
    bool playerstop = false;
    [SerializeField]private bool movecheck ;

    [SerializeField]private bool UiCheck = true;
    public bool bossleftcheck = false;
    public bool bossrightcheck = false;

    [SerializeField] private int movetime = 5;


    public demo_B5 boss;

    public Player_Main player;

    public GameObject boss_obj;
    public GameObject player_obj;

    private BgmCon bgm;


    //[SerializeField] Vector3 now_vector;
    //[SerializeField] Vector3 dif_vector;
    //[SerializeField] Vector3 des_vector;


    private IEnumerator Stopplayer()
    {
        yield return new WaitForSeconds(5);

        // 正常に動いた
        movecheck = true;
        playerstop = true;
        // ボス行動開始
        boss.start = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        boss_obj = GameObject.Find("boss5");
        boss = boss_obj.GetComponent<demo_B5>();

        player_obj = GameObject.Find("player");
        player = player_obj.GetComponent<Player_Main>();

        bgm = GameObject.FindWithTag("MainCamera").GetComponent<BgmCon>();

        // 現在の壁の座標を代入
        dif_wall.left = leftwallobj.transform.position;
        dif_wall.right = rightwallobj.transform.position;
        
        // どの位置に壁をおろすか
        dif_wall.left.y -= des_y;
        dif_wall.right.y -= des_y;

        updownUi.left = topImage.transform.position;
        updownUi.right = downImage.transform.position;
        def_posUi.left = topImage.transform.position;
        def_posUi.right = downImage.transform.position;

        updownUi.left.y -= 150;
        updownUi.right.y += 150;

        // ボスステージに入るまでUiをいったん消しておく
        bossUi.enabled = false;

        // 最初の位置を取得する
        now_wall.left = leftwallobj.transform.position;
        now_wall.right = rightwallobj.transform.position;

        playerstop = true;
        movecheck = false;

    }
    
    // Update is called once per frame
    void Update()
    {
        // プレイヤーが所定位置に行った場合
           
        //プレイヤーが触れたかどうかを判定する

        if(playercheck)
        {

            bossleftcheck = false;

            if(!bossleftcheck&&movecheck)
            {
                leftwallobj.gameObject.transform.position = Vector3.MoveTowards(leftwallobj.transform.position, dif_wall.left, blocktime * Time.deltaTime);

            }

            // 演出を開始する
            if (!movecheck)
            {
                // プレイヤーの処理をいったん止める
                player.isStop = 1;

                //player.H_MoveAxis = 0;
                

                // ボス用演出のUIを表示する
                bossUi.enabled = true;

                rightwallobj.gameObject.transform.position = Vector3.MoveTowards(rightwallobj.transform.position, dif_wall.right, blocktime * Time.deltaTime);
                leftwallobj.gameObject.transform.position = Vector3.MoveTowards(leftwallobj.transform.position, dif_wall.left, blocktime * Time.deltaTime);

                Debug.Log("両壁おろしまーす");

                // UIを移動させる
                Debug.Log("UI移動中");
                topImage.transform.position = Vector3.MoveTowards(topImage.transform.position, updownUi.left, blocktime);
                downImage.transform.position = Vector3.MoveTowards(downImage.transform.position, updownUi.right, blocktime);


                Debug.Log("mainのUI消すよー");
                mainUi.enabled = false;

                // 移動し終わったら
                if ((rightwallobj.transform.position == dif_wall.right && leftwallobj.transform.position == dif_wall.left)
                    &&(topImage.transform.position == updownUi.left && downImage.transform.position == updownUi.right))
                {

                    StartCoroutine(Stopplayer());
                }
            }

            // 移動が完了した時
            if(movecheck)
            {
                topImage.transform.position = Vector3.MoveTowards(topImage.transform.position, def_posUi.left, blocktime);
                downImage.transform.position = Vector3.MoveTowards(downImage.transform.position, def_posUi.right, blocktime);

                Debug.Log("UI戻すよー");
                mainUi.enabled = true;

                Debug.Log("正常にうごいてるよーーー");
            }
        }

        if (playerstop)
        {
            if (this.gameObject.transform.position.x <= player_obj.transform.position.x)
            {
                // プレイヤーを操作可能状態にする
                player.isStop = 0;

                // プレイヤーの行動を停止させる
                playerstop = false;
            }
        }

        if(bossleftcheck)
        {
            // ボスステージでやられてもう一回壁を閉じるときに必要
            leftwallobj.gameObject.transform.position = Vector3.MoveTowards(leftwallobj.transform.position, now_wall.left, blocktime * Time.deltaTime);
        }

        if(bossrightcheck)
        {
            // ボスがやられたとき
            rightwallobj.gameObject.transform.position = Vector3.MoveTowards(rightwallobj.transform.position, now_wall.right, blocktime * Time.deltaTime);
            
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // プレイヤーがボスステージに入ったら
        if(other.gameObject.tag == "Player")
        {
            // ムービーを流す
            playercheck = true;

            bgm.BossBattleStart();

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // ボスエリアから離れた時

            // 演出をなくす
            playercheck = false;
           
            bossleftcheck = true;

            bgm.FieldBGMStart();

            Debug.Log("動いたー？");
        }
    }

}


