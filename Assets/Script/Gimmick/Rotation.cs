using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//=================================================
// ��]���̃X�N���v�g�B���S�_��

public class Rotation : MonoBehaviour
{
    public float angularVelocity = 1;

    private List<GameObject> l_childrenObj = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        // ��x���X�g���N���A
        l_childrenObj.Clear();

        // �q�I�u�W�F��S�Ċi�[
        for (byte i = 0; i < transform.childCount; i++)
        {
            l_childrenObj.Add(transform.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // ���̃I�u�W�F����]������B
        transform.eulerAngles += new Vector3(0,0,angularVelocity);
        
        // �q�I�u�W�F���t�����ɉ�]������B
        for(byte i = 0; i < l_childrenObj.Count; i++)
        {
            l_childrenObj[i].transform.eulerAngles -= new Vector3(0,0,angularVelocity);
        }
    }

    // �Q�[���I�����Ƀ��X�g��j��
    private void OnApplicationQuit()
    {
        l_childrenObj.Clear();
    }
}
