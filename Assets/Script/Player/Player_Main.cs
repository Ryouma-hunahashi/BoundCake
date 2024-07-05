using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//==================================================
//          メインスクリプト    ver4.1
// ※プレイヤーにあった挙動がすべて分散、改変されました
// ※これはプレイヤーの挙動を制御する、メインのスクリプトです
// ※実際の行動などは別スクリプトから取得してください
//==================================================
// 作成日2023/04/28    更新日2023/05/14
// 宮﨑
public class Player_Main : MonoBehaviour
{
    [System.Serializable]
    public struct RayDebug
    {
        [Header("表示設定")]
        // 進行方向に向けたレイ
        public bool lookRayHorizontal;
        // 頭上に向けたレイ
        public bool lookRayGrab;
        // 通過用のレイ
        public bool lookRayPlatform;
        // 接地関係のレイ
        public bool lookRayGround;
    };

    [Tooltip("レイの表示設定")]
    [SerializeField] private RayDebug rayDebug;

    // 略称の説明 --------------------
    // [ pl ] = player
    // [ Pos ] = position
    // [ H ] = horizontal
    // [ V ] = vertical
    // [ flg ] = flag
    //--------------------------------

    // ゲームパッドの入力情報を取得する
    float H_MoveAxis = 0.5f;    // Lスティック
    bool jumpButton = false;    // ジャンプボタン

    // 自身の情報を取得する
    private Rigidbody pl_Rb;
    public CapsuleCollider pl_Col;
    private Vector3 pl_Pos;
    private Vector3 pl_Scale;
    [System.NonSerialized] public Animator pl_Anim;
    private DamageAct pl_Act;

    // 自身の挙動情報を取得する
    private Player_Ride pl_Ride;    // 乗場時の挙動情報
    private Player_Grab pl_Grab;    // 掴み時の挙動情報
    private Player_Jump pl_Jump;    // 跳躍時の挙動情報
    private Player_Fall pl_Fall;    // 落下時の挙動情報
    private Player_Wave pl_Wave;    // 波発生の挙動情報


    // プレイヤーが干渉する外部情報
    public PlayerAudio pl_Audio;     // プレイヤーのサウンド再生用関数
    // private CameraVibration scrennVibe;  // カメラを揺らすスクリプト
    public LandingGround groundLand;
    public PlayerEffectManager ef_Manager;

    bool jumpButtonPressed;
    bool jumpButtonReleased;

    private bool grabCancelButton;

    // コントローラーの操作を止める
    public int isStop;

    // レイヤーの設定
    [Header("----- レイヤーの設定 -----"), Space(5)]
    [SerializeField] private LayerMask ground_Layer = 1 << 6;      // 地面のレイヤー
    [SerializeField] private LayerMask platform_Layer = 1 << 8;    // すり抜け床のレイヤー
                                                                   // レイの距離
    [Header("レイの距離")]

    // 壁判定をとるための距離
    [Tooltip("壁判定を取得する距離")]
    [SerializeField] private float H_RayDistance = 1.0f;        // 壁判定が入る位置

    // 掴みが可能な距離
    [Tooltip("掴みが可能な距離")]
    [SerializeField] private float grab_RayDistance = 1.05f;    // 掴み可能地点までの距離

    // 通過が可能な距離
    [Tooltip("通過を可能にする距離")]
    [SerializeField] private float platform_RayDistance = 1.1f; // 通過可能開始地点までの距離

    // 接地状態になるまでの距離
    [Tooltip("ジャンプ可能距離")]
    [SerializeField] private float ground_RayDistance = 0.35f;  // ジャンプ可能地点までの距離
    [Tooltip("ファジー可能距離")]
    [SerializeField] private float fuzzy_RayDistance = 2.75f;   // ファジー可能地点までの距離

    // レイの位置
    [Header("レイの位置")]

    // 接地判定を複数取るためのレイの位置
    [Tooltip("接地判定を出す位置の設定")]
    [SerializeField] private float ground_RayPos = 0.75f;       // 接地判定出現位置
    [Tooltip("接地判定のレイ間の位置設定")]
    [SerializeField] private float betweenRayPos = 0.825f;      // 接地判定間の距離


