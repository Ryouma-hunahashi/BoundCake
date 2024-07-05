using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �{�X�̕��g
// ��[Setup_AvatarPoint]�X�N���v�g���t�^���ꂽ
// �@�I�u�W�F�N�g�����݂��Ă���V�[�����Ŏg�p���Ă�������
//==================================================
// �����2023/05/26    �X�V��2023/05/30
// �{��
public class Boss4_Avatar_Main : MonoBehaviour
{
    // ���g�̏��
    public int myID;
    public bool active = true;
    public Rigidbody avatartRb;
    public Animator avatarAnim;
    public SpriteRenderer avatarMesh;
    private Animator anim;
    private EnemyEffectManager ef_Manager;
    private BossAudio audio;

    // �o�����̃A�j���[�V�����ݒ�
    public bool startAnim;

    // �㏸�̐ݒ�
    public sbyte ascendSpeed = 30;

    // �g�̓����蔻��
    public bool damage;
    public bool invisible;
    private GameObject waveHit;
    private GameObject waveLog;

    private void OnTriggerEnter(Collider other)
    {
        // �U�����󂯂��Ȃ�ȍ~�����𔲂���
        if (damage || invisible) return;

        // �U���ɐG�ꂽ��
        if (other.CompareTag("Wave"))
        {
            if (waveHit == null)
            {
                damage = true;
                waveHit = other.gameObject;

                // �_���[�W�A�j���[�V�������J�n ----------

                // ���ł��J�n����
                StartCoroutine(RemoveAvatar());

                // �X�V�����g�����O�ƈ�v���Ȃ��Ȃ�
                if (waveHit != waveLog)
                {
                    // �g�̃��O���X�V����
                    waveLog = waveHit;

                }//----- if_stop -----

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void OnTriggerExit(Collider other)
    {
        // �U���ɐG��Ă�����
        if (other.CompareTag("Wave"))
        {
            // �G�ꂽ�g���i�[����Ă���Ȃ�
            if (waveHit != null)
            {
                waveHit = null;

            }//----- if_stop -----

        }//----- if_stop -----
    }

    private void Start()
    {
        // �O�͖{�̂ɂȂ邽�ߕ\�����Ȃ�
        if (myID == 0) active = false;

        // ���g�̏����擾
        GameObject thisObj = this.transform.gameObject;
        avatartRb = thisObj.GetComponent<Rigidbody>();
        avatarAnim = thisObj.GetComponent<Animator>();
        avatarMesh = thisObj.GetComponent<SpriteRenderer>();
        anim = thisObj.GetComponent<Animator>();
        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        audio = GetComponent<BossAudio>();
    }

    private void FixedUpdate()
    {
        // �O�͖{�̂ɂȂ邽�ߕ\�����Ȃ�
        if (myID == 0) active = false;

        if (avatarMesh.enabled)
        {
            // �����
            invisible = false;

            if(!startAnim)
            {
                startAnim = true;
                // �o���̃A�j���[�V���� ----------
                audio.bossSource.Stop();
                audio.Boss4_DummySpawnSound();
                anim.Play("Spawn");
                Debug.Log("�������`");

            }//----- if_stop -----
        }
        else
        {
            // �s�����
            invisible = true;

            if (startAnim)
            {
                startAnim = false;
                // �ޏ�̃A�j���[�V���� ----------
                audio.bossSource.Stop();
                audio.Boss4_DummyDeleteSound();
                ef_Manager.StartRemoveInvisible();
                Debug.Log("�������傤�`");

            }//----- if_stop -----
        }
    }

    //==================================================
    //          ���g�����֐�
    // ���U�����󂯂����A��_�����A�N�V������
    // �@�������A�|���ꂽ��Ԃ��o�͂��܂�
    //==================================================
    // �����2023/05/30
    // �{��
    private IEnumerator RemoveAvatar()
    {
        // �����l���i�[����
        sbyte speed = ascendSpeed;
        anim.Play("Damage");
        
        while (true)
        {
            // �㏸���x���Ȃ��Ȃ����珈���𔲂���
            if(avatartRb.velocity.y < 0) break;

            // ���x��t�^����
            avatartRb.velocity = new Vector3(avatartRb.velocity.x, speed, 0);
            speed--;

            // �P�t���[���x��������
            yield return null;

        }//----- while_stop -----

        audio.bossSource.Stop();
        audio.Boss4_DummyDeleteSound();
        // ���x������
        avatartRb.velocity = Vector3.zero;
        

        // ���g����������
        avatarMesh.enabled = false;
        active = false;
    }
}