using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossAct_Stage3 : MonoBehaviour
{
    // �x���֐��̎擾
    IEnumerator bossAtk;
    IEnumerator bossCT;

    [SerializeField] private bool actTest = true;

    // �_���[�W����
    private bool damage;        // ��_���[�W��
    private bool invincibilitu; // ���G���

    // �X�e�[�^�X�N���X�̎擾
    [SerializeField] public BossStatusManager bossStatusManager;

    // �t�F�C�Y���
    [SerializeField] private byte phase;

    [SerializeField] private ushort[] shockTime;

    // ������
    private bool makeGround;    // ���g�̋x�e�p��
    private bool[] shockGround; // �G�U�����̐���

    // �U���̏��
    [SerializeField] private bool nowAttack;
    [SerializeField] private bool nowCT;

    [SerializeField] private byte coolTime;

    private IEnumerator BossCoolTime()
    {
        nowCT = true;

        for(int i = 0; i < coolTime; i++)
        {
            yield return null;
        }

        nowCT = false;
    }

    private IEnumerator BossAttack()
    {
        nowAttack = true;

        for(int i = 0; i < shockTime[phase]; i++)
        {
            yield return null;
        }

        nowAttack = false;

        bossCT = BossCoolTime();
        StartCoroutine(bossCT);
    }

    private void OnTriggerEnter(Collider other)
    {

    }

    private void FixedUpdate()
    {
        if(actTest || (!nowCT && !nowAttack))
        {
            bossAtk = BossAttack();
            StartCoroutine(bossAtk);

            actTest = false;
        }
    }
}