    // 移動の設定
    [Header("----- 移動の設定 -----"), Space(5)]

    [Tooltip("通常移動速度")]
    public float moveSpeed = 13f;       // 地上の移動速度
    [System.NonSerialized]
    public float holdInitalValueSpeed;  // 移動速度の初期値を格納する
    [Tooltip("空中での移動速度")]
    public float jumpSpeed;             // 空中速度

    [Tooltip("移動不可の状態にするか")]
    public bool immovable = false;

    // 各判定の状態
    public bool H_RayWall;      // 壁の判定
    public bool inRayRide;      // 乗場判定
    public bool underRayGrab;   // 掴み判定
    public bool onRayGround;    // 接地判定
    public bool nowJump;        // 跳躍判定

    private bool grabSetFg = false;
    private bool grabReleaseFg = false;

    [Tooltip("リスポーン地点の設定")]
    [SerializeField] private Vector3 respawnPosition;
    [Header("コインによるHP回復設定")]
    [Tooltip("コインいくつでHPを回復するか")]
    [SerializeField] private byte RecoverCoin = 30;

    // あさわ
    [Tooltip("リスポーンマネージャー設定")]
    [SerializeField] private CheckPointManager respawnManager;

    //private enum CameraVibe
    //{
    //    ON,
    //    OFF,
    //}

    //[Header("カメラを振動させるか否か")]
    //[SerializeField] private CameraVibe cameraVibe = CameraVibe.ON;

