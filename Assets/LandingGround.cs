using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class LandingGround : MonoBehaviour
{
    //���g��Rigidbody
    Rigidbody rb;
    //�U������̂ɂ����鎞��
    //[SerializeField] float landTime = 0.5f;
    [SerializeField] Vector3 landingSpeed = new Vector3(0, 2, 0);
    //�U���̉e�����y�Ԕ͈�
    //[SerializeField] float landRange = 2f;

    //�Q�[���J�n���̎��I�u�W�F�N�g�̍��W
     Vector3 startPos;
    //�Q�[���J�n���̎��̍���
    private float startPosY;
    //�U���������Ɏ����ǂ��܂Œ��ނ�
    [Header("�������ޗ�")]
    [SerializeField] float targetPosY=1;
    [Header("�������ރX�s�[�h(�t���[��)")]
    [Tooltip("�����̑��x")]
    [SerializeField] float landSpeed = 15.0f;
    private float defaultLandSpeed;
    //�������ތ��E�̍��W�E
    Vector3 targetPos;
    //�����㏸����ۂ�
    [SerializeField] float upRate = 1;
    [SerializeField] string EnemyTag="Enemy";
    //�n�ʂƐڐG�����G�I�u�W�F�N�g���i�[����p�̃��X�g
    //public List<GameObject> EnemyObjList = new List<GameObject>();
    public bool moveFlg = false;

    //=====================================
    private bool AlphaFlg = false;
    //=====================================

    private float landAccel;
    private float landVelocity = 0;

    public bool nowCharge = false;
    private float chargeAccel = 0;
    private float chargeVelocity = 0;

    private const float LandingIndex = Mathf.PI / 2;
    //�q�I�u�W�F�N�g��VFX�R���|�[�l���g
    private VisualEffect ChildrenEffect;
    private vfxManager vfxManager;

    private bool vfxShiftFg = false;
    private float shiftScale;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        ChildrenEffect = GetComponentInChildren<VisualEffect>();
        vfxManager = GetComponentInChildren<vfxManager>();
        if(ChildrenEffect == null||vfxManager==null)
        {
            Debug.LogError("VFX���˂��I");
        }
        startPos =transform.position;
        startPosY=transform.position.y;
        targetPos=new Vector3(startPos.x, startPosY-targetPosY, startPos.z);

        landAccel = LandingIndex / landSpeed;

        //=====================================
        AlphaFlg = ChildrenEffect.GetBool("AlphaFlg");
        if(GetComponent<alphaZeroSpring>() != null)
        {
            AlphaFlg = false;
        }
        //=====================================

        defaultLandSpeed = landSpeed;
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    private void FixedUpdate()
    {
        if (gameObject.CompareTag("Floor"))
        {
            startPos = transform.parent.position;
            startPosY = startPos.y;
            targetPos = new Vector3(startPos.x, startPosY - targetPosY, startPos.z);
        }
        else if (transform.parent != null && transform.parent.CompareTag("Floor"))
        {
            startPos = transform.parent.position;
            startPosY = startPos.y;
            targetPos = new Vector3(startPos.x, startPosY - targetPosY, startPos.z);
        }
            // ���������o�Ă����

            if (nowCharge)
            {
                if (landVelocity <= LandingIndex)
                {
                    var pos = transform.position;
                    landVelocity += landAccel;
                    shiftScale = targetPosY * (Mathf.Sin(landVelocity));
                    pos.y = startPosY - shiftScale;
                    if (pos.y <= transform.position.y)
                    {
                        transform.position = pos;
                        if (vfxShiftFg)
                        {
                            vfxManager.chargeShift = shiftScale;
                        }
                    }
                }
            }
            else if (moveFlg)
            {
                var pos = transform.position;
                landVelocity += landAccel;
                pos.y = startPosY - targetPosY * (Mathf.Cos(landVelocity));
                transform.position = pos;
                //rb.AddForce(landingSpeed*upRate);
                //if (transform.position.y > startPosY)
                //{
                //    moveFlg = 0;
                //    rb.velocity = Vector3.zero;
                //    transform.position = startPos;

                //}
                if (landVelocity > LandingIndex)
                {
                    pos.y = startPosY;
                    transform.position = pos;
                    landVelocity = 0;
                    moveFlg = false;
                }
            }
            else
            {

                landVelocity = 0;
                var pos = transform.position;
                pos.y = startPosY;
                transform.position = pos;
                vfxManager.chargeShift = 0;
                shiftScale = 0;
            }
        
        


#if UNITY_EDITOR
        if(defaultLandSpeed!=landSpeed)
        {
            defaultLandSpeed = landSpeed;
            landAccel = LandingIndex / landSpeed;
        }
#endif
        //else if(moveFlg < 0)
        //{
        //    //rb.AddForce(-landingSpeed);

        //}

        //�n�ʂ����ލۂ̏���E�����̐ݒ�
        //if (transform.position.y > startPosY)
        //{
        //    moveFlg = 0;
        //    rb.velocity = Vector3.zero;
        //    transform.position = startPos;

        //}
        //if (transform.position.y < targetPos.y)
        //{
        //    rb.velocity = Vector3.zero;
        //    transform.position = targetPos;
        //    moveFlg = 1;

        //}

    }

    //����h�炵�n�߂鏈��
    public void lnadGround(float _landDis,float _landSpeed)
    {
        //�g�̔����n�_������͈͓��ɓG�����݂��Ă��邩�����X�g�Ŋm�F����
        //foreach(GameObject obj in EnemyObjList)
        //{
        //    //���݂��Ă����Y���W�̌Œ����������
        //    if (Mathf.Abs(obj.transform.position.x-pointX) < landRange)
        //    {
        //        obj.GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionZ;
        //    }
        //}

        // �����Ă���r���łȂ����
        if (!moveFlg)
        {
            // �X�s�[�h���X�V
            landSpeed = _landSpeed;
            // �����鋗�����X�V
            targetPosY = _landDis;
            // �^�[�Q�b�g�̍ŏI����
            targetPos = new Vector3(transform.position.x, startPosY - targetPosY, transform.position.z);
            // ������p�����x���X�V
            landAccel = LandingIndex / landSpeed;
            // �|�W�V������ڕW�n�_�ɕύX
            transform.position = targetPos;
            // ������
            moveFlg = true;
            //landingtime�v���p�e�B�ɊJ�n���Ԃ𑗐M����
            ChildrenEffect.SetFloat("LandingTime", Time.time);

            //=====================================
            if (AlphaFlg == true)
            {
                ChildrenEffect.SendEvent("Start");
            }
            //=====================================
            //rb.velocity = -landingSpeed;
        }
        //rb.velocity=landingSpeed*upRate;

    }

    public void ChargeLand(float _maxLandDis, float _landSpeed,bool _shiftFg = false)
    {
        if (!nowCharge)
        {

            landSpeed = _landSpeed;
            landAccel = LandingIndex / landSpeed;
            targetPosY = _maxLandDis;
            targetPos = new Vector3(transform.position.x, startPosY - targetPosY, transform.position.z);
            nowCharge = true;
            vfxShiftFg = _shiftFg;
        }

        //// �p�����x���v�Z
        //// 0�`1�̊ԂőJ�ڂ��������̂�90�����`���[�W�̌��E�����Ŋ��鎖�ŎZ�o
        //chargeAccel = LandingIndex / _chargeLimit;
        //// ���݂̃`���[�W���p�����x�ɏ�Z
        //chargeVelocity = chargeAccel*_chargePoint;
        //// �|�W�V�������ꎞ�i�[
        //var pos = transform.position;
        //// �����ʒu���猻�݂̃`���[�W�l�ŎZ�o�������݊p�x��sin�ɕ��荞�݉�����B
        //pos.y = startPosY - _maxLandDis * (Mathf.Sin(chargeVelocity));
        //// �|�W�V�������X�V
        //transform.position = pos;
        
        //// �n�ʂ��`���[�W���ɂ���B
        //nowCharge = true;
        //// ���E�l�Ƀ`���[�W�x�����B���Ă��Ȃ����
        //if (_chargeLimit != _chargePoint)
        //{
        //    // ��x�ŉ����鋗���̋ߎ��l��Ԃ��B(�����ɂ͈ꏏ�łȂ�����)
        //    return _maxLandDis * (Mathf.Sin(chargeAccel));
        //}
        //// ���B���Ă����
        //else
        //{
        //    // 0��Ԃ��A����ȏ�̌������Ȃ����B
        //    return 0;
        //}

        
    }

    public IEnumerator ChargeEnd()
    {
        if(nowCharge)
        {
            for (byte i = 0; i < 3; i++)
            {
                //if(nowCharge)
                //{
                //    yield break;
                //}
                yield return null;
            }
            nowCharge = false;
        }
    }

    

    //�G���n�ʂɍŏ��ɐG�ꂽ�Ƃ���Y���W���Œ肳����
    //private void OnCollisionEnter(Collision collision)
    //{
        ////�Փ˂����I�u�W�F�N�g�̃^�O���G
        //if(collision.transform.CompareTag(EnemyTag))
        //{
        //    //�Փ˂����I�u�W�F�N�g�̈ʒu�𒲐�����

        //    //�Փ˂����I�u�W�F�N�g��Rigidbody���擾����
        //    rb = collision.rigidbody;
        //    //�I�u�W�F�N�g�̑S��]�EY���W�EZ���W���Œ肷��
        //    rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezePositionZ;


        //}
        
        //if(collision.gameObject.CompareTag("Player"))
        //{
        //    Debug.Log("���߁I");
        //    lnadGround(collision.contacts[0].point.x);
        //}
    //}
    //private void OnCollisionExit(Collision collision)
    //{
    //    if(collision.gameObject.CompareTag("Player"))
    //    {
    //        moveFlg = -1;
    //        rb.velocity = -landingSpeed;
    //    }
    //}
    
}
