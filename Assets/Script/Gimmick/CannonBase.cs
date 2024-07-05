using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CannonBase : MonoBehaviour
{
    // Start is called before the first frame update

    public enum VECTOR
    { 
        RIGHT,
        LEFT,
        UP,
        DOWN,
    }
    
    [System.Serializable] public struct BulletData
    {
        public GameObject obj;
        public CannonBullet sqript;
    }

    private Dictionary<VECTOR, float> vec2Rad = new Dictionary<VECTOR, float>(); 
    
    



    [Header("-----�e�̃Q�[���I�u�W�F�N�g-----\n")]
    [SerializeField] private GameObject bulletObj;
    // �I�u�W�F�N�g�v�[���ۑ��p
    [SerializeField]private List<BulletData> l_bulletPool = new List<BulletData>();
    // �e�̃I�u�W�F�N�g�v�[���̕ۊǏꏊ
    private Vector3 stayPos = new Vector3(20, 0, -50);
    [Header("�v�[�����̏����I�u�W�F�N�g��")]
    [SerializeField] private byte defPoolCnt = 2;
    private byte nowPoolCnt = 0;

    
    [Header("\n-----�e�̔��ːݒ�-----\n")]
    [Header("���˂̃f�B���C�^�C��(�b)")]
    [SerializeField] private float shotDelay = 5.0f;
    [Header("�e�̑��x")]
    [SerializeField] private float speed = 5.0f;
    // �e�̔��˃x�N�g��
    private Vector3 shotVec;
    [Header("�e�̔��˕���")]
    [SerializeField] private VECTOR vec = VECTOR.RIGHT;
    [Header("�e�����R���ł���܂ł̎���(�b)")]
    [SerializeField] private float destroyTIme = 8.0f;
    [Header("-----�e�𔭎˂���ʒu�̕␳�l-----\n")]
    [Header("���˕����ɂ��̒l���̕␳���|���܂�")]
    [SerializeField] private float shotShiftVal = 0.0f;
    // ���ۂ̕␳�l
    private Vector3 shiftVec;

    // ���������p�ϐ�
    private float nowDelayTime = 0.0f;
    private Vector3 pos;

    // �J�����Ɏʂ��Ă��邩
    private bool cameraVisibling = false;

    void Start()
    {
        if(bulletObj == null)
        {
            Debug.LogError("�e�̃v���n�u��o�^���Ă�������");
        }
        // ���̃|�W�V������ۑ�
        pos = this.transform.position;

        // �e�����ɑΉ������p�x�����W�A���p�œo�^
        vec2Rad.Add(VECTOR.RIGHT, Mathf.Deg2Rad * 0.0f);
        vec2Rad.Add(VECTOR.LEFT, Mathf.Deg2Rad * 180.0f);
        vec2Rad.Add(VECTOR.UP, Mathf.Deg2Rad * 90.0f);
        vec2Rad.Add(VECTOR.DOWN, Mathf.Deg2Rad * 270.0f);
        
        // ���ˈʒu�̕␳�l���v�Z
        SetShiftValue();

        // �I�u�W�F�N�g�v�[���̓o�^
        for(byte i = 0;i<defPoolCnt;i++)
        {
            l_bulletPool.Add(InitNewBulletPool());
        }
    }

    // Update is called once per frame
    void Update()
    {
        // �J�����Ɏʂ��Ă��Ȃ��Ƃ��������~�߂�B
        if(!cameraVisibling)
        {
            return;
        }
        nowDelayTime += Time.deltaTime;
        if(nowDelayTime >= shotDelay)
        {
            GetBulletPool().sqript.Shot(speed, shiftVec + pos, shotVec, destroyTIme,vec);
            nowDelayTime = 0.0f;
        }
    }


    private BulletData InitNewBulletPool()
    {
        BulletData newData;

        newData.obj = Instantiate(bulletObj, stayPos, Quaternion.identity);
        newData.sqript = newData.obj.GetComponent<CannonBullet>();
        if(newData.sqript == null)
        {
            Debug.LogError("�o���b�g�X�N���v�g�̎擾�Ɏ��s");
        }
        nowPoolCnt++;

        return newData;
    }

    private BulletData GetBulletPool()
    {
        byte i;
        for (i = 0;i<nowPoolCnt;i++)
        {
            if(l_bulletPool[i].obj.activeSelf == false)
            {
                l_bulletPool[i].obj.SetActive(true);
                l_bulletPool[i].sqript.End();
            }
            if(l_bulletPool[i].sqript.CheckState(CannonBullet.STATE.STAY))
            {
                break;
            }
            else if(i==nowPoolCnt-1)
            {
                l_bulletPool.Add(InitNewBulletPool());
            }
        }
        return l_bulletPool[i];
    }

    private void SetShiftValue()
    {
        var vec3 = Vector3.zero;
        vec3.x = Mathf.Cos(vec2Rad[vec]);
        vec3.y = Mathf.Sin(vec2Rad[vec]);
        vec3.z = 0.0f;
        shotVec = vec3;
        shiftVec = vec3*shotShiftVal;
        
    }

    public void SetShotVec(VECTOR _vec)
    {
        vec = _vec;
        SetShiftValue();
    }

    private void OnBecameVisible()
    {
        cameraVisibling = true;
    }
    private void OnBecameInvisible()
    {
        cameraVisibling = false;
        nowDelayTime = 0.0f;
    }

    private void OnApplicationQuit()
    {
        vec2Rad.Clear();
        l_bulletPool.Clear();
    }
}