    private int groundColInsID = 0;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Respawn"))
        {
            respawnPosition = other.gameObject.GetComponent<RespornPoint>().respornPosition;

        }
        //if(other.gameObject.tag == "GameOver")
        //{
        //	Resporn();
        //}
    }
    private void Start()
    {
        // 自身の各情報を取得する
        pl_Rb = this.GetComponent<Rigidbody>();
        pl_Col = this.GetComponent<CapsuleCollider>();
        pl_Pos = this.transform.position;
        pl_Scale = this.transform.localScale;
        pl_Anim = this.GetComponent<Animator>();
        pl_Act = this.GetComponent<DamageAct>();

        // 自身の各挙動を格納
        pl_Grab = this.GetComponentInChildren<Player_Grab>();
        pl_Ride = this.GetComponentInChildren<Player_Ride>();
        pl_Jump = this.GetComponentInChildren<Player_Jump>();
        pl_Fall = this.GetComponentInChildren<Player_Fall>();
        pl_Wave = this.GetComponentInChildren<Player_Wave>();

        // 外部干渉スクリプトを格納
        pl_Audio = GetComponent<PlayerAudio>();
        ef_Manager = this.GetComponentInChildren<PlayerEffectManager>();
        //// カメラを振動させるようにしているなら
        //if (cameraVibe == CameraVibe.ON)
        //{
        //    // カメラ振動スクリプトを格納
        //    scrennVibe = GameObject.FindWithTag("MainCamera").GetComponent<CameraVibration>();
        //    // なけりゃエラー
        //    if (scrennVibe == null)
        //    {
        //        Debug.LogError("[CameraVibration]が見つかりません");

        //    }//-----if_stop-----
        //}//-----if_stop-----
        // 情報が一つでも入っていないのなら
        if (!pl_Rb || !pl_Col || pl_Pos == null || pl_Scale == null || !pl_Anim ||
            !pl_Grab || !pl_Ride || !pl_Jump || !pl_Fall || !pl_Wave ||
            !pl_Audio)
        {
            // エラー関数を呼ぶ
            GetError();

        }//----- if_stop -----

        respawnPosition = transform.position;

        // 自身の移動速度の初期値を格納しておく
        holdInitalValueSpeed = moveSpeed;

        // respawnManager設定
        respawnManager = GameObject.Find("RespawnManager").GetComponent<CheckPointManager>(); // あさわ
        if(respawnManager==null)
        {
            Debug.LogError("リスポーンマネージャーが見つかりません");
        }
    }

    private void Update()
    {
        if (isStop != 0) { return; }

        // ゲームパッドが接続されていなければnull
        if (Gamepad.current == null) return;

        // 入力情報の取得 ---------------

        // ジャンプ入力
        // 押された瞬間の情報を取得
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            jumpButtonPressed = true;
        }

        // 離された瞬間の情報を取得
        if (Gamepad.current.buttonSouth.wasReleasedThisFrame)
        {
            jumpButtonReleased = true;
        }

        // 掴み離すボタン
        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            grabCancelButton = true;
        }

        // 押されている間の状態を取得
        jumpButton = Gamepad.current.buttonSouth.isPressed;

        //if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        //{
        //    jumpButton = true;

        //}//----- if_stop -----
        //// ジャンプボタンが離された瞬間
        //else if (Gamepad.current.buttonSouth.wasReleasedThisFrame)
        //{
        //    jumpButton = false;

        //}//----- elseif_stop -----
    }

    private void FixedUpdate()
    {
        // 動きを止める
        if(isStop != 0) { return; }

        // 自身の座標を取得し続ける
        pl_Pos = this.transform.position;

        // 移動入力の方向を確認
        H_MoveAxis = Input.GetAxisRaw("Horizontal");

        // レイの更新 ---------------
        // 進行方向へ向かうレイの位置を更新する -------------------
        Ray horizontalRay = new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(0.0f, 0.0f, 0.0f));
        if (this.transform.localScale.x < 0.0f)
        {
            horizontalRay = new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(-1.0f, 0.0f, 0.0f));

        }//----- if_stop -----
        else if (this.transform.localScale.x > 0.0f)
        {
            horizontalRay = new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(1.0f, 0.0f, 0.0f));

        }//----- elseif_stop -----

        // 進行方向の当たり判定を格納
        RaycastHit horizontalRayHit;

        // 進行方向のレイの条件を指定しておく
        bool horizontalRayFlg = Physics.Raycast(horizontalRay, out horizontalRayHit, H_RayDistance, ground_Layer);

        // 頭上へ向かうレイの位置を更新する ----- 編集予定地( 宮﨑 )
        Ray[] upperRay = new Ray[2]
        {
            new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(0.0f, 1.0f, 0.0f)),
            new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(0.0f, 1.0f, 0.0f)),
        };

        // 頭上の当たり判定を格納
        RaycastHit[] upperRayHit = new RaycastHit[2]
        {
            new RaycastHit(),
            new RaycastHit(),
        };

        // 頭上へ向かうレイの条件を指定しておく
        bool[] upperRayFlg = new bool[2]
        {
            // 掴み用のレイ
            Physics.Raycast(upperRay[0], out upperRayHit[0], grab_RayDistance, ground_Layer),

            // 通過用のレイ
            Physics.Raycast(upperRay[1], out upperRayHit[1], platform_RayDistance, platform_Layer),
        };

        // 接地判定用のレイの位置を更新する -----
        Ray[,] underRay = new Ray[2, 3]
        {
            {// グラウンドレイヤー
                    new Ray(new Vector3(pl_Pos.x - betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x + betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),

            },
            {// プラットフォームレイヤー
                    new Ray(new Vector3(pl_Pos.x - betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x + betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
            },
        };

        // 当たり判定の格納
        // [0]:左 [1]:中 [2]:右
        RaycastHit[,] underRayHit = new RaycastHit[2, 3]
        {
            {// グラウンドレイ
                new RaycastHit(),
                new RaycastHit(),
                new RaycastHit(),
            },
            {// プラットフォームレイ
                new RaycastHit(),
                new RaycastHit(),
                new RaycastHit(),
            },
        };

        // 接地判定のレイの条件を指定しておく
        bool[,] underRayFlg = new bool[2, 3]
        {
            {// グラウンドレイヤー
                    Physics.Raycast(underRay[0,0], out underRayHit[0,0], ground_RayDistance, ground_Layer),
                    Physics.Raycast(underRay[0,1], out underRayHit[0,1], ground_RayDistance, ground_Layer),
                    Physics.Raycast(underRay[0,2], out underRayHit[0,2], ground_RayDistance, ground_Layer),
            },
            {// プラットフォームレイヤー
                    Physics.Raycast(underRay[1,0], out underRayHit[1,0], ground_RayDistance, platform_Layer),
                    Physics.Raycast(underRay[1,1], out underRayHit[1,1], ground_RayDistance, platform_Layer),
                    Physics.Raycast(underRay[1,2], out underRayHit[1,2], ground_RayDistance, platform_Layer),
            },
        };

        // --------------------------

#if UNITY_EDITOR
        // 進行方向のレイが設定された値に触れている間画面上に表示する
        // 進行方向に向けたレイを表示する
        if (horizontalRayFlg && rayDebug.lookRayHorizontal)
        {
            // レイをシーン上から見えるようにする
            Debug.DrawRay(horizontalRay.origin, horizontalRay.direction * H_RayDistance, Color.blue, 1, false);

        }//----- if_stop -----

        // 頭上に向けたレイを表示する
        if (upperRayFlg[0] && rayDebug.lookRayGrab)
        {
            // レイをシーン上から見えるようにする
            Debug.DrawRay(upperRay[0].origin, upperRay[0].direction * grab_RayDistance, Color.yellow, 1, false);

        }//----- if_stop -----

        // 通過用のレイを表示する
        if (upperRayFlg[1] && rayDebug.lookRayPlatform)
        {
            // レイをシーン上から見えるようにする
            Debug.DrawRay(upperRay[1].origin, upperRay[1].direction * platform_RayDistance, Color.yellow, 1, false);

        }//----- if_stop -----

        // 接地判定用のレイを表示する
        if (rayDebug.lookRayGround)
        {
            // 左,間,右しか検出しないので[ 3回 ]繰り返す
            for (int i = 0; i < 3; i++)
            {
                if (underRayFlg[0, i])
                {
                    Debug.DrawRay(underRay[0, i].origin, underRay[0, i].direction * ground_RayDistance, Color.green, 1, false);

                }//----- if_stop -----

                if (underRayFlg[1, i])
                {
                    Debug.DrawRay(underRay[1, i].origin, underRay[1, i].direction * ground_RayDistance, Color.gray, 1, false);

                }//----- if_stop -----

            }//----- for_stop -----

        }//----- if_stop -----
#endif

        // 以上,当たり判定の取得終了 ------------------------------

        //------------------------------
        // レイが触れた時の処理を実施
        //------------------------------
        // 壁判定の取得
        if (horizontalRayFlg)
        {
            H_RayWall = true;

        }//----- if_stop -----
        else
        {
            H_RayWall = false;

        }//----- else_stop -----

        if (upperRayFlg[0])
        {
            
            transform.SetParent(upperRayHit[0].transform.parent);
            if (groundColInsID != upperRayHit[0].colliderInstanceID)
            {
                groundLand = upperRayHit[0].transform.GetComponent<LandingGround>();
                groundColInsID = upperRayHit[0].colliderInstanceID;
            }

            underRayGrab = true;

        }//----- if_stop -----
        else
        {
            if(underRayGrab)
            {
                if (!inRayRide)
                {
                    pl_Audio.SeparateSound();
                }
                pl_Anim.SetBool("grabRereasing", true);
                ef_Manager.StopAura();
                
            }
            underRayGrab = false;
            

        }//----- else_stop -----

        // すり抜け判定の取得
        if (upperRayFlg[1])
        {
            inRayRide = true;
            if (upperRayHit[1].collider.gameObject.CompareTag("Floor"))
            {
                transform.SetParent(upperRayHit[1].transform.parent);
            }
            else if (upperRayHit[1].collider.transform.parent!=null&&
                upperRayHit[1].collider.transform.parent.CompareTag("Floor"))
            {

            }
            // すり抜けと掴み床は別判定なので
            // 掴まないようにする処理は必要ないかも -----

        }//----- if_stop -----

        

        if (underRayFlg[0, 1])
        {
            transform.SetParent(underRayHit[0, 1].transform.parent);
        }
        else if (underRayFlg[1, 1])
        {
            transform.SetParent(underRayHit[1, 1].transform.parent);
        }
        else if(!upperRayFlg[0]&&!upperRayFlg[1])
        {
            transform.SetParent(null);
            
        }
        // 接地判定を取得
        if (((underRayFlg[0, 0] && underRayFlg[0, 1]) || (underRayFlg[0, 1] && underRayFlg[0, 2])) ||
           ((underRayFlg[1, 0] && underRayFlg[1, 1]) || (underRayFlg[1, 1] && underRayFlg[1, 2])))
        {

            if(underRayFlg[0,1])
            {
                if(groundColInsID!=underRayHit[0,1].colliderInstanceID)
                {
                    groundLand = underRayHit[0, 1].transform.GetComponent<LandingGround>();
                    groundColInsID = underRayHit[0,1].colliderInstanceID;
                }
            }
            else if(underRayFlg[1,1])
            {
                if (groundColInsID != underRayHit[1, 1].colliderInstanceID)
                {
                    groundLand = underRayHit[1, 1].transform.GetComponent<LandingGround>();
                    groundColInsID = underRayHit[1, 1].colliderInstanceID;
                }
            }

            if (!onRayGround && nowJump)
            {
                
                if (underRayFlg[0, 1])
                {
                    Debug.Log("通常地面で波を発生させるぞ！");
                    
                    //groundLand = underRayHit[0, 1].transform.GetComponent<LandingGround>();
                    //Debug.Log(groundLand.GetInstanceID());
                    pl_Wave.WaveCreate(underRayHit[0, 1],
                                        Mathf.Abs(pl_Fall.fallPowLog * pl_Wave.waveHightIndex),
                                        underRayHit[0, 1].transform.position.y);

                }
                else if (underRayFlg[1, 1])
                {
                    //Debug.Log("すり抜け地面で波を発生させるぞ！");
                    //groundLand = underRayHit[1, 1].transform.GetComponent<LandingGround>();
                    pl_Wave.WaveCreate(underRayHit[1, 1],
                                        Mathf.Abs(pl_Fall.fallPowLog * pl_Wave.waveHightIndex),
                                        underRayHit[1, 1].transform.position.y);

                }
                // -----------------------この辺に着地アニメーション
                pl_Anim.SetBool("JumpEnding",true);

                nowJump = false;
            }

            //if(groundLand == null)
            //{
            //    if(underRayFlg[0,1])
            //    {
            //        groundLand = underRayHit[0, 1].collider.gameObject.GetComponent<LandingGround>();
            //    }
            //    else if(underRayFlg[1,1])
            //    {
            //        groundLand = underRayHit[1, 1].collider.gameObject.GetComponent<LandingGround>();
            //    }
            //}

            onRayGround = true;

            // 接地中なので掴まないようにする
            //underRayGrab = false;

            // 通過後なら着地
            if (inRayRide)
            {
                inRayRide = false;

            }//----- if_stop -----

        }//----- if_stop -----
        else if (!underRayFlg[0, 1] && !underRayFlg[1, 1])
        {
            onRayGround = false;
            pl_Jump.chargeNow = false;
            //if (groundLand != null)
            //{
            //    groundLand.nowCharge = false;
            //}
        }//----- elseif_stop -----

        if (grabCancelButton)
        {
            if (underRayGrab&&!grabReleaseFg)
            {
                
                pl_Audio.playerSource.Stop();
                pl_Audio.SeparateSound();
                
                pl_Anim.SetBool("grabRereasing", true);
                grabReleaseFg = true;
            }
            immovable = false;
            underRayGrab = false;
            
        }

        // 掴み状態なら
        if (underRayGrab && !inRayRide && !upperRayHit[0].collider.CompareTag("WaveEnd")
            && !upperRayHit[0].collider.CompareTag("UnableClimb") && (!upperRayHit[0].collider.CompareTag("NoGrab")))
        {
            if (!grabSetFg)
            {
                
                pl_Anim.SetBool("grabRereasing", false);
                pl_Anim.Play("grabStart");
                pl_Audio.playerSource.Stop();
                pl_Audio.GrabSound();
                grabSetFg = true;
                grabReleaseFg=false;
            }
            // ジャンプボタンが押された瞬間
            if (jumpButtonPressed)
            {
                jumpButtonPressed = false;

                // チャージ初期値を格納
                pl_Grab.chargePoint = pl_Grab.chargeStartPoint;

            }//----- if_stop -----

            

            // ジャンプボタンが離された瞬間
            if (jumpButtonReleased)
            {
                jumpButtonReleased = false;
                jumpButton = false;

                // 移動を可能にする
                immovable = false;
                // 落下処理をストップさせる
                StopCoroutine(pl_Fall.FallTheAfterJump());
                

                pl_Rb.velocity = new Vector3(pl_Rb.velocity.x, 0, pl_Rb.velocity.z);
                pl_Audio.playerSource.Stop();
                pl_Anim.Play("GrabJump");
                pl_Audio.PullSound();
                //pl_Anim.SetBool("grabJumping",true);
                // ジャンプ動作に移行する
                StartCoroutine(pl_Grab.ChargeJump(pl_Grab.chargePoint));
                

            }//----- if_stop -----

            // ジャンプボタンが押されている間
            if (jumpButton)
            {
                // 移動を不可能にする
                immovable = true;

                // チャージを開始する
                pl_Grab.ChargePower();

            }//----- if_stop -----
            else
            {
                // 移動を可能にする
                immovable = false;

                if (!jumpButtonReleased)
                {
                    // チャージ状態を解除する
                    pl_Grab.chargePoint = pl_Grab.chargeStartPoint;
                    pl_Grab.chargeNow = false;
                }

            }//----- else_stop -----

            if (!inRayRide)
            {
                // 移動不可にする
                pl_Rb.velocity = new Vector3(0, 0, pl_Rb.velocity.z);

            }//----- if_stop -----

        }//----- if_stop -----

        if (underRayGrab && upperRayHit[0].collider.CompareTag("UnableClimb"))
        {
            if (!grabSetFg)
            {

                pl_Anim.SetBool("grabRereasing", false);
                pl_Anim.Play("grabStart");
                pl_Audio.playerSource.Stop();
                pl_Audio.GrabSound();
                grabSetFg = true;
                grabReleaseFg = false;
            }
            if (jumpButtonPressed)
            {
                jumpButtonPressed = false;

            }//----- if_stop -----
            if (jumpButtonReleased)
            {
                jumpButtonReleased = false;

            }//----- if_stop -----

            if (!inRayRide)
            {
                // 移動不可にする
                pl_Rb.velocity = new Vector3(0, 0, pl_Rb.velocity.z);

            }//----- if_stop -----

        }//----- if_stop -----

        if (underRayGrab && (upperRayHit[0].collider.CompareTag("NoGrab") ||upperRayHit[0].collider.CompareTag("WaveEnd")))
        {
            if (jumpButtonPressed)
            {
                jumpButtonPressed = false;

            }//----- if_stop -----
            if (jumpButtonReleased)
            {
                jumpButtonReleased = false;

            }//----- if_stop -----
            underRayGrab = false;

        }//----- if_stop -----

        // 自身が地面に触れているなら
        if (onRayGround && !underRayGrab)
        {
            // 速度が変更されているなら
            if (moveSpeed != holdInitalValueSpeed)
            {
                // 自身の速度をもとに戻す
                moveSpeed = holdInitalValueSpeed;

            }//----- if_stop -----

            grabCancelButton = false;

            grabSetFg = false;

            // ジャンプボタンが離された瞬間
            if (jumpButtonReleased)
            {
                jumpButtonReleased = false;
                jumpButton = false;

            

                if (pl_Jump.changePoint <= pl_Jump.chargePoint)
                {
                    // 強ジャンプ状態
                    pl_Jump.changeJump = 1;
                    //if(pl_Anim.)
                    pl_Audio.playerSource.Stop();
                    pl_Anim.SetBool("strongJumping", true);
                    pl_Audio.StrongJumpSound();
                    ef_Manager.StopAura();
                }
                else
                {
                    
                    // 弱ジャンプ状態
                    pl_Anim.SetBool("weakJumping", true);
                    pl_Audio.playerSource.Stop();
                    pl_Audio.WeakJumpSound();
                    ef_Manager.StopAura();
                    pl_Jump.changeJump = 0;
                }

                // チャージ値を保持しておく ----- 編集予定地( 宮﨑 )
                pl_Jump.chargeLog = pl_Jump.chargePoint;

                // ジャンプ動作に移行する
                StartCoroutine(pl_Jump.ChargeJump(pl_Jump.jumpPow[pl_Jump.changeJump]));

            }//----- if_stop -----

            // ジャンプボタンが押されている間
            if (jumpButton)
            {
                // 移動を不可能にする
                immovable = true;

                // チャージを開始する
                pl_Jump.ChargePower();

            }//----- if_stop -----
            else
            {
                // 移動を可能にする
                immovable = false;
                if (nowJump)
                {
                    // チャージ状態を解除する
                    pl_Jump.chargePoint = 0;
                    pl_Jump.chargeNow = false;
                }
            }//----- else_stop -----

        }//----- if_stop -----
        else if (!onRayGround && !underRayGrab)
        {
            jumpButtonReleased = false;
            if (groundLand != null)
            {
                StartCoroutine(groundLand.ChargeEnd());
            }

            // ジャンプ中の移動速度でないなら
            if (moveSpeed != jumpSpeed)
            {
                // ジャンプ時の移動速度に変更する
                moveSpeed = jumpSpeed;

            }//----- if_stop -----
            // 落下を開始する
            StartCoroutine(pl_Fall.FallTheAfterJump());

        }//----- else_stop -----

        // 自身が壁に触れているなら
        if (H_RayWall)
        {
            // 移動不可にする
            pl_Rb.velocity = new Vector3(0, pl_Rb.velocity.y, pl_Rb.velocity.z);

        }//----- if_stop -----
        else
        {
            // 移動を行う
            pl_Rb.velocity = new Vector3(moveSpeed * H_MoveAxis, pl_Rb.velocity.y, pl_Rb.velocity.z);

        }//----- else_stop -----

        if (pl_Jump.chargeNow)
        {
            // 移動不可にする
            pl_Rb.velocity = new Vector3(0, pl_Rb.velocity.y, pl_Rb.velocity.z);

        }//----- if_stop -----
        if(pl_Grab.chargeNow)
        {
            // 移動不可にする
            pl_Rb.velocity = new Vector3(0, pl_Rb.velocity.y, pl_Rb.velocity.z);
        }

        // スティック入力がされているなら
        if (H_MoveAxis != 0)
        {
            // 自身の向きを変更する
            ChangePlayerDirection();

        }//----- if_stop -----
        else
        {
            pl_Anim.SetBool("running",false);
            if (pl_Audio.playerSource.clip == pl_Audio.playerWalkSound || pl_Audio.playerSource.clip == pl_Audio.playerGrabMoveSound)
            {
                pl_Audio.playerSource.Stop();
            }
        }

        if(StatusManager.coinCount>=RecoverCoin)
        {
            if(StatusManager.nowHitPoint<StatusManager.maxHitPoint)
            {
                StatusManager.coinCount-=RecoverCoin;
                StatusManager.nowHitPoint++;
            }
        }
    }

    //==================================================
    //          自身の向きを変更する
    // ※コントローラー情報によって変化します
    //==================================================
    // 作成日2023/05/01
    // 宮﨑
    private void ChangePlayerDirection()
    {
        // 移動アニメーションの追加 ---------- 前 Ver ではここにありました
        if (!pl_Grab.chargeNow&&!pl_Jump.chargeNow)
        {
            pl_Anim.SetBool("running", true);
            if (onRayGround && !underRayGrab && moveSpeed == holdInitalValueSpeed)
            {
                if (!pl_Audio.playerSource.isPlaying)
                {
                    pl_Audio.WalkSound();
                }
            }
            else if (!onRayGround && underRayGrab)
            {
                if (!pl_Audio.playerSource.isPlaying)
                {
                    pl_Audio.GrabMoveSound();
                }
            }
           
            
            Debug.Log(pl_Audio.playerSource.clip);
        }
        else
        {
            pl_Anim.SetBool("running",false );
        }
        

        // 自身が向いている方向の変更
        if (H_MoveAxis > 0.15f)
        {
            // 波の向きを右に
            pl_Wave.waveAngle = 1;
            // 右を向く
            this.transform.localScale = new Vector3(pl_Scale.x, pl_Scale.y, pl_Scale.z);

            // 波の方向を変更 ---------- 前 Ver ではここにありました

        }//----- if_stop -----
        else if (H_MoveAxis < -0.15f)
        {
            // 波の向きを左に
            pl_Wave.waveAngle = -1;
            // 左を向く
            this.transform.localScale = new Vector3(-pl_Scale.x, pl_Scale.y, pl_Scale.z);

            // 波の方向を変更 ---------- 前 Ver ではここにありました

        }//----- elseif_stop -----

    }

    //==================================================
    // オブジェクトを保存した地点に移動させる。
    // 引数無し、戻り値無し
    //==================================================
    // 制作日2023/04/03
    // 高田
    public IEnumerator Respawn()
    {
        isStop = 1; // あさわ
        pl_Rb.velocity = Vector3.zero;  // あさわ
        respawnManager.SetIsNowRespawning(true);  // あさわ
        yield return respawnManager.MakeSmallerImageScale();  // あさわ


        //yield return null;
        //AnimatorPlay(Dead,true);
        //StartColutine(DeadStay());
        nowJump = false;
        pl_Act.getDamage = true;
        pl_Anim.SetBool("JumpEnding", true);
        transform.position = respawnPosition;
        transform.localScale = pl_Scale;
        StatusManager.nowHitPoint = 3;
        pl_Rb.velocity = Vector3.zero;

        respawnManager.Reset();   // あさわ

        respawnManager.respawnCnt++;  // あさわ
        yield return StartCoroutine(respawnManager.MakeLargerImageScale());  // あさわ
        respawnManager.SetIsNowRespawning(false); // あさわ
        isStop = 0; // あさわ
    }

    //==================================================
    //          スタート開始時エラー
    // ※スタート時に取得できなかったものがあるとエラーを吐きます
    //==================================================
    // 作成日2023/04/28
    // 宮﨑
    private void GetError()
    {
        //[Rigidbody] が取得できなかった場合
        if (pl_Rb == null)
        {
            Debug.LogError("[pl_Rb]に[Rigidbody]が入っていません！");

        }//----- if_stop -----

        // [CapsuleCollider]が取得できなかった場合
        if (pl_Col == null)
        {
            Debug.LogError("[pl_Col]に[CapsuleCollider]が入っていません！");

        }//----- if_stop -----

        // [position]が取得できなかった場合
        if (pl_Pos == null)
        {
            Debug.LogError("[pl_Pos]に[position]が入っていません！");

        }//----- if_stop -----

        // [localScale]が取得できなかった場合
        if (pl_Scale == null)
        {
            Debug.LogError("[pl_Scale]に[localScale]が入っていません！");

        }//----- if_stop -----

        // [Animator]が取得できなかった場合
        if (pl_Anim == null)
        {
            Debug.LogError("[Animator]が見つかりません");
        }//-----if_stop-----

        // [Player_Ride]が取得できなかった場合
        if (pl_Ride == null)
        {
            Debug.LogError("[pl_Jumpに[Player_Ride]が入っていません！");

        }//----- if_stop -----

        // [Player_Grab]が取得できなかった場合
        if (pl_Grab == null)
        {
            Debug.LogError("[pl_Jumpに[Player_Grab]が入っていません！");

        }//----- if_stop -----

        // [Player_Jump]が取得できなかった場合
        if (pl_Jump == null)
        {
            Debug.LogError("[pl_Jumpに[Player_Jump]が入っていません！");

        }//----- if_stop -----

        // [Player_Fall]が取得できなかった場合
        if (pl_Fall == null)
        {
            Debug.LogError("[pl_Jumpに[Player_Fall]が入っていません！");

        }//----- if_stop -----

        if (pl_Wave == null)
        {
            Debug.LogError("[pl_Wave]に[Player_Wave]が入っていません！");
        }

        if (pl_Audio == null)
        {
            Debug.LogError("[pl_Audio]に[PlayerAudio]が入っていません！");
        }
    }
}
