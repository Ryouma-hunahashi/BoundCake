using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UI;

public class Bossdirection : MonoBehaviour
{
    // �{�X�̏��ɂ��ǂ�����Ƃ��ɋN���鏈��
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

    // �X�e�[�W�͈̔͂ɓ�������c�������������ă{�X�X�e�[�W����łȂ�������
    [SerializeField] private GameObject leftwallobj;
    [SerializeField] private GameObject rightwallobj;
    // �{�X�̉��o����UI���ꎞ�I�ɏ�������
    [SerializeField]private Canvas mainUi;
    // �{�X�X�e�[�W�ɓ������Ƃ��ɏ����O�g�����Ԃ��Ŗ��߂�
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

        // ����ɓ�����
        movecheck = true;
        playerstop = true;
        // �{�X�s���J�n
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

        // ���݂̕ǂ̍��W����
        dif_wall.left = leftwallobj.transform.position;
        dif_wall.right = rightwallobj.transform.position;
        
        // �ǂ̈ʒu�ɕǂ����낷��
        dif_wall.left.y -= des_y;
        dif_wall.right.y -= des_y;

        updownUi.left = topImage.transform.position;
        updownUi.right = downImage.transform.position;
        def_posUi.left = topImage.transform.position;
        def_posUi.right = downImage.transform.position;

        updownUi.left.y -= 150;
        updownUi.right.y += 150;

        // �{�X�X�e�[�W�ɓ���܂�Ui��������������Ă���
        bossUi.enabled = false;

        // �ŏ��̈ʒu���擾����
        now_wall.left = leftwallobj.transform.position;
        now_wall.right = rightwallobj.transform.position;

        playerstop = true;
        movecheck = false;

    }
    
    // Update is called once per frame
    void Update()
    {
        // �v���C���[������ʒu�ɍs�����ꍇ
           
        //�v���C���[���G�ꂽ���ǂ����𔻒肷��

        if(playercheck)
        {

            bossleftcheck = false;

            if(!bossleftcheck&&movecheck)
            {
                leftwallobj.gameObject.transform.position = Vector3.MoveTowards(leftwallobj.transform.position, dif_wall.left, blocktime * Time.deltaTime);

            }

            // ���o���J�n����
            if (!movecheck)
            {
                // �v���C���[�̏�������������~�߂�
                player.isStop = 1;

                //player.H_MoveAxis = 0;
                

                // �{�X�p���o��UI��\������
                bossUi.enabled = true;

                rightwallobj.gameObject.transform.position = Vector3.MoveTowards(rightwallobj.transform.position, dif_wall.right, blocktime * Time.deltaTime);
                leftwallobj.gameObject.transform.position = Vector3.MoveTowards(leftwallobj.transform.position, dif_wall.left, blocktime * Time.deltaTime);

                Debug.Log("���ǂ��낵�܁[��");

                // UI���ړ�������
                Debug.Log("UI�ړ���");
                topImage.transform.position = Vector3.MoveTowards(topImage.transform.position, updownUi.left, blocktime);
                downImage.transform.position = Vector3.MoveTowards(downImage.transform.position, updownUi.right, blocktime);


                Debug.Log("main��UI������[");
                mainUi.enabled = false;

                // �ړ����I�������
                if ((rightwallobj.transform.position == dif_wall.right && leftwallobj.transform.position == dif_wall.left)
                    &&(topImage.transform.position == updownUi.left && downImage.transform.position == updownUi.right))
                {

                    StartCoroutine(Stopplayer());
                }
            }

            // �ړ�������������
            if(movecheck)
            {
                topImage.transform.position = Vector3.MoveTowards(topImage.transform.position, def_posUi.left, blocktime);
                downImage.transform.position = Vector3.MoveTowards(downImage.transform.position, def_posUi.right, blocktime);

                Debug.Log("UI�߂���[");
                mainUi.enabled = true;

                Debug.Log("����ɂ������Ă��[�[�[");
            }
        }

        if (playerstop)
        {
            if (this.gameObject.transform.position.x <= player_obj.transform.position.x)
            {
                // �v���C���[�𑀍�\��Ԃɂ���
                player.isStop = 0;

                // �v���C���[�̍s�����~������
                playerstop = false;
            }
        }

        if(bossleftcheck)
        {
            // �{�X�X�e�[�W�ł���Ă������ǂ����Ƃ��ɕK�v
            leftwallobj.gameObject.transform.position = Vector3.MoveTowards(leftwallobj.transform.position, now_wall.left, blocktime * Time.deltaTime);
        }

        if(bossrightcheck)
        {
            // �{�X�����ꂽ�Ƃ�
            rightwallobj.gameObject.transform.position = Vector3.MoveTowards(rightwallobj.transform.position, now_wall.right, blocktime * Time.deltaTime);
            
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        // �v���C���[���{�X�X�e�[�W�ɓ�������
        if(other.gameObject.tag == "Player")
        {
            // ���[�r�[�𗬂�
            playercheck = true;

            bgm.BossBattleStart();

        }
    }

    public void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            // �{�X�G���A���痣�ꂽ��

            // ���o���Ȃ���
            playercheck = false;
           
            bossleftcheck = true;

            bgm.FieldBGMStart();

            Debug.Log("�������[�H");
        }
    }

}


