using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//==================================================
//          ���C���X�N���v�g    ver4.1
// ���v���C���[�ɂ��������������ׂĕ��U�A���ς���܂���
// ������̓v���C���[�̋����𐧌䂷��A���C���̃X�N���v�g�ł�
// �����ۂ̍s���Ȃǂ͕ʃX�N���v�g����擾���Ă�������
//==================================================
// �쐬��2023/04/28    �X�V��2023/05/14
// �{��
public class Player_Main : MonoBehaviour
{
    [System.Serializable]
    public struct RayDebug
    {
        [Header("�\���ݒ�")]
        // �i�s�����Ɍ��������C
        public bool lookRayHorizontal;
        // ����Ɍ��������C
        public bool lookRayGrab;
        // �ʉߗp�̃��C
        public bool lookRayPlatform;
        // �ڒn�֌W�̃��C
        public bool lookRayGround;
    };

    [Tooltip("���C�̕\���ݒ�")]
    [SerializeField] private RayDebug rayDebug;

    // ���̂̐��� --------------------
    // [ pl ] = player
    // [ Pos ] = position
    // [ H ] = horizontal
    // [ V ] = vertical
    // [ flg ] = flag
    //--------------------------------

    // �Q�[���p�b�h�̓��͏����擾����
    float H_MoveAxis = 0.5f;    // L�X�e�B�b�N
    bool jumpButton = false;    // �W�����v�{�^��

    // ���g�̏����擾����
    private Rigidbody pl_Rb;
    public CapsuleCollider pl_Col;
    private Vector3 pl_Pos;
    private Vector3 pl_Scale;
    [System.NonSerialized] public Animator pl_Anim;
    private DamageAct pl_Act;

    // ���g�̋��������擾����
    private Player_Ride pl_Ride;    // ��ꎞ�̋������
    private Player_Grab pl_Grab;    // �͂ݎ��̋������
    private Player_Jump pl_Jump;    // ���􎞂̋������
    private Player_Fall pl_Fall;    // �������̋������
    private Player_Wave pl_Wave;    // �g�����̋������


    // �v���C���[��������O�����
    public PlayerAudio pl_Audio;     // �v���C���[�̃T�E���h�Đ��p�֐�
    // private CameraVibration scrennVibe;  // �J������h�炷�X�N���v�g
    public LandingGround groundLand;
    public PlayerEffectManager ef_Manager;

    bool jumpButtonPressed;
    bool jumpButtonReleased;

    private bool grabCancelButton;

    // �R���g���[���[�̑�����~�߂�
    public int isStop;

    // ���C���[�̐ݒ�
    [Header("----- ���C���[�̐ݒ� -----"), Space(5)]
    [SerializeField] private LayerMask ground_Layer = 1 << 6;      // �n�ʂ̃��C���[
    [SerializeField] private LayerMask platform_Layer = 1 << 8;    // ���蔲�����̃��C���[
                                                                   // ���C�̋���
    [Header("���C�̋���")]

    // �ǔ�����Ƃ邽�߂̋���
    [Tooltip("�ǔ�����擾���鋗��")]
    [SerializeField] private float H_RayDistance = 1.0f;        // �ǔ��肪����ʒu

    // �݂͂��\�ȋ���
    [Tooltip("�݂͂��\�ȋ���")]
    [SerializeField] private float grab_RayDistance = 1.05f;    // �͂݉\�n�_�܂ł̋���

    // �ʉ߂��\�ȋ���
    [Tooltip("�ʉ߂��\�ɂ��鋗��")]
    [SerializeField] private float platform_RayDistance = 1.1f; // �ʉ߉\�J�n�n�_�܂ł̋���

    // �ڒn��ԂɂȂ�܂ł̋���
    [Tooltip("�W�����v�\����")]
    [SerializeField] private float ground_RayDistance = 0.35f;  // �W�����v�\�n�_�܂ł̋���
    [Tooltip("�t�@�W�[�\����")]
    [SerializeField] private float fuzzy_RayDistance = 2.75f;   // �t�@�W�[�\�n�_�܂ł̋���

    // ���C�̈ʒu
    [Header("���C�̈ʒu")]

    // �ڒn����𕡐���邽�߂̃��C�̈ʒu
    [Tooltip("�ڒn������o���ʒu�̐ݒ�")]
    [SerializeField] private float ground_RayPos = 0.75f;       // �ڒn����o���ʒu
    [Tooltip("�ڒn����̃��C�Ԃ̈ʒu�ݒ�")]
    [SerializeField] private float betweenRayPos = 0.825f;      // �ڒn����Ԃ̋���


