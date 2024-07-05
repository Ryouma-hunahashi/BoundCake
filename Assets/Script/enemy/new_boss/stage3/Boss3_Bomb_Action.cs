using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ���e�{�̂̏���
// ���o�������u�ԂɃJ�E���g���J�n���āA
// �@���������u�ԁA���g�𒆐S��
// �@�������֐U���𔭐������܂��@
//==================================================
// �����2023/05/26    �X�V��2023/05/28
// �{�� ���c
public class Boss3_Bomb_Action : MonoBehaviour
{
    // ���g�̎w�߂��擾
    private Setup_BombPointer opMain;
    public Boss3_Main bossMain;
    public Animator bombAnim;
    private BossAudio bombAudio;

    // �����܂ł̎���
    [SerializeField] private byte bombCount;

    // �g�̐ݒ�
    [Header("----- �g�̐ݒ� -----")]
    private WavePool pool;
    public Transform ground;   // ���e����p���鎅��vfxManager
    private sbyte arrayNum = 0;     // �g�𐶐�����Ƃ��߂�l���N���[����
    [SerializeField] private float waveSpeed = 7.5f;    // �g�̑��x
    [SerializeField] private float waveWidth = 0.225f;  // �g�̐U����
    [SerializeField] private float waveHight = 2.0f;    // �g�̍���
    [SerializeField] private GameObject waveObj;        // �g�̔���v���n�u
    public int waveCount = 3;   // �v�[���ɐ�������g�̐�
    public int waveAngle = 1;  // �g�̕���

    //// �g�̃R���W�����̃I�u�W�F�N�g�v�[��
    //[System.NonSerialized] public List<GameObject> l_waveCollisionObj = new List<GameObject>();

    //// �g�R���W�����ɃR���|�[�l���g����Ă���g����̃v�[��
    //// �Y�����͔g����̃v�[���ɑΉ�
    //private List<waveCollition> l_waveCollitions = new List<waveCollition>();


    // ���݂̏��

    private void Start()
    {
        // ���g�̎w�߂��擾
        GameObject opObj = GameObject.Find("BombPointOperator").gameObject;
        opMain = opObj.GetComponent<Setup_BombPointer>();
        bombAnim = GetComponent<Animator>();
        if (bombAnim == null) Debug.LogError("���e�ɃA�j���[�^�[���Ȃ�");
        bombAudio = GetComponent<BossAudio>();
        pool = GetComponent<WavePool>();
        pool.AddEtoPDamage();
        //// �g�̊e�����擾
        //for (int i = 0; i < waveCount; i++)
        //{
        //    l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
        //    l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
        //    l_waveCollisionObj[i].AddComponent<EtoP_Damage>();

        //}//----- for_stop -----

        //StartCoroutine(BombCountDown());
    }

    public IEnumerator BombCountDown()
    {
        
        
        for(int i = 0; i < bombCount; i++)
        {
            if(i==bombCount-60)
            {
                Debug.Log("Bomb!!");
                bombAudio.Boss3_ExplosionSound();
            }
            // �P�t���[���x��������
            yield return null;

        }//----- for_stop -----
        
        // �������J�n����
        BlastWaveAttack();
    }

    private void BlastWaveAttack()
    {
        //// ��������׌����ڂ��\���ɂ���
        //MeshRenderer myMesh = this.gameObject.GetComponent<MeshRenderer>();
        //myMesh.enabled = true;

        bombAnim.SetBool("Bombing", true);
        

        ground = transform.parent.transform.parent.GetChild(0);

        // ���g�𒆐S�ɗ������֐U���𔭐������� ---------- ���肢���܂��B
        StartCoroutine(WaveWideSpawn());


        
    }

    private IEnumerator WaveWideSpawn()
    {
        

        waveAngle = 1;
        pool.WaveCreate(waveSpeed*waveAngle,waveWidth,waveHight,waveCollition.WAVE_TYPE.ENEMY,
            transform.position.x + 0.25f, ground);

        for (byte j = 0; j < 3; j++)
        {
            yield return null;
        }

        
        waveAngle = -1;
        pool.WaveCreate(waveSpeed * waveAngle, waveWidth, waveHight, waveCollition.WAVE_TYPE.ENEMY,
            transform.position.x - 0.25f, ground);

        for (byte i = 0;i<10; i++)
        {
            yield return null;
        }
        // �U��������,���g��j�󂷂�
        opMain.blasted = true;
        transform.position = new Vector3(20, 0, 50);
        bombAnim.SetBool("Bombing", false);
        
        transform.parent = null;
        ground = null;
    }
    //public void WaveCreate(float startPosX, float startPosY)
    //{
    //    // �z��ԍ����w�肷��
    //    arrayNum = vfxManager.WaveSpawn(startPosX, waveSpeed * waveAngle, waveHight, waveWidth, 1);
    //    Debug.Log("�{���g����");
    //    for (int i = 0; i < waveCount; i++)
    //    {
    //        if (l_waveCollisionObj[i].transform.position.z != 0)
    //        {
    //            l_waveCollisionObj[i].transform.SetParent(transform);
    //            // �����蔻��ɔԍ����w��
    //            l_waveCollitions[i].waveNum = arrayNum;
    //            // �N���[���ɑΉ������� vfxManager ��ݒ�
    //            l_waveCollitions[i].vfxManager = vfxManager;
    //            // �g���v���C���[�������������g�ɐݒ�
    //            l_waveCollitions[i].waveType = waveCollition.WAVE_TYPE.ENEMY;
    //            // �R���W�����𓮂���
    //            l_waveCollitions[i].waveMode = waveCollition.WAVE_MODE.PLAY;
    //            // �R���W�����̍�����g�̍����ɐݒ�
    //            l_waveCollitions[i].transform.localScale = new Vector3(0, 0, 1);
    //            // �R���W�����̔����ʒu��g�̔����ʒu�ɐݒ�
    //            l_waveCollitions[i].waveStartPosition = new Vector3(startPosX, startPosY, 0);

    //            // �R���W�����̍ő卂�x��ݒ�
    //            l_waveCollitions[i].maxWaveHight = waveHight;

    //            break;
    //        }//----- if_stop -----
    //        else if (i == waveCount - 1)
    //        {
    //            l_waveCollisionObj.Add(Instantiate(waveObj, new Vector3(10, 0, 50), Quaternion.identity));
    //            l_waveCollitions.Add(l_waveCollisionObj[i].GetComponent<waveCollition>());
    //            waveCount++;

    //        }//----- elseif_stop -----

    //    }//----- for_stop -----
    //}

    //private void OnApplicationQuit()
    //{
    //    l_waveCollisionObj.Clear();
    //    l_waveCollitions.Clear();
    //}


}
