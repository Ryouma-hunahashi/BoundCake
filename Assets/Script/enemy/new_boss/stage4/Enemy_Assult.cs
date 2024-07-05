using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
//          �G���u�̒��i�U��
//==================================================
// �쐬��2023/05/17    �X�V��2023/05/19
// �{��
public class Enemy_Assult : MonoBehaviour
{
    // �e�̏����擾
    private Rigidbody par_Rb;
    private EnemyEffectManager ef_Manager;

    // �W�I��ݒ�
    public GameObject target;
    private bool setTarget;     // �W�I�̗L��

    // �ǂ��j�󂷂邩
    [SerializeField] private bool hitDestroy;       // �����ɓ��������Ƃ�
    [SerializeField] private bool finishDestroy;    // �t���[���I����

    [SerializeField] private bool startAction;

    // ���i�̐ݒ�
    [Header("----- ���i�̐ݒ� -----")]
    [SerializeField] private float speed;           // ���x
    [SerializeField] private ushort distanceFrame;  // ����(�w��t���[����)
    [SerializeField] private bool speedup;          // �����̗L��
    [SerializeField] private float upperLimit;      // �����̏��
    private float addSpeed = 0;                     // �����x

    //private void OnTriggerEnter(Collider other)
    //{
    //    // �����ɐG�ꂽ���ɔj�󂷂�Ȃ�
    //    if (hitDestroy)
    //    {
    //        // ���g��j�󂷂�
    //        Destroy(this.transform.parent.gameObject);

    //    }//----- if_stop -----
    //}

    private void Start()
    {
        // �e�̏����擾
        target = GameObject.Find("player");
        par_Rb = transform.parent.GetComponent<Rigidbody>();
        ef_Manager = GetComponentInChildren<EnemyEffectManager>();
        // �W�I���ݒ肳��Ă���Ȃ�
        if(target != null)
        {
            // �W�I�w���Ԃɂ���
            setTarget = true;

        }//----- if_stop -----
        else
        {
            // �W�I�����Ԃɂ���
            setTarget = false;

        }//----- else_stop -----
    }

    private void Update()
    {
        if(startAction)
        {
            StartCoroutine(Assult());
            startAction = false;

        }//----- if_stop -----
    }

    private IEnumerator Assult()
    {
        // �W�I�����܂��Ă���Ȃ������ݒ肷��
        if (setTarget)
        {
            transform.parent.LookAt(target.transform.position);
            ef_Manager.StartBullet(target.transform.position,transform.position);
        }


        for(int i = 0; i < distanceFrame; i++)
        {
            // �P�t���[���x��������
            yield return null;

            // ��������Ȃ�
            if(speedup)
            {
                // �w��̏���l�܂ŉ���
                if (addSpeed < upperLimit)
                {
                    addSpeed++;

                }//----- if_stop -----

            }//----- if_stop -----

            // �ړ����x�̍X�V
            par_Rb.velocity = new Vector3(transform.parent.forward.x * (speed + addSpeed), transform.parent.forward.y * (speed + addSpeed), 0);

        }//----- for_stop -----

        // ���x�𖳂���
        par_Rb.velocity = Vector3.zero;

        if(finishDestroy)
        {
            // ���g��j�󂷂�
            Destroy(this.transform.parent.gameObject);

        }//----- if_stop -----
    }
}