    // �ړ��̐ݒ�
    [Header("----- �ړ��̐ݒ� -----"), Space(5)]

    [Tooltip("�ʏ�ړ����x")]
    public float moveSpeed = 13f;       // �n��̈ړ����x
    [System.NonSerialized]
    public float holdInitalValueSpeed;  // �ړ����x�̏����l���i�[����
    [Tooltip("�󒆂ł̈ړ����x")]
    public float jumpSpeed;             // �󒆑��x

    [Tooltip("�ړ��s�̏�Ԃɂ��邩")]
    public bool immovable = false;

    // �e����̏��
    public bool H_RayWall;      // �ǂ̔���
    public bool inRayRide;      // ��ꔻ��
    public bool underRayGrab;   // �͂ݔ���
    public bool onRayGround;    // �ڒn����
    public bool nowJump;        // ��������

    private bool grabSetFg = false;
    private bool grabReleaseFg = false;

    [Tooltip("���X�|�[���n�_�̐ݒ�")]
    [SerializeField] private Vector3 respawnPosition;
    [Header("�R�C���ɂ��HP�񕜐ݒ�")]
    [Tooltip("�R�C��������HP���񕜂��邩")]
    [SerializeField] private byte RecoverCoin = 30;

    // ������
    [Tooltip("���X�|�[���}�l�[�W���[�ݒ�")]
    [SerializeField] private CheckPointManager respawnManager;

    //private enum CameraVibe
    //{
    //    ON,
    //    OFF,
    //}

    //[Header("�J������U�������邩�ۂ�")]
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
        // ���g�̊e�����擾����
        pl_Rb = this.GetComponent<Rigidbody>();
        pl_Col = this.GetComponent<CapsuleCollider>();
        pl_Pos = this.transform.position;
        pl_Scale = this.transform.localScale;
        pl_Anim = this.GetComponent<Animator>();
        pl_Act = this.GetComponent<DamageAct>();

        // ���g�̊e�������i�[
        pl_Grab = this.GetComponentInChildren<Player_Grab>();
        pl_Ride = this.GetComponentInChildren<Player_Ride>();
        pl_Jump = this.GetComponentInChildren<Player_Jump>();
        pl_Fall = this.GetComponentInChildren<Player_Fall>();
        pl_Wave = this.GetComponentInChildren<Player_Wave>();

        // �O�����X�N���v�g���i�[
        pl_Audio = GetComponent<PlayerAudio>();
        ef_Manager = this.GetComponentInChildren<PlayerEffectManager>();
        //// �J������U��������悤�ɂ��Ă���Ȃ�
        //if (cameraVibe == CameraVibe.ON)
        //{
        //    // �J�����U���X�N���v�g���i�[
        //    scrennVibe = GameObject.FindWithTag("MainCamera").GetComponent<CameraVibration>();
        //    // �Ȃ����G���[
        //    if (scrennVibe == null)
        //    {
        //        Debug.LogError("[CameraVibration]��������܂���");

        //    }//-----if_stop-----
        //}//-----if_stop-----
        // ��񂪈�ł������Ă��Ȃ��̂Ȃ�
        if (!pl_Rb || !pl_Col || pl_Pos == null || pl_Scale == null || !pl_Anim ||
            !pl_Grab || !pl_Ride || !pl_Jump || !pl_Fall || !pl_Wave ||
            !pl_Audio)
        {
            // �G���[�֐����Ă�
            GetError();

        }//----- if_stop -----

        respawnPosition = transform.position;

        // ���g�̈ړ����x�̏����l���i�[���Ă���
        holdInitalValueSpeed = moveSpeed;

