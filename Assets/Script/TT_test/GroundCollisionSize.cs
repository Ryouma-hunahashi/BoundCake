using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCollisionSize : MonoBehaviour
{
    [Header("プレイヤーのタグ")]
    [SerializeField] private string playerTagName = "Player";
    [Header("掴みの余裕")]
    [Tooltip("プレイヤーサイズの半分に乗算する")]
    [SerializeField] private float colScaleMarginIndex = 1.2f;
    // [Player] オブジェクト格納用
    private GameObject playerObject;


    // 3つの[BoxCollider]を格納させる。
    // 0：中心のCollider
    // 1：　右のCollider
    // 2：　左のCollider
    private BoxCollider[] groundCollider = new BoxCollider[3];

    // 左右の [BoxCollider] のXサイズ
    private float bothScaleX;
    // 左右の [BoxCollider] の中心X
    private float bothCollisionCentorX;
    // 真ん中の [BoxCollider] のXサイズ
    private float bottomCollisionScaleX;

    [System.NonSerialized] public bool colSizeSetFg = false;

    
    void Awake()
    {
        // プレイヤータグからプレイヤーを探索
        playerObject = GameObject.FindWithTag(playerTagName);
        if (playerObject == null)
        {//----- if_start -----

            Debug.LogError("プレイヤーオブジェクトが見つかりません");

        }//----- if_stop -----

        // コンポーネントしてある全ての[BoxCollider]を取得
        groundCollider = GetComponents<BoxCollider>();
        // 三つコンポーネントされていなければエラー
        if(groundCollider.Length != 3)
        {//----- if_start -----

            Debug.LogError("[BoxCollider]が足りません");

        }//----- if_stop -----

        // Collision上でのプレイヤーサイズの半分を計算。掴みの余裕を持たせるため1.2を掛ける
        bothScaleX = (playerObject.transform.localScale.x / 2) / transform.localScale.x * colScaleMarginIndex;

        // コリジョンの大きさの半分からサイズの半分を引くことで、左右の中心を計算
        bothCollisionCentorX = 0.5f - bothScaleX / 2;

        // コリジョンの大きさから左右の大きさ分引くことで、真ん中のコリジョンの大きさを計算
        bottomCollisionScaleX = 1.0f - bothScaleX * 2;



        //===== 真ん中の[BoxCollider] =====

        // isTriggerが付いていれば切る
        if(groundCollider[0].isTrigger == true)
        {//----- if_start -----

            groundCollider[0].isTrigger = false;

        }//----- if_stop -----

        // ど真ん中に中心を固定
        groundCollider[0].center = Vector3.zero;

        // Xサイズを [bottomCollisionScaleX] に変更
        groundCollider[0].size = new Vector3(bottomCollisionScaleX, groundCollider[0].size.y, groundCollider[0].size.z);



        //===== 右の[BoxCollider] =====

        // isTriggerが付いていなければ付ける
        if (groundCollider[1].isTrigger == false)
        {//----- if_start -----

            groundCollider[1].isTrigger = true;

        }//----- if_stop -----

        // 右側( + )の [Trigger] コリジョンの中心を指定
        groundCollider[1].center = new Vector3(bothCollisionCentorX, 0, 0);
        // Xサイズを [bothScaleX] に変更
        groundCollider[1].size = new Vector3(bothScaleX, groundCollider[1].size.y, groundCollider[1].size.z);


        // isTriggerが付いていなければ付ける
        if (groundCollider[2].isTrigger == false)
        {//----- if_start -----

            groundCollider[2].isTrigger = true;

        }//----- if_stop -----

        // 左側( - )の [Trigger] コリジョンの中心を指定
        groundCollider[2].center = new Vector3(-bothCollisionCentorX, 0, 0);
        // Xサイズを [bothScaleX] に変更
        groundCollider[2].size = new Vector3(bothScaleX, groundCollider[2].size.y, groundCollider[2].size.z);

        

    }

    // Start is called before the first frame update
    //void Start()
    //{

    //}

    // Update is called once per frame
    void Update()
    {
        if(colSizeSetFg == true)
        {
            // Collision上でのプレイヤーサイズの半分を計算。掴みの余裕を持たせるため1.2を掛ける
            bothScaleX = (Mathf.Abs(playerObject.transform.localScale.x / 2)) / transform.localScale.x * colScaleMarginIndex;

            // コリジョンの大きさの半分からサイズの半分を引くことで、左右の中心を計算
            bothCollisionCentorX = 0.5f - bothScaleX / 2;

            // コリジョンの大きさから左右の大きさ分引くことで、真ん中のコリジョンの大きさを計算
            bottomCollisionScaleX = 1.0f - bothScaleX * 2;



            //===== 真ん中の[BoxCollider] =====

           

            // ど真ん中に中心を固定
            groundCollider[0].center = Vector3.zero;

            // Xサイズを [bottomCollisionScaleX] に変更
            groundCollider[0].size = new Vector3(bottomCollisionScaleX, groundCollider[0].size.y, groundCollider[0].size.z);



            //===== 右の[BoxCollider] =====

            

            // 右側( + )の [Trigger] コリジョンの中心を指定
            groundCollider[1].center = new Vector3(bothCollisionCentorX, 0, 0);
            // Xサイズを [bothScaleX] に変更
            groundCollider[1].size = new Vector3(bothScaleX, groundCollider[1].size.y, groundCollider[1].size.z);


            

            // 左側( - )の [Trigger] コリジョンの中心を指定
            groundCollider[2].center = new Vector3(-bothCollisionCentorX, 0, 0);
            // Xサイズを [bothScaleX] に変更
            groundCollider[2].size = new Vector3(bothScaleX, groundCollider[2].size.y, groundCollider[2].size.z);
        }
    }
}
