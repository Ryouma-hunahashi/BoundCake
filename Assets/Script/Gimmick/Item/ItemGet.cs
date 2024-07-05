using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGet : MonoBehaviour
{
    // �v���C���[�ɂ���p
    private void OnTriggerEnter(Collider other)
    {
        // �R�C���^�O�ɐG�ꂽ��
        if (other.gameObject.tag == "Coin")
        {// -----if start -----
            // ItemDeta����̃I�u�W�F�N�g���󂯎��
            ItemDeta item = other.gameObject.GetComponent<ItemDeta>();
            if (item.getFg == true)
            {
                // ItemDeta����̃I�u�W�F�N�g���󂯎��

                // �X�R�A�����Z����
                StatusManager.coinCount++;
                StatusManager.gameScore += item.value;

                //�A�C�e��(�R�C��)���폜����
                other.gameObject.SetActive(false);
            }

        }// -----if stop -----
    }
}
