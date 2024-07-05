using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          ���e����������X�N���v�g
// �������n�_�̏������Ƃɂ����֕��̂���������
// ��[Setup_BombPointer]�̕t���Ă���I�u�W�F�N�g���K�v
//==================================================
// �����2023/05/26    �X�V��2023/05/28
// �{��
public class Boss3_Bomb : MonoBehaviour
{
    [SerializeField] private bool testSpawn;

    // �w�߂̏��
    private GameObject bombOperator;

    // �e�̏��
    private GameObject parObj;

    // ���e�̃v���n�u���i�[
    public GameObject bombObj;
    [SerializeField] private byte bombCount = 4;
    private List<GameObject> l_bombPool = new List<GameObject>();
    private List<Boss3_Bomb_Action> l_bombScripts = new List<Boss3_Bomb_Action>();

    // �������̎擾
    private Boss3_Main parMain;

    private void Start()
    {
        // ���e���i�[����Ă��Ȃ��Ȃ�
        if(bombObj == null)
        {
            // �x�������\������
            Debug.LogWarning(this.transform.name + "�̔��e���i�[����Ă��܂�����I");

        }//----- if_stop -----
        for(byte i = 0; i < bombCount; i++)
        {
            l_bombPool.Add(Instantiate(bombObj, new Vector3(20, 0, 50), Quaternion.identity));
            l_bombScripts.Add(l_bombPool[i].GetComponent<Boss3_Bomb_Action>());
        }

        // �w�߂̏����擾
        bombOperator = GameObject.Find("BombPointOperator").gameObject;

        // �e���̎擾
        parObj = transform.parent.gameObject;

        // �������̎擾
        parMain = parObj.GetComponent<Boss3_Main>();
    }

    private void FixedUpdate()
    {
        // ���e���i�[����Ă��Ȃ��Ȃ珈�����΂�
        if (bombObj == null) return;

        if(testSpawn)
        {
            StartCoroutine(SpawnBomb());
            testSpawn = false;
        }
    }

    //==================================================
    //          �����n�_�Ɋe�I�u�W�F�N�g����������
    // �������n�_�̏������Ƃɂ����֕��̂���������
    // ���S�n�сA�{�X�{�̈ȊO�ɂ͔��e��z�u���܂�
    //==================================================
    // �����2023/05/26    �X�V��2023/05/28
    // �{��
    public IEnumerator SpawnBomb()
    {
        // �w�߂̎q�I�u�W�F�N�g�̐����擾����
        int opChildCnt = bombOperator.transform.childCount;

        // �{�X�̏o���n�_��ݒ�
        int randomBossPos = Random.Range(0,opChildCnt);

        // �v���C���[������n�_��ݒ�
        int randomSafePos = Random.Range(0, opChildCnt);

        // ���鏰���{�X�o���n�_�Ɠ����Ȃ�Ē��I���s��
        while(randomBossPos == randomSafePos)
        {
            randomSafePos = Random.Range(0, opChildCnt);
        }//----- if_stop -----

        // �e���q�I�u�W�F�N�g�ɂȂ��Ă����Ȃ��������
        if (parObj.transform.parent != null) parObj.transform.parent = null;

        // ���g�̐e�I�u�W�F�N�g�����̎q�I�u�W�F�N�g�Ɏw��
        parObj.transform.position = Vector3.zero+new Vector3(0,1,0);
        parObj.transform.SetParent(GameObject.Find("BombPoint" + randomBossPos).transform,false);

        // �{�̏o�����̃A�j���[�V�����͂���...? ----------
        parMain.anim.SetBool("Grawing",true);
        parMain.audio.bossSource.Stop();
        parMain.audio.bossSource.loop = false;
        parMain.audio.Boss3_SpawnSound();

        // �w�߂̎q�I�u�W�F�N�g�̐��J��Ԃ�
        for (int i = 0; i < opChildCnt; i++)
        {
            // �w��̎q�I�u�W�F�N�g���i�[
            GameObject thisObj = GameObject.Find("BombPoint" + i).gameObject;
            
            Vector3 thisObj_Pos = thisObj.transform.position;

            // �i�[�����I�u�W�F�N�g���e�I�u�W�F�N�g�Ȃ珈���𔲂���
            if (thisObj == parObj) yield break;

            // �����n�_�Ƀ{�X�̏o������\��Ȃ珈�����΂�
            if (i == randomBossPos) continue;

            // �����n�_�Ɉ��S�n�т��o������\��Ȃ珈�����΂�
            if (i == randomSafePos) continue;

            Debug.Log("BombPoint" + i);
            Debug.Log("randomBossPos="+randomBossPos);
            Debug.Log("randomSafePos=" + randomSafePos);

            for (byte j = 0; j < bombCount; j++)
            {
                if (l_bombPool[j].transform.position.z != thisObj_Pos.z+1)
                {
                    // �����������e�����̎q�I�u�W�F�N�g�ɕύX����
                    l_bombPool[j].transform.SetParent(thisObj.transform, false);
                    l_bombPool[j].transform.position = new Vector3(thisObj_Pos.x, thisObj_Pos.y+0.1f, thisObj_Pos.z + 1);
                    StartCoroutine(l_bombScripts[j].BombCountDown());
                    l_bombScripts[j].bombAnim.Play("BombGrawUp");
                    //for (byte k = 0; k < 3; k++)
                    //{
                    //    yield return null;
                    //}
                    break;
                }
                else if (j == bombCount - 1)
                {
                    l_bombPool.Add(Instantiate(bombObj, new Vector3(20, 0, 50), Quaternion.identity));
                    l_bombScripts.Add(l_bombPool[j].GetComponent<Boss3_Bomb_Action>());
                    bombCount++;
                }
            }
            //// ���e���o��������
            //GameObject spawnObj = Instantiate(bombObj);
            //spawnObj.GetComponent<Boss3_Bomb_Action>().bossMain = parMain;
            
        }
        
    }
    private void OnApplicationQuit()
    {
        l_bombPool.Clear();
        l_bombScripts.Clear();
    }
}