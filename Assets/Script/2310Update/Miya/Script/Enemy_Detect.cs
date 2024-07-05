using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Detect : MonoBehaviour
{
    private demo_Enemy_A enemyA;

    [SerializeField]
    private float searchDistance;   // ���m����

    // ��ԗp�ϐ�
    private bool isDetect;          // ���m���

    // �Q�b�^�[
    public bool GetDetect() { return isDetect; }    // ���m���

    private void Start()
    {
        enemyA = GetComponent<demo_Enemy_A>();
    }

    private void Update()
    {
        // ���g�̈ʒu���W���擾����
        Vector3 myPos = this.transform.position;

        // �v���C���[�̈ʒu���W���擾����
        Vector3 plPos = GameObject.Find("player").transform.position;

        // �v���C���[�Ǝ��g�̋������擾����
        float dist = Vector3.Distance(plPos, transform.position);

        // �v���C���[�܂ł̋��������m�������Ȃ�
        if (dist < searchDistance)
        {
            // �v���C���[�̈ʒu�������̍��Ȃ�
            if(myPos.x > plPos.x)
            {
                // �����̌��������ɂ���
                enemyA.SetDirection(demo_Enemy_A.DIRECTION.LEFT);
            }
            else if(myPos.x < plPos.x)
            {
                // �����̌������E�ɂ���
                enemyA.SetDirection(demo_Enemy_A.DIRECTION.RIGHT);
            }

            // ���m��Ԃɂ���
            isDetect = true;

        }//----- if_stop -----
        else
        {
            // �񌟒m��Ԃɂ���
            isDetect = false;

        }//----- else_stop -----
    }

}