        // respawnManager�ݒ�
        respawnManager = GameObject.Find("RespawnManager").GetComponent<CheckPointManager>(); // ������
        if(respawnManager==null)
        {
            Debug.LogError("���X�|�[���}�l�[�W���[��������܂���");
        }
    }

    private void Update()
    {
        if (isStop != 0) { return; }

        // �Q�[���p�b�h���ڑ�����Ă��Ȃ����null
        if (Gamepad.current == null) return;

        // ���͏��̎擾 ---------------

        // �W�����v����
        // �����ꂽ�u�Ԃ̏����擾
        if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        {
            jumpButtonPressed = true;
        }

        // �����ꂽ�u�Ԃ̏����擾
        if (Gamepad.current.buttonSouth.wasReleasedThisFrame)
        {
            jumpButtonReleased = true;
        }

        // �͂ݗ����{�^��
        if (Gamepad.current.buttonEast.wasPressedThisFrame)
        {
            grabCancelButton = true;
        }

        // ������Ă���Ԃ̏�Ԃ��擾
        jumpButton = Gamepad.current.buttonSouth.isPressed;

        //if (Gamepad.current.buttonSouth.wasPressedThisFrame)
        //{
        //    jumpButton = true;

        //}//----- if_stop -----
        //// �W�����v�{�^���������ꂽ�u��
        //else if (Gamepad.current.buttonSouth.wasReleasedThisFrame)
        //{
        //    jumpButton = false;

        //}//----- elseif_stop -----
    }

    private void FixedUpdate()
    {
        // �������~�߂�
        if(isStop != 0) { return; }

        // ���g�̍��W���擾��������
        pl_Pos = this.transform.position;

        // �ړ����͂̕������m�F
        H_MoveAxis = Input.GetAxisRaw("Horizontal");

        // ���C�̍X�V ---------------
        // �i�s�����֌��������C�̈ʒu���X�V���� -------------------
        Ray horizontalRay = new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(0.0f, 0.0f, 0.0f));
        if (this.transform.localScale.x < 0.0f)
        {
            horizontalRay = new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(-1.0f, 0.0f, 0.0f));

        }//----- if_stop -----
        else if (this.transform.localScale.x > 0.0f)
        {
            horizontalRay = new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(1.0f, 0.0f, 0.0f));

        }//----- elseif_stop -----

        // �i�s�����̓����蔻����i�[
        RaycastHit horizontalRayHit;

        // �i�s�����̃��C�̏������w�肵�Ă���
        bool horizontalRayFlg = Physics.Raycast(horizontalRay, out horizontalRayHit, H_RayDistance, ground_Layer);

        // ����֌��������C�̈ʒu���X�V���� ----- �ҏW�\��n( �{�� )
        Ray[] upperRay = new Ray[2]
        {
            new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(0.0f, 1.0f, 0.0f)),
            new Ray(new Vector3(pl_Pos.x, pl_Pos.y, pl_Pos.z), new Vector3(0.0f, 1.0f, 0.0f)),
        };

        // ����̓����蔻����i�[
        RaycastHit[] upperRayHit = new RaycastHit[2]
        {
            new RaycastHit(),
            new RaycastHit(),
        };

        // ����֌��������C�̏������w�肵�Ă���
        bool[] upperRayFlg = new bool[2]
        {
            // �͂ݗp�̃��C
            Physics.Raycast(upperRay[0], out upperRayHit[0], grab_RayDistance, ground_Layer),

            // �ʉߗp�̃��C
            Physics.Raycast(upperRay[1], out upperRayHit[1], platform_RayDistance, platform_Layer),
        };

        // �ڒn����p�̃��C�̈ʒu���X�V���� -----
        Ray[,] underRay = new Ray[2, 3]
        {
            {// �O���E���h���C���[
                    new Ray(new Vector3(pl_Pos.x - betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x + betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),

            },
            {// �v���b�g�t�H�[�����C���[
                    new Ray(new Vector3(pl_Pos.x - betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
                    new Ray(new Vector3(pl_Pos.x + betweenRayPos, pl_Pos.y - ground_RayPos, pl_Pos.z), new Vector3(0.0f, -1.0f, 0.0f)),
            },
        };

        // �����蔻��̊i�[
        // [0]:�� [1]:�� [2]:�E
        RaycastHit[,] underRayHit = new RaycastHit[2, 3]
        {
            {// �O���E���h���C
                new RaycastHit(),
                new RaycastHit(),
                new RaycastHit(),
            },
            {// �v���b�g�t�H�[�����C
                new RaycastHit(),
                new RaycastHit(),
                new RaycastHit(),
            },
        };

        // �ڒn����̃��C�̏������w�肵�Ă���
        bool[,] underRayFlg = new bool[2, 3]
        {
            {// �O���E���h���C���[
                    Physics.Raycast(underRay[0,0], out underRayHit[0,0], ground_RayDistance, ground_Layer),
                    Physics.Raycast(underRay[0,1], out underRayHit[0,1], ground_RayDistance, ground_Layer),
                    Physics.Raycast(underRay[0,2], out underRayHit[0,2], ground_RayDistance, ground_Layer),
            },
            {// �v���b�g�t�H�[�����C���[
                    Physics.Raycast(underRay[1,0], out underRayHit[1,0], ground_RayDistance, platform_Layer),
                    Physics.Raycast(underRay[1,1], out underRayHit[1,1], ground_RayDistance, platform_Layer),
                    Physics.Raycast(underRay[1,2], out underRayHit[1,2], ground_RayDistance, platform_Layer),
            },
        };

        // --------------------------

#if UNITY_EDITOR
        // �i�s�����̃��C���ݒ肳�ꂽ�l�ɐG��Ă���ԉ�ʏ�ɕ\������
        // �i�s�����Ɍ��������C��\������
        if (horizontalRayFlg && rayDebug.lookRayHorizontal)
        {
            // ���C���V�[���ォ�猩����悤�ɂ���
            Debug.DrawRay(horizontalRay.origin, horizontalRay.direction * H_RayDistance, Color.blue, 1, false);

        }//----- if_stop -----

        // ����Ɍ��������C��\������
        if (upperRayFlg[0] && rayDebug.lookRayGrab)
        {
            // ���C���V�[���ォ�猩����悤�ɂ���
            Debug.DrawRay(upperRay[0].origin, upperRay[0].direction * grab_RayDistance, Color.yellow, 1, false);

        }//----- if_stop -----

        // �ʉߗp�̃��C��\������
        if (upperRayFlg[1] && rayDebug.lookRayPlatform)
        {
            // ���C���V�[���ォ�猩����悤�ɂ���
            Debug.DrawRay(upperRay[1].origin, upperRay[1].direction * platform_RayDistance, Color.yellow, 1, false);

        }//----- if_stop -----

        // �ڒn����p�̃��C��\������
        if (rayDebug.lookRayGround)
        {
            // ��,��,�E�������o���Ȃ��̂�[ 3�� ]�J��Ԃ�
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

        // �ȏ�,�����蔻��̎擾�I�� ------------------------------

        //------------------------------
        // ���C���G�ꂽ���̏��������{
        //------------------------------
        // �ǔ���̎擾
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

        // ���蔲������̎擾
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
            // ���蔲���ƒ͂ݏ��͕ʔ���Ȃ̂�
            // �͂܂Ȃ��悤�ɂ��鏈���͕K�v�Ȃ����� -----

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
        // �ڒn������擾
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
                    Debug.Log("�ʏ�n�ʂŔg�𔭐������邼�I");
                    
                    //groundLand = underRayHit[0, 1].transform.GetComponent<LandingGround>();
                    //Debug.Log(groundLand.GetInstanceID());
                    pl_Wave.WaveCreate(underRayHit[0, 1],
                                        Mathf.Abs(pl_Fall.fallPowLog * pl_Wave.waveHightIndex),
                                        underRayHit[0, 1].transform.position.y);

                }
                else if (underRayFlg[1, 1])
                {
                    //Debug.Log("���蔲���n�ʂŔg�𔭐������邼�I");
                    //groundLand = underRayHit[1, 1].transform.GetComponent<LandingGround>();
                    pl_Wave.WaveCreate(underRayHit[1, 1],
                                        Mathf.Abs(pl_Fall.fallPowLog * pl_Wave.waveHightIndex),
                                        underRayHit[1, 1].transform.position.y);

                }
                // -----------------------���̕ӂɒ��n�A�j���[�V����
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

            // �ڒn���Ȃ̂Œ͂܂Ȃ��悤�ɂ���
            //underRayGrab = false;

            // �ʉߌ�Ȃ璅�n
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

        // �͂ݏ�ԂȂ�
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
            // �W�����v�{�^���������ꂽ�u��
            if (jumpButtonPressed)
            {
                jumpButtonPressed = false;

                // �`���[�W�����l���i�[
                pl_Grab.chargePoint = pl_Grab.chargeStartPoint;

            }//----- if_stop -----

            

            // �W�����v�{�^���������ꂽ�u��
            if (jumpButtonReleased)
            {
                jumpButtonReleased = false;
                jumpButton = false;

                // �ړ����\�ɂ���
                immovable = false;
                // �����������X�g�b�v������
                StopCoroutine(pl_Fall.FallTheAfterJump());
                

                pl_Rb.velocity = new Vector3(pl_Rb.velocity.x, 0, pl_Rb.velocity.z);
                pl_Audio.playerSource.Stop();
                pl_Anim.Play("GrabJump");
                pl_Audio.PullSound();
                //pl_Anim.SetBool("grabJumping",true);
                // �W�����v����Ɉڍs����
                StartCoroutine(pl_Grab.ChargeJump(pl_Grab.chargePoint));
                

            }//----- if_stop -----

            // �W�����v�{�^����������Ă����
            if (jumpButton)
            {
                // �ړ���s�\�ɂ���
                immovable = true;

                // �`���[�W���J�n����
                pl_Grab.ChargePower();

            }//----- if_stop -----
            else
            {
                // �ړ����\�ɂ���
                immovable = false;

                if (!jumpButtonReleased)
                {
                    // �`���[�W��Ԃ���������
                    pl_Grab.chargePoint = pl_Grab.chargeStartPoint;
                    pl_Grab.chargeNow = false;
                }

            }//----- else_stop -----

            if (!inRayRide)
            {
                // �ړ��s�ɂ���
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
                // �ړ��s�ɂ���
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

        // ���g���n�ʂɐG��Ă���Ȃ�
        if (onRayGround && !underRayGrab)
        {
            // ���x���ύX����Ă���Ȃ�
            if (moveSpeed != holdInitalValueSpeed)
            {
                // ���g�̑��x�����Ƃɖ߂�
                moveSpeed = holdInitalValueSpeed;

            }//----- if_stop -----

            grabCancelButton = false;

            grabSetFg = false;

            // �W�����v�{�^���������ꂽ�u��
            if (jumpButtonReleased)
            {
                jumpButtonReleased = false;
                jumpButton = false;

            

                if (pl_Jump.changePoint <= pl_Jump.chargePoint)
                {
                    // ���W�����v���
                    pl_Jump.changeJump = 1;
                    //if(pl_Anim.)
                    pl_Audio.playerSource.Stop();
                    pl_Anim.SetBool("strongJumping", true);
                    pl_Audio.StrongJumpSound();
                    ef_Manager.StopAura();
                }
                else
                {
                    
                    // ��W�����v���
                    pl_Anim.SetBool("weakJumping", true);
                    pl_Audio.playerSource.Stop();
                    pl_Audio.WeakJumpSound();
                    ef_Manager.StopAura();
                    pl_Jump.changeJump = 0;
                }

                // �`���[�W�l��ێ����Ă��� ----- �ҏW�\��n( �{�� )
                pl_Jump.chargeLog = pl_Jump.chargePoint;

                // �W�����v����Ɉڍs����
                StartCoroutine(pl_Jump.ChargeJump(pl_Jump.jumpPow[pl_Jump.changeJump]));

            }//----- if_stop -----

            // �W�����v�{�^����������Ă����
            if (jumpButton)
            {
                // �ړ���s�\�ɂ���
                immovable = true;

                // �`���[�W���J�n����
                pl_Jump.ChargePower();

            }//----- if_stop -----
            else
            {
                // �ړ����\�ɂ���
                immovable = false;
                if (nowJump)
                {
                    // �`���[�W��Ԃ���������
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

            // �W�����v���̈ړ����x�łȂ��Ȃ�
            if (moveSpeed != jumpSpeed)
            {
                // �W�����v���̈ړ����x�ɕύX����
                moveSpeed = jumpSpeed;

            }//----- if_stop -----
            // �������J�n����
            StartCoroutine(pl_Fall.FallTheAfterJump());

        }//----- else_stop -----

        // ���g���ǂɐG��Ă���Ȃ�
        if (H_RayWall)
        {
            // �ړ��s�ɂ���
            pl_Rb.velocity = new Vector3(0, pl_Rb.velocity.y, pl_Rb.velocity.z);

        }//----- if_stop -----
        else
        {
            // �ړ����s��
            pl_Rb.velocity = new Vector3(moveSpeed * H_MoveAxis, pl_Rb.velocity.y, pl_Rb.velocity.z);

        }//----- else_stop -----

        if (pl_Jump.chargeNow)
        {
            // �ړ��s�ɂ���
            pl_Rb.velocity = new Vector3(0, pl_Rb.velocity.y, pl_Rb.velocity.z);

        }//----- if_stop -----
        if(pl_Grab.chargeNow)
        {
            // �ړ��s�ɂ���
            pl_Rb.velocity = new Vector3(0, pl_Rb.velocity.y, pl_Rb.velocity.z);
        }

        // �X�e�B�b�N���͂�����Ă���Ȃ�
        if (H_MoveAxis != 0)
        {
            // ���g�̌�����ύX����
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
    //          ���g�̌�����ύX����
    // ���R���g���[���[���ɂ���ĕω����܂�
    //==================================================
    // �쐬��2023/05/01
    // �{��
    private void ChangePlayerDirection()
    {
        // �ړ��A�j���[�V�����̒ǉ� ---------- �O Ver �ł͂����ɂ���܂���
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
        

        // ���g�������Ă�������̕ύX
        if (H_MoveAxis > 0.15f)
        {
            // �g�̌������E��
            pl_Wave.waveAngle = 1;
            // �E������
            this.transform.localScale = new Vector3(pl_Scale.x, pl_Scale.y, pl_Scale.z);

            // �g�̕�����ύX ---------- �O Ver �ł͂����ɂ���܂���

        }//----- if_stop -----
        else if (H_MoveAxis < -0.15f)
        {
            // �g�̌���������
            pl_Wave.waveAngle = -1;
            // ��������
            this.transform.localScale = new Vector3(-pl_Scale.x, pl_Scale.y, pl_Scale.z);

            // �g�̕�����ύX ---------- �O Ver �ł͂����ɂ���܂���

        }//----- elseif_stop -----

    }

    //==================================================
    // �I�u�W�F�N�g��ۑ������n�_�Ɉړ�������B
    // ���������A�߂�l����
    //==================================================
    // �����2023/04/03
    // ���c
    public IEnumerator Respawn()
    {
        isStop = 1; // ������
        pl_Rb.velocity = Vector3.zero;  // ������
        respawnManager.SetIsNowRespawning(true);  // ������
        yield return respawnManager.MakeSmallerImageScale();  // ������


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

        respawnManager.Reset();   // ������

        respawnManager.respawnCnt++;  // ������
        yield return StartCoroutine(respawnManager.MakeLargerImageScale());  // ������
        respawnManager.SetIsNowRespawning(false); // ������
        isStop = 0; // ������
    }

    //==================================================
    //          �X�^�[�g�J�n���G���[
    // ���X�^�[�g���Ɏ擾�ł��Ȃ��������̂�����ƃG���[��f���܂�
    //==================================================
    // �쐬��2023/04/28
    // �{��
    private void GetError()
    {
        //[Rigidbody] ���擾�ł��Ȃ������ꍇ
        if (pl_Rb == null)
        {
            Debug.LogError("[pl_Rb]��[Rigidbody]�������Ă��܂���I");

        }//----- if_stop -----

        // [CapsuleCollider]���擾�ł��Ȃ������ꍇ
        if (pl_Col == null)
        {
            Debug.LogError("[pl_Col]��[CapsuleCollider]�������Ă��܂���I");

        }//----- if_stop -----

        // [position]���擾�ł��Ȃ������ꍇ
        if (pl_Pos == null)
        {
            Debug.LogError("[pl_Pos]��[position]�������Ă��܂���I");

        }//----- if_stop -----

        // [localScale]���擾�ł��Ȃ������ꍇ
        if (pl_Scale == null)
        {
            Debug.LogError("[pl_Scale]��[localScale]�������Ă��܂���I");

        }//----- if_stop -----

        // [Animator]���擾�ł��Ȃ������ꍇ
        if (pl_Anim == null)
        {
            Debug.LogError("[Animator]��������܂���");
        }//-----if_stop-----

        // [Player_Ride]���擾�ł��Ȃ������ꍇ
        if (pl_Ride == null)
        {
            Debug.LogError("[pl_Jump��[Player_Ride]�������Ă��܂���I");

        }//----- if_stop -----

        // [Player_Grab]���擾�ł��Ȃ������ꍇ
        if (pl_Grab == null)
        {
            Debug.LogError("[pl_Jump��[Player_Grab]�������Ă��܂���I");

        }//----- if_stop -----

        // [Player_Jump]���擾�ł��Ȃ������ꍇ
        if (pl_Jump == null)
        {
            Debug.LogError("[pl_Jump��[Player_Jump]�������Ă��܂���I");

        }//----- if_stop -----

        // [Player_Fall]���擾�ł��Ȃ������ꍇ
        if (pl_Fall == null)
        {
            Debug.LogError("[pl_Jump��[Player_Fall]�������Ă��܂���I");

        }//----- if_stop -----

        if (pl_Wave == null)
        {
            Debug.LogError("[pl_Wave]��[Player_Wave]�������Ă��܂���I");
        }

        if (pl_Audio == null)
        {
            Debug.LogError("[pl_Audio]��[PlayerAudio]�������Ă��܂���I");
        }
    }
}
