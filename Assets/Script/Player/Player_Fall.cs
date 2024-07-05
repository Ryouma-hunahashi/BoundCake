using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//==================================================
// ��������
//==================================================
// �쐬��2023/04/29    �X�V��2023/05/01
// �{��
public class Player_Fall : MonoBehaviour
{
    // �e(�v���C���[)�����擾
    private Rigidbody parentRb;
    private Player_Main parentScript;

    // �e�̎q�ɂ���X�N���v�g���擾
    private Player_Jump pl_Jump;

    // �������x�̐ݒ� ----------
    public sbyte maxFallPow = 30;
    // ���݂̗������x
    public sbyte fallPowLog;

    private void Start()
    {
        // �e�̏����擾
        parentRb = transform.parent.GetComponent<Rigidbody>();
        parentScript = transform.parent.GetComponent<Player_Main>();

        // �e�̎q�ɂ�������擾
        pl_Jump = transform.parent.GetComponentInChildren<Player_Jump>();
    }

    //==================================================
    //          �W�����v�㗎�����s
    // ���ړ����x�����鍀�ڂ�����̂ŃR���[�`���ɂȂ��Ă܂�
    // �����������̃A�j���[�V�����͂����ɏ���
    // �߂�l : �Ȃ�
    //  ����  : �Ȃ�
    //==================================================
    // �쐬��2023/04/30    �X�V��2023/05/04
    // �{��
    public IEnumerator FallTheAfterJump()
    {
        sbyte fallPow = 0;

        

        // �e�I�u�W�F�N�g���ڒn���Ă��Ȃ��ԌJ��Ԃ�
        while(!parentScript.onRayGround)
        {
            //Debug.Log("������");

            if(parentScript.underRayGrab)
            {
                fallPowLog = fallPow;
                // ���[�v�𔲂���
                yield break;

            }//----- if_stop -----

            // ���������x�̏���l�܂ő��x���㏸������
            if(fallPow > -maxFallPow)
            {
                fallPow--;

            }//----- if_stop -----

            // �P�t���[�����炷
            yield return null;

            fallPowLog = fallPow;

            // �����̑��x���X�V����
            parentRb.velocity = new Vector3(parentRb.velocity.x, fallPow, parentRb.velocity.z);

        }//----- while_stop -----
    }
}
