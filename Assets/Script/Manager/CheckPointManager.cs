using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

//============================
// ������
// ���X�|�[���ɔ��������܂Ƃ�
//============================



public class CheckPointManager : MonoBehaviour
{
    private Vector3 playerPosition;
    public int respawnCnt = 0;
    const float MAX_IMAGE_SCALE = 25.0f;

    [SerializeField] private float nowImageScale = MAX_IMAGE_SCALE;
    [SerializeField] private GameObject[] enemies;
    [SerializeField] private Vector3[] enemiesPos;
    [SerializeField] private List<ItemDeta> l_cookies = new List<ItemDeta>();
    [SerializeField] private GameObject boss;
    [SerializeField] private Vector3 bossPosition;

    [SerializeField] private RectTransform image;
    [SerializeField] private Vector3 respawnpoint;
    [SerializeField] static public bool isNowRespawning;

    [SerializeField] private Transform playerTrans;

    // Start is called before the first frame update
    void Start()
    {
        // �V�[����̓G���擾
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        Debug.Log("���̃V�[���̃G�l�~�[�̐��F"+enemies.Length);

        // �G�̏����ʒu��ۑ�
        enemiesPos = new Vector3[enemies.Length];
        for (int i = 0; i < enemies.Length; i++)
        {
            enemiesPos[i] = enemies[i].transform.position;
        }

        // �{�X�̏����l��ۑ�
        if(boss != null)
        {
            bossPosition = boss.transform.position;
        }

        // �V�[����̃N�b�L�[���擾
        var cookiesBuf = GameObject.FindGameObjectsWithTag("Coin");
        for(byte i = 0; i < cookiesBuf.Length; i++)
        {
            l_cookies.Add(cookiesBuf[i].GetComponent<ItemDeta>());
        }
        

        Debug.Log("���̃V�[���̃N�b�L�[�̐��F" + l_cookies.Count);

        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;
        if(playerTrans == null)
        {
            Debug.LogError("�v���C���[��������܂���");
        }

        

        // isNowRespawning������
        isNowRespawning = false;
    }


    //Update is called once per frame
    void Update()
    {
        // �����[�v�Ö��C���[�W���v���C���[�ɋz��������
        InitImage();
#if UNITY_EDITOR
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(MakeSmallerImageScale());
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            StartCoroutine(MakeLargerImageScale());
        }
#endif
    }


    //// ���X�|�[���n�_�ۑ�
    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Respawn"))
    //    {
    //        respawnpoint = other.gameObject.GetComponent<RespornPoint>().respornPosition;

    //    }
    //    //if(other.gameObject.tag == "GameOver")
    //    //{
    //    //	Resporn();
    //    //}
    //}


    /// <summary>
    /// ���X�|�[�����̃X�e�[�W���������܂Ƃ߂��֐�
    /// </summary>
    public void Reset()
    {
        if(boss != null) 
        {
            ResetBoss();
        }
        ResetCookies();
        ResetEnemies();
    }


    // �{�X�퓬���Ɏ��񂾎��̏���
    //�{�X�̍��ǂ��グ��
    private void ResetBoss()
    {
        //boss.bossleftcheck = true;
        boss.transform.position = bossPosition;
    }

    // �N�b�L�[�̏�����
    private void ResetCookies()
    {
        for(int i = 0; i < l_cookies.Count; i++)
        {
            l_cookies[i].gameObject.SetActive(true);
            l_cookies[i].MakeGettableCoin();
        }
    }

    // �G�̏�����
    private void ResetEnemies()
    {
        for(int i = 0; i  < enemies.Length; i++)
        {
            // �ʒu��񏉊���
            enemies[i].transform.position = enemiesPos[i];
        }
    }

    // �Ö��̈ʒu��ݒ肷��
    private void InitImage()
    {
        float posZ = image.transform.position.z;
        playerPosition = playerTrans.position;
        playerPosition.z = posZ;
        image.transform.position = playerPosition;
    }

    // �Ö��̃X�P�[�����k��������
    public IEnumerator MakeSmallerImageScale()
    {
        var wait = new WaitForSeconds(0.01f);
        while (nowImageScale > 0.03)
        {
            yield return wait;
            nowImageScale  = nowImageScale * 15.0f / 16.0f;
            image.localScale = new(nowImageScale, nowImageScale, nowImageScale); ;
        }
    }


    // �Ö��̃X�P�[�����g�傳����
    public IEnumerator MakeLargerImageScale()
    {
        var wait = new WaitForSeconds(0.01f);
        while (nowImageScale < MAX_IMAGE_SCALE)
        {
            yield return wait;
            nowImageScale = nowImageScale * 16.0f / 15.0f;
            image.localScale = new(nowImageScale, nowImageScale, nowImageScale); ;
        }
    }

    // ���X�|�[�������񐔂�Ԃ��֐�
    public int GetRespawnCnt()
    {
        return respawnCnt;
    }

    // ���݃��X�|�[�������ǂ�����Ԃ��֐�
    public bool GetIsNowRespawning()
    {
        return isNowRespawning;
    }

    // ���݃��X�|�[������ݒ肷��֐�
    public void SetIsNowRespawning(bool nextState)
    {
        isNowRespawning = nextState;
    }

    // �A�v���P�[�V�����I�����Ƀ��X�g��j��
    private void OnApplicationQuit()
    {
        enemies.Free();
        enemiesPos.Free();
        l_cookies.Clear();
    }
}
