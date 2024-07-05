using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Boss : MonoBehaviour
{
    public enum STATE
    {
        // ���o�n��
        WARNING,    // �o��
        ENDING,     // ����
        PHASE,      // �t�F�C�Y�ύX

        // �U���ȊO�̏��
        IDLE,
        MOVE,
        DAMAGE,
        STAN,

        // �U�����
        ATK01,
        ATK02, 
        ATK03, 
        ATK04,
        ATK05,
    }

    // ���g�̏��
    private STATE state;

    // �Q�b�g�Z�b�^
    public STATE GetState() { return state; }
    public void SetState(STATE _state) { this.state = _state; }

    // �I�u�W�F�N�g�ɂ��Ă���f�[�^
    private Rigidbody rb;
    private Animator anim;
    private BossAudio audio;

    // �t���}�l�[�W���[
    private EnemyEffectManager fxManager;

    // �Q�b�g�Z�b�^
    public Rigidbody GetRigidbody() { return rb; }
    public Animator GetAnimator() { return anim; }
    public BossAudio GetAudio() { return audio; }
    public EnemyEffectManager GetEffectManager() { return fxManager; }

    // �^�[�Q�b�g�̏��
    [SerializeField] private GameObject target;

    // �Q�b�g�Z�b�^
    public GameObject GetTarget() { return target; }

    // �t�F�C�Y��Ԃ̃f�[�^
    private byte phase;

    public void NextPhase() { phase++; }    // �t�F�C�Y���㏸������
    public int GetPhase() { return phase; } // ���̃t�F�C�Y���擾����

    // �ҋ@���Ԃ̐ݒ�
    [SerializeField] private float idleTime;

    private void Awake()
    {
        // ���g�̏����擾
        rb = this.GetComponent<Rigidbody>();
        anim = this.GetComponent<Animator>();
        audio = this.GetComponent<BossAudio>();
        fxManager = this.GetComponentInChildren<EnemyEffectManager>();

        // �^�[�Q�b�g���ݒ肳��Ă��Ȃ��Ȃ�
        if(!target)
        {
            // �v���C���[���������Ċi�[����
            target = GameObject.Find("player").gameObject;
        }
    }

    /// <summary>
    /// ������ϐ��̒l�ɂ���đҋ@���Ԃ��ω�����֐�
    /// �����̐ݒ�F
    /// (-1)�ҋ@���Ȃ�
    /// (�O)�ϐ����g�p����
    /// (�P)�����𗘗p����
    /// </summary>
    /// �߂�l�F�Ȃ�
    ///  ���� �F(1)�ҋ@���鎞��
    /// </summary>
    public IEnumerator IdleTime(float _wait = 0)
    {
        // �����̐��l��"�O����"�Ȃ珈���𔲂���
        if (_wait < 0) yield break;

        // �ҋ@�J�n�̃��O��\��
        Debug.Log("< " + this.name + " >�F�ҋ@��ԂɈڍs���܂����B");

        // �����̐��l��"0�ł͂Ȃ�"�Ȃ�
        if (_wait != 0)
        {
            // �������g�p���郍�O��\��
            Debug.Log("< " + this.name + " >�F[ �ҋ@��� ]�Ɉ������g�p���āi" + _wait + "�j�b�ҋ@���܂�");

            // �ҋ@�������w�肵�������ōs��
            yield return new WaitForSeconds(_wait);

        }//----- if_stop -----
        else
        {
            // �ϐ����g�p���郍�O��\��
            Debug.Log("< " + this.name + " >�F[ �ҋ@��� ]�ɕϐ����g�p���āi" + idleTime + "�j�b�ҋ@���܂�");

            // �ҋ@�������w�肵���ϐ��ōs��
            yield return new WaitForSeconds(idleTime);

        }//----- else_stop -----
    }
}
