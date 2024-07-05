using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_MobSpawn : MonoBehaviour
{
    public GameObject boss5Obj;
    // �{�X�̃��C���X�N���v�g���i�[
    private Boss5_Main boss5Main;

    // ���g�̎q�̏��
    private GameObject childObj;

    [Tooltip("�w�肳�ꂽ�t�F�C�Y�ŏ������܂�")]
    [SerializeField] private byte phaseSpawn;
    [SerializeField] private byte smakeFrame = 30;
    private bool spawnFlag = true;
    [SerializeField] private bool started;

    private void Start()
    {
        boss5Main = boss5Obj.GetComponent<Boss5_Main>();
        // ���g�̎q�I�u�W�F�N�g���擾����
        childObj = this.transform.GetChild(0).gameObject;
    }

    private void FixedUpdate()
    {
        // �{�X�̓������J�n����Ă���Ƃ�
        if(boss5Main.startAction || started)
        {

            // �{�X�̃t�F�C�Y���ݒ肵���t�F�C�Y�ƈ�v�����Ƃ���
            if (spawnFlag && boss5Main.nowPhase == phaseSpawn)
            {
                //------------�X�|�[���G�t�F�N�g------------
                // ���g�̎q�I�u�W�F�N�g���A�N�e�B�u�ɂ���
                childObj.SetActive(true);
                spawnFlag = false;
                StartCoroutine(EffectStop());

            }//----- if_stop -----
        }

        if(boss5Main.status.hitPoint <= 0)
        {
            this.gameObject.SetActive(false);
        }
    }

    private IEnumerator EffectStop()
    {
        for (byte i = 0;i<smakeFrame;i++)
        {
            yield return null;
        }
        // �ɃG�t�F�N�g�~�߂�
    }
}
